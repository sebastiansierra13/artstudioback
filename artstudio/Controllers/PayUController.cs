using artstudio.DTOs;
using artstudio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using artstudio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Hosting;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Globalization;
using System.Text;

namespace artstudio.Controllers
{
    [ApiController]
    [Route("api/payu")]
    public class PayUController : ControllerBase
    {
        private readonly IPayUService _payUService;
        private readonly MiDbContext _context;
        private readonly ILogger<PayUController> _logger;

        public PayUController(IPayUService payUService, MiDbContext context, ILogger<PayUController> logger)
        {
            _payUService = payUService;
            _context = context;
            _logger = logger;
        }

        // Crear el formulario de pago
        [HttpPost("createPaymentForm")]
        public IActionResult CreatePaymentForm([FromBody] PayUFormDTO payUFormDTO)
        {
            if (payUFormDTO == null || payUFormDTO.Productos == null || !payUFormDTO.Productos.Any())
                return BadRequest("Datos inv�lidos.");

            try
            {
                var paymentForm = _payUService.GeneratePaymentForm(payUFormDTO);

                // Retornar el formulario de pago, incluyendo el ReferenceCode generado
                return Ok(paymentForm);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el formulario de pago.");
                return StatusCode(500, "Ocurri� un error al procesar la solicitud.");
            }
        }




        [HttpPost("confirmation")]
        public IActionResult HandlePayUConfirmation([FromForm] PayUConfirmationDTO confirmation)
        {
            try
            {
                // Validar la firma recibida
                bool isSignatureValid = _payUService.ValidateSignature(confirmation);
                if (!isSignatureValid)
                {
                    return BadRequest("Firma inv�lida.");
                }

                // Buscar la orden en la base de datos utilizando la referencia de venta
                var order = _context.Orders.Include(o => o.Orderproducts)
                                           .SingleOrDefault(o => o.ReferenceCode == confirmation.Reference_sale);

                if (order == null)
                {
                    return NotFound("No se encontr� la orden.");
                }

                // Actualizar el estado de la transacci�n en la base de datos
                order.OrderStatus = GetTransactionStateMessage(confirmation.State_pol);
                order.UpdatedAt = DateTime.Now;
                _context.Orders.Update(order);
                _context.SaveChanges();

                // Si la transacci�n fue aprobada, enviar correo al vendedor
                if (confirmation.State_pol == "4") // Estado 4 = Aprobado
                {
                    SendOrderConfirmationEmail(order);
                }

                return Ok("Confirmaci�n recibida exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la confirmaci�n del pago.");
                return StatusCode(500, "Error en el procesamiento de la confirmaci�n.");
            }
        }



        [HttpPost("createOrder")]
        public IActionResult CreateOrder([FromBody] PayUFormDTO payUFormDTO)
        {
            try
            {
                // Utilizamos el ReferenceCode ya generado por el servicio de PayU
                string referenceCode = payUFormDTO.ReferenceCode;

                // Crear un nuevo pedido (Order)
                var newOrder = new Order
                {
                    BuyerEmail = payUFormDTO.BuyerEmail,
                    TotalAmount = payUFormDTO.Total,
                    Currency = "COP", // Moneda predeterminada desde el backend
                    OrderStatus = "Pending",  // Estado inicial del pedido
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ReferenceCode = referenceCode // Usar el c�digo de referencia que viene del servicio PayU
                };

                _context.Orders.Add(newOrder);
                _context.SaveChanges(); // Guardar el pedido

                // Guardar los productos asociados al pedido
                foreach (var producto in payUFormDTO.Productos)
                {
                    var newOrderProduct = new Orderproduct
                    {
                        OrderId = newOrder.OrderId,
                        ProductId = producto.IdProducto,
                        ProductName = producto.NombreProducto,
                        TamanhoPoster = producto.TamanhoPoster,
                        PrecioPoster = producto.PrecioPoster,
                        PrecioMarco = producto.PrecioMarco,
                        Cantidad = producto.Cantidad, // Guardar la cantidad
                        Subtotal = (producto.PrecioPoster + producto.PrecioMarco) * producto.Cantidad, // Calcular el subtotal con la cantidad
                        ProductImageUrl = GetProductImageUrl((int)producto.IdProducto) // Obtener la URL de la primera imagen
                    };

                    _context.Orderproducts.Add(newOrderProduct);
                }

                _context.SaveChanges(); // Guardar los productos

                // Enviar respuesta de �xito
                return Ok(new { orderId = newOrder.OrderId, referenceCode = newOrder.ReferenceCode });
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la orden.");
                return StatusCode(500, "Error al crear la orden.");
            }
        }



        // M�todo para obtener la primera imagen de un producto desde la lista de im�genes
        private string GetProductImageUrl(int productId)
        {
            var product = _context.Productos.FirstOrDefault(p => p.IdProducto == productId);
            if (product != null && !string.IsNullOrEmpty(product.Imagenes))
            {
                // Las im�genes est�n guardadas como una lista separada por comas, tomamos la primera
                var imageUrls = product.Imagenes.Split(',');
                return imageUrls.Length > 0 ? imageUrls[0] : string.Empty;
            }
            return string.Empty; // Si no hay imagen
        }


        [HttpGet("response")]
        public IActionResult HandlePayUResponse([FromQuery] PayUResponseDTO response)
        {
            try
            {
                // Validar la firma recibida
                bool isSignatureValid = _payUService.ValidateSignature(response);

                if (!isSignatureValid)
                {
                    return BadRequest("Firma inv�lida.");
                }

                // Obtener el entorno actual desde las variables de entorno
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

               
               
                // Codificar todos los par�metros que podr�an tener caracteres especiales
                string estado = Uri.EscapeDataString(GetTransactionStateMessage(response.TransactionState));
                string referencia = Uri.EscapeDataString(response.ReferenceCode);
                string valor = Uri.EscapeDataString(response.TX_VALUE.ToString(CultureInfo.InvariantCulture));
                string moneda = Uri.EscapeDataString(response.Currency);
                string fecha = Uri.EscapeDataString(response.ProcessingDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                string metodoPago = Uri.EscapeDataString(response.LapPaymentMethod);
                string descripcion = Uri.EscapeDataString(response.Description);
                string authorizationCode = Uri.EscapeDataString(response.AuthorizationCode);

                // Configurar la URL del frontend seg�n el entorno
                string frontendUrl;
                if (environment == "Production")
                {
                    frontendUrl = $"https://artstudio.com.co/order-summary?estado={estado}&referencia={referencia}&valor={valor}&moneda={moneda}&fecha={fecha}&metodoPago={metodoPago}&descripcion={descripcion}&authorizationCode={authorizationCode}";
                }
                else // Desarrollo
                {
                    frontendUrl = $"http://localhost:4200/order-summary?estado={estado}&referencia={referencia}&valor={valor}&moneda={moneda}&fecha={fecha}&metodoPago={metodoPago}&descripcion={descripcion}&authorizationCode={authorizationCode}";
                }

                // Redirigir al frontend con los datos de la transacci�n
                return Redirect(frontendUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la respuesta del pago.");
                return StatusCode(500, "Error en el procesamiento.");
            }
        }


        [HttpGet("order-details")]
        public IActionResult GetOrderDetails(string referenceCode)
        {
            var order = _context.Orders
                                .Include(o => o.Orderproducts)
                                .SingleOrDefault(o => o.ReferenceCode == referenceCode);

            if (order == null)
            {
                return NotFound("No se encontr� la orden.");
            }

            // Preparar los productos de la orden para devolverlos en la respuesta
            var orderDetails = new
            {
                order.ReferenceCode,
                order.TotalAmount,
                order.Currency,
                order.OrderStatus,
                order.CreatedAt,
                order.UpdatedAt,
                Products = order.Orderproducts.Select(op => new
                {
                    op.ProductName,
                    op.TamanhoPoster,
                    op.PrecioPoster,
                    op.PrecioMarco,
                    op.Cantidad,
                    op.Subtotal,
                    ProductImageUrl = GetFirstProductImage(op.ProductId) // Obtener la primera imagen del producto
                    
                }).ToList()


            };


            // Devolver los detalles completos del pedido (incluyendo los productos)
            return Ok(orderDetails);
        }

        // M�todo auxiliar para obtener la primera imagen de un producto
        private string GetFirstProductImage(long? productId)
        {
            if (productId == null) return string.Empty;

            var product = _context.Productos.FirstOrDefault(p => p.IdProducto == productId);

            if (product != null && !string.IsNullOrEmpty(product.Imagenes))
            {
                // Las im�genes est�n guardadas como un array de URLs en formato JSON, extraemos la primera
                var imageUrls = product.Imagenes.Trim('[', ']').Split(',');
                if (imageUrls.Length > 0)
                {
                    return imageUrls[0].Trim('"'); // Limpiar las comillas y devolver la primera URL
                    
                }
                
            }

            return string.Empty; // Si no hay imagen, devolver un string vac�o
        }





        private async Task SendOrderConfirmationEmail(Order order)
        {
            try
            {
                // Configuraci�n del correo
                var fromAddress = new MailAddress("pedidosartstudio@gmail.com", "ArtStudio Store");
                var toAddress = new MailAddress("artstudiomg2024@gmail.com"); // Correo de la tienda o destinatario
                string fromPassword = "fwjzycoliosyjgux";

                // Aqu� corregimos 'const' a 'string' porque 'order.ReferenceCode' es una variable
                string subject = $"Nuevo Pedido Aprobado: {order.ReferenceCode}";

                // Crear el cuerpo del correo
                var body = new StringBuilder();
                body.AppendLine("<h2>Detalles del Pedido</h2>");
                body.AppendLine($"<p><strong>Referencia:</strong> {order.ReferenceCode}</p>");
                body.AppendLine($"<p><strong>Total:</strong> {order.TotalAmount} {order.Currency}</p>");
                body.AppendLine($"<p><strong>Estado:</strong> Aprobado</p>");
                body.AppendLine("<h3>Productos:</h3><ul>");

                foreach (var product in order.Orderproducts)
                {
                    body.AppendLine("<li>");
                    body.AppendLine($"<p><strong>Producto:</strong> {product.ProductName} (Tama�o: {product.TamanhoPoster})</p>");
                    body.AppendLine($"<p><strong>Precio:</strong> P�ster - ${product.PrecioPoster}, Marco - ${product.PrecioMarco}</p>");
                    body.AppendLine($"<p><strong>Cantidad:</strong> {product.Cantidad}</p>");
                    body.AppendLine($"<p><strong>Subtotal:</strong> ${product.Subtotal}</p>");
                    body.AppendLine("</li>");
                }
                body.AppendLine("</ul>");
                body.AppendLine($"<p><strong>Total del Pedido:</strong> {order.TotalAmount} {order.Currency}</p>");

                // Configuraci�n del SMTP
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // O usa el servidor SMTP de tu elecci�n
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                // Crear el mensaje de correo
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject, // Aqu� ya no es constante
                    Body = body.ToString(),
                    IsBodyHtml = true // El cuerpo del correo es en formato HTML
                };

                // Enviar el correo
                smtp.Send(message);

                _logger.LogInformation("Correo de confirmaci�n enviado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo de confirmaci�n.");
            }
        }



        // M�todo auxiliar para obtener un mensaje claro del estado de la transacci�n
        private string GetTransactionStateMessage(string transactionState)
        {
            switch (transactionState)
            {
                case "4": return "Transacci�n aprobada";
                case "6": return "Transacci�n rechazada";
                case "7": return "Pago pendiente";
                default: return "Estado desconocido";
            }
        }


        // M�todo para validar productos, medidas y precios
        private async Task<bool> ValidateProductsAndPrices(List<ProductoDTO> productos)
        {
            foreach (var producto in productos)
            {
                // Buscar las medidas y precios en la tabla Precio
                var precioEnDb = await _context.Precios.SingleOrDefaultAsync(p => p.TamanhoPoster == producto.TamanhoPoster);

                if (precioEnDb == null)
                {
                    _logger.LogWarning("Tama�o inv�lido para el producto: {ProductoNombre}", producto.NombreProducto);
                    return false;
                }

                // Verificar que los precios coinciden con los de la base de datos
                if (producto.PrecioPoster != precioEnDb.PrecioPoster || producto.PrecioMarco != precioEnDb.PrecioMarco)
                {
                    _logger.LogWarning("Diferencia en el precio para el producto: {ProductoNombre}", producto.NombreProducto);
                    return false; // El precio fue manipulado en el frontend
                }
            }

            return true; // Todos los productos son v�lidos
        }

        // M�todo para recalcular el total en el backend
        private decimal RecalculateTotal(List<ProductoDTO> productos)
        {
            decimal total = 0;

            foreach (var producto in productos)
            {
                // Buscar el precio en la base de datos
                var precioEnDb = _context.Precios.SingleOrDefault(p => p.TamanhoPoster == producto.TamanhoPoster);
                if (precioEnDb != null)
                {
                    // Sumar los precios del poster y del marco
                    total += precioEnDb.PrecioPoster + precioEnDb.PrecioMarco;
                }
            }

            return total;
        }
    }
}

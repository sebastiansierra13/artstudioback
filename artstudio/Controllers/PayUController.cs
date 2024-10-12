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
                return BadRequest("Datos inválidos.");

            try
            {
                var paymentForm = _payUService.GeneratePaymentForm(payUFormDTO);

                // Retornar el formulario de pago, incluyendo el ReferenceCode generado
                return Ok(paymentForm);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el formulario de pago.");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
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
                    return BadRequest("Firma inválida.");
                }

                // Buscar la orden en la base de datos utilizando la referencia de venta
                var order = _context.Orders.Include(o => o.Orderproducts)
                                           .SingleOrDefault(o => o.ReferenceCode == confirmation.Reference_sale);

                if (order == null)
                {
                    return NotFound("No se encontró la orden.");
                }


                // Actualizar el estado de la transacción en la base de datos
                order.OrderStatus = GetTransactionStateMessage(confirmation.State_pol);
                order.UpdatedAt = DateTime.Now;
                _context.Orders.Update(order);
                _context.SaveChanges();

                // Si la transacción fue aprobada, enviar correo al vendedor
                if (confirmation.State_pol == "4") // Estado 4 = Aprobado
                {
                    SendOrderConfirmationEmail(order); // Enviar el PayUFormDTO junto con la orden
                }

                return Ok("Confirmación recibida exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la confirmación del pago.");
                return StatusCode(500, "Error en el procesamiento de la confirmación.");
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
                    BuyerFullName = $"{payUFormDTO.FirstName} {payUFormDTO.LastName}", // Nombre completo del comprador
                    MobilePhone = payUFormDTO.Phone, // Teléfono del comprador
                    StreetName = payUFormDTO.StreetName, // Dirección del comprador
                    Apartment = payUFormDTO.Apartment, // Detalles adicionales como apartamento
                    Neighborhood = payUFormDTO.Neighborhood, // Barrio del comprador
                    City = payUFormDTO.City, // Ciudad del comprador
                    Department = payUFormDTO.Department, // Departamento del comprador
                    Postcode = payUFormDTO.Postcode, // Código postal (opcional)
                    Extra1 = payUFormDTO.Extra1, // Extra1 del formulario
                    Extra2 = payUFormDTO.Extra2, // Extra2 del formulario
                    TotalAmount = payUFormDTO.Total,
                    Currency = "COP", // Moneda predeterminada desde el backend
                    OrderStatus = "Pending",  // Estado inicial del pedido
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ReferenceCode = referenceCode // Usar el código de referencia que viene del servicio PayU
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

                // Enviar respuesta de éxito
                return Ok(new { orderId = newOrder.OrderId, referenceCode = newOrder.ReferenceCode });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la orden.");
                return StatusCode(500, "Error al crear la orden.");
            }
        }




        // Método para obtener la primera imagen de un producto desde la lista de imágenes
        private string GetProductImageUrl(int productId)
        {
            var product = _context.Productos.FirstOrDefault(p => p.IdProducto == productId);
            if (product != null && !string.IsNullOrEmpty(product.Imagenes))
            {
                // Las imágenes están guardadas como una lista separada por comas, tomamos la primera
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
                    return BadRequest("Firma inválida.");
                }

                // Interpretar correctamente los estados de la transacción
                string estadoTransaccion = GetTransactionStateMessage(response.TransactionState);

                // Codificar los parámetros
                string estado = Uri.EscapeDataString(estadoTransaccion);
                string referencia = Uri.EscapeDataString(response.ReferenceCode);
                string valor = Uri.EscapeDataString(response.TX_VALUE.ToString(CultureInfo.InvariantCulture));
                string moneda = Uri.EscapeDataString(response.Currency);
                string fecha = Uri.EscapeDataString(response.ProcessingDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                string metodoPago = Uri.EscapeDataString(response.LapPaymentMethod);
                string descripcion = Uri.EscapeDataString(response.Description);
                string authorizationCode = Uri.EscapeDataString(response.AuthorizationCode);

                // Configurar la URL del frontend según el entorno
                string frontendUrl;
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                {
                    frontendUrl = $"https://artstudio.com.co/order-summary?estado={estado}&referencia={referencia}&valor={valor}&moneda={moneda}&fecha={fecha}&metodoPago={metodoPago}&descripcion={descripcion}&authorizationCode={authorizationCode}";
                }
                else // Desarrollo
                {
                    frontendUrl = $"http://localhost:4200/order-summary?estado={estado}&referencia={referencia}&valor={valor}&moneda={moneda}&fecha={fecha}&metodoPago={metodoPago}&descripcion={descripcion}&authorizationCode={authorizationCode}";
                }

                // Redirigir al frontend con los datos de la transacción
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
                return NotFound("No se encontró la orden.");
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

        // Método auxiliar para obtener la primera imagen de un producto
        private string GetFirstProductImage(long? productId)
        {
            if (productId == null) return string.Empty;

            var product = _context.Productos.FirstOrDefault(p => p.IdProducto == productId);

            if (product != null && !string.IsNullOrEmpty(product.Imagenes))
            {
                // Las imágenes están guardadas como un array de URLs en formato JSON, extraemos la primera
                var imageUrls = product.Imagenes.Trim('[', ']').Split(',');
                if (imageUrls.Length > 0)
                {
                    return imageUrls[0].Trim('"'); // Limpiar las comillas y devolver la primera URL
                    
                }
                
            }

            return string.Empty; // Si no hay imagen, devolver un string vacío
        }





        private async Task SendOrderConfirmationEmail(Order order)
        {
            try
            {
                // Configuración del correo
                var fromAddress = new MailAddress("pedidosartstudio@gmail.com", "ArtStudio Store");
                var toAddress = new MailAddress("artstudiomg2024@gmail.com"); // Correo de la tienda o destinatario
                string fromPassword = "jboylagpzsgkmreg ";
                string subject = $"Nuevo Pedido Aprobado: {order.ReferenceCode}";

                // Crear el cuerpo del correo con datos ordenados en una tabla y configurado en UTF-8
                var body = new StringBuilder();

                // Estilos para el correo
                body.AppendLine("<html><head><meta charset='UTF-8'><style>");
                body.AppendLine("body { font-family: 'Arial', sans-serif; background-color: #f4f4f4; color: #333; }");
                body.AppendLine("h2 { color: #007bff; }");
                body.AppendLine(".container { width: 100%; max-width: 800px; margin: 0 auto; padding: 20px; background-color: #fff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1); }");
                body.AppendLine(".header { text-align: center; padding-bottom: 20px; }");
                body.AppendLine("table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
                body.AppendLine("th, td { padding: 12px 15px; border: 1px solid #ddd; text-align: left; }");
                body.AppendLine("th { background-color: #007bff; color: #fff; }");
                body.AppendLine(".total { font-weight: bold; background-color: #f8f9fa; }");
                body.AppendLine(".footer { text-align: center; font-size: 14px; color: #888; }");
                body.AppendLine(".watermark-container { position: relative; z-index: 1; background-image: url('https://firebasestorage.googleapis.com/v0/b/fireartstudio-8586e.appspot.com/o/images%2FFondoCorreo%2F6a171ca33cfb9f1d1150072c2ce982c5.jpg?alt=media&token=69b00229-f5b0-43df-adf4-52c792c3ee71'); background-repeat: no-repeat; background-position: center; background-size: 500px 500px; opacity: 0.1; text-align: center; padding: 20px; }");
                body.AppendLine(".content { position: relative; z-index: 2; }");
                body.AppendLine("</style></head><body>");

                // Contenedor principal
                body.AppendLine("<div class='container'>");

                // Encabezado
                body.AppendLine("<div class='header'><h2>Recibiste un nuevo pedido en ArtStudio!</h2></div>");

                // Información del cliente
                body.AppendLine("<h3>Detalles del Cliente</h3>");
                body.AppendLine("<table>");
                body.AppendLine("<tr><th>Nombre Completo</th><td>" + order.BuyerFullName + "</td></tr>");
                body.AppendLine("<tr><th>Correo Electrónico</th><td>" + order.BuyerEmail + "</td></tr>");
                body.AppendLine("<tr><th>Teléfono</th><td>" + order.MobilePhone + "</td></tr>");
                body.AppendLine("<tr><th>Dirección</th><td>" + order.StreetName + ", " + order.City + ", " + order.Department + "</td></tr>");
                body.AppendLine("<tr><th>Código Postal</th><td>" + (order.Postcode ?? "No proporcionado") + "</td></tr>");
                body.AppendLine("<tr><th>Notas del Pedido</th><td>" + (order.Extra1 ?? "Ninguna") + "</td></tr>");
                body.AppendLine("</table>");

                // Detalles del Pedido
                body.AppendLine("<h3>Detalles del Pedido</h3>");
                body.AppendLine("<table>");
                body.AppendLine("<tr><th>Referencia del Pedido</th><td>" + order.ReferenceCode + "</td></tr>");
                body.AppendLine("<tr><th>Total</th><td>" + order.TotalAmount.ToString("C") + "</td></tr>");
                body.AppendLine("<tr><th>Estado del Pedido</th><td>" + order.OrderStatus + "</td></tr>");
                body.AppendLine("</table>");

                // Productos
                body.AppendLine("<h3>Productos</h3>");
                body.AppendLine("<table>");
                body.AppendLine("<tr><th>Producto</th><th>Tamaño</th><th>Precio</th><th>Cantidad</th><th>Subtotal</th></tr>");

                foreach (var product in order.Orderproducts)
                {
                    body.AppendLine("<tr>");
                    body.AppendLine("<td>" + product.ProductName + "</td>");
                    body.AppendLine("<td>" + product.TamanhoPoster + "</td>");
                    body.AppendLine("<td>$" + product.PrecioPoster + " (Marco: $" + product.PrecioMarco + ")</td>");
                    body.AppendLine("<td>" + product.Cantidad + "</td>");
                    body.AppendLine("<td>$" + product.Subtotal + "</td>");
                    body.AppendLine("</tr>");
                }

                body.AppendLine("<tr class='total'><td colspan='4'>Total</td><td>$" + order.TotalAmount + "</td></tr>");
                body.AppendLine("</table>");

                // Cierre del contenedor
                body.AppendLine("</div>");

                // Pie de página
                body.AppendLine("<div class='footer'>ArtStudio - Todos los derechos reservados</div>");

                // Cierre de HTML
                body.AppendLine("</body></html>");

                // Configuración del SMTP
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // O usa el servidor SMTP de tu elección
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                // Crear el mensaje de correo
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body.ToString(),
                    IsBodyHtml = true // El cuerpo del correo es en formato HTML
                };

                // Enviar el correo
                await smtp.SendMailAsync(message);

                _logger.LogInformation("Correo de confirmación enviado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo de confirmación.");
            }
        }







        // Método auxiliar para obtener un mensaje claro del estado de la transacción
        private string GetTransactionStateMessage(string transactionState)
        {
            return transactionState switch
            {
                "4" => "aprobada",  // Aprobada
                "6" => "declinada", // Declinado
                "7" => "pendiente", // Pendiente
                _ => "desconocida"  // Cualquier otro estado
            };
        }



        // Método para validar productos, medidas y precios
        private async Task<bool> ValidateProductsAndPrices(List<ProductoDTO> productos)
        {
            foreach (var producto in productos)
            {
                // Buscar las medidas y precios en la tabla Precio
                var precioEnDb = await _context.Precios.SingleOrDefaultAsync(p => p.TamanhoPoster == producto.TamanhoPoster);

                if (precioEnDb == null)
                {
                    _logger.LogWarning("Tamaño inválido para el producto: {ProductoNombre}", producto.NombreProducto);
                    return false;
                }

                // Verificar que los precios coinciden con los de la base de datos
                if (producto.PrecioPoster != precioEnDb.PrecioPoster || producto.PrecioMarco != precioEnDb.PrecioMarco)
                {
                    _logger.LogWarning("Diferencia en el precio para el producto: {ProductoNombre}", producto.NombreProducto);
                    return false; // El precio fue manipulado en el frontend
                }
            }

            return true; // Todos los productos son válidos
        }

        // Método para recalcular el total en el backend
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

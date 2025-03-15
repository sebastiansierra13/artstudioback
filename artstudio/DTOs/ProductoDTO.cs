namespace artstudio.DTOs
{
    public class ProductoDTO
    {
        public long IdProducto { get; set; }
        public string NombreProducto { get; set; } = null!;
        public string TamanhoPoster { get; set; } = null!;   
        public decimal PrecioPoster { get; set; }
        public decimal PrecioMarco { get; set; }
        public int Cantidad { get; set; } // Cantidad del producto agregado al carrito
    }


}
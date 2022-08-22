namespace ProyectoFinal.Controllers.DTOS
{
    public class PostVenta
    {
        public int Id { get; set; }
        public string Comentarios { get; set; }
        public int IdUsuario { get; set; }
        public int IdProducto { get; set; }
        public int Stock { get; set; }
        public string Status { get; set; }
        public string DetalleVenta { get; set; }
    }
}

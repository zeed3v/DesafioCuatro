using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Controllers.DTOS;
using ProyectoFinal.Model;
using ProyectoFinal.Repository;

namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VentaController : ControllerBase
    {
        [HttpGet(Name = "GetVentas")]
        public List<Venta> GetVentas()
        {
            return VentaHandler.GetVentas();
        }

        
        [HttpPost(Name = "CargarVentas")]
        public List<PostVenta> CargarVentas()
        {
            return VentaHandler.CargarVentas();
        }
        
    }
}

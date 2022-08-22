using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TraerNombreController
    {
        [HttpGet]
        public string TraerNombre()
        {
            return "Proyecto Final Coder";
        }
    }
        
}

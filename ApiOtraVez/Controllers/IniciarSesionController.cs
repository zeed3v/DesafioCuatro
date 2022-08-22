using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Model;
using ProyectoFinal.Repository;

namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IniciarSesionController : ControllerBase
    {
        [HttpGet("{nombreUsuario}/{contraseña}")]
        public bool IniciarSesion(string nombreUsuario, string contraseña)
        {
            Usuario usuario = UsuarioHandler.IniciarSesion(nombreUsuario, contraseña);
            if (usuario.Nombre == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

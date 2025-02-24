using Microsoft.AspNetCore.Mvc;
using ServidorApiRestaurante.Model;

namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("cliente")] // Ruta: dirección/cliente/   https://localhost:7233/
    public class ClienteController : ControllerBase
    {
        [HttpGet]
        [Route("listar")]
        public dynamic listarCliente()
        {
            List<Cliente> clientes = new List<Cliente>()
            {
                new Cliente
                {
                    id = "1",
                    correo = "google@gmail.com",
                    edad = "19",
                    nombre = "Bernardo Peña"

                },
                new Cliente
                {
                    id = "1",
                    correo = "miguelgoogle@gmail.com",
                    edad = "23",
                    nombre = "Miguel Mantilla"
                }
            };
            return clientes;
            
        }

        [HttpGet]
        [Route("listarxid")]
        public dynamic listarClientexid(string _id)
        {
            return new Cliente
            {
                id = _id,
                correo = "miguelgoogle@gmail.com",
                edad = "23",
                nombre = "Miguel Mantilla"
            };
        }

        [HttpPost]
        [Route("guardar")]
        public dynamic guardarCliente(Cliente cliente)
        {
            cliente.id = "3";
            return new
            {
                success = true,
                message = "cliente registrado",
                result = cliente
            };
        }

        [HttpPost]
        [Route("eliminar")]
        public dynamic eliminarCliente(Cliente cliente)
        {
            return new
            {
                success = true,
                message = "cliente eliminado",
                result = cliente
            };
        }
    }
}

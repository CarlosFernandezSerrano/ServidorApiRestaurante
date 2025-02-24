using Microsoft.AspNetCore.Mvc;
using ServidorApiRestaurante.Model;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("cliente")] // Ruta: dirección/cliente/   https://localhost:7233/
    public class ClienteController : ControllerBase
    {
        /*[HttpGet]
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
            
        }*/

        /*[HttpGet]
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
        }*/

        [HttpPost]
        [Route("guardar")]
        public dynamic guardarCliente(Cliente cliente)
        {
            Cliente cliente1 = new Cliente("2", "Carlos", "23", "Carlos@gmail.com");
            return JsonSerializer.Serialize(cliente1);
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

        [HttpDelete]
        [Route("borrarxid/{id}")]
        public dynamic borrarClientexid(string id)
        {
            if (id.CompareTo("1") == 0)
            {
                return new
                {
                    result = "Cliente con id " + id + " eliminado."
                };
            }
            else
            {
                return new
                {
                    result = "Cliente con id " + id + " no eliminado."
                };
            }
                
        }

        /*[HttpPut]
        [Route("actualizarClientexid/{id}")]
        public dynamic actualizarClientexid(string id, [FromBody] string clienteid)
        {
            
            if (id.CompareTo("1") == 0)
            {
                return new
                {
                    result = "Cliente con id " + id + " actualizado."
                };
            }
            else
            {
                return new
                {
                    result = "Cliente con id " + id + " no actualizado."
                };
            }
        }*/

        /*[HttpPatch]
        [Route("actualizarClientexidValorEspecífico/{id}")]
        public dynamic actualizarClientexidValorEspecífico(string id)
        {
            if (id.CompareTo("1") == 0)
            {
                return new
                {
                    result = "Cliente con id " + id + " actualizado valor específico."
                };
            }
            else
            {
                return new
                {
                    result = "Cliente con id " + id + " no actualizado valor específico."
                };
            }
        }*/
    }
}

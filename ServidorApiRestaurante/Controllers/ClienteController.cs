using Microsoft.AspNetCore.Mvc;
using ServidorApiRestaurante.Model;

using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("cliente")] // Ruta: dirección/cliente/   https://localhost:7233/
    public class ClienteController : ControllerBase
    {
        [HttpGet]
        [Route("existe/{nombre}")]
        public dynamic existeCliente(string nombre)
        {
            List<Cliente> clientes = new List<Cliente>();
            List<string> restaurantesEjemplo = new List<string>();
            restaurantesEjemplo.Add("Restaurante1");
            Cliente c = new Cliente(nombre, "gsdgsgsag", "empleado", "0", restaurantesEjemplo);
            restaurantesEjemplo.Add("Restaurante2");
            Cliente c2 = new Cliente("Ramos", "sgdagsdgs", "empleado", "1",restaurantesEjemplo);
            clientes.Add(c);
            clientes.Add(c2);
            /*List<Cliente> clientes = new List<Cliente>()
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
            };*/
            return clientes;

        }
        [HttpGet]
        [Route("listar")]
        public dynamic listarCliente()
        {
            

            List<Cliente> clientes = new List<Cliente>();
            List<string> restaurantesEjemplo = new List<string>();
            restaurantesEjemplo.Add("Restaurante1");
            restaurantesEjemplo.Add("Restaurante2");
            Cliente c = new Cliente("Fernando", "gsdgsgsag", "empleado", "0", restaurantesEjemplo);
            List<string> restaurantesEjemplo2 = new List<string>();
            restaurantesEjemplo2.Add("Restaurante3");
            restaurantesEjemplo2.Add("Restaurante4");
            restaurantesEjemplo2.Add("Restaurante5");
            Cliente c2 = new Cliente("Ramos", "sgdagsdgs", "empleado", "1", restaurantesEjemplo2);
            clientes.Add(c);
            clientes.Add(c2);
            /*List<Cliente> clientes = new List<Cliente>()
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
            };*/
            return clientes;
            
        }

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
            List<string> restaurantesEjemplo = new List<string>();
            restaurantesEjemplo.Add("Restaurante1");
            Cliente c = new Cliente("Fernando", "gsdgsgsag", "empleado", "0", restaurantesEjemplo);
            return JsonSerializer.Serialize(c);
            /*cliente.id = "3";
            return new
            {
                success = true,
                message = "cliente registrado",
                result = cliente
            };*/
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

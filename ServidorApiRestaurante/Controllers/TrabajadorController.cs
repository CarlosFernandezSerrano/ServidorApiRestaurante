using Microsoft.AspNetCore.Mvc;
using ServidorApiRestaurante.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServidorApiRestaurante.Controllers
{        
    [ApiController]
    [Route("cliente")] // Ruta: dirección/cliente/   https://localhost:7233/
    public class TrabajadorController : ControllerBase
    {
        //SQLiteController sQLiteController;

        // La API se conecta a la BDD
        [HttpGet]
        [Route("cbdd")]
        public dynamic conectarConLaBDD()
        {
            // Obtén la ruta absoluta de la carpeta 'base_datos' dentro del proyecto
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "BDD"); // Ruta en la carpeta del ejecutable

            // La ruta completa al archivo de la base de datos
            string rutaBDDFichero = Path.Combine(folderPath, "miBaseDeDatos.db");

            Trace.WriteLine("DataBasePath: " + rutaBDDFichero); //Mostrar contenido en salida
            
            //sQLiteController = new SQLiteController(rutaBDDFichero);
            Trace.WriteLine("A: ");
            //sQLiteController.CreateDatabase();
            Trace.WriteLine("B: ");
            return new { result = true };

        }


        [HttpGet]
        [Route("existe/{nombre}")]
        public dynamic existeCliente(string nombre)
        {
            /*List<Trabajador> clientes = new List<Trabajador>();
            Trabajador c = new Trabajador(nombre, "gsdgsgsag", 1, 1);
            Trabajador c2 = new Trabajador(nombre, "gsdgsgsag", 1, 1);
            clientes.Add(c);
            clientes.Add(c2);*/
            return new { sucess = true };
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
            //return clientes;

        }

        [HttpGet]
        [Route("listar")]
        public dynamic listarCliente()
        {
            /*List<Trabajador> clientes = new List<Trabajador>();
            Trabajador c = new Trabajador(0, "Fernando", "gsdgsgsag", 1, 1);
            Trabajador c2 = new Trabajador(0, "Ramos", "gsdgsgsag", 1, 1);
            clientes.Add(c);
            clientes.Add(c2);*/
            return new { sucess = true };
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
            //return clientes;
            
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
        public dynamic guardarCliente(Trabajador cliente)
        {
            /*List<string> restaurantesEjemplo = new List<string>();
            Trabajador c = new Trabajador(0, "Fernando", "gsdgsgsag", 1, 1);
            return JsonSerializer.Serialize(c);*/
            return new { sucess = true };
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
        public dynamic eliminarCliente(Trabajador cliente)
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

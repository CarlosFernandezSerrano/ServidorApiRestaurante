using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServidorApiRestaurante.Controllers
{        
    [ApiController]
    [Route("trabajador")] // Ruta: dirección/cliente/   https://localhost:7233/
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
        [Route("registrarUser/{nombreYPassword}")]
        public dynamic RegistroTrabajador(string nombreYPassword)
        {
            string[] cads = nombreYPassword.Split("*");
            Trace.WriteLine("Cads 1: " + cads[0]);
            Trace.WriteLine("Cads 2: " + cads[1]);
            // Veo si existe el trabajador
            //bool b = ExisteTrabajador(cads[0]);

            InsertarRegistro(BDDController.ConnectionString, cads[0], HashearContraseña(cads[1]));
            Trace.WriteLine("Trabajador resgistrado correctamente");
            return new { sucess = true };

        }

        [HttpGet]
        [Route("logIn/{nombreYPassword}")]
        public dynamic LogInTrabajador(string nombreYPassword)
        {
            string[] cads = nombreYPassword.Split("*");
            Trace.WriteLine("Cads 1: " + cads[0]);
            Trace.WriteLine("Cads 2: " + cads[1]);
            // Veo si existe el trabajador
            //bool b = ExisteTrabajador(cads[0]);
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

        //Método que comprueba si el nombre puesto en el logIn ya está puesto en la tabla de trabajadores
        private bool ExisteTrabajador(string nombre)
        {
            throw new NotImplementedException();
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
        public dynamic guardarCliente(Trabajador trabajador)
        {
            //Trabajador c = new Trabajador(0, "Fernando", "gsdgsgsag", 1, 1);
            //return JsonSerializer.Serialize(c);
            //return new { sucess = true };
            //trabajador.Id = 3;
            /*return 
            
                success = true,
                message = "trabajador registrado",
                trabajador
            ;*/
            return trabajador;
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

        public static string HashearContraseña(string contraseña)
        {
            return BCrypt.Net.BCrypt.HashPassword(contraseña);
        }

        //Devuelve true si la contraseña es correcta.
        /*public bool VerificarContraseña(string contraseñaIngresada)
        {
            return BCrypt.Net.BCrypt.Verify(contraseñaIngresada, this.password);
        }*/


        private static void InsertarRegistro(string connectionString, string nombre, string password)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Trabajadores'
            string insertQuery = "INSERT INTO Trabajadores (nombre, password, rol_id) VALUES (@nombre, @password, @rol_id)";

            // Usamos 'using' para asegurar que la conexión se cierre correctamente
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    // Abrimos la conexión con la base de datos
                    connection.Open();

                    // Creamos el comando para ejecutar la consulta SQL
                    using (var cmd = new MySqlCommand(insertQuery, connection))
                    {
                        // Asignamos los parámetros con sus respectivos valores
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@rol_id", 1);

                        // Ejecutamos la consulta. ExecuteNonQuery devuelve el número de filas afectadas
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        Trace.WriteLine("Registro insertado correctamente. Filas afectadas: " + filasAfectadas);
                    }
                }
                catch (MySqlException ex)
                {
                    // Capturamos errores relacionados con MySQL
                    Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    // Capturamos errores de operación inválida en la conexión
                    Trace.WriteLine("Error de operación inválida: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Capturamos cualquier otro error inesperado
                    Trace.WriteLine("Error inesperado: " + ex.Message);
                }
            }
        }
    }
}

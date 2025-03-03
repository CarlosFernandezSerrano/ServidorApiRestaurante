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
                
        [HttpPost]
        [Route("obtenerID")]
        public dynamic GetIDTrabajador(Trabajador t)
        {
            int id  = ObtenerIdTrabajador(t.Nombre);
            Trace.WriteLine("ID Trabajador: " + id);
            return new { result = id  };
        }

        [HttpPost]
        [Route("registrarUser")]
        public dynamic RegistroTrabajador(Trabajador trabajador)
        {
            Trace.WriteLine("trabajador.Nombre: " + trabajador.Nombre);
            Trace.WriteLine("trabajador.Password " + trabajador.Password);
            
            // Compruebo si existe el trabajador antes de intentar insertarlo, para que no se creen IDs vacíos.
            if (ExisteTrabajador(trabajador.Nombre)){
                return new { result = 2 };
            }
            else
            {
                int num = InsertarRegistro(BDDController.ConnectionString, trabajador.Nombre, HashearContraseña(trabajador.Password));
                return new { result = num };
            }
        }

        [HttpPost]
        [Route("logIn")]
        public dynamic LogInTrabajador(Trabajador trabajador)
        {
            // Compruebo si existe el trabajador. Aquí no puedo buscar por id, porque no lo tengo cuando inicio sesión.
            if (ExisteTrabajador(trabajador.Nombre))
            {
                
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }

           

            //return new { sucess = true };
        }

        

        [HttpGet]
        [Route("existe/{nombre}")]
        public dynamic existeCliente(string nombre)
        {
        
            return new { sucess = true };
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

        // Devuelve true si la contraseña es correcta.
        /*public bool VerificarContraseña(string contraseñaIngresada, string hashPassword) 
        {
            return BCrypt.Net.BCrypt.Verify(contraseñaIngresada, hashPassword);
        }*/

        //Método que comprueba si un trabajador (por nombre) ya está registrado
        private static bool ExisteTrabajador(string nombre)
        {
            string query = "SELECT COUNT(*) FROM Trabajadores WHERE nombre = @nombre";

            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);

                        int count = Convert.ToInt32(cmd.ExecuteScalar()); // Obtiene el número de coincidencias
                        return count > 0; // Si es mayor a 0, el trabajador existe
                    }
                }
                catch (MySqlException ex)
                {
                    Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);
                    throw new Exception("Error al verificar la existencia del trabajador: " + ex.Message);
                }
            }
        }

        private static int InsertarRegistro(string connectionString, string nombre, string password)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Trabajadores'
            string insertQuery = "INSERT INTO Trabajadores (nombre, password, rol_id, restaurante_ID) VALUES (@nombre, @password, @rol_id, @restaurante_ID)";

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
                        cmd.Parameters.AddWithValue("@restaurante_ID", DBNull.Value);

                        // Ejecutamos la consulta. ExecuteNonQuery devuelve el número de filas afectadas
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        Trace.WriteLine("Trabajador insertado correctamente. Filas afectadas: " + filasAfectadas);
                        return 1;
                    }
                }
                catch (MySqlException ex)
                {
                    // Capturamos errores relacionados con MySQL
                    Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);
                    return 2;
                    
                }
                catch (InvalidOperationException ex)
                {
                    // Capturamos errores de operación inválida en la conexión
                    Trace.WriteLine("Error de operación inválida: " + ex.Message);
                    return -3;
                }
                catch (Exception ex)
                {
                    // Capturamos cualquier otro error inesperado
                    Trace.WriteLine("Error inesperado: " + ex.Message);
                    return 0;
                }
            }
        }

        private static int ObtenerIdTrabajador(string nombre)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ID FROM Trabajadores WHERE nombre = @nombre";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        object result = cmd.ExecuteScalar(); // Ejecuta la consulta y devuelve la primera columna de la primera fila

                        if (result != null)
                        {
                            return Convert.ToInt32(result); // Devuelve el ID encontrado
                        }
                        else
                        {
                            return 0; // No se encontró el trabajador
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener ID del trabajador: " + ex.Message);
                    return -1; 
                }
            }
        }

        // La API se conecta a la BDD. Método que utilicé con SQLite. Ya no lo uso, se puede eliminar.
        /*[HttpGet]
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

        }*/
    }
}

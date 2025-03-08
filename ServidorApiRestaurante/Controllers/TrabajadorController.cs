using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServidorApiRestaurante.Controllers
{        
    [ApiController]
    [Route("trabajador")] // Ruta: dirección/trabajador/   https://localhost:7233/
    public class TrabajadorController : ControllerBase
    {
                
        [HttpPost]
        [Route("obtenerTrabajadorPorNombre")]
        public dynamic ObtenerTrabajadorConNombre(Trabajador t)
        {
            //int id  = ObtenerIdTrabajador(t.Nombre);
            Trabajador trabajador = ObtenerTrabajadorPorNombre(t.Nombre);
            Trace.WriteLine("ID Trabajador: " + trabajador.Id);
            trabajador.Nombre = "";
            return trabajador;
            //return new { result = id  };
        }

        [HttpGet]
        [Route("obtenerTrabajadorPorId/{id}")]
        public dynamic GetRol_IDTrabajador(int id)
        {
            if (ExisteTrabajadorConID(id))
            {
                Trabajador t = ObtenerTrabajadorPorId(id);
                t.Id = 0; t.Nombre = "";
                return t;
            }
            else
            {
                return new { result = 0 };
            }
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
                string hashPassword = ObtenerPasswordTrabajador(trabajador.Nombre);
                Trace.WriteLine("trabajador.Password " + hashPassword);
                bool passwordCorrecta = VerificarContraseña(trabajador.Password, hashPassword);
                if (passwordCorrecta)
                {
                    Trace.WriteLine("Contraseña correcta");
                    return new { result = 1 };
                }
                else
                {
                    Trace.WriteLine("Contraseña incorrecta");
                    return new { result = 0 };
                }
            }
            else
            {
                return new { result = -1 };
            }
        }


        [HttpGet]
        [Route("existe/{id}")]
        public dynamic ExisteTrabajador(int id) 
        {
            if (ExisteTrabajadorConID(id))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }
        }

        

        [HttpGet]
        [Route("listar")]
        public dynamic listarCliente()
        {
            return new { sucess = 232 };            
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

        [HttpPut]
        [Route("actualizarTrabajador")]
        public dynamic ActualizarTrabajadorXid(Trabajador t)
        {
            int i = ActualizarTrabajadorPorId(t);
            
            // La actualización fue un éxito y se lo comunico al cliente
            if (i.Equals(1)){
                return new
                {
                    result = true
                };
            }
            else
            {
                return new
                {
                    result = false
                };
            }
                
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
        public static bool VerificarContraseña(string contraseñaIngresada, string hashPassword) 
        {
            return BCrypt.Net.BCrypt.Verify(contraseñaIngresada, hashPassword);
        }

        //Método que comprueba (por nombre) si un trabajador ya está registrado
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

        //Método que comprueba (por id) si un trabajador está registrado
        private static bool ExisteTrabajadorConID(int id)
        {
            string query = "SELECT COUNT(*) FROM Trabajadores WHERE ID = @id";

            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

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

        private static Trabajador ObtenerTrabajadorPorNombre(string nombre)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Trabajadores WHERE nombre = @nombre";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("ID");
                                string Nombre = reader.GetString("Nombre");
                                int Rol_Id = reader.GetInt32("Rol_ID");
                                int Restaurante_Id = reader.IsDBNull("Restaurante_ID") ? 0 : reader.GetInt32("Restaurante_ID");
                                Trabajador trabajador = new Trabajador(Id, Nombre, "", Rol_Id, Restaurante_Id);
                                
                                return trabajador;
                            }
                            else
                            {
                                throw new Exception("Error al obtener trabajador");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener trabajador: " + ex.Message);
                    throw new Exception("Error al obtener trabajador: " + ex.Message);
                }
            }
        }

        private static Trabajador ObtenerTrabajadorPorId(int id)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Trabajadores WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("ID");
                                string Nombre = reader.GetString("Nombre");
                                int Rol_Id = reader.GetInt32("Rol_ID");
                                int Restaurante_Id = reader.IsDBNull("Restaurante_ID") ? 0 : reader.GetInt32("Restaurante_ID");
                                Trabajador trabajador = new Trabajador(Id, Nombre, "", Rol_Id, Restaurante_Id);

                                return trabajador;
                            }
                            else
                            {
                                throw new Exception("Error al obtener trabajador");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener trabajador: " + ex.Message);
                    throw new Exception("Error al obtener trabajador: " + ex.Message);
                }
            }
        }

        private static string ObtenerPasswordTrabajador(string nombre)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Password FROM Trabajadores WHERE nombre = @nombre";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        object result = cmd.ExecuteScalar(); // Ejecuta la consulta y devuelve la primera columna de la primera fila

                        return result is string str ? str : string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener Password del trabajador: " + ex.Message);
                    throw new Exception("Error al obtener Password del trabajador: " + ex.Message);
                }
            }
        }

        private static int ActualizarTrabajadorPorId(Trabajador t)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Trabajadores SET Nombre = @nombre, Rol_ID = @rol_id, Restaurante_ID = @restaurante_id WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@nombre", t.Nombre);
                        cmd.Parameters.AddWithValue("@rol_id", t.Rol_Id);
                        cmd.Parameters.AddWithValue("@restaurante_id", t.Restaurante_Id);
                        cmd.Parameters.AddWithValue("@id", t.Id);

                        // Ejecuta la sentencia y retorna el número de filas afectadas
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // La actualización fue exitosa
                            Console.WriteLine("Registro actualizado correctamente.");
                            return 1;
                        }
                        else
                        {
                            // No se encontró ningún registro con ese ID
                            Console.WriteLine("No se actualizó ningún registro.");
                            return 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al actualizar trabajador: " + ex.Message);
                    throw new Exception("Error al actualizar trabajador: " + ex.Message);
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

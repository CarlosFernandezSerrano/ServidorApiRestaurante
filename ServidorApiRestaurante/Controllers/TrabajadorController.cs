using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServidorApiRestaurante.Controllers
{        
    [ApiController]
    [Route("trabajador")] // Ruta: dirección/trabajador/   https://localhost:7233/
    public class TrabajadorController : ControllerBase
    {

        [Authorize]
        [ValidarTokenFilterController]
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

        [Authorize]
        [ValidarTokenFilterController]
        [HttpGet]
        [Route("obtenerTrabajadorPorId/{id}")]
        public dynamic GetRol_IDTrabajador(int id)
        {
            if (ExisteTrabajadorConID(id))
            {
                Trabajador t = ObtenerTrabajadorPorId(id);
                t.Id = 0;
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
                
                if (!num.Equals(1))
                {
                    return new { result = num };
                    
                }
                else
                {
                    Trabajador t = ObtenerTrabajadorPorNombre(trabajador.Nombre);
                    return CreaciónDeTokenParaCliente(t);
                }                    
            }
        }

        public IConfiguration _configuration;

        public TrabajadorController(IConfiguration configuration)
        {
            _configuration = configuration;
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

                    Trabajador t = ObtenerTrabajadorPorNombre(trabajador.Nombre);
                    return CreaciónDeTokenParaCliente(t);

                    //return new { result = 1 };
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

        private dynamic CreaciónDeTokenParaCliente(Trabajador trabajador)
        {
            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", trabajador.Id.ToString()), // qué es mejor 
                new Claim("id", trabajador.Nombre)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                signingCredentials: singIn
                );

            return new
            {
                result = 1,
                message = "exito",
                token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        [Authorize]
        [ValidarTokenFilterController]
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

        [Authorize]
        [ValidarTokenFilterController]
        [HttpPut]
        [Route("actualizarTrabajador")]
        public dynamic ActualizarTrabajadorXid(Trabajador t)
        {
            Trace.WriteLine("Llega a actualizar trabajador");
            int i = ActualizarTrabajadorPorId(t);
            
            // La actualización fue un éxito y se lo comunico al cliente
            if (i.Equals(1))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }
        }

        [Authorize]
        [ValidarTokenFilterController]
        [SoloAdminsFilterController]
        [HttpPut]
        [Route("actualizarTrabajadorPorGerente")]
        public dynamic ActualizarTrabajadorPorGerenteXid(Trabajador trabajador)
        {
            Trace.WriteLine("Llega a actualizar trabajadores");

            // Si se quiere poner un nombre distinto a un trabajador, pero ya existe otro trabajador con ese nombre: result = 0
            if (ExisteTrabajador(trabajador.Nombre))
            {
                Trabajador t = ObtenerTrabajadorPorNombre(trabajador.Nombre);
                
                if (!trabajador.Id.Equals(t.Id))
                {
                    return new { result = 0 };
                }
            }

            int i = ActualizarTrabajadorPorId(trabajador);

            // La actualización fue un éxito y se lo comunico al cliente
            if (i.Equals(1))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }
        }

        [Authorize]
        [ValidarTokenFilterController]
        [SoloAdminsFilterController]
        [HttpDelete]
        [Route("eliminarxid/{id}")]
        public dynamic EliminarTrabajadorxid(string id)
        {
            Trace.WriteLine("Llega a eliminar trabajador x ID");
            int num = EliminarTrabajadorConID(id);

            if (num.Equals(1))
            {
                Trace.WriteLine("Trabajador con id " + id + " eliminado.");
                return new { result = 1 };
            }
            else
            {
                Trace.WriteLine("Trabajador con id " + id + " no eliminado.");
                return new { result = 0 };
            }
        }


        [Authorize]
        [ValidarTokenFilterController]
        [SoloAdminsFilterController]
        [HttpGet]
        [Route("getTrabajadoresDeRestaurante/{id_Restaurante}")]
        public dynamic ObtenerTrabajadoresxidRest(string id_Restaurante)
        {
            Trace.WriteLine("Pasa por obtener trabajadores de un restaurante");

            List<Trabajador> trabajadores = ObtenerTrabajadoresDeRestaurante(id_Restaurante);

            return trabajadores;
        }

        [Authorize]
        [ValidarTokenFilterController]
        [SoloAdminsFilterController]
        [HttpGet]
        [Route("getTrabajadoresSinRestaurante")]
        public dynamic ObtenerTrabajadoresSinRest()
        {
            Trace.WriteLine("Pasa por obtener trabajadores sin un restaurante");

            List<Trabajador> trabajadores = ObtenerTrabajadoresSinRestaurante();

            return trabajadores;
        }

        [Authorize]
        [ValidarTokenFilterController]
        [SoloAdminsFilterController]
        [HttpPut]
        [Route("actualizarRestauranteIDTrabajador")]
        public dynamic ActualizarRestauranteIDTrabajadorXid(Trabajador t)
        {
            Trace.WriteLine("Llega a actualizar Restaurante_ID del trabajador");
            int i = ActualizarRestauranteIDTrabajadorPorId(t);

            // La actualización fue un éxito y se lo comunico al cliente
            if (i.Equals(1))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }
        }

        private static List<Trabajador> ObtenerTrabajadoresSinRestaurante()
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Trabajadores WHERE Rol_ID = @rol_ID AND Restaurante_ID IS NULL";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@rol_ID", 1);

                        using (var reader = cmd.ExecuteReader())
                        {
                            List<Trabajador> trabajadores = new List<Trabajador>();

                            while (reader.Read())
                            {
                                int ID = reader.GetInt32("ID");
                                string nombre = reader.GetString("Nombre");
                                int rol_ID = reader.GetInt32("Rol_ID");

                                trabajadores.Add(new Trabajador(ID, nombre, "", rol_ID, 0));
                            }

                            return trabajadores;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener trabajadores: " + ex.Message);
                    throw new Exception("Error al obtener trabajadores: " + ex.Message);
                }
            }
        }

        private static List<Trabajador> ObtenerTrabajadoresDeRestaurante(string id_Restaurante)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Trabajadores WHERE Restaurante_ID = @restauranteId";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@restauranteId", id_Restaurante);
                        using (var reader = cmd.ExecuteReader())
                        {
                            List<Trabajador> trabajadores = new List<Trabajador>();

                            while (reader.Read())
                            {
                                int ID = reader.GetInt32("ID");
                                string nombre = reader.GetString("Nombre");
                                int rol_ID = reader.GetInt32("Rol_ID");

                                trabajadores.Add(new Trabajador(ID, nombre, "", rol_ID, 0 ));
                            }

                            return trabajadores;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener trabajadores: " + ex.Message);
                    throw new Exception("Error al obtener trabajadores: " + ex.Message);
                }
            }
        }

        private static int EliminarTrabajadorConID(string trabajador_id)
        {
            string query = "DELETE FROM Trabajadores WHERE ID = @id";

            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", trabajador_id);

                        int filasAfectadas = cmd.ExecuteNonQuery(); // Devuelve el número de filas eliminadas

                        return filasAfectadas > 0 ? 1 : 0;
                    }
                }
                catch (MySqlException ex)
                {
                    Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);
                    throw new Exception("Error al verificar la existencia del trabajador: " + ex.Message);
                }
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
        public static bool ExisteTrabajadorConID(int id)
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
                    Trace.WriteLine("Error al obtener trabajador: "+ nombre + ex.Message);
                    throw new Exception("Error al obtener trabajador: " + ex.Message);
                }
            }
        }

        public static Trabajador ObtenerTrabajadorPorId(int id)
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

        private static int ActualizarRestauranteIDTrabajadorPorId(Trabajador t)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Trabajadores SET Restaurante_ID = @restaurante_id WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
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

    }
}

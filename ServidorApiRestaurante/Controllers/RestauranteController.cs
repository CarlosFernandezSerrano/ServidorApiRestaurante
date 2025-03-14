using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;
using ServidorApiRestaurante.Models;
using System.Data;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("restaurante")] // Ruta: dirección/restaurante/   https://localhost:7233/
    public class RestauranteController : ControllerBase
    {
        [HttpPost]
        [Route("registrarRestaurante")]
        public dynamic RegistroRestaurante(Restaurante restaurante)
        {
            Trace.WriteLine("restaurante.Nombre: " + restaurante.Nombre);
            Trace.WriteLine("restaurante.HoraApertura " + restaurante.HoraApertura);
            Trace.WriteLine("restaurante.HoraCierre " + restaurante.HoraCierre);

            // Compruebo si existe el trabajador antes de intentar insertarlo, para que no se creen IDs vacíos.
            if (ExisteRestauranteConNombre(restaurante.Nombre))
            {
                return new { result = 2 };
            }
            else
            {
                int num = InsertarRegistro(BDDController.ConnectionString, restaurante.Nombre, restaurante.HoraApertura, restaurante.HoraCierre);
                return new { result = num };
            }
        }

        [HttpGet]
        [Route("existe/{id}")]
        public dynamic ExisteRestauranteConUnID(int id)
        {
            if (ExisteRestauranteConID(id))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }
        }

        [HttpGet]
        [Route("existeConNombre/{nombre}")]
        public dynamic ExisteRestauranteConUnNombre(string nombre)
        {
            if (ExisteRestauranteConNombre(nombre))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }
        }

        [HttpGet]
        [Route("getRestaurantePorNombre/{nombre}")]
        public dynamic ObtenerRestauranteConNombre(string nombre)
        {
            Restaurante restaurante = ObtenerRestaurantePorNombre(nombre);
            return restaurante;
        }

        [HttpGet]
        [Route("getRestaurantePorId/{id}")]
        public dynamic ObtenerRestauranteConId(int id)
        {
            Restaurante restaurante = ObtenerRestaurantePorId(id);
            List<Mesa> mesas = ObtenerTodasLasMesasDeUnRestaurante(id);
            List<Trabajador> trabajadores = ObtenerTodosLosTrabajadoresDeUnRestaurante(id);
            restaurante.Mesas = mesas;
            restaurante.Trabajadores = trabajadores;
            return restaurante;
        }

        [HttpPut]
        [Route("actualizarRestaurante")]
        public dynamic ActualizarRestauranteXid(Restaurante r)
        {
            Trace.WriteLine("Llega a actualizar restaurante");
            int i = ActualizarRestaurantePorId(r);

            int j = 0;
            int contErrores = 0;
            foreach (var mesa in r.Mesas)
            {
                j = MesaController.ActualizarMesa(mesa);

                if (j.Equals(0))
                {
                    contErrores++;
                }
            }

            // La actualización fue un éxito y se lo comunico al cliente
            if (contErrores.Equals(0) && i.Equals(1))
            {
                return new
                {
                    result = 1
                };
            }
            else
            {
                return new
                {
                    result = 0
                };
            }
        }

        [HttpPost]
        [Route("registrarMesas")]
        public dynamic RegistroMesas(Restaurante restaurante)
        {
            Trace.WriteLine("restaurante.Nombre: " + restaurante.Nombre);
            Trace.WriteLine("restaurante.HoraApertura " + restaurante.HoraApertura);
            Trace.WriteLine("restaurante.HoraCierre " + restaurante.HoraCierre);

            int i = 0;
            int contErrores = 0;
            foreach (var mesa in restaurante.Mesas)
            {
                i = MesaController.RegistrarMesa(mesa);

                if (i.Equals(0))
                {
                    contErrores++;
                }
            }

            if (contErrores > 0)
            {
                return new { result = 0 };
            }
            else
            {
                return new { result = 1 };
            }
        }

        private static bool ExisteRestauranteConID(int id)
        {
            string query = "SELECT COUNT(*) FROM Restaurantes WHERE ID = @id";

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

        //Método que comprueba (por nombre) si un restaurante ya está registrado
        private static bool ExisteRestauranteConNombre(string nombre)
        {
            string query = "SELECT COUNT(*) FROM Restaurantes WHERE Nombre = @nombre";

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

        private static int ActualizarRestaurantePorId(Restaurante r)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Restaurantes SET Nombre = @nombre, Hora_Apertura = @horaApertura, Hora_Cierre = @horaCierre WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@nombre", r.Nombre);
                        cmd.Parameters.AddWithValue("@horaApertura", r.HoraApertura);
                        cmd.Parameters.AddWithValue("@horaCierre", r.HoraCierre);
                        cmd.Parameters.AddWithValue("@id", r.Id);

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
                    Trace.WriteLine("Error al actualizar restaurante: " + ex.Message);
                    throw new Exception("Error al actualizar trabajador: " + ex.Message);
                }
            }
        }

        private static int InsertarRegistro(string connectionString, string nombre, string horaApertura, string horaCierre)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Restaurantes'
            string insertQuery = "INSERT INTO Restaurantes (nombre, hora_apertura, hora_cierre) VALUES (@nombre, @hora_apertura, @hora_cierre)";

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
                        cmd.Parameters.AddWithValue("@hora_apertura", horaApertura);
                        cmd.Parameters.AddWithValue("@hora_cierre", horaCierre);

                        // Ejecutamos la consulta. ExecuteNonQuery devuelve el número de filas afectadas
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        Trace.WriteLine("Restaurante insertado correctamente. Filas afectadas: " + filasAfectadas);
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

        private static Restaurante ObtenerRestaurantePorNombre(string nombre)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Restaurantes WHERE nombre = @nombre";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("ID");
                                string Nombre = reader.GetString("Nombre");
                                string Hora_Apertura = reader.GetString("Hora_Apertura");
                                string Hora_Cierre = reader.GetString("Hora_Cierre");
                                Restaurante restaurante = new Restaurante(Id, Nombre, Hora_Apertura, Hora_Cierre, new List<Mesa>(), new List<Trabajador>());

                                return restaurante;
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

        private static Restaurante ObtenerRestaurantePorId(int id)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Restaurantes WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("ID");
                                string Nombre = reader.GetString("Nombre");
                                string Hora_Apertura = reader.GetString("Hora_Apertura");
                                string Hora_Cierre = reader.GetString("Hora_Cierre");
                                Restaurante restaurante = new Restaurante(Id, Nombre, Hora_Apertura, Hora_Cierre, new List<Mesa>(), new List<Trabajador>());

                                return restaurante;
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

        private static List<Mesa> ObtenerTodasLasMesasDeUnRestaurante(int restaurante_Id)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Mesas WHERE Restaurante_ID = @restauranteId";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@restauranteId", restaurante_Id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            List<Mesa> mesas = new List<Mesa>();

                            while (reader.Read())
                            {
                                int id = reader.GetInt32("ID");
                                float posX = reader.GetFloat("PosX");
                                float posY = reader.GetFloat("PosY");
                                float width = reader.GetFloat("Width");
                                float height = reader.GetFloat("Height");
                                float scaleX = reader.GetFloat("ScaleX");
                                float scaleY = reader.GetFloat("ScaleY");
                                bool disponible = reader.GetBoolean("Disponible");

                                mesas.Add(new Mesa(id, posX, posY, width, height, scaleX, scaleY, disponible, restaurante_Id));
                            }

                            return mesas;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener mesas: " + ex.Message);
                    throw new Exception("Error al obtener mesas: " + ex.Message);
                }
            }
        }

        private static List<Trabajador> ObtenerTodosLosTrabajadoresDeUnRestaurante(int restaurante_Id)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Trabajadores WHERE Restaurante_ID = @restauranteId";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@restauranteId", restaurante_Id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            List<Trabajador> trabajadores = new List<Trabajador>();

                            while (reader.Read())
                            {
                                int id = reader.GetInt32("ID");
                                string nombre = reader.GetString("Nombre");
                                int rol_ID = reader.GetInt32("Rol_ID");
                                int restaurante_ID = reader.GetInt32("Restaurante_ID");

                                trabajadores.Add(new Trabajador(id, nombre, "", rol_ID, restaurante_ID));
                            }

                            return trabajadores;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener mesas: " + ex.Message);
                    throw new Exception("Error al obtener mesas: " + ex.Message);
                }
            }
        }
    }
}

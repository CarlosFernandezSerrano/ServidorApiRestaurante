using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Diagnostics;

namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("restaurante")] // Ruta: dirección/restaurante/   https://localhost:7233/
    public class RestauranteController : ControllerBase
    {
        [HttpPost]
        [Route("registrarRestaurante")]
        public dynamic RegistroTrabajador(Restaurante restaurante)
        {
            Trace.WriteLine("restaurante.Nombre: " + restaurante.Nombre);
            Trace.WriteLine("restaurante.HoraApertura " + restaurante.HoraApertura);
            Trace.WriteLine("restaurante.HoraCierre " + restaurante.HoraCierre);

            // Compruebo si existe el trabajador antes de intentar insertarlo, para que no se creen IDs vacíos.
            if (ExisteRestaurante(restaurante.Nombre))
            {
                return new { result = 2 };
            }
            else
            {
                int num = InsertarRegistro(BDDController.ConnectionString, restaurante.Nombre, restaurante.HoraApertura, restaurante.HoraCierre);
                return new { result = num };
            }
        }

        //Método que comprueba (por nombre) si un trabajador ya está registrado
        private static bool ExisteRestaurante(string nombre)
        {
            string query = "SELECT COUNT(*) FROM Restaurantes WHERE nombre = @nombre";

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

        private static int InsertarRegistro(string connectionString, string nombre, string horaApertura, string horaCierre)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Trabajadores'
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
    }
}

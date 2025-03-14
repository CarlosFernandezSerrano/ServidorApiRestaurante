using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("reserva")] // Ruta: dirección/reserva/   https://localhost:7233/
    public class ReservasController
    {

        [HttpGet]
        [Route("existe/{id_Mesa}")]
        public dynamic ExisteReservaConUnaMesa_ID(int id_Mesa)
        {
            if (ExisteReservaConMesa_ID(id_Mesa))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }
        }

        private static bool ExisteReservaConMesa_ID(int id_Mesa)
        {
            string query = "SELECT COUNT(*) FROM Reservas WHERE Mesa_ID = @id";

            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id_Mesa);

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
    }
}

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpPost]
        [Route("crearReserva")]
        public dynamic CrearReservaConFecha(Reserva reserva)
        {
            if (!ExisteReservaEnMesa_ID_EnFechaYHora(reserva))
            {
                Trace.WriteLine("Pasa por método crearReserva");
                int num = InsertarRegistro(reserva);
                return new { result = num };
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

        private static bool ExisteReservaEnMesa_ID_EnFechaYHora(Reserva reserva)
        {
            return false; // Mejorar en el futuro
        }

        private static int InsertarRegistro(Reserva reserva)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Trabajadores'
            string insertQuery = "INSERT INTO Reservas (Fecha, Hora, Estado, Cliente_ID, Mesa_ID) VALUES (@fecha, @hora, @estado, @cliente_id, @mesa_ID)";

            // Usamos 'using' para asegurar que la conexión se cierre correctamente
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    // Abrimos la conexión con la base de datos
                    connection.Open();

                    // Creamos el comando para ejecutar la consulta SQL
                    using (var cmd = new MySqlCommand(insertQuery, connection))
                    {
                        // Asignamos los parámetros con sus respectivos valores
                        cmd.Parameters.AddWithValue("@fecha", reserva.Fecha);
                        cmd.Parameters.AddWithValue("@hora", reserva.Hora);
                        cmd.Parameters.AddWithValue("@estado", ""+EstadoReserva.Confirmada);
                        cmd.Parameters.AddWithValue("@cliente_id", null);
                        cmd.Parameters.AddWithValue("@mesa_ID", reserva.Mesa_Id);

                        // Ejecutamos la consulta. ExecuteNonQuery devuelve el número de filas afectadas
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        Trace.WriteLine("Reserva insertada correctamente. Filas afectadas: " + filasAfectadas);
                        MesaController.PonerMesaDisponibleONoDisponible(reserva.Mesa_Id, false); // False = no disponible, ya que hago una reserva para el momento y ocupo al instante la mesa
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

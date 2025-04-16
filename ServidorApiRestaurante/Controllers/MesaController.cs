using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Diagnostics;

namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("mesa")] // Ruta: dirección/mesa/   https://localhost:7233/
    public class MesaController : ControllerBase
    {

        [Authorize]
        [ValidarTokenFilterController]
        [HttpDelete]
        [Route("borrarxid/{id}")]
        public dynamic BorrarMesaxID(string id)
        {
            Trace.WriteLine("Llega a borrar mesa x ID");
            int num = EliminarMesaConID(id);

            if (num.Equals(1))
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
        [Route("actualizarCampoDisponible")]
        public dynamic ActualizarCampoDisponibleMesa(Mesa mesa)
        {
            Trace.WriteLine("Llega a actualizar campo disponible de mesa");
            
            int num = ActualizarCampoDisponibleDeMesa(mesa.Id, mesa.Disponible);

            if (num.Equals(1))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }
        }
        

        private static int EliminarMesaConID(string mesa_ID)
        {
            string query = "DELETE FROM Mesas WHERE ID = @id";

            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", mesa_ID);

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

        public static int RegistrarMesa(Mesa mesa)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Restaurantes'
            string insertQuery = "INSERT INTO Mesas (PosX, PosY, Width, Height, ScaleX, ScaleY, CantPers, Disponible, Restaurante_ID) VALUES (@PosX, @PosY, @Width, @Height, @ScaleX, @ScaleY, @CantPers, @Disponible, @Restaurante_ID)";

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
                        cmd.Parameters.AddWithValue("@PosX", mesa.PosX);
                        cmd.Parameters.AddWithValue("@PosY", mesa.PosY);
                        cmd.Parameters.AddWithValue("@Width", mesa.Width);
                        cmd.Parameters.AddWithValue("@Height", mesa.Height);
                        cmd.Parameters.AddWithValue("@ScaleX", mesa.ScaleX);
                        cmd.Parameters.AddWithValue("@ScaleY", mesa.ScaleY); 
                        cmd.Parameters.AddWithValue("@CantPers", mesa.CantPers);
                        cmd.Parameters.AddWithValue("@Disponible", mesa.Disponible);
                        cmd.Parameters.AddWithValue("@Restaurante_ID", mesa.Restaurante_ID);

                        // Ejecutamos la consulta. ExecuteNonQuery devuelve el número de filas afectadas
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        Trace.WriteLine("Mesa insertada correctamente. Filas afectadas: " + filasAfectadas);
                        return 1;
                    }
                }
                catch (MySqlException ex)
                {
                    // Capturamos errores relacionados con MySQL
                    Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);
                    return 0;
                }
                catch (InvalidOperationException ex)
                {
                    // Capturamos errores de operación inválida en la conexión
                    Trace.WriteLine("Error de operación inválida: " + ex.Message);
                    return 0;
                }
                catch (Exception ex)
                {
                    // Capturamos cualquier otro error inesperado
                    Trace.WriteLine("Error inesperado: " + ex.Message);
                    return 0;
                }
            }
        }

        public static int ActualizarMesa(Mesa mesa)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Mesas SET PosX = @posX, PosY = @posY, Width = @width, Height = @height, ScaleX = @scaleX, ScaleY = @scaleY, CantPers = @cantPers WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@posX", mesa.PosX);
                        cmd.Parameters.AddWithValue("@posY", mesa.PosY);
                        cmd.Parameters.AddWithValue("@width", mesa.Width);
                        cmd.Parameters.AddWithValue("@height", mesa.Height);
                        cmd.Parameters.AddWithValue("@scaleX", mesa.ScaleX);
                        cmd.Parameters.AddWithValue("@scaleY", mesa.ScaleY);
                        cmd.Parameters.AddWithValue("@cantPers", mesa.CantPers);
                        cmd.Parameters.AddWithValue("@id", mesa.Id);

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

        public static int ActualizarCampoDisponibleDeMesa(int id_Mesa, bool b)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Mesas SET Disponible = @disponible WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@disponible", b);
                        cmd.Parameters.AddWithValue("@id", id_Mesa);

                        // Ejecuta la sentencia y retorna el número de filas afectadas
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // La actualización fue exitosa
                            Trace.WriteLine("Registro actualizado correctamente.");
                            return 1;
                        }
                        else
                        {
                            // No se encontró ningún registro con ese ID
                            Trace.WriteLine("No se actualizó ningún registro.");
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

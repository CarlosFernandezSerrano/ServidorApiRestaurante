using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Diagnostics;

namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("mesa")] // Ruta: dirección/mesa/   https://localhost:7233/
    public class MesaController
    {


        public static int RegistrarMesa(Mesa mesa)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Restaurantes'
            string insertQuery = "INSERT INTO Mesas (PosX, PosY, Width, Height, ScaleX, ScaleY, Disponible, Restaurante_ID) VALUES (@PosX, @PosY, @Width, @Height, @ScaleX, @ScaleY, @Disponible, @Restaurante_ID)";

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
                        cmd.Parameters.AddWithValue("@Disponible", mesa.Disponible);
                        cmd.Parameters.AddWithValue("@Restaurante_ID", mesa.Restaurante_ID);

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
                    string query = "UPDATE Mesas SET PosX = @posX, PosY = @posY, Width = @width, Height = @height, ScaleX = @scaleX, ScaleY = @scaleY WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@posX", mesa.PosX);
                        cmd.Parameters.AddWithValue("@posY", mesa.PosY);
                        cmd.Parameters.AddWithValue("@width", mesa.Width);
                        cmd.Parameters.AddWithValue("@height", mesa.Height);
                        cmd.Parameters.AddWithValue("@scaleX", mesa.ScaleX);
                        cmd.Parameters.AddWithValue("@scaleY", mesa.ScaleY);
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
    }
}

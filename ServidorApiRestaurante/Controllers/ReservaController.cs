using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Data;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("reserva")] // Ruta: dirección/reserva/   https://localhost:7233/
    public class ReservaController : ControllerBase
    {

        [Authorize]
        [ValidarTokenFilterController]
        [HttpGet]
        [Route("existe/{id_Mesa}")]
        public dynamic ExisteReservaConUnaMesa_ID(int id_Mesa)
        {
            if (ExisteReservaConfirmadaEnMesa(id_Mesa))
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
        [HttpPost]
        [Route("crearReserva")]
        public dynamic CrearReserva(Reserva reserva)
        {
            Trace.WriteLine("Pasa por método crearReserva");

            // Si la reserva tiene datos del cliente, se intenta crear la reserva desde el canvas "Crear Reserva" en el cliente
            if (reserva.Cliente.Dni.Trim().Length > 0)
            {
                string dniCodificadoABase64 = AESCipher.Encrypt(reserva.Cliente.Dni);

                int num = ClienteController.InsertarRegistro(reserva.Cliente);

                // El cliente se ha creado correctamente
                if (num.Equals(1))
                {
                    // Obtengo todos los datos del cliente creado
                    Cliente clienteEnBDD = ClienteController.ObtenerClientePorDni(dniCodificadoABase64);

                    // Creo la reserva con los datos del cliente
                    reserva.Cliente_Id = clienteEnBDD.Id; // Coloco el ID del cliente existente en la nueva reserva que creo
                    int num3 = InsertarRegistro(reserva);
                    return new { result = num3 };
                }
                /*bool existeDniCliente = ClienteController.ExisteDniCliente(dniCodificadoABase64);

                // Existe el cliente en la BDD
                if (existeDniCliente)
                {
                    Cliente clienteEnBDD = ClienteController.ObtenerClientePorDni(dniCodificadoABase64);
                    int resultadoActualización = ClienteController.ActualizarDatosDelCliente(reserva.Cliente, dniCodificadoABase64);
                    // Si la actualización no ha sido un éxito, se envía la respuesta al cliente
                    if (!resultadoActualización.Equals(1))
                    {
                        return new { result = resultadoActualización };
                    }

                    // El cliente se ha actualizado y ahora creo la reserva con sus datos
                    reserva.Cliente_Id = clienteEnBDD.Id; // Coloco el ID del cliente existente en la nueva reserva que creo
                    int num = InsertarRegistro(reserva);
                    return new { result = num };
                }
                else // No existe el cliente en la BDD, así que lo creo
                {
                    int num = ClienteController.InsertarRegistro(reserva.Cliente);

                    // El cliente se ha creado correctamente
                    if (num.Equals(1))
                    {
                        // Obtengo todos los datos del cliente creado
                        Cliente clienteEnBDD = ClienteController.ObtenerClientePorDni(dniCodificadoABase64);

                        // Creo la reserva con los datos del cliente
                        reserva.Cliente_Id = clienteEnBDD.Id; // Coloco el ID del cliente existente en la nueva reserva que creo
                        int num3 = InsertarRegistro(reserva);
                        return new { result = num3 };
                    }
                }*/
                return new { result = 0 };
            }
            else // No hay datos para el cliente por lo que la reserva se crea para el momento
            {
                int num = InsertarRegistro(reserva);
                return new { result = num };
            }                
        }

        [Authorize]
        [ValidarTokenFilterController]
        [HttpPut]
        [Route("actualizarEstadoReserva")]
        public dynamic ActualizarEstadoReserva(Reserva reserva)
        {
            Trace.WriteLine("Llega a actualizar estado de reserva");
            int i = ActualizarEstadoDeReserva(reserva);

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

        
        private static bool ExisteReservaConfirmadaEnMesa(int id_Mesa)
        {
            string query = "SELECT COUNT(*) FROM Reservas WHERE Mesa_ID = @id AND Estado = @estado";

            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id_Mesa);
                        cmd.Parameters.AddWithValue("@estado", ""+EstadoReserva.Confirmada);

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

        private static int InsertarRegistro(Reserva reserva)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Reservas'
            string insertQuery = "INSERT INTO Reservas (Fecha, Hora, Estado, CantComensales, Cliente_ID, Mesa_ID) VALUES (@fecha, @hora, @estado, @cantComensales, @cliente_id, @mesa_ID)";

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
                        cmd.Parameters.AddWithValue("@estado", ""+reserva.Estado);
                        cmd.Parameters.AddWithValue("@cantComensales", reserva.CantComensales);

                        if (reserva.Cliente_Id.Equals(0))
                        {
                            cmd.Parameters.AddWithValue("@cliente_id", null); // No puedo poner 0 porque no existe en la bdd un registro en la tabla clientes con ID = 0
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@cliente_id", reserva.Cliente_Id); 
                        }

                        cmd.Parameters.AddWithValue("@mesa_ID", reserva.Mesa_Id);

                        // Ejecutamos la consulta. ExecuteNonQuery devuelve el número de filas afectadas
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        Trace.WriteLine("Reserva insertada correctamente. Filas afectadas: " + filasAfectadas);
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

        public static List<Reserva> ObtenerReservasConIDMesa(int id_Mesa)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Reservas WHERE Mesa_ID = @mesa_ID";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@mesa_ID", id_Mesa);
                        using (var reader = cmd.ExecuteReader())
                        {
                            List<Reserva> reservas = new List<Reserva>();

                            while (reader.Read())
                            {
                                int id = reader.GetInt32("ID");
                                string fecha = reader.GetString("Fecha");
                                string hora = reader.GetString("Hora");
                                string estado = reader.GetString("Estado");
                                int cantComensales = reader.GetInt32("CantComensales");
                                int cliente_ID = reader.IsDBNull("Cliente_ID") ? 0 : reader.GetInt32("Cliente_ID");
                                int mesa_ID = reader.GetInt32("Mesa_ID");

                                Cliente cliente;
                                if (cliente_ID.Equals(0))
                                {
                                    cliente = new Cliente("", "", "");
                                }
                                else
                                {
                                    cliente = ClienteController.ObtenerClienteConID(cliente_ID);
                                }

                                reservas.Add(new Reserva(id, fecha, hora, estado, cantComensales, cliente_ID, mesa_ID, cliente));
                            }
                            return reservas;
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

        private static int ActualizarEstadoDeReserva(Reserva reserva)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Reservas SET Estado = @estado WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@estado", reserva.Estado);
                        cmd.Parameters.AddWithValue("@id", reserva.Id);

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
                    Trace.WriteLine("Error al actualizar reserva: " + ex.Message);
                    throw new Exception("Error al actualizar reserva: " + ex.Message);
                }
            }
        }


    }
}

using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServidorApiRestaurante.Controllers
{
    public class ClienteController
    {
        

        public static Cliente ObtenerClienteConID(int id)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Clientes WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("ID");
                                string Nombre = reader.GetString("Nombre");
                                string Dni = reader.GetString("Dni");
                                string NumTelefono = reader.GetString("Num_Telefono");

                                string dniOriginal = AESCipher.Decrypt(Dni);

                                Cliente cliente = new Cliente(Id, Nombre, dniOriginal, NumTelefono);

                                return cliente;
                            }
                            else
                            {
                                Trace.WriteLine("1 Error al obtener cliente: ");
                                return new Cliente("", "", "");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener cliente: " + ex.Message);
                    throw new Exception("Error al obtener cliente: " + ex.Message);
                }
            }
        }

        

        public static void ComprobarQueFuncionaAES()
        {
            Trace.WriteLine("Clave (key): " + Convert.ToBase64String(AESCipher.key));
            Trace.WriteLine("IV (iv): " + Convert.ToBase64String(AESCipher.iv));
            string cadCifrada = AESCipher.Encrypt("Hola Bayan, qué bien que hicimos las paces, la mamá de la mamá");
            Trace.WriteLine("+ + Palabra cifrada: " + cadCifrada);
            //Trace.WriteLine("+ + Prueba: " + AESCipher.AESKeyBase64+"   ;    "+AESCipher.AESIVBase64);
            string cadDescifrada = AESCipher.Decrypt(cadCifrada);

            Trace.WriteLine("+ + Palabra descifrada: " + cadDescifrada);
            Trace.WriteLine(" -- - - -- - - -- - -- - -- - -- - --  -");
            for (int i = 0; i < 5; i++)
            {
                string cadCifrada2 = AESCipher.Encrypt("Hola Bayan, qué bien que hicimos las paces, la mamá de la mamá");
                Trace.WriteLine("+ + Palabra cifrada: " + cadCifrada2);
                //Trace.WriteLine("+ + Prueba: " + AESCipher.AESKeyBase64+"   ;    "+AESCipher.AESIVBase64);
                string cadDescifrada2 = AESCipher.Decrypt(cadCifrada2);

                Trace.WriteLine("+ + Palabra descifrada: " + cadDescifrada2);
            }
            

        }

        public static bool ExisteDniCliente(string dniBase64)
        {
            string query = "SELECT COUNT(*) FROM Clientes WHERE Dni = @dni";

            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@dni", dniBase64);

                        int count = Convert.ToInt32(cmd.ExecuteScalar()); // Obtiene el número de coincidencias
                        return count > 0; // Si es mayor a 0, el cliente existe
                    }
                }
                catch (MySqlException ex)
                {
                    Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);
                    throw new Exception("Error al verificar la existencia del cliente: " + ex.Message);
                }
            }
        }

        public static Cliente ObtenerClientePorDni(string dniBase64)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Clientes WHERE Dni = @dni";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@dni", dniBase64);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("ID");
                                string Nombre = reader.GetString("Nombre");
                                string Dni = reader.GetString("Dni");
                                string NumTelefono = reader.GetString("Num_Telefono");

                                Cliente cliente = new Cliente(Id, Nombre, Dni, NumTelefono);

                                return cliente;
                            }
                            else
                            {
                                Trace.WriteLine("1 Error al obtener cliente: ");
                                return new Cliente("", "", "");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener cliente: " + ex.Message);
                    throw new Exception("Error al obtener cliente: " + ex.Message);
                }
            }
        }

        public static int ActualizarDatosDelClienteSiEsNecesario(Cliente cliente, string dniBase64)
        {
            // Actualizo tanto el nombre como el número de teléfono si están puestos al crear la reserva
            if (cliente.Nombre.Trim().Length > 0 && cliente.NumTelefono.Trim().Length > 0)
            {
                return ActualizarNombreYNumTeléfono(cliente.Nombre, cliente.NumTelefono, dniBase64);
            }

            // Si el empleado ha puesto un número de teléfono, se actualiza
            if (cliente.NumTelefono.Trim().Length > 0)
            {
                return ActualizarNumTeléfono(cliente.NumTelefono, dniBase64);
            }
            else // El empleado (en la app clientee) tiene que poner el nombre de la persona del dni si o sí. Por lo que si no ha puesto un número de teléfono, se actualiza sólo el nombre. Por si quiere que le llamen de otra forma
            {
                return ActualizarNombre(cliente.Nombre, dniBase64);
            }
        }

        private static int ActualizarNombreYNumTeléfono(string nombre, string numTelefono, string dniBase64)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Clientes SET Nombre = @nombre, Num_Telefono = @num_telefono WHERE Dni = @dni";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@num_telefono", numTelefono);
                        cmd.Parameters.AddWithValue("@dni", dniBase64);

                        // Ejecuta la sentencia y retorna el número de filas afectadas
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // La actualización fue exitosa
                            Console.WriteLine("Cliente actualizado correctamente.");
                            return 1;
                        }
                        else
                        {
                            // No se encontró ningún registro con ese dni
                            Console.WriteLine("No se actualizó el cliente");
                            return 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al actualizar cliente: " + ex.Message);
                    throw new Exception("Error al actualizar cliente: " + ex.Message);
                }
            }
        }

        private static int ActualizarNombre(string nombre, string dniBase64)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Clientes SET Nombre = @nombre WHERE Dni = @dni";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@dni", dniBase64);

                        // Ejecuta la sentencia y retorna el número de filas afectadas
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // La actualización fue exitosa
                            Console.WriteLine("Cliente actualizado correctamente.");
                            return 1;
                        }
                        else
                        {
                            // No se encontró ningún registro con ese dni
                            Console.WriteLine("No se actualizó el cliente");
                            return 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al actualizar cliente: " + ex.Message);
                    throw new Exception("Error al actualizar cliente: " + ex.Message);
                }
            }
        }

        private static int ActualizarNumTeléfono(string numTelefono, string dniBase64)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Clientes SET Num_Telefono = @num_telefono WHERE Dni = @dni";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@num_telefono", numTelefono);
                        cmd.Parameters.AddWithValue("@dni", dniBase64);

                        // Ejecuta la sentencia y retorna el número de filas afectadas
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // La actualización fue exitosa
                            Console.WriteLine("Cliente actualizado correctamente.");
                            return 1;
                        }
                        else
                        {
                            // No se encontró ningún registro con ese dni
                            Console.WriteLine("No se actualizó el cliente");
                            return 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al actualizar cliente: " + ex.Message);
                    throw new Exception("Error al actualizar cliente: " + ex.Message);
                }
            }
        }

        public static int InsertarRegistro(Cliente cliente)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Clientes'
            string insertQuery = "INSERT INTO Clientes (Nombre, Dni, Num_Telefono) VALUES (@nombre, @dni, @num_telefono)";

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
                        cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
                        cmd.Parameters.AddWithValue("@dni", AESCipher.Encrypt(cliente.Dni));
                        cmd.Parameters.AddWithValue("@num_telefono", cliente.NumTelefono);

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
    }
}

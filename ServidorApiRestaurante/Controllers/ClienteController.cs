using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Data;
using System.Diagnostics;

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
    }
}

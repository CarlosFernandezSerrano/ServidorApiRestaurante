using Microsoft.AspNetCore.Authorization;
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
    [Route("factura")]
    public class FacturaController : ControllerBase
    {
        [Authorize]
        [ValidarTokenFilterController]
        [HttpPost]
        [Route("crearFactura")]
        public dynamic CrearFactura(Factura factura)
        {

            if (ExisteFacturaID(factura.id))
            {
                return new { result = 2 };
            }
            else
            {
                int num = InsertarRegistro(BDDController.ConnectionString, factura.id, factura.total);
                return new { result = num };
            }
        }
        [Authorize]
        [ValidarTokenFilterController]
        [HttpGet]
        [Route("getFactura/{id}")]
        public static dynamic ObtenerFacturaPorID(int id)
        {
            Factura f = getFacturaByID(id);
            return f;
        }
        private static bool ExisteFacturaID(int id)
        {
            string query = "SELECT count(*) FROM facturas WHERE ID=@id";
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar()); // Obtiene el número de coincidencias
                        return count > 0; // Si es mayor a 0, factura existe
                    }
                }
                catch (MySqlException ex)
                {
                    Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);
                    throw new Exception("Error al verificar la existencia de la factura: " + ex.Message);
                }
            }
        }

        public static Factura getFacturaByID(int id)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM facturas WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("ID");
                                float Total = reader.GetFloat("Total");
                                string Categoria = reader.GetString("Categoria");
                                Factura f = new Factura(Id, Total, new List<Pedido>());

                                return f;
                            }
                            else
                            {
                                throw new Exception("Error al obtener factura");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener factura: " + ex.Message);
                    throw new Exception("Error al obtener factura: " + ex.Message);
                }
            }
        }

        private static int InsertarRegistro(string connectionString, int id, float total)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Facturas'
            string insertQuery = "INSERT INTO Facturas (id, total) VALUES (@id, @total)";

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
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@total", total);

                        // Ejecutamos la consulta. ExecuteNonQuery devuelve el número de filas afectadas
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        Trace.WriteLine("Factura insertada correctamente. Filas afectadas: " + filasAfectadas);
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
        //OBTENER TODOS LOS PEDIDOS A PARTIR DE LA FACTURA
        public static List<Pedido> getListaPedidos(int idFactura)
        {
            List<Pedido> lista = new List<Pedido>();
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Pedido WHERE factura = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", idFactura);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int Id = reader.GetInt32("id");
                                string Fecha = reader.GetString("Fecha");
                                int Mesa = reader.GetInt32("Mesa");
                                string Estado = reader.GetString("Estado");
                                Factura f = ObtenerFacturaPorID(reader.GetInt32("Factura"));

                                lista.Add(new Pedido(Id, Fecha, Mesa, Estado, PedidoController.getListaArticulos(Id), f));
                            }

                            return lista;
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
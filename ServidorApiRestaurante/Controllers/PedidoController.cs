using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;
using ServidorApiRestaurante.Models;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("pedido")]
    public class PedidoController : ControllerBase
    {
        [Authorize]
        [ValidarTokenFilterController]
        [HttpPost]
        [Route("crearPedido")]
        public dynamic CrearPedido(Pedido pedido)
        {

            // Compruebo si existe el trabajador antes de intentar insertarlo, para que no se creen IDs vacíos.
            if (ExistePedidoID(pedido.id))
            {
                return new { result = 2 };
            }
            else
            {
                int num = InsertarRegistro(BDDController.ConnectionString, pedido.id, pedido.fecha, pedido.mesa, pedido.factura.id);
                return new { result = num };
            }
        }

        [Authorize]
        [ValidarTokenFilterController]
        [HttpGet]
        [Route("getPedido/{id}")]
        public dynamic ObtenerPedidoPorID(int id)
        {
            Pedido p = getPedidoByID(id);
            return p;
        }
        private static bool ExistePedidoID(int id)
        {
            string query = "SELECT count(*) FROM pedidos WHERE ID=@id";
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar()); // Obtiene el número de coincidencias
                        return count > 0; // Si es mayor a 0, el articulo existe
                    }
                }
                catch (MySqlException ex)
                {
                    Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);
                    throw new Exception("Error al verificar la existencia del articulo: " + ex.Message);
                }
            }
        }

        public static Pedido getPedidoByID(int id)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Pedidos WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("id");
                                string Fecha = reader.GetString("Fecha");
                                int Mesa = reader.GetInt32("Mesa");
                                string Estado = reader.GetString("Estado");
                                Factura f = FacturaController.ObtenerFacturaPorID(reader.GetInt32("Factura"));
                                Pedido p = new Pedido(Id, Fecha,Mesa, Estado,getListaArticulos(Id),f);
                                return p;
                            }
                            else
                            {
                                throw new Exception("Error al obtener articulo");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener articulo: " + ex.Message);
                    throw new Exception("Error al obtener articulo: " + ex.Message);
                }
            }
        }

        private static int InsertarRegistro(string connectionString, int id, string fecha, int mesa, int factura)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Articulos'
            string insertQuery = "INSERT INTO Pedidos (id, fecha, mesa, estado, factura) VALUES (@id, @fecha, @mesa, @estado, @factura)";

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
                        cmd.Parameters.AddWithValue("@fecha", fecha);
                        cmd.Parameters.AddWithValue("@mesa", mesa);
                        cmd.Parameters.AddWithValue("@estado", "Apuntado");
                        cmd.Parameters.AddWithValue("@factura", factura);

                        // Ejecutamos la consulta. ExecuteNonQuery devuelve el número de filas afectadas
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        Trace.WriteLine("Articulo insertado correctamente. Filas afectadas: " + filasAfectadas);
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
            
        }//OBTENER LISTA DE INSTANCIAARTÍCULOS EN EL PEDIDO
        public static List<InstanciaArticulo> getListaArticulos(int idPedido)
        {
            List<InstanciaArticulo> lista=new List<InstanciaArticulo>();
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM InstanciaArticulos WHERE IDPedido = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", idPedido);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int idArt = reader.GetInt32("idArticulo");
                                int IdPed = reader.GetInt32("idPedido");
                                int Cantidad = reader.GetInt32("Cantidad");
                                float Precio = reader.GetFloat("Precio");

                                lista.Add(new InstanciaArticulo(idArt, IdPed, Cantidad, Precio));
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
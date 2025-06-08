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
        /*[Authorize]
        [ValidarTokenFilterController]*/
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
                int num = InsertarRegistro(BDDController.ConnectionString, pedido.id, pedido.fecha, pedido.mesa,pedido.estado, pedido.factura);
                return new { result = num };
            }
        }

        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getPedido/{id}")]
        public dynamic ObtenerPedidoPorID(int id)
        {
            Pedido p = getPedidoByID(id);
            return p;
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("existePedido/{id}")]
        public bool ExistePedido(int id)
        {
            return ExistePedido(id);
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
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getArticulos/{id}")]
        public dynamic getArticulos(int id){
            return getListaArticulos(id);
        }
        
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getTotal/{id}")]
        public dynamic getTotal(int id){
            List<InstanciaArticulo> articulos=getListaArticulos(id);
            float total=0;
            foreach (InstanciaArticulo a in articulos){
                total += new ArticuloController().ObtenerArticuloPorID(a.idArticulo).precio;
            }
            return total;
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getNumPedidos")]
        public dynamic ObtenerNumPedidos()
        {
            int num= GetNumPedidos();
            return num;
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getTodosPedidos")]
        public dynamic ObtenerTodosPedidos()
        {
            List<Pedido> lista = GetAllPedidos();
            return lista;
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpPut]
        [Route("cambiarEstado")]
        public dynamic CambiarEstado(Pedido p)
        {
            Trace.WriteLine("Llega a cambiar estado");

            int num = updateEstado(p.id, p.estado);

            if (num.Equals(1))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpDelete]
        [Route("borrar/{id}")]
        public dynamic borrarPedido(int id)
        {
            Trace.WriteLine("Llega a borrar mesa x ID");
            int num = deletePedido(id);

            if (num.Equals(1))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }

        }

        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("maxID")]
        public dynamic MaxID()
        {
            int max = getMax();
            return max;
        }

        public static int getMax()
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "select max(id) from pedidos";
                    Trace.WriteLine("Conectado");
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Trace.WriteLine("Leído");
                                int Id = reader.GetInt32("max(id)");
                                return Id;
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
                    return 0;
                }
            }
        }
        public static int deletePedido(int id)
        {
            string query = "DELETE FROM Pedidos WHERE ID = @id";

            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

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
        public static int updateEstado(int id, string estado)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE pedidos SET estado = @estado WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@estado", estado);

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
        private static List<Pedido> GetAllPedidos()
        {
            List<Pedido> lista=new List<Pedido>();
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Pedidos";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                string fecha = reader.GetString("fecha");
                                string estado = reader.GetString("estado");
                                int mesa = reader.GetInt32("mesa");
                                int factura = reader.GetInt32("factura");

                                lista.Add(new Pedido(id,fecha,mesa,estado,factura));
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
        private static int GetNumPedidos()
        {
            string query = "SELECT count(*) FROM pedidos;";
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {

                        int count = Convert.ToInt32(cmd.ExecuteScalar()); // Obtiene el número de coincidencias
                        return count; // Si es mayor a 0, factura existe
                    }
                }
                catch (MySqlException ex)
                {
                    Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);
                    throw new Exception("Error al verificar la existencia de la factura: " + ex.Message);
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
                                int f = reader.GetInt32("Factura");
                                Pedido p = new Pedido(Id, Fecha,Mesa, Estado,f);
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

        private static int InsertarRegistro(string connectionString, int id, string fecha, int mesa,string estado, int factura)
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
                        cmd.Parameters.AddWithValue("@estado", estado);
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

                                lista.Add(new InstanciaArticulo(idArt, IdPed, Cantidad));
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
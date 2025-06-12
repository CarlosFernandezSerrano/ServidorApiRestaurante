using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;
using Org.BouncyCastle.Crypto;
using ServidorApiRestaurante.Models;
using System.Data;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

//OBTENER FACTURAS POR MESA Y LUEGO VER SI EXISTE UNA ACTIVA
//MÉTODO PUT PARA CAMBIAR UNA FACTURA DE ACTIVA A INACTIVA
namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("factura")]
    public class FacturaController : ControllerBase
    {
        /*[Authorize]
        [ValidarTokenFilterController]*/
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
                int num = InsertarRegistro(BDDController.ConnectionString, factura.id, factura.total,factura.activa,factura.mesa);
                return new { result = num };
            }
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getFactura/{id}")]
        public dynamic ObtenerFacturaPorID(int id)
        {
            Factura f = getFacturaByID(id);
            return f;
        }

        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getPedidos/{id}")]
        public dynamic ObtenerPedidos(int id)
        {
            List<Pedido> lista= getListaPedidos(id);
            return lista;
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getTotal/{id}")]
        public dynamic ObtenerTotal(int id)
        {
            //ESTO TAMBIÉN DEBERÍA ACTUALIZAR EL TOTAL TAL VEZ
            float total=0;
            List<Pedido> lista= getListaPedidos(id);
            PedidoController PC=new PedidoController();
            foreach (Pedido p in lista){
                total+=PC.getTotal(p.id);
            }
            return total;
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getNumFacturas")]
        public dynamic ObtenerNumFacturas()
        {
            int num= GetNumFacturas();
            return num;
        }

        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpPut]
        [Route("actualizarActiva")]
        public dynamic ActualizarActiva(Factura f)
        {
            Trace.WriteLine("Llega a actualizar campo activa de factura");

            int num = updateActivo(f.id,f.activa);

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
        [Route("existeFactura/{id}")]
        public bool ExisteFactura(int id)
        {
            return ExisteFacturaID(id);
        }
        private static int GetNumFacturas()
        {
            string query = "SELECT count(*) FROM facturas;";
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
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpDelete]
        [Route("borrarPedidos/{idF}")]
        public dynamic BorrarPedidos(string idF)
        {
            Trace.WriteLine("Llega a borrar mesa x ID");
            int num = deletePedidos(idF);

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
        [Route("obtenerActiva/{mesa}")]
        public dynamic ObtenerActiva(int mesa)
        {
            Factura f = getActiva(mesa);
            return f;
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
                        string query = "select max(id) from facturas";
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
        public static Factura getActiva(int idMesa)
        {
            Factura fac = null;
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM facturas WHERE mesa = @idM AND activa=1";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@idM", idMesa);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int Id = reader.GetInt32("id");
                                float total = reader.GetFloat("total");
                                int activa = reader.GetInt32("activa");
                                int Mesa = reader.GetInt32("mesa");
                                fac = new Factura(Id, total,(activa==1), Mesa);//Esto debería dar un solo resultado, pero en caso contrario (Si sale algo mal) iremos con la factura más antigua
                            }

                            return fac;
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
        private static int deletePedidos(string idF)
        {
            string query = "DELETE FROM pedidos WHERE factura = @id";

            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", idF);

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
        public static int updateActivo(int id, bool a)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE facturas SET activa = @activa WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@id", id);
                        if (a)
                        {
                            cmd.Parameters.AddWithValue("@activa", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@activa", 0);
                        }

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
        private static bool ExisteFacturaID(int id)
        {
            string query = "SELECT count(*) FROM facturas WHERE id=@id";
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
                                bool activa = reader.GetInt32("Activa")==1;
                                int mesa = reader.GetInt32("mesa");
                                Factura f = new Factura(Id, Total,activa,mesa);

                                return f;
                            }
                            else
                            {
                                Trace.WriteLine("Error al obtener factura");
                                return null;
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

        private static int InsertarRegistro(string connectionString, int id, float total, bool act,int mesa)
        {
            int activa;
            if (act)
            {
                activa = 1;
            }
            else activa = 0;
                // Consulta SQL parametrizada para insertar datos en la tabla 'Facturas'
                string insertQuery = "INSERT INTO facturas (id, total,activa,mesa) VALUES (@id, @total,@activa,@mesa)";

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
                        cmd.Parameters.AddWithValue("@activa", activa);
                        cmd.Parameters.AddWithValue("@mesa", mesa);

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
                    string query = "SELECT * FROM pedidos WHERE factura = @id";

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
                                int f= reader.GetInt32("Factura");//en realidad esto no haría falta

                                lista.Add(new Pedido(Id, Fecha, Mesa, Estado,f));
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
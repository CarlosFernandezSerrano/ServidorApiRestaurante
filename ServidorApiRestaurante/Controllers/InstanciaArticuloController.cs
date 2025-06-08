using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;
using ServidorApiRestaurante.Models;
using System.Data;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
//HACER FUNCIÓN PARA AUMENTAR NUM CUANDO SE CREE Y TAL VEZ DISMINUIR NUM
//IMPLEMENTAR UN BOTÓN PARA ELIMINAR UNA INSTANCIAARTICULO
namespace ServidorApiRestaurante.Controllers
{
    [ApiController]
    [Route("instanciaArticulo")]
    public class InstanciaArticuloController : ControllerBase
    {
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpPost]
        [Route("crearInstancia")]
        public dynamic CrearInstancia(InstanciaArticulo instancia)
        {
            Trace.WriteLine("Pingo");
            Trace.WriteLine("idArticulo:" + instancia.idArticulo);
            Trace.WriteLine("idPedido: " + instancia.idPedido);


            // Compruebo si existe el trabajador antes de intentar insertarlo, para que no se creen IDs vacíos.
            if (ExisteArticuloID(instancia.idArticulo,instancia.idPedido))
            {
                return new { result = 2 };
            }
            else
            {
                int num = InsertarRegistro(BDDController.ConnectionString, instancia.idArticulo, instancia.idPedido, instancia.cantidad);
                return new { result = num };
            }
        }

        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getInstancia/{idA}/{idP}")]
        public dynamic ObtenerInstanciaPorID(int idA,int idP)
        {
            InstanciaArticulo art = getInstanciaByID(idA,idP);
            return art;
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpPut]
        [Route("aumentar")]
        public dynamic AumentarArticulo(InstanciaArticulo art)
        {
            Trace.WriteLine("Llega a cambiar estado");

            int num=AumentarArticuloID(art);

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
        [Route("borrar/{pid}/{aid}")]
        public dynamic borrarInstancia(int aid, int pid)
        {
            Trace.WriteLine("Llega a borrar mesa x ID");
            int num = deleteInstancia(aid,pid);

            if (num.Equals(1))
            {
                return new { result = 1 };
            }
            else
            {
                return new { result = 0 };
            }
        }

        public static int deleteInstancia(int aid,int pid)
        {
            string query = "DELETE FROM instanciaarticulos WHERE IDpedido = @pid AND IDarticulo=@aid";

            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@aid", aid);
                        cmd.Parameters.AddWithValue("@pid", pid);

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

        private static int AumentarArticuloID(InstanciaArticulo art){
            // Consulta SQL parametrizada para insertar datos en la tabla 'Articulos'
            string insertQuery = "UPDATE InstanciaArticulos SET cantidad=@cantidad WHERE idArticulo=@idArticulo AND idPedido=@idPedido";
            Trace.WriteLine("Entramos aumentar");
            InstanciaArticulo insta = getInstanciaByID(art.idArticulo, art.idPedido);
            if (insta != null)
            {
                int cantidadOriginal = insta.cantidad;
                using (var connection = new MySqlConnection(BDDController.ConnectionString))
                {
                    try
                    {
                        // Abrimos la conexión con la base de datos
                        connection.Open();
                        Trace.WriteLine("Conseguimos conexion");
                        // Creamos el comando para ejecutar la consulta SQL
                        using (var cmd = new MySqlCommand(insertQuery, connection))
                        {
                            // Asignamos los parámetros con sus respectivos valores
                            cmd.Parameters.AddWithValue("@idArticulo", art.idArticulo);
                            cmd.Parameters.AddWithValue("@idPedido", art.idPedido);
                            cmd.Parameters.AddWithValue("@cantidad", cantidadOriginal + art.cantidad);
                            Trace.WriteLine("Addeamos parametros");
                            // Ejecutamos la consulta. ExecuteNonQuery devuelve el número de filas afectadas
                            int filasAfectadas = cmd.ExecuteNonQuery();
                            if (filasAfectadas > 0)
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
                    catch (MySqlException ex)
                    {
                        // Capturamos errores relacionados con MySQL
                        Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);

                    }
                    catch (InvalidOperationException ex)
                    {
                        // Capturamos errores de operación inválida en la conexión
                        Trace.WriteLine("Error de operación inválida: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // Capturamos cualquier otro error inesperado
                        Trace.WriteLine("Error inesperado: " + ex.Message);
                    }

                    return 2;
                }
            }
            else return 3;
                Trace.WriteLine("Obtenemos cantidad");
            // Usamos 'using' para asegurar que la conexión se cierre correctamente
            
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("existeInstancia/{idA}/{idP}")]
        public bool ExisteInstancia(int idA,int idP)
        {
            return ExisteArticuloID(idA,idP);
        }
        private static bool ExisteArticuloID(int idA,int idP)
        {
            string query = "SELECT count(*) FROM instanciaarticulos WHERE IDARTICULO=@idArticulo AND IDPEDIDO=@idPedido";
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@idArticulo", idA);
                        cmd.Parameters.AddWithValue("@idPedido", idP);

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

        private static InstanciaArticulo getInstanciaByID(int idA, int idP)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM InstanciaArticulos WHERE idArticulo = @idArticulo AND idPedido=@idPedido";
                    Trace.WriteLine("Entramos get");
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@idArticulo", idA);
                        cmd.Parameters.AddWithValue("@idPedido", idP);
                        Trace.WriteLine("Entramos var cmd");
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Trace.WriteLine("Entramos loop");
                                int idArt = reader.GetInt32("idArticulo");
                                int IdPed = reader.GetInt32("idPedido");
                                int Cantidad = reader.GetInt32("Cantidad");
                                InstanciaArticulo art = new InstanciaArticulo(idArt,IdPed,Cantidad);
                                Trace.WriteLine("Salimos loop");
                                return art;
                            }
                            else
                            {
                                Trace.WriteLine("No existe tal instancia");
                                return null;
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

        private static int InsertarRegistro(string connectionString, int idA, int idP, int cantidad)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Articulos'
            string insertQuery = "INSERT INTO InstanciaArticulos (idArticulo, idPedido, cantidad) VALUES (@idArticulo, @idPedido, @cantidad)";

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
                        cmd.Parameters.AddWithValue("@idArticulo", idA);
                        cmd.Parameters.AddWithValue("@idPedido", idP);
                        cmd.Parameters.AddWithValue("@cantidad", cantidad);

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
        }
        /*
        No son necesarios, cuando se necesite obtener el pedido/factura se hará con sus controllers directamente
        [Authorize]
        [ValidarTokenFilterController]
        [HttpGet]
        [Route("getInstancia/{idA}/{idP}")]
        public dynamic getPedido(int id)
        {
            return PedidoController.getPedidoByID(id);
        }
        public dynamic getFactura(int id)
        {
            return FacturaController.getFacturaByID(id);
        }*/
    }
}
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
    [Route("instanciaArticulo")]
    public class InstanciaArticuloController : ControllerBase
    {
        [Authorize]
        [ValidarTokenFilterController]
        [HttpPost]
        [Route("crearInstancia")]
        public dynamic CrearInstancia(InstanciaArticulo instancia)
        {
            Trace.WriteLine("idArticulo:" + instancia.idArticulo);
            Trace.WriteLine("idPedido: " + instancia.idPedido);


            // Compruebo si existe el trabajador antes de intentar insertarlo, para que no se creen IDs vacíos.
            if (ExisteArticuloID(instancia.idArticulo,instancia.idPedido))
            {
                return new { result = 2 };
            }
            else
            {
                int num = InsertarRegistro(BDDController.ConnectionString, instancia.idArticulo, instancia.idPedido, instancia.cantidad, instancia.precio);
                return new { result = num };
            }
        }

        [Authorize]
        [ValidarTokenFilterController]
        [HttpGet]
        [Route("getInstancia/{idA}/{idP}")]
        public dynamic ObtenerInstanciaPorID(int idA,int idP)
        {
            InstanciaArticulo art = getInstanciaByID(idA,idP);
            return art;
        }
        private static bool ExisteArticuloID(int idA,int idP)
        {
            string query = "SELECT count(*) FROM articulos WHERE IDARTICULO=@idArticulo AND IDPEDIDO=@idPedido";
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
                    string query = "SELECT * FROM Articulos WHERE idArticulo = @idArticulo AND idPedido=@idPedido";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@idArticulo", idA);
                        cmd.Parameters.AddWithValue("@idPedido", idP);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int idArt = reader.GetInt32("idArticulo");
                                int IdPed = reader.GetInt32("idPedido");
                                int Cantidad = reader.GetInt32("Cantidad");
                                float Precio = reader.GetFloat("Precio");
                                InstanciaArticulo art = new InstanciaArticulo(idArt,IdPed,Cantidad,Precio);

                                return art;
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

        private static int InsertarRegistro(string connectionString, int idA, int idP, int cantidad, float precio)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Articulos'
            string insertQuery = "INSERT INTO InstanciaArticulos (idArticulo, idPedido, cantidad, precio) VALUES (@idArticulo, @idPedido, @cantidad, @precio)";

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
                        cmd.Parameters.AddWithValue("@precio", precio);

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
        public static Pedido getPedido(int id)
        {
            return PedidoController.getPedidoByID(id);
        }
        public static Factura getFactura(int id)
        {
            return FacturaController.getFacturaByID(id);
        }
    }
}
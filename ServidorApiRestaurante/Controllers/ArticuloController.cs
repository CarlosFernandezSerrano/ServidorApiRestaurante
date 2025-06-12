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
    [Route("articulo")]
    public class ArticuloController : ControllerBase
    {
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpPost]
        [Route("crearArticulo")]
        public dynamic CrearArticulo(Articulo articulo)
        {
            Trace.WriteLine("articulo.id: " + articulo.id);
            Trace.WriteLine("articulo.nombre: " + articulo.nombre);
            Trace.WriteLine("articulo.precio " + articulo.precio);
            Trace.WriteLine("articulo.categoria " + articulo.categoria);

            // Compruebo si existe el trabajador antes de intentar insertarlo, para que no se creen IDs vac�os.
            if (ExisteArticuloID(articulo.id)||existeArticuloNombre(articulo.nombre))
            {
                return new { result = 2 };
            }
            else
            {
                int num = InsertarRegistro(BDDController.ConnectionString, articulo.id, articulo.nombre, articulo.precio, articulo.categoria);
                return new { result = num };
            }
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getArticulo/{id}")]
        public dynamic ObtenerArticuloPorID(int id)
        {
            Articulo art = getArticuloByID(id);
            return art;
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getArticuloNombre/{nombre}")]
        public dynamic ObtenerArticuloPorNombre(string nombre)
        {
            Articulo art = getArticuloByNombre(nombre);
            return art;
        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getNumArticulos")]
        public dynamic ObtenerNumArticulos()
        {
            int num = GetNumArticulos();
            return num;
        }

        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("getArticulosCat/{cat}")]
        public dynamic ObtenerArticulosCat(string cat)
        {
            List<Articulo> lista = GetArticulosCat(cat);
            return lista;
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

        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpDelete]
        [Route("borrar/{id}")]
        public dynamic borrarArticulo(int id)
        {
            Trace.WriteLine("Llega a borrar articulo x ID");
            int num = deleteArticulo(id);

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
        [HttpPut]
        [Route("modificar")]
        public dynamic modificarArticulo(Articulo art)
        {
            Trace.WriteLine("Llega a borrar articulo x ID");
            int num = updateArticulo(art);

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
        [Route("obtenerTodos")]
        public dynamic obtenerTodosArticulos()
        {
            Trace.WriteLine("Llega a borrar articulo x ID");
            return GetAllArticulos();

        }
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("existeArticuloID/{id}")]
        public bool existeArticuloID(int id)
        {
            return existsArticuloID(id);
        }

        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet]
        [Route("existeArticuloN/{nombre}")]
        public bool existeArticuloNombre(string nombre)
        {
            return existsArticuloN(nombre);
        }

        /*[Authorize]
        [ValidarTokenFilterController]*/
        /*[HttpPut("{id}/imagen")]
        public async Task<IActionResult> UpdateArticuloImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            using var connection = new MySqlConnection(BDDController.ConnectionString);
            await connection.OpenAsync();

            var cmd = new MySqlCommand("UPDATE articulos SET Imagen = @imagen WHERE Id = @id", connection);
            cmd.Parameters.AddWithValue("@imagen", imageBytes);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
                return NotFound("Artículo no encontrado");

            return Ok("Imagen actualizada");
        }
        */
        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpPost("{id}/actualizar-imagen")]
        public async Task<IActionResult> ActualizarImagen(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se recibió ningún archivo");

            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            using var connection = new MySqlConnection(BDDController.ConnectionString);
            await connection.OpenAsync();

            var cmd = new MySqlCommand("UPDATE articulos SET Imagen = @imagen WHERE Id = @id", connection);
            cmd.Parameters.AddWithValue("@imagen", imageBytes);
            cmd.Parameters.AddWithValue("@id", id);

            int affectedRows = await cmd.ExecuteNonQueryAsync();

            if (affectedRows == 0)
                return NotFound("Artículo no encontrado");

            return Ok("Imagen actualizada correctamente");
        }

        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            Trace.WriteLine("Prueba get1");
            using var connection = new MySqlConnection(BDDController.ConnectionString);
            await connection.OpenAsync();
            Trace.WriteLine("Prueba get2");
            var cmd = new MySqlCommand("SELECT imagen FROM articulos WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            Trace.WriteLine("Prueba get3");
            var reader = await cmd.ExecuteReaderAsync();
            Trace.WriteLine("Prueba get32");
            if (await reader.ReadAsync())
            {
                Trace.WriteLine("Prueba get4");
                var imageData = (byte[])reader["imagen"];
                Trace.WriteLine("Prueba get5");
                return File(imageData, "image/jpeg");
            }

            return NotFound();
        }

        /*[Authorize]
        [ValidarTokenFilterController]*/
        [HttpGet("tieneImagen/{id}")]
        public bool tieneImagen(int id)
        {
            string query = "SELECT count(*) FROM articulos WHERE imagen IS NOT NULL and ID=@id";
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
        /*[HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, int articleId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var imageBytes = ms.ToArray();

            using var connection = new MySqlConnection("https://localhost:7233/articulo/upload");
            await connection.OpenAsync();

            var cmd = new MySqlCommand("INSERT INTO Images (Name, ImageData, idArticulo) VALUES (@name, @data, @articleId)", connection);
            cmd.Parameters.AddWithValue("@name", file.FileName);
            cmd.Parameters.AddWithValue("@data", imageBytes);
            cmd.Parameters.AddWithValue("@articleId", articleId);
            await cmd.ExecuteNonQueryAsync();

            return Ok("Image uploaded");
        }
        /*[Authorize]
        [ValidarTokenFilterController]
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            using var connection = new MySqlConnection(BDDController.ConnectionString);
            await connection.OpenAsync();

            var cmd = new MySqlCommand("SELECT ImageData FROM Images WHERE Id = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);

            var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var imageData = (byte[])reader["ImageData"];
                return File(imageData, "image/jpeg");
            }

            return NotFound();
        }

        /*[Authorize]
        [ValidarTokenFilterController]
        [HttpGet("imageExiste/{id}")]
        public dynamic existsImage(int id)
        {
            string query = "SELECT count(*) FROM images WHERE ID=@id";
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
        }*/


        private static bool existsArticuloID(int id)
        {
            string query = "SELECT count(*) FROM articulos WHERE ID=@id";
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
        private static bool existsArticuloN(string nombre)
        {
            string query = "SELECT count(*) FROM articulos WHERE nombre=@nombre";
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);

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
        private static List<Articulo> GetAllArticulos()
        {
            List<Articulo> lista = new List<Articulo>();
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM articulos";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                string nombre = reader.GetString("nombre");
                                string categoria = reader.GetString("categoria");
                                float precio = reader.GetInt32("precio");

                                lista.Add(new Articulo(id, nombre, precio, categoria));
                            }

                            return lista;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener precios: " + ex.Message);
                    throw new Exception("Error al obtener precios: " + ex.Message);
                }
            }

        }
        public static int updateArticulo(Articulo a)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE articulos SET precio=@precio, nombre=@nombre, categoria=@categoria WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        // Asignación de valores a los parámetros
                        cmd.Parameters.AddWithValue("@nombre", a.nombre);
                        cmd.Parameters.AddWithValue("@precio", a.precio);
                        cmd.Parameters.AddWithValue("@categoria", a.categoria);
                        cmd.Parameters.AddWithValue("@id", a.id);

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

        public static int deleteArticulo(int id)
        {
            string query = "DELETE FROM articulos WHERE ID = @id";

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
                    throw new Exception("Error al verificar la existencia del articulo: " + ex.Message);
                }
            }
        }

        public static int getMax()
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "select max(id) from articulos";
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
        private List<Articulo> GetArticulosCat(string cat)
        {
            List<Articulo> lista = new List<Articulo>();
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM articulos WHERE categoria = @categoria";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@categoria", cat);
                        using (var reader = cmd.ExecuteReader())
                        {
                            Trace.WriteLine("Entrado a getartcat");
                            while (reader.Read())
                            {
                                Trace.WriteLine("Entrado a loop");
                                int id = reader.GetInt32("id");
                                float precio = reader.GetFloat("precio");
                                string nombre = reader.GetString("nombre");
                                string categoria = reader.GetString("categoria");

                                lista.Add(new Articulo(id, nombre, precio, categoria));
                            }

                            return lista;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener precios: " + ex.Message);
                    throw new Exception("Error al obtener precios: " + ex.Message);
                }
            }
        }

        private static int GetNumArticulos()
        {
            string query = "SELECT count(*) FROM articulos;";
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
        private static bool ExisteArticuloID(int id)
        {
            string query = "SELECT count(*) FROM articulos WHERE ID=@id";
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar()); // Obtiene el n�mero de coincidencias
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

        private static Articulo getArticuloByID(int id)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM articulos WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("ID");
                                string Nombre = reader.GetString("Nombre");
                                float Precio = reader.GetFloat("Precio");
                                string Categoria = reader.GetString("Categoria");
                                Articulo art = new Articulo(Id, Nombre, Precio, Categoria);

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
        private static Articulo getArticuloByNombre(string nombre)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM articulos WHERE nombre = @nombre";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("ID");
                                string Nombre = reader.GetString("Nombre");
                                float Precio = reader.GetFloat("Precio");
                                string Categoria = reader.GetString("Categoria");
                                Articulo art = new Articulo(Id, Nombre, Precio, Categoria);

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
        private static int InsertarRegistro(string connectionString, int id, string nombre, float precio, string categoria)
        {
            // Consulta SQL parametrizada para insertar datos en la tabla 'Articulos'
            string insertQuery = "INSERT INTO articulos (id, nombre, precio, categoria) VALUES (@id, @nombre, @precio, @categoria)";

            // Usamos 'using' para asegurar que la conexi�n se cierre correctamente
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    // Abrimos la conexi�n con la base de datos
                    connection.Open();

                    // Creamos el comando para ejecutar la consulta SQL
                    using (var cmd = new MySqlCommand(insertQuery, connection))
                    {
                        // Asignamos los par�metros con sus respectivos valores
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@precio", precio);
                        cmd.Parameters.AddWithValue("@categoria", categoria);

                        // Ejecutamos la consulta. ExecuteNonQuery devuelve el n�mero de filas afectadas
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
                    // Capturamos errores de operaci�n inv�lida en la conexi�n
                    Trace.WriteLine("Error de operaci�n inv�lida: " + ex.Message);
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
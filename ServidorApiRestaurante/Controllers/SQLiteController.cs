using Microsoft.AspNetCore.Mvc.RazorPages;
using ServidorApiRestaurante.Models;
using System.Data.SQLite;
using System.Diagnostics;
using System.Xml.Linq;

namespace ServidorApiRestaurante.Controllers
{
    public class SQLiteController
    {
        private static string connectionString;

        public SQLiteController(string databasePath)
        {
            // Aquí definimos la cadena de conexión con SQLite
            connectionString = $"Data Source={databasePath};Version=3;";
        }

        public void CreateDatabase()
        {
            // Obtener la ruta de la base de datos desde la cadena de conexión
            string dbFilePath = new SQLiteConnectionStringBuilder(connectionString).DataSource;

            // Verificar si la base de datos ya existe
            if (File.Exists(dbFilePath))
            {
                Trace.WriteLine("La base de datos ya existe. No es necesario crearla.");
                return; // Salimos del método si la base de datos ya existe
            }else
            {
                Trace.WriteLine("La base de datos no existe. Es necesario crearla.");
                return; //Igualmente, pongo return para pruebas. En el futuro quitar el return
            }


                // Se crea una conexión a la base de datos SQLite usando la cadena de conexión definida en "connectionString".
                // Si el archivo de la base de datos no existe en la ruta especificada, SQLite lo creará automáticamente al abrir la conexión.
                using (var connection = new SQLiteConnection(connectionString))
                {
                    // Abre la conexión con la base de datos.
                    // Si la base de datos no existe, este paso la creará en la ubicación especificada en connectionString.
                    connection.Open();

                    // Defino la consulta SQL para crear la tabla "Users" si aún no existe.
                    string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,  -- Identificador único de cada usuario, se genera automáticamente.
                Name TEXT NOT NULL UNIQUE,                   -- Nombre del usuario, obligatorio y único.
                Password TEXT NOT NULL,                      -- Contraseña del usuario, obligatoria.
                Rol TEXT NOT NULL,                           -- Rol del usuario, obligatoria.
                CantMapas INTEGER NOT NULL,                  -- CantMapas del usuario, obligatorio.
                NombresRestaurantes TEXT NOT NULL            -- NombresRestaurantes del usuario, obligatorio.
            );";

                    // Se crea un comando SQL que ejecutará la consulta de creación de la tabla.
                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        // Ejecuta la consulta SQL. Como es un comando que no devuelve resultados (no es una consulta SELECT),
                        // usamos ExecuteNonQuery().
                        command.ExecuteNonQuery();
                    }
                } // Al salir del bloque "using", la conexión y los recursos se liberan automáticamente.
        }

        // Insertar un nuevo usuario
        public void InsertUser(string name, int age)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO Users (Name, Age) VALUES (@Name, @Age)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Obtener todos los usuarios
        public List<Trabajador> GetClientes()
        {
            var users = new List<Trabajador>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT Id, Name, Age FROM Users";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string contraseña = reader.GetString(2);
                            int rol_Id = reader.GetInt32(0);
                            int restaurante_Id = reader.GetInt32(0);

                            users.Add(new Trabajador(id, name, contraseña, new Rol(), new Restaurante()));
                        }
                    }
                }
            }
            return users;
        }

        // Actualizar un usuario
        public void UpdateUser(int id, string name, int age)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE Users SET Name = @Name, Age = @Age WHERE Id = @Id";
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Eliminar un usuario
        public void DeleteUser(int id)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Users WHERE Id = @Id";
                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

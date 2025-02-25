using Microsoft.AspNetCore.Mvc.RazorPages;
using ServidorApiRestaurante.Model;
using System.Data.SQLite;
using System.Xml.Linq;

namespace ServidorApiRestaurante.Controllers
{
    public class SQLiteController
    {
        private string connectionString;

        public SQLiteController(string databasePath)
        {
            // Aquí definimos la cadena de conexión con SQLite
            connectionString = $"Data Source={databasePath};Version=3;";
        }

        public void CreateDatabase()
        {
            // Conexión y creación de la base de datos si no existe
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Users (
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Name TEXT NOT NULL,
                                            Age INTEGER NOT NULL
                                          );";

                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
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
        public List<Cliente> GetClientes()
        {
            var users = new List<Cliente>();
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
                            //Id = reader.GetInt32(0),
                            string name = reader.GetString(1);
                            string contraseña = reader.GetString(2);
                            string rol = reader.GetString(3);
                            string cantMapas = reader.GetString(4);
                            string mapas = reader.GetString(5);

                            string [] arrayListaMapas = mapas.Trim().Split(","); //El valor de la cadena mapas, será algo como: "nombreMapa1,nombreMapa2,etc"
                            List<string> listaMapas = arrayListaMapas.ToList();

                            users.Add(new Cliente(name, contraseña, rol, cantMapas, listaMapas));
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

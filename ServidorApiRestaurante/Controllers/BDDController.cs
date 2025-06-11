using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Diagnostics;
using System.Security.Cryptography;

namespace ServidorApiRestaurante.Controllers
{
    public class BDDController
    {
        //public static readonly string ConnectionString = "server=localhost;port=3306;user id=root;password=;database=restdb";
        public static readonly string ConnectionString = "server=ballast.proxy.rlwy.net;port=27564;user id=root;password=hiAkqzCZQpwFiFUTSdpAsyroJvlZdLzd;database=railway;";  //"server=localhost;port=3306;user id=root;password=;database=restdb";


        public static void CrearBDD()
        {
            // Cadena de conexión al servidor MySQL (no necesitas especificar la base de datos aquí)
            string connectionString = "server=localhost;port=3306;user id=root;password=";
            Trace.WriteLine("PASA POR AQUÍ AL EJECUTAR EL PROGRAMA ----------------------:"); //Mostrar contenido en salida
            // Nombre de la base de datos que quieres crear
            string databaseName = "restdb";

            // Crear la conexión al servidor
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Verificar si la base de datos ya existe
                    string checkDatabaseQuery = $"SHOW DATABASES LIKE '{databaseName}'";
                    using (var cmd = new MySqlCommand(checkDatabaseQuery, connection))
                    {
                        var result = cmd.ExecuteScalar(); // Ejecuta la consulta y obtiene un solo valor
                        if (result != null)
                        {
                            // Si la base de datos existe, mostrar el mensaje
                            Trace.WriteLine($"La base de datos {databaseName} ya existe.");
                        }
                        else
                        {
                            // Si la base de datos no existe, crearla
                            string createDatabaseQuery = $"CREATE DATABASE {databaseName};";
                            using (var createCmd = new MySqlCommand(createDatabaseQuery, connection))
                            {
                                createCmd.ExecuteNonQuery();
                                Trace.WriteLine($"Base de datos {databaseName} creada exitosamente.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al conectar o crear la base de datos: " + ex.Message);
                }
            }
        }

        public static void CrearTablas()
        {
            //string connectionString = 

            // Defino la consulta SQL para crear la tabla si no existe.
            string consultaDeTablaRols = @"
            CREATE TABLE IF NOT EXISTS Rols (
            ID INTEGER AUTO_INCREMENT PRIMARY KEY,  
            Nombre VARCHAR(40) UNIQUE NOT NULL            
            );";
            CrearTabla(ConnectionString, consultaDeTablaRols, "Rols");

            string consultaDeTablaRestaurantes = @"
            CREATE TABLE IF NOT EXISTS Restaurantes (
            ID INTEGER AUTO_INCREMENT PRIMARY KEY,  
            Nombre VARCHAR(40) UNIQUE NOT NULL,            
            Hora_Apertura VARCHAR(10) NOT NULL, 
            Hora_Cierre VARCHAR(10) NOT NULL,
            TiempoParaComer VARCHAR(10) NOT NULL
            );";
            CrearTabla(ConnectionString, consultaDeTablaRestaurantes, "Restaurantes");

            //// BCrypt genera un hash siempre de una longitud fija de 60 caracteres
            string consultaDeTablaTrabajadores = @"
            CREATE TABLE IF NOT EXISTS Trabajadores (
            ID INTEGER AUTO_INCREMENT PRIMARY KEY,  
            Nombre VARCHAR(40) UNIQUE NOT NULL,            
            Password VARCHAR(60) NOT NULL,              
            Rol_ID INTEGER NOT NULL,                
            Restaurante_ID INTEGER,        
            FOREIGN KEY (Rol_ID) REFERENCES Rols(ID), 
            FOREIGN KEY (Restaurante_ID) REFERENCES Restaurantes(ID) ON DELETE CASCADE 
            );";
            CrearTabla(ConnectionString, consultaDeTablaTrabajadores, "Trabajadores");

            string consultaDeTablaMesas = @"
            CREATE TABLE IF NOT EXISTS Mesas (
            ID INTEGER AUTO_INCREMENT PRIMARY KEY,  
            PosX FLOAT NOT NULL,  
            PosY FLOAT NOT NULL, 
            Width FLOAT NOT NULL,
            Height FLOAT NOT NULL,
            ScaleX FLOAT NOT NULL, 
            ScaleY FLOAT NOT NULL,
            CantPers INTEGER NOT NULL,
            Disponible BOOLEAN NOT NULL DEFAULT 1,
            Restaurante_ID INTEGER NOT NULL,        
            FOREIGN KEY (Restaurante_ID) REFERENCES Restaurantes(ID) ON DELETE CASCADE 
            );";
            CrearTabla(ConnectionString, consultaDeTablaMesas, "Mesas");

            string consultaDeTablaClientes = @"
            CREATE TABLE IF NOT EXISTS Clientes (
            ID INTEGER AUTO_INCREMENT PRIMARY KEY,  
            Nombre VARCHAR(40) NOT NULL,
            Dni VARCHAR(50) NOT NULL,
            Num_Telefono VARCHAR(50)
            );";
            CrearTabla(ConnectionString, consultaDeTablaClientes, "Clientes");

            string consultaDeTablaReservas = @"
            CREATE TABLE IF NOT EXISTS Reservas (
            ID INTEGER AUTO_INCREMENT PRIMARY KEY,
            Fecha VARCHAR(10) NOT NULL,
            Hora VARCHAR(9) NOT NULL,
            Estado VARCHAR(10) NOT NULL, 
            CantComensales INTEGER NOT NULL,
            Cliente_ID INTEGER,
            Mesa_ID INTEGER NOT NULL,
            FOREIGN KEY (Cliente_ID) REFERENCES Clientes(ID),
            FOREIGN KEY (Mesa_ID) REFERENCES Mesas(ID) ON DELETE CASCADE 
            );"; 
            CrearTabla(ConnectionString, consultaDeTablaReservas, "Reservas");

            string consultaArticulos = @"CREATE TABLE articulos (
  id int(11) NOT NULL,
  precio float DEFAULT NULL,
  nombre varchar(64) DEFAULT NULL,
  categoria varchar(32) DEFAULT NULL,
  imagen longblob DEFAULT NULL,
  PRIMARY KEY (id)
)
;INSERT IGNORE INTO articulos VALUES (1,5,'coca cola','bebidas'),(2,50,'Filete wagyu','Platos'),(3,10,'Fondue','postres'),(4,7,'Bravas','Entrantes'),(6,6,'string','string'),(15,0,'string','string'),(99,23,'Bistec','Platos');
";
            CrearTabla(ConnectionString, consultaArticulos, "Articulos");

            string consultaFacturas = @"CREATE TABLE IF NOT EXISTS facturas (
  id int(11) NOT NULL,
  total float DEFAULT NULL,
  activa int(11) DEFAULT NULL,
  mesa int(11) NOT NULL,
  PRIMARY KEY (id)
);
";
            CrearTabla(ConnectionString, consultaFacturas, "Facturas");

            string consultaPedidos = @"CREATE TABLE IF NOT EXISTS pedidos (
  factura int(11) NOT NULL,
  id int(11) NOT NULL,
  fecha varchar(10) DEFAULT NULL,
  estado varchar(12) DEFAULT NULL,
  mesa int(11) NOT NULL,
  PRIMARY KEY (id),
  KEY mesa (mesa),
  KEY factura (factura),
  CONSTRAINT pedidos_ibfk_3 FOREIGN KEY (mesa) REFERENCES mesas(ID) ON DELETE CASCADE,
  CONSTRAINT pedidos_ibfk_4 FOREIGN KEY (factura) REFERENCES facturas(id) ON DELETE CASCADE
);
";
            CrearTabla(ConnectionString, consultaPedidos, "Pedidos");

            string consultaInstancias = @"CREATE TABLE IF NOT EXISTS instanciaarticulos (
  idArticulo int(11) NOT NULL,
  idPedido int(11) NOT NULL,
  cantidad int(11) DEFAULT NULL,
  PRIMARY KEY (idArticulo,idPedido),
  KEY idPedido (idPedido),
  CONSTRAINT instanciaarticulos_ibfk_3 FOREIGN KEY (idArticulo) REFERENCES articulos(id) ON DELETE CASCADE,
  CONSTRAINT instanciaarticulos_ibfk_4 FOREIGN KEY (idPedido) REFERENCES pedidos(id) ON DELETE CASCADE
);

";
            CrearTabla(ConnectionString, consultaInstancias, "InstanciaArticulos");
        }

        private static void CrearTabla(string connectionString, string createTableQuery, string nombreTabla)
        {
            // Utilizamos 'using' para asegurar que la conexión se cierre correctamente una vez que terminemos.
            // 'using' gestiona automáticamente el ciclo de vida del objeto (en este caso, MySqlConnection).
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    // Abrimos la conexión con la base de datos.
                    connection.Open();

                    // Ahora ejecutamos la consulta SQL en la base de datos a través de MySQLCommand.
                    using (var cmd = new MySqlCommand(createTableQuery, connection))
                    {
                        // Ejecutamos la consulta. 'ExecuteNonQuery' se utiliza porque no esperamos resultados (es una consulta de tipo 'CREATE').
                        cmd.ExecuteNonQuery();

                        // Si la consulta se ejecutó sin problemas, escribimos un mensaje en el log indicando que la tabla fue creada o ya existía.
                        Trace.WriteLine("Tabla "+nombreTabla+" creada o ya existe.");
                    }
                }
                catch (MySqlException ex)
                {
                    // Si ocurre un error relacionado con MySQL, capturamos esta excepción.
                    Trace.WriteLine("Error relacionado con MySQL: " + ex.Message);
                    // Aquí puedes manejar otros detalles como el código de error: ex.ErrorCode, ex.Number
                }
                catch (InvalidOperationException ex)
                {
                    // Si ocurre un error al operar sobre la conexión, como un estado inválido.
                    Trace.WriteLine("Error de operación inválida: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Si ocurre cualquier otro error no capturado por los bloques anteriores, lo capturamos aquí.
                    Trace.WriteLine("Error inesperado: " + ex.Message);
                }
            }
        }

        public static void InsertarRegistrosRol()
        {
            List<string> roles = new List<string>{ "Empleado", "Gerente" };
            foreach (string rol in roles)
            {
                // Si no existe el rol, se crea
                if (!RolController.Existe(rol))
                {
                    RolController.InsertarRegistroRol(ConnectionString, rol);
                }
                else
                {
                    Trace.WriteLine("El rol " + rol + " ya existe.");
                }
            }
        }

                
    }
}

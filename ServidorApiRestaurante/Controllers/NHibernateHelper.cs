using FluentNHibernate.Cfg;
using NHibernate;
using FluentNHibernate.Cfg.Db;
using ServidorApiRestaurante.Mappings;


namespace ServidorApiRestaurante.Controllers
{
    public static class NHibernateHelper
    {
        private static ISessionFactory? sessionFactory;  // El SessionFactory que se va a crear de forma perezosa (Lazy).
        private static readonly object LockObject = new object();  // Objeto de bloqueo para asegurar que solo un hilo pueda crear el SessionFactory.

        // Propiedad que devuelve el SessionFactory, inicializándolo si es necesario.
        public static ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                    InitializeSessionFactory();  // Si el SessionFactory no ha sido creado, lo inicializamos.
                return sessionFactory;  // Devuelve el SessionFactory ya creado.
            }
        }

        // Método privado para inicializar el SessionFactory.
        private static void InitializeSessionFactory()
        {
            if (sessionFactory == null)
            {
                lock (LockObject)  // Usamos el bloqueo para asegurarnos de que solo un hilo lo cree.
                {
                    if (sessionFactory == null) // Doble verificación de null para seguridad en hilos.
                    {
                        try
                        {
                            // Configuración de NHibernate usando Fluent NHibernate.
                            sessionFactory = Fluently.Configure()
                                .Database(MySQLConfiguration.Standard
                                    .ConnectionString("Server=localhost;Database=tu_basedatos;User Id=tu_usuario;Password=tu_contraseña;"))  // Cadena de conexión a tu base de datos.
                                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<TrabajadorMap>())  // Agregamos los mapeos de las clases (como TrabajadorMap).
                                .BuildSessionFactory();  // Construye el SessionFactory.
                        }
                        catch (Exception ex)
                        {
                            // Si ocurre un error, lo capturamos y lo mostramos.
                            Console.WriteLine("Error al configurar NHibernate: " + ex.Message);
                            throw;  // Lanzamos la excepción nuevamente para evitar que el código siga ejecutándose sin una configuración válida.
                        }
                    }
                }
            }
        }

        // Método para abrir una sesión.
        public static NHibernate.ISession OpenSession()
        {
            return SessionFactory.OpenSession();  // Devuelve una nueva sesión utilizando el SessionFactory.
        }

        // Método para cerrar una sesión de manera segura.
        public static void CloseSession(NHibernate.ISession session)
        {
            session?.Dispose();  // Si la sesión no es nula, se cierra correctamente (liberando recursos).
        }
    }
}

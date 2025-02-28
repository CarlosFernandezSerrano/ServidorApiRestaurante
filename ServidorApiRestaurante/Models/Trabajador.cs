
namespace ServidorApiRestaurante.Models
{
    public class Trabajador
    {
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; } = string.Empty;
        public virtual string HashContraseña { get; set; } = string.Empty; // Se guarda en la BDD
        public virtual Rol? Rol { get; set; } // Relación con Rol
        public virtual Restaurante? Restaurante { get; set; } // Relación con Restaurante

        // Propiedad no persistida en la BDD
        public virtual string Contraseña
        {
            set
            {
                if (string.IsNullOrEmpty(HashContraseña)) // Solo si está vacío
                {
                    HashContraseña = HashearContraseña(value);
                }
            }
        }

        // Constructor sin parámetros requerido por NHibernate
        public Trabajador() { }

        public Trabajador(string nombre, string contraseña, Rol? rol = null, Restaurante? restaurante = null)
        {
            Nombre = nombre;
            Contraseña = contraseña; // Se almacena el hash automáticamente
            Rol = rol;
            Restaurante = restaurante;
        }

        public Trabajador(int id, string nombre, string contraseña, Rol? rol = null, Restaurante? restaurante = null)
        {
            Id = id;
            Contraseña = contraseña; // Se almacena el hash automáticamente
            Nombre = nombre;
            Rol = rol;
            Restaurante = restaurante;
        }

        private static string HashearContraseña(string contraseña)
        {
            return BCrypt.Net.BCrypt.HashPassword(contraseña);
        }

        //Devuelve true si la contraseña es correcta.
        public bool VerificarContraseña(string contraseñaIngresada)
        {
            return BCrypt.Net.BCrypt.Verify(contraseñaIngresada, this.HashContraseña);
        }

    }
}

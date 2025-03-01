
namespace ServidorApiRestaurante.Models
{
    public class Trabajador
    {
        /*public virtual int Id { get; set; }
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

        */

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string HashContraseña { get; set; } = string.Empty;
        public int Rol_Id { get; set; }
        public int Restaurante_ID { get; set; }

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

        public Trabajador(string nombre, string contraseña, int rol_Id, int restaurante_Id)
        {
            this.Nombre = nombre;
            this.Contraseña = contraseña; // Se almacena el hash automáticamente
            this.Rol_Id = rol_Id;
            this.Restaurante_ID = restaurante_Id;
        }

        public Trabajador(int id, string nombre, string contraseña, int rol_Id, int restaurante_Id)
        {
            this.Id = id;
            this.Nombre = nombre;
            this.Contraseña = contraseña; // Se almacena el hash automáticamente
            this.Rol_Id = rol_Id;
            this.Restaurante_ID = restaurante_Id;
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

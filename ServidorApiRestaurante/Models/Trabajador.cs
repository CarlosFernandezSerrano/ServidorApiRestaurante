
using System.Text.Json.Serialization;

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
        public string Password { get; set; } = string.Empty;
        public int Rol_Id { get; set; }
        public int Restaurante_Id { get; set; }


        // Constructor sin parámetros requerido para la deserialización
        //public Trabajador() { }

        public Trabajador(string nombre, string contraseña, int rol_Id, int restaurante_Id)
        {
            this.Nombre = nombre;
            this.Password = contraseña; 
            this.Rol_Id = rol_Id;
            this.Restaurante_Id = restaurante_Id;
        }

        [JsonConstructor]
        public Trabajador(int id, string nombre, string password, int rol_Id, int restaurante_Id)
        {
            this.Id = id;
            this.Nombre = nombre;
            this.Password = password;
            this.Rol_Id = rol_Id;
            this.Restaurante_Id = restaurante_Id;
        }


    }
}

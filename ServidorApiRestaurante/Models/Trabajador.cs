
using ServidorApiRestaurante.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServidorApiRestaurante.Model
{
    public class Trabajador
    {
        [Key] // Clave primaria
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; } = string.Empty;
        [NotMapped] // Esta propiedad no se guarda en la BDD
        public virtual string Contraseña { get; set; } = string.Empty;
        public virtual Rol? Rol { get; set; } // Relación con Rol
        public virtual Restaurante? Restaurante { get; set; } // Relación con Restaurante

        public virtual string HashContraseña { get; set; } = string.Empty; // Se guarda en la BDD

        // Constructor sin parámetros requerido por NHibernate
        public Trabajador() { }

        public Trabajador(string nombre, string contraseña, Rol rol, Restaurante restaurante)
        {
            this.Nombre = nombre;
            this.Contraseña = contraseña;
            this.HashContraseña = HashearContraseña(contraseña); // Se almacena el hash
            this.Rol = rol;
            this.Restaurante = restaurante;
        }        

        public Trabajador(int id, string nombre, string contraseña, Rol rol, Restaurante restaurante)
        {
            this.Id = id;
            this.Nombre = nombre;
            this.Contraseña = contraseña;
            this.HashContraseña = HashearContraseña(contraseña); // Se almacena el hash
            this.Rol = rol;
            this.Restaurante = restaurante;
        }

        private string HashearContraseña(string contraseña)
        {
            return BCrypt.Net.BCrypt.HashPassword(contraseña);
        }

        public bool VerificarContraseña(string contraseñaIngresada)
        {
            return BCrypt.Net.BCrypt.Verify(contraseñaIngresada, this.HashContraseña);
        }

    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServidorApiRestaurante.Models
{
    public class Rol
    {
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; } = string.Empty;

        // Opcional: Podrías agregar otras propiedades en el futuro

        // Constructor sin parámetros requerido por NHibernate
        public Rol() { }

        public Rol(string nombre)
        {
            this.Nombre = nombre;
        }

        // Constructor con parámetros
        public Rol(int id, string nombre)
        {
            this.Id = id;
            this.Nombre = nombre;
        }
    }
}

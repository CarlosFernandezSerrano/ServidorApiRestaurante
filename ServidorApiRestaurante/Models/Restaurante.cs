using System.ComponentModel.DataAnnotations;

namespace ServidorApiRestaurante.Models
{
    public class Restaurante
    {
        [Key] // Clave primaria
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; } = string.Empty;
        public virtual string Hora_apertura { get; set; } = string.Empty;
        public virtual string Hora_cierre { get; set; } = string.Empty;


        // Constructor sin parámetros requerido por NHibernate
        public Restaurante() {}

        public Restaurante(string nombre, string hora_apertura, string hora_cierre)
        {
            this.Nombre = nombre;
            this.Hora_apertura = hora_apertura;
            this.Hora_cierre = hora_cierre;
        }

        public Restaurante(int Id, string nombre, string hora_apertura, string hora_cierre)
        {
            this.Id = Id;
            this.Nombre = nombre;
            this.Hora_apertura = hora_apertura;
            this.Hora_cierre = hora_cierre;
        }


    }
}

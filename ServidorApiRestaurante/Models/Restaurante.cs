
using System.Text.Json.Serialization;

namespace ServidorApiRestaurante.Models
{
    public class Restaurante
    {
        /*public virtual int Id { get; set; }
        public virtual string Nombre { get; set; } = string.Empty;
        public virtual string HoraApertura { get; set; } = string.Empty;
        public virtual string HoraCierre { get; set; } = string.Empty;


        // Constructor sin parámetros requerido por NHibernate
        public Restaurante() {}*/

        public int Id { get; set; }
        public string Nombre { get; set; } 
        public string HoraApertura { get; set; }
        public string HoraCierre { get; set; }
        public List<Mesa> Mesas { get; set; } = new List<Mesa>();
        public List<Trabajador> Trabajadores { get; set; } = new List<Trabajador>();

        public Restaurante(string nombre, string horaApertura, string horaCierre, List<Mesa> mesas, List<Trabajador> trabajadores)
        {
            this.Nombre = nombre;
            this.HoraApertura = horaApertura;
            this.HoraCierre = horaCierre;
            this.Mesas = mesas;
            this.Trabajadores = trabajadores;
        }

        [JsonConstructor]
        public Restaurante(int Id, string nombre, string horaApertura, string horaCierre, List<Mesa> mesas, List<Trabajador> trabajadores)
        {
            this.Id = Id;
            this.Nombre = nombre;
            this.HoraApertura = horaApertura;
            this.HoraCierre = horaCierre;
            this.Mesas = mesas;
            this.Trabajadores = trabajadores;
        }


    }
}

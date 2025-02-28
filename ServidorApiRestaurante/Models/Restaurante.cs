
namespace ServidorApiRestaurante.Models
{
    public class Restaurante
    {
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; } = string.Empty;
        public virtual string HoraApertura { get; set; } = string.Empty;
        public virtual string HoraCierre { get; set; } = string.Empty;


        // Constructor sin parámetros requerido por NHibernate
        public Restaurante() {}

        public Restaurante(string nombre, string horaApertura, string horaCierre)
        {
            this.Nombre = nombre;
            this.HoraApertura = horaApertura;
            this.HoraCierre = horaCierre;
        }

        public Restaurante(int Id, string nombre, string horaApertura, string horaCierre)
        {
            this.Id = Id;
            this.Nombre = nombre;
            this.HoraApertura = horaApertura;
            this.HoraCierre = horaCierre;
        }


    }
}

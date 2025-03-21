using System.Text.Json.Serialization;

namespace ServidorApiRestaurante.Models
{
    public enum EstadoReserva
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    public class Reserva
    {
        /*public virtual int Id { get; set; }
        public virtual string Fecha { get; set; } = string.Empty; // "YYYY-MM-DD"
        public virtual string Hora { get; set; } = string.Empty; // "HH:mm:ss"
        public virtual EstadoReserva Estado { get; set; }
        public virtual Cliente? Cliente { get; set; }
        public virtual Mesa? Mesa { get; set; }

        // Constructor sin parámetros requerido por NHibernate
        public Reserva() { }*/

        public int Id { get; set; }
        public string Fecha { get; set; } // "YYYY-MM-DD"
        public string Hora { get; set; } // "HH:mm:ss"
        public string Estado { get; set; }
        public int Cliente_Id { get; set; }
        public int Mesa_Id { get; set; }

        public Reserva(string fecha, string hora, string estado, int cliente_id, int mesa_id)
        {
            this.Fecha = fecha;
            this.Hora = hora;
            this.Estado = estado;
            this.Cliente_Id = cliente_id;
            this.Mesa_Id = mesa_id;
        }

        [JsonConstructor]
        public Reserva(int id, string fecha, string hora, string estado, int cliente_id, int mesa_id)
        {
            this.Id = id;
            this.Fecha = fecha;
            this.Hora = hora;
            this.Estado = estado;
            this.Cliente_Id = cliente_id;
            this.Mesa_Id = mesa_id;
        }
    }
}

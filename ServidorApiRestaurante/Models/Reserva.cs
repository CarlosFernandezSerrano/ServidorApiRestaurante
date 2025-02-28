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
        public virtual int Id { get; set; }
        public virtual string Fecha { get; set; } = string.Empty; // "YYYY-MM-DD"
        public virtual string Hora { get; set; } = string.Empty; // "HH:mm:ss"
        public virtual EstadoReserva Estado { get; set; }
        public virtual Cliente? Cliente { get; set; }
        public virtual Mesa? Mesa { get; set; }

        // Constructor sin parámetros requerido por NHibernate
        public Reserva() { }

        public Reserva(string fecha, string hora, EstadoReserva estado, Cliente? cliente = null, Mesa? mesa = null)
        {
            this.Fecha = fecha;
            this.Hora = hora;
            this.Cliente = cliente;
            this.Estado = estado;
            this.Mesa = mesa;
        }

        public Reserva(int id, string fecha, string hora, EstadoReserva estado, Cliente? cliente = null, Mesa? mesa = null)
        {
            this.Id = id;
            this.Fecha = fecha;
            this.Hora = hora;
            this.Cliente = cliente;
            this.Estado = estado;
            this.Mesa = mesa;
        }
    }
}

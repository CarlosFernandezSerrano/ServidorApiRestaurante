using System.Text.Json.Serialization;

namespace ServidorApiRestaurante.Models
{
    public enum EstadoReserva
    {
        Terminada, // Cuando acaba una reserva
        Pendiente, // Para cuando la reserva esté en uso
        Confirmada, // Para cuando se crea una reserva, pero aún no es el día y la hora
        Cancelada // Cuando se cancela una reserva
    }

    public class Reserva
    {
        public int Id { get; set; }
        public string Fecha { get; set; } // "YYYY-MM-DD"
        public string Hora { get; set; } // "HH:mm:ss"
        public string Estado { get; set; }
        public int CantComensales { get; set; }
        public int Cliente_Id { get; set; }
        public int Mesa_Id { get; set; }
        public Cliente Cliente { get; set; }

        public Reserva(string fecha, string hora, string estado, int cantComensales, int cliente_id, int mesa_id, Cliente cliente)
        {
            this.Fecha = fecha;
            this.Hora = hora;
            this.Estado = estado;
            this.CantComensales = cantComensales;
            this.Cliente_Id = cliente_id;
            this.Mesa_Id = mesa_id;
            this.Cliente = cliente;
        }

        [JsonConstructor]
        public Reserva(int id, string fecha, string hora, string estado, int cantComensales, int cliente_id, int mesa_id, Cliente cliente)
        {
            this.Id = id;
            this.Fecha = fecha;
            this.Hora = hora;
            this.Estado = estado;
            this.CantComensales = cantComensales;
            this.Cliente_Id = cliente_id;
            this.Mesa_Id = mesa_id;
            this.Cliente = cliente;
        }
    }
}

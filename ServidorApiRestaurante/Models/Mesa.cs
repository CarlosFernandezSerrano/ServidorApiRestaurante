
using System.Text.Json.Serialization;

namespace ServidorApiRestaurante.Models
{
    public class Mesa
    {
        public int Id { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public int CantPers { get; set; }
        public bool Disponible { get; set; }
        public int Restaurante_ID { get; set; }
        public List<Reserva> Reservas { get; set; } = new List<Reserva>();

        public Mesa(float posX, float posY, float width, float height, float scaleX, float scaleY, int cantPers, bool disponible, int restaurante_Id, List<Reserva> reservas)
        {
            this.PosX = posX;
            this.PosY = posY;
            this.Width = width;
            this.Height = height;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.CantPers = cantPers;
            this.Disponible = disponible;
            this.Restaurante_ID = restaurante_Id;
            this.Reservas = reservas;
        }

        [JsonConstructor]
        public Mesa(int id, float posX, float posY, float width, float height, float scaleX, float scaleY, int cantPers, bool disponible, int restaurante_Id, List<Reserva> reservas)
        {
            this.Id = id;
            this.PosX = posX;
            this.PosY = posY;
            this.Width = width;
            this.Height = height;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.CantPers = cantPers;
            this.Disponible = disponible;
            this.Restaurante_ID = restaurante_Id;
            this.Reservas = reservas;
        }

        public string Mostrar()
        {
            return this.Id + " " + this.PosX + " " + this.PosY + " " + this.ScaleX + " " + this.ScaleY + " " + this.CantPers + " " + this.Disponible + " " + this.Restaurante_ID;
        }
    }
}

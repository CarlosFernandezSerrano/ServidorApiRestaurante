
using System.Text.Json.Serialization;

namespace ServidorApiRestaurante.Models
{
    public class Mesa
    {
        /*public virtual int Id { get; set; }
        public virtual float PosX { get; set; }
        public virtual float PosY { get; set; }
        public virtual float ScaleX { get; set; }
        public virtual float ScaleY { get; set; }
        public virtual bool Disponible { get; set; }
        public virtual Restaurante? Restaurante { get; set; }


        public Mesa() { }*/

        public int Id { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public bool Disponible { get; set; }
        public int Restaurante_ID { get; set; }

        public Mesa(float posX, float posY, float width, float height, float scaleX, float scaleY, bool disponible, int restaurante_Id)
        {
            this.PosX = posX;
            this.PosY = posY;
            this.Width = width;
            this.Height = height;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.Disponible = disponible;
            this.Restaurante_ID = restaurante_Id;
        }

        [JsonConstructor]
        public Mesa(int id, float posX, float posY, float width, float height, float scaleX, float scaleY, bool disponible, int restaurante_Id)
        {
            this.Id = id;
            this.PosX = posX;
            this.PosY = posY;
            this.Width = width;
            this.Height = height;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.Disponible = disponible;
            this.Restaurante_ID = restaurante_Id;
        }
    }
}

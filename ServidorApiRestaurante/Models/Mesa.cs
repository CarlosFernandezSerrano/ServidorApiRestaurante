
namespace ServidorApiRestaurante.Models
{
    public class Mesa
    {
        public virtual int Id { get; set; }
        public virtual float PosX { get; set; }
        public virtual float PosY { get; set; }
        public virtual float ScaleX { get; set; }
        public virtual float ScaleY { get; set; }
        public virtual bool Disponible { get; set; }
        public virtual Restaurante? Restaurante { get; set; }


        public Mesa() { }

        public Mesa(float posX, float posY, float scaleX, float scaleY, bool disponible, Restaurante? restaurante = null)
        {
            this.PosX = posX;
            this.PosY = posY;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.Disponible = disponible;
            this.Restaurante = restaurante;
        }

        public Mesa(int id, float posX, float posY, float scaleX, float scaleY, bool disponible, Restaurante? restaurante = null)
        {
            this.Id = id;
            this.PosX = posX;
            this.PosY = posY;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.Disponible = disponible;
            this.Restaurante = restaurante;
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServidorApiRestaurante.Models
{
    public class Mesa
    {
        [Key] // Clave primaria
        public virtual int Id { get; set; }

        [Required] // La propiedad 'Nombre' es obligatoria
        [Column(TypeName = "TEXT")] //Float
        public virtual float PosX { get; set; }

        [Required] // La propiedad 'Nombre' es obligatoria
        [Column(TypeName = "TEXT")] //Float
        public virtual float PosY { get; set; }

        [Required] // La propiedad 'Nombre' es obligatoria
        [Column(TypeName = "TEXT")] //Float
        public virtual float ScaleX { get; set; }

        [Required] // La propiedad 'Nombre' es obligatoria
        [Column(TypeName = "TEXT")] //Float
        public virtual float ScaleY { get; set; }

        [Required] // La propiedad 'Dni' es obligatoria
        [Column(TypeName = "BOOL")]
        public virtual bool Disponible { get; set; }

        [Column(TypeName = "INTEGER")]
        public virtual Restaurante? Restaurante { get; set; }


        public Mesa() { }

        public Mesa(float PosX, float PosY, float ScaleX, float ScaleY, bool Disponible, Restaurante Restaurante)
        {
            this.PosX = PosX;
            this.PosY = PosY;
            this.ScaleY = ScaleY;
            this.ScaleY = ScaleY;
            this.Disponible = Disponible;
            this.Restaurante = Restaurante;
        }

        public Mesa(int Id, float PosX, float PosY, float ScaleX, float ScaleY, bool Disponible, Restaurante Restaurante)
        {
            this.Id = Id;
            this.PosX = PosX;
            this.PosY = PosY;
            this.ScaleY = ScaleY;
            this.ScaleY = ScaleY;
            this.Disponible = Disponible;
            this.Restaurante = Restaurante;
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServidorApiRestaurante.Models
{
    public class Cliente
    {
        [Key] // Clave primaria
        public virtual int Id { get; set; }

        [Required] // La propiedad 'Nombre' es obligatoria
        [Column(TypeName = "TEXT")]
        public virtual string Nombre { get; set; } = string.Empty;

        [Required] // La propiedad 'Dni' es obligatoria
        [Column(TypeName = "TEXT")]
        public virtual string Dni { get; set; } = string.Empty;

        [Column(TypeName = "VARCHAR(15)")]
        public virtual string Num_Teléfono { get; set; } = string.Empty;


        public Cliente() {}
        
        public Cliente(string Nombre, string Dni, string num_Teléfono)
        {
            this.Nombre = Nombre;
            this.Dni = Dni;
            this.Num_Teléfono = num_Teléfono;
        }

        public Cliente(int Id, string Nombre, string Dni, string num_Teléfono)
        {
            this.Id = Id;
            this.Nombre = Nombre;
            this.Dni = Dni;
            this.Num_Teléfono = num_Teléfono;
        }


    }
}

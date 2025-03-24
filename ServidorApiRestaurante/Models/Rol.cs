using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServidorApiRestaurante.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public Rol(string nombre)
        {
            this.Nombre = nombre;
        }

        [JsonConstructor]
        public Rol(int id, string nombre)
        {
            this.Id = id;
            this.Nombre = nombre;
        }
    }
}

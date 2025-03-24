
using System.Text.Json.Serialization;

namespace ServidorApiRestaurante.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Dni { get; set; }
        public string NumTelefono { get; set; }

        public Cliente(string nombre, string dni, string numTelefono)
        {
            this.Nombre = nombre;
            this.Dni = dni;
            this.NumTelefono = numTelefono;
        }

        [JsonConstructor]
        public Cliente(int id, string nombre, string dni, string numTelefono)
        {
            this.Id = id;
            this.Nombre = nombre;
            this.Dni = dni;
            this.NumTelefono = numTelefono;
        }


    }
}

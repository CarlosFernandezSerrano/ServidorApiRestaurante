
namespace ServidorApiRestaurante.Models
{
    public class Cliente
    {
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; } = string.Empty;
        public virtual string Dni { get; set; } = string.Empty;
        public virtual string NumTelefono { get; set; } = string.Empty;


        public Cliente() {}

        public Cliente(string nombre, string dni, string numTelefono)
        {
            this.Nombre = nombre;
            this.Dni = dni;
            this.NumTelefono = numTelefono;
        }

        public Cliente(int id, string nombre, string dni, string numTelefono)
        {
            this.Id = id;
            this.Nombre = nombre;
            this.Dni = dni;
            this.NumTelefono = numTelefono;
        }


    }
}

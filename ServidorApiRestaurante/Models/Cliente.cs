namespace ServidorApiRestaurante.Model
{
    public class Cliente
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public string edad { get; set; }
        public string correo { get; set; }

        public Cliente(string id, string nombre, string edad, string correo)
        {
            this.id = id;
            this.nombre = nombre;
            this.edad = edad;
            this.correo = correo;
        }
    }
}

namespace ServidorApiRestaurante.Model
{
    public class Cliente
    {
        public string nombre { get; set; }
        public string contraseña { get; set; }
        public string rol { get; set; }
        public string cantMapas { get; set; }
        public List<string> restaurantes { get; set; }

        public Cliente(string nombre, string contraseña, string rol, string cantMapas, List<string> restaurantes)
        {
            this.nombre = nombre;
            this.contraseña = contraseña;
            this.rol = rol;
            this.cantMapas = cantMapas;
            this.restaurantes = restaurantes;
        }


    }
}

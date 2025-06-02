using System.Text.Json.Serialization;
public class Articulo
{
    public int id { get; set; }
    public string nombre { get; set; }
    public float precio { get; set; }
    public string categoria {  get; set; }

    [JsonConstructor]
    public Articulo(int id, string nombre, float precio, string categoria)
    {
        this.id = id;
        this.nombre = nombre;
        this.precio = precio;
        this.categoria = categoria;
    }
    public string Mostrar()
    {
        return this.id + " " + this.nombre + " " + this.precio+" "+this.categoria;
    }
}

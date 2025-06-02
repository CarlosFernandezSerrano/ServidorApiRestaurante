using System.Text.Json.Serialization;

public class InstanciaArticulo
{
    public int idArticulo { get; set; }
    public int idPedido { get; set; }
    public int cantidad { get; set; }
    public float precio {  get; set; }
    public Articulo articulo;

    public InstanciaArticulo(int idArt, int idPedido, int cantidad, float precio)
    {
        this.idArticulo = idArt;
        this.idPedido = idPedido;
        this.cantidad = cantidad;
        this.precio = precio;
    }

    public string Mostrar()
    {
        return this.idArticulo + " " + this.idPedido + " " + this.cantidad+" "+this.precio+" "+articulo.Mostrar();
    }
	
    [JsonConstructor]
    public InstanciaArticulo(Articulo art, int idPedido, int cantidad, float precio)
    {
        this.idArticulo = art.id;
        this.idPedido = idPedido;
        this.cantidad = cantidad; 
        this.precio = precio;
        this.articulo = art;
    }
    public string getNombre()
    {
        return articulo.nombre;
    }
}

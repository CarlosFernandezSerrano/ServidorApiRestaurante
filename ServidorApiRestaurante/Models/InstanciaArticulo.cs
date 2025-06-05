using System.Text.Json.Serialization;
public class InstanciaArticulo
{
    public int idArticulo { get; set; }
    public int idPedido { get; set; }
    public int cantidad { get; set; }

    [JsonConstructor]
    public InstanciaArticulo(int idArticulo, int idPedido, int cantidad)
    {
        this.idArticulo = idArticulo;
        this.idPedido = idPedido;
        this.cantidad = cantidad;
    }

    public string Mostrar()
    {
        return this.idArticulo + " " + this.idPedido + " " + this.cantidad;
    }
    /*public InstanciaArticulo(Articulo art, int idPedido, int cantidad, float precio)
    {
        this.idArticulo = art.id;
        this.idPedido = idPedido;
        this.cantidad = cantidad; 
        this.precio = precio;
        this.articulo = art;
    }*/
    /*public string getNombre()
    {
        return articulo.nombre;
    }*/
}

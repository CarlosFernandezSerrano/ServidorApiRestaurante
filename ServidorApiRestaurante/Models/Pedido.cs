using System.Text.Json.Serialization;
public enum EstadoPedido
{
    Apuntado,
    EnCocina,
    Completado,
    Pagado
}
//Tal vez no haga falta tener el enum sin utilizar, pero mejor asegurarse
public class Pedido
{
    public int id { get; set; }
    public string fecha { get; set; }
    public int mesa { get; set; }
    public string estado { get; set; }
    public int factura { get; set; }
    //public List<InstanciaArticulo> listaArticulos { get; set; }

    [JsonConstructor]
    public Pedido(int id, string fecha, int mesa, string estado, int factura)
    {
        this.id = id;
        this.fecha = fecha;
        this.estado = estado;
        this.mesa = mesa;
        this.factura = factura;
        //this.listaArticulos = new List<InstanciaArticulo>();
        //this.factura = factura;
    }


    /*[JsonConstructor]
    public Pedido(int id, string fecha, int mesa, string estado, List<InstanciaArticulo> articulos, Factura factura)
    {
        this.id = id;
        this.fecha = fecha;
        this.mesa = mesa;
        this.listaArticulos = articulos;
        this.factura = factura;
        this.estado = estado;
    }*/


    public EstadoPedido getEstado(string e)
    {
        if (e.ToUpper().Equals("ENCOCINA"))
            return EstadoPedido.EnCocina;
        else if (e.ToUpper().Equals("COMPLETADO"))
            return EstadoPedido.Completado;
        else if (e.ToUpper().Equals("PAGADO"))
            return EstadoPedido.Pagado;
        else return EstadoPedido.Apuntado;
    }
    /*
    REALIZAR ESTO MEDIANTE UNA PETICIÓN AL SERVIDOR
    public string MostrarLista()
    { 
        string str = "lista:";
        /*foreach (InstanciaArticulo art in listaArticulos)
            str += art.Mostrar() + "\n";
        return str;
    }
    */

    public string Mostrar()
    {
        return "id:" + this.id + " " + "fecha:" + this.fecha + " " + "mesa:" + this.mesa + " " + "estado:" + this.estado+ " "+this.factura;
    }

    /*
    REALIZAR TODO ESTO MEDIANTE PETICIONES AL SERVIDOR
    public void AddArticulo(InstanciaArticulo art)
    {
        listaArticulos.Add(art);
    }

    public void RemoveArticulo(InstanciaArticulo art)
    {
        listaArticulos.Remove(art);
    }

    public void RemoveArticulo(int id)
    {
        foreach (InstanciaArticulo art in listaArticulos)
        {
            if (art.idArticulo == id && art.idPedido == this.id)
                listaArticulos.Remove(art);
        }
    }
    public InstanciaArticulo getArticulo(int idArt)
    {
        foreach (InstanciaArticulo a in listaArticulos)
        {
            if (a.idArticulo == idArt) return a;
        }
        return null;
    }
    public float totalPedido()
    {
        float total = 0;
        foreach (InstanciaArticulo art in listaArticulos)
        {
            total += art.precio * art.cantidad;
        }
        return total;
    }
    /*public string listarArticulos()
    {
        string s = "";
        foreach (InstanciaArticulo a in listaArticulos)
        {
            s += a.getNombre() + " " + a.cantidad + "\n";
        }
        return s;
    }*/
}

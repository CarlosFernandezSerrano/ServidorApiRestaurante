using System.Text.Json.Serialization;

public class Factura
{
    public int id { get; set; }
    public float total {  get; set; }
    public bool activa { get; set; }
    public int mesa { get; set; }
    //public List<Pedido> listaPedidos { get; private set; }

    /*[JsonConstructor]
    public Factura(int id, float total, List<Pedido> listaPedidos)
    {
		this.id = id;
		this.total=total;
        this.listaPedidos = listaPedidos;
    }*/
    [JsonConstructor]
    public Factura(int id,float total,bool activa, int mesa)
    {
        this.id=id;
        this.total = total;
        this.activa = activa;
        this.mesa = mesa;
    }
    /* HACER EN SERVIDOR
    public string MostrarPedidos()
    {
        string str= "";
        foreach (Pedido p in this.listaPedidos)
        {
            str += p.Mostrar()+"\n";
        }
        return str;
    }*/
    public string Mostrar()
    {
        return "" + id+" "+total+" "+activa;
    }
    /*
    HACER EN SERVIDOR
    public void calcularTotal()
    {
            this.total = 0;
        foreach (Pedido p in listaPedidos)
        {
            total += p.totalPedido();
        }
    }
	public void addPedido(Pedido p){
		listaPedidos.Add(p);
	}*/
}

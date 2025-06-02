using System.Text.Json.Serialization;

public class Factura
{
    public int id { get; private set; }
    public float total {  get; private set; }
    public List<Pedido> listaPedidos { get; private set; }

    [JsonConstructor]
    public Factura(int id, float total, List<Pedido> listaPedidos)
    {
		this.id = id;
		this.total=total;
        this.listaPedidos = listaPedidos;
    }
    public Factura(int id)
    {
        this.listaPedidos = new List<Pedido>();
        this.total = 0;
    }
    public string MostrarPedidos()
    {
        string str= "";
        foreach (Pedido p in this.listaPedidos)
        {
            str += p.Mostrar()+"\n";
        }
        return str;
    }
    public string Mostrar()
    {
        return "" + id + " " + total + MostrarPedidos();
    }
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
	}
}

using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class GestionarPedidosController : MonoBehaviour
{
    //se debe marcar la categoría seleccionada
    public RectTransform fondoPedidos;
    public GestionarPedidosController instanceGestionarPedidosController;
    public MétodosAPIController instanceMétodosApiController;
    public TextMeshProUGUI titulo;
    public TextMeshProUGUI articulosPedidos;
    public TextMeshProUGUI selMesa;
    public Articulo articuloPrueba;
    public Pedido pedido;
    public Factura factura;
    public GestionarFacturasController instanceGestionarFacturasController;
    public GameObject canvasFacturas;
    public Sprite imagenPrueba;
    public int idMesa;
    void Awake()
    {
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;
        if (instanceGestionarPedidosController == null)
        {
            instanceGestionarPedidosController = this;
        }
        //CAMBIAR COMO OBTENER MESA
        //Mesa m = new Mesa(1, 0, 0, 0, 0, 0, 0, 0, true, 1, null);
        idMesa = 1;
        //SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
        articuloPrueba = new Articulo(1,"coca cola",(float)1.50,"bebidas");
        factura = new Factura(1);
        pedido = new Pedido(1,"16:00",1,factura);
        addArticulo(articuloPrueba);
    }
    void Start()
    {
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;

        TrabajadorController.ComprobandoDatosTrabajador = false;


    }
    public void cambiarMesa()
    {
        string str2 = "";
        var matches = Regex.Matches(selMesa.text, @"\d+");
        foreach (var match in matches)
        {
            str2 += match;
        }
        idMesa = int.Parse(str2);
        titulo.text = ""+idMesa;
    }
    // Start is called before the first frame update
    public void volverAMain()
    {
        SceneManager.LoadScene("Main");
    }


    public void crearBotonArticulo()
    {
        GameObject boton = new GameObject("Articulo-" + 5);
        boton.transform.SetParent(fondoPedidos);
        RectTransform rt = boton.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 200);
        boton.AddComponent<CanvasRenderer>();
        UnityEngine.UI.Image imagen = boton.AddComponent<UnityEngine.UI.Image>();
        imagen.sprite = imagenPrueba;
    }


    public void addArticuloPrueba()
    {
        addArticulo(articuloPrueba);
    }

    public void addArticulo(Articulo a)
    {
        InstanciaArticulo art = new InstanciaArticulo(a,pedido.id,1,10);
        bool existe = false;
        foreach (InstanciaArticulo ar in pedido.listaArticulos)
        {
            if (art.idArticulo == ar.idArticulo)
            {
                existe = true;
                art = ar;
            }
        }
        if (!existe)
        {
            pedido.AddArticulo(art);
        }
        else
        {
            art.cantidad = art.cantidad + 1;
        }
        articulosPedidos.text = pedido.listarArticulos();
    }

    public void pasarAFacturas()
    {
        instanceGestionarFacturasController.pedido = pedido;
        instanceGestionarFacturasController.factura = factura;
        canvasFacturas.SetActive(true);
    }
    
    private async Task registrarPedido(Pedido p)
    {
        string cad = await instanceMétodosApiController.PostDataAsync("pedido/crearPedido",p);
        //Tal vez se deba usar int en vez de Result
        //En principio no se usará la respuesta para nada, salvo comprobar si el programa funciona correctamente
        //Resultado resultado=JsonConvert.DeserializeObject<Resultado>(cad);
    }

    public void finalizarPedido()
    {
        registrarPedido(articuloPrueba);
    }

}

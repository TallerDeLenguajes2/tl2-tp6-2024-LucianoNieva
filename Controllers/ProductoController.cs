using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_LucianoNieva.Models;
using repoProduct;
using producto;

namespace tl2_tp6_2024_LucianoNieva.Controllers;

public class ProductoControllers : Controller
{
    private readonly ILogger<ProductoControllers> _logger;
    private readonly RepoProduct RepositorioProducto;

    public ProductoControllers(ILogger<ProductoControllers> logger)
    {
        _logger = logger;
        RepositorioProducto = new RepoProduct();
    }

    public IActionResult Listar()
    {
        try
        {
            var producto = RepositorioProducto.ListarProducto();
            return View(producto);
        }
        catch (Exception)
        {
            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }
    }


    [HttpGet]
public IActionResult Crear()
{
    var producto = new Producto();  // Inicializas un nuevo producto
    return View(producto);  // Pasas el objeto a la vista
}


    [HttpPost]
    public IActionResult Crear(Producto producto)
    {
        if (ModelState.IsValid)
        {
            RepositorioProducto.CrearNuevo(producto);
            return RedirectToAction(nameof(Index));
        }
        return View(producto);
    }

    [HttpGet]
    public IActionResult Modificar(int id)
    {
        var producto = RepositorioProducto.ObtenerProductoPorId(id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    [HttpPost]
    public IActionResult Modificar(int id, Producto producto)
    {
        if (ModelState.IsValid)
        {
            RepositorioProducto.ModificarProducto(id, producto);
            return RedirectToAction(nameof(Index));
        }
        return View(producto);
    }
    public IActionResult Eliminar(int id)
    {
        RepositorioProducto.EliminarProductoPorId(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index()
    {
        return View(RepositorioProducto.ListarProducto());
    }
    
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
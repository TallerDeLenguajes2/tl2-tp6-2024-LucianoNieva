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

    public ProductoControllers(ILogger<ProductoControllers> logger, RepoProduct RepositorioProducto)
    {
        _logger = logger;
        this.RepositorioProducto = RepositorioProducto;
    }

    public IActionResult Listar()
    {
        try
        {
            return View(RepositorioProducto.ListarProducto());
        }
        catch (Exception)
        {
            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }
    }

    
    [HttpGet]
public IActionResult Crear()
{
    var producto = new Producto();  // Inicializas el modelo si es necesario
    return View(producto);  // Pasas el objeto a la vista
}


    [HttpPost]
    public IActionResult Crear(Producto producto)
    {
        if (ModelState.IsValid)
        {
            RepositorioProducto.CrearNuevo(producto);
            return RedirectToAction("Listar");
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
                producto.IdProducto = id;
                RepositorioProducto.ModificarProducto(id, producto);
                return RedirectToAction("Listar");
            }
            return View(producto);
        }
         public IActionResult Eliminar(int id)
        {
            RepositorioProducto.EliminarProductoPorId(id);
            return RedirectToAction("Listar");
        }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

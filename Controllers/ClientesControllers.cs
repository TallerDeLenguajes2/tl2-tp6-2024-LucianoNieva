using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_LucianoNieva.Models;
using clientesRepository;
using clientes;
namespace tl2_tp6_2024_LucianoNieva.Controllers;

public class ClientesController : Controller
{
    private readonly ILogger<ClientesController> _logger;
    private readonly ClientesRepository _ClientesRepository;

    public ClientesController(ILogger<ClientesController> logger)
    {
        _logger = logger;
        _ClientesRepository = new ClientesRepository();
    }

    [HttpGet]
    public IActionResult ListarClientes()
    {
        var clientes = _ClientesRepository.obtenerClientes();
        return Ok(clientes);
    }

    [HttpGet]
    public IActionResult CrearCliente()
    {

        return View();
    }

    [HttpPost]
     [ValidateAntiForgeryToken]
    public IActionResult CrearCliente(Clientes cliente)
    {

        if (ModelState.IsValid)
        {
            _ClientesRepository.CrearCliente(cliente);
            return RedirectToAction(nameof(Index));
        }

        return View(cliente);

    }

    [HttpGet]
    public IActionResult ModificarCliente(int id){

        var cliente = _ClientesRepository.ObtenerCliente(id);
        if (cliente == null)
        {
            return NotFound();            
        }
        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ModificarCliente(int id, Clientes cliente){

        if (ModelState.IsValid){

            _ClientesRepository.modificarCliente(id, cliente);
            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }

    [HttpGet]
    public IActionResult EliminarCliente(int id)
    {
        var cliente = _ClientesRepository.ObtenerCliente(id);
        if (cliente == null)
        {
            return NotFound();
        }
        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken] //Es una buena práctica proteger las acciones POST con tokens antifalsificación para prevenir ataques Cross-Site Request Forgery (CSRF).
    public IActionResult EliminarClienteConfirmado(int id)
    {
        //En este caso no es necesario el ModelState.IsValid porque solo recibo un dato simple(id)
        _ClientesRepository.EliminarCliente(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index()
    {
        return View(_ClientesRepository.obtenerClientes());
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
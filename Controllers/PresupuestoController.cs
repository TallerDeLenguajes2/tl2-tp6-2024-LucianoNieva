using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using presupuestosRepository;
using repoProduct;
using producto;
using presupuestos;

namespace tl2_tp6_2024_LucianoNieva.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly ILogger<PresupuestosController> _logger;
        private readonly PresupuestosRepository _presupuestoRepo;
        private readonly RepoProduct _productoRepo;

        public PresupuestosController(ILogger<PresupuestosController> logger, PresupuestosRepository presupuestoRepo, RepoProduct productoRepo)
        {
            _logger = logger;
            _presupuestoRepo = presupuestoRepo;
            _productoRepo = productoRepo;
        }

        // Listar todos los presupuestos
        public IActionResult ListarPre()
        {
            var presupuestos = _presupuestoRepo.ListarPresupuestos();
            return View(presupuestos);
        }

        // Crear un presupuesto (GET)
        public IActionResult CrearPre()
        {
            return View();
        }

        // Crear un presupuesto (POST)
        [HttpPost]
        public IActionResult CrearPre(Presupuestos presupuesto)
        {
            if (ModelState.IsValid)
            {
                _presupuestoRepo.CrearNuevo(presupuesto);
                return RedirectToAction("ListarPre");
            }
            return View(presupuesto);
        }

        // Modificar un presupuesto (GET)
        public IActionResult ModificarPre(int id)
        {
            var presupuesto = _presupuestoRepo.ObtenerPresupuestoPorId(id);
            if (presupuesto == null) return NotFound();
            return View(presupuesto);
        }

        // Modificar un presupuesto (POST)
        [HttpPost]
        public IActionResult ModificarPre(Presupuestos presupuesto)
        {
            if (ModelState.IsValid)
            {
                _presupuestoRepo.ModificarPresupuesto(presupuesto);
                return RedirectToAction("ListarPre");
            }
            return View(presupuesto);
        }

        // Eliminar un presupuesto
        public IActionResult EliminarPre(int id)
        {
            _presupuestoRepo.EliminarPresupuesto(id);
            return RedirectToAction("ListarPre");
        }

        // Ver detalles de un presupuesto específico
        public IActionResult VerPresupuestoPre(int id)
        {
            var presupuesto = _presupuestoRepo.ObtenerPresupuestoPorId(id);
            return View(presupuesto);
        }

        // Agregar un producto a un presupuesto (GET)
        public IActionResult AgregarProductoPre(int id)
        {
            var presupuesto = _presupuestoRepo.ObtenerPresupuestoPorId(id);
            ViewBag.Productos = _productoRepo.ListarProducto(); // Obtener lista de productos
            return View(presupuesto);
        }

        // Agregar un producto a un presupuesto (POST)
        [HttpPost]
        public IActionResult AgregarProductoPre(int id, int productoId, int cantidad)
        {
            var presupuesto = _presupuestoRepo.ObtenerPresupuestoPorId(id);
            var producto = _productoRepo.ObtenerProductoPorId(productoId);

            if (presupuesto != null && producto != null)
            {
                _presupuestoRepo.AgregarProductoAPresupuesto(id, producto, cantidad);
            }
            return RedirectToAction("VerPresupuestoPre", new { id });
        }
    }
}

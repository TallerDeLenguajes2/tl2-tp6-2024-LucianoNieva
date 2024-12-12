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

        public PresupuestosController(ILogger<PresupuestosController> logger)
        {
            _logger = logger;
            _presupuestoRepo = new PresupuestosRepository();
            _productoRepo = new RepoProduct();
        }

        // Listar todos los presupuestos
        [HttpGet]
        public IActionResult ListarPre()
        {
            var presupuestos = _presupuestoRepo.ListarPresupuestos();
            return View(presupuestos);
        }

        [HttpGet]
        public IActionResult ListarDetalles(int id)
        {
            var detalles = _presupuestoRepo.ObtenerDetalle(id);
            return View(detalles);
        }
        // Crear un presupuesto (GET)
        [HttpGet]
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
                if (presupuesto.Detalle == null || presupuesto.Detalle.Count == 0)
                {
                    ModelState.AddModelError("", "Debe incluir al menos un detalle en el presupuesto.");
                    return View(presupuesto);
                }

                _presupuestoRepo.CrearPre(presupuesto);
                return RedirectToAction(nameof(Index));
            }
            return View(presupuesto);
        }

        // Modificar un presupuesto (GET)
        [HttpGet]
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
                return RedirectToAction(nameof(Index));
            }
            return View(presupuesto);
        }

        // Eliminar un presupuesto
        [HttpGet]
        public IActionResult EliminarPre(int id)
        {
            var presupuesto = _presupuestoRepo.ObtenerPresupuestoPorId(id);
            if (presupuesto == null) return NotFound();
            return View(presupuesto);
        }

        [HttpPost]
        public IActionResult EliminarPresupuestoConfirmado(int id)
        {
            _presupuestoRepo.EliminarPresupuesto(id);
            return RedirectToAction(nameof(Index));
        }

            // Ver detalles de un presupuesto específico
            public IActionResult VerPresupuestoPre(int id)
            {
                var presupuesto = _presupuestoRepo.ObtenerPresupuestoPorId(id);
                if (presupuesto == null) return NotFound();
                return View(presupuesto);
            }

            // Agregar un producto a un presupuesto (GET)
            [HttpGet]
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
            

        public IActionResult Index()
        {
            return View(_presupuestoRepo.ListarPresupuestos());
        }

    }


}

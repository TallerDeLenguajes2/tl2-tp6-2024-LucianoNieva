using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using presupuestosRepository;
using repoProduct;
using presupuestos;
using clientesRepository;
using presupuestosDetalles;
using modificarPresupuesto;

namespace tl2_tp6_2024_LucianoNieva.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly ILogger<PresupuestosController> _logger;
        private readonly PresupuestosRepository _presupuestoRepo;
        private readonly ClientesRepository _ClientesRepository;
        private readonly RepoProduct _productoRepo;

        public PresupuestosController(ILogger<PresupuestosController> logger)
        {
            _logger = logger;
            _presupuestoRepo = new PresupuestosRepository();
            _ClientesRepository = new ClientesRepository();
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
            var modelo = new PresupuestoViewModel()
            {
                Productos = _productoRepo.ListarProducto(),
                Clientes = _ClientesRepository.obtenerClientes(),
            };
            return View(modelo);
        }

        // Crear un presupuesto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPre(PresupuestoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.ClienteIdSeleccionado == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar un cliente antes de agregar productos.");
                    viewModel.Clientes = _ClientesRepository.obtenerClientes();
                    viewModel.Productos = _productoRepo.ListarProducto();
                    return View(viewModel);
                }

                // Verifica si la lista de productos seleccionados no está vacía
                if (viewModel.ProductosSeleccionados == null || viewModel.ProductosSeleccionados.Count == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos un producto.");
                    viewModel.Clientes = _ClientesRepository.obtenerClientes();
                    viewModel.Productos = _productoRepo.ListarProducto();
                    return View(viewModel);
                }

                var cliente = _ClientesRepository.ObtenerCliente(viewModel.ClienteIdSeleccionado);
                var nuevoPresupuesto = new Presupuestos
                {
                    Cliente = cliente,
                    FechaCreacion = DateTime.Now.ToString(),
                    Detalle = new List<PresupuestosDetalles>()
                };

                foreach (var productoSeleccionado in viewModel.ProductosSeleccionados)
                {
                    if (productoSeleccionado.ProductoId > 0 && productoSeleccionado.Cantidad > 0)
                    {
                        var producto = _productoRepo.ObtenerProductoPorId(productoSeleccionado.ProductoId);
                        nuevoPresupuesto.Detalle.Add(new PresupuestosDetalles
                        {
                            Producto = producto,
                            Cantidad = productoSeleccionado.Cantidad
                        });
                    }
                }

                _presupuestoRepo.CrearPre(nuevoPresupuesto);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Clientes = _ClientesRepository.obtenerClientes();
            viewModel.Productos = _productoRepo.ListarProducto();
            return View(viewModel);
        }


        [HttpGet]
        public IActionResult ModificarPresupuesto(int id)
        {
            // Cargo el presupuesto y los clientes desde la base de datos
            var presupuesto = _presupuestoRepo.ObtenerPresupuestoPorId(id);
            var clientes = _ClientesRepository.obtenerClientes();
            var productos = _productoRepo.ListarProducto();

            var viewModel = new ModificarPresupuesto
            {
                Clientes = clientes,
                Productos = productos,
                Presupuesto = presupuesto,
                ClienteIdSeleccionado = presupuesto.Cliente.ClienteId
            };
            return View(viewModel);
        }

        // Modificar un presupuesto (GET)
        [HttpPost]
        public IActionResult ModificarPre(ModificarPresupuesto vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.ClienteIdSeleccionado == 0)
                {
                    ModelState.AddModelError("ClienteIdSeleccionado", "Debe seleccionar un cliente válido");
                    vm.Clientes = _ClientesRepository.obtenerClientes();
                    vm.Productos = _productoRepo.ListarProducto();
                    return View(vm);
                }

                var presupuestoExistente = _presupuestoRepo.ObtenerPresupuestoPorId(vm.Presupuesto.IdPresupuesto);
                if (presupuestoExistente == null)
                {
                    return NotFound();
                }

                // Crear el presupuesto actualizado
                var presupuestoActualizado = new Presupuestos
                {
                    IdPresupuesto = vm.Presupuesto.IdPresupuesto,
                    FechaCreacion = presupuestoExistente.FechaCreacion,
                    Cliente = new clientes.Clientes { ClienteId = vm.ClienteIdSeleccionado },
                    Detalle = vm.Presupuesto.Detalle?
                        .Where(d => d.Cantidad > 0 && d.Producto != null) // Filtrar elementos válidos
                        .Select(d => new PresupuestosDetalles
                        {
                            Producto = new producto.Producto
                            {
                                IdProducto = d.Producto.IdProducto,
                                Descripcion = d.Producto.Descripcion,
                                Precio = d.Producto.Precio
                            },
                            Cantidad = d.Cantidad
                        })
                        .ToList() ?? new List<PresupuestosDetalles>()
                };

                try
                {
                    _presupuestoRepo.ModificarPresupuesto(presupuestoActualizado);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
                    vm.Clientes = _ClientesRepository.obtenerClientes();
                    vm.Productos = _productoRepo.ListarProducto();
                    return View(vm);
                }
            }

            return View(vm);
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
using clientes;
using presupuestos;
using producto;

namespace modificarPresupuesto
{
    public class ModificarPresupuesto
    {

        public ModificarPresupuesto()
        {
            Presupuesto = new Presupuestos();
            Clientes = new List<Clientes>();
            Productos = new List<Producto>();


        }

        public Presupuestos Presupuesto { get; set; }  // Presupuesto actual
        public List<Clientes> Clientes { get; set; }    // Lista de clientes para el desplegable
        public int ClienteIdSeleccionado { get; set; } // ID del cliente actualmente seleccionado
        public List<Producto> Productos { get; set; }
    }
}
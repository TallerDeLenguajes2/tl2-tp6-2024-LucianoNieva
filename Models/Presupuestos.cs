using System;
using presupuestosDetalles;
namespace presupuestos
{
    public class Presupuestos
    {

        private int idPresupuesto;
        private string nombreDestinatario;
        private List<PresupuestosDetalles> detalle;
        private DateTime? fechaCreacion;

        public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
        public string NombreDestinatario { get => nombreDestinatario; set => nombreDestinatario = value; }
        public List<PresupuestosDetalles> Detalle { get => detalle; set => detalle = value; }
        public DateTime? FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }

        public double MontoPresupuesto()
        {
            int acumulador = 0;

            foreach (var item in Detalle)
            {
                acumulador += item.Producto.Precio;
            }
            return acumulador;
        }

        public double MontoPresupuestoConIva()
        {
            double acumulador = 0;

            foreach (var item in Detalle)
            {
                acumulador += item.Producto.Precio;
            }
            return acumulador + (acumulador * 0.21);
        }
        public int CantidadProductos()
        {
            int cantProductos = 0;

            foreach (var item in detalle)
            {
                cantProductos++;
            }
            return cantProductos;
        }
    }
}
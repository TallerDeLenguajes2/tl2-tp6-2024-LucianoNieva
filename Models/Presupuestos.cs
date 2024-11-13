using System;
using presupuestosDetalles;
using clientes;
namespace presupuestos
{
    public class Presupuestos
    {

        private int idPresupuesto;
        private Clientes cliente;
        private List<PresupuestosDetalles> detalle;
        private DateTime? fechaCreacion;

        public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
        public List<PresupuestosDetalles> Detalle { get => detalle; set => detalle = value; }
        public DateTime? FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }
        public Clientes Cliente { get => cliente; set => cliente = value; }

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
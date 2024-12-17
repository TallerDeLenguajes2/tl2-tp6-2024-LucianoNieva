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
        private string fechaCreacion;

        public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
        public List<PresupuestosDetalles> Detalle { get => detalle; set => detalle = value; }
        public string FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }
        public Clientes Cliente { get => cliente; set => cliente = value; }

        public Presupuestos(){}
        public Presupuestos(int id, Clientes clientes, string fechaCreacion)
        {
            this.idPresupuesto = id;
            this.cliente = clientes;
            this.fechaCreacion = fechaCreacion;
            detalle = new List<PresupuestosDetalles>();
        }

        public Presupuestos(int v)
        {
            FechaCreacion = DateTime.Today.ToString("dd-MM-yyyy");
            detalle = new List<PresupuestosDetalles>();
        }

        public double MontoPresupuesto()
        {
            int monto = detalle.Sum(d => d.Cantidad * d.Producto.Precio);
            return monto;
        }

        public double MontoPresupuestoConIva()
        {
            return MontoPresupuesto() * 1.21;
        }
        public int CantidadProductos()
        {
            return detalle.Sum(d => d.Cantidad);
        }
    }
}

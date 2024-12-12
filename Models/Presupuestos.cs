using System;
using Microsoft.Data.Sqlite;
using presupuestosDetalles;
namespace presupuestos
{
    public class Presupuestos
    {
        private static int autoincremento = 1;
        private int idPresupuesto;
        private string nombreDestinatario;
        private List<PresupuestosDetalles> detalle;
        private string fechaCreacion;

        public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
        public string NombreDestinatario { get => nombreDestinatario; set => nombreDestinatario = value; }
        public List<PresupuestosDetalles> Detalle { get => detalle; set => detalle = value; }
        public string FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }

        public Presupuestos()
        {
            IdPresupuesto = autoincremento++;
            FechaCreacion = DateTime.Today.ToString("dd-MM-yyyy");
            detalle = new List<PresupuestosDetalles>();
        }

        public Presupuestos(int id,string nombreDestinatario, string fechaCreacion)
        {
            this.idPresupuesto = id;
            this.nombreDestinatario = nombreDestinatario;
            this.fechaCreacion = fechaCreacion;
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
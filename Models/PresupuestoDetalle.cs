using System;
using producto;

namespace presupuestosDetalles
{
    public class PresupuestosDetalles{

        private Producto producto;
        private int cantidad;

        public Producto Producto { get => producto; set => producto = value; }
        public int Cantidad { get => cantidad; set => cantidad = value; }
    }
}
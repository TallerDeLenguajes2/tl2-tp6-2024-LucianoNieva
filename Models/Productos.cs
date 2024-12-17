using System;
using System.ComponentModel.DataAnnotations;

namespace producto
{
    public class Producto{

        private int idProducto;
        private string descripcion;
        private int precio;

        public Producto(){}

        public Producto(int idProducto, string descripcion, int precio){
            this.idProducto=idProducto;
            this.descripcion=descripcion;
            this.precio=precio;
        }

        public int IdProducto { get => idProducto; set => idProducto = value; }

        [Required(ErrorMessage = "Ingrese una descripcion")]
        [StringLength(250, ErrorMessage = "Solo puede tener 250 caracteres")]
        public string Descripcion { get => descripcion; set => descripcion = value; }

        [Required(ErrorMessage = "Ingrese un precio")]
        [Range(0,int.MaxValue, ErrorMessage = "Ingrese un numero positivo")]
        public int Precio { get => precio; set => precio = value; }
    }
}
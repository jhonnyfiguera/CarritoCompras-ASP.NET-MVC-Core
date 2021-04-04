using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class CarritoItem
    {
        #region propiedades
        public Guid Id { get; private set; }

        public Carrito Carrito { get; set; }

        public Producto Producto { get; set; }

        public decimal ValorUnitario { get; set; }

        public int Cantidad { get; set; }

        public  decimal Subtotal { get; set; }

        public Guid CarritoId { get; private set; }

        #endregion

        #region Constructores
        public CarritoItem(Carrito carrito, Producto producto, decimal valorUnitario, int cantidad, decimal subtotal)
        {
            Id = Guid.NewGuid();
            Carrito = carrito;
            Producto = producto;
            ValorUnitario = valorUnitario;
            Cantidad = cantidad;
            Subtotal = subtotal;
            //queda pendiente definir bien carritoId respecto a su FK
        }

        public CarritoItem():this (new Carrito(), new Producto(), 0, 0, 0)

        {
        }


        #endregion
    }
}

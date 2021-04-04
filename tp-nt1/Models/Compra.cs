using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Compra
    {
        #region Propiedades
        public Guid Id { get; private set; }
        public Cliente Cliente { get; set; }
        public Carrito Carrito { get; set; }
        public decimal Total { get; set; }
        //Propiedades FK
        public Guid ClienteId { get; set; }
        #endregion

        #region Propiedades
        public Compra(Cliente cliente, Carrito carrito, decimal total)
        {
            Cliente = cliente;
            Carrito = carrito;
            Total = total;
            //Propiedades FK
           // ClienteId = clienteId;
        }

        public Compra () : this(new Cliente(), new Carrito(), 0)
        {

        }
        #endregion
    }
}

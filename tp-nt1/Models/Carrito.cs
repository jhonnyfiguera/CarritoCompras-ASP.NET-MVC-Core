using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Carrito
    {
        #region Propiedades
        public Guid Id { get; private set; }
        public Boolean Activo { get; set; }
        public Cliente Cliente { get; set; }
        public List<CarritoItem> CarritosItems { get; private set; }
        public decimal Subtotal { get; set; }
        //Propiedades FK
        public Guid ClienteId { get; set; }
        public Guid CompraId { get; set; }
        #endregion

        #region Constructores
        public Carrito(Boolean activo, Cliente cliente, decimal subtotal)
        {
            Id = Guid.NewGuid();
            Activo = activo;
            Cliente = cliente;
            CarritosItems = new List<CarritoItem>();
            Subtotal = subtotal;
            //Pendiente Definicion de FK
            //ClienteId / CompraId;
            
        }

        public Carrito () : this (false, new Cliente(), 0)
        {

        }
        #endregion

        




    }
}

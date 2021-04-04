using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class StockItem
    {
        #region Propiedades
        public Guid Id { get; private set; }

        public Sucursal Sucursal { get; set; }

        public Producto Producto { get; set; }

        public int Cantidad { get; set; }

        #endregion

        #region Constructores

        public StockItem(Sucursal sucursal, Producto producto, int cantidad)
        {
            Id = Guid.NewGuid();
            Sucursal = sucursal;
            Producto = producto;
            Cantidad = cantidad;
        }

        public StockItem() : this(new Sucursal(), new Producto(), 0)
        {
        }

        #endregion
    }
}
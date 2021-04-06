using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Empleado : Persona
    {
        #region Constructores
        public Empleado() : : base("Sin Asignar", "Sin Asignar", "Sin Asignar", 0, "Sin Asignar", "Sin Asignar", 0, 0)
        {
        }
        #endregion
        #region Métodos
        public void ListarCompras() { }
        public void DarAltaEmpleado() { }
        public void CrearProducto() { }
        public void CrearCategoria() { }
        public void CrearSucursal() { }
        public void AgregarProductosStockSucursal() { }
        public void HabilitarDeshabilitarProducto() { }
        #endregion
    }
}

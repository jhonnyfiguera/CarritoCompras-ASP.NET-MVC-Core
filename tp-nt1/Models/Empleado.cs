using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Empleado : Persona
    {
        #region Constructores
        public Empleado() : base(string nombre, string apellido, string email, string numero, string caracteristica, string calle, int piso, string dpto)
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

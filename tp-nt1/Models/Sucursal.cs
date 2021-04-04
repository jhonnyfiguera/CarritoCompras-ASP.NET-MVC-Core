using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Sucursal
    {
        #region Propiedades
        public Guid Id { get; private set; }
        public string Nombre { get; set; }
        public Telefono Telefono { get; set; }
        public Direccion Direccion { get; set; }
        public string Email { get; set; }

        #region Constructores
        public Sucursal(string nombre, Telefono telefono, Direccion direccion, string email)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Telefono = telefono;
            Direccion = direccion;
            Email = email;
        }

        public Sucursal() : this("Sin Asignar", new Telefono(), new Direccion(), "Sin Asignar")
        {

        }
        #endregion


    }
}

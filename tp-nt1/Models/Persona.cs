using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public abstract class Persona
    {
        #region Propiedades
        public Guid Id { get;  private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public Telefono Telefono { get; set; }
        public Direccion Direccion { get; set; }
        public string Email { get; set; } //Pendiente definir manejo de Mail 
        public DateTime FechaAlta { get; private set; } //Fecha de Alta se registra en el alta de Persona y no se puede modificar posterior
        #endregion
        #region Constructores
        public Persona(string nombre, string apellido, string email, string numero, string caracteristica, string calle, int piso, string dpto)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Apellido = apellido;
            Telefono = new Telefono(numero, caracteristica);
            Direccion = new Direccion(calle, piso, dpto);
            Email = email;
            FechaAlta = new DateTime(); //Pendiente resolver parametros
        }
        public Persona() : this("Sin Asignar", "Sin Asignar", "Sin Asignar", "Sin Asignar", "Sin Asignar", "Sin Asignar", 0, "Sin Asignar")
        {
        }
        #endregion


    }
}

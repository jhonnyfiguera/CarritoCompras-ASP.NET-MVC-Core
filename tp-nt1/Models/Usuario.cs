using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Usuario
    {
        #region Propiedades
        public Guid Id { get; private set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Email { get; set; } //Pendiente definir manejo de Mail 
        public DateTime FechaAlta { get; set; }
        #endregion
        #region Constructores
        public Usuario(string nombre, string password, string email)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Password = password;
            Email = email;
            FechaAlta = new DateTime(); //Pendiente resolver parametros
        }
        public Usuario() : this("Sin Asignar", "Sin Asignar", "Sin Asignar")
        {
        }
        #endregion
    }
}

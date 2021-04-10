using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public abstract class Persona
    {
        #region Propiedades
        [Key]
        public Guid Id { get;  private set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El nombre admite sólo caracteres alfabéticos")]
        public string Nombre { get; private set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El apellido admite sólo caracteres alfabéticos")]
        public string Apellido { get; private set; }

        public Telefono Telefono { get; set; }

        public Direccion Direccion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(10, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MaxLength(100, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Email { get; set; } //Pendiente definir manejo de Mail 

        public DateTime FechaAlta { get; private set; } //Fecha de Alta se registra en el alta de Persona y no se puede modificar posterior
        #endregion

    }
}


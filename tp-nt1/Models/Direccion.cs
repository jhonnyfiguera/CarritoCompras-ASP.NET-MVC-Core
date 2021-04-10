using System;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public class Direccion
    {
        #region Propiedades
        [Key]
        public Guid Id { get; private set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El nombre admite sólo caracteres alfabéticos")]
        public string Calle { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 1000000, ErrorMessage = "La {0} se debe encontrar entre {1} y {2}")]
        public int Piso { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El nombre admite sólo caracteres alfabéticos")]
        [Display(Name = "Departamento")]
        public string Dpto { get; set; }


        //public Guid ClienteId { get; set; }
        //public Guid EmpleadoId { get; set; } 
       // public Guid SucursalId { get; set; }



        #endregion

    }
}
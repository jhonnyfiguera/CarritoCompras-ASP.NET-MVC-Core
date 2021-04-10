using System;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public class Telefono
    {
        #region Propiedades
        [Key]
        public Guid Id { get; private set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[0-9]{4}\.[0-9]{4}", ErrorMessage = "El {0} debe tener un formato NNNN-NNNN")]
        public string Numero { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[0-9]{3}", ErrorMessage = "El {0} debe tener un formato NNN")]
        public string Caracteristica { get; set; }

        //public Guid ClienteId { get; set; } 
        //public Guid EmpleadoId { get; set; }
        //public Guid SucursalId { get; set; }



        #endregion

    }
}
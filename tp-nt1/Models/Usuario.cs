using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace tp_nt1.Models
{
    public class Usuario
    {
        #region Propiedades
        [Key]
        public Guid Id { get; private set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(8, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MaxLength(50, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password, ErrorMessage = "Debe ingresar una {0} Correcta")]
        [Display(Name = "Contrasenia")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(10, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MaxLength(100, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Email { get; set; } //Pendiente definir manejo de Mail 

        public DateTime FechaAlta { get; set; }
        #endregion
    }
}

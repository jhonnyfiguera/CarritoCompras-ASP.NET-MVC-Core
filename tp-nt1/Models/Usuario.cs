using System;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public abstract class Usuario
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(3, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")]
        [MaxLength(30, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El {0} admite sólo caracteres alfabéticos")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(3, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")]
        [MaxLength(30, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El {0} admite sólo caracteres alfabéticos")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(8, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")]
        [MaxLength(20, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [RegularExpression(@"[0-9]{2}\[0-9]{4}\[0-9]{4}", ErrorMessage = "El {0} debe tener un formato 12 3456 7890")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(200, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(8, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MaxLength(80, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        public DateTime FechaAlta { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(1, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MaxLength(80, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [Display(Name ="Nombre de Usuario")]
        public string Username { get; set; }

        public byte[] Password { get; set; }
    }
}
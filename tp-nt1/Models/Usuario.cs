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
        [MaxLength(13, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [RegularExpression(@"[0-9]*", ErrorMessage = "El {0} admite sólo caracteres numéricos")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[a-zA-Z áéíóú]{3,50}\, [0-9 a-zA-Z áéíóú .]{3,50}\, [0-9]{3,4}", ErrorMessage = "El {0} admite la siguiente estructura Localidad, Calle, Número")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[0-9 a-zA-Z áéíóú ._-]{3,50}\@[0-9 a-zA-Z áéíóú .]{8,20}", ErrorMessage = "El {0} admite la siguiente estructura ejemplo@ejemplo.com")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(80, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }

        [Display(Name = "Fecha de Alta")]
        public DateTime FechaAlta { get; set; }

        [ScaffoldColumn(false)] // Utilizamos esto para que no se autogenere el campo password cuando hacemos scaffolding
        public byte[] Password { get; set; } // La password es de tipo array de bytes para almacenar las contraseñas encriptadas

        public abstract Rol Rol { get; }

    }
}
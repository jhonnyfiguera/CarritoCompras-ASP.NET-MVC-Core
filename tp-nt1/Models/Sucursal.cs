using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public class Sucursal
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(3, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")]
        [MaxLength(30, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(8, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")]
        [MaxLength(13, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")]
        [RegularExpression(@"[0-9]*", ErrorMessage = "El {0} admite sólo caracteres numéricos")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[a-zA-Z áéíóú]{3,50}\, [0-9 a-zA-Z áéíóú .]{3,50}\, [0-9]{3,4}", ErrorMessage = "El {0} admite esta estructura Localidad, Calle, Número")]
        public string Direccion { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[0-9 a-zA-Z áéíóú ._-]{3,50}\@[0-9 a-zA-Z áéíóú .]{8,20}", ErrorMessage = "El {0} admite esta estructura ejemplo@ejemplo.com")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        public List<StockItem> StockItems { get; set; }
    }
}
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
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El nombre admite sólo caracteres alfabéticos")]
        [MinLength(3, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MaxLength(15, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(20, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(200, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        public List<StockItem> StockItems { get; set; }
    }
}

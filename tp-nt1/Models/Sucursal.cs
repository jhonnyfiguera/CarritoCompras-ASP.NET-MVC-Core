using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public class Sucursal
    {
        #region Propiedades
        [Key]
        public Guid Id { get; private set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El nombre admite sólo caracteres alfabéticos")]
        [MinLength(3, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MaxLength(15, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Telefono Telefono { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Direccion Direccion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        public List<StockItem> StockItems { get; set; }




    }
}

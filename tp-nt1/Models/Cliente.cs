using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace tp_nt1.Models
{
    public class Cliente : Usuario
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(7, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")] 
        [MaxLength(9, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [RegularExpression(@"[0-9]*", ErrorMessage = "El {0} admite sólo caracteres numéricos")]
        public string Dni { get; set; }

        public List<Compra> Compras { get; set; }

        public List<Carrito> Carritos { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public class CarritoItem
    {
        [Key]
        public Guid Id { get; set; }

        public Carrito Carrito { get; set; }

        public Producto Producto { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 1000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        [Display(Name = "Valor Unitario")]
        public decimal ValorUnitario { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 1000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 1000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        public decimal Subtotal { get; set; }
    }
}

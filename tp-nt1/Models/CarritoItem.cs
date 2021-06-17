using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp_nt1.Models
{
    public class CarritoItem
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Carrito))]
        public Guid CarritoId { get; set; }
        public Carrito Carrito { get; set; }

        [ForeignKey(nameof(Producto))]
        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 100000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        [Display(Name = "Valor Unitario")]
        public decimal ValorUnitario { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, 100000, ErrorMessage = "El valor debe estar entre {1} y {2} ")]
        [RegularExpression(@"[0-9]*", ErrorMessage = "No se admite Decimales ni Negativos")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 10000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        public decimal Subtotal { get; set; }
    }
}
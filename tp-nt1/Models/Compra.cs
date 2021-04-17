using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp_nt1.Models
{
    public class Compra
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 100000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        public decimal Total { get; set; }

        [ForeignKey(nameof(Cliente))]
        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [ForeignKey(nameof(Carrito))]
        public Guid CarritoId { get; set; }
        public Carrito Carrito { get; set; }
    }
}
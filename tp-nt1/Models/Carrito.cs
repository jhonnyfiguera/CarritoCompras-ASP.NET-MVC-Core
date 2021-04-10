using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public class Carrito
    {
        [Key]
        public Guid Id { get; set; }

        public bool Activo { get; set; }

        public Cliente Cliente { get; set; }

        public List<CarritoItem> CarritosItems { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 10000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        public decimal Subtotal { get; set; }
    }
}

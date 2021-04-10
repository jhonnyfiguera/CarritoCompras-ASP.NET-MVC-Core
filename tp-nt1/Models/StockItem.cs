using System;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public class StockItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 1000000, ErrorMessage = "La {0} se debe encontrar entre {1} y {2}")]
        public int Cantidad { get; set; }

        public Sucursal Sucursal { get; set; }

        public Producto Producto { get; set; }
    }
}
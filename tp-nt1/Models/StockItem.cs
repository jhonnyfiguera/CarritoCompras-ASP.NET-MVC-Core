using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp_nt1.Models
{
    public class StockItem
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Sucursal))]
        public Guid SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }

        [ForeignKey(nameof(Producto))]
        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, 100000, ErrorMessage = "El valor debe estar entre {1} y {2} ")]
        [RegularExpression(@"[0-9]*", ErrorMessage = "No se admite Decimales ni Negativos")]
        public int Cantidad { get; set; }
    }
}
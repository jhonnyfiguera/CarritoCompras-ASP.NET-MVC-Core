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
        public int Cantidad { get; set; }
    }
}
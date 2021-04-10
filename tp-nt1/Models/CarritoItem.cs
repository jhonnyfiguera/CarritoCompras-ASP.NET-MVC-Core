using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class CarritoItem
    {
        #region propiedades

        [Key]
        public Guid Id { get; private set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Carrito Carrito { get; set; }

        
        public Producto Producto { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 1000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        [Display (Name = "Valor Unitario")]
        public decimal ValorUnitario { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 1000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        public int Cantidad { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 1000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        public  decimal Subtotal { get; set; }

        //public Guid CarritoId { get; private set; }

        #endregion

        


        
    }
}

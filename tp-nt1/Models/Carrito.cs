using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Carrito
    {
        #region Propiedades



        [Key]
        public Guid Id { get; private set; }

        public Boolean Activo { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Cliente Cliente { get; set; }

        public List<CarritoItem> CarritosItems { get; private set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 10000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        public decimal Subtotal { get; set; }


        //Propiedades FK
        //public Guid ClienteId { get; set; }
        //public Guid CompraId { get; set; }
        #endregion

        

        




    }
}

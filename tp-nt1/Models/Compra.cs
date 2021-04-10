using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Compra
    {
        #region Propiedades

        [Key]
        public Guid Id { get; private set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Cliente Cliente { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Carrito Carrito { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 1000000, ErrorMessage = "El {0} debe estar entre {1} y {2} ")]
        public decimal Total { get; set; }
        //Propiedades FK


        //public Guid ClienteId { get; set; }
        #endregion

        #region Propiedades
        public Compra(Cliente cliente, Carrito carrito, decimal total)
        {
            Cliente = cliente;
            Carrito = carrito;
            Total = total;
            //Propiedades FK
           // ClienteId = clienteId;
        }

        public Compra () : this(new Cliente(), new Carrito(), 0)
        {

        }
        #endregion
    }
}

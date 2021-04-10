using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class StockItem
    {
        /*
        StockItem:
        -- Pueden crearse pero nunca pueden eliminarse desde el sistema. 
        -- Son dependientes de la surcursal.
        -- Puede modificarse la cantidad que se dispone de dicho producto, 
           en todo momento a través de la propiedad Cantidad.
        -- Si se elimina la sucursal del stockItem, éste elemento también será eliminado.*/

        #region Propiedades
        [Key]
        public Guid Id { get; private set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Sucursal Sucursal { get; set; }
        //Pendiente FK preguntar al prof.
        //public Guid CategoriaId { get; private set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Producto Producto { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        //Pendiente Validar: Maxima cantidad el maximo de la cantida... 
        [Range(0,1000000,ErrorMessage = "La {0} se debe encontrar entre {1} y {2}")]
        public int Cantidad { get; set; }
        #endregion
    }
}
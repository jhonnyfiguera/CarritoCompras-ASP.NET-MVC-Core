using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Categoria
    {
        /*
          Producto y Categoría:
       -- No pueden eliminarse del sistema.
       -- Solo los productos pueden dehabilitarse.*/
        #region propiedades
        [Key]
        public Guid Id { get; private set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(3, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MaxLength(15, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(10, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MaxLength(25, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Descripcion { get; set; }

        public List<Producto> Productos { get; private set; }

        //Pendiente FK preguntar al prof.
        //public Guid ProductoId { get; private set; }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public class Categoria
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(3, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")]
        [MaxLength(30, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(3, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")]
        [MaxLength(30, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Descripcion { get; set; }

        public List<Producto> Productos { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp_nt1.Models
{
    public class Producto
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(3, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")]
        [MaxLength(30, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(3, ErrorMessage = "El campo {0} admite un mínimo de {1} caracteres")]
        [MaxLength(200, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 100000000, ErrorMessage = "El {0} se debe encontrar entre {1} y {2}")]
        [Display(Name = "Precio Vigente")]
        public decimal PrecioVigente { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool Activo { get; set; }

        [ForeignKey(nameof(Categoria))]
        public Guid CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}
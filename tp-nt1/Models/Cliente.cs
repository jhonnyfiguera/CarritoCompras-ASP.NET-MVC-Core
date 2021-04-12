using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace tp_nt1.Models
{
    public class Cliente : Usuario
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[0-9]{2}\[0-9]{3}\[0-9]{3}", ErrorMessage = "El {0} debe tener un formato NN NNN NNN")]
        public string Dni { get; set; }

        public List<Compra> Compras { get; set; }

        public List<Carrito> Carritos { get; set; }
    }
}
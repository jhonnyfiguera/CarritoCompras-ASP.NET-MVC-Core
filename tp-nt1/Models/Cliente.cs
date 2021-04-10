using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace tp_nt1.Models
{
    public class Cliente : Persona
    {
        #region Propiedades
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[0-9]{2}\.[0-9]{3}\.[0-9]{3}", ErrorMessage = "El dni debe tener un formato NN.NNN.NNN")]
        public string Dni { get; private set; }

        public List<Compra> Compras { get; private set; }

        public List<Carrito> Carritos { get; private set; }
        #endregion
    }
}
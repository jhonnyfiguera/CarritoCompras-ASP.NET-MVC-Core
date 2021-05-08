using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Administrador : Usuario
    {
        public override Rol Rol => Rol.Administrador;
    }
}

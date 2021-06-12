using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Extensions
{
    public static class IntExtensions
    {
        public static void ValidarInput(this int cantidad)
        {
            // evaluar criterios
            if (cantidad < 1 || cantidad > 100000)
            {
                throw new Exception("La Cantidad debe estar entre 1 y 100000.");
            }
        }
    }
}
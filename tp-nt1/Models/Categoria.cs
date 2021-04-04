using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Categoria
    {
        #region propiedades
        public Guid Id { get; private set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public List<Producto> Productos { get; private set; }


        #endregion

        #region constructores

        public Categoria(string nombre, string descripcion)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Descripcion = descripcion;
            Productos = new List<Producto>();
        }

        public Categoria():this("sin asignar","sin asignar")
        {
        }
        #endregion
    }
}

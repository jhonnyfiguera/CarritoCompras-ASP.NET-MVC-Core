using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tp_nt1.Models
{
    public class Producto
    {
        #region propiedades
        public Guid Id { get; private set; }

        public string  Nombre { get; set; }

        public string Descripcion { get; set; }

        public decimal PrecioVigente { get; set; }

        public Boolean Activo { get; set; }

        public Categoria Categoria { get; set; }
        // pendiente FK
        public Guid CarritoId { get;private set; }

        public Guid CategoriaId { get; private set; }

        #endregion

        #region constructor
        public Producto(string nombre, string descripcion, decimal precioVigente, Boolean activo, Categoria categoria)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Descripcion = descripcion;
            PrecioVigente = precioVigente;
            Activo = activo;
            Categoria = categoria;
            //CarritoId  - CategoriaId    PENDIENTE 
             
        }

        public Producto():this("sin asignar","sin asignar",0,false,new Categoria())
        {
        }

        #endregion
    }
}

using System;

namespace tp_nt1.Models
{
    public class Direccion
    {
        #region Propiedades
        public Guid Id { get; private set; }
        public string Calle { get; set; }
        public int Piso { get; set; }
        public string Dpto { get; set; }
        public Guid ClienteId { get; set; } 
        public Guid EmpleadoId { get; set; }
        public Guid SucursalId { get; set; }
        #endregion
        #region Constructores
        public Direccion(string calle, int piso, string dpto)
        {
            Id = Guid.NewGuid();
            Calle = calle;
            Piso = piso;
            Dpto = dpto;
            // ClienteId, EmpleadoId, SucursalId: ¿se resuelve por LinQ las FK?
        }
        public Direccion() : this("Sin Asignar", 0, "Sin Asignar")
        {
        }
        #endregion
    }
}
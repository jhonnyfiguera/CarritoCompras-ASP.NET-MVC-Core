using System;

namespace tp_nt1.Models
{
    public class Telefono
    {
        #region Propiedades
        public Guid Id { get; private set; }
        public string Numero { get; set; }
        public string Caracteristica { get; set; }
        public Guid ClienteId { get; set; }
        public Guid EmpleadoId { get; set; }
        public Guid SucursalId { get; set; }
        #endregion
        #region Constructores
        public Telefono(string numero, string caracteristica)
        {
            Id = Guid.NewGuid();
            Numero = numero;
            Caracteristica = caracteristica;
            // ClienteId, EmpleadoId, SucursalId: ¿se resuelve por LinQ las FK?
        }
        public Telefono() : this("Sin Asignar", "Sin Asignar")
        {
        }
        #endregion
    }
}
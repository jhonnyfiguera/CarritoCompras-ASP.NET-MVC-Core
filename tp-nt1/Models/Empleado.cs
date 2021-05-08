namespace tp_nt1.Models
{
    public class Empleado : Usuario
    {
        public override Rol Rol => Rol.Empleado;
    }
}

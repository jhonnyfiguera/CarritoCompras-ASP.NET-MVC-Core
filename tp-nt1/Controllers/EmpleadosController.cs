using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Extensions;
using tp_nt1.Models;

//Empleado
//Los empleados, deben ser agregados por otro empleado o administrador.
//Al momento del alta del empleado se le definirá un username y password.
//También se le asignará a estas cuentas el rol de Empleado automáticamente.
//El empleado puede listar las compras realizadas en el mes en modo listado, ordenado de forma descendente por valor de compra.
//El empleado puede dar de alta otros empleados.
//El empleado puede crear productos, categorias, Sucursales, agregar productos al stock de cada sucursal.
//El empleado puede habilitar y/o deshabilitar productos.

namespace tp_nt1.Controllers
{
    [Authorize(Roles = "Administrador, Empleado")]
    public class EmpleadosController : Controller
    {

        private readonly CarritoDbContext _context;


        public EmpleadosController(CarritoDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Empleados.ToListAsync());
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Empleado empleado, string password)
        {
            try
            {
                password.ValidarPassword();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(Empleado.Password), ex.Message);
            }

            if (_context.Empleados.Any(e => e.Username == empleado.Username))
            {
                ModelState.AddModelError(nameof(empleado.Username), "El Nombre de Usuario ya existe; debes ingresar uno diferente o Iniciar sesión.");
            }

            if (_context.Empleados.Any(e => e.Email == empleado.Email))
            {
                ModelState.AddModelError(nameof(empleado.Email), "El Email ya existe; debes ingresar uno diferente o Iniciar sesión.");
            }

            if (ModelState.IsValid)
            {
                empleado.Id = Guid.NewGuid();
                empleado.FechaAlta = DateTime.Now;
                empleado.Password = password.Encriptar();

                _context.Add(empleado);
                _context.SaveChanges();

                return RedirectToAction(nameof(Details), new { empleado.Id});
            }

            return View(empleado);
        }


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

     
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Empleado empleado, string password)
        {
            if (!string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    password.ValidarPassword();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(Empleado.Password), ex.Message);
                }
            }

            //Pendiente
            if (_context.Empleados.Any(e => e.Email == empleado.Email))
            {
                ModelState.AddModelError(nameof(empleado.Email), "El Email ya existe; debes ingresar uno diferente.");
            }

            if (id != empleado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var empleadoDatabase = _context.Empleados.Find(id);

                    empleadoDatabase.Nombre = empleado.Nombre;
                    empleadoDatabase.Apellido = empleado.Apellido;
                    empleadoDatabase.Telefono = empleado.Telefono;
                    empleadoDatabase.Direccion = empleado.Direccion;
                    empleadoDatabase.Email = empleado.Email;

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        empleadoDatabase.Password = password.Encriptar();
                    }

                    await _context.SaveChangesAsync();

                    TempData["EditIn"] = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(empleado);
        }


        #region Acciones del Empleado
        [Authorize(Roles = nameof(Rol.Empleado))]
        [HttpGet]
        public IActionResult Editarme()
        {
            var username = User.Identity.Name;
            var empleado = _context.Empleados.FirstOrDefault(e => e.Username == username);

            return View(empleado);
        }


        [Authorize(Roles = nameof(Rol.Empleado))]
        [HttpPost]
        public IActionResult Editarme(Empleado empleado, string password)
        {
            if (!string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    password.ValidarPassword();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(Empleado.Password), ex.Message);
                }
            }

            //Pendiente
            if (_context.Empleados.Any(e => e.Email == empleado.Email))
            {
                ModelState.AddModelError(nameof(empleado.Email), "El Email ya existe; debes ingresar uno diferente.");
            }

            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var empleadoDatabase = _context.Empleados.FirstOrDefault(e => e.Username == username);

                empleadoDatabase.Telefono = empleado.Telefono;
                empleadoDatabase.Direccion = empleado.Direccion;
                empleadoDatabase.Email = empleado.Email;

                if (!string.IsNullOrWhiteSpace(password))
                {
                    empleadoDatabase.Password = password.Encriptar();
                }

                _context.SaveChanges();

                TempData["EditIn"] = true;

                return RedirectToAction(nameof(Details), new { empleado.Id });
            }

            return View(empleado);
        }
        #endregion


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool EmpleadoExists(Guid id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}
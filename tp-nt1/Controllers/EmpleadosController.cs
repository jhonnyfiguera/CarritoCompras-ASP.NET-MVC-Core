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



        // GET: Empleados
        public async Task<IActionResult> Index()
        {
            return View(await _context.Empleados.ToListAsync());
        }



        // GET: Empleados/Details/5
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



        // GET: Empleados/Create
        public IActionResult Create()
        {
            return View();
        }



        // POST: Empleados/Create
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
                ModelState.AddModelError(nameof(Cliente.Password), ex.Message);
            }

            if (_context.Clientes.Any(e => e.Username == empleado.Username))
            {
                ModelState.AddModelError(nameof(empleado.Username), "El Nombre de Usuario ya existe; debes ingresar uno diferente o Iniciar sesión.");
            }

            if (_context.Clientes.Any(e => e.Email == empleado.Email))
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
                return RedirectToAction(nameof(AccesosController.Ingresar), "Accesos");
            }
            return View(empleado);
        }



        // GET: Empleados/Edit/5
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



        // POST: Empleados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nombre,Apellido,Telefono,Direccion,Email,Username,Password,FechaAlta")] Empleado empleado)
        {
            if (id != empleado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleado);
                    await _context.SaveChangesAsync();
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



        // GET: Empleados/Delete/5
        [Authorize(Roles = nameof(Rol.Administrador))]
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



        // POST: Empleados/Delete/5
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
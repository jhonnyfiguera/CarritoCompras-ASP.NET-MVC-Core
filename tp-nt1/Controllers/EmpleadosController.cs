using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using tp_nt1.DataBase;
using tp_nt1.Extensions;
using tp_nt1.Models;


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
        public IActionResult Index()
        {
            return View(_context.Empleados.ToList());
        }


        [HttpGet]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = _context.Empleados
                .FirstOrDefault(m => m.Id == id);

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
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = _context.Empleados.Find(id);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

     
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Empleado empleado, string password)
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

            if (_context.Empleados.Any(e => e.Username == empleado.Username && e.Id != id))
            {
                ModelState.AddModelError(nameof(empleado.Username), "El Nombre de Usuario ya existe; debes ingresar uno diferente.");
            }

            if (_context.Empleados.Any(e => e.Email == empleado.Email && e.Id != id))
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
                    empleadoDatabase.Username = empleado.Username;

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        empleadoDatabase.Password = password.Encriptar();
                    }

                    _context.SaveChanges();

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

            var auxEmpleado = _context.Empleados.FirstOrDefaultAsync(e => e.Email == empleado.Email).Result;

            if (_context.Empleados.Any(e => e.Email == empleado.Email) && auxEmpleado.Username != empleado.Username)
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


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpGet]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = _context.Empleados
                .FirstOrDefault(m => m.Id == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var empleado = _context.Empleados.Find(id);
            _context.Empleados.Remove(empleado);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        private bool EmpleadoExists(Guid id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}
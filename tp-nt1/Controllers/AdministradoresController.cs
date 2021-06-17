using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Extensions;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    [Authorize(Roles = nameof(Rol.Administrador))]
    public class AdministradoresController : Controller
    {

        private readonly CarritoDbContext _context;


        public AdministradoresController(CarritoDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Administradores.ToList());
        }

 
        [HttpGet]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = _context.Administradores
                .FirstOrDefault(m => m.Id == id);

            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Administrador administrador, string password)
        {
            try
            {
                password.ValidarPassword();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(Administrador.Password), ex.Message);
            }

            if (_context.Administradores.Any(a => a.Username == administrador.Username))
            {
                ModelState.AddModelError(nameof(Administrador.Username), "El Nombre de Usuario ya existe; debes ingresar uno diferente o Iniciar sesión.");
            }

            if (_context.Administradores.Any(a => a.Email == administrador.Email))
            {
                ModelState.AddModelError(nameof(Administrador.Email), "El Email ya existe; debes ingresar uno diferente o Iniciar sesión.");
            }

            if (ModelState.IsValid)
            {
                administrador.Id = Guid.NewGuid();
                administrador.FechaAlta = DateTime.Now;
                administrador.Password = password.Encriptar();

                _context.Add(administrador);
                _context.SaveChanges();

                return RedirectToAction(nameof(Details), new { administrador.Id });
            }

            return View(administrador);
        }


        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = _context.Administradores.Find(id);

            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Administrador administrador, string password)
        {
            if (!string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    password.ValidarPassword();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(Administrador.Password), ex.Message);
                }
            }

            if (_context.Administradores.Any(a => a.Username == administrador.Username && a.Id != id))
            {
                ModelState.AddModelError(nameof(administrador.Username), "El Nombre de Usuario ya existe; debes ingresar uno diferente.");
            }

            if (_context.Administradores.Any(a => a.Email == administrador.Email && a.Id != id))
            {
                ModelState.AddModelError(nameof(administrador.Email), "El Email ya existe; debes ingresar uno diferente.");
            }

            if (id != administrador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var admDatabase = _context.Administradores.Find(id);

                    admDatabase.Nombre = administrador.Nombre;
                    admDatabase.Apellido = administrador.Apellido;
                    admDatabase.Telefono = administrador.Telefono;
                    admDatabase.Direccion = administrador.Direccion;
                    admDatabase.Email = administrador.Email;
                    admDatabase.Username = administrador.Username;

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        admDatabase.Password = password.Encriptar();
                    }

                    _context.SaveChanges();

                    TempData["EditIn"] = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministradorExists(administrador.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(administrador);
        }


        private bool AdministradorExists(Guid id)
        {
            return _context.Administradores.Any(e => e.Id == id);
        }
    }
}
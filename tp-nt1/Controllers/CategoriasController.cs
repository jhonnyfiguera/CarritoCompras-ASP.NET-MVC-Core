using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using tp_nt1.DataBase;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    public class CategoriasController : Controller
    {

        private readonly CarritoDbContext _context;


        public CategoriasController(CarritoDbContext context)
        {
            _context = context;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Categorias.ToList());
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {

            if (_context.Categorias.Any(c => c.Nombre == categoria.Nombre))
            {
                ModelState.AddModelError(nameof(categoria.Nombre), "El Nombre de Categoria ya existe; debes ingresar uno diferente.");
            }

            if (ModelState.IsValid)
            {
                categoria.Id = Guid.NewGuid();
                _context.Add(categoria);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = _context.Categorias.Find(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (_context.Categorias.Any(c => c.Nombre == categoria.Nombre && c.Id != id))
            {
                ModelState.AddModelError(nameof(categoria.Nombre), "El Nombre de Categoria ya existe; debes ingresar uno diferente.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var categoriaDatabase = _context.Categorias.Find(id);

                    categoriaDatabase.Nombre = categoria.Nombre;
                    categoriaDatabase.Descripcion = categoria.Descripcion;
              
                    _context.SaveChanges();

                    TempData["EditIn"] = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
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

            return View(categoria);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpGet]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = _context.Categorias
                .FirstOrDefault(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var categoria = _context.Categorias.Find(id);
            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        private bool CategoriaExists(Guid id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
}
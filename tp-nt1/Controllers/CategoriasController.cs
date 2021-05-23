using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Models;

//Enunciado Categoría:
//No pueden eliminarse del sistema.

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categorias.ToListAsync());
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
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
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                categoria.Id = Guid.NewGuid();
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var categoriaDatabase = _context.Categorias.Find(id);

                    categoriaDatabase.Nombre = categoria.Nombre;
                    categoriaDatabase.Descripcion = categoria.Descripcion;
              
                    await _context.SaveChangesAsync();

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


        #region No se puede Eliminar una Categoria
        //[Authorize(Roles = nameof(Rol.Administrador))]
        //[HttpGet]
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var categoria = await _context.Categorias
        //        .FirstOrDefaultAsync(m => m.Id == id);

        //    if (categoria == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(categoria);
        //}


        //[Authorize(Roles = nameof(Rol.Administrador))]
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var categoria = await _context.Categorias.FindAsync(id);
        //    _context.Categorias.Remove(categoria);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        #endregion


        private bool CategoriaExists(Guid id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
}
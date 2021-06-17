using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    public class SucursalesController : Controller
    {

        private readonly CarritoDbContext _context;

        public SucursalesController(CarritoDbContext context)
        {
            _context = context;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Sucursal.ToList());
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
        public IActionResult Create(Sucursal sucursal)
        {
            if (_context.Sucursal.Any(s => s.Nombre == sucursal.Nombre))
            {
                ModelState.AddModelError(nameof(sucursal.Nombre), "El Nombre de Sucursal ya existe; debes ingresar uno diferente.");
            }

            if (_context.Sucursal.Any(s => s.Direccion == sucursal.Direccion))
            {
                ModelState.AddModelError(nameof(sucursal.Direccion), "La direccion de Sucursal ya existe; debes ingresar uno diferente.");
            }

            if (ModelState.IsValid)
            {
                sucursal.Id = Guid.NewGuid();
                _context.Add(sucursal);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
           
            return View(sucursal);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = _context.Sucursal
               .Include(s => s.StockItems)
               .FirstOrDefault(m => m.Id == id);

            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Sucursal sucursal)
        {
            if (id != sucursal.Id)
            {
                return NotFound();
            }

            if (_context.Sucursal.Any(s => s.Nombre == sucursal.Nombre && s.Id != id))
            {
                ModelState.AddModelError(nameof(sucursal.Nombre), "El Nombre de Sucursal ya existe; debes ingresar uno diferente.");
            }

            if (_context.Sucursal.Any(s => s.Direccion == sucursal.Direccion && s.Id != id))
            {
                ModelState.AddModelError(nameof(sucursal.Nombre), "La direccion ya existe; debes ingresar una diferente.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sucursal);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SucursalExists(sucursal.Id))
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

            return View(sucursal);
        }


        [HttpGet]
        [Authorize(Roles = "Administrador, Empleado")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = _context.Sucursal
                .Include(s => s.StockItems)
                .FirstOrDefault(m => m.Id == id);

            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var sucursal = _context.Sucursal
                .Include(s => s.StockItems)
                .FirstOrDefault(m => m.Id == id);

            var stock = sucursal.StockItems.Sum(s => s.Cantidad);
  
            if (stock == 0)
            {
                _context.Sucursal.Remove(sucursal);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MensajeError = "No se puede eliminar Sucursal, total productos en Stock : ";
            ViewBag.stock = stock;

            return View(sucursal);
        }


        private bool SucursalExists(Guid id)
        {
            return _context.Sucursal.Any(e => e.Id == id);
        }
    }
}
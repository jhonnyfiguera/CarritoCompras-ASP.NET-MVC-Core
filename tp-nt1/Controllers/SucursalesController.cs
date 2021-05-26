using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Models;

//Sucursal
//Cada sucursal tendrá su propio stock y sus datos de locación y contacto.
//Por el mercado tan volátil las sucursales pueden crearse y eliminarse en todo momento.
//Para poder eliminar una sucursal, la misma no tiene que tener productos en su stock.

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sucursal.ToListAsync());
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursal
                .Include(s => s.StockItems)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
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
        public async Task<IActionResult> Create(Sucursal sucursal)
        {
            if (ModelState.IsValid)
            {
                sucursal.Id = Guid.NewGuid();
                _context.Add(sucursal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           
            return View(sucursal);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursal
               .Include(s => s.StockItems)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Sucursal sucursal)
        {
            if (id != sucursal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sucursal);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursal
                .Include(s => s.StockItems)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var sucursal = await _context.Sucursal
                .Include(s => s.StockItems)
                .FirstOrDefaultAsync(m => m.Id == id);

            var stock = sucursal.StockItems.Sum(s => s.Cantidad);
  
            if (stock == 0)
            {
                _context.Sucursal.Remove(sucursal);
                await _context.SaveChangesAsync();
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
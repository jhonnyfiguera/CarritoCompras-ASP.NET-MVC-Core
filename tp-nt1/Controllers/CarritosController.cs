using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    [Authorize(Roles = nameof(Rol.Cliente))]
    public class CarritosController : Controller
    {

        private readonly CarritoDbContext _context;


        public CarritosController(CarritoDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var carritoDbContext = _context.Carritos.Include(c => c.Cliente);
            return View(await carritoDbContext.ToListAsync());
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id || m.ClienteId == id);

            if (carrito == null)
            {
                return NotFound();
            }

            var listaProductos = _context.CarritoItems.Select(c => c.Subtotal).ToList();
            var precioTotal = (decimal) 0;
            foreach (var aux in listaProductos)
            {
                precioTotal += aux;
            }

            carrito.Subtotal = precioTotal;
            await _context.SaveChangesAsync();

            return View(carrito);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos.FindAsync(id);
            if (carrito == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Activo,ClienteId,Subtotal")] Carrito carrito)
        {
            if (id != carrito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var carrito = await _context.Carritos.FindAsync(id);
            _context.Carritos.Remove(carrito);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult VaciarCarrito()
        {
            var idClienteLogueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito =
                _context.Carritos
                .Include(c => c.CarritosItems).ThenInclude(m => m.Producto)
                .Include(c => c.Cliente)
                .FirstOrDefault(m => m.ClienteId == idClienteLogueado && m.Activo == true);

            if (carrito == null)
            {
                return NotFound();
            }

            if (carrito.CarritosItems.Count == 0)
            {
                TempData["Vacio"] = true;
                return RedirectToAction(nameof(CarritoItemsController.MisItems), "CarritoItems");
            }

            return View(carrito.CarritosItems);
        }


        [HttpPost, ActionName("VaciarCarrito")]
        [ValidateAntiForgeryToken]
        public IActionResult VaciarCarritoConfirmar()
        {
            var idClienteLoqueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito =
                _context.Carritos
                .Include(m => m.CarritosItems)
                .Include(m => m.Cliente)
                .FirstOrDefault(m => m.ClienteId == idClienteLoqueado && m.Activo == true);

            if (carrito.CarritosItems != null)
            {
                carrito.CarritosItems.Clear();
                carrito.Subtotal = 0;
                _context.SaveChanges();
            }
            return RedirectToAction("MisItems", "CarritoItems");
        }

        private bool CarritoExists(Guid id)
        {
            return _context.Carritos.Any(e => e.Id == id);
        }
    }
}
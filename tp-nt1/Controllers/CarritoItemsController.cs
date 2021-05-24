using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tp_nt1.DataBase;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    [Authorize(Roles = nameof(Rol.Cliente))]
    public class CarritoItemsController : Controller
    {

        private readonly CarritoDbContext _context;



        public CarritoItemsController(CarritoDbContext context)
        {
            _context = context;
        }



        // GET: CarritoItems
        public async Task<IActionResult> Index()
        {
            var carritoDbContext = _context.CarritoItems.Include(c => c.Carrito).Include(c => c.Producto);
            return View(await carritoDbContext.ToListAsync());
        }

        // GET: CarritoItems/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItems
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        // GET: CarritoItems/Create
        public IActionResult Create(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = _context.Productos.Find(id);
            if (producto == null)
            {
                return NotFound();
            }

            var cId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var carrito =  _context.Carritos
            .Include(c => c.Cliente)
            .FirstOrDefault(m => m.ClienteId == cId);
            if (carrito == null)
            {
                return NotFound();
            }

            var carritoItem = new CarritoItem
            {
                Id = Guid.NewGuid(),
                CarritoId = carrito.Id,
                ProductoId = producto.Id,
                ValorUnitario = producto.PrecioVigente,
                Cantidad = 1,
                Subtotal = producto.PrecioVigente * 1,
            };

            return View(carritoItem);
        }

        // POST: CarritoItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarritoItem carritoItemAux, Guid? id)
        {

            if (id == null || carritoItemAux.Id == null)
            {
                return NotFound();
            }

            var producto = _context.Productos.Find(id);
            if (producto == null)
            {
                return NotFound();
            }

            var cId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var carrito = _context.Carritos
            .Include(c => c.Cliente)
            .FirstOrDefault(m => m.ClienteId == cId);
            if (carrito == null)
            {
                return NotFound();
            }

            var carritoItem = new CarritoItem
            {
                Id = Guid.NewGuid(),
                ProductoId = producto.Id,
                CarritoId = carrito.Id,
                ValorUnitario = producto.PrecioVigente,
                Cantidad = carritoItemAux.Cantidad,
                Subtotal = producto.PrecioVigente * carritoItemAux.Cantidad,
            };
            _context.Add(carritoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: CarritoItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItems.FindAsync(id);
            if (carritoItem == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // POST: CarritoItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CarritoId,ProductoId,ValorUnitario,Cantidad,Subtotal")] CarritoItem carritoItem)
        {
            if (id != carritoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carritoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoItemExists(carritoItem.Id))
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
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // GET: CarritoItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItems
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        // POST: CarritoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var carritoItem = await _context.CarritoItems.FindAsync(id);
            _context.CarritoItems.Remove(carritoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoItemExists(Guid id)
        {
            return _context.CarritoItems.Any(e => e.Id == id);
        }
    }
}

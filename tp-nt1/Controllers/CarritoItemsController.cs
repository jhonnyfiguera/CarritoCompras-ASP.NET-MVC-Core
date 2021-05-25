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



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var carritoDbContext = _context.CarritoItems.Include(c => c.Carrito).Include(c => c.Producto);
            return View(await carritoDbContext.ToListAsync());
        }

        [HttpGet]
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

        [HttpGet]
        public IActionResult Agregar(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = _context.Productos.Find(id);
            if (producto == null || producto.Activo == false)
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
                Producto = producto,
                Carrito = carrito,
                ValorUnitario = producto.PrecioVigente,
                Cantidad = 1,
                Subtotal = producto.PrecioVigente * 1,
            };

            return View(carritoItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(int cantidad, Guid? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var producto = _context.Productos.Find(id);
            if (producto == null || producto.Activo == false)
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

            var listaProductos = _context.CarritoItems.Select(p => p.Producto.Nombre).ToList();
            var indice = 0;
            var encontro = false;            
            while (!encontro && indice < listaProductos.Count)
            {
               if (listaProductos[indice].Equals(producto.Nombre))
                {
                    encontro = true;
                }
                indice++;
            }

            if (encontro)
            {
                var miCarrito2 = _context.CarritoItems.FirstOrDefault(y => y.ProductoId == id);
                var carritoItemDataBase = _context.CarritoItems.Find(miCarrito2.Id);
                var listaCantidad = _context.CarritoItems.Where(p => p.Producto.Nombre == producto.Nombre).Select(c => c.Cantidad).ToList();
                var nuevaCantidad = listaCantidad[0] + cantidad;
                var listaSubtotal = _context.CarritoItems.Where(p => p.Producto.Nombre == producto.Nombre).Select(c => c.Subtotal).ToList();
                var nuevoSubtotal = listaSubtotal[0] + (producto.PrecioVigente * cantidad);
                carritoItemDataBase.Cantidad = nuevaCantidad;
                carritoItemDataBase.Subtotal = nuevoSubtotal;
                await _context.SaveChangesAsync();
            } 
            else 
            {
                var carritoItem = new CarritoItem
                {
                    Id = Guid.NewGuid(),
                    ProductoId = producto.Id,
                    CarritoId = carrito.Id,
                    ValorUnitario = producto.PrecioVigente,
                    Cantidad = cantidad,
                    Subtotal = producto.PrecioVigente * cantidad,
                    Producto = producto,
                    Carrito = carrito,
                };
                _context.Add(carritoItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItemAux = await _context.CarritoItems.FindAsync(id);
            if (carritoItemAux == null)
            {
                return NotFound();
            }

            var producto = _context.Productos.Include(p => p.Categoria).FirstOrDefault(p => p.Id == carritoItemAux.ProductoId);

            var carritoItem = new CarritoItem
            {
                Id = carritoItemAux.Id,
                CarritoId = carritoItemAux.CarritoId,
                ProductoId = carritoItemAux.ProductoId,
                Producto = producto,
                ValorUnitario = producto.PrecioVigente,
                Cantidad = carritoItemAux.Cantidad,
                Subtotal = carritoItemAux.Subtotal,
            };

            //ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoId);
            //ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoId);
            return View(carritoItem);
        }

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

        [HttpGet]
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

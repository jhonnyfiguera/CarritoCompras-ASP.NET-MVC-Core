using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    public class ComprasController : Controller
    {
        private readonly CarritoDbContext _context;

        public ComprasController(CarritoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var carritoDbContext = _context.Compras.Include(c => c.Carrito).Include(c => c.Cliente);
            return View(await carritoDbContext.ToListAsync());
        }

        [HttpGet]
        public IActionResult Details(Sucursal sucursalSeleccionada)
        {
            if (sucursalSeleccionada == null)
            {
                return NotFound();
            }

            var idClienteLogueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cliente = _context.Clientes
                .Include(c => c.Compras)
                .FirstOrDefault(i => i.Id == idClienteLogueado);
            var ultimaCompra = cliente.Compras[0];
            var compra = _context.Compras.FirstOrDefault(c => c.Id == ultimaCompra.Id);
            var carrito = _context.Carritos
                .Include(i => i.CarritosItems).ThenInclude(p => p.Producto)
                .FirstOrDefault(c => c.Id == compra.CarritoId);

            Compra compraAux = new Compra
            {
                Id = compra.Id,
                Total = compra.Total,
                ClienteId = compra.ClienteId,
                Cliente = cliente,
                CarritoId = compra.CarritoId,
                Carrito = carrito
            };

            Tuple<Compra, Sucursal> modelAux = new Tuple<Compra, Sucursal>(compra, sucursalSeleccionada);
            return View(modelAux);
        }

        [HttpGet]
        public IActionResult Create()
        {

            var idClienteLogueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito =
                _context.Carritos
                .Include(c => c.CarritosItems).ThenInclude(m => m.Producto)
                .Include(c => c.Cliente)
                .FirstOrDefault(m => m.ClienteId == idClienteLogueado && m.Activo == true);

            if (carrito.CarritosItems.Count == 0)
            {
                return RedirectToAction(nameof(CarritoItemsController.MisItems), "CarritoItems");
            }

            ViewData["SucursalId"] = new SelectList(_context.Sucursal, "Id", "Direccion");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Guid? sucursalId)
        {
            if (sucursalId == null)
            {
                return NotFound();
            }

            var sucursalSeleccionada =
            _context.Sucursal
            .Include(s => s.StockItems).ThenInclude(m => m.Producto)
            .FirstOrDefault(m => m.Id == sucursalId);

            var idClienteLogueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito =
                _context.Carritos
                .Include(c => c.CarritosItems).ThenInclude(m => m.Producto)
                .Include(c => c.Cliente).ThenInclude(m => m.Compras) //nuevo
                .FirstOrDefault(m => m.ClienteId == idClienteLogueado && m.Activo == true);

            if (sucursalSeleccionada == null || carrito == null)
            {
                return NotFound();
            }

            #region Metodo Privado para validar stock, metodo recibe sucural y la lista de los items del carrito y devuelve boolean 
            var sinStock = false;
            var indice = 0;
            while (!sinStock && indice < carrito.CarritosItems.Count)
            {
                var itemCarrito = carrito.CarritosItems[indice];
                var validoStock = sucursalSeleccionada.StockItems.FirstOrDefault(p => p.ProductoId == itemCarrito.ProductoId && p.Cantidad >= itemCarrito.Cantidad);
                if (validoStock == null)
                {
                    sinStock = true;
                }
                indice++;
            }
            #endregion

            if (!sinStock) 
            {
                #region Metodo de restar stock en sucursal
                foreach (var item in carrito.CarritosItems)
                {
                    var itemStock = sucursalSeleccionada.StockItems.FirstOrDefault(p => p.ProductoId == item.ProductoId);
                    itemStock.Cantidad -= item.Cantidad;
                    _context.SaveChanges();
                }
                #endregion

                #region Metodo para crear la compra
                Compra compra = new Compra
                {
                    Id = Guid.NewGuid(),
                    ClienteId = idClienteLogueado,
                    Cliente = carrito.Cliente,
                    CarritoId = carrito.Id,
                    Carrito = carrito,
                    Total = carrito.CarritosItems.Sum(s => s.Subtotal), // Cuando carrito tenga el subtotal actualizado cambiar esta linea
                };
                _context.Add(compra);

                #endregion

                #region Metodo para inactivar carrito y crear
                carrito.Activo = false;
                carrito.Subtotal = carrito.CarritosItems.Sum(s => s.Subtotal);
                Carrito nuevoCarrito = new Carrito
                {
                    Id = Guid.NewGuid(),
                    Activo = true,
                    ClienteId = idClienteLogueado,
                    Subtotal = 0
                };
                _context.Add(nuevoCarrito);
                #endregion

                 _context.SaveChanges();
                return RedirectToAction(nameof(Details), sucursalSeleccionada) ;
            }
            else
            {
                //recorro sucursales
                //Retorno varios botones con diferentes vista
            }



            ViewData["SucursalId"] = new SelectList(_context.Sucursal, "Id", "Direccion");
            return View();
        }

        // GET: Compras/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", compra.ClienteId);
            return View(compra);
        }

        // POST: Compras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Total,ClienteId,CarritoId")] Compra compra)
        {
            if (id != compra.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompraExists(compra.Id))
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
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", compra.ClienteId);
            return View(compra);
        }

        // GET: Compras/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var compra = await _context.Compras.FindAsync(id);
            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompraExists(Guid id)
        {
            return _context.Compras.Any(e => e.Id == id);
        }
    }
}

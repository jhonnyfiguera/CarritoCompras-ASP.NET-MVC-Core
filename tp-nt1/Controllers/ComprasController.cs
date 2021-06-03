using Microsoft.AspNetCore.Authorization;
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


        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var carritoDbContext = _context.Compras.Include(c => c.Carrito).Include(c => c.Cliente);
            return View(await carritoDbContext.ToListAsync());
        }


        [Authorize(Roles = nameof(Rol.Cliente))]
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
                TempData["Vacio"] = true;
                return RedirectToAction(nameof(CarritoItemsController.MisItems), "CarritoItems");
            }

            ViewData["SucursalId"] = new SelectList(_context.Sucursal, "Id", "Direccion");

            return View();
        }


        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearCompra(Guid? sucursalId)
        {
            if (sucursalId == null)
            {
                return NotFound();
            }

            var idClienteLogueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito =
                _context.Carritos
                .Include(c => c.CarritosItems).ThenInclude(m => m.Producto)
                .Include(c => c.Cliente).ThenInclude(m => m.Compras)
                .FirstOrDefault(m => m.ClienteId == idClienteLogueado && m.Activo == true);

            var miSucursal = _context.Sucursal.Find(sucursalId);

            if (miSucursal == null || carrito == null)
            {
                return NotFound();
            }

            if (!SinStockEnSucursal(miSucursal.Id, carrito.CarritosItems))
            {
                foreach (var item in carrito.CarritosItems)
                {
                    var stockItemSucursal = miSucursal.StockItems.FirstOrDefault(p => p.ProductoId == item.ProductoId);
                    stockItemSucursal.Cantidad -= item.Cantidad;
                    _context.SaveChanges();
                }

                Compra compra = new Compra
                {
                    Id = Guid.NewGuid(),
                    ClienteId = idClienteLogueado,
                    Cliente = carrito.Cliente,
                    CarritoId = carrito.Id,
                    Carrito = carrito,
                    Total = carrito.Subtotal,
                };
                _context.Add(compra);

                carrito.Activo = false;
                Carrito nuevoCarrito = new Carrito
                {
                    Id = Guid.NewGuid(),
                    Activo = true,
                    ClienteId = idClienteLogueado,
                    Subtotal = 0
                };
                _context.Add(nuevoCarrito);

                _context.SaveChanges();

                Tuple<Compra, Sucursal> modelo = new Tuple<Compra, Sucursal>(compra, miSucursal);

                return View(modelo);
            }
            else
            {
                var otrasSucursales = _context.Sucursal.Where(m => m.Id != miSucursal.Id).ToList();

                var sucursalesConStock = new List<Sucursal>();

                foreach (var sucursal in otrasSucursales)
                {
                    if (!SinStockEnSucursal(sucursal.Id, carrito.CarritosItems))
                    {
                        sucursalesConStock.Add(sucursal);
                    }
                }

                if (sucursalesConStock.Count > 0)
                {
                    ViewBag.MensajeSinStock = "En la " + miSucursal.Nombre + ", no contamos con stock en algunos productos que seleccionastes";

                    ViewBag.MensajeContinuarCompra = "En las siguientes sucursales, si contamos con stock de todos tus productos, para finalizar tu compra selecciona alguna de ellas";
                    ViewData["SucursalId"] = new SelectList(sucursalesConStock, "Id", "Direccion");

                    return View(nameof(Create));
                }
            }

            ViewBag.MensajeNohayStock = "En ninguna sucursal contamos con stock de algunos productos que seleccionastes";

            return View(nameof(Create));
        }


        private bool SinStockEnSucursal(Guid? id, List<CarritoItem> carritosItems)
        {
            var sucursal =
            _context.Sucursal
            .Include(s => s.StockItems).ThenInclude(m => m.Producto)
            .FirstOrDefault(m => m.Id == id);

            var sinStock = false;
            var indice = 0;

            while (!sinStock && indice < carritosItems.Count)
            {
                var itemCarrito = carritosItems[indice];
                var itemSucursal = sucursal.StockItems.FirstOrDefault(m => m.ProductoId == itemCarrito.ProductoId && m.Cantidad >= itemCarrito.Cantidad);

                if (itemSucursal == null)
                {
                    sinStock = true;
                }
                indice++;
            }
            return sinStock;
        }


        private bool CompraExists(Guid id)
        {
            return _context.Compras.Any(e => e.Id == id);
        }
    }
}
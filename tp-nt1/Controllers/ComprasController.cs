using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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


        [Authorize(Roles = nameof(Rol.Empleado))]
        [HttpGet]
        public IActionResult ComprasDelMes()
        {
            var comprasDelMes = _context.Compras
                .Include(c => c.Cliente)
                .Include(c => c.Carrito).ThenInclude(c => c.CarritosItems).ThenInclude(c => c.Producto)
                .Where(c => c.FechaCompra.Month == DateTime.Now.Month && c.FechaCompra.Year == DateTime.Now.Year)
                .OrderByDescending(compra => ((int)compra.Total)).ToList();

            ViewBag.Titulo = "Compras "+ DateTime.Now.ToString("MMMM") + " del " + DateTime.Now.Year;

            return View("ComprasReportes", comprasDelMes);
        }

        [Authorize(Roles = nameof(Rol.Empleado))]
        [HttpGet]
        public IActionResult ComprasHistorial()
        {
            var comprasHistorial = _context.Compras
                .Include(c => c.Cliente)
                .Include(c => c.Carrito).ThenInclude(c => c.CarritosItems).ThenInclude(c => c.Producto)
                .OrderByDescending(compra => compra.FechaCompra).ToList();
            ViewBag.Titulo = "Historial de Compras";
            return View("ComprasReportes", comprasHistorial);
        }


        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpGet]
        public IActionResult ComprasRealizadas()
        {
            var idClienteLogueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var compras = _context.Compras
                .Include(c => c.Carrito).ThenInclude(c => c.CarritosItems).ThenInclude(c => c.Producto)
                .Include(c => c.Cliente).ThenInclude(c => c.Compras)
                .Where(c => c.ClienteId == idClienteLogueado);

            return View(compras.ToList());
        }


        [Authorize]
        [HttpGet]
        public IActionResult DetalleCompra(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra =
                _context.Compras
                .Include(c => c.Cliente)
                .Include(c => c.Carrito).ThenInclude(c => c.CarritosItems).ThenInclude(c => c.Producto)
                .FirstOrDefault(m => m.Id == id);

            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
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

            if (HayStockEnSucursal(miSucursal.Id, carrito.CarritosItems))
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
                    FechaCompra = DateTime.Now,
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
                    if (HayStockEnSucursal(sucursal.Id, carrito.CarritosItems))
                    {
                        sucursalesConStock.Add(sucursal);
                    }
                }

                if (sucursalesConStock.Count > 0)
                {
                    ViewBag.MensajeSinStock = "Lo sentimos, en " + miSucursal.Nombre + " no contamos con stock suficiente para completar el Pedido. " +
                         "¡Por favor, intenta en otra Sucursal!";

                    ViewData["SucursalId"] = new SelectList(sucursalesConStock, "Id", "Direccion");

                    return View(nameof(Create));
                }
            }

            TempData["SinStock"] = true;
            return RedirectToAction(nameof(CarritoItemsController.MisItems), "CarritoItems");
        }


        private bool HayStockEnSucursal(Guid? id, List<CarritoItem> carritosItems)
        {
            var sucursal =
            _context.Sucursal
            .Include(s => s.StockItems).ThenInclude(m => m.Producto)
            .FirstOrDefault(m => m.Id == id);

            return carritosItems.All(c => sucursal.StockItems
                .Any(s => s.ProductoId == c.ProductoId && s.Cantidad >= c.Cantidad));
        }
    }
}
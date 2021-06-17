using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using tp_nt1.DataBase;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    [Authorize(Roles = nameof(Rol.Empleado))]

    public class StockItemsController : Controller
    {

        private readonly CarritoDbContext _context;

        public StockItemsController(CarritoDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var carritoDbContext = _context.StockItems.Include(s => s.Producto).Include(s => s.Sucursal);
            return View(carritoDbContext.ToList());
        }


        [HttpGet]
        public IActionResult AgregarStock()
        {
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre");
            ViewData["SucursalId"] = new SelectList(_context.Sucursal, "Id", "Direccion");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarStock(StockItem stockItem)
        {
            var itemAuxiliar =
            _context.StockItems
            .FirstOrDefault(f => f.ProductoId == stockItem.ProductoId
            && f.SucursalId == stockItem.SucursalId);

            if (itemAuxiliar != null)
            {
                itemAuxiliar.Cantidad += stockItem.Cantidad;
            }
            else
            {
                stockItem.Id = Guid.NewGuid();
                _context.Add(stockItem);
            }
            _context.SaveChanges();
            TempData["EditIn"] = true;

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockItem = _context.StockItems
                .Include(s => s.Producto)
                .Include(s => s.Sucursal)
                .FirstOrDefault(m => m.Id == id);

            if (stockItem == null)
            {
                return NotFound();
            }
            return View(stockItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, int Cantidad)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockItemDataBase = _context.StockItems.Find(id);

            stockItemDataBase.Cantidad = Cantidad;

            _context.SaveChanges();
            TempData["EditIn"] = true;
            return RedirectToAction(nameof(Index));
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Models;

//No se puede eliminarse del sistema.
//Solo los productos pueden dehabilitarse.

namespace tp_nt1.Controllers
{
    public class ProductosController : Controller
    {

        private readonly CarritoDbContext _context;


        public ProductosController(CarritoDbContext context)
        {
            _context = context;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            var productos = 
                _context.Productos
                .Include(p => p.Categoria)
                .ToList();

            List<String> productosSinStock = new List<String>();

            foreach (var p in productos)
            {
                if (!HayStockEnSucursales(p.Id))
                {
                    productosSinStock.Add(p.Nombre);
                }
            }

            Tuple<List<Producto>, List<string>> modelo = new Tuple<List<Producto>, List<string>>(productos, productosSinStock);

            return View(modelo);
        }


        private bool HayStockEnSucursales(Guid? idProducto)
        {
            var sucursales =
                _context.Sucursal
                .Include(p => p.StockItems)
                .ThenInclude(p => p.Producto)
                .ToList();

            int i = 0;
            bool hayStock = false;

            while (!hayStock && i < sucursales.Count)
            {
                var sucursal =
                    _context.Sucursal
                    .Include(p => p.StockItems)
                    .ThenInclude(p => p.Producto)
                    .FirstOrDefault(m => m.Id == sucursales[i].Id);

                if (sucursal.StockItems.Any(p => p.ProductoId == idProducto && p.Cantidad > 0))
                {
                    hayStock =  true;
                }
                i++;
            }
            return hayStock;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Categoria"] = new SelectList(_context.Categorias, "Id", "Nombre");

            return View();
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {

            if (_context.Productos.Any(p => p.Nombre == producto.Nombre))
            {
                ModelState.AddModelError(nameof(producto.Nombre), "El Nombre del Producto ya existe; debes ingresar uno diferente.");
            }

            if (ModelState.IsValid)
            {
                producto.Id = Guid.NewGuid();
                _context.Add(producto);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categoria"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);

            return View(producto);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpGet]
        public IActionResult Edit(Guid? id)
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
            ViewData["Categoria"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);

            return View(producto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (_context.Productos.Any(c => c.Nombre == producto.Nombre && c.Id != id))
            {
                ModelState.AddModelError(nameof(producto.Nombre), "El Nombre del Producto ya existe; debes ingresar uno diferente.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            ViewData["Categoria"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }


        #region No se puede Eliminar un Producto
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var producto = await _context.Productos.FindAsync(id);
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion


        private bool ProductoExists(Guid id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
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
        public IActionResult Index(string nombre, Guid? categoriaId, int? precio, string estado)
        {

            var estadoAux = false;

            if (!string.IsNullOrWhiteSpace(estado) && estado.SequenceEqual("Activo"))
            {
                estadoAux = true;
            }

            var productos =
                _context.Productos
                .Include(x => x.Categoria)
                .Where(x => (string.IsNullOrWhiteSpace(nombre) || EF.Functions.Like(x.Nombre, $"%{nombre}%"))
                             && (string.IsNullOrWhiteSpace(estado) || x.Activo == estadoAux) 
                            && (!precio.HasValue || (int)x.PrecioVigente <= precio.Value)
                            && (!categoriaId.HasValue || x.CategoriaId == categoriaId.Value)).ToList();

            ViewBag.Nombre = nombre;
            ViewBag.Categorias = new SelectList(_context.Categorias, nameof(Categoria.Id), nameof(Categoria.Nombre), categoriaId);
            ViewBag.Precio = new SelectList(new int[6] { 200, 400, 700, 900, 1300, 1600 }, precio);
            ViewBag.Estado = new SelectList(new string[2] { "Activo", "Inactivo" }, estado);

            List<string> sinStockProductos = new List<String>();

            foreach (var p in productos)
            {
                if (!_context.StockItems.Any(s => s.ProductoId == p.Id && s.Cantidad > 0))
                {
                    sinStockProductos.Add(p.Nombre);
                }
            }

            Tuple<List<Producto>, List<string>> modelo = new Tuple<List<Producto>, List<string>>(productos, sinStockProductos);

            return View(modelo);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = 
                _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefault(m => m.Id == id);

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

        private bool ProductoExists(Guid id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
        #endregion
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Extensions;
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
        public IActionResult MisItems()
        {
            var idClienteLogueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito =
                _context.Carritos
                .Include(c => c.CarritosItems).ThenInclude(m => m.Producto)
                .Include(c => c.Cliente)
                .FirstOrDefault(m => m.ClienteId == idClienteLogueado && m.Activo == true);

            return View(carrito.CarritosItems);
        }


        [HttpGet]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItem =  
                _context.CarritoItems
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefault(m => m.Id == id);

            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }


        [HttpGet]
        public IActionResult Agregar(Guid? productoId)
        {
            if (productoId == null)
            {
                return NotFound();
            }

            var producto = _context.Productos.Find(productoId);

            if (producto == null || producto.Activo == false)
            {
                return NotFound();
            }

            CarritoItem carritoItem = new CarritoItem
            {
                Producto = producto,
            };

            return View(carritoItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Agregar(int cantidad, Guid? productoId)
        {
            bool cantidadValida = true;
            try
            {
                cantidad.ValidarInput();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(CarritoItem.Cantidad), ex.Message);
                cantidadValida = false;
            }

            if (productoId == null)
            {
                return NotFound();
            }

            var producto = 
                _context.Productos
                .Include(c => c.Categoria)
                .FirstOrDefault(m => m.Id == productoId);

            if (producto == null || producto.Activo == false)
            {
                return NotFound();
            }

            var idClienteLogueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito = 
                _context.Carritos
                .Include(c => c.CarritosItems)
                .Include(c => c.Cliente)
                .FirstOrDefault(m => m.ClienteId == idClienteLogueado && m.Activo == true);

            if (carrito == null)
            {
                return NotFound();
            }
           
            if (cantidadValida)
            {
                var miCarritoItems = carrito.CarritosItems.FirstOrDefault(m => m.ProductoId == productoId);
                if (miCarritoItems != null)
                {
                    miCarritoItems.Cantidad += cantidad;
                    miCarritoItems.Subtotal += (producto.PrecioVigente * cantidad);
                }
                else
                {
                    CarritoItem carritoItem = new CarritoItem
                    {
                        Id = Guid.NewGuid(),
                        ProductoId = producto.Id,
                        Producto = producto,
                        CarritoId = carrito.Id,
                        Carrito = carrito,
                        ValorUnitario = producto.PrecioVigente,
                        Cantidad = cantidad,
                        Subtotal = producto.PrecioVigente * cantidad,
                    };
                    _context.Add(carritoItem);
                }
                carrito.Subtotal = carrito.CarritosItems.Sum(s => s.Subtotal);
                _context.SaveChanges();
                return RedirectToAction(nameof(MisItems));
            }

            CarritoItem carritoAux = new CarritoItem
            {
                Producto = producto,
            };
            return View(carritoAux);
        }


        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miCarritoItem = _context.CarritoItems
                .Include(p => p.Producto)
                .FirstOrDefault(c => c.Id == id);

            if (miCarritoItem == null)
            {
                return NotFound();
            }

            return View(miCarritoItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, int cantidad)
        {
            bool cantidadValida = true;
            try
            {
                cantidad.ValidarInput();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(CarritoItem.Cantidad), ex.Message);
                cantidadValida = false;
            }

            if (id == null)
            {
                return NotFound();
            }

            var miCarritoItems =
                _context.CarritoItems
                .Include(m => m.Carrito).ThenInclude(m => m.CarritosItems)
                .Include(m => m.Producto)
                .FirstOrDefault(c => c.Id == id);

            if (miCarritoItems != null && cantidadValida)
            {
                miCarritoItems.Cantidad = (int)cantidad;
                miCarritoItems.Subtotal = (miCarritoItems.Producto.PrecioVigente * (int)cantidad);
                miCarritoItems.Carrito.Subtotal = miCarritoItems.Carrito.CarritosItems.Sum(s => s.Subtotal);
                _context.SaveChanges();
                TempData["EditIn"] = true;
                return RedirectToAction(nameof(MisItems));
            }

            return View(miCarritoItems);
        }


        [HttpGet]
        public IActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItem = _context.CarritoItems
                .Include(c => c.Producto)
                .Include(c => c.Carrito)
                .FirstOrDefault(m => m.Id == id);

            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }


        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmar(Guid id)
        {
            var carritoItem =
             _context.CarritoItems
            .Include(m => m.Carrito).ThenInclude(m => m.CarritosItems)
            .Include(m => m.Producto)
            .FirstOrDefault(c => c.Id == id);

            carritoItem.Carrito.Subtotal = carritoItem.Carrito.CarritosItems.Sum(s => s.Subtotal) - carritoItem.Subtotal;
            _context.CarritoItems.Remove(carritoItem);

            _context.SaveChanges();

            return RedirectToAction(nameof(MisItems));
        }


        private bool CarritoItemExists(Guid id)
        {
            return _context.CarritoItems.Any(e => e.Id == id);
        }
    }
}
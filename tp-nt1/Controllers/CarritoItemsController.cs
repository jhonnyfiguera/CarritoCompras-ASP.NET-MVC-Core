using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class CarritoItemsController : Controller
    {

        private readonly CarritoDbContext _context;


        public CarritoItemsController(CarritoDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Administrador, Empleado")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var carritoDbContext = 
                _context.CarritoItems
                .Include(c => c.Carrito)
                .Include(c => c.Producto);

            return View(await carritoDbContext.ToListAsync());
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

            if (carrito.CarritosItems.Count == 0)
            {
                ViewBag.Error = "Para Finalizar tu compra el Carrito no puede estar Vacio.";
            }

            return View(carrito.CarritosItems);
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItem = await 
                _context.CarritoItems
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

            var idClienteLoqueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito =  
                _context.Carritos
                .Include(c => c.Cliente)
                .FirstOrDefault(m => m.ClienteId == idClienteLoqueado && m.Activo == true);

            if (carrito == null)
            {
                return NotFound();
            }

            CarritoItem carritoItem = new CarritoItem
            {
                Id = Guid.NewGuid(),
                CarritoId = carrito.Id,
                Carrito = carrito,
                ProductoId = producto.Id,
                Producto = producto,
                ValorUnitario = producto.PrecioVigente,
                Cantidad = 1,
                Subtotal = producto.PrecioVigente * 1,
            };
       
            return View(carritoItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Agregar(int cantidad, Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = 
                _context.Productos
                .Include(c => c.Categoria)
                .FirstOrDefault(m => m.Id == id);

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

            var miCarritoItems = carrito.CarritosItems.FirstOrDefault(m => m.ProductoId == id);
           
            if (miCarritoItems != null)
            {
               miCarritoItems.Cantidad += cantidad;
               miCarritoItems.Subtotal += (producto.PrecioVigente * cantidad);
               _context.SaveChanges();
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
                _context.SaveChanges();
            }
            //carrito.Subtotal = carrito.CarritosItems.Sum(s => s.Subtotal);
            //_context.SaveChanges();
            return RedirectToAction(nameof(MisItems));
        }


        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miCarritoItem = _context.CarritoItems.Find(id);

            if (miCarritoItem == null)
            {
                return NotFound();
            }

            var producto = 
                _context.Productos
                .Include(m => m.Categoria)
                .FirstOrDefault(m => m.Id == miCarritoItem.ProductoId);

            var idClienteLogueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito = 
                _context.Carritos
                .Include(m => m.CarritosItems)
                .Include(m => m.Cliente)
                .FirstOrDefault(m => m.Id == idClienteLogueado && m.Activo == true);

            CarritoItem carritoItem = new CarritoItem
            {
                Id = miCarritoItem.Id,
                CarritoId = miCarritoItem.CarritoId,
                Carrito = carrito,
                ProductoId = miCarritoItem.ProductoId,
                Producto = producto,
                ValorUnitario = producto.PrecioVigente,
                Cantidad = miCarritoItem.Cantidad,
                Subtotal = miCarritoItem.Subtotal,
            };

            return View(carritoItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, int cantidad)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miCarritoItems = 
                _context.CarritoItems
                .Include(m => m.Carrito)
                .Include(m => m.Producto)
                .FirstOrDefault(c => c.Id == id);        

            if (miCarritoItems != null)
            {
                miCarritoItems.Cantidad = cantidad;
                miCarritoItems.Subtotal = (miCarritoItems.Producto.PrecioVigente * cantidad);
                _context.SaveChanges();
                //miCarritoItems.Carrito.Subtotal = miCarritoItems.Carrito.CarritosItems.Sum(s => s.Subtotal);
                //_context.SaveChanges();
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
            .Include(m => m.Carrito)
            .Include(m => m.Producto)
            .FirstOrDefault(c => c.Id == id);
            _context.CarritoItems.Remove(carritoItem);
            //carritoItem.Carrito.Subtotal = carritoItem.Carrito.CarritosItems.Sum(s => s.Subtotal);
            _context.SaveChanges();
            return RedirectToAction(nameof(MisItems));
        }


        private bool CarritoItemExists(Guid id)
        {
            return _context.CarritoItems.Any(e => e.Id == id);
        }
    }
}
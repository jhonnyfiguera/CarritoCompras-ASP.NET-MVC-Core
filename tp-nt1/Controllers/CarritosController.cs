using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using tp_nt1.DataBase;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    public class CarritosController : Controller
    {
        private readonly CarritoDbContext _context;

        public CarritosController(CarritoDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpGet]
        public IActionResult VaciarCarrito()
        {
            var idClienteLogueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito =
                _context.Carritos
                .Include(c => c.CarritosItems).ThenInclude(m => m.Producto)
                .Include(c => c.Cliente)
                .FirstOrDefault(m => m.ClienteId == idClienteLogueado && m.Activo == true);

            if (carrito == null)
            {
                return NotFound();
            }

            if (carrito.CarritosItems.Count == 0)
            {
                TempData["Vacio"] = true;
                return RedirectToAction(nameof(CarritoItemsController.MisItems), "CarritoItems");
            }

            return View(carrito.CarritosItems);
        }


        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpPost, ActionName("VaciarCarrito")]
        [ValidateAntiForgeryToken]
        public IActionResult VaciarCarritoConfirmar()
        {
            var idClienteLoqueado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito =
                _context.Carritos
                .Include(m => m.CarritosItems)
                .Include(m => m.Cliente)
                .FirstOrDefault(m => m.ClienteId == idClienteLoqueado && m.Activo == true);

            if (carrito.CarritosItems != null)
            {
                carrito.CarritosItems.Clear();
                carrito.Subtotal = 0;
                _context.SaveChanges();
            }
            return RedirectToAction("MisItems", "CarritoItems");
        }

    }
}
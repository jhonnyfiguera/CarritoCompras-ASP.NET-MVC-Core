using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tp_nt1.DataBase;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    public class ClientesController : Controller
    {
        private readonly CarritoDbContext _context;

        public ClientesController(CarritoDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        [Authorize(Roles = nameof(Rol.Cliente))]
        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [AllowAnonymous]
        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Dni,Id,Nombre,Apellido,Telefono,Direccion,Email,Username,Password,FechaAlta")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                //var username = _context.Clientes.FirstOrDefault(cliente => cliente.Username == cliente.Username);//¿Cómo es esto?
                //if (username == null)
                //{
                    cliente.Id = Guid.NewGuid();
                    cliente.FechaAlta = DateTime.Now;
                    _context.Add(cliente);
                    Carrito carrito = new Carrito
                    {
                        Id = Guid.NewGuid(),
                        Activo = true,
                        ClienteId = cliente.Id,
                        Subtotal = 0
                    };
                    _context.Add(carrito);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(AccesosController.Ingresar), "Accesos");
                //}
                ViewBag.Error = "Debes ingresar un usuario diferente.";
            }
            return View(cliente);
        }

        [Authorize(Roles = nameof(Rol.Cliente))]
        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [Authorize(Roles = nameof(Rol.Cliente))]
        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Dni,Id,Nombre,Apellido,Telefono,Direccion,Email,Username,Password,FechaAlta")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
            return View(cliente);
        }

        //[Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        //[Authorize(Roles = nameof(Rol.Administrador))]
        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(Guid id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}

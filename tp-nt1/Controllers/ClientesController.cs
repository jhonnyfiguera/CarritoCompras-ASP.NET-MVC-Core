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
        [Authorize(Roles = nameof(Rol.Administrador))]
        [Authorize(Roles = nameof(Rol.Empleado))]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }


        // GET: Clientes/Details/5
        [Authorize(Roles = nameof(Rol.Cliente))]
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


        // GET: Clientes/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Dni,Id,Nombre,Apellido,Telefono,Direccion,Email,Username,Password,FechaAlta")] Cliente cliente)
        {
            var username = _context.Clientes.FirstOrDefault(cliente => cliente.Username == cliente.Username);//¿Cómo es esto?
            if (username == null)
            {

            }
            ViewBag.Error = "Debes ingresar un usuario diferente.";

            if (ModelState.IsValid)
            {
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
            }
            
            return View(cliente);
        }


        // GET: Clientes/Edit/5
        [Authorize(Roles = nameof(Rol.Cliente))]
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


        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = nameof(Rol.Cliente))]
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


        // GET: Clientes/Delete/5
        //[Authorize(Roles = nameof(Rol.Administrador))]
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


        // POST: Clientes/Delete/5
        //[Authorize(Roles = nameof(Rol.Administrador))]
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

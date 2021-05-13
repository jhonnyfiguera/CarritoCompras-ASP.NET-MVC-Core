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
    
    public class AdministradoresController : Controller
    {
        private readonly CarritoDbContext _context;

        public AdministradoresController(CarritoDbContext context)
        {
            _context = context;
        }

        // GET: Administradores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Administradores.ToListAsync());
        }

        // GET: Administradores/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }

        // GET: Administradores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administradores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Telefono,Direccion,Email,Username,FechaAlta")] Administrador administrador)
        {
            if (ModelState.IsValid)
            {
                administrador.Id = Guid.NewGuid();
                administrador.Username = "Administrador";
                administrador.Password = new byte[8]{1,2,3,4,5,6,7,8};
                administrador.FechaAlta = DateTime.Now;
                administrador.Nombre = "Sin Nombre";
                administrador.Apellido = "Sin Apellido";
                administrador.Telefono = "Sin Teléfono";
                administrador.Direccion = "Sin Dirección, Sin Dirección, 0000";
                administrador.Email = "SinAsignar@gmail.com";
                _context.Add(administrador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(administrador);
        }

        // GET: Administradores/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradores.FindAsync(id);
            if (administrador == null)
            {
                return NotFound();
            }
            return View(administrador);
        }

        // POST: Administradores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nombre,Apellido,Telefono,Direccion,Email,Username,FechaAlta")] Administrador administrador)
        {
            if (id != administrador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(administrador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministradorExists(administrador.Id))
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
            return View(administrador);
        }

        // GET: Administradores/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }

        // POST: Administradores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var administrador = await _context.Administradores.FindAsync(id);
            _context.Administradores.Remove(administrador);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministradorExists(Guid id)
        {
            return _context.Administradores.Any(e => e.Id == id);
        }
    }
}

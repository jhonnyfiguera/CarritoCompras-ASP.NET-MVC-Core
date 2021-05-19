using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Extensions;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    [Authorize(Roles = nameof(Rol.Administrador))]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string password)
        {
            try
            {
                password.ValidarPassword();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(Administrador.Password), ex.Message);
            }

            Administrador administrador = new Administrador
            {
                Nombre = "Administrador del Sistema",
                Apellido = "Administrador del Sistema",
                Telefono = "1100000000",
                Direccion = "Administrador, Administrador, 0000",
                Email = "administrador@gmail.com",
                Username = "administrador",
            };

            if (ModelState.IsValid)
            {
                administrador.Id = Guid.NewGuid();
                administrador.FechaAlta = DateTime.Now;
                administrador.Password = password.Encriptar();

                _context.Add(administrador);
                _context.SaveChanges();
                return RedirectToAction(nameof(AccesosController.Ingresar), "Accesos");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nombre,Apellido,Telefono,Direccion,Email,Username,FechaAlta")] Administrador administrador, string password)
        {
            if (!string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    password.ValidarPassword();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(Cliente.Password), ex.Message);
                }
            }

            if (id != administrador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var admDatabase = _context.Administradores.Find(id);

                    admDatabase.Username = administrador.Username;
                   
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        admDatabase.Password = password.Encriptar();
                    }

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
                return RedirectToAction(nameof(Details), new { id });
            }
            return View(administrador);
        }



        //// GET: Administradores/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var administrador = await _context.Administradores
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (administrador == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(administrador);
        //}



        //// POST: Administradores/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var administrador = await _context.Administradores.FindAsync(id);
        //    _context.Administradores.Remove(administrador);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}



        private bool AdministradorExists(Guid id)
        {
            return _context.Administradores.Any(e => e.Id == id);
        }
    }
}
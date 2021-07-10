using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using tp_nt1.DataBase;
using tp_nt1.Extensions;
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


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Clientes.ToList());
        }


        [Authorize]
        [HttpGet]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = _context.Clientes
                .FirstOrDefault(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente , string password)
        {
            try
            {
                password.ValidarPassword();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(Cliente.Password), ex.Message);
            }

            if (_context.Clientes.Any(c => c.Username == cliente.Username))
            {
                ModelState.AddModelError(nameof(cliente.Username), "El Nombre de Usuario ya existe; debes ingresar uno diferente o Iniciar sesión.");
            }

            if (_context.Clientes.Any(c => c.Email == cliente.Email))
            {
                ModelState.AddModelError(nameof(cliente.Email), "El Email ya existe; debes ingresar uno diferente o Iniciar sesión.");
            }

            if (ModelState.IsValid)
            {
                cliente.Id = Guid.NewGuid();
                cliente.FechaAlta = DateTime.Now;
                cliente.Password = password.Encriptar();
                _context.Add(cliente);

                Carrito carrito = new Carrito
                {
                    Id = Guid.NewGuid(),
                    Activo = true,
                    ClienteId = cliente.Id,
                    Subtotal = 0,
                    MensajeActualizacion = "SinMensaje"
                };
                _context.Add(carrito);

                _context.SaveChanges();

                return RedirectToAction(nameof(AccesosController.Ingresar), "Accesos");
            }

            return View(cliente);
        }


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Cliente cliente, string password)
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
    
            if (_context.Clientes.Any(c => c.Username == cliente.Username && c.Id != id))
            {
                ModelState.AddModelError(nameof(cliente.Username), "El Nombre de Usuario ya existe; debes ingresar uno diferente.");
            }

            if (_context.Clientes.Any(c => c.Email == cliente.Email && c.Id != id))
            {
                ModelState.AddModelError(nameof(cliente.Email), "El Email ya existe; debes ingresar uno diferente.");
            }

            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var clienteDatabase = _context.Clientes.Find(id);

                    clienteDatabase.Dni = cliente.Dni;
                    clienteDatabase.Nombre = cliente.Nombre;
                    clienteDatabase.Apellido = cliente.Apellido;
                    clienteDatabase.Telefono = cliente.Telefono;
                    clienteDatabase.Direccion = cliente.Direccion;
                    clienteDatabase.Email = cliente.Email;
                    clienteDatabase.Username = cliente.Username;

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        clienteDatabase.Password = password.Encriptar();
                    }

                    _context.SaveChanges();

                    TempData["EditIn"] = true;
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


        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpGet]
        public IActionResult Editarme()
        {
            var username = User.Identity.Name;
            var cliente = _context.Clientes.FirstOrDefault(c => c.Username == username);

            return View(cliente);
        }


        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpPost]
        public IActionResult Editarme(Cliente cliente, string password)
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

            var auxCliente = _context.Clientes.FirstOrDefaultAsync(e => e.Email == cliente.Email).Result;

            if (_context.Clientes.Any(e => e.Email == cliente.Email) && auxCliente.Username != cliente.Username)
            {
                ModelState.AddModelError(nameof(cliente.Email), "El Email ya existe; debes ingresar uno diferente.");
            }

            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var clienteDatabase = _context.Clientes.FirstOrDefault(c => c.Username == username);

                clienteDatabase.Telefono = cliente.Telefono;
                clienteDatabase.Direccion = cliente.Direccion;
                clienteDatabase.Email = cliente.Email;

                if (!string.IsNullOrWhiteSpace(password))
                {
                    clienteDatabase.Password = password.Encriptar();
                }

                _context.SaveChanges();

                TempData["EditIn"] = true;

                return RedirectToAction(nameof(Details), new { cliente.Id });
            }

            return View(cliente);
        }


        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpGet]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = _context.Clientes
                .FirstOrDefault(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }



        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var cliente = _context.Clientes.Find(id);
            _context.Clientes.Remove(cliente);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        private bool ClienteExists(Guid id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
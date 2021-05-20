using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Extensions;
using tp_nt1.Models;

//Enunciado Cliente: 
//Los Clientes pueden auto registrarse.
//La autoregistración desde el sitio es exclusiva para los clientes, por lo cual, se le asignará dicho rol automáticamente.
//Un usuario puede navegar los productos y sus descripciones sin iniciar sesión, es decir, de forma anónima.
//Para agregar productos al carrito debe iniciar sesión primero.
//El cliente, puede agregar diferentes productos en el carrito y por cada producto modificar la cantidad que quiere.
//Esta acción no implica validación en stock.
//El ciente verá el subtotal por cada producto/cantidad.
//También verá el subtotal del carrito.
//El cliente puede finalizar la compra.
//Cuando elija finalizar la compra se le solicitará un lugar para retirar (una sucursal).
//El cliente puede vaciar el carrito.
//El cliente puede actualizar sus datos de contacto tales como el teléfono, dirección, etc., 
//pero no puede modificar sus datos básicos tales como DNI, Nombre, Apellido, etc.

namespace tp_nt1.Controllers
{
    public class ClientesController : Controller
    {


        // Propiedad 
        private readonly CarritoDbContext _context;



        // Constructor: ClientesController
        public ClientesController(CarritoDbContext context)
        {
            _context = context;
        }



        // GET: Clientes/Index
        [Authorize(Roles = "Administrador, Empleado")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }



        // GET: Clientes/Details/5
        [Authorize]
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

            if (_context.Clientes.Any(cte => cte.Username == cliente.Username))
            {
                ModelState.AddModelError(nameof(cliente.Username), "El Nombre de Usuario ya existe; debes ingresar uno diferente o Iniciar sesión.");
            }

            if (_context.Clientes.Any(cte => cte.Email == cliente.Email))
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
                    Subtotal = 0
                };
                _context.Add(carrito);
                _context.SaveChanges();
                return RedirectToAction(nameof(AccesosController.Ingresar), "Accesos");
            }
            return View(cliente);
        }



        // GET: Clientes/Edit/5
        [Authorize(Roles = nameof(Rol.Administrador))]
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
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Dni,Id,Nombre,Apellido,Telefono,Direccion,Email,Username,Password,FechaAlta")] Cliente cliente, string password)
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

            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (_context.Clientes.Any(cte => cte.Email == cliente.Email))
            {
                ModelState.AddModelError(nameof(cliente.Email), "El Email ya existe; debes ingresar uno diferente.");
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

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        clienteDatabase.Password = password.Encriptar();
                    }

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



        #region Acciones del Cliente
        // GET: Clientes/EditMe
        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpGet]
        public IActionResult Editarme()
        {
            var username = User.Identity.Name;
            var cliente = _context.Clientes.FirstOrDefault(cliente => cliente.Username == username);

            return View(cliente);
        }



        // Post: Clientes/EditMe/(Object,123)
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


            if (_context.Clientes.Any(cte => cte.Email == cliente.Email))
            {
                ModelState.AddModelError(nameof(cliente.Email), "El Email ya existe; debes ingresar uno diferente.");
            }


            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var clienteDatabase = _context.Clientes.FirstOrDefault(cliente => cliente.Username == username);

                clienteDatabase.Telefono = cliente.Telefono;
                clienteDatabase.Direccion = cliente.Direccion;
                clienteDatabase.Email = cliente.Email;

                if (!string.IsNullOrWhiteSpace(password))
                {
                    clienteDatabase.Password = password.Encriptar();
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Details), new { cliente.Id });
            }

            return View(cliente);
        }
        #endregion



        // GET: Clientes/Delete/5
        [Authorize(Roles = nameof(Rol.Administrador))]
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
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // Metodo Privado
        private bool ClienteExists(Guid id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
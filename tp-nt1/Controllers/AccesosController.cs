using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using tp_nt1.DataBase;
using tp_nt1.Extensions;
using tp_nt1.Models;


namespace tp_nt1.Controllers
{
    [AllowAnonymous]
    public class AccesosController : Controller
    {

        private readonly CarritoDbContext _context;

        private const string _Return_Url = "ReturnUrl";


        public AccesosController(CarritoDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Ingresar(string returnUrl)
        {
            TempData[_Return_Url] = returnUrl;
            return View();
        }


        [HttpPost]
        public IActionResult Ingresar(string username, string password, Rol rol)
        {
            string returnUrl = TempData[_Return_Url] as string;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                Usuario usuario = null;

                if (rol == Rol.Cliente)
                {
                    usuario = _context.Clientes.FirstOrDefault(c => c.Username == username);
                }

                else if (rol == Rol.Empleado)
                {
                    usuario = _context.Empleados.FirstOrDefault(e => e.Username == username);
                }

                else if (rol == Rol.Administrador)
                {
                    usuario = _context.Administradores.FirstOrDefault(a => a.Username == username);
                }

                if (usuario != null)
                {
                    var passwordEncriptada = password.Encriptar();

                    
                    if ((usuario.Password.SequenceEqual(passwordEncriptada)))
                    {
                        ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                        identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Username));
                        identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Rol.ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.GivenName, usuario.Nombre));
                        identity.AddClaim(new Claim(ClaimTypes.Surname, usuario.Apellido));

                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();

                        TempData["LoggedIn"] = true;

                        if (!string.IsNullOrWhiteSpace(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
            }
            ViewBag.Error = "Usuario, Contraseña o Rol incorrecto";
            ViewBag.UserName = username;
            ViewBag.Rol = rol; 
            TempData[_Return_Url] = returnUrl;

            return View();
        }


        [Authorize]
        [HttpPost]
        public IActionResult Salir()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        [Authorize]
        [HttpGet]
        public IActionResult NoAutorizado()
        {
            return View();
        }
    }
}
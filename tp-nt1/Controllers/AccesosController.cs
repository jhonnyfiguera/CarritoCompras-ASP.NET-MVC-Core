using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tp_nt1.DataBase;
using tp_nt1.Extensions;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
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
                    usuario = _context.Clientes.FirstOrDefault(cliente => cliente.Username == username);
                } 
                else if (rol == Rol.Empleado)
                {
                        usuario = _context.Empleados.FirstOrDefault(empleado => empleado.Username == username);
                } 
                else if (rol == Rol.Administrador)
                {
                        usuario = _context.Administradores.FirstOrDefault(administrador => administrador.Username == username);
                }
                
                if (usuario != null)
                {
                    var passwordEncriptada = password.Encriptar();

                    if (usuario.Password.SequenceEqual(passwordEncriptada))
                    {
                        ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                        identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Username));

                        identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Rol.ToString()));

                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));

                        identity.AddClaim(new Claim(ClaimTypes.GivenName, usuario.Nombre));

                        identity.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));

                        identity.AddClaim(new Claim(ClaimTypes.MobilePhone, usuario.Telefono));

                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();

                        TempData["LoggedIn"] = true;

                        if (!string.IsNullOrWhiteSpace(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction(nameof(HomeController.Index), "Home"); //Definir a que página lo redireccionamos
                    }
                }
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            ViewBag.UserName = username;
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

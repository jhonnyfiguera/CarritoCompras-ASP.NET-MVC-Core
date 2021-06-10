using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using tp_nt1.DataBase;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    public class HomeController : Controller
    {

        private readonly CarritoDbContext _context;
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger , CarritoDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Categorias = new SelectList(_context.Categorias, nameof(Categoria.Id), nameof(Categoria.Nombre));
            ViewBag.Precio = new SelectList(new int[6] { 200, 400, 700, 900, 1300, 1600 });
            ViewBag.Estado = new SelectList(new string[2] {"Activo","Inactivo"});

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
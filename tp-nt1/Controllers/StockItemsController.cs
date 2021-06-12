﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using tp_nt1.DataBase;
using tp_nt1.Extensions;
using tp_nt1.Models;

namespace tp_nt1.Controllers
{
    [Authorize(Roles = nameof(Rol.Empleado))]

    public class StockItemsController : Controller
    {

        private readonly CarritoDbContext _context;

        public StockItemsController(CarritoDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var carritoDbContext = _context.StockItems.Include(s => s.Producto).Include(s => s.Sucursal);
            return View(carritoDbContext.ToList());
        }


        [HttpGet]
        public IActionResult AgregarStock()
        {
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre");
            ViewData["SucursalId"] = new SelectList(_context.Sucursal, "Id", "Direccion");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult AgregarStock(StockItem stockItem)
        {
            bool cantidadValida = true;
            try
            {
                stockItem.Cantidad.ValidarInput();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(StockItem.Cantidad), ex.Message);
                cantidadValida = false;
            }

            if (cantidadValida)
            {
                var itemAuxiliar =
                _context.StockItems
                .FirstOrDefault(f => f.ProductoId == stockItem.ProductoId
                && f.SucursalId == stockItem.SucursalId);

                if (itemAuxiliar != null)
                {
                    itemAuxiliar.Cantidad += stockItem.Cantidad;
                }
                else
                {
                    stockItem.Id = Guid.NewGuid();
                    _context.Add(stockItem);
                }
                _context.SaveChanges();
                TempData["EditIn"] = true;
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre");
            ViewData["SucursalId"] = new SelectList(_context.Sucursal, "Id", "Direccion");
            return View();
        }


        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockItem = _context.StockItems.Find(id);
            if (stockItem == null)
            {
                return NotFound();
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", stockItem.ProductoId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursal, "Id", "Direccion", stockItem.SucursalId);
            return View(stockItem);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, StockItem stockItem)
        {
            if (id != stockItem.Id)
            {
                return NotFound();
            }

            bool cantidadValida = true;
            try
            {
                stockItem.Cantidad.ValidarInput();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(StockItem.Cantidad), ex.Message);
                cantidadValida = false;
            }

            if (cantidadValida)
            {
                _context.Update(stockItem);
                _context.SaveChanges();
                TempData["EditIn"] = true;
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", stockItem.ProductoId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursal, "Id", "Direccion", stockItem.SucursalId);
            return View(stockItem);
        }


        [HttpGet]
        public IActionResult Eliminar(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockItem =  _context.StockItems
                .Include(s => s.Producto)
                .Include(s => s.Sucursal)
                .FirstOrDefault(m => m.Id == id);
            if (stockItem == null)
            {
                return NotFound();
            }

            return View(stockItem);
        }


        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmar(Guid id)
        {
            var stockItem =  _context.StockItems.Find(id);
            _context.StockItems.Remove(stockItem);
             _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        private bool StockItemExists(Guid id)
        {
            return _context.StockItems.Any(e => e.Id == id);
        }
    }
}
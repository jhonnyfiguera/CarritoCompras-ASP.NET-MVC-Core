using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tp_nt1.Models;

namespace tp_nt1.DataBase
{
    public class CarritoDbContext : DbContext
    {
        public CarritoDbContext(DbContextOptions<CarritoDbContext> opciones) : base(opciones)
        {
        }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<Sucursal> Sucursal { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
    }
}

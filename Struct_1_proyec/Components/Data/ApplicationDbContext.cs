using Microsoft.EntityFrameworkCore;
using Struct_1_proyec.Models;

namespace Struct_1_proyec.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
            
    }
    public DbSet<Envio> Envios { get; set; }
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<TrackingLog> TrackingLogs => Set<TrackingLog>();
    public DbSet<Cotizacion> Cotizaciones => Set<Cotizacion>();
    public DbSet<Contacto> Contactos => Set<Contacto>();
}


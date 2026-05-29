using Microsoft.EntityFrameworkCore;
using Struct_1_proyec.Data;
using Struct_1_proyec.Models;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        // 1. Asegura que la estructura de la base de datos exista
        await context.Database.EnsureCreatedAsync();

        // 2. PROTECCIÓN: Si ya hay datos, no hacemos nada.
        if (context.Pedidos.Any())
        {
            return;
        }

        // 3. Solo si la tabla está vacía, insertamos los datos de prueba
        var pedidoEjemplo = new Pedido
        {
            NumeroGuia = "RERF-502-2026",
            Cliente = "Distribuidora PYME S.A.",
            Origen = "Guatemala (Capital)",
            Destino = "Quetzaltenango (Xela)",
            Peso = 8.5m,
            ValorProducto = 750.00m,
            Estado = "En camino",
            FechaCreacion = DateTime.Now.AddDays(-2)
        };

        context.Pedidos.Add(pedidoEjemplo);
        await context.SaveChangesAsync();

        // Línea de tiempo de estados (Logs)
        context.TrackingLogs.AddRange(
            new TrackingLog
            {
                PedidoId = pedidoEjemplo.Id,
                Estado = "Pendiente",
                Comentario = "Guía generada en el sistema. Paquete recolectado en origen.",
                Fecha = DateTime.Now.AddDays(-2)
            },
            new TrackingLog
            {
                PedidoId = pedidoEjemplo.Id,
                Estado = "En camino",
                Comentario = "El paquete salió del Centro de Distribución Central rumbo a sucursal Xela.",
                Fecha = DateTime.Now.AddDays(-1)
            }
        );
        await context.SaveChangesAsync();

    }
}
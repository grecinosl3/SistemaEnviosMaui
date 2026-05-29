using Microsoft.EntityFrameworkCore;
using Struct_1_proyec.Data;
using Struct_1_proyec.Models;

namespace Struct_1_proyec.Services;

public class TrackingService
{
    private readonly ApplicationDbContext _context;

    public TrackingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido?> BuscarGuia(string guia)
    {
        return await _context.Pedidos
            .Include(p => p.Logs)
            .FirstOrDefaultAsync(x => x.NumeroGuia == guia);
    }
}
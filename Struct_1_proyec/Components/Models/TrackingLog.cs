namespace Struct_1_proyec.Models;
// Modelo de TrackingLog para registrar el historial de estados de un pedido
public class TrackingLog
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public Pedido Pedido { get; set; } = default!;

    public string Estado { get; set; } = string.Empty;

    public string Comentario { get; set; } = string.Empty;
    // Fecha del registro del estado, se asigna automáticamente al crear el log
    public DateTime Fecha { get; set; } = DateTime.Now;
}
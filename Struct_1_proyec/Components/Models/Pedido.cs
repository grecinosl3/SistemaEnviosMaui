using System.ComponentModel.DataAnnotations;

namespace Struct_1_proyec.Models;
// Modelo de Pedido para la aplicación de logística
public class Pedido
{
    public int Id { get; set; }

    [Required]
    public string NumeroGuia { get; set; } = string.Empty;

    public string Cliente { get; set; } = string.Empty;

    public string Origen { get; set; } = string.Empty;

    public string Destino { get; set; } = string.Empty;

    public decimal Peso { get; set; }

    public decimal ValorProducto { get; set; }

    public string Estado { get; set; } = "Pendiente";
    // Fecha de creación del pedido,
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Relación con TrackingLog 
    public List<TrackingLog> Logs { get; set; } = new();
}
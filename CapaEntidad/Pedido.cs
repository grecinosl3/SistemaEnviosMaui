using System;
using System.Collections.Generic;
using System.Text;

namespace CapaEntidad
{
    public class Pedido
    {
        public int IdPedido { get; set; }

        // Objetos completos (Para mostrar Nombre del cliente o del producto en la pantalla)
        public Cliente oCliente { get; set; }
        public Producto oProducto { get; set; } // OJO: Si un pedido tiene muchos productos, usa solo la lista de abajo

        // Datos de control
        public int IdModerador { get; set; }
        public int IdRepartidor { get; set; }

        // Información del envío
        public DateTime FechaPedido { get; set; } = DateTime.Now; // Valor por defecto
        public DateTime? FechaEntrega { get; set; }
        public string Direccion { get; set; }
        public string Estado { get; set; } // Ej: "Pendiente", "En Camino", "Entregado"

        // Dinero
        public string MetodoPago { get; set; }
        public decimal Total { get; set; }

        // El "corazón" del pedido
        public List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
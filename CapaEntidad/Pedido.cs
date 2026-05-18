using System;
using System.Collections.Generic;

namespace CapaEntidad
{
    public class Pedido
    {
        public int IdPedido { get; set; } // Este será el Número de Guía

        // La Empresa/Comercio que envía el paquete
        public int IdCliente { get; set; }
        public Cliente oCliente { get; set; }

        // Personal operativo (Se quedan como null al inicio hasta que el Admin los asigne)
        public int? IdModerador { get; set; }  // El administrador que despacha la ruta
        public int? IdRepartidor { get; set; } // El motorista que lleva el paquete

        // Fechas del ciclo logístico
        public DateTime FechaPedido { get; set; } = DateTime.Now; // Cuando la empresa creó la guía
        public DateTime? FechaEntrega { get; set; }               // Cuando el motorista la entrega en la calle

        // Datos del Destinatario Final (A quién se le entrega el paquete físico)
        public string NombreDestinatario { get; set; }
        public string TelefonoDestinatario { get; set; }
        public string DireccionEntrega { get; set; } // Dirección exacta de la casa/oficina del comprador

        // Control de Estados (Registrado, En Bodega, En Ruta, Entregado, Devuelto, Liquidado)
        public string Estado { get; set; } = "Registrado";

        // Finanzas del Paquete
        public string MetodoPago { get; set; } // Ej: "Pago Contra Entrega (COD)" o "Flete Prepagado"
        public decimal MontoCOD { get; set; }   // ¡VITAL! El efectivo que el motorista debe traer de la calle
        public decimal CostoFlete { get; set; } // Lo que tú cobras por el envío (Viene de tu inventario)
        public decimal Total { get; set; }      // Monto total de la operación

        public string Notas { get; set; } // Ej: "Entregar solo de tarde / Caja frágil"

        // El flete o fletes aplicados a este envío (Tu carrito de fletes)
        public List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
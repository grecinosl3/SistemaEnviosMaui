using System;
using System.Collections.Generic;

namespace CapaEntidad
{
    public class Pedido
    {
        public int IdPedido { get; set; } 

        // La Empresa que envía el paquete
        public int IdCliente { get; set; }
        public int? IdPiloto { get; set; }
        public Cliente oCliente { get; set; }

        // Personal operativo 
        public int? IdModerador { get; set; }  
        public int? IdRepartidor { get; set; } 

        // Fechas del ciclo logístico
        public DateTime FechaPedido { get; set; } = DateTime.Now; 
        public DateTime? FechaEntrega { get; set; }              

        // Datos del Destinatario Final 
        public string NombreDestinatario { get; set; }
        public string TelefonoDestinatario { get; set; }
        public string DireccionEntrega { get; set; } 

        // Control de Estados (Registrado, En Bodega, En Ruta, Entregado, Devuelto, Liquidado)
        public string Estado { get; set; } = "Registrado";

        // Finanzas del Paquete
        public string MetodoPago { get; set; } 
        public decimal MontoCOD { get; set; }   
        public decimal CostoFlete { get; set; } 
        public decimal Total { get; set; }  

        public string Notas { get; set; }

      
        public List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
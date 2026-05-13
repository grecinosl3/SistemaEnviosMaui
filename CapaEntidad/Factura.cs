using System;
using System.Collections.Generic;
using System.Text;

namespace CapaEntidad
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public int IdPedido { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal Total { get; set; }
        public string TipoFactura { get; set; }

    }
}

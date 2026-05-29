using System;

namespace Struct_1_proyec.Models
{
    public class Cotizacion
    {
        public int Id { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public decimal PesoLbs { get; set; }
        public decimal ValorProducto { get; set; }
        public decimal CostoBase { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
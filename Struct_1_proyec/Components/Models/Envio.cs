using System;
using System.Collections.Generic;

namespace Struct_1_proyec.Models
{
    public class Envio
    {
        // --- Propiedades que ya tenías en tu vista Web ---
        public string NombreRemitente { get; set; } = string.Empty;
        public string TelefonoRemitente { get; set; } = string.Empty;
        public string DireccionRecoleccion { get; set; } = string.Empty;
        public string NombreDestinatario { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
        public string DireccionExacta { get; set; } = string.Empty;
        public string ReferenciaVisual { get; set; } = string.Empty;
        public double PesoLbs { get; set; }
        public string MetodoPago { get; set; } = "Efectivo (COD)";
        public string NombreFactura { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string NumeroGuia { get; set; } = string.Empty;

        // --- NUEVAS PROPIEDADES: Para acoplarse al formato de MAUI sin romper nada ---
        public int IdCliente { get; set; } = 1; // Por defecto 1 (puedes cambiarlo al buscar por NIT)
        public string TipoFactura { get; set; } = "Factura Electrónica";
        public string NotasEnvio { get; set; } = string.Empty;
        public double TotalPagar { get; set; }

        // Esta lista simula la tabla de productos que tiene MAUI abajo
        public List<ItemGridWeb> ProductosGrid { get; set; } = new List<ItemGridWeb>();
    }

    // Clase auxiliar para que la Web pueda estructurar productos individuales como MAUI
    public class ItemGridWeb
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public double PrecioTarifa { get; set; }
    }
}
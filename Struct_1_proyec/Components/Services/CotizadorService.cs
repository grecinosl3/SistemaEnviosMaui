using System;

namespace Struct_1_proyec.Services
{
    public class CotizadorService
    {
        public decimal CalcularCostoEnvio(string destino, decimal peso)
        {
            // Tarifa base por defecto en Guatemala
            decimal tarifaBase = 25.00m;

            // Lógica tarifaria según el departamento de destino
            switch (destino)
            {
                case "Guatemala":
                    tarifaBase = 22.00m;
                    break;
                case "Petén":
                case "Huehuetenango":
                case "Izabal":
                case "San Marcos":
                    tarifaBase = 40.00m;
                    break;
                case "Quetzaltenango":
                case "Escuintla":
                case "Chiquimula":
                case "Zacapa":
                    tarifaBase = 32.00m;
                    break;
                default:
                    tarifaBase = 28.00m;
                    break;
            }

            // Recargo por exceso de peso (Más de 5 Libras)
            decimal recargoPeso = 0;
            if (peso > 5)
            {
                recargoPeso = (peso - 5) * 2.50m;
            }

            return tarifaBase + recargoPeso;
        }
    }
}
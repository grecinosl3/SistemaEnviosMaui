using System;
using System.Collections.Generic;
using System.Text;

namespace CapaEntidad
{
    public class DashboardMetrics
    {
        public int PaquetesPendientes { get; set; }
        public int PaquetesEnRuta { get; set; }
        public int PaquetesLiquidados { get; set; }
        public decimal TotalEfectivoMes { get; set; }
    }
}
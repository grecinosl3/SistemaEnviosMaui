using System;
using System.Collections.Generic;
using System.Text;

namespace CapaEntidad
{
    public class Cliente : Usuario
    {

        public int IdCliente { get; set; }
        public string CUI { get; set; }
        public string Direccion { get; set; }
        public string MetodoPagoDefault { get; set; }


    }
}

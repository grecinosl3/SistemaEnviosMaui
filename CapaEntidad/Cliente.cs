using System;
using System.Collections.Generic;
using System.Text;

namespace CapaEntidad
{
    public class Cliente
    {
        // Identificador único de la empresa en tu sistema
        public int IdCliente { get; set; }

        // Datos Fiscales de la Empresa
        public string NombreComercial { get; set; }  
        public string RazonSocial { get; set; }    
        public string NIT { get; set; }             

        // Datos del Contacto Operativo (con quien habla tu oficina)
        public string NombreContacto { get; set; }   
        public string TelefonoContacto { get; set; }
        public string CorreoContacto { get; set; }

        // Logística y Finanzas
        public string DireccionBodega { get; set; }  
        public string CuentaBancaria { get; set; }   
        public string Banco { get; set; }

        // Control Interno
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }
}
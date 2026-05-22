using System;
using System.Collections.Generic;
using System.Text;

namespace CapaEntidad
{
    public class ChatConversacion
    {

        public int IdConversacion { get; set; }
        public int IdUsuarioUno { get; set; }
        public int IdUsuarioDos { get; set; }
        public DateTime FechaCreacion { get; set; }

        // 💡 Propiedades auxiliares (No están en la tabla, pero nos servirán en la interfaz)
        public string? NombreOtroUsuario { get; set; }
        public string? UltimoMensaje { get; set; }
        public DateTime? FechaUltimoMensaje { get; set; }

    }
}

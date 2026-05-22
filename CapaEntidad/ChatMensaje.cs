using System;
using System.Collections.Generic;
using System.Text;

namespace CapaEntidad
{
    public class ChatMensaje
    {
        public long IdMensaje { get; set; }
        public int IdConversacion { get; set; }
        public int IdRemitente { get; set; }
        public string? Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }
        public bool EstadoLeido { get; set; }

        // 💡 Propiedad auxiliar para el diseño visual en MAUI
        // Nos ayudará a saber si la burbuja va a la derecha (mía) o izquierda (otro)
        public bool EsMensajeMio { get; set; }

    }
}

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

        //  Propiedad auxiliar para el diseño visual en MAUI
        public bool EsMensajeMio { get; set; }

    }
}

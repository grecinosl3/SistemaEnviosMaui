using CapaDatos;
using CapaEntidad;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class CN_Chat
    {
        private CD_Chat objCapaDato = new CD_Chat();

        public bool RegistrarMensaje(ChatMensaje obj, out string MensajeError)
        {
            MensajeError = string.Empty;

            // Validaciones básicas de seguridad
            if (string.IsNullOrWhiteSpace(obj.Mensaje))
            {
                MensajeError = "El texto del mensaje no puede estar vacío.";
                return false;
            }

            if (obj.IdConversacion <= 0 || obj.IdRemitente <= 0)
            {
                MensajeError = "El ID de conversación o el remitente no son válidos.";
                return false;
            }

            // Si pasa los filtros, se lo manda a la Capa de Datos para que lo guarde en SQL
            return objCapaDato.RegistrarMensaje(obj, out MensajeError);
        }

        public List<ChatMensaje> ListarMensajesPorConversacion(int idConversacion)
        {
            // Validación rápida de seguridad
            if (idConversacion <= 0) return new List<ChatMensaje>();

            // Le pide los datos a la capa inferior
            return objCapaDato.ListarMensajesPorConversacion(idConversacion);
        }
    }
}
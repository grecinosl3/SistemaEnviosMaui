using CapaEntidad;
using Microsoft.Maui.Controls;

namespace SistemaEnviosMaui.ViewModels
{
    public class ChatTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? PlantillaMensajeMio { get; set; }
        public DataTemplate? PlantillaMensajeOtro { get; set; }

        protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
        {
            var mensaje = (ChatMensaje)item;

            // 🚨 CAMBIO AQUÍ: Si el IdRemitente es 1, eres tú (verde). Si no, es otra persona (blanco).
            return mensaje.IdRemitente == 1 ? PlantillaMensajeMio : PlantillaMensajeOtro;
        }
    }
}
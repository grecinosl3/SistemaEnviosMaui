using CapaEntidad;
using CapaNegocio;
using Microsoft.AspNetCore.SignalR;

namespace SistemaEnviosWebAPI.Hubs
{
    public class ChatHub : Hub
    {
        public async Task UnirseASala(string idConversacion)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, idConversacion);
                Console.WriteLine($"👥 Cliente conectado unido a la sala: {idConversacion}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en UnirseASala: {ex.Message}");
            }
        }

        public async Task EnviarMensaje(ChatMensaje mensaje)
        {
            try
            {
                string error;
                var chatNegocio = new CN_Chat();

                // Guarda en SQL Server
                bool guardado = chatNegocio.RegistrarMensaje(mensaje, out error);

                if (guardado)
                {
                    Console.WriteLine($"✅ Mensaje guardado en SQL: {mensaje.Mensaje}");
                    // Retransmite a todos en la sala en tiempo real
                    await Clients.Group(mensaje.IdConversacion.ToString()).SendAsync("RecibirMensaje", mensaje);
                }
                else
                {
                    Console.WriteLine($"⚠️ Error al registrar: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error crítico en EnviarMensaje: {ex.Message}");
            }
        }

        public async Task ObtenerHistorial(string idConversacion)
        {
            try
            {
                if (int.TryParse(idConversacion, out int idConv))
                {
                    var chatNegocio = new CN_Chat();

                    // Llama al nuevo método que agregamos en tu CN_Chat
                    List<ChatMensaje> historial = chatNegocio.ListarMensajesPorConversacion(idConv);

                    // Envía el pasado únicamente al usuario que lo solicitó
                    await Clients.Caller.SendAsync("RecibirHistorial", historial);
                    Console.WriteLine($"📚 Historial enviado. Sala {idConversacion}: {historial.Count} mensajes.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerHistorial: {ex.Message}");
            }
        }
    }
}
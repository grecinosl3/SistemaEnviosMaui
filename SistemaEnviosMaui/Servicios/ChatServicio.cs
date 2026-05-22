using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using CapaEntidad; // 👈 Para que tu app reconozca el molde 'ChatMensaje'

namespace SistemaEnviosMaui.Servicios
{
    public class ChatServicio
    {
        private readonly HubConnection _conexion;

        // Evento que avisará a tus pantallas de MAUI cuando llegue un mensaje nuevo de otra persona
        public event Action<ChatMensaje>? OnMensajeRecibido;

        public ChatServicio()
        {
            // Configuramos la conexión apuntando exactamente a tu servidor local
            _conexion = new HubConnectionBuilder()
                .WithUrl("http://localhost:5228/chathub") // 👈 ¡Tu puerto mágico!
                .WithAutomaticReconnect() // Si se cae el internet, se reconecta solo
                .Build();

            // Escuchamos cuando el servidor nos grite "RecibirMensaje"
            _conexion.On<ChatMensaje>("RecibirMensaje", (mensaje) =>
            {
                // Disparamos el evento para que la pantalla se actualice en tiempo real
                OnMensajeRecibido?.Invoke(mensaje);
            });
        }

        // Método para encender el chat al abrir la app
        public async Task ConectarAsync()
        {
            if (_conexion.State == HubConnectionState.Disconnected)
            {
                await _conexion.StartAsync();
            }
        }

        // Método para unirse al cuarto de una conversación específica
        public async Task UnirseAConversacionAsync(int idConversacion)
        {
            await _conexion.InvokeAsync("UnirseASala", idConversacion);
        }

        // Método para mandar un mensaje al servidor
        public async Task EnviarMensajeAsync(ChatMensaje mensaje)
        {
            await _conexion.InvokeAsync("EnviarMensaje", mensaje);
        }
    }
}
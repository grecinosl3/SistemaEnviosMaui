/*using CapaEntidad;
using SistemaEnviosMaui.Servicios;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SistemaEnviosMaui.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private readonly ChatServicio _chatServicio;
        private string _textoMensaje = string.Empty;
        private int _idConversacionActual = 1; // ID de prueba para conectar el canal

        // Lista que actualiza la pantalla de MAUI automáticamente cuando llega un mensaje
        public ObservableCollection<ChatMensaje> Mensajes { get; set; } = new();

        // Propiedad enlazada a la caja de texto donde escribes
        public string TextoMensaje
        {
            get => _textoMensaje;
            set { _textoMensaje = value; OnPropertyChanged(); }
        }

        public ICommand EnviarComando { get; }

        public ChatViewModel()
        {
            // Instanciamos el motor del chat
            _chatServicio = new ChatServicio();

            // Evento: cuando el servidor nos mande un mensaje, lo metemos a la lista visual
            _chatServicio.OnMensajeRecibido += (nuevoMensaje) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Mensajes.Add(nuevoMensaje);
                });
            };

            // Configuramos el botón Enviar
            EnviarComando = new Command(async () => await EnviarMensajeAsync());

            // Nos conectamos de fondo al iniciar
            _ = InicializarChatAsync();

            // Mensaje simulado de otra persona para probar el diseño
            // Mensajes.Add(new ChatMensaje { Mensaje = "¡Hola Gerson! ¿Cómo vas con el sistema?", FechaEnvio = DateTime.Now.AddMinutes(-5), EsMensajeMio = false });
            // Mensajes.Add(new ChatMensaje { Mensaje = "Ya veo que las burbujas funcionan", FechaEnvio = DateTime.Now.AddMinutes(-4), EsMensajeMio = false });
        }

        private async Task InicializarChatAsync()
        {
            try
            {
                await _chatServicio.ConectarAsync();
                await _chatServicio.UnirseAConversacionAsync(_idConversacionActual);
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error de Conexión", ex.Message, "OK");
            }
        }

        private async Task EnviarMensajeAsync()
        {
            if (string.IsNullOrWhiteSpace(TextoMensaje)) return;

            var nuevoMensaje = new ChatMensaje
            {
                IdConversacion = _idConversacionActual,
                IdRemitente = 1, // Remitente de prueba
                Mensaje = TextoMensaje,
                FechaEnvio = DateTime.Now
            };

            try
            {
                await _chatServicio.EnviarMensajeAsync(nuevoMensaje);
                TextoMensaje = string.Empty; // Limpiamos la caja al enviar
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", $"No se envió: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}*/
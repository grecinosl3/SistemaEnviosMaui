using CapaEntidad;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using SistemaEnviosMaui.ViewModels;

namespace SistemaEnviosMaui.Views;

// 🚨 CORREGIDO: Ahora hereda de ContentView para incrustarse en tu contenedor principal
public partial class ChatPage : ContentView
{
    public ObservableCollection<ChatMensaje> ListaMensajes { get; set; } = new ObservableCollection<ChatMensaje>();
    private HubConnection _connection;

    private int _idUsuarioLogueado;
    private int _idSalaActual;

    public ChatPage(int idUsuario, int idSala)
    {
        InitializeComponent();

        _idUsuarioLogueado = idUsuario;
        _idSalaActual = idSala;

        // Establecemos el contexto de datos para el binding con el nuevo look oscuro
        this.BindingContext = this;

        _ = InicializarSignalR();
    }

    private async Task InicializarSignalR()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5228/chathub")
            .Build();

        // 👂 ESCUCHADOR 1: Captura mensajes nuevos en tiempo real
        _connection.On<ChatMensaje>("RecibirMensaje", (nuevoMensaje) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Compara con tu ID dinámico de la base de datos
                nuevoMensaje.EsMensajeMio = (nuevoMensaje.IdRemitente == _idUsuarioLogueado);
                ListaMensajes.Add(nuevoMensaje);
            });
        });

        // 🔥 ESCUCHADOR 2: Captura el historial completo al abrir este módulo
        _connection.On<List<ChatMensaje>>("RecibirHistorial", (listaHistorial) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListaMensajes.Clear();

                foreach (var mensaje in listaHistorial)
                {
                    mensaje.EsMensajeMio = (mensaje.IdRemitente == _idUsuarioLogueado);
                    ListaMensajes.Add(mensaje);
                }
            });
        });

        try
        {
            await _connection.StartAsync();
            Console.WriteLine("✅ Conexión con SignalR establecida con éxito.");

            // 👥 Entramos a la sala usando el ID real
            await _connection.InvokeAsync("UnirseASala", _idSalaActual.ToString());
            Console.WriteLine($"👥 Unido con éxito a la sala de chat #{_idSalaActual}");

            // ⏳ El respiro para estabilizar la sesión
            await Task.Delay(500);

            // 📚 Pedimos el historial dinámico
            await _connection.InvokeAsync("ObtenerHistorial", _idSalaActual.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al iniciar SignalR: {ex.Message}");
        }
    }

    private async void BtnEnviar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtMensaje.Text)) return;

        // Construimos el objeto con los datos vivos de tu sesión iniciada
        var miMensaje = new ChatMensaje
        {
            IdConversacion = _idSalaActual,
            IdRemitente = _idUsuarioLogueado,
            Mensaje = txtMensaje.Text,
            FechaEnvio = DateTime.Now,
            EsMensajeMio = true
        };

        try
        {
            await _connection.InvokeAsync("EnviarMensaje", miMensaje);
            txtMensaje.Text = string.Empty; // Limpiamos tu Entry personalizado
        }
        catch (Exception ex)
        {
            // Como ahora es un ContentView, usamos Application.Current para el diálogo de alerta
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
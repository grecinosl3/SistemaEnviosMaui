using CapaEntidad;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using SistemaEnviosMaui.ViewModels;

namespace SistemaEnviosMaui.Views;

public partial class ChatPage : ContentView
{
    public ObservableCollection<ChatMensaje> ListaMensajes { get; set; } = new ObservableCollection<ChatMensaje>();
    private HubConnection _connection;

    private int _idUsuarioLogueado;
    private int _idSalaActual;


    public ChatPage(int idUsuario, int idSala, string nombreContacto)
    {
        InitializeComponent();

        _idUsuarioLogueado = idUsuario;
        _idSalaActual = idSala;
        lblTituloChat.Text = nombreContacto;

        this.BindingContext = this;

        _ = InicializarSignalR();
    }

    private async Task InicializarSignalR()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5228/chathub")
            .Build();

        //  Captura mensajes nuevos en tiempo real
        _connection.On<ChatMensaje>("RecibirMensaje", (nuevoMensaje) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                nuevoMensaje.EsMensajeMio = (nuevoMensaje.IdRemitente == _idUsuarioLogueado);
                ListaMensajes.Add(nuevoMensaje);
            });
        });

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
            Console.WriteLine(" Conexión con SignalR establecida con éxito.");

            await _connection.InvokeAsync("UnirseASala", _idSalaActual.ToString());
            Console.WriteLine($" Unido con éxito a la sala de chat #{_idSalaActual}");

            await Task.Delay(500);

            await _connection.InvokeAsync("ObtenerHistorial", _idSalaActual.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error al iniciar SignalR: {ex.Message}");
        }
    }

    private async void BtnEnviar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtMensaje.Text)) return;

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
            txtMensaje.Text = string.Empty; 
        }
        catch (Exception ex)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
    private void btnVolver_Clicked(object sender, EventArgs e)
    {
        var principalPage = Application.Current?.MainPage as PrincipalPage;
        if (principalPage == null && Application.Current?.MainPage?.Navigation != null)
        {
            principalPage = Application.Current.MainPage.Navigation.NavigationStack.FirstOrDefault(p => p is PrincipalPage) as PrincipalPage;
        }

        if (principalPage != null)
        {
            var contenedor = principalPage.FindByName<ContentView>("ContenedorPrincipal");
            if (contenedor != null)
            {
                contenedor.Content = new ListaChatsPage(_idUsuarioLogueado);
            }
        }
    }
}
using CapaEntidad;
using CapaNegocio;
using System.Collections.ObjectModel;

namespace SistemaEnviosMaui.Views;

public partial class ListaChatsPage : ContentView
{
    public ObservableCollection<Usuario> ListaUsuarios { get; set; } = new ObservableCollection<Usuario>();
    private Usuario _contactoSeleccionado;
    private int _idUsuarioLogueado;

    public ListaChatsPage(int idUsuarioLogueado)
    {
        InitializeComponent();
        _idUsuarioLogueado = idUsuarioLogueado;

        CargarListaUsuariosReal();
    }

    private void CargarListaUsuariosReal()
    {
        try
        {
            List<Usuario> listaCompleta = new CN_Usuario().Listar();

            if (listaCompleta != null)
            {
                ListaUsuarios.Clear();

                foreach (var usuario in listaCompleta)
                {
                    if (usuario.IdUsuario != _idUsuarioLogueado)
                    {
                        ListaUsuarios.Add(usuario);
                    }
                }

                dgvContactos.ItemsSource = ListaUsuarios;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(" Error al cargar usuarios: " + ex.Message);
        }
    }

    private void dgvContactos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _contactoSeleccionado = e.CurrentSelection.FirstOrDefault() as Usuario;

        if (_contactoSeleccionado != null)
        {
            lblNombreContacto.Text = _contactoSeleccionado.NombreCompleto;
            lblRolContacto.Text = _contactoSeleccionado.oRol?.NombreRol ?? "Sin Rol";
            btnAbrirChat.IsEnabled = true;
        }
    }

    private async void btnAbrirChat_Click(object sender, EventArgs e)
    {
        if (_contactoSeleccionado == null) return;

        try
        {
            int idSalaUnica = int.Parse($"{Math.Min(_idUsuarioLogueado, _contactoSeleccionado.IdUsuario)}{Math.Max(_idUsuarioLogueado, _contactoSeleccionado.IdUsuario)}");

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
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        var pantallaChat = new ChatPage(_idUsuarioLogueado, idSalaUnica, _contactoSeleccionado.NombreCompleto);

                        contenedor.Content = pantallaChat;
                    });
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error de Sistema", "No se encontró el contenedor visual llamado 'ContenedorPrincipal'.", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Error de Navegación", "No se pudo enlazar la interfaz con la página principal.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error Inesperado", $"No se pudo abrir el chat: {ex.Message}", "OK");
        }
    }
}
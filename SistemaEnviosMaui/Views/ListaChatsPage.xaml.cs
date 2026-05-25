using CapaEntidad;
using CapaNegocio; // 🚀 Conexión directa a tus datos locales
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

        // Carga directa desde la base de datos local
        CargarListaUsuariosReal();
    }

    private void CargarListaUsuariosReal()
    {
        try
        {
            // 📡 Traemos la lista directo de tu clase de negocio (como en Inventario)
            List<Usuario> listaCompleta = new CN_Usuario().Listar();

            if (listaCompleta != null)
            {
                ListaUsuarios.Clear();

                foreach (var usuario in listaCompleta)
                {
                    // Evitamos que te veas a ti mismo en la lista
                    if (usuario.IdUsuario != _idUsuarioLogueado)
                    {
                        ListaUsuarios.Add(usuario);
                    }
                }

                // Vinculamos la colección real al dgvContactos
                dgvContactos.ItemsSource = ListaUsuarios;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error al cargar usuarios: " + ex.Message);
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
            // 1. Generamos la sala matemática entre ambos IDs
            int idSalaUnica = int.Parse($"{Math.Min(_idUsuarioLogueado, _contactoSeleccionado.IdUsuario)}{Math.Max(_idUsuarioLogueado, _contactoSeleccionado.IdUsuario)}");

            // 2. Buscamos la página principal recorriendo la pila de navegación de MAUI
            var principalPage = Application.Current?.MainPage as PrincipalPage;

            // Si tu MainPage está envuelto en una NavigationPage o Shell, lo buscamos de forma alterna:
            if (principalPage == null && Application.Current?.MainPage?.Navigation != null)
            {
                principalPage = Application.Current.MainPage.Navigation.NavigationStack.FirstOrDefault(p => p is PrincipalPage) as PrincipalPage;
            }

            // 3. Si encontramos la página principal, hacemos el cambio físico del contenido
            if (principalPage != null)
            {
                // Buscamos el contenedor por su tipo o su nombre
                var contenedor = principalPage.FindByName<ContentView>("ContenedorPrincipal");

                if (contenedor != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Le pasamos: tu ID, el ID de la sala, y el Nombre del contacto
                        var pantallaChat = new ChatPage(_idUsuarioLogueado, idSalaUnica, _contactoSeleccionado.NombreCompleto);

                        // Inyectamos la interfaz del chat en tu contenedor
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
using SistemaEnviosMaui.Views;

namespace SistemaEnviosMaui.Views;

public partial class PrincipalPage : ContentPage
{
	public PrincipalPage(string nombreUsuario = "Invitado")
	{
		InitializeComponent();
        lblUsuarioNombre.Text = nombreUsuario;
    }

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }

    private async void OnMenuOptionTapped(object sender, TappedEventArgs e)
    {
        // Obtenemos el nombre de la opción (Inicio, Usuarios, etc.)
        var opcion = e.Parameter.ToString();

        switch (opcion)
        {
            case "Usuarios":
                ContenedorPrincipal.Content = new UsuariosView();
                break;
            case "Pedidos":
                ContenedorPrincipal.Content = new PedidosPage();
                break;
            case "Inventario":
                ContenedorPrincipal.Content = new InventarioPage();
                break;
        }

    }

}
using CapaNegocio;
using CapaEntidad;

namespace SistemaEnviosMaui.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void btnIngresar_Clicked(object sender, EventArgs e)
    {
        string correo = txtCorreo.Text;
        string clave = txtPassword.Text;

        if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(clave))
        {
            await DisplayAlert("Atención", "Por favor llene todos los campos", "OK");
            return;
        }
        
        indicador.IsRunning = true; // Mostramos que está cargando

        // Llamamos a la capa de negocio
        Usuario user = new CN_Usuario().Login(correo, clave);

        indicador.IsRunning = false;

        if (user != null)
        {
            // Si el login es correcto, navegamos a la App principal
            await DisplayAlert("Éxito", "Bienvenido " + user.NombreCompleto, "OK");
            // Esto cierra el Login y te manda a la nueva página
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());

           
        }
        else
        {
            await DisplayAlert("Error", "Correo o contraseña incorrectos", "OK");
        }


    }

}
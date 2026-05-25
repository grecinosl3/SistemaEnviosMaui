using CapaNegocio;
using CapaEntidad;
using System;
using Microsoft.Maui.Controls;

namespace SistemaEnviosMaui.Views
{
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

            indicador.IsRunning = true; 

            // Llamamos a la capa de negocio
            Usuario user = new CN_Usuario().Login(correo, clave);

            indicador.IsRunning = false;

            if (user != null)
            {
                // Si el login es correcto, saludamos al usuario
                await DisplayAlert("Éxito", "Bienvenido " + user.NombreCompleto, "OK");

                Application.Current.MainPage = new PrincipalPage(user);
            }
            else
            {
                await DisplayAlert("Error", "Correo o contraseña incorrectos", "OK");
            }
        }
        private void OnSalirClicked(object sender, EventArgs e)
        {
            // Aquí programas la lógica para salir de la app si lo deseas
        }

        private void OnRegistroClicked(object sender, EventArgs e)
        {
            // Aquí programas la navegación a la pantalla de registro
        }

    }
}
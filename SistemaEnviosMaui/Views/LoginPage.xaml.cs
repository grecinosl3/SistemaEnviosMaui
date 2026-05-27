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
            indicador.IsRunning = true;

            // Llamamos a la capa de negocio
            Usuario user = new CN_Usuario().Login(correo, clave);

            indicador.IsRunning = false;
            indicador.IsVisible = false;

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
        private void OnOjoClicked(object sender, EventArgs e)
        {
            txtPassword.IsPassword = !txtPassword.IsPassword;

            // Cambia el ícono según el estado
            if (sender is Label ojo)
                ojo.Text = txtPassword.IsPassword ? "👁️" : "🙈";
        }
        private void OnSalirClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }

        private async void OnRegistroClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("registro");
        }

        private async void OnSoporteTapped(object sender, TappedEventArgs e)
        {
            try
            {
                // Reemplaza el 50200000000 por el número de soporte de tu país (con código de área sin el +)
                string telefonoSoporte = "50230975787";
                string mensaje = "Hola, necesito soporte técnico. Tengo problemas para acceder al Sistema de Envíos.";

                // Creamos el enlace oficial de la API de WhatsApp
                string urlWhatsApp = $"https://wa.me/{telefonoSoporte}?text={Uri.EscapeDataString(mensaje)}";

                await Launcher.Default.OpenAsync(new Uri(urlWhatsApp));
            }
            catch (Exception ex)
            {
                // Si no tiene WhatsApp instalado (como en la PC de escritorio de prueba), muestra la alerta
                await DisplayAlert("Soporte Técnico", "No se pudo abrir WhatsApp. Por favor, comunícate al teléfono: +502 30975787", "OK");
            }
        }
    }
}
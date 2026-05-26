using Microsoft.Extensions.DependencyInjection;
using SistemaEnviosMaui.Views;

namespace SistemaEnviosMaui
{

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            //Para ver la ventana de chat
            //var pantallaChat = new Views.ChatPage();
            //return new Window(pantallaChat);

            // DESCOMENTAR PARA USAR LA VENTANA PRINCIPAL
            return new Window(new AppShell());

            //var pantallaPrincipal = new NavigationPage(new Views.PrincipalPage("Usuario Prueba"));

            //return new Window(pantallaPrincipal);
        }
    }
}
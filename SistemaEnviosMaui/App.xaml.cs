using Microsoft.Extensions.DependencyInjection;

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
            // DESCOMENTAR PARA USAR LA VENTANA PRINCIPAL
            //return new Window(new AppShell());

            var pantallaPrincipal = new NavigationPage(new Views.PrincipalPage("Usuario Prueba"));

            return new Window(pantallaPrincipal);
        }
    }
}
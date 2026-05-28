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
            // DESCOMENTAR PARA USAR LA VENTANA PRINCIPAL
           return new Window(new AppShell());

            //PARA INGRESAR PERO SIN INICIAR SESION, SOLO PARA VER LA VENTANA PRINCIPAL
            /*
            var usuarioSimulado = new CapaEntidad.Usuario
            {
                IdUsuario = 2,
                NombreCompleto = "Usuario Prueba",
                Correo = "admin@sistema.com",
                Activo = true,
                oRol = new CapaEntidad.Rol { IdRol = 1, NombreRol = "Administrador" }
            };

            var pantallaPrincipal = new NavigationPage(new Views.PrincipalPage(usuarioSimulado));
            return new Window(pantallaPrincipal);
            */
        }
    }
}
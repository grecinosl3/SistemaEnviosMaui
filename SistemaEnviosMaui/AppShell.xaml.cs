using SistemaEnviosMaui.Views;

namespace SistemaEnviosMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("registro", typeof(SistemaEnviosMaui.Views.RegistroPage));
        }
    }
}

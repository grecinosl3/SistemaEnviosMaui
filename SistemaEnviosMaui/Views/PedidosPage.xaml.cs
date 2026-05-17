namespace SistemaEnviosMaui.Views;

public partial class PedidosPage : ContentView
{
    public PedidosPage()
    {
        InitializeComponent();
    }

    private void OnTabClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;

        if (button == btnTabRegistrar)
        {
            viewRegistrar.IsVisible = true;
            viewDetalle.IsVisible = false;

            btnTabRegistrar.BackgroundColor = Color.FromArgb("#3498db");
            btnTabRegistrar.TextColor = Colors.White;
            btnTabDetalle.BackgroundColor = Colors.Transparent;
            btnTabDetalle.TextColor = Color.FromArgb("#888888");
        }
        else
        {
            viewRegistrar.IsVisible = false;
            viewDetalle.IsVisible = true;

            btnTabDetalle.BackgroundColor = Color.FromArgb("#3498db");
            btnTabDetalle.TextColor = Colors.White;
            btnTabRegistrar.BackgroundColor = Colors.Transparent;
            btnTabRegistrar.TextColor = Color.FromArgb("#888888");
        }
    }

    private void OnBuscarClienteClicked(object sender, EventArgs e)
    {
        // 1. Validar que el usuario haya escrito algo
        string codigo = txtCodigoCliente.Text?.Trim();

        if (string.IsNullOrEmpty(codigo))
        {
            // Nota: Al ser ContentView, usamos Application.Current.MainPage para mostrar alertas
            Application.Current.MainPage.DisplayAlert("Atención", "Por favor, ingresa un código de cliente.", "OK");
            return;
        }

        // 2. BUSQUEDA REAL (Aquí llamarías a tu método de Capa Negocio, ej: CN_Cliente.Obtener(codigo))
        // Por ahora simularemos que encuentra a un cliente si escribe "CLI-100" o cualquier código
        if (codigo.ToUpper() == "CLI-100")
        {
            txtNombreCliente.Text = "Juan Carlos Pérez";
            txtDireccionCliente.Text = "6a. Calle 4-12, Zona 1, Jalapa";
        }
        else if (codigo.ToUpper() == "CLI-001")
        {
            txtNombreCliente.Text = "María López";
            txtDireccionCliente.Text = "Av. Chipilapa 2-45, Barrio San Francisco";
        }
        else
        {
            // Si no lo encuentra, limpiamos los campos
            txtNombreCliente.Text = "";
            txtDireccionCliente.Text = "";
            Application.Current.MainPage.DisplayAlert("No encontrado", "El código de cliente no existe en la base de datos.", "OK");
        }
    }

}
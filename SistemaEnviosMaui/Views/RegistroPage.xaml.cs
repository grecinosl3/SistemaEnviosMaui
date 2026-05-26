namespace SistemaEnviosMaui.Views;

public partial class RegistroPage : ContentPage
{
	public RegistroPage()
	{
		InitializeComponent();
	}
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        // Regresa de forma animada a la pantalla anterior (el Login)
        await Shell.Current.GoToAsync("..");
    }
}
namespace SistemaEnviosMaui.Views;

public partial class RegistroPage : ContentPage
{
	public RegistroPage()
	{
		InitializeComponent();
	}
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        
        await Shell.Current.GoToAsync("..");
    }
}
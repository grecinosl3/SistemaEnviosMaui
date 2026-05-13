using System;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using CapaDatos;


namespace SistemaEnviosMaui

{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            
            InitializeComponent();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await ProbarConexion();
            });
        }


        private async Task ProbarConexion()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    con.Open();
                    await DisplayAlertAsync("OK", "Conectado correctamente", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", ex.Message, "Cerrar");
            }
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}

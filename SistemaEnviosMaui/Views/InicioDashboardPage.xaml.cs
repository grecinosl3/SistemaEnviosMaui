using CapaDatos;
using Microsoft.Maui.Controls;
using System;

namespace SistemaEnviosMaui.Views
{
    public partial class InicioDashboardPage : ContentView
    {
        private CD_Dashboard objetoCapaDatos = new CD_Dashboard();

        public InicioDashboardPage()
        {
            InitializeComponent();

            this.Loaded += (s, e) => CargarDatosDashboard();
        }

        private void CargarDatosDashboard()
        {
            try
            {
                var metricas = objetoCapaDatos.ObtenerMetricasDashboard();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblPendientes.Text = metricas.Item1.ToString();
                    lblEnRuta.Text = metricas.Item2.ToString();
                    lblLiquidados.Text = metricas.Item3.ToString();
                    lblTotalEfectivo.Text = string.Format("Q {0:N2}", metricas.Item4);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar el dashboard: " + ex.Message);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblPendientes.Text = "-";
                    lblEnRuta.Text = "-";
                    lblLiquidados.Text = "-";
                    lblTotalEfectivo.Text = "Q 0.00";
                });
            }
        }
    }
}
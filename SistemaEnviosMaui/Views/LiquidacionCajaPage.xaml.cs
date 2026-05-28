using CapaEntidad;
using CapaNegocio;
using CapaDatos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace SistemaEnviosMaui.Views
{
    public partial class LiquidacionCajaPage : ContentView
    {
        private CD_Pedido _cdPedido = new CD_Pedido();
        private CD_Repartidor _cdRepartidor = new CD_Repartidor();

        public ObservableCollection<Pedido> ListaCobros { get; set; } = new ObservableCollection<Pedido>();
        public ObservableCollection<Repartidor> ListaPilotos { get; set; } = new ObservableCollection<Repartidor>();

        public LiquidacionCajaPage()
        {
            InitializeComponent();

            this.BindingContext = this;

            CargarPilotos();
        }

        private void CargarPilotos()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var pilotosBD = _cdRepartidor.ListarRepartidores();

                    ListaPilotos.Clear();
                    cboRepartidores.ItemsSource = null;

                    if (pilotosBD != null && pilotosBD.Count > 0)
                    {
                        foreach (var p in pilotosBD)
                        {
                            if (p.Activo)
                            {
                                ListaPilotos.Add(p);
                            }
                        }
                    }
                    cboRepartidores.ItemsSource = ListaPilotos;
                });
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"No se cargaron los pilotos: {ex.Message}", "OK");
            }
        }

        private void OnPilotoSeleccionadoChanged(object sender, EventArgs e)
        {
            var piloto = cboRepartidores.SelectedItem as Repartidor;
            if (piloto == null) return;

            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lstGuiasACobrar.ItemsSource = null;
                    ListaCobros.Clear();

                    var guiasEntregadas = _cdPedido.ObtenerPedidosParaLiquidar(piloto.IdRepartidor);

                    decimal acumuladoEfectivo = 0;
                    decimal acumuladoFletes = 0;

                    if (guiasEntregadas != null && guiasEntregadas.Count > 0)
                    {
                        foreach (var g in guiasEntregadas)
                        {
                            ListaCobros.Add(g);
                            acumuladoEfectivo += g.Total;
                            acumuladoFletes += g.CostoFlete;
                        }
                    }

                    lstGuiasACobrar.ItemsSource = ListaCobros;

                    lblTotalEfectivo.Text = $"Q {acumuladoEfectivo:F2}";
                    lblTotalFletes.Text = $"Q {acumuladoFletes:F2}";
                    lblCantidadPaquetes.Text = ListaCobros.Count.ToString();

                    btnLiquidar.IsEnabled = ListaCobros.Count > 0;
                });
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error de Filtrado", ex.Message, "OK");
            }
        }

        private async void OnLiquidarCajaClicked(object sender, EventArgs e)
        {
            var piloto = cboRepartidores.SelectedItem as Repartidor;
            if (piloto == null) return;

            bool confirmar = await Application.Current.MainPage.DisplayAlert("Confirmar Arqueo",
                $"¿Confirma que recibió físicamente el efectivo completo de manos de {piloto.NombreCompleto}?", "Sí, Recibido", "Cancelar");

            if (!confirmar) return;

            string mensaje;
            bool exito = _cdPedido.LiquidarPedidosPiloto(piloto.IdRepartidor, out mensaje);

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", "Caja del piloto cerrada correctamente.", "OK");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lstGuiasACobrar.ItemsSource = null;
                    ListaCobros.Clear();

                    lblTotalEfectivo.Text = "Q 0.00";
                    lblTotalFletes.Text = "Q 0.00";
                    lblCantidadPaquetes.Text = "0";
                    btnLiquidar.IsEnabled = false;
                    cboRepartidores.SelectedIndex = -1;
                });
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error en Arqueo", mensaje, "OK");
            }
        }
    }
}
using CapaEntidad;
using CapaNegocio;
using CapaDatos;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace SistemaEnviosMaui.Views
{
    public partial class RepartidoresPage : ContentView
    {
        private CN_Repartidor _cnRepartidor = new CN_Repartidor();
        private CD_Repartidor _cdRepartidor = new CD_Repartidor();

        public ObservableCollection<Repartidor> ListaRepartidores { get; set; } = new ObservableCollection<Repartidor>();

        public RepartidoresPage()
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
                    ListaRepartidores.Clear();
                    List<Repartidor> deBD = _cdRepartidor.ListarRepartidores();

                    if (deBD != null && deBD.Count > 0)
                    {
                        foreach (var piloto in deBD)
                        {
                            ListaRepartidores.Add(piloto);
                        }
                    }
                    lstRepartidores.ItemsSource = ListaRepartidores;
                });
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error de Carga", $"No se pudieron renderizar los pilotos: {ex.Message}", "OK");
            }
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            var piloto = new Repartidor
            {
                IdRepartidor = Convert.ToInt32(txtIdRepartidor.Text),
                Nombre = txtNombre.Text?.Trim(),
                Apellidos = txtApellidos.Text?.Trim(),
                Telefono = txtTelefono.Text?.Trim(),
                TipoVehiculo = cboTipoVehiculo.SelectedItem?.ToString(),
                PlacaVehiculo = txtPlaca.Text?.Trim().ToUpper(),
                Activo = chkActivo.IsChecked
            };

            string mensaje;
            bool exito;

            if (piloto.IdRepartidor == 0)
            {
                exito = _cnRepartidor.Registrar(piloto, out mensaje);
                if (exito) await Application.Current.MainPage.DisplayAlert("Sistema", "Piloto registrado con éxito.", "OK");
            }
            else
            {
                exito = _cnRepartidor.Editar(piloto, out mensaje);
                if (exito) await Application.Current.MainPage.DisplayAlert("Sistema", "Datos actualizados correctamente.", "OK");
            }

            if (exito)
            {
                LimpiarControles();
                CargarPilotos();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Validación", mensaje, "OK");
            }
        }

        private void OnEditarFilaClicked(object sender, EventArgs e)
        {
            var boton = sender as Button;
            var seleccionado = boton?.CommandParameter as Repartidor;

            if (seleccionado != null)
            {
                lblTituloFormulario.Text = "EDITAR DATOS DE PILOTO";
                lblTituloFormulario.TextColor = Color.FromArgb("#e67e22");

                txtIdRepartidor.Text = seleccionado.IdRepartidor.ToString();
                txtNombre.Text = seleccionado.Nombre;
                txtApellidos.Text = seleccionado.Apellidos;
                txtTelefono.Text = seleccionado.Telefono;
                cboTipoVehiculo.SelectedItem = seleccionado.TipoVehiculo;
                txtPlaca.Text = seleccionado.PlacaVehiculo;
                chkActivo.IsChecked = seleccionado.Activo;

                btnCancelar.IsVisible = true;
                btnGuardar.Text = " Actualizar Piloto";
                btnGuardar.BackgroundColor = Color.FromArgb("#e67e22");
            }
        }

        private void OnCancelarClicked(object sender, EventArgs e)
        {
            LimpiarControles();
        }

        private void LimpiarControles()
        {
            lblTituloFormulario.Text = "REGISTRAR NUEVO PILOTO";
            lblTituloFormulario.TextColor = Color.FromArgb("#3498db");

            txtIdRepartidor.Text = "0";
            txtNombre.Text = string.Empty;
            txtApellidos.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            cboTipoVehiculo.SelectedIndex = -1;
            txtPlaca.Text = string.Empty;
            chkActivo.IsChecked = true;

            btnCancelar.IsVisible = false;
            btnGuardar.Text = "Guardar Piloto";
            btnGuardar.BackgroundColor = Color.FromArgb("#2ecc71");
        }
    }
}
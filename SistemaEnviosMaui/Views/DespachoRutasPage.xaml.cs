using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace SistemaEnviosMaui.Views
{
    public partial class DespachoRutasPage : ContentView
    {
        private CN_Pedido _cnPedido = new CN_Pedido();
        private CN_Repartidor _cnRepartidor = new CN_Repartidor();

        public ObservableCollection<Pedido> ListaPendientes { get; set; } = new ObservableCollection<Pedido>();
        public ObservableCollection<Repartidor> ListaRepartidores { get; set; } = new ObservableCollection<Repartidor>();

        private Pedido _pedidoSeleccionado;

        public DespachoRutasPage()
        {
            InitializeComponent();
            lstPendientes.ItemsSource = ListaPendientes;
            CargarDatosPantalla();
        }

        private void CargarDatosPantalla()
        {
            try
            {
                // 1. Cargar las guías que están en estado "Registrado"
                var pendientesBD = _cnPedido.ListarPendientes();
                ListaPendientes.Clear();
                foreach (var p in pendientesBD)
                {
                    ListaPendientes.Add(p);
                }

                // 2. Cargar el combobox con los pilotos de la base de datos
                var repartidoresBD = _cnRepartidor.Listar();
                ListaRepartidores.Clear();
                cboRepartidores.ItemsSource = null; // Reset rápido

                foreach (var r in repartidoresBD)
                {
                    if (r.Activo) // Solo mostramos los pilotos que estén disponibles
                    {
                        ListaRepartidores.Add(r);
                    }
                }
                cboRepartidores.ItemsSource = ListaRepartidores;
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error Logístico", $"Fallo al cargar datos: {ex.Message}", "OK");
            }
        }

        private void OnPedidoSeleccionado(object sender, SelectionChangedEventArgs e)
        {
            _pedidoSeleccionado = e.CurrentSelection.FirstOrDefault() as Pedido;

            if (_pedidoSeleccionado != null)
            {
                lblGuíaSeleccionada.Text = $"Guía # {_pedidoSeleccionado.IdPedido}";
                lblDestinatario.Text = _pedidoSeleccionado.NombreDestinatario;
            }
            else
            {
                lblGuíaSeleccionada.Text = "Ninguna - Seleccione de la lista";
                lblDestinatario.Text = "--";
            }
        }

        private async void OnDespacharClicked(object sender, EventArgs e)
        {
            if (_pedidoSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Por favor, seleccione una guía de la tabla primero.", "OK");
                return;
            }

            var pilotoSeleccionado = cboRepartidores.SelectedItem as Repartidor;
            if (pilotoSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Debe seleccionar un piloto encargado para este despacho.", "OK");
                return;
            }

            string mensaje;
            // Ejecutamos la asignación y cambio automático a "En Ruta"
            bool exito = _cnPedido.DespacharARuta(_pedidoSeleccionado.IdPedido, pilotoSeleccionado.IdRepartidor, out mensaje);

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Despacho Exitoso", $"La guía #{_pedidoSeleccionado.IdPedido} ahora está en ruta con {pilotoSeleccionado.Nombre}.", "OK");

                // Limpiar el formulario derecho
                _pedidoSeleccionado = null;
                lblGuíaSeleccionada.Text = "Ninguna - Seleccione de la lista";
                lblDestinatario.Text = "--";
                cboRepartidores.SelectedIndex = -1;

                // Refrescar la tabla para desaparecer el que se fue a la calle
                CargarDatosPantalla();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", mensaje, "OK");
            }
        }
    }
}
using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace SistemaEnviosMaui.Views
{
    public partial class DespachoRutasPage : ContentView
    {
        private readonly CN_Pedido _cnPedido = new();
        private readonly CN_Repartidor _cnRepartidor = new();

        public ObservableCollection<Pedido> ListaPendientes { get; set; } = new();
        public ObservableCollection<Repartidor> ListaRepartidores { get; set; } = new();

        private Pedido _pedidoSeleccionado;

        public DespachoRutasPage()
        {
            InitializeComponent();

            // Disparar la carga de datos de la Base de Datos en segundo plano de inmediato
            Task.Run(async () => await CargarDatosPantallaAsync());
        }

        private async Task CargarDatosPantallaAsync()
        {
            try
            {
                // Consultas pesadas a la Capa de Negocio (SQL Server) fuera del hilo de UI
                var pedidosBD = _cnPedido.ListarPendientes();
                var repartidoresBD = _cnRepartidor.Listar();

                // Acoplamos los datos de forma segura dentro del MainThread gráfico
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Llenamos la colección de Pedidos Pendientes
                    ListaPendientes.Clear();
                    foreach (var p in pedidosBD)
                    {
                        ListaPendientes.Add(p);
                    }

                    // Llenamos la colección de Repartidores Activos
                    ListaRepartidores.Clear();
                    foreach (var r in repartidoresBD)
                    {
                        if (r.Activo)
                        {
                            ListaRepartidores.Add(r);
                        }
                    }

                    // Forzamos el refresco del enlace de datos (DataBinding manual preventivo)
                    lstPendientes.ItemsSource = null;
                    lstPendientes.ItemsSource = ListaPendientes;

                    cboRepartidores.ItemsSource = null;
                    cboRepartidores.ItemsSource = ListaRepartidores;
                });
            }
            catch (Exception ex)
            {
                // Garantizamos que la alerta de excepción se dibuje en el hilo correcto
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error Logístico", $"Fallo al cargar datos: {ex.Message}", "OK");
                    }
                });
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
            if (Application.Current?.MainPage == null) return;

            if (_pedidoSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Por favor, seleccione una guía de la tabla primero.", "OK");
                return;
            }

            if (cboRepartidores.SelectedItem is not Repartidor pilotoSeleccionado)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Debe seleccionar un piloto encargado para este despacho.", "OK");
                return;
            }

            // Ejecutamos la actualización SQL en background
            bool exito = false;
            string mensaje = string.Empty;
            int idPedido = _pedidoSeleccionado.IdPedido;
            int idRepartidor = pilotoSeleccionado.IdRepartidor;

            await Task.Run(() =>
            {
                exito = _cnPedido.DespacharARuta(idPedido, idRepartidor, out mensaje);
            });

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Despacho Exitoso", $"La guía #{idPedido} ahora está en ruta con {pilotoSeleccionado.Nombre}.", "OK");

                // Restablecemos el formulario derecho de forma limpia
                _pedidoSeleccionado = null;
                lblGuíaSeleccionada.Text = "Ninguna - Seleccione de la lista";
                lblDestinatario.Text = "--";
                cboRepartidores.SelectedItem = null; 

                // Refrescar y volver a consultar la Base de Datos de forma asíncrona
                await CargarDatosPantallaAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", mensaje, "OK");
            }
        }
    }
}
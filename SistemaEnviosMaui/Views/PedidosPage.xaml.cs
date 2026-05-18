using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace SistemaEnviosMaui.Views
{
    public partial class PedidosPage : ContentView
    {
        private CN_Pedido _cnPedido = new CN_Pedido();
        private CN_Cliente _cnCliente = new CN_Cliente();

        public ObservableCollection<DetalleTemporal> ListaDetalles { get; set; } = new ObservableCollection<DetalleTemporal>();
        public ObservableCollection<Pedido> HistorialPedidos { get; set; } = new ObservableCollection<Pedido>();

        private Cliente _clienteCargado = null;
        private int _idProductoSimulado = 1;
        private decimal _totalAcumulado = 0;

        public PedidosPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            lstDetallePedido.ItemsSource = ListaDetalles;
            lstHistorialPedidos.ItemsSource = HistorialPedidos;

            CargarHistorialCompleto();
        }

        // CONTROLADOR DE TABS VIRTUALES
        private void OnTabClicked(object sender, EventArgs e)
        {
            var botonPresionado = sender as Button;

            if (botonPresionado == btnTabRegistrar)
            {
                btnTabRegistrar.BackgroundColor = Color.FromArgb("#3498db");
                btnTabRegistrar.TextColor = Colors.White;
                btnTabDetalle.BackgroundColor = Colors.Transparent;
                btnTabDetalle.TextColor = Color.FromArgb("#888888");

                viewRegistrar.IsVisible = true;
                viewDetalle.IsVisible = false;
            }
            else if (botonPresionado == btnTabDetalle)
            {
                btnTabDetalle.BackgroundColor = Color.FromArgb("#3498db");
                btnTabDetalle.TextColor = Colors.White;
                btnTabRegistrar.BackgroundColor = Colors.Transparent;
                btnTabRegistrar.TextColor = Color.FromArgb("#888888");

                viewRegistrar.IsVisible = false;
                viewDetalle.IsVisible = true;

                CargarHistorialCompleto();
            }
        }

        // BUSCADOR EN BASE DE DATOS DE EMPRESAS (REMITENTE)
        private async void OnBuscarClienteClicked(object sender, EventArgs e)
        {
            string nitIngresado = txtCodigoCliente.Text?.Trim();

            if (string.IsNullOrEmpty(nitIngresado))
            {
                await Application.Current.MainPage.DisplayAlert("Sistema", "Por favor ingresa el NIT o ID corporativo.", "OK");
                return;
            }

            var empresa = _cnCliente.Listar().FirstOrDefault(c =>
                (c.NIT != null && c.NIT.Equals(nitIngresado, StringComparison.OrdinalIgnoreCase)) ||
                c.IdCliente.ToString() == nitIngresado);

            if (empresa != null)
            {
                _clienteCargado = empresa;
                txtNombreCliente.Text = empresa.NombreComercial;
                txtDireccionCliente.Text = empresa.DireccionBodega;
            }
            else
            {
                _clienteCargado = null;
                txtNombreCliente.Text = string.Empty;
                txtDireccionCliente.Text = string.Empty;
                await Application.Current.MainPage.DisplayAlert("Aviso", "Empresa no encontrada en los registros B2B.", "OK");
            }
        }

        // BUSCADOR DE TARIFAS / SERVICIOS LOGÍSTICOS
        private async void OnBuscarProductoClicked(object sender, EventArgs e)
        {
            string codProducto = txtCodProducto.Text?.Trim();

            if (string.IsNullOrEmpty(codProducto))
            {
                await Application.Current.MainPage.DisplayAlert("Sistema", "Ingresa código de servicio (STD/COD).", "OK");
                return;
            }

            if (codProducto.ToUpper() == "STD" || codProducto == "1")
            {
                _idProductoSimulado = 1;
                txtNombreProducto.Text = "Flete Estándar Capital/Departamentos";
                txtPrecioUnitario.Text = "40.00";
            }
            else if (codProducto.ToUpper() == "COD" || codProducto == "2")
            {
                _idProductoSimulado = 2;
                txtNombreProducto.Text = "Servicio Recolección Contra Entrega";
                txtPrecioUnitario.Text = "55.00";
            }
            else
            {
                _idProductoSimulado = 99;
                txtNombreProducto.Text = $"Paquete Especial [{codProducto.ToUpper()}]";
                txtPrecioUnitario.Text = "35.00";
            }
        }

        // GESTIÓN DE LA GRILLA TEMPORAL DE ITEMS
        private async void OnAgregarProductoClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombreProducto.Text))
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Busca un servicio con la lupa primero.", "OK");
                return;
            }

            if (!int.TryParse(txtCantidadProducto.Text, out int cantidad) || cantidad <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Cantidad errónea.", "OK");
                return;
            }

            decimal precio = Convert.ToDecimal(txtPrecioUnitario.Text);
            decimal subtotal = precio * cantidad;

            ListaDetalles.Add(new DetalleTemporal
            {
                IdProducto = _idProductoSimulado,
                NombreProducto = txtNombreProducto.Text,
                PrecioUnitario = precio,
                Cantidad = cantidad,
                Subtotal = subtotal
            });

            RecalcularGranTotal();

            txtCodProducto.Text = string.Empty;
            txtNombreProducto.Text = string.Empty;
            txtPrecioUnitario.Text = string.Empty;
            txtCantidadProducto.Text = "1";
        }

        private void RecalcularGranTotal()
        {
            _totalAcumulado = ListaDetalles.Sum(x => x.Subtotal);
            txtTotalPagar.Text = $"Q {_totalAcumulado:F2}";
        }

        // PROCESAR Y ENVIAR GUÍA CENTRAL A SQL SERVER
        private async void OnRegistrarPedidoClicked(object sender, EventArgs e)
        {
            try
            {
                if (_clienteCargado == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Cargue un remitente corporativo válido.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDestinatario.Text) ||
                    string.IsNullOrWhiteSpace(txtTelefonoDestinatario.Text) ||
                    string.IsNullOrWhiteSpace(txtDireccionEntrega.Text))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Los Datos del Destinatario (Nombre, Teléfono y Dirección) son obligatorios para la ruta.", "OK");
                    return;
                }

                if (cboMetodoPago.SelectedItem == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Defina el método de liquidación del envío.", "OK");
                    return;
                }

                if (ListaDetalles.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Debe asignar al menos un flete o bulto al detalle.", "OK");
                    return;
                }

                // Construimos el objeto respetando las propiedades del Backend de Logística
                var nuevoPedido = new Pedido
                {
                    IdCliente = _clienteCargado.IdCliente,
                    NombreDestinatario = txtDestinatario.Text.Trim(),
                    TelefonoDestinatario = txtTelefonoDestinatario.Text.Trim(),
                    DireccionEntrega = txtDireccionEntrega.Text.Trim(),
                    MetodoPago = cboMetodoPago.SelectedItem.ToString(),
                    Estado = "Registrado",
                    CostoFlete = _totalAcumulado,
                    MontoCOD = 0,
                    Notas = $"Factura: {cboTipoFactura.SelectedItem?.ToString() ?? "No requerida"} | Notas: {txtNotasEnvio.Text?.Trim()}",
                    Detalles = new List<DetallePedido>()
                };

                foreach (var item in ListaDetalles)
                {
                    nuevoPedido.Detalles.Add(new DetallePedido
                    {
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario
                    });
                }

                string mensajeResultado;
                bool exito = _cnPedido.Registrar(nuevoPedido, out mensajeResultado);

                if (exito)
                {
                    await Application.Current.MainPage.DisplayAlert("¡Éxito!", $"Guía registrada correctamente.\nNo. Tracking Asignado: #{mensajeResultado}", "OK");
                    LimpiarFormulario();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Validación de Negocio", mensajeResultado, "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error Crítico", ex.Message, "OK");
            }
        }

        private void LimpiarFormulario()
        {
            _clienteCargado = null;
            txtCodigoCliente.Text = string.Empty;
            txtNombreCliente.Text = string.Empty;
            txtDireccionCliente.Text = string.Empty;

            txtDestinatario.Text = string.Empty;
            txtTelefonoDestinatario.Text = string.Empty;
            txtDireccionEntrega.Text = string.Empty;
            txtNotasEnvio.Text = string.Empty;

            cboMetodoPago.SelectedIndex = -1;
            cboTipoFactura.SelectedIndex = -1;

            ListaDetalles.Clear();
            RecalcularGranTotal();
        }

        // HISTORIAL DE ENVÍOS (MÓDULO TRACKING)
        private void CargarHistorialCompleto()
        {
            try
            {
                var deBD = _cnPedido.Listar();
                HistorialPedidos.Clear();

                foreach (var p in deBD)
                {
                    if (p.oCliente == null)
                        p.oCliente = new Cliente { NombreComercial = "Emisor No Definido" };

                    HistorialPedidos.Add(p);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error historial: {ex.Message}");
            }
        }

        private void OnBuscarHistorialClicked(object sender, EventArgs e)
        {
            string filtro = txtBuscarPedido.Text?.Trim();

            if (string.IsNullOrEmpty(filtro))
            {
                CargarHistorialCompleto();
                return;
            }

            var filtrados = _cnPedido.Listar().Where(p =>
                p.IdPedido.ToString() == filtro ||
                (p.oCliente?.NombreComercial != null && p.oCliente.NombreComercial.Contains(filtro, StringComparison.OrdinalIgnoreCase)) ||
                (p.Estado != null && p.Estado.Contains(filtro, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            HistorialPedidos.Clear();
            foreach (var p in filtrados)
            {
                if (p.oCliente == null) p.oCliente = new Cliente { NombreComercial = "Emisor No Definido" };
                HistorialPedidos.Add(p);
            }
        }

        private async void OnPedidoSeleccionado(object sender, SelectionChangedEventArgs e)
        {
            var seleccionado = e.CurrentSelection.FirstOrDefault() as Pedido;
            if (seleccionado == null) return;

            string opcion = await Application.Current.MainPage.DisplayActionSheet(
                $"Control de Orden #{seleccionado.IdPedido}",
                "Regresar",
                null,
                "Ver Hoja de Ruta / Datos Destino", "Modificar Estado del Envío");

            if (opcion == "Ver Hoja de Ruta / Datos Destino")
            {
                await Application.Current.MainPage.DisplayAlert(
                    $"Destino Orden #{seleccionado.IdPedido}",
                    $"Destinatario: {seleccionado.NombreDestinatario}\nTeléfono: {seleccionado.TelefonoDestinatario}\nDirección Entrega: {seleccionado.DireccionEntrega}\nDetalles adicionales: {seleccionado.Notas}",
                    "Cerrar");
            }
            else if (opcion == "Modificar Estado del Envío")
            {
                string nuevoEstado = await Application.Current.MainPage.DisplayActionSheet(
                    "Mover Estado de Guía",
                    "Cancelar",
                    null,
                    "En Bodega", "En Ruta", "Entregado", "Devuelto");

                if (!string.IsNullOrEmpty(nuevoEstado) && nuevoEstado != "Cancelar")
                {
                    string msg;
                    bool oK = _cnPedido.CambiarEstado(seleccionado.IdPedido, nuevoEstado, out msg);
                    if (oK)
                    {
                        await Application.Current.MainPage.DisplayAlert("Operación Exitosa", $"Estado actualizado a [{nuevoEstado}] con éxito.", "OK");
                        CargarHistorialCompleto();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", msg, "OK");
                    }
                }
            }

            lstHistorialPedidos.SelectedItem = null;
        }
    }

    public class DetalleTemporal
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }

        public string PrecioDisplay => $"Q {PrecioUnitario:F2}";
        public string SubTotalDisplay => $"Q {Subtotal:F2}";
    }
}
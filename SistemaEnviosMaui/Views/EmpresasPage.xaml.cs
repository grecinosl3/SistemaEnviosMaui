using CapaEntidad;
using CapaNegocio;
using System.Collections.ObjectModel;

namespace SistemaEnviosMaui.Views
{
    public partial class ClientesView : ContentView
    {
        // Colección reactiva para llenar el Grid/CollectionView de la derecha
        public ObservableCollection<Cliente> MisClientes { get; set; } = new ObservableCollection<Cliente>();

        // Instancia de la Capa de Negocio
        private CN_Cliente _cnCliente = new CN_Cliente();

        // Variable global para saber a qué empresa tenemos seleccionada en la tabla
        private Cliente _clienteSeleccionado;

        public ClientesView()
        {
            InitializeComponent();

            // Vinculamos la vista con este código para que el CollectionView reconozca "MisClientes"
            this.BindingContext = this;

            // Cargamos la tabla en cuanto se abra la pantalla
            CargarClientesBD();

            // Forzamos al picker de estado a arrancar por defecto en "Activo"
            pckEstado.SelectedIndex = 0;
        }

        // 1. CARGAR LISTADO DESDE BASE DE DATOS
        private void CargarClientesBD()
        {
            try
            {
                var listaDesdeBD = _cnCliente.Listar();
                MisClientes.Clear();

                foreach (var c in listaDesdeBD)
                {
                    MisClientes.Add(c);
                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error al cargar", ex.Message, "OK");
            }
        }

        // 2. BOTÓN GUARDAR (REGISTRAR O EDITAR)
        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            try
            {
                // Armamos el objeto Cliente con lo que el usuario escribió en los Entry
                var clienteFormulario = new Cliente
                {
                    IdCliente = _clienteSeleccionado?.IdCliente ?? 0, // Si es nuevo es 0, si estamos editando mantiene su ID
                    NombreComercial = txtNombreComercial.Text?.Trim(),
                    RazonSocial = txtRazonSocial.Text?.Trim(),
                    NIT = txtNIT.Text?.Trim(),
                    NombreContacto = txtNombreContacto.Text?.Trim(),
                    TelefonoContacto = txtTelefonoContacto.Text?.Trim(),
                    CorreoContacto = txtCorreoContacto.Text?.Trim(),
                    DireccionBodega = txtDireccionBodega.Text?.Trim(),
                    Banco = txtBanco.Text?.Trim(),
                    CuentaBancaria = txtCuentaBancaria.Text?.Trim(),
                    Activo = pckEstado.SelectedItem?.ToString() == "Activo"
                };

                string mensaje;

                // SI ES UN CLIENTE NUEVO (REGISTRAR)
                if (clienteFormulario.IdCliente == 0)
                {
                    int idGenerado = _cnCliente.Registrar(clienteFormulario, out mensaje);

                    if (idGenerado > 0)
                    {
                        clienteFormulario.IdCliente = idGenerado;
                        clienteFormulario.FechaRegistro = DateTime.Now;

                        MainThread.BeginInvokeOnMainThread(() => {
                            MisClientes.Add(clienteFormulario); // Lo agrega inmediatamente a la tabla
                        });

                        await Application.Current.MainPage.DisplayAlert("¡Éxito!", "Empresa afiliada correctamente.", "OK");
                        OnLimpiarClicked(null, null);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Validación", mensaje, "OK");
                    }
                }
                // SI YA EXISTÍA (EDITAR)
                else
                {
                    bool editadoExitoso = _cnCliente.Editar(clienteFormulario, out mensaje);

                    if (editadoExitoso)
                    {
                        await Application.Current.MainPage.DisplayAlert("¡Éxito!", "Datos de la empresa actualizados.", "OK");
                        CargarClientesBD(); // Recarga la tabla para ver los cambios reflejados
                        OnLimpiarClicked(null, null);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Validación", mensaje, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error Crítico", ex.Message, "OK");
            }
        }

        // 3. SELECCIONAR UN CLIENTE DE LA TABLA (Para cargarlo en los campos de texto)
        private void OnClienteSeleccionado(object sender, SelectionChangedEventArgs e)
        {
            _clienteSeleccionado = e.CurrentSelection.FirstOrDefault() as Cliente;

            if (_clienteSeleccionado != null)
            {
                // Pasamos los datos de la fila seleccionada a las cajas de texto
                txtNombreComercial.Text = _clienteSeleccionado.NombreComercial;
                txtRazonSocial.Text = _clienteSeleccionado.RazonSocial;
                txtNIT.Text = _clienteSeleccionado.NIT;
                txtNombreContacto.Text = _clienteSeleccionado.NombreContacto;
                txtTelefonoContacto.Text = _clienteSeleccionado.TelefonoContacto;
                txtCorreoContacto.Text = _clienteSeleccionado.CorreoContacto;
                txtDireccionBodega.Text = _clienteSeleccionado.DireccionBodega;
                txtBanco.Text = _clienteSeleccionado.Banco;
                txtCuentaBancaria.Text = _clienteSeleccionado.CuentaBancaria;

                pckEstado.SelectedItem = _clienteSeleccionado.Activo ? "Activo" : "Inactivo";

                // Cambiamos el texto del botón principal para guiar al usuario
                btnGuardar.Text = "ACTUALIZAR DATOS";
            }
        }

        // 4. DAR DE BAJA / DESACTIVAR (Eliminar en modelo logístico)
        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            if (_clienteSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Selecciona una empresa de la lista primero.", "OK");
                return;
            }

            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Confirmar",
                $"¿Seguro que deseas dar de baja a la empresa '{_clienteSeleccionado.NombreComercial}'?\n(Esto no borrará sus guías pasadas, pero impedirá generar nuevos envíos).",
                "Sí, Desactivar", "No");

            if (confirmar)
            {
                string mensaje;
                bool dadoDeBaja = _cnCliente.Eliminar(_clienteSeleccionado, out mensaje);

                if (dadoDeBaja)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "La empresa ha sido inactivada del sistema.", "OK");
                    CargarClientesBD(); // Actualiza la tabla visual
                    OnLimpiarClicked(null, null);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", mensaje, "OK");
                }
            }
        }

        // 5. LIMPIAR EL FORMULARIO
        private void OnLimpiarClicked(object sender, EventArgs e)
        {
            txtNombreComercial.Text = string.Empty;
            txtRazonSocial.Text = string.Empty;
            txtNIT.Text = string.Empty;
            txtNombreContacto.Text = string.Empty;
            txtTelefonoContacto.Text = string.Empty;
            txtCorreoContacto.Text = string.Empty;
            txtDireccionBodega.Text = string.Empty;
            txtBanco.Text = string.Empty;
            txtCuentaBancaria.Text = string.Empty;

            pckEstado.SelectedIndex = 0; // Regresa a Activo por defecto
            _clienteSeleccionado = null; // Reseteamos la selección

            btnGuardar.Text = "GUARDAR CLIENTE"; // Regresa el botón a su estado original
        }
    }
}
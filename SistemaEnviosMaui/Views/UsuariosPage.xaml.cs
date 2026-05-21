using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace SistemaEnviosMaui.Views
{
    public partial class UsuariosPage : ContentView
    {
        public ObservableCollection<Usuario> MisUsuarios { get; set; } = new ObservableCollection<Usuario>();

        private CN_Usuario _cnUsuario = new CN_Usuario();
        private Usuario _usuarioSeleccionado;

        public UsuariosPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            CargarUsuariosBD();

            // Valores por defecto para el formulario
            pckEstado.SelectedIndex = 0;
            pckRol.SelectedIndex = 1; // Por defecto Operador Logístico
        }

        // 1. CARGAR DATOS DESDE LA DB (CON PARCHE DE SEGURIDAD MULTIHILO)
        private void CargarUsuariosBD()
        {
            try
            {
                var lista = _cnUsuario.Listar();

                // Forzamos la ejecución en el hilo principal de la pantalla
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MisUsuarios.Clear();
                    foreach (var u in lista)
                    {
                        if (u.oRol == null)
                        {
                            u.oRol = new Rol();
                            u.oRol.NombreRol = "Personal del Sistema";
                        }
                        else if (string.IsNullOrEmpty(u.oRol.NombreRol))
                        {
                            u.oRol.NombreRol = "Asignado";
                        }

                        MisUsuarios.Add(u);
                    }
                });
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", "No se pudo cargar el personal: " + ex.Message, "OK");
            }
        }

        // 2. GUARDAR / EDITAR
        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            try
            {
                var usuarioFormulario = new Usuario
                {
                    IdUsuario = _usuarioSeleccionado?.IdUsuario ?? 0,
                    NombreCompleto = txtNombre.Text?.Trim(),
                    Correo = txtCorreo.Text?.Trim(),
                    Telefono = txtTelefono.Text?.Trim(),
                    Contrasena = txtClave.Text?.Trim(),
                    oRol = new Rol { IdRol = pckRol.SelectedIndex + 1 },
                    Activo = pckEstado.SelectedItem?.ToString() == "Activo"
                };

                string mensaje;

                if (usuarioFormulario.IdUsuario == 0) // NUEVO
                {
                    int idGenerado = _cnUsuario.Registrar(usuarioFormulario, out mensaje);

                    if (idGenerado > 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("¡Éxito!", "Trabajador registrado en el sistema.", "OK");
                        CargarUsuariosBD();
                        OnLimpiarClicked(null, null);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Validación", mensaje, "OK");
                    }
                }
                else // EDITAR
                {
                    bool editado = _cnUsuario.Editar(usuarioFormulario, out mensaje);

                    if (editado)
                    {
                        await Application.Current.MainPage.DisplayAlert("¡Éxito!", "Datos del usuario actualizados.", "OK");
                        CargarUsuariosBD();
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

        // 3. SELECCIÓN DE FILA
        private void OnUsuarioSeleccionado(object sender, SelectionChangedEventArgs e)
        {
            _usuarioSeleccionado = e.CurrentSelection.FirstOrDefault() as Usuario;

            if (_usuarioSeleccionado != null)
            {
                txtNombre.Text = _usuarioSeleccionado.NombreCompleto;
                txtCorreo.Text = _usuarioSeleccionado.Correo;
                txtTelefono.Text = _usuarioSeleccionado.Telefono;
                txtClave.Text = _usuarioSeleccionado.Contrasena;
                txtDocumento.Text = string.Empty;

                if (_usuarioSeleccionado.oRol != null)
                {
                    pckRol.SelectedIndex = _usuarioSeleccionado.oRol.IdRol - 1;
                }

                pckEstado.SelectedItem = _usuarioSeleccionado.Activo ? "Activo" : "Inactivo";
                btnGuardar.Text = "ACTUALIZAR PERSONAL";
            }
        }

        // 4. BAJA DE TRABAJADOR
        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Selecciona un usuario de la lista.", "OK");
                return;
            }

            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Confirmar",
                $"¿Deseas dar de baja a {_usuarioSeleccionado.NombreCompleto}?\nNo podrá ingresar al sistema.",
                "Sí, Inactivar",
                "No"
            );

            if (confirmar)
            {
                string mensaje;
                bool eliminado = _cnUsuario.Eliminar(_usuarioSeleccionado, out mensaje);

                if (eliminado)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Usuario inactivado.", "OK");
                    CargarUsuariosBD();
                    OnLimpiarClicked(null, null);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", mensaje, "OK");
                }
            }
        }

        // 5. LIMPIAR FORMULARIO
        private void OnLimpiarClicked(object sender, EventArgs e)
        {
            txtNombre.Text = string.Empty;
            txtDocumento.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            txtCorreo.Text = string.Empty;
            txtClave.Text = string.Empty;
            pckRol.SelectedIndex = 1;
            pckEstado.SelectedIndex = 0;

            _usuarioSeleccionado = null;
            lstUsuarios.SelectedItem = null;
            btnGuardar.Text = "GUARDAR USUARIO";
        }
    }
}
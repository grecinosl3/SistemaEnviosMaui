using CapaEntidad;
using System.Collections.ObjectModel;

namespace SistemaEnviosMaui.Views;

public partial class UsuariosView : ContentView
{
    public ObservableCollection<Usuario> MisUsuarios { get; set; } = new ObservableCollection<Usuario>();
    public ObservableCollection<Rol> MisRoles { get; set; } = new ObservableCollection<Rol>();

    Usuario usuarioSeleccionado;

    public UsuariosView()
    {
        InitializeComponent();

        // 1. Inicializamos la lista
        MisUsuarios = new ObservableCollection<Usuario>();
        MisRoles = new ObservableCollection<Rol>();

        this.BindingContext = this;

        // 2. Cargamos los datos de la BD
        CargarRoles();
        CargarUsuariosBD();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            // --- PASO 2: Obtener el objeto Rol real del Picker ---
            var rolSeleccionado = pckRol.SelectedItem as Rol;

            if (rolSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Por favor, selecciona un rol para el usuario", "OK");
                return;
            }

            // 1. Creamos el objeto Usuario con el ID REAL del rol
            var nuevoUsuario = new Usuario
            {
                NombreCompleto = txtNombre.Text,
                Correo = txtCorreo.Text,
                Telefono = "",
                Contrasena = "123",
                Activo = true,
                oRol = new Rol
                {
                    IdRol = rolSeleccionado.IdRol, 
                    NombreRol = rolSeleccionado.NombreRol
                }
            };

            // 2. Llamamos a tu CapaDatos.Registrar (Esto ya lo tenías perfecto)
            string mensaje;
            int idGenerado = new CapaDatos.CD_Usuario().Registrar(nuevoUsuario, out mensaje);

            if (idGenerado != 0)
            {
                // 3. Si se guardó en SQL, lo agregamos a la lista visual
                nuevoUsuario.IdUsuario = idGenerado;
                MainThread.BeginInvokeOnMainThread(() => {
                    MisUsuarios.Add(nuevoUsuario);
                });

                await Application.Current.MainPage.DisplayAlert("¡Éxito!", "Guardado en Base de Datos", "OK");
                OnLimpiarClicked(null, null);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", mensaje, "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error Crítico", ex.Message, "OK");
        }
    }

    private void OnLimpiarClicked(object sender, EventArgs e)
    {
        txtNombre.Text = string.Empty;
        txtCorreo.Text = string.Empty;
        if (pckRol != null) pckRol.SelectedIndex = -1;
        if (pckEstado != null) pckEstado.SelectedIndex = -1;
    }

    private void CargarUsuariosBD()
    {
        // Aquí es donde invocas a tu CapaDatos
        var listaDesdeBD = new CapaDatos.CD_Usuario().ListarUsuarios();

        MisUsuarios.Clear();

        foreach (var u in listaDesdeBD)
        {
            MisUsuarios.Add(u);
        }
    }


    // En el constructor, después de inicializar la lista:
    private void CargarRoles()
    {

        MisRoles.Clear();
        MisRoles.Add(new Rol { IdRol = 1, NombreRol = "Administrador" });
        MisRoles.Add(new Rol { IdRol = 2, NombreRol = "Moderador" });
        MisRoles.Add(new Rol { IdRol = 3, NombreRol = "Repartidor" });

        pckRol.ItemsSource = MisRoles;
        pckRol.ItemDisplayBinding = new Binding("NombreRol");
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        // 1. Verificar si hay alguien seleccionado
        if (usuarioSeleccionado == null)
        {
            await Application.Current.MainPage.DisplayAlert("Atención", "Selecciona un usuario de la lista primero", "OK");
            return;
        }

        // 2. Preguntar confirmación (Importante para no borrar por error)
        bool confirmar = await Application.Current.MainPage.DisplayAlert("Confirmar", $"¿Deseas eliminar a {usuarioSeleccionado.NombreCompleto}?", "Sí", "No");

        if (confirmar)
        {
            string mensaje;
            // 3. Llamar a la Capa Datos
            bool eliminado = new CapaDatos.CD_Usuario().Eliminar(usuarioSeleccionado.IdUsuario, out mensaje);

            if (eliminado)
            {
                // 4. Quitarlo de la lista visual (ObservableCollection)
                MisUsuarios.Remove(usuarioSeleccionado);
                await Application.Current.MainPage.DisplayAlert("Éxito", "Usuario eliminado", "OK");
                OnLimpiarClicked(null, null); // Limpiamos los campos
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", mensaje, "OK");
            }
        }
    }

    // Este método sirve para capturar al usuario cuando lo tocas en la lista
    private void OnUsuarioSeleccionado(object sender, SelectionChangedEventArgs e)
    {
        usuarioSeleccionado = e.CurrentSelection.FirstOrDefault() as Usuario;

        if (usuarioSeleccionado != null)
        {
            // Opcional: Llenar los campos de la izquierda con los datos del seleccionado
            txtNombre.Text = usuarioSeleccionado.NombreCompleto;
            txtCorreo.Text = usuarioSeleccionado.Correo;
            // los demas
        }
    }


}
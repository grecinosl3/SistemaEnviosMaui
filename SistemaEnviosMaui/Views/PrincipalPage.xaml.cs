using CapaEntidad;
using SistemaEnviosMaui.Views;
using System;
using Microsoft.Maui.Controls;

namespace SistemaEnviosMaui.Views
{
    public partial class PrincipalPage : ContentPage
    {
        private Usuario _usuarioActual; // 👈 Guardamos los datos del login aquí
        // Modificamos el constructor para que reciba el objeto Usuario completo
        public PrincipalPage(Usuario usuarioLogueado)
        {
            InitializeComponent();

            // Si por alguna razón viene nulo, lo manejamos como invitado para evitar caídas
            if (usuarioLogueado != null)
            {
                _usuarioActual = usuarioLogueado;
                // 1. Pintamos el nombre real en el encabezado azul
                lblUsuarioNombre.Text = usuarioLogueado.NombreCompleto;

                // 2. Ejecutamos el filtro de seguridad según su Rol
                CargarMenuSegunRol(usuarioLogueado);
            }
            else
            {
                lblUsuarioNombre.Text = "Invitado";
            }
        }

        // Método encargado de ocultar o mostrar las opciones del menú superior
        private void CargarMenuSegunRol(Usuario usuarioLogueado)
        {
            // Apagamos todas las pestañas primero por seguridad (Lista negra)
            MenuUsuarios.IsVisible = false;
            MenuClientes.IsVisible = false;
            MenuInventario.IsVisible = false;
            MenuPedidos.IsVisible = false;
            MenuRepartidores.IsVisible = false;
            MenuFacturas.IsVisible = false;
            MenuDespacho.IsVisible = false;
            MenuMensajes.IsVisible = false;

            // La pantalla de Inicio siempre se queda activa para todos
            MenuInicio.IsVisible = true;

            // Encendemos los accesos permitidos según el IdRol
            switch (usuarioLogueado.oRol.IdRol)
            {
                case 1: //  ADMINISTRADOR
                    MenuUsuarios.IsVisible = true;
                    MenuClientes.IsVisible = true;
                    MenuInventario.IsVisible = true;
                    MenuPedidos.IsVisible = true;
                    MenuRepartidores.IsVisible = true;
                    MenuFacturas.IsVisible = true;
                    MenuDespacho.IsVisible = true;
                    MenuMensajes.IsVisible = true;
                    break;

                case 2: // MODERADOR (Operador Logístico)
                    MenuClientes.IsVisible = true;
                    MenuInventario.IsVisible = true;
                    MenuPedidos.IsVisible = true;
                    MenuRepartidores.IsVisible = true;
                    MenuFacturas.IsVisible = true;
                    MenuDespacho.IsVisible = true;
                    MenuMensajes.IsVisible = true;
                    break;

                case 3: //  REPARTIDOR (Piloto)
                    MenuPedidos.IsVisible = true;
                    MenuDespacho.IsVisible = true;
                    MenuMensajes.IsVisible = true;
                    break;

                case 4: //  CLIENTE (Distribuidora)
                    MenuPedidos.IsVisible = true;
                    break;
            }
        }
        private void IluminarPestanaActiva(string opcionSeleccionada)
        {
            // 1. Primero volvemos a poner TODOS los botones en su estado "Apagado" (Oscuro)
            Border[] todosLosMenus = { MenuInicio, MenuUsuarios, MenuClientes, MenuInventario, MenuPedidos, MenuRepartidores, MenuFacturas, MenuDespacho, MenuMensajes };

            foreach (var menu in todosLosMenus)
            {
                if (menu != null)
                {
                    menu.BackgroundColor = Color.FromArgb("#1E293B"); // Gris oscuro original
                    menu.Stroke = Color.FromArgb("#2A364F");          // Borde apagado
                }
            }

            // 2. Encendemos únicamente el que seleccionó el usuario (Azul Neón Eléctrico)
            switch (opcionSeleccionada)
            {
                case "Inicio":
                    MenuInicio.BackgroundColor = Color.FromArgb("#1E3A8A"); 
                    MenuInicio.Stroke = Color.FromArgb("#3B82F6");          
                    break;
                case "Usuarios":
                    MenuUsuarios.BackgroundColor = Color.FromArgb("#1E3A8A");
                    MenuUsuarios.Stroke = Color.FromArgb("#3B82F6");
                    break;
                case "Clientes":
                    MenuClientes.BackgroundColor = Color.FromArgb("#1E3A8A");
                    MenuClientes.Stroke = Color.FromArgb("#3B82F6");
                    break;
                case "Inventario":
                    MenuInventario.BackgroundColor = Color.FromArgb("#1E3A8A");
                    MenuInventario.Stroke = Color.FromArgb("#3B82F6");
                    break;
                case "Pedidos":
                    MenuPedidos.BackgroundColor = Color.FromArgb("#1E3A8A");
                    MenuPedidos.Stroke = Color.FromArgb("#3B82F6");
                    break;
                case "Repartidores":
                    MenuRepartidores.BackgroundColor = Color.FromArgb("#1E3A8A");
                    MenuRepartidores.Stroke = Color.FromArgb("#3B82F6");
                    break;
                case "Facturas":
                    MenuFacturas.BackgroundColor = Color.FromArgb("#1E3A8A");
                    MenuFacturas.Stroke = Color.FromArgb("#3B82F6");
                    break;
                case "Despacho":
                    MenuDespacho.BackgroundColor = Color.FromArgb("#1E3A8A");
                    MenuDespacho.Stroke = Color.FromArgb("#3B82F6");
                    break;
                case "Mensajes":
                    MenuMensajes.BackgroundColor = Color.FromArgb("#1E3A8A");
                    MenuMensajes.Stroke = Color.FromArgb("#3B82F6");
                    break;
            }
        }

        private async void OnCerrarSesionClicked(object sender, EventArgs e)
        {
            bool seguro = await DisplayAlert("Cerrar Sesión", "¿Seguro que deseas salir del Sistema de Envíos?", "Sí, Salir", "Cancelar");

            if (seguro)
            {
                // Cambiamos la página principal del celular/computadora directo al Login
                // Nota: Reemplaza 'LoginPage' por el nombre exacto de tu ventana de Login si se llama distinto
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        private void OnMenuOptionTapped(object sender, TappedEventArgs e)
        {
            // Validamos que el parámetro no venga vacío para evitar caídas
            if (e.Parameter == null) return;

            // Obtenemos el nombre de la opción (Inicio, Usuarios, Clientes, etc.)
            var opcion = e.Parameter.ToString();

            IluminarPestanaActiva(opcion);

            switch (opcion)
            {
                case "Inicio":
                    ContenedorPrincipal.Content = new InicioDashboardPage();
                    break;

                case "Usuarios":
                    ContenedorPrincipal.Content = new UsuariosPage();
                    break;

                case "Clientes":
                    ContenedorPrincipal.Content = new ClientesView();
                    break;

                case "Inventario":
                    ContenedorPrincipal.Content = new InventarioPage();
                    break;

                case "Pedidos":
                    ContenedorPrincipal.Content = new PedidosPage();
                    break;

                case "Repartidores":
                    ContenedorPrincipal.Content = new RepartidoresPage();
                    break;

                case "Facturas":
                    ContenedorPrincipal.Content = new LiquidacionCajaPage();
                    break;

                case "Despacho":
                    ContenedorPrincipal.Content = new DespachoRutasPage();
                    break;
                case "Mensajes":
                    int idUsuarioReal = _usuarioActual != null ? _usuarioActual.IdUsuario : 1;
                    int idSalaChat = 1; // Tu sala comodín por ahora

                    // Inyectamos el chat en el contenedor pasándole sus parámetros reales
                    ContenedorPrincipal.Content = new ChatPage(idUsuarioReal, idSalaChat);
                    break;
            }
        }
    }
}
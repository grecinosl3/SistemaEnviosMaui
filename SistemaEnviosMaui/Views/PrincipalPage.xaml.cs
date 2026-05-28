using CapaEntidad;
using SistemaEnviosMaui.Views;
using System;
using Microsoft.Maui.Controls;

namespace SistemaEnviosMaui.Views
{
    public partial class PrincipalPage : ContentPage
    {
        private Usuario _usuarioActual;
        
        public PrincipalPage(Usuario usuarioLogueado)
        {
            InitializeComponent();

            if (usuarioLogueado != null)
            {
                _usuarioActual = usuarioLogueado;
                lblUsuarioNombre.Text = usuarioLogueado.NombreCompleto;

                CargarMenuSegunRol(usuarioLogueado);
            }
            else
            {
                lblUsuarioNombre.Text = "Invitado";
            }
        }

        private void CargarMenuSegunRol(Usuario usuarioLogueado)
        {
            MenuUsuarios.IsVisible = false;
            MenuClientes.IsVisible = false;
            MenuInventario.IsVisible = false;
            MenuPedidos.IsVisible = false;
            MenuRepartidores.IsVisible = false;
            MenuFacturas.IsVisible = false;
            MenuDespacho.IsVisible = false;
            MenuMensajes.IsVisible = false;

            MenuInicio.IsVisible = true;

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

                case 2: // MODERADOR 
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

                case 4: //  CLIENTE 
                    MenuPedidos.IsVisible = true;
                    break;
            }
        }
        private void IluminarPestanaActiva(string opcionSeleccionada)
        {
            Border[] todosLosMenus = { MenuInicio, MenuUsuarios, MenuClientes, MenuInventario, MenuPedidos, MenuRepartidores, MenuFacturas, MenuDespacho, MenuMensajes };

            foreach (var menu in todosLosMenus)
            {
                if (menu != null)
                {
                    menu.BackgroundColor = Color.FromArgb("#1E293B"); 
                    menu.Stroke = Color.FromArgb("#2A364F");         
                }
            }

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
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        private void OnMenuOptionTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter == null) return;

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

                    ContenedorPrincipal.Content = new ListaChatsPage(idUsuarioReal);
                    break;
            }
        }

        private void OnSidebarPointerEntered(object sender, PointerEventArgs e)
        {
            txtInicio.IsVisible = true;
            txtUsuarios.IsVisible = true;
            txtClientes.IsVisible = true;
            txtInventario.IsVisible = true;
            txtPedidos.IsVisible = true;
            txtRepartidores.IsVisible = true;
            txtFacturas.IsVisible = true;
            txtDespacho.IsVisible = true;
            txtMensajes.IsVisible = true;

            lblLogoTexto.IsVisible = true;
            lblUsuarioNombre.IsVisible = true;

            Animation anchoAnimation = new Animation(v => SidebarBorder.WidthRequest = v, 75, 220);
            anchoAnimation.Commit(this, "ExpandirSidebar", length: 180, easing: Easing.CubicOut);
        }

        private void OnSidebarPointerExited(object sender, PointerEventArgs e)
        {
            txtInicio.IsVisible = false;
            txtUsuarios.IsVisible = false;
            txtClientes.IsVisible = false;
            txtInventario.IsVisible = false;
            txtPedidos.IsVisible = false;
            txtRepartidores.IsVisible = false;
            txtFacturas.IsVisible = false;
            txtDespacho.IsVisible = false;
            txtMensajes.IsVisible = false;

            lblLogoTexto.IsVisible = false;
            lblUsuarioNombre.IsVisible = false;

            Animation anchoAnimation = new Animation(v => SidebarBorder.WidthRequest = v, 220, 75);
            anchoAnimation.Commit(this, "ColapsarSidebar", length: 180, easing: Easing.CubicIn);
        }
    }
}
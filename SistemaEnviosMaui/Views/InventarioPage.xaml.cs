using CapaEntidad;
using CapaNegocio;

namespace SistemaEnviosMaui.Views;

public partial class InventarioPage : ContentView
{
    private int _idProductoSeleccionado = 0;
    public InventarioPage()
    {
        InitializeComponent();
        CargarCombos();
        CargarListaProductos();
    }
    private void CargarCombos()
    {
        // Cargar Estados (Activo/No Activo)
        cboestado.ItemsSource = new List<string> { "Activo", "No Activo" };
        cboestado.SelectedIndex = 0;

        // Cargar Categorías desde la Capa de Negocio
        try
        {
            List<Categoria> listaCategoria = new CN_Categoria().Listar();
            cbocategoria.ItemsSource = listaCategoria;
            cbocategoria.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al cargar categorías: " + ex.Message);
        }
    }
    private void CargarListaProductos()
    {
        List<Producto> lista = new CN_Producto().Listar();

        dgvdata.ItemsSource = lista;
    }

    private async void btnguardar_Click(object sender, EventArgs e)
    {
        string mensaje = string.Empty;

        
        Producto obj = new Producto()
        {
            IdProducto = _idProductoSeleccionado,
            Codigo = txtcodigo.Text,
            Nombre = txtnombre.Text,
            Descripcion = txtdescripcion.Text,
            
            Precio = 0, 
            Stock = 0,  
            oCategoria = (Categoria)cbocategoria.SelectedItem,
            Activo = cboestado.SelectedItem.ToString() == "Activo"
        };

        if (obj.IdProducto == 0)
        {
            bool resultado = new CN_Producto().Registrar(obj, out mensaje);

            if (resultado)
            {
                await Shell.Current.DisplayAlert("Sistema", "Producto Registrado", "OK");
                Limpiar();
                CargarListaProductos();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", mensaje, "OK");
            }
        }
        else
        {
            bool resultado = new CN_Producto().Editar(obj, out mensaje);

            if (resultado)
            {
                await Shell.Current.DisplayAlert("Sistema", "Producto Actualizado", "OK");
                Limpiar();
                CargarListaProductos();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", mensaje, "OK");
            }
        }
    }

    private void btnlimpiar_Click(object sender, EventArgs e)
    {
        Limpiar();
    }

    private async void btneliminar_Click(object sender, EventArgs e)
    {
        // Validamos que haya un producto seleccionado
        if (_idProductoSeleccionado == 0)
        {
            await Shell.Current.DisplayAlert("Sistema", "Por favor, seleccione un producto de la lista para eliminar.", "OK");
            return;
        }

        // Pedimos confirmación al usuario
        bool respuesta = await Shell.Current.DisplayAlert("Confirmación", "¿Está seguro de que desea eliminar este producto?", "Sí", "No");

        if (respuesta)
        {
            string mensaje = string.Empty;

            Producto obj = new Producto() { IdProducto = _idProductoSeleccionado };

            bool resultado = new CN_Producto().Eliminar(obj, out mensaje);

            if (resultado)
            {
                await Shell.Current.DisplayAlert("Sistema", "Producto eliminado correctamente.", "OK");

                Limpiar();
                CargarListaProductos();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", mensaje, "OK");
            }
        }

    }

    private void Limpiar()
    {
        txtcodigo.Text = "";
        txtnombre.Text = "";
        cbocategoria.SelectedIndex = 0;
        cboestado.SelectedIndex = 0;
        txtcodigo.Focus();
    }
    private void dgvdata_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Producto seleccionado)
        {
            txtcodigo.Text = seleccionado.Codigo;
            txtnombre.Text = seleccionado.Nombre;
            txtdescripcion.Text = seleccionado.Descripcion;

            cbocategoria.SelectedItem = cbocategoria.ItemsSource.Cast<Categoria>()
                .FirstOrDefault(c => c.IdCategoria == seleccionado.oCategoria.IdCategoria);

            cboestado.SelectedItem = seleccionado.Activo ? "Activo" : "No Activo";

            _idProductoSeleccionado = seleccionado.IdProducto;
        }
    }
}
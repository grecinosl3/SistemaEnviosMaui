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
        // Llamamos al método de carga al iniciar
        CargarListaProductos();
    }
    private void CargarCombos()
    {
        // 1. Cargar Estados (Activo/No Activo)
        cboestado.ItemsSource = new List<string> { "Activo", "No Activo" };
        cboestado.SelectedIndex = 0;

        // 2. Cargar Categorías desde la Capa de Negocio
        try
        {
            List<Categoria> listaCategoria = new CN_Categoria().Listar();
            cbocategoria.ItemsSource = listaCategoria;
            cbocategoria.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            // Por ahora un log simple si falla la conexión
            Console.WriteLine("Error al cargar categorías: " + ex.Message);
        }
    }
    private void CargarListaProductos()
    {
        // Usamos tu Capa de Negocio que ya adaptamos
        List<Producto> lista = new CN_Producto().Listar();

        // Asignamos la lista al CollectionView que nombramos en el XAML
        dgvdata.ItemsSource = lista;
    }

    private void btnguardar_Click(object sender, EventArgs e)
    {
        // Aquí programaremos el registro y edición
    }

    private void btnlimpiar_Click(object sender, EventArgs e)
    {
        Limpiar();
    }

    private void btneliminar_Click(object sender, EventArgs e)
    {
        // Aquí programaremos la eliminación
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

            // Seleccionar categoría en el Picker
            cbocategoria.SelectedItem = cbocategoria.ItemsSource.Cast<Categoria>()
                .FirstOrDefault(c => c.IdCategoria == seleccionado.oCategoria.IdCategoria);

            // Seleccionar estado
            cboestado.SelectedItem = seleccionado.Activo ? "Activo" : "No Activo";

            // Guardamos el ID en una variable para saber que vamos a EDITAR y no a INSERTAR
            _idProductoSeleccionado = seleccionado.IdProducto;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace CapaEntidad
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; }
        public int IdCategoria { get; set; }
        public Categoria oCategoria { get; set; }

    }
}

using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapaNegocio
{
    public class CN_Producto
    {
        private CD_Producto objcd_producto = new CD_Producto();

        public List<Producto> Listar()
        {
            return objcd_producto.ListarProductos();
        }

        public bool Registrar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Nombre == "")
                Mensaje += "El nombre del producto es obligatorio\n";

            if (obj.Precio <= 0)
                Mensaje += "El precio debe ser mayor a 0\n";

            if (obj.Stock < 0)
                Mensaje += "El stock no puede ser negativo\n";

            if (obj.IdCategoria <= 0)
                Mensaje += "Debe seleccionar una categoría\n";

            if (Mensaje != string.Empty)
                return false;

            return objcd_producto.InsertarProducto(obj, out Mensaje);
        }

        public bool Editar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.IdProducto <= 0)
                Mensaje += "Producto inválido\n";

            if (string.IsNullOrWhiteSpace(obj.Nombre))
                Mensaje += "El nombre del producto es obligatorio\n";

            if (obj.Precio <= 0)
                Mensaje += "El precio debe ser mayor a 0\n";

            if (obj.Stock < 0)
                Mensaje += "El stock no puede ser negativo\n";

            if (obj.IdCategoria <= 0)
                Mensaje += "Debe seleccionar una categoría\n";

            if (Mensaje != string.Empty)
                return false;

            return objcd_producto.EditarProducto(obj, out Mensaje);
        }

        public bool Desactivar(int idProducto, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (idProducto <= 0)
            {
                Mensaje = "Producto inválido";
                return false;
            }

            objcd_producto.DesactivarProducto(idProducto);
            return true;
        }

        public bool Eliminar(Producto obj, out string Mensaje)
        {
            return objcd_producto.Eliminar(obj, out Mensaje);
        }
        public Producto BuscarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;

            return Listar().FirstOrDefault(p => p.Codigo.Trim().ToUpper() == codigo.Trim().ToUpper());
        }
    }
}
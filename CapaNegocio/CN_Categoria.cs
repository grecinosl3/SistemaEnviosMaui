using CapaDatos;
using CapaEntidad;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CapaNegocio
{
    public class CN_Categoria
    {

        private CD_Categoria objcd_categoria = new CD_Categoria();

        public List<Categoria> Listar()
        {
            return objcd_categoria.Listar();
        }
        public bool Registrar(Categoria obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Nombre))
            {
                Mensaje += "El nombre de la categoría es obligatorio\n";
            }

            if (obj.TiempoEntrega < 0)
            {
                Mensaje += "El tiempo de entrega no puede ser negativo\n";
            }

            if (Mensaje != string.Empty)
            {
                return false;
            }

            return objcd_categoria.InsertarCategoria(obj, out Mensaje);
        }
        public bool Editar(Categoria obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.IdCategoria <= 0)
            {
                Mensaje += "ID de categoría no válido\n";
            }

            if (string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje += "El nombre de la categoría no puede estar vacío\n";
            }

            if (Mensaje != string.Empty)
            {
                return false;
            }

            return objcd_categoria.EditarCategoria(obj, out Mensaje);
        }
        public bool Eliminar(int id, out string Mensaje)
        {
            return objcd_categoria.Eliminar(id, out Mensaje);
        }
    }
}

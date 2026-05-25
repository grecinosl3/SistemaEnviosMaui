using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapaNegocio
{
    public class CN_Usuario
    {

        private CD_Usuario objCapaDato = new CD_Usuario();
        private CD_Usuario objcd_usuario = new CD_Usuario();

        public List<Usuario> Listar()
        {
            return objCapaDato.ListarUsuarios();
        }

        public Usuario Login(string correo, string contrasena)
        {
            // Validaciones básicas antes de ir a la base de datos
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                return null;
            }
            return objCapaDato.Login(correo, contrasena);
        }

        public int Registrar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.NombreCompleto))
                Mensaje += "Es necesario el nombre del usuario\n";

            if (string.IsNullOrEmpty(obj.Correo))
                Mensaje += "Es necesario el correo del usuario\n";

            if (string.IsNullOrEmpty(obj.Contrasena))
                Mensaje += "Es necesario la contraseña del usuario\n";

            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objCapaDato.Registrar(obj, out Mensaje);
            }
        }

        public bool Editar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.NombreCompleto))
                Mensaje += "Es necesario el nombre del usuario\n";

            if (string.IsNullOrEmpty(obj.Correo))
                Mensaje += "Es necesario el correo del usuario\n";

            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objCapaDato.Editar(obj, out Mensaje);
            }
        }

        public bool Eliminar(Usuario obj, out string Mensaje)
        {
            return objCapaDato.Eliminar(obj.IdUsuario, out Mensaje);
        }



        public bool CambiarRol(int idUsuario, int nuevoIdRol, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (idUsuario <= 0)
                Mensaje += "El ID de usuario seleccionado es inválido.\n";

            if (nuevoIdRol < 1 || nuevoIdRol > 3)
                Mensaje += "El rango seleccionado no existe.\n";

            if (Mensaje != string.Empty)
                return false;

            return objcd_usuario.ActualizarRol(idUsuario, nuevoIdRol, out Mensaje);
        }
    }
}
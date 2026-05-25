using CapaDato;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;

namespace CapaNegocio
{
    public class CN_Cliente
    {
        private CD_Cliente objcd_cliente = new CD_Cliente();

        public List<Cliente> Listar()
        {
            return objcd_cliente.Listar();
        }

        // REGISTRAR EMPRESA CLIENTE
        public int Registrar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            // --- Reglas de Validación para Empresas (B2B) ---
            if (string.IsNullOrWhiteSpace(obj.NombreComercial))
                Mensaje += "El Nombre Comercial de la empresa es obligatorio.\n";

            if (string.IsNullOrWhiteSpace(obj.NIT))
                Mensaje += "El NIT es obligatorio para la facturación.\n";

            if (string.IsNullOrWhiteSpace(obj.NombreContacto))
                Mensaje += "El Nombre del Contacto encargado es obligatorio.\n";

            if (string.IsNullOrWhiteSpace(obj.TelefonoContacto))
                Mensaje += "El Teléfono del contacto es obligatorio.\n";

            if (string.IsNullOrWhiteSpace(obj.DireccionBodega))
                Mensaje += "La Dirección de la Bodega de recolección es obligatoria.\n";

            // Si hay algún error, frenamos el registro de inmediato
            if (Mensaje != string.Empty)
                return 0;

            return objcd_cliente.Registrar(obj, out Mensaje);
        }

        // EDITAR EMPRESA CLIENTE
        public bool Editar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.IdCliente <= 0)
                Mensaje += "Empresa cliente inválida.\n";

            if (string.IsNullOrWhiteSpace(obj.NombreComercial))
                Mensaje += "El Nombre Comercial de la empresa es obligatorio.\n";

            if (string.IsNullOrWhiteSpace(obj.NIT))
                Mensaje += "El NIT es obligatorio.\n";

            if (string.IsNullOrWhiteSpace(obj.NombreContacto))
                Mensaje += "El Nombre del Contacto encargado es obligatorio.\n";

            if (string.IsNullOrWhiteSpace(obj.TelefonoContacto))
                Mensaje += "El Teléfono del contacto es obligatorio.\n";

            if (string.IsNullOrWhiteSpace(obj.DireccionBodega))
                Mensaje += "La Dirección de la Bodega de recolección es obligatoria.\n";

            if (Mensaje != string.Empty)
                return false;

            return objcd_cliente.Editar(obj, out Mensaje);
        }

        // ELIMINAR CLIENTE 
        public bool Eliminar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.IdCliente <= 0)
            {
                Mensaje = "Empresa cliente inválida.";
                return false;
            }

            return objcd_cliente.Eliminar(obj.IdCliente, out Mensaje);
        }

        // NUEVO BUSCADOR: Buscar por NIT 
        public Cliente BuscarPorNIT(string nit)
        {
            if (string.IsNullOrWhiteSpace(nit)) return null;
            return Listar().FirstOrDefault(c => c.NIT.Trim().ToUpper() == nit.Trim().ToUpper());
        }

        // NUEVO BUSCADOR: Buscar por Nombre Comercial
        public Cliente BuscarPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            return Listar().FirstOrDefault(c => c.NombreComercial.Trim().ToUpper().Contains(nombre.Trim().ToUpper()));
        }
    }
}
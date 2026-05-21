using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class CN_Repartidor
    {
        private CD_Repartidor objcd_repartidor = new CD_Repartidor();

        public List<Repartidor> Listar()
        {
            return objcd_repartidor.ListarRepartidores();
        }

        public bool Registrar(Repartidor obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.Nombre))
                Mensaje += "El nombre del repartidor es obligatorio.\n";

            if (string.IsNullOrWhiteSpace(obj.Apellidos))
                Mensaje += "El apellido del repartidor es obligatorio.\n";

            if (string.IsNullOrWhiteSpace(obj.Telefono))
                Mensaje += "El teléfono es obligatorio para contactar al piloto.\n";

            if (string.IsNullOrWhiteSpace(obj.PlacaVehiculo))
                Mensaje += "La placa del vehículo es obligatoria por temas de seguridad vial.\n";

            if (string.IsNullOrWhiteSpace(obj.TipoVehiculo))
                Mensaje += "Debe seleccionar un tipo de vehículo válido.\n";

            if (Mensaje != string.Empty)
                return false;

            int idGenerado = objcd_repartidor.Registrar(obj, out Mensaje);
            if (idGenerado > 0)
            {
                obj.IdRepartidor = idGenerado;
                return true;
            }
            return false;
        }

        public bool Editar(Repartidor obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.IdRepartidor <= 0)
                Mensaje += "ID de repartidor no válido para edición.\n";

            if (string.IsNullOrWhiteSpace(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Apellidos))
                Mensaje += "Nombre y Apellidos son obligatorios.\n";

            if (string.IsNullOrWhiteSpace(obj.PlacaVehiculo))
                Mensaje += "La placa del vehículo no puede guardarse vacía.\n";

            if (Mensaje != string.Empty)
                return false;

            return objcd_repartidor.Editar(obj, out Mensaje);
        }
    }
}
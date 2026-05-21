using System;

namespace CapaEntidad
{
    public class Repartidor
    {
        public int IdRepartidor { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string TipoVehiculo { get; set; }
        public string PlacaVehiculo { get; set; }
        public bool Activo { get; set; }

        // Propiedad auxiliar para mostrar el nombre completo en los selectores de MAUI
        public string NombreCompleto => $"{Nombre} {Apellidos} ({TipoVehiculo})";
    }
        
}
namespace CapaEntidad
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Contrasena { get; set; }

        public Rol oRol { get; set; } // "Administrador", "Moderador", "Repartidor", 

        public bool Activo { get; set; }
        public DateTime FechaCreado { get; set; }


        // Propiedad de ayuda para la interfaz (UX)
        public string EstadoTexto => Activo ? "Activo" : "Inactivo";
    }
}

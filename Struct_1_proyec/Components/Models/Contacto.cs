namespace Struct_1_proyec.Models;

public class Contacto
{
    // Información de contacto
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    // Régimen y Gestión de Tickets
    public string TipoContacto { get; set; } = "Consulta General";

    public string NumeroGuia { get; set; } = string.Empty;

    public string Mensaje { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.Now;
}
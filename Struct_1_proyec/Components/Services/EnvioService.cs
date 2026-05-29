using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CapaEntidad;
using CapaNegocio;

namespace Struct_1_proyec.Services
{
    public class EnvioService
    {
        private readonly CN_Pedido _cnPedido = new CN_Pedido();

        // Ahora el servicio recibe directamente el objeto oficial de tu arquitectura
        public async Task<(bool exito, string mensaje, string numeroGuia)> RegistrarEnvio(Pedido nuevoPedido)
        {
            try
            {
                // Validación de respaldo por si la web no mandó renglones de productos
                if (nuevoPedido.Detalles == null || nuevoPedido.Detalles.Count == 0)
                {
                    nuevoPedido.Detalles = new List<DetallePedido>
                    {
                        new DetallePedido
                        {
                            IdProducto = 1, // Tu ID de flete/paquete genérico en la base de datos
                            Cantidad = 1,
                            PrecioUnitario = nuevoPedido.Total
                        }
                    };
                }

                // Mandamos a guardar a SQL Server usando tu CapaNegocio (exactamente igual que MAUI)
                string mensajeSalida;
                bool resultado = _cnPedido.Registrar(nuevoPedido, out mensajeSalida);

                if (resultado)
                {
                    // Al guardar con éxito, SQL Server genera el IdPedido (Identity autonumérico).
                    // Tu Capa de Datos lo asigna automáticamente de vuelta al objeto, 
                    // así que lo usamos directamente como tu Número de Guía.
                    string guiaGenerada = nuevoPedido.IdPedido.ToString();

                    return (true, "Envío registrado con éxito en el sistema central.", guiaGenerada);
                }
                else
                {
                    return (false, $"Error en reglas de negocio: {mensajeSalida}", string.Empty);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error crítico en el servicio: {ex.Message}", string.Empty);
            }
        }
    }
}
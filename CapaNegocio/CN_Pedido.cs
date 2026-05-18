using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class CN_Pedido
    {
        private CD_Pedido objcd_pedido = new CD_Pedido();

        // 1. LISTAR TODAS LAS GUÍAS
        public List<Pedido> Listar()
        {
            return objcd_pedido.ListarPedidos();
        }

        public List<DetallePedido> ObtenerDetallePedido(int idPedido)
        {
            if (idPedido <= 0)
                return new List<DetallePedido>();

            // Conecta con la Capa de Datos para traer las filas con sus respectivos oProducto llenos
            return objcd_pedido.ObtenerDetallePedido(idPedido);
        }

        // 2. CREAR / REGISTRAR NUEVA GUÍA DE ENVÍO
        public bool Registrar(Pedido pedido, out string Mensaje)
        {
            Mensaje = string.Empty;

            // --- Reglas de Validación para Guías Logísticas ---
            if (pedido.IdCliente <= 0)
                Mensaje += "Debe seleccionar una empresa cliente válida (Remitente).\n";

            if (string.IsNullOrWhiteSpace(pedido.NombreDestinatario))
                Mensaje += "El nombre del destinatario que recibe es obligatorio.\n";

            if (string.IsNullOrWhiteSpace(pedido.TelefonoDestinatario))
                Mensaje += "El teléfono de quien recibe es obligatorio para coordinar la entrega.\n";

            if (string.IsNullOrWhiteSpace(pedido.DireccionEntrega))
                Mensaje += "La dirección de entrega exacta es obligatoria.\n";

            if (string.IsNullOrWhiteSpace(pedido.MetodoPago))
                Mensaje += "Debe especificar el método de pago (Ej: COD, Pagado, etc.).\n";

            if (pedido.CostoFlete < 0)
                Mensaje += "El costo del flete de envío no puede ser negativo.\n";

            if (pedido.MontoCOD < 0)
                Mensaje += "El monto a cobrar contra entrega (COD) no puede ser negativo.\n";

            if (Mensaje != string.Empty)
                return false;

            pedido.FechaPedido = DateTime.Now;

            // Toda guía arranca en estado 'Registrado' por defecto en el sistema central
            if (string.IsNullOrEmpty(pedido.Estado))
                pedido.Estado = "Registrado";

            // El total de la operación logística es el Flete + el valor del paquete si es contra entrega
            pedido.Total = pedido.CostoFlete + pedido.MontoCOD;

            // Aseguramos que la lista de detalles no vaya nula para que no rompa la transacción
            if (pedido.Detalles == null)
                pedido.Detalles = new List<DetallePedido>();

            // Mandamos los datos limpios a la transacción en CapaDatos
            return objcd_pedido.InsertarPedido(pedido, out Mensaje);
        }

        // 3. CAMBIAR EL ESTADO DE UNA GUÍA (Flujo de ruta)
        public bool CambiarEstado(int idPedido, string estado, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (idPedido <= 0)
            {
                Mensaje = "El número de guía (IdPedido) es inválido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(estado))
            {
                Mensaje = "El estado de envío no puede ir vacío.";
                return false;
            }

            // Lista de estados permitidos para el control interno de mensajería
            var estadosValidos = new List<string> { "Registrado", "En Bodega", "En Ruta", "Entregado", "Devuelto", "Cancelado" };
            if (!estadosValidos.Contains(estado))
            {
                Mensaje = $"El estado '{estado}' no es un estado logístico válido.";
                return false;
            }

            return objcd_pedido.CambiarEstado(idPedido, estado, out Mensaje);
        }

        // 4. ASIGNAR REPARTIDOR / MOTORISTA A UNA GUÍA
        public bool AsignarRepartidor(int idPedido, int idRepartidor, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (idPedido <= 0)
                Mensaje += "Guía de envío inválida.\n";

            if (idRepartidor <= 0)
                Mensaje += "Debe seleccionar un repartidor/motorista válido.\n";

            if (Mensaje != string.Empty)
                return false;

            return objcd_pedido.AsignarRepartidor(idPedido, idRepartidor, out Mensaje);
        }
    }
}
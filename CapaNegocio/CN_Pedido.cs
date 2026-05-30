using CapaDatos;
using CapaEntidad;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

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

            return objcd_pedido.ObtenerDetallePedido(idPedido);
        }

        // 2. CREAR / REGISTRAR NUEVA GUÍA DE ENVÍO
        public bool Registrar(Pedido pedido, out string Mensaje)
        {
            Mensaje = string.Empty;

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

            if (string.IsNullOrEmpty(pedido.Estado))
                pedido.Estado = "Registrado";

            pedido.Total = pedido.CostoFlete + pedido.MontoCOD;

            if (pedido.Detalles == null)
                pedido.Detalles = new List<DetallePedido>();

            return objcd_pedido.InsertarPedido(pedido, out Mensaje);
        }

        // 3. CAMBIAR EL ESTADO DE UNA GUÍA 
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

            var estadosValidos = new List<string> { "Registrado", "En Bodega", "En Ruta", "Entregado", "Devuelto", "Cancelado" };
            if (!estadosValidos.Contains(estado))
            {
                Mensaje = $"El estado '{estado}' no es un estado logístico válido.";
                return false;
            }

            return objcd_pedido.CambiarEstado(idPedido, estado, out Mensaje);
        }

        // 4. ASIGNAR REPARTIDOR 
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

        //  PUENTE PARA TRAER LOS PENDIENTES
        public List<Pedido> ListarPendientes()
        {
            return objcd_pedido.ListarPedidosPendientes();
        }

        // VALIDACIÓN LOGÍSTICA PARA LA ASIGNACIÓN
        public bool DespacharARuta(int idPedido, int idRepartidor, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (idPedido <= 0)
                Mensaje += "Número de guía o pedido inválido.\n";

            if (idRepartidor <= 0)
                Mensaje += "Debe seleccionar obligatoriamente un piloto para la entrega.\n";

            if (Mensaje != string.Empty)
                return false;

            // Pasamos idRepartidor al nuevo parámetro idPiloto de la Capa de Datos
            return objcd_pedido.AsignarPilotoEnBD(idPedido, idRepartidor, out Mensaje);
        }

        //  ENLACE PARA TRAER LA CARGA DE COBROS DEL PILOTO
        public List<Pedido> ListarPedidosParaLiquidar(int idRepartidor)
        {
            if (idRepartidor <= 0)
                return new List<Pedido>();

            return objcd_pedido.ObtenerPedidosParaLiquidar(idRepartidor);
        }

        //  VALIDACIÓN PARA EFECTUAR EL CIERRE DE CAJA
        public bool LiquidarCajaPiloto(int idRepartidor, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (idRepartidor <= 0)
            {
                Mensaje = "Debe seleccionar un piloto válido para realizar la liquidación.";
                return false;
            }

            return objcd_pedido.LiquidarPedidosPiloto(idRepartidor, out Mensaje);
        }

        public DashboardMetrics GetDashboard()
        {
            return objcd_pedido.ObtenerMetricasDashboard();
        }
        public List<Repartidor> ListarRepartidores()
        {
            List<Repartidor> lista = new List<Repartidor>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    // 1. OJO: Revisa que tu consulta SQL incluya la columna IdUsuario
                    string query = "SELECT IdRepartidor, IdUsuario, Nombre, Apellidos, Telefono, TipoVehiculo, PlacaVehiculo, Activo FROM Repartidores WHERE Activo = 1";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Repartidor()
                            {
                                IdRepartidor = Convert.ToInt32(dr["IdRepartidor"]),

                                // 2. ¡ESTA LÍNEA ES LA QUE TE FALTA! Mapea el valor al objeto de C#
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),

                                Nombre = dr["Nombre"].ToString(),
                                Apellidos = dr["Apellidos"].ToString(),
                                Telefono = dr["Telefono"].ToString(),
                                TipoVehiculo = dr["TipoVehiculo"].ToString(),
                                PlacaVehiculo = dr["PlacaVehiculo"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<Repartidor>();
                }
            }
            return lista;
        }
    }
}
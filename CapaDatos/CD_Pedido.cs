using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;

namespace CapaDatos
{
    public class CD_Pedido
    {
        // 1. OBTENER CORRELATIVO PARA EL NÚMERO DE GUÍA
        public int ObtenerCorrelativo()
        {
            int idcorrelativo = 0;
            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    string query = "SELECT ISNULL(MAX(IdPedido), 0) + 1 FROM Pedidos";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
                    idcorrelativo = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception)
                {
                    idcorrelativo = 1; // Si está vacía, arranca en 1
                }
            }
            return idcorrelativo;
        }

        // 2. REGISTRAR UNA GUÍA DE ENVÍO DIRECTA (CON TRANSACCIÓN EN C#)
        public bool InsertarPedido(Pedido pedido, out string Mensaje)
        {
            Mensaje = string.Empty;
            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // Query adaptado a la nueva estructura de la tabla Pedidos B2B
                    string queryPedido = @"INSERT INTO Pedidos
                        (IdCliente, IdUsuario, FechaPedido, FechaEntrega, NombreDestinatario, TelefonoDestinatario, DireccionEntrega, Estado, MetodoPago, MontoCOD, CostoFlete, Total, Notas)
                        VALUES
                        (@IdCliente, @IdUsuario, @FechaPedido, @FechaEntrega, @NombreDestinatario, @TelefonoDestinatario, @DireccionEntrega, @Estado, @MetodoPago, @MontoCOD, @CostoFlete, @Total, @Notas);
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdPedido = new SqlCommand(queryPedido, con, trans);

                    cmdPedido.Parameters.AddWithValue("@IdCliente", pedido.IdCliente);
                    cmdPedido.Parameters.AddWithValue("@IdUsuario", (object)pedido.IdRepartidor ?? DBNull.Value); // Mapeado a IdUsuario (Repartidor)
                    cmdPedido.Parameters.AddWithValue("@FechaPedido", pedido.FechaPedido);
                    cmdPedido.Parameters.AddWithValue("@FechaEntrega", (object)pedido.FechaEntrega ?? DBNull.Value);
                    cmdPedido.Parameters.AddWithValue("@NombreDestinatario", pedido.NombreDestinatario);
                    cmdPedido.Parameters.AddWithValue("@TelefonoDestinatario", pedido.TelefonoDestinatario);
                    cmdPedido.Parameters.AddWithValue("@DireccionEntrega", pedido.DireccionEntrega);
                    cmdPedido.Parameters.AddWithValue("@Estado", pedido.Estado);
                    cmdPedido.Parameters.AddWithValue("@MetodoPago", pedido.MetodoPago);
                    cmdPedido.Parameters.AddWithValue("@MontoCOD", pedido.MontoCOD);
                    cmdPedido.Parameters.AddWithValue("@CostoFlete", pedido.CostoFlete);
                    cmdPedido.Parameters.AddWithValue("@Total", pedido.Total);
                    cmdPedido.Parameters.AddWithValue("@Notas", (object)pedido.Notas ?? DBNull.Value);

                    int idPedidoGenerado = Convert.ToInt32(cmdPedido.ExecuteScalar());

                    // Si tienes una tabla DetallePedido adaptada, aquí insertaría sus fletes aplicados
                    foreach (var item in pedido.Detalles)
                    {
                        string queryDetalle = @"INSERT INTO DetallePedido
                            (IdPedido, IdProducto, Cantidad, PrecioUnitario, Subtotal)
                            VALUES
                            (@IdPedido, @IdProducto, @Cantidad, @PrecioUnitario, @Subtotal)";

                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, con, trans);
                        cmdDetalle.Parameters.AddWithValue("@IdPedido", idPedidoGenerado);
                        cmdDetalle.Parameters.AddWithValue("@IdProducto", item.IdProducto);
                        cmdDetalle.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                        cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", item.PrecioUnitario);
                        cmdDetalle.Parameters.AddWithValue("@Subtotal", item.Subtotal);

                        cmdDetalle.ExecuteNonQuery();
                    }

                    trans.Commit();
                    Mensaje = idPedidoGenerado.ToString(); // Retornamos el número de guía generado
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Mensaje = ex.Message;
                    return false;
                }
            }
        }

        // 3. LISTAR TODAS LAS GUÍAS EN EL SISTEMA
        public List<Pedido> ListarPedidos()
        {
            List<Pedido> lista = new List<Pedido>();

            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    // Traemos el pedido amarrado al Nombre Comercial de la Empresa dueña
                    string query = @"SELECT p.IdPedido, p.IdCliente, p.IdUsuario,
                                            p.FechaPedido, p.FechaEntrega, p.NombreDestinatario, p.TelefonoDestinatario,
                                            p.DireccionEntrega, p.Estado, p.MetodoPago, p.MontoCOD, p.CostoFlete, p.Total, p.Notas,
                                            c.NombreComercial, c.NIT
                                     FROM Pedidos p
                                     INNER JOIN Clientes c ON c.IdCliente = p.IdCliente";

                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Pedido
                            {
                                IdPedido = Convert.ToInt32(dr["IdPedido"]),
                                IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                IdRepartidor = dr["IdUsuario"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["IdUsuario"]),
                                FechaPedido = Convert.ToDateTime(dr["FechaPedido"]),
                                FechaEntrega = dr["FechaEntrega"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaEntrega"]),
                                NombreDestinatario = dr["NombreDestinatario"].ToString(),
                                TelefonoDestinatario = dr["TelefonoDestinatario"].ToString(),
                                DireccionEntrega = dr["DireccionEntrega"].ToString(),
                                Estado = dr["Estado"].ToString(),
                                MetodoPago = dr["MetodoPago"].ToString(),
                                MontoCOD = Convert.ToDecimal(dr["MontoCOD"]),
                                CostoFlete = Convert.ToDecimal(dr["CostoFlete"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                Notas = dr["Notas"] != DBNull.Value ? dr["Notas"].ToString() : string.Empty,
                                oCliente = new Cliente
                                {
                                    IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                    NombreComercial = dr["NombreComercial"].ToString(),
                                    NIT = dr["NIT"].ToString()
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR EN LISTAR PEDIDOS: " + ex.Message);
                    lista = new List<Pedido>();
                }
            }
            return lista;
        }

        // 4. CAMBIAR ESTADO DE LA GUÍA (Para el flujo: Registrado -> En Bodega -> En Ruta -> Entregado)
        public bool CambiarEstado(int idPedido, string estado, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    string query = "UPDATE Pedidos SET Estado = @Estado";

                    // Si pasa a Entregado, sellamos de una vez la FechaEntrega con la hora del sistema
                    if (estado == "Entregado")
                        query += ", FechaEntrega = GETDATE()";

                    query += " WHERE IdPedido = @IdPedido";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Estado", estado);
                    cmd.Parameters.AddWithValue("@IdPedido", idPedido);

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    resultado = false;
                    Mensaje = ex.Message;
                }
            }
            return resultado;
        }

        // 5. ASIGNAR UN REPARTIDOR / MOTORISTA A UNA GUÍA
        public bool AsignarRepartidor(int idPedido, int idRepartidor, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    string query = "UPDATE Pedidos SET IdUsuario = @IdRepartidor, Estado = 'En Ruta' WHERE IdPedido = @IdPedido";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdRepartidor", idRepartidor);
                    cmd.Parameters.AddWithValue("@IdPedido", idPedido);

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    resultado = false;
                    Mensaje = ex.Message;
                }
            }
            return resultado;
        }

        public List<DetallePedido> ObtenerDetallePedido(int idPedido)
        {
            List<DetallePedido> lista = new List<DetallePedido>();

            // Usamos tu Conexion nativa. Cambia "Conexion.cn" por el nombre exacto de tu cadena si varía.
            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    // Consulta limpia que amarra el detalle con los datos de tu entidad Producto
                    string query = @"
                        SELECT d.IdDetalle, d.IdPedido, d.IdProducto, d.Cantidad, d.PrecioUnitario,
                               p.Nombre, p.Codigo, p.Descripcion
                        FROM DETALLE_PEDIDO d
                        INNER JOIN PRODUCTO p ON d.IdProducto = p.IdProducto
                        WHERE d.IdPedido = @idpedido";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@idpedido", idPedido);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new DetallePedido()
                            {
                                IdDetalle = Convert.ToInt32(dr["IdDetalle"]),
                                IdPedido = Convert.ToInt32(dr["IdPedido"]),
                                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"]),
                                // Llenamos el subobjeto Producto para que tu XAML en MAUI no tire nulos
                                oProducto = new Producto()
                                {
                                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                    Nombre = dr["Nombre"].ToString()
                                }
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    lista = new List<DetallePedido>(); // En caso de fallo devolvemos la lista vacía de seguridad
                }
            }
            return lista;
        }

        //  MÉTODO 1: FILTRAR PEDIDOS PENDIENTES DE RUTA
        public List<Pedido> ListarPedidosPendientes()
        {
            List<Pedido> lista = new List<Pedido>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    // El query correcto que habla el mismo idioma de tu base de datos
                    string query = @"
                SELECT IdPedido, NombreDestinatario, DireccionEntrega, Total, FechaPedido
                FROM Pedidos
                WHERE (Estado = 'Registrado' OR Estado = 'REGISTRADO') 
                AND (IdUsuario IS NULL OR IdUsuario = 0)";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Pedido()
                            {
                                IdPedido = Convert.ToInt32(dr["IdPedido"]),
                                NombreDestinatario = dr["NombreDestinatario"].ToString(),
                                DireccionEntrega = dr["DireccionEntrega"].ToString(),
                                Total = Convert.ToDecimal(dr["Total"]),
                                FechaPedido = Convert.ToDateTime(dr["FechaPedido"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR DESPACHO LEER: " + ex.Message);
                    lista = new List<Pedido>();
                }
            }
            return lista;
        }

        //  MÉTODO 2: ACTUALIZAR EL CHOFER Y PASAR A "EN RUTA"
        // 🛠️ Nombre corregido para que tu CN_Pedido lo encuentre a la perfección
        public bool AsignarPilotoEnBD(int idPedido, int idUsuario, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    // El query que apunta a tu tabla real 'Pedidos' y tu columna 'IdUsuario'
                    string query = @"
                UPDATE Pedidos 
                SET IdUsuario = @idusuario, 
                    Estado = 'En Ruta',
                    FechaEntrega = GETDATE()
                WHERE IdPedido = @idpedido";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@idusuario", idUsuario);
                    cmd.Parameters.AddWithValue("@idpedido", idPedido);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        respuesta = true;
                    }
                    else
                    {
                        Mensaje = "No se pudo asignar el pedido o el pedido ya no existe.";
                    }
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Mensaje = ex.Message;
                }
            }
            return respuesta;
        }


        // 1. OBTENER ENVIOS ENTREGADOS POR REPARTIDOR PARA COBRO COD
        public List<Pedido> ObtenerPedidosParaLiquidar(int idRepartidor)
        {
            List<Pedido> lista = new List<Pedido>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    // 🛠️ CORRECCIÓN DE COLUMNA Y TABLA: Cambiado a p.IdUsuario y 'Pedidos'
                    string query = @"
                SELECT p.IdPedido, p.NombreDestinatario, p.DireccionEntrega, p.MontoCOD, p.CostoFlete, p.Total, p.FechaPedido
                FROM Pedidos p
                WHERE p.IdUsuario = @idrepartidor 
                AND p.MetodoPago = 'COD' 
                AND p.Estado = 'Entregado'";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@idrepartidor", idRepartidor);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Pedido()
                            {
                                IdPedido = Convert.ToInt32(dr["IdPedido"]),
                                NombreDestinatario = dr["NombreDestinatario"].ToString(),
                                DireccionEntrega = dr["DireccionEntrega"].ToString(),
                                MontoCOD = Convert.ToDecimal(dr["MontoCOD"]),
                                CostoFlete = Convert.ToDecimal(dr["CostoFlete"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                FechaPedido = Convert.ToDateTime(dr["FechaPedido"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Imprime el error real en la consola de salida de Visual Studio por si acaso
                    System.Diagnostics.Debug.WriteLine("CRITICAL SQL ERROR: " + ex.Message);
                    lista = new List<Pedido>();
                }
            }
            return lista;
        }

        // 2. PROCESAR LA LIQUIDACIÓN COBRADA (Cerrar el ciclo del dinero)
        public bool LiquidarPedidosPiloto(int idRepartidor, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    // 🛠️ CORRECCIÓN AQUÍ TAMBIÉN: Cambiado a IdUsuario y 'Pedidos'
                    string query = @"
                UPDATE Pedidos 
                SET Estado = 'Liquidado' 
                WHERE IdUsuario = @idrepartidor 
                AND MetodoPago = 'COD' 
                AND Estado = 'Entregado'";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@idrepartidor", idRepartidor);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        respuesta = true;
                    }
                    else
                    {
                        Mensaje = "No se encontraron pedidos pendientes de liquidar para este piloto.";
                    }
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Mensaje = ex.Message;
                }
            }
            return respuesta;
        }




        public DashboardMetrics ObtenerMetricasDashboard()
        {
            DashboardMetrics metrics = new DashboardMetrics();

            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    // Consulta consolidada para traer todo de un solo golpe por rendimiento
                    string query = @"
                SELECT 
                    COUNT(CASE WHEN Estado = 'Registrado' THEN 1 END) as Pendientes,
                    COUNT(CASE WHEN Estado = 'En Ruta' THEN 1 END) as EnRuta,
                    COUNT(CASE WHEN Estado = 'Liquidado' THEN 1 END) as Liquidados,
                    ISNULL(SUM(CASE WHEN Estado = 'Liquidado' AND MetodoPago = 'COD' THEN Total END), 0) as TotalMes
                FROM PEDIDO";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            metrics.PaquetesPendientes = Convert.ToInt32(dr["Pendientes"]);
                            metrics.PaquetesEnRuta = Convert.ToInt32(dr["EnRuta"]);
                            metrics.PaquetesLiquidados = Convert.ToInt32(dr["Liquidados"]);
                            metrics.TotalEfectivoMes = Convert.ToDecimal(dr["TotalMes"]);
                        }
                    }
                }
                catch (Exception)
                {
                    metrics = new DashboardMetrics(); // Si falla, devuelve todo en 0
                }
            }
            return metrics;
        }


    }
}
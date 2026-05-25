using System;
using System.Data;
using Microsoft.Data.SqlClient; 

namespace CapaDatos
{
    public class CD_Dashboard
    {
        public Tuple<int, int, int, decimal> ObtenerMetricasDashboard()
        {
            int pendientes = 0;
            int enRuta = 0;
            int entregados = 0;
            decimal totalEfectivo = 0;

            string query = @"
                SELECT 
                    COUNT(CASE WHEN Estado = 'Registrado' OR Estado = 'En Bodega' THEN 1 END) as Pendientes,
                    COUNT(CASE WHEN Estado = 'En Ruta' THEN 1 END) as EnRuta,
                    COUNT(CASE WHEN Estado = 'Entregado' OR Estado = 'Liquidado' THEN 1 END) as Entregados,
                    ISNULL(SUM(CASE WHEN Estado = 'Entregado' OR Estado = 'Liquidado' THEN Total ELSE 0 END), 0) as TotalEfectivo
                FROM Pedidos;";

            try
            {
                
                using (SqlConnection conexion = new SqlConnection(Conexion.Cadena))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.CommandType = CommandType.Text;
                        conexion.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                pendientes = Convert.ToInt32(dr["Pendientes"]);
                                enRuta = Convert.ToInt32(dr["EnRuta"]);
                                entregados = Convert.ToInt32(dr["Entregados"]);
                                totalEfectivo = Convert.ToDecimal(dr["TotalEfectivo"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en CD_Dashboard: " + ex.Message);
            }

            return new Tuple<int, int, int, decimal>(pendientes, enRuta, entregados, totalEfectivo);
        }
    }
}
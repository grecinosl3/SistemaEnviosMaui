using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Repartidor
    {
        // 1. LISTAR REPARTIDORES
        public List<Repartidor> ListarRepartidores()
        {
            List<Repartidor> lista = new List<Repartidor>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    string query = "SELECT IdRepartidor, Nombre, Apellidos, Telefono, TipoVehiculo, PlacaVehiculo, Activo FROM REPARTIDOR";
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
                catch (Exception)
                {
                    lista = new List<Repartidor>();
                }
            }
            return lista;
        }

        // 2. REGISTRAR REPARTIDOR
        public int Registrar(Repartidor obj, out string Mensaje)
        {
            int idRepartidorGenerado = 0;
            Mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    string query = @"INSERT INTO REPARTIDOR (Nombre, Apellidos, Telefono, TipoVehiculo, PlacaVehiculo, Activo) 
                                     VALUES (@nombre, @apellidos, @telefono, @tipovehiculo, @placavehiculo, @activo);
                                     SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@apellidos", obj.Apellidos);
                    cmd.Parameters.AddWithValue("@telefono", obj.Telefono);
                    cmd.Parameters.AddWithValue("@tipovehiculo", obj.TipoVehiculo);
                    cmd.Parameters.AddWithValue("@placavehiculo", obj.PlacaVehiculo);
                    cmd.Parameters.AddWithValue("@activo", obj.Activo);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
                    idRepartidorGenerado = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    idRepartidorGenerado = 0;
                    Mensaje = ex.Message;
                }
            }
            return idRepartidorGenerado;
        }

        // 3. MODIFICAR REPARTIDOR
        public bool Editar(Repartidor obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    string query = @"UPDATE REPARTIDOR SET 
                                     Nombre = @nombre, Apellidos = @apellidos, Telefono = @telefono, 
                                     TipoVehiculo = @tipovehiculo, PlacaVehiculo = @placavehiculo, Activo = @activo 
                                     WHERE IdRepartidor = @idrepartidor";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@idrepartidor", obj.IdRepartidor);
                    cmd.Parameters.AddWithValue("@nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@apellidos", obj.Apellidos);
                    cmd.Parameters.AddWithValue("@telefono", obj.Telefono);
                    cmd.Parameters.AddWithValue("@tipovehiculo", obj.TipoVehiculo);
                    cmd.Parameters.AddWithValue("@placavehiculo", obj.PlacaVehiculo);
                    cmd.Parameters.AddWithValue("@activo", obj.Activo);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
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
    }
}
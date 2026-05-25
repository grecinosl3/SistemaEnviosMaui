using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CapaEntidad;
using System.Linq; 

namespace CapaDatos
{
    public class CD_Chat
    {
        private string cadenaConexion = Conexion.Cadena;

        public bool RegistrarMensaje(ChatMensaje obj, out string MensajeError)
        {
            bool respuesta = false;
            MensajeError = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarMensaje", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    string salaStr = obj.IdConversacion.ToString();

                    int idUsuarioUno = 0;
                    int idUsuarioDos = 0;

                    if (salaStr.Length >= 2)
                    {
                        idUsuarioUno = obj.IdRemitente;

                        string idUnoStr = salaStr.Substring(0, 1);
                        string idDosStr = salaStr.Substring(1);

                        int u1 = int.Parse(idUnoStr);
                        int u2 = int.Parse(idDosStr);

                        idUsuarioUno = Math.Min(u1, u2);
                        idUsuarioDos = Math.Max(u1, u2);
                    }
                    else
                    {
                        idUsuarioUno = obj.IdRemitente;
                        idUsuarioDos = obj.IdRemitente;
                    }

                    //  Mapeamos los parámetros clásicos
                    cmd.Parameters.AddWithValue("@IdConversacion", obj.IdConversacion);
                    cmd.Parameters.AddWithValue("@IdRemitente", obj.IdRemitente);
                    cmd.Parameters.AddWithValue("@Mensaje", obj.Mensaje);
                    cmd.Parameters.AddWithValue("@FechaEnvio", obj.FechaEnvio != DateTime.MinValue ? obj.FechaEnvio : DateTime.Now);
                    cmd.Parameters.AddWithValue("@IdUsuarioUno", idUsuarioUno);
                    cmd.Parameters.AddWithValue("@IdUsuarioDos", idUsuarioDos);

                    oconexion.Open();

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null && resultado != DBNull.Value)
                    {
                        respuesta = true;
                    }
                    else
                    {
                        respuesta = true;
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                MensajeError = ex.Message;
            }

            return respuesta;
        }

        public List<ChatMensaje> ListarMensajesPorConversacion(int idConversacion)
        {
            List<ChatMensaje> lista = new List<ChatMensaje>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    string query = "SELECT IdConversacion, IdRemitente, Mensaje, FechaEnvio FROM Chat_Mensajes WHERE IdConversacion = @id ORDER BY FechaEnvio ASC";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@id", idConversacion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ChatMensaje()
                            {
                                IdConversacion = Convert.ToInt32(dr["IdConversacion"]),
                                IdRemitente = Convert.ToInt32(dr["IdRemitente"]),
                                Mensaje = dr["Mensaje"].ToString(),
                                FechaEnvio = Convert.ToDateTime(dr["FechaEnvio"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<ChatMensaje>();
                    Console.WriteLine($" Error en CD_Chat (Listar): {ex.Message}");
                }
            }
            return lista;
        }
    }
}
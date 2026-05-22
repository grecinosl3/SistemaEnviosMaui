using System;
using System.Data;
using Microsoft.Data.SqlClient; 
using CapaEntidad;

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
                    // Nos conectamos directo a tu nuevo procedimiento almacenado
                    SqlCommand cmd = new SqlCommand("sp_RegistrarMensaje", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Mapeamos los parámetros exactos que pusimos en SQL Server
                    cmd.Parameters.AddWithValue("@IdConversacion", obj.IdConversacion);
                    cmd.Parameters.AddWithValue("@IdRemitente", obj.IdRemitente);
                    cmd.Parameters.AddWithValue("@Mensaje", obj.Mensaje);
                    cmd.Parameters.AddWithValue("@FechaEnvio", obj.FechaEnvio != DateTime.MinValue ? obj.FechaEnvio : DateTime.Now);

                    oconexion.Open();

                    // Ejecutamos y si devuelve filas afectadas, todo salió bien
                    if (cmd.ExecuteNonQuery() > 0)
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

            // Reemplaza "CadenaConexion" por la variable real que uses en tu proyecto para conectarte a SQL
            using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
            {
                try
                {
                    // Consulta directa para traer los mensajes ordenados del más viejo al más nuevo
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
                    lista = new List<ChatMensaje>(); // Si falla, devuelve la lista vacía para no romper la app
                    Console.WriteLine($" Error en CD_Chat (Listar): {ex.Message}");
                }
            }
            return lista;
        }
    }
}
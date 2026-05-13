using CapaEntidad;
using Microsoft.Data.SqlClient; 
using System;
using System.Collections.Generic;
using System.Data;

namespace CapaDatos
{
    public class CD_Usuario
    {
        public List<Usuario> ListarUsuarios()
        {
            List<Usuario> lista = new List<Usuario>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    
                    string query = @"SELECT u.IdUsuario, 
                                     (u.Nombre + ' ' + u.Apellido) as NombreCompleto, 
                                     u.Correo, 
                                     ISNULL(u.Telefono,'') as Telefono, 
                                     u.IdRol, u.Activo, 
                                     ISNULL(r.NombreRol, 'Sin Rol') as NombreRol
                                     FROM Usuarios u 
                                     LEFT JOIN Rol r ON u.IdRol = r.IdRol";

                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                NombreCompleto = dr["NombreCompleto"].ToString(),
                                Correo = dr["Correo"].ToString(),
                                Telefono = dr["Telefono"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                oRol = new Rol()
                                {
                                    IdRol = Convert.ToInt32(dr["IdRol"]),
                                    NombreRol = dr["NombreRol"].ToString()
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               
                string error = ex.Message;
                lista = new List<Usuario>();
            }

            return lista;
        }

        public Usuario Login(string correo, string contrasena)
        {
            Usuario usuario = null;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    // También concatenamos aquí
                    string query = @"SELECT u.IdUsuario, (u.Nombre + ' ' + u.Apellido) as NombreCompleto, 
                                     u.Correo, u.Telefono, u.Activo,
                                     u.IdRol, r.NombreRol 
                                     FROM Usuarios u
                                     INNER JOIN Rol r ON u.IdRol = r.IdRol
                                     WHERE u.Correo = @correo 
                                     AND u.Contrasena = @contrasena
                                     AND u.Activo = 1";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                NombreCompleto = dr["NombreCompleto"].ToString(),
                                Correo = dr["Correo"].ToString(),
                                Telefono = dr["Telefono"] != DBNull.Value ? dr["Telefono"].ToString() : "",
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                oRol = new Rol()
                                {
                                    IdRol = Convert.ToInt32(dr["IdRol"]),
                                    NombreRol = dr["NombreRol"].ToString()
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                usuario = null;
            }

            return usuario;
        }


        public int Registrar(Usuario obj, out string Mensaje)
        {
            int idUsuarioGenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_REGISTRARUSUARIO", oconexion);

                    // Enviamos el NombreCompleto (el SP se encarga de dividirlo en Nombre y Apellido)
                    cmd.Parameters.AddWithValue("NombreCompleto", obj.NombreCompleto);
                    cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Telefono", obj.Telefono);
                    cmd.Parameters.AddWithValue("Contrasena", obj.Contrasena);
                    cmd.Parameters.AddWithValue("IdRol", obj.oRol.IdRol);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);

                    // Parámetros de salida
                    cmd.Parameters.Add("IdUsuarioResultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    idUsuarioGenerado = Convert.ToInt32(cmd.Parameters["IdUsuarioResultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idUsuarioGenerado = 0;
                Mensaje = ex.Message;
            }
            return idUsuarioGenerado;
        }

        public bool Editar(Usuario obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_EDITARUSUARIO", oconexion);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("NombreCompleto", obj.NombreCompleto);
                    cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Telefono", obj.Telefono);
                    cmd.Parameters.AddWithValue("Contrasena", obj.Contrasena);
                    cmd.Parameters.AddWithValue("IdRol", obj.oRol.IdRol);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);

                    cmd.Parameters.Add("Respuesta", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Respuesta"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;
            }
            return respuesta;
        }

        public bool Eliminar(int idUsuario, out string mensaje)
        {
            bool respuesta = false;
            mensaje = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Usuarios WHERE IdUsuario = @id", con);
                    cmd.Parameters.AddWithValue("@id", idUsuario);
                    con.Open();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        respuesta = true;
                    }
                    else
                    {
                        mensaje = "No se pudo eliminar el usuario.";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                mensaje = ex.Message;
            }
            return respuesta;
        }
    }
}
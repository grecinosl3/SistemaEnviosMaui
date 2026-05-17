using CapaEntidad;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CapaDatos
{
    public class CD_Producto
    {
        public List<Producto> ListarProductos()
        {
            List<Producto> lista = new List<Producto>();


            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_LISTARPRODUCTOS", con);
                    cmd.CommandType = CommandType.StoredProcedure; 
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {

                                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                // Asignamos el ID al código para que se "pinte" en la lista
                                Codigo = dr["IdProducto"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                Stock = Convert.ToInt32(dr["Stock"]),
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                // Usamos la propiedad de objeto para la relación
                                oCategoria = new Categoria
                                {
                                    IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                                    Nombre = dr["NombreCategoria"].ToString()
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Producto>(); // Si hay error, devuelve lista vacía
            }

            return lista;
        }

        public bool InsertarProducto(Producto obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_RegistrarProducto", con);
                    cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("@Precio", obj.Precio);
                    cmd.Parameters.AddWithValue("@Stock", obj.Stock);
                    cmd.Parameters.AddWithValue("@IdCategoria", obj.oCategoria.IdCategoria); 

                    con.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            { 
                respuesta = false; 
                Mensaje = ex.Message; 
            }
            return respuesta;
        }


        public bool EditarProducto(Producto obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                  
                    SqlCommand cmd = new SqlCommand("SP_EDITARPRODUCTO", con);

                    cmd.Parameters.AddWithValue("@IdProducto", obj.IdProducto);
                    cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("@Precio", obj.Precio);
                    cmd.Parameters.AddWithValue("@Stock", obj.Stock);
                    cmd.Parameters.AddWithValue("@IdCategoria", obj.oCategoria.IdCategoria);
                    cmd.Parameters.AddWithValue("@Activo", obj.Activo);
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        respuesta = true;
                    }
                    else
                    {
                        Mensaje = "No se encontró el producto para editar.";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;
            }

            return respuesta;
        }

        public void DesactivarProducto(int idProducto)
        {
            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                string query = "UPDATE Productos SET Activo = 0 WHERE IdProducto = @IdProducto";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool Eliminar(Producto obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.Cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_EliminarProducto", oconexion);
                    cmd.Parameters.AddWithValue("IdProducto", obj.IdProducto);
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
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
    }
}

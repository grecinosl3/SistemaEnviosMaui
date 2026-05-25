using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CapaDato
{
    public class CD_Cliente
    {
        // LISTAR EMPRESAS CLIENTES
        public List<Cliente> Listar()
        {
            List<Cliente> lista = new List<Cliente>();

            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarClientes", con);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Cliente
                            {
                                IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                NombreComercial = dr["NombreComercial"].ToString(),
                                RazonSocial = dr["RazonSocial"] != DBNull.Value ? dr["RazonSocial"].ToString() : string.Empty,
                                NIT = dr["NIT"].ToString(),
                                NombreContacto = dr["NombreContacto"].ToString(),
                                TelefonoContacto = dr["TelefonoContacto"].ToString(),
                                CorreoContacto = dr["CorreoContacto"] != DBNull.Value ? dr["CorreoContacto"].ToString() : string.Empty,
                                DireccionBodega = dr["DireccionBodega"].ToString(),
                                CuentaBancaria = dr["CuentaBancaria"] != DBNull.Value ? dr["CuentaBancaria"].ToString() : string.Empty,
                                Banco = dr["Banco"] != DBNull.Value ? dr["Banco"].ToString() : string.Empty,
                                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                                Activo = Convert.ToBoolean(dr["Activo"])
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    lista = new List<Cliente>(); 
                }
            }
            return lista;
        }

        //  REGISTRAR EMPRESA CLIENTE
        public int Registrar(Cliente obj, out string Mensaje)
        {
            int idGenerado = 0;
            Mensaje = string.Empty;

            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrarCliente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@NombreComercial", obj.NombreComercial);
                cmd.Parameters.AddWithValue("@RazonSocial", (object)obj.RazonSocial ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NIT", obj.NIT);
                cmd.Parameters.AddWithValue("@NombreContacto", obj.NombreContacto);
                cmd.Parameters.AddWithValue("@TelefonoContacto", obj.TelefonoContacto);
                cmd.Parameters.AddWithValue("@CorreoContacto", (object)obj.CorreoContacto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DireccionBodega", obj.DireccionBodega);
                cmd.Parameters.AddWithValue("@CuentaBancaria", (object)obj.CuentaBancaria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Banco", (object)obj.Banco ?? DBNull.Value);

                try
                {
                    con.Open();
                    idGenerado = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    idGenerado = 0;
                    Mensaje = ex.Message;
                }
            }
            return idGenerado;
        }

        //  EDITAR EMPRESA CLIENTE
        public bool Editar(Cliente obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_EditarCliente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCliente", obj.IdCliente);
                cmd.Parameters.AddWithValue("@NombreComercial", obj.NombreComercial);
                cmd.Parameters.AddWithValue("@RazonSocial", (object)obj.RazonSocial ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NIT", obj.NIT);
                cmd.Parameters.AddWithValue("@NombreContacto", obj.NombreContacto);
                cmd.Parameters.AddWithValue("@TelefonoContacto", obj.TelefonoContacto);
                cmd.Parameters.AddWithValue("@CorreoContacto", (object)obj.CorreoContacto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DireccionBodega", obj.DireccionBodega);
                cmd.Parameters.AddWithValue("@CuentaBancaria", (object)obj.CuentaBancaria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Banco", (object)obj.Banco ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Activo", obj.Activo);

                try
                {
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

        // ELIMINAR CLIENTE
        public bool Eliminar(int idCliente, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarCliente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                try
                {
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
    }
}
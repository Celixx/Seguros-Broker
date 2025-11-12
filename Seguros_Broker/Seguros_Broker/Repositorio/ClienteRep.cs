using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Data.SqlClient;

namespace Seguros_Broker.Repositorio
{
    public class ClienteRep : Repositorio
    {
        public List<Cliente> GetClientes()
        {
            var clientes = new List<Cliente>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Trae datos de CLIENTE, JOIN con HOLDING y EJECUTIVO para nombres desplegables
                    string sql = @"SELECT C.TipoIdentificacion, C.ID AS ClienteID, C.Nombre, C.ApellidoPaterno, C.ApellidoMaterno,
                                          C.HoldingID, H.Nombre AS HoldingNombre,
                                          C.EjecutivoID, E.Nombre AS EjecutivoNombre,
                                          C.Fonos, C.PaginaWeb, C.NombreCorto, C.Referencia,
                                          C.Particular_Pais, C.Particular_Region, C.Particular_Ciudad, C.Particular_Comuna, C.Particular_Direccion,
                                          C.Comercial_Pais, C.Comercial_Region, C.Comercial_Ciudad, C.Comercial_Comuna, C.Comercial_Direccion
                                   FROM CLIENTE C
                                   LEFT JOIN HOLDING H ON C.HoldingID = H.ID
                                   LEFT JOIN Ejecutivo E ON C.EjecutivoID = E.ID
                                   ORDER BY C.ID DESC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cliente = new Cliente();

                            cliente.TipoIdentificacion = reader["TipoIdentificacion"] != DBNull.Value ? reader["TipoIdentificacion"].ToString() : "";
                            cliente.ID = reader["ClienteID"] != DBNull.Value ? reader["ClienteID"].ToString() : "";
                            cliente.Nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "";
                            cliente.ApellidoPaterno = reader["ApellidoPaterno"] != DBNull.Value ? reader["ApellidoPaterno"].ToString() : "";
                            cliente.ApellidoMaterno = reader["ApellidoMaterno"] != DBNull.Value ? reader["ApellidoMaterno"].ToString() : "";

                            cliente.HoldingID = reader["HoldingID"] != DBNull.Value ? Convert.ToInt32(reader["HoldingID"]) : 0;
                            cliente.HoldingNombre = reader["HoldingNombre"] != DBNull.Value ? reader["HoldingNombre"].ToString() : "";

                            cliente.EjecutivoID = reader["EjecutivoID"] != DBNull.Value ? reader["EjecutivoID"].ToString() : "";
                            cliente.EjecutivoNombre = reader["EjecutivoNombre"] != DBNull.Value ? reader["EjecutivoNombre"].ToString() : "";

                            cliente.Fonos = reader["Fonos"] != DBNull.Value ? reader["Fonos"].ToString() : "";
                            cliente.PaginaWeb = reader["PaginaWeb"] != DBNull.Value ? reader["PaginaWeb"].ToString() : "";
                            cliente.NombreCorto = reader["NombreCorto"] != DBNull.Value ? reader["NombreCorto"].ToString() : "";
                            cliente.Referencia = reader["Referencia"] != DBNull.Value ? reader["Referencia"].ToString() : "";

                            cliente.Particular_Pais = reader["Particular_Pais"] != DBNull.Value ? reader["Particular_Pais"].ToString() : "DESCONOCIDO";
                            cliente.Particular_Region = reader["Particular_Region"] != DBNull.Value ? reader["Particular_Region"].ToString() : "";
                            cliente.Particular_Ciudad = reader["Particular_Ciudad"] != DBNull.Value ? reader["Particular_Ciudad"].ToString() : "";
                            cliente.Particular_Comuna = reader["Particular_Comuna"] != DBNull.Value ? reader["Particular_Comuna"].ToString() : "";
                            cliente.Particular_Direccion = reader["Particular_Direccion"] != DBNull.Value ? reader["Particular_Direccion"].ToString() : "";

                            cliente.Comercial_Pais = reader["Comercial_Pais"] != DBNull.Value ? reader["Comercial_Pais"].ToString() : "DESCONOCIDO";
                            cliente.Comercial_Region = reader["Comercial_Region"] != DBNull.Value ? reader["Comercial_Region"].ToString() : "";
                            cliente.Comercial_Ciudad = reader["Comercial_Ciudad"] != DBNull.Value ? reader["Comercial_Ciudad"].ToString() : "";
                            cliente.Comercial_Comuna = reader["Comercial_Comuna"] != DBNull.Value ? reader["Comercial_Comuna"].ToString() : "";
                            cliente.Comercial_Direccion = reader["Comercial_Direccion"] != DBNull.Value ? reader["Comercial_Direccion"].ToString() : "";

                            clientes.Add(cliente);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en Clientes: " + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return clientes;
        }

        public async Task<(bool success, string? errorMessage)> CreateClienteAsync(Cliente cliente)
        {
            if (cliente == null)
                return (false, "Cliente nulo.");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    const string checkSql = "SELECT COUNT(1) FROM CLIENTE WHERE ID = @ID";
                    using (var checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.Add("@ID", System.Data.SqlDbType.NVarChar).Value = cliente.ID;
                        var exists = (int)await checkCmd.ExecuteScalarAsync();
                        if (exists > 0)
                        {
                            return (false, "Ya existe un Cliente con el mismo ID");
                        }
                    }

                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"INSERT INTO CLIENTE
                                                    (TipoIdentificacion, ID, Nombre, ApellidoPaterno, ApellidoMaterno, HoldingID, EjecutivoID, Fonos, PaginaWeb, NombreCorto, Referencia,
                                                     Particular_Pais, Particular_Region, Particular_Ciudad, Particular_Comuna, Particular_Direccion,
                                                     Comercial_Pais, Comercial_Region, Comercial_Ciudad, Comercial_Comuna, Comercial_Direccion)
                                                    VALUES
                                                    (@TipoIdentificacion, @ID, @Nombre, @ApellidoPaterno, @ApellidoMaterno, @HoldingID, @EjecutivoID, @Fonos, @PaginaWeb, @NombreCorto, @Referencia,
                                                     @Particular_Pais, @Particular_Region, @Particular_Ciudad, @Particular_Comuna, @Particular_Direccion,
                                                     @Comercial_Pais, @Comercial_Region, @Comercial_Ciudad, @Comercial_Comuna, @Comercial_Direccion);";

                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {
                            cmd.Parameters.Add("@TipoIdentificacion", System.Data.SqlDbType.NVarChar, 50).Value = (object)cliente.TipoIdentificacion ?? DBNull.Value;
                            cmd.Parameters.Add("@ID", System.Data.SqlDbType.NVarChar, 50).Value = (object)cliente.ID ?? DBNull.Value;
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 30).Value = (object)cliente.Nombre ?? DBNull.Value;
                            cmd.Parameters.Add("@ApellidoPaterno", System.Data.SqlDbType.NVarChar, 30).Value = (object)cliente.ApellidoPaterno ?? DBNull.Value;
                            cmd.Parameters.Add("@ApellidoMaterno", System.Data.SqlDbType.NVarChar, 30).Value = (object)cliente.ApellidoMaterno ?? DBNull.Value;
                            cmd.Parameters.Add("@HoldingID", System.Data.SqlDbType.Int).Value = cliente.HoldingID;
                            cmd.Parameters.Add("@EjecutivoID", System.Data.SqlDbType.NVarChar, 10).Value = (object)cliente.EjecutivoID ?? DBNull.Value;
                            cmd.Parameters.Add("@Fonos", System.Data.SqlDbType.VarChar, 15).Value = (object)cliente.Fonos ?? DBNull.Value;
                            cmd.Parameters.Add("@PaginaWeb", System.Data.SqlDbType.VarChar, 40).Value = (object)cliente.PaginaWeb ?? DBNull.Value;
                            cmd.Parameters.Add("@NombreCorto", System.Data.SqlDbType.VarChar, 30).Value = (object)cliente.NombreCorto ?? DBNull.Value;
                            cmd.Parameters.Add("@Referencia", System.Data.SqlDbType.VarChar, 30).Value = (object)cliente.Referencia ?? DBNull.Value;

                            cmd.Parameters.Add("@Particular_Pais", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Particular_Pais ?? DBNull.Value;
                            cmd.Parameters.Add("@Particular_Region", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Particular_Region ?? DBNull.Value;
                            cmd.Parameters.Add("@Particular_Ciudad", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Particular_Ciudad ?? DBNull.Value;
                            cmd.Parameters.Add("@Particular_Comuna", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Particular_Comuna ?? DBNull.Value;
                            cmd.Parameters.Add("@Particular_Direccion", System.Data.SqlDbType.VarChar, 255).Value = (object)cliente.Particular_Direccion ?? DBNull.Value;

                            cmd.Parameters.Add("@Comercial_Pais", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Comercial_Pais ?? DBNull.Value;
                            cmd.Parameters.Add("@Comercial_Region", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Comercial_Region ?? DBNull.Value;
                            cmd.Parameters.Add("@Comercial_Ciudad", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Comercial_Ciudad ?? DBNull.Value;
                            cmd.Parameters.Add("@Comercial_Comuna", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Comercial_Comuna ?? DBNull.Value;
                            cmd.Parameters.Add("@Comercial_Direccion", System.Data.SqlDbType.VarChar, 255).Value = (object)cliente.Comercial_Direccion ?? DBNull.Value;

                            int rows = await cmd.ExecuteNonQueryAsync();

                            if (rows <= 0)
                            {
                                await tran.RollbackAsync();
                                return (false, "No se insertó el registro (0 filas afectadas).");
                            }

                            await tran.CommitAsync();
                            return (true, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public Cliente? GetCliente(string ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM CLIENTE WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Cliente cliente = new Cliente();

                                cliente.TipoIdentificacion = reader["TipoIdentificacion"] != DBNull.Value ? reader["TipoIdentificacion"].ToString() : "";
                                cliente.ID = reader["ID"] != DBNull.Value ? reader["ID"].ToString() : "";
                                cliente.Nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "";
                                cliente.ApellidoPaterno = reader["ApellidoPaterno"] != DBNull.Value ? reader["ApellidoPaterno"].ToString() : "";
                                cliente.ApellidoMaterno = reader["ApellidoMaterno"] != DBNull.Value ? reader["ApellidoMaterno"].ToString() : "";

                                cliente.HoldingID = reader["HoldingID"] != DBNull.Value ? Convert.ToInt32(reader["HoldingID"]) : 0;
                                cliente.EjecutivoID = reader["EjecutivoID"] != DBNull.Value ? reader["EjecutivoID"].ToString() : "";

                                cliente.Fonos = reader["Fonos"] != DBNull.Value ? reader["Fonos"].ToString() : "";
                                cliente.PaginaWeb = reader["PaginaWeb"] != DBNull.Value ? reader["PaginaWeb"].ToString() : "";
                                cliente.NombreCorto = reader["NombreCorto"] != DBNull.Value ? reader["NombreCorto"].ToString() : "";
                                cliente.Referencia = reader["Referencia"] != DBNull.Value ? reader["Referencia"].ToString() : "";

                                cliente.Particular_Pais = reader["Particular_Pais"] != DBNull.Value ? reader["Particular_Pais"].ToString() : "DESCONOCIDO";
                                cliente.Particular_Region = reader["Particular_Region"] != DBNull.Value ? reader["Particular_Region"].ToString() : "";
                                cliente.Particular_Ciudad = reader["Particular_Ciudad"] != DBNull.Value ? reader["Particular_Ciudad"].ToString() : "";
                                cliente.Particular_Comuna = reader["Particular_Comuna"] != DBNull.Value ? reader["Particular_Comuna"].ToString() : "";
                                cliente.Particular_Direccion = reader["Particular_Direccion"] != DBNull.Value ? reader["Particular_Direccion"].ToString() : "";

                                cliente.Comercial_Pais = reader["Comercial_Pais"] != DBNull.Value ? reader["Comercial_Pais"].ToString() : "DESCONOCIDO";
                                cliente.Comercial_Region = reader["Comercial_Region"] != DBNull.Value ? reader["Comercial_Region"].ToString() : "";
                                cliente.Comercial_Ciudad = reader["Comercial_Ciudad"] != DBNull.Value ? reader["Comercial_Ciudad"].ToString() : "";
                                cliente.Comercial_Comuna = reader["Comercial_Comuna"] != DBNull.Value ? reader["Comercial_Comuna"].ToString() : "";
                                cliente.Comercial_Direccion = reader["Comercial_Direccion"] != DBNull.Value ? reader["Comercial_Direccion"].ToString() : "";

                                return cliente;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se encontró el ID, pero ocurrió un error al leer los datos:\n\n" + ex.Message, "Error de Lectura", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine("Exception: " + ex.ToString());
            }

            return null;
        }

        public async Task<(bool success, string? errorMessage)> UpdateClienteAsync(Cliente cliente)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var tran = connection.BeginTransaction())
                    {
                        const string updateSql = @"UPDATE CLIENTE
                                                   SET TipoIdentificacion=@TipoIdentificacion,
                                                       Nombre=@Nombre,
                                                       ApellidoPaterno=@ApellidoPaterno,
                                                       ApellidoMaterno=@ApellidoMaterno,
                                                       HoldingID=@HoldingID,
                                                       EjecutivoID=@EjecutivoID,
                                                       Fonos=@Fonos,
                                                       PaginaWeb=@PaginaWeb,
                                                       NombreCorto=@NombreCorto,
                                                       Referencia=@Referencia,
                                                       Particular_Pais=@Particular_Pais,
                                                       Particular_Region=@Particular_Region,
                                                       Particular_Ciudad=@Particular_Ciudad,
                                                       Particular_Comuna=@Particular_Comuna,
                                                       Particular_Direccion=@Particular_Direccion,
                                                       Comercial_Pais=@Comercial_Pais,
                                                       Comercial_Region=@Comercial_Region,
                                                       Comercial_Ciudad=@Comercial_Ciudad,
                                                       Comercial_Comuna=@Comercial_Comuna,
                                                       Comercial_Direccion=@Comercial_Direccion
                                                   WHERE ID=@ID;";

                        using (var cmd = new SqlCommand(updateSql, connection, tran))
                        {
                            cmd.Parameters.Add("@TipoIdentificacion", System.Data.SqlDbType.NVarChar, 50).Value = (object)cliente.TipoIdentificacion ?? DBNull.Value;
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 30).Value = (object)cliente.Nombre ?? DBNull.Value;
                            cmd.Parameters.Add("@ApellidoPaterno", System.Data.SqlDbType.NVarChar, 30).Value = (object)cliente.ApellidoPaterno ?? DBNull.Value;
                            cmd.Parameters.Add("@ApellidoMaterno", System.Data.SqlDbType.NVarChar, 30).Value = (object)cliente.ApellidoMaterno ?? DBNull.Value;
                            cmd.Parameters.Add("@HoldingID", System.Data.SqlDbType.Int).Value = cliente.HoldingID;
                            cmd.Parameters.Add("@EjecutivoID", System.Data.SqlDbType.NVarChar, 10).Value = (object)cliente.EjecutivoID ?? DBNull.Value;
                            cmd.Parameters.Add("@Fonos", System.Data.SqlDbType.VarChar, 15).Value = (object)cliente.Fonos ?? DBNull.Value;
                            cmd.Parameters.Add("@PaginaWeb", System.Data.SqlDbType.VarChar, 40).Value = (object)cliente.PaginaWeb ?? DBNull.Value;
                            cmd.Parameters.Add("@NombreCorto", System.Data.SqlDbType.VarChar, 30).Value = (object)cliente.NombreCorto ?? DBNull.Value;
                            cmd.Parameters.Add("@Referencia", System.Data.SqlDbType.VarChar, 30).Value = (object)cliente.Referencia ?? DBNull.Value;

                            cmd.Parameters.Add("@Particular_Pais", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Particular_Pais ?? DBNull.Value;
                            cmd.Parameters.Add("@Particular_Region", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Particular_Region ?? DBNull.Value;
                            cmd.Parameters.Add("@Particular_Ciudad", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Particular_Ciudad ?? DBNull.Value;
                            cmd.Parameters.Add("@Particular_Comuna", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Particular_Comuna ?? DBNull.Value;
                            cmd.Parameters.Add("@Particular_Direccion", System.Data.SqlDbType.VarChar, 255).Value = (object)cliente.Particular_Direccion ?? DBNull.Value;

                            cmd.Parameters.Add("@Comercial_Pais", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Comercial_Pais ?? DBNull.Value;
                            cmd.Parameters.Add("@Comercial_Region", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Comercial_Region ?? DBNull.Value;
                            cmd.Parameters.Add("@Comercial_Ciudad", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Comercial_Ciudad ?? DBNull.Value;
                            cmd.Parameters.Add("@Comercial_Comuna", System.Data.SqlDbType.VarChar, 100).Value = (object)cliente.Comercial_Comuna ?? DBNull.Value;
                            cmd.Parameters.Add("@Comercial_Direccion", System.Data.SqlDbType.VarChar, 255).Value = (object)cliente.Comercial_Direccion ?? DBNull.Value;

                            cmd.Parameters.Add("@ID", System.Data.SqlDbType.NVarChar, 50).Value = (object)cliente.ID ?? DBNull.Value;

                            int rows = await cmd.ExecuteNonQueryAsync();

                            if (rows <= 0)
                            {
                                await tran.RollbackAsync();
                                return (false, "No se actualizó el registro (0 filas afectadas).");
                            }

                            await tran.CommitAsync();
                            return (true, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
using Microsoft.Data.SqlClient;
using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Seguros_Broker.Repositorio
{
    internal class SocioRep : Repositorio
    {
        public Socio? GetSocio(int ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM SOCIO WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("@ID", ID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Socio socio = new Socio();

                                socio.tipoId = reader.IsDBNull(0) ? null : reader.GetString(0);
                                socio.ID = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                socio.nombre = reader.IsDBNull(2) ? null : reader.GetString(2);
                                socio.aPaterno = reader.IsDBNull(3) ? null : reader.GetString(3);
                                socio.aMaterno = reader.IsDBNull(4) ? null : reader.GetString(4);
                                socio.fono = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                                socio.celular = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                                socio.mail = reader.IsDBNull(7) ? null : reader.GetString(7);
                                socio.fax = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                                socio.direccion = reader.IsDBNull(9) ? null : reader.GetString(9);
                                socio.observacion = reader.IsDBNull(10) ? null : reader.GetString(10);
                                socio.comision = reader.IsDBNull(11) ? 0 : reader.GetInt32(11);
                                socio.porcentajeComision = reader.IsDBNull(12) ? 0 : reader.GetInt32(12);

                                return socio;
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

        public List<Socio> GetSocios()
        {
            var socios = new List<Socio>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT TipoID, ID, Nombre, APaterno, AMaterno, Fono, Celular, Mail, Fax, Direccion, Observacion, Comision, PorcentajeComision FROM SOCIO ORDER BY ID DESC;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var socio = new Socio();

                                socio.tipoId = reader["TipoID"] != DBNull.Value ? reader["TipoID"].ToString() : "";
                                socio.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                socio.nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "";
                                socio.aPaterno = reader["APaterno"] != DBNull.Value ? reader["APaterno"].ToString() : "";
                                socio.aMaterno = reader["AMaterno"] != DBNull.Value ? reader["AMaterno"].ToString() : "";
                                socio.fono = reader["Fono"] != DBNull.Value ? Convert.ToInt32(reader["Fono"]) : 0;
                                socio.celular = reader["Celular"] != DBNull.Value ? Convert.ToInt32(reader["Celular"]) : 0;
                                socio.mail = reader["Mail"] != DBNull.Value ? reader["Mail"].ToString() : "";
                                socio.fax = reader["Fax"] != DBNull.Value ? Convert.ToInt32(reader["Fax"]) : 0;
                                socio.direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "";
                                socio.observacion = reader["Observacion"] != DBNull.Value ? reader["Observacion"].ToString() : "";
                                socio.comision = reader["Comision"] != DBNull.Value ? Convert.ToInt32(reader["Comision"]) : 0;
                                socio.porcentajeComision = reader["PorcentajeComision"] != DBNull.Value ? Convert.ToInt32(reader["PorcentajeComision"]) : 0;

                                socios.Add(socio);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show("Error en Compañía: " + ex.Message + "\n" + ex.StackTrace, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return socios;
        }

        public async Task<(bool success, string? errorMessage)> CreateSocioAsync(Socio socio)
        {
            if (socio == null)
                return (false, "Socio nulo.");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // verificar existencia por codigo o ID
                    const string checkSql = "SELECT COUNT(1) FROM SOCIO WHERE ID = @ID";
                    using (var checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = socio.ID;

                        var exists = (int)await checkCmd.ExecuteScalarAsync();
                        if (exists > 0)
                        {
                            return (false, "Ya existe un Socio con el mismo Identificación.");
                        }
                    }

                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"INSERT INTO SOCIO
                                                                    (TipoID, ID, Nombre, APaterno, AMaterno, Fono, Celular, Mail, Fax, Direccion, Observacion, Comision, PorcentajeComision)
                                                               VALUES (@TipoID, @ID, @Nombre, @APaterno, @AMaterno, @Fono, @Celular, @Mail, @Fax, @Direccion, @Observacion, @Comision, @PorcentajeComision);";

                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {
                            cmd.Parameters.Add("@TipoID", System.Data.SqlDbType.NVarChar, 15).Value = (object)socio.tipoId ?? DBNull.Value;
                            cmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = socio.ID;
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 25).Value = (object)socio.nombre?? DBNull.Value;
                            cmd.Parameters.Add("@APaterno", System.Data.SqlDbType.NVarChar, 25).Value = (object)socio.aPaterno ?? DBNull.Value;
                            cmd.Parameters.Add("@AMaterno", System.Data.SqlDbType.NVarChar, 25).Value = (object)socio.aMaterno ?? DBNull.Value;
                            cmd.Parameters.Add("@Fono", System.Data.SqlDbType.Int).Value = socio.fono;
                            cmd.Parameters.Add("@Celular", System.Data.SqlDbType.Int).Value = socio.celular;
                            cmd.Parameters.Add("@Mail", System.Data.SqlDbType.NVarChar, 50).Value = (object)socio.mail ?? DBNull.Value;
                            cmd.Parameters.Add("@Fax", System.Data.SqlDbType.Int).Value = socio.fax;
                            cmd.Parameters.Add("@Direccion", System.Data.SqlDbType.NVarChar, 50).Value = (object)socio.direccion ?? DBNull.Value;
                            cmd.Parameters.Add("@Observacion", System.Data.SqlDbType.NVarChar, 50).Value = (object)socio.observacion ?? DBNull.Value;
                            cmd.Parameters.Add("@Comision", System.Data.SqlDbType.Int).Value = socio.comision;
                            cmd.Parameters.Add("@PorcentajeComision", System.Data.SqlDbType.Int).Value = socio.porcentajeComision;

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

        public async Task<(bool success, string? errorMessage)> UpdateSocioAsync(Socio socio)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"UPDATE SOCIO
                                                    SET TipoID=@TipoID, Nombre=@Nombre, APaterno=@APaterno, AMaterno=@AMaterno, Fono=@Fono, Celular=@Celular, Mail=@Mail, Fax=@Fax, Direccion=@Direccion, Observacion=@Observacion, Comision=@Comision, PorcentajeComision=@PorcentajeComision
                                                    WHERE ID=@ID;";
                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {
                            cmd.Parameters.Add("@TipoID", System.Data.SqlDbType.NVarChar, 15).Value = (object)socio.tipoId ?? DBNull.Value;
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 25).Value = (object)socio.nombre ?? DBNull.Value;
                            cmd.Parameters.Add("@APaterno", System.Data.SqlDbType.NVarChar, 25).Value = (object)socio.aPaterno ?? DBNull.Value;
                            cmd.Parameters.Add("@AMaterno", System.Data.SqlDbType.NVarChar, 25).Value = (object)socio.aMaterno ?? DBNull.Value;
                            cmd.Parameters.Add("@Fono", System.Data.SqlDbType.Int).Value = socio.fono;
                            cmd.Parameters.Add("@Celular", System.Data.SqlDbType.Int).Value = socio.celular;
                            cmd.Parameters.Add("@Mail", System.Data.SqlDbType.NVarChar, 50).Value = (object)socio.mail ?? DBNull.Value;
                            cmd.Parameters.Add("@Fax", System.Data.SqlDbType.Int).Value = socio.fax;
                            cmd.Parameters.Add("@Direccion", System.Data.SqlDbType.NVarChar, 50).Value = (object)socio.direccion ?? DBNull.Value;
                            cmd.Parameters.Add("@Observacion", System.Data.SqlDbType.NVarChar, 50).Value = (object)socio.observacion ?? DBNull.Value;
                            cmd.Parameters.Add("@Comision", System.Data.SqlDbType.Int).Value = socio.comision;
                            cmd.Parameters.Add("@PorcentajeComision", System.Data.SqlDbType.Int).Value = socio.porcentajeComision;
                            cmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = socio.ID;

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

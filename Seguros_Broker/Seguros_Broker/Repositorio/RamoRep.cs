using Microsoft.Data.SqlClient;
using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Seguros_Broker.Repositorio
{
    public class RamoRep : Repositorio
    {
        public List<Ramo> GetRamos()
        {
            var ramos = new List<Ramo>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT ID, Nombre FROM RAMO ORDER BY ID ASC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var r = new Ramo();
                            r.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                            r.nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "";
                            ramos.Add(r);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en Ramos: " + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return ramos;
        }

        public async Task<(bool success, string? errorMessage)> CreateRamoAsync(Ramo ramo)
        {
            if (ramo == null) return (false, "Ramo nulo.");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    const string checkSql = "SELECT COUNT(1) FROM RAMO WHERE Nombre = @Nombre";
                    using (var checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar).Value = ramo.nombre;
                        var exists = (int)await checkCmd.ExecuteScalarAsync();
                        if (exists > 0) return (false, "Ya existe un Ramo con el mismo Nombre");
                    }

                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"INSERT INTO RAMO (Nombre) VALUES (@Nombre);";
                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 50).Value = (object)ramo.nombre ?? DBNull.Value;
                            int rows = await cmd.ExecuteNonQueryAsync();
                            if (rows <= 0) { await tran.RollbackAsync(); return (false, "No se insertó el registro (0 filas afectadas)."); }
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

        public Ramo? GetRamo(int ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM RAMO WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var r = new Ramo();
                                r.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                r.nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "";
                                return r;
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

        public async Task<(bool success, string? errorMessage)> UpdateRamoAsync(Ramo ramo)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var tran = connection.BeginTransaction())
                    {
                        const string updateSql = @"UPDATE RAMO SET Nombre=@Nombre WHERE ID=@ID;";
                        using (var cmd = new SqlCommand(updateSql, connection, tran))
                        {
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 50).Value = (object)ramo.nombre ?? DBNull.Value;
                            cmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = ramo.ID;

                            int rows = await cmd.ExecuteNonQueryAsync();
                            if (rows <= 0) { await tran.RollbackAsync(); return (false, "No se actualizó el registro (0 filas afectadas)."); }
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


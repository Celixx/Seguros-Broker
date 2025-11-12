using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Seguros_Broker.Repositorio
{
    public class HoldingRep : Repositorio
    {
        public List<Holding> GetHoldings()
        {
            var holdings = new List<Holding>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT ID, Nombre FROM HOLDING ORDER BY ID ASC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var h = new Holding();

                            h.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                            h.Nombre = reader["Nombre"] != DBNull.Value ? Convert.ToString(reader["Nombre"]) : "";

                            holdings.Add(h);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en Holdings: " + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return holdings;
        }

        public async Task<(bool success, string? errorMessage)> CreateHoldingAsync(Holding holding)
        {
            if (holding == null)
                return (false, "Holding nulo.");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    const string checkSql = "SELECT COUNT(1) FROM HOLDING WHERE Nombre = @Nombre";
                    using (var checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar).Value = holding.Nombre;
                        var exists = (int)await checkCmd.ExecuteScalarAsync();
                        if (exists > 0)
                        {
                            return (false, "Ya existe un Holding con el mismo Nombre");
                        }
                    }

                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"INSERT INTO HOLDING (Nombre) VALUES (@Nombre);";
                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 50).Value = (object)holding.Nombre ?? DBNull.Value;

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

        public Holding? GetHolding(int ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM HOLDING WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Holding h = new Holding();
                                h.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                h.Nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "";
                                return h;
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

        public async Task<(bool success, string? errorMessage)> UpdateHoldingAsync(Holding holding)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var tran = connection.BeginTransaction())
                    {
                        const string updateSql = @"UPDATE HOLDING SET Nombre=@Nombre WHERE ID=@ID;";
                        using (var cmd = new SqlCommand(updateSql, connection, tran))
                        {
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 50).Value = (object)holding.Nombre ?? DBNull.Value;
                            cmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = holding.ID;

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
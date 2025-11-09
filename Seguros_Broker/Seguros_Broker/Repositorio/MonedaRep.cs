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
    public class MonedaRep : Repositorio
    {

        public List<Moneda> GetMonedas()
        {
            var monedas = new List<Moneda>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT MonedaID, Nombre, Simbolo FROM Moneda ORDER BY MonedaID DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var moneda = new Moneda();

                                moneda.monedaId = reader["MonedaID"] != DBNull.Value ? Convert.ToInt32(reader["MonedaID"]) : 0;
                                moneda.nombre =   reader["Nombre"] != DBNull.Value ? Convert.ToString(reader["Nombre"]) : "";
                                moneda.simbolo =  reader["Simbolo"] != DBNull.Value ? Convert.ToString(reader["simbolo"]) : "";

                                monedas.Add(moneda);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show("Error en GetMonedas: " + ex.Message + "\n" + ex.StackTrace, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

            }

            return monedas;
        }


        public Moneda? GetMoneda(int MonedaID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Moneda WHERE MonedaID=@MonedaID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("@MonedaID", MonedaID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Moneda moneda = new Moneda();

                                moneda.monedaId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                moneda.nombre = reader.IsDBNull(1) ? null : reader.GetString(1);
                                moneda.simbolo= reader.IsDBNull(2) ? null : reader.GetString(2);

                                return moneda;
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

        public async Task<(bool success, string? errorMessage)> CreateMonedaAsync(Moneda moneda)
        {
            if (moneda == null)
                return (false, "Moneda nula.");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
        
                    const string checkSql = "SELECT COUNT(1) FROM MONEDA WHERE Nombre = @Nombre";
                    using (var checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar).Value = moneda.nombre;
                        checkCmd.Parameters.Add("@Simbolo", System.Data.SqlDbType.NVarChar).Value = moneda.simbolo;

                        var exists = (int)await checkCmd.ExecuteScalarAsync();
                        if (exists > 0)
                        {
                            return (false, "Ya existe una Moneda con el mismo nombre.");
                        }
                    }

                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"INSERT INTO Moneda
                                                    (Nombre, Simbolo)
                                                VALUES
                                                    (@Nombre, @Simbolo);";

                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {

                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar).Value = moneda.nombre;
                            cmd.Parameters.Add("@Simbolo", System.Data.SqlDbType.NVarChar).Value = moneda.simbolo;


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


    }
}

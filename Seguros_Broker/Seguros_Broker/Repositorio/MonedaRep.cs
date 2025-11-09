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

        //public async Task<(bool success, string? errorMessage)> CreateEjecutivoAsync(EjecutivoM ejecutivo)
        //{
        //    if (ejecutivo == null)
        //        return (false, "Ejecutivo nulo.");

        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionString))
        //        {
        //            await connection.OpenAsync();

        //            Verificar existencia por codigo o ID
        //            const string checkSql = "SELECT COUNT(1) FROM Ejecutivo WHERE codigo = @codigo OR ID = @ID";
        //            using (var checkCmd = new SqlCommand(checkSql, connection))
        //            {
        //                checkCmd.Parameters.Add("@codigo", System.Data.SqlDbType.Int).Value = ejecutivo.codigo;
        //                checkCmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = ejecutivo.ID;

        //                var exists = (int)await checkCmd.ExecuteScalarAsync();
        //                if (exists > 0)
        //                {
        //                    return (false, "Ya existe un Ejecutivo con el mismo Código o Identificación.");
        //                }
        //            }

        //            Insert dentro de una transacción
        //            using (var tran = connection.BeginTransaction())
        //            {
        //                const string insertSql = @"
        //            INSERT INTO Ejecutivo
        //              (codigo, tipoId, ID, nombre, aPaterno, aMaterno, fono, celular, mail, comision, PorcentajeComision, nick, perfil, estado, restricciones)
        //            VALUES
        //              (@codigo, @tipoId, @ID, @nombre, @aPaterno, @aMaterno, @fono, @celular, @mail, @comision, @PorcentajeComision, @nick, @perfil, @estado, @restricciones);";

        //                using (var cmd = new SqlCommand(insertSql, connection, tran))
        //                {
        //                    Parámetros con tipos
        //                    cmd.Parameters.Add("@codigo", System.Data.SqlDbType.Int).Value = ejecutivo.codigo;
        //                    cmd.Parameters.Add("@tipoId", System.Data.SqlDbType.NVarChar, 50).Value = (object)ejecutivo.tipoId ?? DBNull.Value;
        //                    cmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = ejecutivo.ID;
        //                    cmd.Parameters.Add("@nombre", System.Data.SqlDbType.NVarChar, 200).Value = (object)ejecutivo.nombre ?? DBNull.Value;
        //                    cmd.Parameters.Add("@aPaterno", System.Data.SqlDbType.NVarChar, 100).Value = (object)ejecutivo.aPaterno ?? DBNull.Value;
        //                    cmd.Parameters.Add("@aMaterno", System.Data.SqlDbType.NVarChar, 100).Value = (object)ejecutivo.aMaterno ?? DBNull.Value;
        //                    cmd.Parameters.Add("@fono", System.Data.SqlDbType.Int).Value = ejecutivo.fono;
        //                    cmd.Parameters.Add("@celular", System.Data.SqlDbType.Int).Value = ejecutivo.celular;
        //                    cmd.Parameters.Add("@mail", System.Data.SqlDbType.NVarChar, 200).Value = (object)ejecutivo.mail ?? DBNull.Value;
        //                    cmd.Parameters.Add("@comision", System.Data.SqlDbType.Int).Value = ejecutivo.comision;
        //                    cmd.Parameters.Add("@PorcentajeComision", System.Data.SqlDbType.Int).Value = ejecutivo.porcentajeComision;
        //                    cmd.Parameters.Add("@nick", System.Data.SqlDbType.NVarChar, 100).Value = (object)ejecutivo.nick ?? DBNull.Value;
        //                    cmd.Parameters.Add("@perfil", System.Data.SqlDbType.NVarChar, 100).Value = (object)ejecutivo.perfil ?? DBNull.Value;
        //                    cmd.Parameters.Add("@estado", System.Data.SqlDbType.NVarChar, 50).Value = (object)ejecutivo.estado ?? DBNull.Value;
        //                    cmd.Parameters.Add("@restricciones", System.Data.SqlDbType.NVarChar, 200).Value = (object)ejecutivo.restricciones ?? DBNull.Value;

        //                    int rows = await cmd.ExecuteNonQueryAsync();
        //                    if (rows <= 0)
        //                    {
        //                        await tran.RollbackAsync();
        //                        return (false, "No se insertó el registro (0 filas afectadas).");
        //                    }

        //                    await tran.CommitAsync();
        //                    return (true, null);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return (false, ex.Message);
        //    }
        //}


    }
}

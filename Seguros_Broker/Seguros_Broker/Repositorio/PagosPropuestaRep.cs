using Microsoft.Data.SqlClient;
using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Seguros_Broker.Repositorio
{
    public class PagosPropuestaRep : Repositorio
    {
        public async Task<(bool success, string? errorMessage)> CreatePagosForPropuestaAsync(int propuestaId, int numeroPoliza, List<PagoPropuesta> pagos)
        {
            if (propuestaId <= 0) return (false, "PropuestaId inválido.");
            if (pagos == null || pagos.Count == 0) return (false, "No hay pagos para insertar.");

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var tran = conn.BeginTransaction())
                    {
                        const string insertSql = @"
                            INSERT INTO PAGOS_PROPUESTA 
                                (PropuestaID, NumeroPoliza, CuotaNro, Monto, FechaVencimiento, FormaPago, NumeroDocumento, NroTarjCtaCte, TipoTarj, ValidezTarj, Banco)
                            VALUES
                                (@PropuestaID, @NumeroPoliza, @CuotaNro, @Monto, @FechaVencimiento, @FormaPago, @NumeroDocumento, @NroTarjCtaCte, @TipoTarj, @ValidezTarj, @Banco)";

                        using (var cmd = new SqlCommand(insertSql, conn, tran))
                        {
                            cmd.Parameters.Add("@PropuestaID", System.Data.SqlDbType.Int);
                            cmd.Parameters.Add("@NumeroPoliza", System.Data.SqlDbType.Int);
                            cmd.Parameters.Add("@CuotaNro", System.Data.SqlDbType.Int);
                            cmd.Parameters.Add("@Monto", System.Data.SqlDbType.Decimal).Precision = 18;
                            cmd.Parameters["@Monto"].Scale = 2;
                            cmd.Parameters.Add("@FechaVencimiento", System.Data.SqlDbType.Date);
                            cmd.Parameters.Add("@FormaPago", System.Data.SqlDbType.NVarChar, 20);
                            cmd.Parameters.Add("@NumeroDocumento", System.Data.SqlDbType.NVarChar, 100);
                            cmd.Parameters.Add("@NroTarjCtaCte", System.Data.SqlDbType.NVarChar, 100);
                            cmd.Parameters.Add("@TipoTarj", System.Data.SqlDbType.NVarChar, 50);
                            cmd.Parameters.Add("@ValidezTarj", System.Data.SqlDbType.NVarChar, 50);
                            cmd.Parameters.Add("@Banco", System.Data.SqlDbType.NVarChar, 100);

                            foreach (var p in pagos)
                            {
                                cmd.Parameters["@PropuestaID"].Value = propuestaId;
                                cmd.Parameters["@NumeroPoliza"].Value = numeroPoliza;
                                cmd.Parameters["@CuotaNro"].Value = p.CuotaNro;
                                cmd.Parameters["@Monto"].Value = p.Monto;
                                cmd.Parameters["@FechaVencimiento"].Value = p.FechaVencimiento.Date;
                                cmd.Parameters["@FormaPago"].Value = (object)p.FormaPago ?? DBNull.Value;
                                cmd.Parameters["@NumeroDocumento"].Value = (object)p.NumeroDocumento ?? DBNull.Value;
                                cmd.Parameters["@NroTarjCtaCte"].Value = (object)p.NroTarjCtaCte ?? DBNull.Value;
                                cmd.Parameters["@TipoTarj"].Value = (object)p.TipoTarj ?? DBNull.Value;
                                cmd.Parameters["@ValidezTarj"].Value = (object)p.ValidezTarj ?? DBNull.Value;
                                cmd.Parameters["@Banco"].Value = (object)p.Banco ?? DBNull.Value;

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        await tran.CommitAsync();
                        return (true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public List<PagoPropuesta> GetPagosByPropuestaID(int propuestaId)
        {
            var list = new List<PagoPropuesta>();
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT ID, PropuestaID, NumeroPoliza, CuotaNro, Monto, FechaVencimiento,
                               FormaPago, NumeroDocumento, NroTarjCtaCte, TipoTarj, ValidezTarj, Banco
                        FROM PAGOS_PROPUESTA
                        WHERE PropuestaID = @PropuestaID
                        ORDER BY CuotaNro ASC, ID ASC";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@PropuestaID", propuestaId);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var p = new PagoPropuesta
                                {
                                    ID = rdr["ID"] != DBNull.Value ? Convert.ToInt32(rdr["ID"]) : 0,
                                    PropuestaID = rdr["PropuestaID"] != DBNull.Value ? Convert.ToInt32(rdr["PropuestaID"]) : 0,
                                    NumeroPoliza = rdr["NumeroPoliza"] != DBNull.Value ? Convert.ToInt32(rdr["NumeroPoliza"]) : 0,
                                    CuotaNro = rdr["CuotaNro"] != DBNull.Value ? Convert.ToInt32(rdr["CuotaNro"]) : 0,
                                    Monto = rdr["Monto"] != DBNull.Value ? Convert.ToDecimal(rdr["Monto"]) : 0m,
                                    FechaVencimiento = rdr["FechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaVencimiento"]) : DateTime.MinValue,
                                    FormaPago = rdr["FormaPago"] != DBNull.Value ? rdr["FormaPago"].ToString() : "",
                                    NumeroDocumento = rdr["NumeroDocumento"] != DBNull.Value ? rdr["NumeroDocumento"].ToString() : "",
                                    NroTarjCtaCte = rdr["NroTarjCtaCte"] != DBNull.Value ? rdr["NroTarjCtaCte"].ToString() : "",
                                    TipoTarj = rdr["TipoTarj"] != DBNull.Value ? rdr["TipoTarj"].ToString() : "",
                                    ValidezTarj = rdr["ValidezTarj"] != DBNull.Value ? rdr["ValidezTarj"].ToString() : "",
                                    Banco = rdr["Banco"] != DBNull.Value ? rdr["Banco"].ToString() : ""
                                };
                                list.Add(p);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer pagos de propuesta: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return list;
        }

        public List<PagoPropuesta> GetPagosByPropuesta()
        {
            var list = new List<PagoPropuesta>();
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT ID, PropuestaID, NumeroPoliza, CuotaNro, Monto, FechaVencimiento,
                               FormaPago, NumeroDocumento, NroTarjCtaCte, TipoTarj, ValidezTarj, Banco
                        FROM PAGOS_PROPUESTA
                        ORDER BY CuotaNro ASC, ID ASC";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var p = new PagoPropuesta
                                {
                                    ID = rdr["ID"] != DBNull.Value ? Convert.ToInt32(rdr["ID"]) : 0,
                                    PropuestaID = rdr["PropuestaID"] != DBNull.Value ? Convert.ToInt32(rdr["PropuestaID"]) : 0,
                                    NumeroPoliza = rdr["NumeroPoliza"] != DBNull.Value ? Convert.ToInt32(rdr["NumeroPoliza"]) : 0,
                                    CuotaNro = rdr["CuotaNro"] != DBNull.Value ? Convert.ToInt32(rdr["CuotaNro"]) : 0,
                                    Monto = rdr["Monto"] != DBNull.Value ? Convert.ToDecimal(rdr["Monto"]) : 0m,
                                    FechaVencimiento = rdr["FechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaVencimiento"]) : DateTime.MinValue,
                                    FormaPago = rdr["FormaPago"] != DBNull.Value ? rdr["FormaPago"].ToString() : "",
                                    NumeroDocumento = rdr["NumeroDocumento"] != DBNull.Value ? rdr["NumeroDocumento"].ToString() : "",
                                    NroTarjCtaCte = rdr["NroTarjCtaCte"] != DBNull.Value ? rdr["NroTarjCtaCte"].ToString() : "",
                                    TipoTarj = rdr["TipoTarj"] != DBNull.Value ? rdr["TipoTarj"].ToString() : "",
                                    ValidezTarj = rdr["ValidezTarj"] != DBNull.Value ? rdr["ValidezTarj"].ToString() : "",
                                    Banco = rdr["Banco"] != DBNull.Value ? rdr["Banco"].ToString() : ""
                                };
                                list.Add(p);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer pagos de propuesta: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return list;
        }
    }
}

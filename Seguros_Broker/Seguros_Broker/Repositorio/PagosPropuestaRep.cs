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
    }
}

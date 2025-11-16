using Microsoft.Data.SqlClient;
using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Repositorio
{
    public class PropuestaRep : Repositorio
    {
        public async Task<(bool success, string? errorMessage)> CreatePropuestaAsync(Propuesta propuesta)
        {
            if (propuesta == null)
                return (false, "Propuesta nula.");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    const string checkSql = "SELECT COUNT(1) FROM PROPUESTA WHERE NumeroPoliza = @NumeroPoliza";
                    using (var checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.Add("@NumeroPoliza", System.Data.SqlDbType.NVarChar).Value = propuesta.NumeroPoliza;

                        var exists = (int)await checkCmd.ExecuteScalarAsync();
                        if (exists > 0)
                        {
                            return (false, "Ya existe una Póliza con el mismo número de póliza");
                        }
                    }

                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"INSERT INTO [dbo].[PROPUESTA] (
                                NumeroPoliza,
                                RenuevaPoliza,
								FechaRecepcion,
								TipoPoliza,
								FechaIngreso,
								FechaEmision,
								IDRamo,
								IDEjecutivo,
								Area,
								FechaCreacion,
								FechaVigenciaDesde,
								FechaVigenciaHasta,
								IDMoneda,
								ComisionAfecta,
								ComisionExenta,
								MontoAsegurado,
								ComisionTotal,
								PrimaNetaAfecta,
								PrimaNetaExenta,
								PrimaNetaTotal,
								IVA,
								PrimaBrutaTotal,
								IDCliente,
								IDSocio,
								IDGestor,
								IDCompania,
								MateriaAsegurada,
								Observacion)
								VALUES (
                                @NumeroPoliza,
                                @RenuevaPoliza,
								@FechaRecepcion,
								@TipoPoliza,
								@FechaIngreso,
								@FechaEmision,
								@IDRamo,
								@IDEjecutivo,
								@Area,
								@FechaCreacion,
								@FechaVigenciaDesde,
								@FechaVigenciaHasta,
								@IDMoneda,
								@ComisionAfecta,
								@ComisionExenta,
								@MontoAsegurado,
								@ComisionTotal,
								@PrimaNetaAfecta,
								@PrimaNetaExenta,
								@PrimaNetaTotal,
								@IVA,
								@PrimaBrutaTotal,
								@IDCliente,
								@IDSocio,
								@IDGestor,
								@IDCompania,
								@MateriaAsegurada,
								@Observacion
								);";
                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {
                            cmd.Parameters.Add("@NumeroPoliza", SqlDbType.Int).Value = propuesta.NumeroPoliza;
                            cmd.Parameters.Add("@RenuevaPoliza", SqlDbType.Int).Value = propuesta.RenuevaPoliza;
                            cmd.Parameters.Add("@FechaRecepcion", SqlDbType.Date).Value =
                                (object)propuesta.FechaRecepcion ?? DBNull.Value;

                            cmd.Parameters.Add("@TipoPoliza", SqlDbType.NVarChar, 50).Value =
                                (object)propuesta.TipoPoliza ?? DBNull.Value;

                            cmd.Parameters.Add("@FechaIngreso", SqlDbType.Date).Value =
                                (object)propuesta.FechaIngreso ?? DBNull.Value;

                            cmd.Parameters.Add("@FechaEmision", SqlDbType.Date).Value =
                                (object)propuesta.FechaEmision ?? DBNull.Value;

                            cmd.Parameters.Add("@IDRamo", SqlDbType.Int).Value = propuesta.IDRamo;

                            cmd.Parameters.Add("@IDEjecutivo", SqlDbType.NVarChar, 10).Value =
                                (object)propuesta.IDEjecutivo ?? DBNull.Value;

                            cmd.Parameters.Add("@Area", SqlDbType.NVarChar, 50).Value =
                                (object)propuesta.Area ?? DBNull.Value;

                            cmd.Parameters.Add("@FechaCreacion", SqlDbType.Date).Value =
                                (object)propuesta.FechaCreacion ?? DBNull.Value;

                            cmd.Parameters.Add("@FechaVigenciaDesde", SqlDbType.Date).Value =
                                (object)propuesta.FechaVigenciaDesde ?? DBNull.Value;

                            cmd.Parameters.Add("@FechaVigenciaHasta", SqlDbType.Date).Value =
                                (object)propuesta.FechaVigenciaHasta ?? DBNull.Value;

                            cmd.Parameters.Add("@IDMoneda", SqlDbType.Int).Value = propuesta.IDMoneda;

                            cmd.Parameters.Add("@ComisionAfecta", SqlDbType.Int).Value = propuesta.ComisionAfecta;

                            cmd.Parameters.Add("@ComisionExenta", SqlDbType.Int).Value = propuesta.ComisionExenta;

                            cmd.Parameters.Add("@MontoAsegurado", SqlDbType.Int).Value = propuesta.MontoAsegurado;

                            cmd.Parameters.Add("@ComisionTotal", SqlDbType.Int).Value = propuesta.ComisionTotal;

                            cmd.Parameters.Add("@PrimaNetaAfecta", SqlDbType.Int).Value = propuesta.PrimaNetaAfecta;

                            cmd.Parameters.Add("@PrimaNetaExenta", SqlDbType.Int).Value = propuesta.PrimaNetaExenta;

                            cmd.Parameters.Add("@PrimaNetaTotal", SqlDbType.Int).Value = propuesta.PrimaNetaTotal;

                            cmd.Parameters.Add("@IVA", SqlDbType.Int).Value = propuesta.IVA;

                            cmd.Parameters.Add("@PrimaBrutaTotal", SqlDbType.Int).Value = propuesta.PrimaBrutaTotal;

                            cmd.Parameters.Add("@IDCliente", SqlDbType.NVarChar, 20).Value =
                                (object)propuesta.IDCliente ?? DBNull.Value;

                            cmd.Parameters.Add("@IDSocio", SqlDbType.Int).Value = propuesta.IDSocio;

                            cmd.Parameters.Add("@IDGestor", SqlDbType.Int).Value = propuesta.IDGestor;

                            cmd.Parameters.Add("@IDCompania", SqlDbType.NVarChar, 10).Value =
                                (object)propuesta.IDCompania ?? DBNull.Value;

                            cmd.Parameters.Add("@MateriaAsegurada", SqlDbType.NVarChar, 100).Value =
                                (object)propuesta.MateriaAsegurada ?? DBNull.Value;

                            cmd.Parameters.Add("@Observacion", SqlDbType.NVarChar, 200).Value =
                                (object)propuesta.Observacion ?? DBNull.Value;


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

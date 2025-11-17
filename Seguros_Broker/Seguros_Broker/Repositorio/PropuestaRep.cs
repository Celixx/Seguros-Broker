using Microsoft.Data.SqlClient;
using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public List<Propuesta> GetPropuestas()
        {
            var propuestas = new List<Propuesta>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT ID, NumeroPoliza, RenuevaPoliza, FechaRecepcion, TipoPoliza, FechaIngreso, FechaEmision, IDRamo, IDEjecutivo, Area, FechaCreacion, FechaVigenciaDesde, FechaVigenciaHasta, IDMoneda, ComisionAfecta, ComisionExenta, MontoAsegurado, ComisionTotal, PrimaNetaAfecta, PrimaNetaExenta, PrimaNetaTotal, IVA, PrimaBrutaTotal, IDCliente, IDSocio, IDGestor, IDCompania, MateriaAsegurada, Observacion FROM PROPUESTA ORDER BY FechaVigenciaHasta DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var propuesta = new Propuesta();

                                propuesta.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                propuesta.NumeroPoliza = reader["NumeroPoliza"] != DBNull.Value ? Convert.ToInt32(reader["NumeroPoliza"]) : 0;
                                propuesta.RenuevaPoliza = reader["RenuevaPoliza"] != DBNull.Value ? Convert.ToInt32(reader["RenuevaPoliza"]) : 0;
                                propuesta.FechaRecepcion = reader["FechaRecepcion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaRecepcion"]) : (DateTime?)null;
                                propuesta.TipoPoliza = reader["TipoPoliza"] != DBNull.Value ? reader["TipoPoliza"].ToString() : "";
                                propuesta.FechaIngreso = reader["FechaIngreso"] != DBNull.Value ? Convert.ToDateTime(reader["FechaIngreso"]) : (DateTime?)null;
                                propuesta.FechaEmision = reader["FechaEmision"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEmision"]) : (DateTime?)null;
                                propuesta.IDRamo = reader["IDRamo"] != DBNull.Value ? Convert.ToInt32(reader["IDRamo"]) : 0;
                                propuesta.IDEjecutivo = reader["IDEjecutivo"] != DBNull.Value ? reader["IDEjecutivo"].ToString() : "";
                                propuesta.Area = reader["Area"] != DBNull.Value ? reader["Area"].ToString() : "";
                                propuesta.FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : (DateTime?)null;
                                propuesta.FechaVigenciaDesde = reader["FechaVigenciaDesde"] != DBNull.Value ? Convert.ToDateTime(reader["FechaVigenciaDesde"]) : (DateTime?)null;
                                propuesta.FechaVigenciaHasta = reader["FechaVigenciaHasta"] != DBNull.Value ? Convert.ToDateTime(reader["FechaVigenciaHasta"]) : (DateTime?)null;
                                propuesta.IDMoneda = reader["IDMoneda"] != DBNull.Value ? Convert.ToInt32(reader["IDMoneda"]) : 0;
                                propuesta.ComisionAfecta = reader["ComisionAfecta"] != DBNull.Value ? Convert.ToInt32(reader["ComisionAfecta"]) : 0;
                                propuesta.ComisionExenta = reader["ComisionExenta"] != DBNull.Value ? Convert.ToInt32(reader["ComisionExenta"]) : 0;
                                propuesta.MontoAsegurado = reader["MontoAsegurado"] != DBNull.Value ? Convert.ToInt32(reader["MontoAsegurado"]) : 0;
                                propuesta.ComisionTotal = reader["ComisionTotal"] != DBNull.Value ? Convert.ToInt32(reader["ComisionTotal"]) : 0;
                                propuesta.PrimaNetaAfecta = reader["PrimaNetaAfecta"] != DBNull.Value ? Convert.ToInt32(reader["PrimaNetaAfecta"]) : 0;
                                propuesta.PrimaNetaExenta = reader["PrimaNetaExenta"] != DBNull.Value ? Convert.ToInt32(reader["PrimaNetaExenta"]) : 0;
                                propuesta.PrimaNetaTotal = reader["PrimaNetaTotal"] != DBNull.Value ? Convert.ToInt32(reader["PrimaNetaTotal"]) : 0;
                                propuesta.IVA = reader["IVA"] != DBNull.Value ? Convert.ToInt32(reader["IVA"]) : 0;
                                propuesta.PrimaBrutaTotal = reader["PrimaBrutaTotal"] != DBNull.Value ? Convert.ToInt32(reader["PrimaBrutaTotal"]) : 0;
                                propuesta.IDCliente = reader["IDCliente"] != DBNull.Value ? reader["IDCliente"].ToString() : "";
                                propuesta.IDSocio = reader["IDSocio"] != DBNull.Value ? Convert.ToInt32(reader["IDSocio"]) : 0;
                                propuesta.IDGestor = reader["IDGestor"] != DBNull.Value ? Convert.ToInt32(reader["IDGestor"]) : 0;
                                propuesta.IDCompania = reader["IDCompania"] != DBNull.Value ? reader["IDCompania"].ToString() : "";
                                propuesta.MateriaAsegurada = reader["MateriaAsegurada"] != DBNull.Value ? reader["MateriaAsegurada"].ToString() : "";
                                propuesta.Observacion = reader["Observacion"] != DBNull.Value ? reader["Observacion"].ToString() : "";

                                propuestas.Add(propuesta);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show("Error en Propuesta: " + ex.Message + "\n" + ex.StackTrace, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return propuestas;
        }

        public Propuesta? GetPropuesta(int NumeroPoliza)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM PROPUESTA WHERE NumeroPoliza=@NumeroPoliza";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("@NumeroPoliza", NumeroPoliza);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Propuesta propuesta = new Propuesta();

                                propuesta.ID = int.Parse(reader.IsDBNull(reader.GetOrdinal("ID")) ? null : reader.GetString(0));
                                propuesta.NumeroPoliza = NumeroPoliza;
                                propuesta.RenuevaPoliza = int.Parse(reader.IsDBNull(reader.GetOrdinal("RenuevaPoliza")) ? null : reader.GetString(2));
                                propuesta.FechaRecepcion = DateTime.Parse(reader.IsDBNull(reader.GetOrdinal("FechaRecepcion")) ? null : reader.GetString(3));
                                propuesta.TipoPoliza = reader.IsDBNull(reader.GetOrdinal("TipoPoliza")) ? null : reader.GetString(4);
                                propuesta.FechaIngreso = DateTime.Parse(reader.IsDBNull(reader.GetOrdinal("FechaIngreso")) ? null : reader.GetString(5));
                                propuesta.FechaEmision = DateTime.Parse(reader.IsDBNull(reader.GetOrdinal("FechaEmision")) ? null : reader.GetString(6));
                                propuesta.IDRamo = int.Parse(reader.IsDBNull(reader.GetOrdinal("IDRamo")) ? null : reader.GetString(7));

                                return propuesta;
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
    }
}

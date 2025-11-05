using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seguros_Broker.Modelo;


namespace Seguros_Broker.Repositorio
{
    public class EjecutivoRep
    {
        //Reemplazar connectionString 
        private readonly string connectionString = "Data Source=DESKTOP-BJTLMA3;Initial Catalog=brokerBD;Integrated Security=True;Trust Server Certificate=True";

        public List<EjecutivoM> GetEjecutivos()
        {
            var ejecutivos = new List<EjecutivoM>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Codigo, TipoID, ID, Nombre, APaterno, AMaterno, Fono, Celular, Mail, Comision, Nick, Perfil, Estado, Restricciones FROM Ejecutivo ORDER BY ID DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var ejecutivo = new EjecutivoM();

                                
                                ejecutivo.codigo = reader["Codigo"] != DBNull.Value ? Convert.ToInt32(reader["Codigo"]) : 0;
                                ejecutivo.tipoId = reader["TipoID"] != DBNull.Value ? Convert.ToString(reader["TipoID"]) : "";
                                ejecutivo.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                ejecutivo.nombre = reader["Nombre"] != DBNull.Value ? Convert.ToString(reader["Nombre"]) : "";
                                ejecutivo.aPaterno = reader["APaterno"] != DBNull.Value ? Convert.ToString(reader["APaterno"]) : "";
                                ejecutivo.aMaterno = reader["AMaterno"] != DBNull.Value ? Convert.ToString(reader["AMaterno"]) : "";
                                ejecutivo.fono = reader["Fono"] != DBNull.Value ? Convert.ToInt32(reader["Fono"]) : 0;
                                ejecutivo.celular = reader["Celular"] != DBNull.Value ? Convert.ToInt32(reader["Celular"]) : 0;
                                ejecutivo.mail = reader["Mail"] != DBNull.Value ? Convert.ToString(reader["Mail"]) : "";
                                ejecutivo.comision = reader["Comision"] != DBNull.Value ? Convert.ToInt32(reader["Comision"]) : 0;
                                ejecutivo.nick = reader["Nick"] != DBNull.Value ? Convert.ToString(reader["Nick"]) : "";
                                ejecutivo.perfil = reader["Perfil"] != DBNull.Value ? Convert.ToString(reader["Perfil"]) : "";
                                ejecutivo.estado = reader["Estado"] != DBNull.Value ? Convert.ToString(reader["Estado"]) : "";
                                ejecutivo.restricciones = reader["Restricciones"] != DBNull.Value ? Convert.ToString(reader["Restricciones"]) : "";

                                ejecutivos.Add(ejecutivo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                System.Windows.MessageBox.Show("Error en GetEjecutivos: " + ex.Message + "\n" + ex.StackTrace, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                
            }

            return ejecutivos;
        }

        public EjecutivoM? GetEjecutivo(int ID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Ejecutivo WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                EjecutivoM ejecutivo = new EjecutivoM();
                                ejecutivo.codigo = reader.GetInt32(0);
                                ejecutivo.tipoId = reader.GetString(1);
                                ejecutivo.ID = reader.GetInt32(2);
                                ejecutivo.nombre = reader.GetString(3);
                                ejecutivo.aPaterno = reader.GetString(4);
                                ejecutivo.aMaterno = reader.GetString(5);
                                ejecutivo.fono = reader.GetInt32(6);
                                ejecutivo.celular = reader.GetInt32(7);
                                ejecutivo.mail = reader.GetString(8);
                                ejecutivo.comision = reader.GetInt32(9);
                                ejecutivo.nick = reader.GetString(10);
                                ejecutivo.perfil = reader.GetString(11);
                                ejecutivo.estado = reader.GetString(12);
                                ejecutivo.restricciones = reader.GetString(13);

                                return ejecutivo;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.ToString());
            }



            return null;
        }

        public async Task<(bool success, string? errorMessage)> CreateEjecutivoAsync(EjecutivoM ejecutivo)
        {
            if (ejecutivo == null)
                return (false, "Ejecutivo nulo.");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // 1) Verificar existencia por codigo o ID
                    const string checkSql = "SELECT COUNT(1) FROM Ejecutivo WHERE codigo = @codigo OR ID = @ID";
                    using (var checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.Add("@codigo", System.Data.SqlDbType.Int).Value = ejecutivo.codigo;
                        checkCmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = ejecutivo.ID;

                        var exists = (int)await checkCmd.ExecuteScalarAsync();
                        if (exists > 0)
                        {
                            return (false, "Ya existe un Ejecutivo con el mismo Código o Identificación.");
                        }
                    }

                    // 2) Insert dentro de una transacción
                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"
                    INSERT INTO Ejecutivo
                      (codigo, tipoId, ID, nombre, aPaterno, aMaterno, fono, celular, mail, comision, nick, perfil, estado, restricciones)
                    VALUES
                      (@codigo, @tipoId, @ID, @nombre, @aPaterno, @aMaterno, @fono, @celular, @mail, @comision, @nick, @perfil, @estado, @restricciones);";

                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {
                            // Parámetros con tipos (ajusta longitudes si tu BD las requiere)
                            cmd.Parameters.Add("@codigo", System.Data.SqlDbType.Int).Value = ejecutivo.codigo;
                            cmd.Parameters.Add("@tipoId", System.Data.SqlDbType.NVarChar, 50).Value = (object)ejecutivo.tipoId ?? DBNull.Value;
                            cmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = ejecutivo.ID;
                            cmd.Parameters.Add("@nombre", System.Data.SqlDbType.NVarChar, 200).Value = (object)ejecutivo.nombre ?? DBNull.Value;
                            cmd.Parameters.Add("@aPaterno", System.Data.SqlDbType.NVarChar, 100).Value = (object)ejecutivo.aPaterno ?? DBNull.Value;
                            cmd.Parameters.Add("@aMaterno", System.Data.SqlDbType.NVarChar, 100).Value = (object)ejecutivo.aMaterno ?? DBNull.Value;        
                            cmd.Parameters.Add("@fono", System.Data.SqlDbType.Int).Value = ejecutivo.fono;
                            cmd.Parameters.Add("@celular", System.Data.SqlDbType.Int).Value = ejecutivo.celular;
                            cmd.Parameters.Add("@mail", System.Data.SqlDbType.NVarChar, 200).Value = (object)ejecutivo.mail ?? DBNull.Value;
                            cmd.Parameters.Add("@comision", System.Data.SqlDbType.Int).Value = ejecutivo.comision;
                            cmd.Parameters.Add("@nick", System.Data.SqlDbType.NVarChar, 100).Value = (object)ejecutivo.nick ?? DBNull.Value;
                            cmd.Parameters.Add("@perfil", System.Data.SqlDbType.NVarChar, 100).Value = (object)ejecutivo.perfil ?? DBNull.Value;
                            cmd.Parameters.Add("@estado", System.Data.SqlDbType.NVarChar, 50).Value = (object)ejecutivo.estado ?? DBNull.Value;
                            cmd.Parameters.Add("@restricciones", System.Data.SqlDbType.NVarChar, 200).Value = (object)ejecutivo.restricciones ?? DBNull.Value;

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

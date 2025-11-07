using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Seguros_Broker.Repositorio
{
    public class CompaniaRep
    {
        private readonly string connectionString = "Data Source=DESKTOP-HT63IU9;Initial Catalog=brokerBD;Integrated Security=True;Trust Server Certificate=True";

        public List<Compania> GetCompanias()
        {
            var companias = new List<Compania>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT TipoID, ID, Nombre, Grupo, Fono, Pagina_Web, Pais, Ciudad, Region, Comuna, Direccion FROM COMPANIA ORDER BY ID DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var compania = new Compania();

                                compania.tipoID = reader["TipoID"] != DBNull.Value ? Convert.ToString(reader["TipoID"]) : "";
                                compania.ID = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : "";
                                compania.nombre = reader["Nombre"] != DBNull.Value ? Convert.ToString(reader["Nombre"]) : "";
                                compania.grupo = reader["Grupo"] != DBNull.Value ? Convert.ToString(reader["Grupo"]) : "";
                                compania.fono = reader["Fono"] != DBNull.Value ? Convert.ToInt32(reader["Fono"]) : 0;
                                compania.paginaWeb = reader["Pagina_Web"] != DBNull.Value ? Convert.ToString(reader["Pagina_Web"]) : "";
                                compania.pais = reader["Pais"] != DBNull.Value ? Convert.ToString(reader["Pais"]) : "";
                                compania.ciudad = reader["Ciudad"] != DBNull.Value ? Convert.ToString(reader["Ciudad"]) : "";
                                compania.region = reader["Region"] != DBNull.Value ? Convert.ToString(reader["Region"]) : "";
                                compania.comuna = reader["Comuna"] != DBNull.Value ? Convert.ToString(reader["Comuna"]) : "";
                                compania.direccion = reader["Direccion"] != DBNull.Value ? Convert.ToString(reader["Direccion"]) : "";

                                companias.Add(compania);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show("Error en Compañía: " + ex.Message + "\n" + ex.StackTrace, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return companias;
        }

        public async Task<(bool success, string? errorMessage)> CreateCompaniaAsync(Compania compania)
        {
            if (compania == null)
                return (false, "Ejecutivo nulo.");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    const string checkSql = "SELECT COUNT(1) FROM COMPANIA WHERE ID = @ID";
                    using (var checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.Add("@ID", System.Data.SqlDbType.NVarChar).Value = compania.ID;

                        var exists = (int)await checkCmd.ExecuteScalarAsync();
                        if(exists > 0)
                        {
                            return (false, "Ya existe una Compañía con el mismo ID");
                        }
                    }

                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"INSERT INTO COMPANIA
                                                    (TipoID, ID, Nombre, Grupo, Fono, Pagina_Web, Pais, Ciudad, Region, Comuna, Direccion)
                                                    VALUES
                                                    (@TipoID, @ID, @Nombre, @Grupo, @Fono, @Pagina_Web, @Pais, @Ciudad, @Region, @Comuna, @Direccion);";
                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {
                            cmd.Parameters.Add("@TipoId", System.Data.SqlDbType.NVarChar, 50).Value = (object)compania.tipoID ?? DBNull.Value;
                            cmd.Parameters.Add("@ID", System.Data.SqlDbType.NVarChar, 10).Value = (object)compania.ID ?? DBNull.Value;
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 50).Value = (object)compania.nombre ?? DBNull.Value;
                            cmd.Parameters.Add("@Grupo", System.Data.SqlDbType.NVarChar, 15).Value = (object)compania.grupo ?? DBNull.Value;
                            cmd.Parameters.Add("@Fono", System.Data.SqlDbType.Int).Value = compania.fono;
                            cmd.Parameters.Add("@Pagina_Web", System.Data.SqlDbType.NVarChar, 50).Value = (object)compania.paginaWeb ?? DBNull.Value;
                            cmd.Parameters.Add("@Pais", System.Data.SqlDbType.NVarChar, 15).Value = (object)compania.pais ?? DBNull.Value;
                            cmd.Parameters.Add("@Ciudad", System.Data.SqlDbType.NVarChar, 25).Value = (object)compania.ciudad ?? DBNull.Value;
                            cmd.Parameters.Add("@Region", System.Data.SqlDbType.NVarChar, 50).Value = (object)compania.region ?? DBNull.Value;
                            cmd.Parameters.Add("@Comuna", System.Data.SqlDbType.NVarChar, 25).Value = (object)compania.comuna ?? DBNull.Value;
                            cmd.Parameters.Add("@Direccion", System.Data.SqlDbType.NVarChar, 50).Value = (object)compania.direccion ?? DBNull.Value;

                            int rows = await cmd.ExecuteNonQueryAsync();
                            
                            if(rows <= 0)
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

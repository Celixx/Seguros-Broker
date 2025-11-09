using Microsoft.Data.SqlClient;
using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Seguros_Broker.Repositorio
{
    public class GrupoRep : Repositorio
    {
        public List<Grupo> GetGrupos()
        {
            var grupos = new List<Grupo>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT ID, Nombre FROM GRUPO ORDER BY ID ASC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var grupo = new Grupo();

                                grupo.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0; 
                                grupo.Nombre = reader["Nombre"] != DBNull.Value ? Convert.ToString(reader["Nombre"]) : "";                                

                                grupos.Add(grupo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show("Error en Grupos: " + ex.Message + "\n" + ex.StackTrace, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return grupos;
        }

        public async Task<(bool success, string? errorMessage)> CreateGrupoAsync(Grupo grupo)
        {
            if (grupo == null)
                return (false, "Grupo nulo.");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    const string checkSql = "SELECT COUNT(1) FROM GRUPO WHERE Nombre = @Nombre";
                    using (var checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar).Value = grupo.Nombre;

                        var exists = (int)await checkCmd.ExecuteScalarAsync();
                        if (exists > 0)
                        {
                            return (false, "Ya existe una Grupo con el mismo Nombre");
                        }
                    }

                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"INSERT INTO GRUPO
                                                    (Nombre)
                                                    VALUES
                                                    (@Nombre);";
                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 50).Value = (object)grupo.Nombre ?? DBNull.Value;

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

        public Grupo? GetGrupo(int ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM GRUPO WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("@ID", ID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Grupo grupo = new Grupo();

                                grupo.Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(1);                                

                                return grupo;
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

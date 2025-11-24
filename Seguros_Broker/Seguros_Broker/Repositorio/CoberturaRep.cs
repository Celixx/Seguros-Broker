using Microsoft.Data.SqlClient;
using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Seguros_Broker.Repositorio
{
    public class CoberturaRep : Repositorio
    {
        public List<Cobertura> GetCoberturas()
        {
            var list = new List<Cobertura>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT Codigo, Nombre, AfectaExenta, SumaAlMonto, Monto, Prima FROM COBERTURA ORDER BY Codigo ASC";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var c = new Cobertura();
                            c.codigo = reader["Codigo"] != DBNull.Value ? reader["Codigo"].ToString() : "";
                            c.nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "";
                            c.afectaExtenta = reader["AfectaExenta"] != DBNull.Value ? reader["AfectaExenta"].ToString() : "";
                            c.sumaMonto = reader["SumaAlMonto"] != DBNull.Value ? reader["SumaAlMonto"].ToString() : "";
                            c.monto = reader["Monto"] != DBNull.Value ? Convert.ToInt32(reader["Monto"]) : 0;
                            c.prima = reader["Prima"] != DBNull.Value ? Convert.ToInt32(reader["Prima"]) : 0;
                            list.Add(c);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en Coberturas: " + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return list;
        }

        // get de codigos de cobertura asociados a un producto
        public List<string> GetCoberturasByProducto(int productoId)
        {
            var list = new List<string>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();  
                    string sql = "SELECT CoberturaCodigo FROM PRODUCTO_COBERTURA WHERE ProductoID = @ProductoID";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@ProductoID", productoId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(reader["CoberturaCodigo"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer coberturas del producto: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return list;
        }
     
        public async Task<(bool success, string? errorMessage)> AssignCoberturasToProducto(int productoId, List<string> codigos)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var tran = connection.BeginTransaction())
                    {
                        // eliminar asociaciones previas
                        const string deleteSql = "DELETE FROM PRODUCTO_COBERTURA WHERE ProductoID = @ProductoID";
                        using (var delCmd = new SqlCommand(deleteSql, connection, tran))
                        {
                            delCmd.Parameters.AddWithValue("@ProductoID", productoId);
                            await delCmd.ExecuteNonQueryAsync();
                        }

                        // insertar nuevas asociaciones
                        if (codigos != null && codigos.Count > 0)
                        {
                            const string insertSql = "INSERT INTO PRODUCTO_COBERTURA (ProductoID, CoberturaCodigo) VALUES (@ProductoID, @CoberturaCodigo)";
                            using (var insCmd = new SqlCommand(insertSql, connection, tran))
                            {
                                insCmd.Parameters.Add("@ProductoID", System.Data.SqlDbType.Int).Value = productoId;
                                insCmd.Parameters.Add("@CoberturaCodigo", System.Data.SqlDbType.NVarChar, 15);

                                foreach (var codigo in codigos)
                                {
                                    insCmd.Parameters["@CoberturaCodigo"].Value = codigo.Trim();
                                    await insCmd.ExecuteNonQueryAsync();
                                }
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

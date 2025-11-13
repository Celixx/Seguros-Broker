using Microsoft.Data.SqlClient;
using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Seguros_Broker.Repositorio
{
    public class ProductoRep : Repositorio
    {
        public List<Producto> GetProductos()
        {
            var productos = new List<Producto>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT P.ID,
                                          P.Nombre AS ProductoNombre,
                                          P.RamoID,
                                          R.Nombre AS RamoNombre,
                                          P.CompaniaID,
                                          C.Nombre AS CompaniaNombre
                                   FROM PRODUCTO P
                                   LEFT JOIN RAMO R ON P.RamoID = R.ID
                                   LEFT JOIN COMPANIA C ON P.CompaniaID = C.ID
                                   ORDER BY P.ID DESC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var p = new Producto();

                            p.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                            p.nombre = reader["ProductoNombre"] != DBNull.Value ? reader["ProductoNombre"].ToString() : "";
                            p.ramoID = reader["RamoID"] != DBNull.Value ? Convert.ToInt32(reader["RamoID"]) : 0;
                            p.companiaID = reader["CompaniaID"] != DBNull.Value ? reader["CompaniaID"].ToString() : "";

                            productos.Add(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en Productos: " + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return productos;
        }

        public async Task<(bool success, string? errorMessage, int insertedId)> CreateProductoAsync(Producto producto)
        {
            if (producto == null) return (false, "Producto nulo.", 0);

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var tran = connection.BeginTransaction())
                    {
                        const string insertSql = @"INSERT INTO PRODUCTO (Nombre, RamoID, CompaniaID)
                                           VALUES (@Nombre, @RamoID, @CompaniaID);
                                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        using (var cmd = new SqlCommand(insertSql, connection, tran))
                        {
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 30).Value = (object)producto.nombre ?? DBNull.Value;
                            cmd.Parameters.Add("@RamoID", System.Data.SqlDbType.Int).Value = producto.ramoID;
                            cmd.Parameters.Add("@CompaniaID", System.Data.SqlDbType.NVarChar, 10).Value = (object)producto.companiaID ?? DBNull.Value;

                            var result = await cmd.ExecuteScalarAsync();
                            if (result == null)
                            {
                                await tran.RollbackAsync();
                                return (false, "No se insertó el registro.", 0);
                            }

                            int newId = Convert.ToInt32(result);

                            await tran.CommitAsync();
                            return (true, null, newId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, 0);
            }
        }

        public List<ProductoCoberturaView> GetProductoCoberturas(int productoId)
        {
            var list = new List<ProductoCoberturaView>();

            if (productoId <= 0) return list;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
                SELECT P.ID AS ProductoID,
                       P.Nombre AS ProductoNombre,
                       C.Codigo,
                       C.Nombre AS CoberturaNombre,
                       C.AfectaExenta,
                       C.SumaAlMonto,
                       C.Monto,
                       C.Prima
                FROM PRODUCTO_COBERTURA PC
                INNER JOIN PRODUCTO P ON PC.ProductoID = P.ID
                INNER JOIN COBERTURA C ON PC.CoberturaCodigo = C.Codigo
                WHERE PC.ProductoID = @ProductoID
                ORDER BY C.Codigo ASC;";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@ProductoID", productoId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var v = new ProductoCoberturaView
                                {
                                    ProductoID = reader["ProductoID"] != DBNull.Value ? Convert.ToInt32(reader["ProductoID"]) : 0,
                                    ProductoNombre = reader["ProductoNombre"] != DBNull.Value ? reader["ProductoNombre"].ToString() : "",
                                    codigo = reader["Codigo"] != DBNull.Value ? reader["Codigo"].ToString() : "",
                                    nombre = reader["CoberturaNombre"] != DBNull.Value ? reader["CoberturaNombre"].ToString() : "",
                                    afectaExtenta = reader["AfectaExenta"] != DBNull.Value ? reader["AfectaExenta"].ToString() : "",
                                    sumaMonto = reader["SumaAlMonto"] != DBNull.Value ? reader["SumaAlMonto"].ToString() : "",
                                    monto = reader["Monto"] != DBNull.Value ? Convert.ToInt32(reader["Monto"]) : 0,
                                    prima = reader["Prima"] != DBNull.Value ? Convert.ToInt32(reader["Prima"]) : 0
                                };
                                list.Add(v);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al cargar coberturas del producto: " + ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return list;
        }

        public Producto? GetProducto(int ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM PRODUCTO WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var p = new Producto();
                                p.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                p.nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "";
                                p.ramoID = reader["RamoID"] != DBNull.Value ? Convert.ToInt32(reader["RamoID"]) : 0;
                                p.companiaID = reader["CompaniaID"] != DBNull.Value ? reader["CompaniaID"].ToString() : "";
                                return p;
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

        public async Task<(bool success, string? errorMessage)> UpdateProductoAsync(Producto producto)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var tran = connection.BeginTransaction())
                    {
                        const string updateSql = @"UPDATE PRODUCTO
                                                   SET Nombre=@Nombre, RamoID=@RamoID, CompaniaID=@CompaniaID
                                                   WHERE ID=@ID;";

                        using (var cmd = new SqlCommand(updateSql, connection, tran))
                        {
                            cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 30).Value = (object)producto.nombre ?? DBNull.Value;
                            cmd.Parameters.Add("@RamoID", System.Data.SqlDbType.Int).Value = producto.ramoID;
                            cmd.Parameters.Add("@CompaniaID", System.Data.SqlDbType.NVarChar, 10).Value = (object)producto.companiaID ?? DBNull.Value;
                            cmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = producto.ID;

                            int rows = await cmd.ExecuteNonQueryAsync();

                            if (rows <= 0)
                            {
                                await tran.RollbackAsync();
                                return (false, "No se actualizó el registro (0 filas afectadas).");
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

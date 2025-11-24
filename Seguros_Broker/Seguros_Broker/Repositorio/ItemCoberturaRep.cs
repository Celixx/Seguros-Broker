using Seguros_Broker.Modelo;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; 
using System.IO;                       

namespace Seguros_Broker.Repositorio
{
    public class ItemCoberturaRep
    {
        
        private readonly string ConnectionString;

        public ItemCoberturaRep()
        {
            try
            {

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();

                ConnectionString = configuration.GetSection("Settings:connectionString").Value;

                if (string.IsNullOrEmpty(ConnectionString))
                {
                    throw new InvalidOperationException("La cadena de conexión 'Settings:connectionString' no se encontró o está vacía en appsettings.json.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al inicializar la cadena de conexión en ItemCoberturaRep. Verifique appsettings.json y NuGet Packages.", ex);
            }
        }

        // el metodo de guardado
        public async Task<(bool success, string errorMsg)> CreateItemCoberturasAsync(List<ItemCobertura> coberturas)
        {

            using (var conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // insert en sql
                    string sql = @"
                        INSERT INTO ItemCobertura (
                            IdPropuesta, IdItem, CodCobertura, afectaExenta, 
                            SumaAlMonto, Monto, Prima
                        ) 
                        VALUES (
                            @IdPropuesta, @IdItem, @CodCobertura, @AfectaExenta, 
                            @SumaAlMonto, @Monto, @Prima
                        )";

                    foreach (var itemCob in coberturas)
                    {
                        using (var cmd = new SqlCommand(sql, conn, transaction))
                        {

                            cmd.Parameters.AddWithValue("@IdPropuesta", itemCob.IdPropuesta);
                            cmd.Parameters.AddWithValue("@IdItem", itemCob.IdItem);
                            cmd.Parameters.AddWithValue("@CodCobertura", itemCob.CodCobertura);
                            cmd.Parameters.AddWithValue("@AfectaExenta", (object)itemCob.AfectaExenta ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@SumaAlMonto", (object)itemCob.SumaAlMonto ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Monto", itemCob.Monto);
                            cmd.Parameters.AddWithValue("@Prima", itemCob.Prima);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    transaction.Commit();
                    return (true, null); 
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return (false, ex.Message); 
                }
            }
        }

        public async Task<List<ItemResumenCobertura>> GetResumenItemsPorPropuestaAsync(int idPropuesta)
        {
            var resumen = new List<ItemResumenCobertura>();

            string sql = @"
        SELECT 
            T1.IdItem,
            MAX(T1.Prima) as PrimaNeta 
        FROM ItemCobertura AS T1
        WHERE T1.IdPropuesta = @IdPropuesta
        GROUP BY T1.IdItem";

            using (var conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IdPropuesta", idPropuesta);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            resumen.Add(new ItemResumenCobertura
                            {

                                Numero = reader.GetInt32(reader.GetOrdinal("IdItem")),
                                PrimaNeta = reader.GetDecimal(reader.GetOrdinal("PrimaNeta"))
                            });
                        }
                    }
                }
            }
            return resumen;
        }
    }
}
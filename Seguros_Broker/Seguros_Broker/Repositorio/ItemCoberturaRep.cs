using Seguros_Broker.Modelo;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // Necesario para leer appsettings
using System.IO;                       // Necesario para I/O

namespace Seguros_Broker.Repositorio
{
    public class ItemCoberturaRep
    {
        // Variable para almacenar la cadena de conexión
        private readonly string ConnectionString;

        // Constructor para leer la cadena de conexión
        public ItemCoberturaRep()
        {
            try
            {
                // Construye la configuración para encontrar appsettings.json
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();

                // Lee la cadena de conexión desde la sección "Settings:connectionString"
                ConnectionString = configuration.GetSection("Settings:connectionString").Value;

                if (string.IsNullOrEmpty(ConnectionString))
                {
                    throw new InvalidOperationException("La cadena de conexión 'Settings:connectionString' no se encontró o está vacía en appsettings.json.");
                }
            }
            catch (Exception ex)
            {
                // Maneja el error si no se puede cargar la configuración
                throw new InvalidOperationException("Error al inicializar la cadena de conexión en ItemCoberturaRep. Verifique appsettings.json y NuGet Packages.", ex);
            }
        }

        // El método de guardado
        public async Task<(bool success, string errorMsg)> CreateItemCoberturasAsync(List<ItemCobertura> coberturas)
        {
            // Usamos una transacción para asegurar que todas se inserten o ninguna
            using (var conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Consulta SQL de inserción
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
                            // Parámetros
                            cmd.Parameters.AddWithValue("@IdPropuesta", itemCob.IdPropuesta);
                            cmd.Parameters.AddWithValue("@IdItem", itemCob.IdItem);
                            cmd.Parameters.AddWithValue("@CodCobertura", itemCob.CodCobertura);

                            // Aseguramos que los valores sean nulos si son null para SQL, o un string vacío. 
                            // Dado que en SQL los definiste como VARCHAR(20), es seguro pasar string.Empty.
                            cmd.Parameters.AddWithValue("@AfectaExenta", (object)itemCob.AfectaExenta ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@SumaAlMonto", (object)itemCob.SumaAlMonto ?? DBNull.Value);

                            cmd.Parameters.AddWithValue("@Monto", itemCob.Monto);
                            cmd.Parameters.AddWithValue("@Prima", itemCob.Prima);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    transaction.Commit();
                    return (true, null); // Éxito
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return (false, ex.Message); // Fallo
                }
            }
        }

        public async Task<List<ItemResumenCobertura>> GetResumenItemsPorPropuestaAsync(int idPropuesta)
        {
            var resumen = new List<ItemResumenCobertura>();

            // Consulta SQL: Agrupa por IdItem y usa MAX(Prima) para obtener la Prima Neta Total guardada
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
                                // Mapeo: IdItem a Numero
                                Numero = reader.GetInt32(reader.GetOrdinal("IdItem")),
                                // Mapeo: MAX(Prima) a PrimaNeta
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
using Seguros_Broker.Modelo;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration; // <-- NUEVO USING
using System.IO; // <-- NUEVO USING

namespace Seguros_Broker.Repositorio
{
    public class ItemRep
    {
        // 1. Cambia la constante por una variable de solo lectura (readonly)
        private readonly string ConnectionString;

        // 2. Agrega un constructor para inicializar la ConnectionString
        public ItemRep()
        {
            // Construye la configuración
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Busca appsettings en el directorio de ejecución
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            // Lee la cadena de conexión desde la sección "Settings"
            // Nota: El 'connectionString' de aquí debe coincidir con el del JSON
            ConnectionString = configuration.GetSection("Settings:connectionString").Value;

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'Settings:connectionString' no se encontró en appsettings.json.");
            }
        }

        // 3. Modifica tu método GetItemsByRut para que no sea estático si es que lo es,
        //    y utiliza la variable de instancia (ya la tienes bien definida).
        public List<Item> GetItemsByRut(string rut)
        {
            var items = new List<Item>();

            string sql = "SELECT * FROM Item WHERE RutCliente = @Rut";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Rut", rut);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Item
                        {
                            IdItem = reader["IdItem"] != DBNull.Value ? Convert.ToInt32(reader["IdItem"]) : 0,

                            MateriaAsegurada = reader["MateriaAsegurada"]?.ToString(),
                            Anno = reader["Anno"]?.ToString(),
                            Patente = reader["Patente"]?.ToString(),
                            MinutaItem = reader["MinutaItem"]?.ToString(),
                            Carroceria = reader["Carroceria"]?.ToString(),
                            Propietario = reader["Propietario"]?.ToString(),
                            Tipo = reader["Tipo"]?.ToString(),
                            NumeroMotor = reader["NumeroMotor"]?.ToString(),
                            Color = reader["Color"]?.ToString(),
                            Chasis = reader["Chasis"]?.ToString(),
                            ValorComercial = reader["ValorComercial"]?.ToString(),
                            Modelo = reader["Modelo"]?.ToString(),
                            NumeroChasis = reader["NumeroChasis"]?.ToString(),
                            Uso = reader["Uso"]?.ToString(),

                            FechaDesde = reader["FechaDesde"] != DBNull.Value
                                        ? (DateTime?)Convert.ToDateTime(reader["FechaDesde"])
                                        : null,

                            FechaHasta = reader["FechaHasta"] != DBNull.Value
                                        ? (DateTime?)Convert.ToDateTime(reader["FechaHasta"])
                                        : null
                        };

                        items.Add(item);
                    }
                }
            }

            return items;
        }

    }
}
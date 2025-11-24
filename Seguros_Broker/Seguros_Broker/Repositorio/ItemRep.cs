using Seguros_Broker.Modelo;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration; 
using System.IO; 

namespace Seguros_Broker.Repositorio
{

    public class ItemRep
    {
        private readonly string ConnectionString;

        public ItemRep()
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();


            ConnectionString = configuration.GetSection("Settings:connectionString").Value;

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'Settings:connectionString' no se encontró en appsettings.json.");
            }
        }


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
        // agrega esto dentro de la clase ItemRep
        public bool AgregarItem(Item item)
        {

            string sql = @"INSERT INTO Item (
                        RutCliente, MateriaAsegurada, Anno, Patente, MinutaItem, 
                        Carroceria, Propietario, Tipo, NumeroMotor, Color, 
                        Chasis, ValorComercial, Modelo, NumeroChasis, Uso, 
                        FechaDesde, FechaHasta
                    ) VALUES (
                        @RutCliente, @MateriaAsegurada, @Anno, @Patente, @MinutaItem, 
                        @Carroceria, @Propietario, @Tipo, @NumeroMotor, @Color, 
                        @Chasis, @ValorComercial, @Modelo, @NumeroChasis, @Uso, 
                        @FechaDesde, @FechaHasta
                    )";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {

                cmd.Parameters.AddWithValue("@RutCliente", item.RutCliente ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MateriaAsegurada", item.MateriaAsegurada ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Anno", item.Anno ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Patente", item.Patente ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MinutaItem", item.MinutaItem ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Carroceria", item.Carroceria ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Propietario", item.Propietario ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Tipo", item.Tipo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NumeroMotor", item.NumeroMotor ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Color", item.Color ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Chasis", item.Chasis ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ValorComercial", item.ValorComercial ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Modelo", item.Modelo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NumeroChasis", item.NumeroChasis ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Uso", item.Uso ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaDesde", item.FechaDesde ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaHasta", item.FechaHasta ?? (object)DBNull.Value);

                try
                {
                    conn.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al guardar el item: " + ex.Message);
                }
            }
        }
    }
}
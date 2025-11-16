using Microsoft.Data.SqlClient;
using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Seguros_Broker.Repositorio
{
    public class ClienteDatosPagoRep : Repositorio
    {
        public List<ClienteDatosPago> GetDatosPagoByClienteID(string clienteID)
        {
            var list = new List<ClienteDatosPago>();
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT ID, ClienteID, NumeroDocumento, NroTarjetaCuenta, TipoTarjeta, ValidezTarjeta, Banco
                                   FROM CLIENTE_DATOS_PAGO
                                   WHERE ClienteID = @ClienteID";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClienteID", clienteID ?? "");
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                list.Add(new ClienteDatosPago
                                {
                                    ID = rdr["ID"] != DBNull.Value ? Convert.ToInt32(rdr["ID"]) : 0,
                                    ClienteID = rdr["ClienteID"] != DBNull.Value ? rdr["ClienteID"].ToString() : "",
                                    NumeroDocumento = rdr["NumeroDocumento"] != DBNull.Value ? rdr["NumeroDocumento"].ToString() : "",
                                    NroTarjetaCuenta = rdr["NroTarjetaCuenta"] != DBNull.Value ? rdr["NroTarjetaCuenta"].ToString() : "",
                                    TipoTarjeta = rdr["TipoTarjeta"] != DBNull.Value ? rdr["TipoTarjeta"].ToString() : "",
                                    ValidezTarjeta = rdr["ValidezTarjeta"] != DBNull.Value ? rdr["ValidezTarjeta"].ToString() : "",
                                    Banco = rdr["Banco"] != DBNull.Value ? rdr["Banco"].ToString() : ""
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer datos de pago cliente: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return list;
        }
    }
}

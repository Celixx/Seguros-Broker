using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Seguros_Broker.Modelo;

namespace Seguros_Broker.Repositorio
{
    internal class SocioRep : Repositorio
    {
      public List<Socio> GetSocios() 
        {
            var socios = new List<Socio>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT TipoID, ID, Nombre, APaterno, AMaterno, Fono, Celular, Mail, Fax, Direccion, Observacion, Comision, PorcentajeComision FROM SOCIO ORDER BY ID DESC;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var socio = new Socio();

                                socio.tipoId = reader["TipoID"] != DBNull.Value ? reader["TipoID"].ToString() : "";
                                socio.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                socio.nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "";
                                socio.aPaterno = reader["APaterno"] != DBNull.Value ? reader["APaterno"].ToString() : "";
                                socio.aMaterno = reader["AMaterno"] != DBNull.Value ? reader["AMaterno"].ToString() : "";
                                socio.fono = reader["Fono"] != DBNull.Value ? Convert.ToInt32(reader["Fono"]) : 0;
                                socio.celular = reader["Celular"] != DBNull.Value ? Convert.ToInt32(reader["Celular"]) : 0;
                                socio.mail = reader["Mail"] != DBNull.Value ? reader["Mail"].ToString() : "";
                                socio.fax = reader["Fax"] != DBNull.Value ? Convert.ToInt32(reader["Fax"]) : 0;
                                socio.direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "";
                                socio.observacion = reader["Observacion"] != DBNull.Value ? reader["Observacion"].ToString() : "";
                                socio.comision = reader["Comision"] != DBNull.Value ? Convert.ToInt32(reader["Comision"]) : 0;
                                socio.porcentajeComision = reader["PorcentajeComision"] != DBNull.Value ? Convert.ToInt32(reader["PorcentajeComision"]) : 0;

                                socios.Add(socio);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show("Error en Compañía: " + ex.Message + "\n" + ex.StackTrace, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return socios;
        }
    }
}

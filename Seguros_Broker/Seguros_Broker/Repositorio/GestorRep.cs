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
    class GestorRep : Repositorio
    {
        public Gestor? GetGestor(int ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM GESTOR WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("@ID", ID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Gestor gestor = new Gestor();

                                gestor.tipoId = reader.IsDBNull(0) ? null : reader.GetString(0);
                                gestor.ID = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                gestor.nombre = reader.IsDBNull(2) ? null : reader.GetString(2);
                                gestor.aPaterno = reader.IsDBNull(3) ? null : reader.GetString(3);
                                gestor.aMaterno = reader.IsDBNull(4) ? null : reader.GetString(4);
                                gestor.fono = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                                gestor.celular = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                                gestor.mail = reader.IsDBNull(7) ? null : reader.GetString(7);
                                gestor.fax = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                                gestor.direccion = reader.IsDBNull(9) ? null : reader.GetString(9);
                                gestor.observacion = reader.IsDBNull(10) ? null : reader.GetString(10);
                                gestor.comision = reader.IsDBNull(11) ? 0 : reader.GetInt32(11);
                                gestor.porcentajeComision = reader.IsDBNull(12) ? 0 : reader.GetInt32(12);

                                return gestor;
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

        public List<Gestor> GetGestores()
        {
            var gestores = new List<Gestor>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT TipoID, ID, Nombre, APaterno, AMaterno, Fono, Celular, Mail, Fax, Direccion, Observacion, Comision, PorcentajeComision FROM GESTOR ORDER BY ID DESC;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var gestor = new Gestor();

                                gestor.tipoId = reader["TipoID"] != DBNull.Value ? reader["TipoID"].ToString() : "";
                                gestor.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                gestor.nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "";
                                gestor.aPaterno = reader["APaterno"] != DBNull.Value ? reader["APaterno"].ToString() : "";
                                gestor.aMaterno = reader["AMaterno"] != DBNull.Value ? reader["AMaterno"].ToString() : "";
                                gestor.fono = reader["Fono"] != DBNull.Value ? Convert.ToInt32(reader["Fono"]) : 0;
                                gestor.celular = reader["Celular"] != DBNull.Value ? Convert.ToInt32(reader["Celular"]) : 0;
                                gestor.mail = reader["Mail"] != DBNull.Value ? reader["Mail"].ToString() : "";
                                gestor.fax = reader["Fax"] != DBNull.Value ? Convert.ToInt32(reader["Fax"]) : 0;
                                gestor.direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "";
                                gestor.observacion = reader["Observacion"] != DBNull.Value ? reader["Observacion"].ToString() : "";
                                gestor.comision = reader["Comision"] != DBNull.Value ? Convert.ToInt32(reader["Comision"]) : 0;
                                gestor.porcentajeComision = reader["PorcentajeComision"] != DBNull.Value ? Convert.ToInt32(reader["PorcentajeComision"]) : 0;

                                gestores.Add(gestor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show("Error en Compañía: " + ex.Message + "\n" + ex.StackTrace, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return gestores;
        }
    }
}

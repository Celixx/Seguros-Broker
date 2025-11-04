using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seguros_Broker.Modelo;


namespace Seguros_Broker.Repositorio
{
    public class EjecutivoRep
    {

        private readonly string connectionString = "Data Source=DESKTOP-BJTLMA3;Initial Catalog=brokerBD;Integrated Security=True;Trust Server Certificate=True";

        public List<EjecutivoM> GetEjecutivos()
        {
            var ejecutivos = new List<EjecutivoM>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Codigo, TipoID, ID, Nombre, APaterno, AMaterno, Fono, Celular, Mail, Comision, Nick, Perfil, Estado, Restricciones FROM Ejecutivo ORDER BY ID DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var ejecutivo = new EjecutivoM();

                                
                                ejecutivo.codigo = reader["Codigo"] != DBNull.Value ? Convert.ToInt32(reader["Codigo"]) : 0;
                                ejecutivo.tipoId = reader["TipoID"] != DBNull.Value ? Convert.ToString(reader["TipoID"]) : "";
                                ejecutivo.ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                ejecutivo.nombre = reader["Nombre"] != DBNull.Value ? Convert.ToString(reader["Nombre"]) : "";
                                ejecutivo.aPaterno = reader["APaterno"] != DBNull.Value ? Convert.ToString(reader["APaterno"]) : "";
                                ejecutivo.aMaterno = reader["AMaterno"] != DBNull.Value ? Convert.ToString(reader["AMaterno"]) : "";
                                ejecutivo.fono = reader["Fono"] != DBNull.Value ? Convert.ToInt32(reader["Fono"]) : 0;
                                ejecutivo.celular = reader["Celular"] != DBNull.Value ? Convert.ToInt32(reader["Celular"]) : 0;
                                ejecutivo.mail = reader["Mail"] != DBNull.Value ? Convert.ToString(reader["Mail"]) : "";
                                ejecutivo.comision = reader["Comision"] != DBNull.Value ? Convert.ToInt32(reader["Comision"]) : 0;
                                ejecutivo.nick = reader["Nick"] != DBNull.Value ? Convert.ToString(reader["Nick"]) : "";
                                ejecutivo.perfil = reader["Perfil"] != DBNull.Value ? Convert.ToString(reader["Perfil"]) : "";
                                ejecutivo.estado = reader["Estado"] != DBNull.Value ? Convert.ToString(reader["Estado"]) : "";
                                ejecutivo.restricciones = reader["Restricciones"] != DBNull.Value ? Convert.ToString(reader["Restricciones"]) : "";

                                ejecutivos.Add(ejecutivo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                System.Windows.MessageBox.Show("Error en GetEjecutivos: " + ex.Message + "\n" + ex.StackTrace, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                
            }

            return ejecutivos;
        }



        public EjecutivoM? GetEjecutivo(int ID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Ejecutivo WHERE ID=@ID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                EjecutivoM ejecutivo = new EjecutivoM();
                                ejecutivo.codigo = reader.GetInt32(0);
                                ejecutivo.tipoId = reader.GetString(1);
                                ejecutivo.ID = reader.GetInt32(2);
                                ejecutivo.nombre = reader.GetString(3);
                                ejecutivo.aPaterno = reader.GetString(4);
                                ejecutivo.aMaterno = reader.GetString(5);
                                ejecutivo.fono = reader.GetInt32(6);
                                ejecutivo.celular = reader.GetInt32(7);
                                ejecutivo.mail = reader.GetString(8);
                                ejecutivo.comision = reader.GetInt32(9);
                                ejecutivo.nick = reader.GetString(10);
                                ejecutivo.perfil = reader.GetString(11);
                                ejecutivo.estado = reader.GetString(12);
                                ejecutivo.restricciones = reader.GetString(13);

                                return ejecutivo;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.ToString());
            }



            return null;
        }

        public void CreateEjecutivo(EjecutivoM ejecutivo)
        {

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Ejecutivo " +
                                 "(codigo, tipoId, ID, nombre, aPaterno, aMaterno, fono, celular, mail, comision, nick, perfil, estado, restricciones) VALUES " +
                                 "(@codigo, @tipoId, @ID, @nombre, @aPaterno, @aMaterno, @fono, @celular, @mail, @comision, @nick, @perfil, @estado, @restricciones); ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@codigo", ejecutivo.codigo);
                        command.Parameters.AddWithValue("@tipoId", ejecutivo.tipoId);
                        command.Parameters.AddWithValue("@ID", ejecutivo.ID);
                        command.Parameters.AddWithValue("@nombre", ejecutivo.nombre);
                        command.Parameters.AddWithValue("@aPaterno", ejecutivo.aPaterno);
                        command.Parameters.AddWithValue("@aMaterno", ejecutivo.aMaterno);
                        command.Parameters.AddWithValue("@fono", ejecutivo.fono);
                        command.Parameters.AddWithValue("@celular", ejecutivo.celular);
                        command.Parameters.AddWithValue("@mail", ejecutivo.mail);
                        command.Parameters.AddWithValue("@comision", ejecutivo.comision);
                        command.Parameters.AddWithValue("@nick", ejecutivo.nick);
                        command.Parameters.AddWithValue("@perfil", ejecutivo.perfil);
                        command.Parameters.AddWithValue("@estado", ejecutivo.estado);
                        command.Parameters.AddWithValue("@restricciones", ejecutivo.restricciones);

                        command.ExecuteNonQuery();
                    }

                }

                

            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.ToString());
            }

        }


    }
}

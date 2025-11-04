using Seguros_Broker.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Seguros_Broker
{

    public partial class Ejecutivo : Window
    {
        

        public Ejecutivo()
        {
            InitializeComponent();

            ReadEjecutivo();
            
        }

        private void ReadEjecutivo()
        {
            var repo = new EjecutivoRep();
            var ejecutivos = repo.GetEjecutivos();

            MessageBox.Show($"Ejecutivos obtenidos: {ejecutivos?.Count ?? 0}");
            this.dataGridEjecutivo.ItemsSource = ejecutivos;
        }



        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

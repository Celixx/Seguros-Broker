using System;
using System.Collections.Generic;
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
using Seguros_Broker.Repositorio; 
using Seguros_Broker.Modelo;      
using System.Collections.ObjectModel;

namespace Seguros_Broker
{
    public partial class VentanaAgregarCobertura : Window
    {
        // Declarar la lista
        public ObservableCollection<Cobertura> ListaCoberturas { get; set; }

        public VentanaAgregarCobertura()
        {
            InitializeComponent();

            // Inicializar la lista
            ListaCoberturas = new ObservableCollection<Cobertura>();

            // Conectar el DataGrid a la lista
            CoberturasDataGrid.ItemsSource = ListaCoberturas;
        }

        // El método que se llama al cargar la ventana
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        
        private void CargarDatos()
        {
            // Limpiar la lista por si acaso
            ListaCoberturas.Clear();

            // Crear una instancia de tu repositorio
            CoberturaRep rep = new CoberturaRep();

            // Obtener los datos usando tu método
            var coberturasDesdeDb = rep.GetCoberturas();

            // Añadir los datos a la lista que ve el DataGrid
            foreach (var cob in coberturasDesdeDb)
            {
                ListaCoberturas.Add(cob);
            }

            
        }
    }
}
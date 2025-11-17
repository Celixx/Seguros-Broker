using System;
using System.Windows;
using System.Collections.ObjectModel;
using Seguros_Broker.Repositorio;
using Seguros_Broker.Modelo;
using System.Collections.Generic; // <-- AÑADIR ESTE
using System.Linq; // <-- AÑADIR ESTE

namespace Seguros_Broker
{
    public partial class VentanaAgregarCobertura : Window
    {
        public ObservableCollection<Cobertura> ListaCoberturas { get; set; }

        // 1. Propiedad PÚBLICA para guardar los resultados
        public List<Cobertura> CoberturasSeleccionadas { get; private set; }

        public VentanaAgregarCobertura()
        {
            InitializeComponent();

            ListaCoberturas = new ObservableCollection<Cobertura>();
            CoberturasDataGrid.ItemsSource = ListaCoberturas;

            // 2. Inicializar la lista de resultados
            CoberturasSeleccionadas = new List<Cobertura>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            ListaCoberturas.Clear();
            CoberturaRep rep = new CoberturaRep();
            var coberturasDesdeDb = rep.GetCoberturas();

            foreach (var cob in coberturasDesdeDb)
            {
                ListaCoberturas.Add(cob);
            }
        }

        // 3. Evento Click para el botón "aceptar"
        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            // Filtrar la lista principal y guardar solo los seleccionados
            CoberturasSeleccionadas = ListaCoberturas.Where(c => c.IsSelected).ToList();

            // 4. Marcar el resultado como "OK" (importante para ShowDialog)
            this.DialogResult = true;

            // 5. Cerrar esta ventana (no es necesario this.Close() 
            //    cuando DialogResult se establece)
        }
    }
}
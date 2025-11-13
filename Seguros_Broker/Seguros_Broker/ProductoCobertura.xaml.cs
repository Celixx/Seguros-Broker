// ProductoCobertura.xaml.cs
using Seguros_Broker.Repositorio;
using Seguros_Broker.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Seguros_Broker
{
    public partial class ProductoCobertura : Window
    {
        private int productoId;
        private CoberturaRep coberturaRep = new CoberturaRep();

        public class CoberturaSelectable
        {
            public bool IsSelected { get; set; }
            public string codigo { get; set; } = "";
            public string nombre { get; set; } = "";
        }

        private List<CoberturaSelectable> items;

        public ProductoCobertura(int productoId)
        {
            InitializeComponent();
            this.productoId = productoId;
            LoadCoberturas();
        }

        private void LoadCoberturas()
        {
            var todas = coberturaRep.GetCoberturas();
            var asociados = coberturaRep.GetCoberturasByProducto(productoId) ?? new List<string>();

            items = todas.Select(c => new CoberturaSelectable
            {
                IsSelected = asociados.Contains(c.codigo),
                codigo = c.codigo,
                nombre = c.nombre
            }).ToList();

            dataGridCoberturasSeleccion.ItemsSource = items;
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            var seleccionadas = items.Where(i => i.IsSelected).Select(i => i.codigo).ToList();

            var result = await coberturaRep.AssignCoberturasToProducto(productoId, seleccionadas);
            if (result.success)
            {
                MessageBox.Show("Coberturas asociadas correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("No se pudo asociar coberturas: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string q = txtSearch.Text?.Trim().ToUpper() ?? "";
            if (string.IsNullOrEmpty(q))
            {
                dataGridCoberturasSeleccion.ItemsSource = items;
                return;
            }

            var filtrado = items.Where(c => (c.codigo ?? "").ToUpper().Contains(q) || (c.nombre ?? "").ToUpper().Contains(q)).ToList();
            dataGridCoberturasSeleccion.ItemsSource = filtrado;
        }
    }
}

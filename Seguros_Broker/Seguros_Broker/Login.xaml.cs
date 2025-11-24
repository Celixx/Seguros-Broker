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

namespace Seguros_Broker
{

    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void TxtUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderUsuario.Visibility =
                string.IsNullOrWhiteSpace(TxtUsuario.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var ventanaPrincipal = new VentanaPrincipal();
            ventanaPrincipal.Show();

            this.Close();
        }

        private void OlvidoContrasena_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Redirigiendo al proceso de recuperación de contraseña.", "Información");
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

using Seguros_Broker.Modelo;
using Seguros_Broker.Repositorio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
    /// <summary>
    /// Interaction logic for VentanaPrincipal.xaml
    /// </summary>
    public partial class NuevaPropuestaCaratula : Window
    {
        
        private ClienteDatosPagoRep clienteDatosPagoRep = new ClienteDatosPagoRep();
        private List<ClienteDatosPago> clienteDatosPagoCache = new List<ClienteDatosPago>();

        private List<Moneda> monedas;
        private MonedaRep monedaRep= new MonedaRep();
        private ClienteRep clienteRep = new ClienteRep();
        private SocioRep socioRep = new SocioRep();
        private GestorRep gestorRep = new GestorRep();
        private CompaniaRep companiaRep = new CompaniaRep();
        private EjecutivoRep ejecutivoRep = new EjecutivoRep();         
        private PropuestaRep propuestaRep = new PropuestaRep();
        private ItemRep itemRep = new ItemRep();
        public ObservableCollection<Cobertura> CoberturasDePropuesta { get; set; }
        public ObservableCollection<ItemResumenCobertura> ItemsConCobertura { get; set; }


        public NuevaPropuestaCaratula()
        {
            InitializeComponent();

            this.monedas = monedaRep.GetMonedas();

            CoberturasDePropuesta = new ObservableCollection<Cobertura>();
            CoberturasDataGrid.ItemsSource = CoberturasDePropuesta;

          
            this.ItemsConCobertura = new ObservableCollection<ItemResumenCobertura>();

            
            this.DataContext = this; 

            

            cbMonedas.ItemsSource = monedas;

            InitializeComponent();
            HookEvents();
            cbFormaCompromiso.Items.Clear();
            cbFormaCompromiso.Items.Add("PAC");
            cbFormaCompromiso.SelectedIndex = 0;

            //BORRAR
            TabItems.IsEnabled = true;
            TabPlan.IsEnabled = true;
            TabMinuta.IsEnabled = true;
            TabBitacora.IsEnabled = true;
            TabDocumentos.IsEnabled = true;

            // grid vacío
            dataGridPlanPagos.ItemsSource = new List<PlanPagoRow>();


        }

        private List<Modelo.EjecutivoM> GetEjecutivo()
        {
            var repo = new EjecutivoRep();
            var ejecutivos = repo.GetEjecutivos();
            return ejecutivos;
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones
            var errores = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(TxtNumeroPoliza.Text))
                errores.Add("Número de Póliza (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtCodigoRamo.Text))
                errores.Add("Código Ramo (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtCodigoEjecutivo.Text))
                errores.Add("Código Ejecutivo de Cuenta (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtCodigoEjecutivoResponsable.Text))
                errores.Add("Código Ejecutivo Responsable (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtAreaNegocio.Text))
                errores.Add("Código Área de negocio (obligatorio).");

            if (DpDesde.SelectedDate == null)
                errores.Add("Fecha de ingreso (obligatorio).");

            if (DpHasta.SelectedDate == null)
                errores.Add("Fecha de término (obligatorio).");

            if (cbMonedas.SelectedItem == null)
                errores.Add("Tipo de moneda (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtComisionAfectaPorcentaje.Text) && string.IsNullOrWhiteSpace(TxtComisionExentaPorcentaje.Text))
                errores.Add("Comisión afecta o Comisión exenta (Obligatorio)");

            //if (!string.IsNullOrWhiteSpace(TxtComisionAfectaPorcentaje.Text) && !string.IsNullOrWhiteSpace(TxtComisionExentaPorcentaje.Text))
            //    errores.Add("Comisión afecta y Comisión exenta no pueden estar rellenados ambos");

            if (string.IsNullOrWhiteSpace(TxtRutCliente1.Text))
                errores.Add("RUT Facturar a (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtRutContratante.Text))
                errores.Add("RUT Contratante (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtRutAsegurado.Text))
                errores.Add("RUT Asegurado (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtRutAFavorDe.Text))
                errores.Add("RUT A favor de (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtRutSocio.Text))
                errores.Add("RUT Socio (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtRutGestor.Text))
                errores.Add("RUT Gestor (obligatorio).");

            if (string.IsNullOrWhiteSpace(TxtRutCompania.Text))
                errores.Add("RUT Compañía (obligatorio).");

            if (cbEjecutivosCompania.SelectedItem == null)
                errores.Add("Ejecutivo Compañía (obligatorio).");

            if (TxtMateriaAsegurada.Text == "")
                errores.Add("Matería Asegurada (obligatorio).");            

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var monedaSeleccionada = new Moneda();
            monedaSeleccionada = (Moneda)cbMonedas.SelectedItem;
            var clienteSeleccionado = new Cliente();
            clienteSeleccionado = clienteRep.GetCliente(TxtRutCliente1.Text);
            var socioSeleccionado = new Socio();
            socioSeleccionado = socioRep.GetSocio(int.Parse(TxtRutSocio.Text));

            var ejecutivos = GetEjecutivo();
            var ejecutivoSeleccionado = new EjecutivoM();
            foreach (var ejecutivo in ejecutivos)
            {
                if (ejecutivo.codigo.ToString() == TxtCodigoEjecutivo.Text)
                {
                    ejecutivoSeleccionado = ejecutivo;
                }
            }

            var selectedTipoPoliza = "";

            if (RbConvencional.IsChecked == true)
            {
                selectedTipoPoliza = RbConvencional.Content.ToString();
            }
            else if(RbColectiva.IsChecked == true)
            {
                selectedTipoPoliza = RbColectiva.Content.ToString();
            }

            var nuevaPropuesta = new Propuesta
            {
                TipoPoliza = selectedTipoPoliza,
                FechaRecepcion = DtFechaRecepcion.SelectedDate,
                NumeroPoliza = int.Parse(TxtNumeroPoliza.Text),
                RenuevaPoliza = int.Parse(TxtRenuevaPoliza.Text),
                FechaIngreso = DpFechaIngreso.SelectedDate,
                FechaEmision = DpTermino.SelectedDate,
                IDRamo = int.Parse(TxtCodigoRamo.Text),
                IDEjecutivo = ejecutivoSeleccionado.ID,
                Area = TxtAreanNegocio.Text,
                FechaCreacion = DpFechaCreacion.SelectedDate,
                FechaVigenciaDesde = DpDesde.SelectedDate,
                FechaVigenciaHasta = DpHasta.SelectedDate,
                IDMoneda = monedaSeleccionada.monedaId,
                ComisionAfecta = int.Parse(TxtComisionAfectaPorcentaje.Text),
                ComisionExenta = int.Parse(TxtComisionExentaPorcentaje.Text),
                MontoAsegurado = int.Parse(TxtMontoAsegurado.Text),
                ComisionTotal = int.Parse(TxtComisionTotal.Text),
                PrimaNetaAfecta = int.Parse(TxtPrimaNetaAfecta.Text),
                PrimaNetaExenta = int.Parse(TxtPrimaNetaExenta.Text),
                PrimaNetaTotal = int.Parse(TxtPrimaNetaTotal.Text),
                IVA = int.Parse(TxtIva.Text),
                PrimaBrutaTotal = int.Parse(TxtPrimaBrutaTotal.Text),
                IDCliente = clienteSeleccionado.ID,
                IDSocio = socioSeleccionado.ID,
                IDGestor = int.Parse(TxtRutGestor.Text),
                IDCompania = TxtRutCompania.Text,
                MateriaAsegurada = TxtMateriaAsegurada.Text,
                Observacion = TxtObservacion.Text
            };

            var result = await propuestaRep.CreatePropuestaAsync(nuevaPropuesta);

            if (result.success)
            {
                MessageBox.Show("Propuesta guardada correctramente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                //mostrar mensaje de error del repo
                MessageBox.Show("No se pudo guardar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Seguro que quiere salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void BtnBuscarRamo_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCodigoRamo.Text == "")
            {
                MessageBox.Show("Por favor ingrese un ID para buscar.");
            }
            else
            {
                var ramoRep = new RamoRep();
                var ramoBuscado = ramoRep.GetRamo(int.Parse(TxtCodigoRamo.Text));

                TxtRamo.Visibility = Visibility.Visible;
                TxtRamo.Text = ramoBuscado.nombre;
                return;
            }
        }


        private void BtnBuscarEjecutivoCuenta_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCodigoEjecutivo.Text == "")
            {
                MessageBox.Show("Por favor ingrese un ID para buscar.");
            }
            else
            {
                var ejecutivos = GetEjecutivo();

                foreach (var ejecutivo in ejecutivos)
                {
                    if (ejecutivo.codigo.ToString() == TxtCodigoEjecutivo.Text)
                    {
                        TxtEjecutivoCuenta.Visibility = Visibility.Visible;
                        TxtEjecutivoCuenta.Text = ejecutivo.nombre + " " + ejecutivo.aPaterno + " " + ejecutivo.aMaterno;
                        return;
                    }
                }

                MessageBox.Show($"No se encontró ningún ejecutivo con el ID: {TxtCodigoEjecutivo.Text}");
            }
        }

        private void BtnBuscarEjecutivoResponsable_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCodigoEjecutivo.Text == "")
            {
                MessageBox.Show("Por favor ingrese un ID para buscar.");
            }
            else
            {
                var ejecutivos = GetEjecutivo();

                foreach (var ejecutivo in ejecutivos)
                {
                    if (ejecutivo.codigo.ToString() == TxtCodigoEjecutivoResponsable.Text)
                    {
                        TxtEjecutivoResponsable.Visibility = Visibility.Visible;
                        TxtEjecutivoResponsable.Text = ejecutivo.nombre + " " + ejecutivo.aPaterno + " " + ejecutivo.aMaterno;
                        return;
                    }
                }

                MessageBox.Show($"No se encontró ningún ejecutivo con el ID: {TxtCodigoEjecutivo.Text}");
            }
        }
        private void BtnBuscarArea_Click(object sender, RoutedEventArgs e)
        {
            TxtAreanNegocio.Visibility = Visibility.Visible;
            TxtAreanNegocio.Text = "Generales";
        }
        private void BtnBuscarRutFacturar_Click(object sender, RoutedEventArgs e)
        {
            if (TxtRutCliente1.Text == "")
            {
                MessageBox.Show("Por favor ingrese un ID.");
            }
            else
            {
                try
                {
                    var rutFacturar = clienteRep.GetCliente(TxtRutCliente1.Text);
                    if (rutFacturar == null)
                    {
                        MessageBox.Show("Error, el ID no se encuentra registrado");
                    }
                    else
                    {
                        TxtFacturarA.Visibility = Visibility.Visible;
                        TxtFacturarA.Text = rutFacturar.Nombre;
                    }
                        
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error, el ID no se encuentra registrado");
                }
            }
        }
        private void BtnBuscarRutContratante_Click(object sender, RoutedEventArgs e)
        {
            if (TxtRutContratante.Text == "")
            {
                MessageBox.Show("Por favor ingrese un ID.");
            }
            else
            {
                try
                {
                    var rutContratante = clienteRep.GetCliente(TxtRutContratante.Text);
                    if (rutContratante == null)
                    {
                        MessageBox.Show("Error, el ID no se encuentra registrado");
                    }
                    else
                    {
                        TxtContratante.Visibility = Visibility.Visible;
                        TxtContratante.Text = rutContratante.Nombre;
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error, el ID no se encuentra registrado");
                }
            }
        }
        private void BtnBuscarRutAsegurado_Click(object sender, RoutedEventArgs e)
        {
            if (TxtRutAsegurado.Text == "")
            {
                MessageBox.Show("Por favor ingrese un ID.");
            }
            else
            {
                try
                {
                    var rutAsegurado = clienteRep.GetCliente(TxtRutAsegurado.Text);
                    if (rutAsegurado == null)
                    {
                        MessageBox.Show("Error, el ID no se encuentra registrado");
                    }
                    else
                    {
                        TxtAsegurado.Visibility = Visibility.Visible;
                        TxtAsegurado.Text = rutAsegurado.Nombre;
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error, el ID no se encuentra registrado");
                }
            }
        }
        private void BtnBuscarRutAFavorDe_Click(object sender, RoutedEventArgs e)
        {
            if (TxtRutAFavorDe.Text == "")
            {
                MessageBox.Show("Por favor ingrese un ID.");
            }
            else
            {
                try
                {
                    var rutAFavorDe = clienteRep.GetCliente(TxtRutAFavorDe.Text);
                    if (rutAFavorDe == null)
                    {
                        MessageBox.Show("Error, el ID no se encuentra registrado");
                    }
                    else
                    {
                        TxtAFavorDe.Visibility = Visibility.Visible;
                        TxtAFavorDe.Text = rutAFavorDe.Nombre;
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error, el ID no se encuentra registrado");
                }
            }
        }
        private void BtnBuscarRutSocio_Click(object sender, RoutedEventArgs e)
        {
            if (TxtRutSocio.Text == "")
            {
                MessageBox.Show("Por favor ingrese un ID.");
            }
            else
            {
                try
                {
                    var rutSocio = socioRep.GetSocio(int.Parse(TxtRutSocio.Text));
                    if (rutSocio == null)
                    {
                        MessageBox.Show("Error, el ID no se encuentra registrado");
                    }
                    else
                    {
                        TxtSocio.Visibility = Visibility.Visible;
                        TxtSocio.Text = rutSocio.nombre;

                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error, el ID no se encuentra registrado");
                }
            }
        }
        private void BtnBuscarRutGestor_Click(object sender, RoutedEventArgs e)
        {
            if (TxtRutGestor.Text == "")
            {
                MessageBox.Show("Por favor ingrese un ID.");
            }
            else
            {
                try
                {
                    var rutGestor = gestorRep.GetGestor(int.Parse(TxtRutGestor.Text));
                    if (rutGestor == null)
                    {
                        MessageBox.Show("Error, el ID no se encuentra registrado");
                    }
                    else
                    {
                        TxtGestor.Visibility = Visibility.Visible;
                        TxtGestor.Text = rutGestor.nombre;

                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error, el ID no se encuentra registrado");
                }
            }
        }
        private void BtnBuscarRutCompania_Click(object sender, RoutedEventArgs e)
        {
            if (TxtRutCompania.Text == "")
            {
                MessageBox.Show("Por favor ingrese un ID.");
            }
            else
            {
                try
                {
                    var compania = companiaRep.GetCompania(TxtRutCompania.Text);
                    var ejecutivos = ejecutivoRep.GetEjecutivos();
                    if (compania == null)
                    {
                        MessageBox.Show("Error, el ID no se encuentra registrado");
                    }
                    else
                    {
                        TxtCompania.Visibility = Visibility.Visible;
                        TxtCompania.Text = compania.nombre;
                        TxtGenerales.Visibility = Visibility.Visible;
                        TxtGenerales.Text = "Generales";

                        cbEjecutivosCompania.ItemsSource = ejecutivos;

                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error, el ID no se encuentra registrado");
                }
            }
        }
        private void BtnLimpiarEjecutivo_Click(object sender, RoutedEventArgs e)
        {
            cbEjecutivosCompania.SelectedItem = null;
        }

        private void BtnAgregarItem(object sender, RoutedEventArgs e)
        {
            var VentanaAgregarNuevoItem = new VentanaAgregarNuevoItem();
            VentanaAgregarNuevoItem.ShowDialog();
        }

        private void cbFormatoHoja_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnPdf_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TxtCodigoRamo_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BtnBuscarMoneda_Click(object sender, RoutedEventArgs e)
        {

            if (TxtComisionAfectaPorcentaje.Text == "")
            {
                TxtComisionAfectaTexto.Visibility = Visibility.Visible;
                TxtComisionExentaTexto.Visibility = Visibility.Visible;
                TxtMontoAsegurado.Visibility = Visibility.Visible;
                TxtMontoAsegurado.Text = "0";
                TxtRamo.Visibility = Visibility.Visible;
                TxtMontoAsegurado.Text = "0";
                TxtComisionExentaTexto.Text = "759";
                TxtComisionAfectaTexto.Text = "0";
                TxtPrimaNetaAfecta.IsEnabled = false;
                TxtPrimaNetaAfecta.Visibility = Visibility.Visible;
                TxtPrimaNetaAfecta.Text = "5064";
                TxtComisionTotal.Text = "759";
                TxtPrimaNetaExenta.Visibility = Visibility.Visible;
                TxtPrimaNetaExenta.Text = "0";
                TxtComisionTotal.Visibility = Visibility.Visible;
                TxtPrimaBrutaTotal.Visibility = Visibility.Visible;
                TxtPrimaNetaTotal.Visibility = Visibility.Visible;
                TxtPrimaNetaTotal.Text = "5064";
                TxtPrimaBrutaTotal.Text = "5064";
                TxtIva.Visibility = Visibility.Visible;
                TxtIva.Text = "962";
                TxtPrimaBrutaTotal.Visibility = Visibility.Visible;
                TxtPrimaNetaTotal.Text = "6026";
                TxtComisionAfectaPorcentaje.IsEnabled = false;
                TxtComisionAfectaPorcentaje.Text = "0";
            }
            else
            {
                TxtComisionAfectaTexto.Visibility = Visibility.Visible;
                TxtComisionExentaTexto.Visibility = Visibility.Visible;
                TxtMontoAsegurado.Visibility = Visibility.Visible;
                TxtMontoAsegurado.Text = "0";
                TxtRamo.Visibility = Visibility.Visible;
                TxtMontoAsegurado.Text = "1";
                TxtComisionAfectaTexto.Text = "379";
                TxtComisionExentaTexto.Text = "0";
                TxtComisionExentaPorcentaje.Text = "0";
                TxtPrimaNetaAfecta.IsEnabled = false;
                TxtPrimaNetaAfecta.Visibility = Visibility.Visible;
                TxtPrimaNetaAfecta.Text = "2532";
                TxtComisionTotal.Text = "379";
                TxtPrimaNetaExenta.Visibility = Visibility.Visible;
                TxtPrimaNetaExenta.Text = "0";
                TxtComisionTotal.Visibility = Visibility.Visible;
                TxtPrimaBrutaTotal.Visibility = Visibility.Visible;
                TxtPrimaNetaTotal.Visibility = Visibility.Visible;
                TxtPrimaNetaTotal.Text = "2532";
                TxtPrimaBrutaTotal.Text = "3013";
                TxtIva.Visibility= Visibility.Visible;
                TxtIva.Text = "481";
                TxtPrimaBrutaTotal.Visibility= Visibility.Visible;
                TxtPrimaNetaTotal.Text = "2532";
                TxtComisionExentaPorcentaje.IsEnabled = false;
            }

            try
            {

                // sincroniza los valores de Carátula hacia Plan de Pago
                SyncFromCaratula();

                // reconstruye el grid del plan de pago 
                RebuildPlanGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al sincronizar Carátula -> Plan de Pago: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAgregarCobertura(object sender, RoutedEventArgs e)
        {
            // Crear la ventana pop-up
            VentanaAgregarCobertura ventanaSeleccion = new VentanaAgregarCobertura();

            // Abrirla como un DIÁLOGO (esto pausa el código aquí hasta que se cierre)
            bool? resultado = ventanaSeleccion.ShowDialog();

            // Comprobar si el usuario hizo click en "aceptar"
            if (resultado == true)
            {
                // Obtener la lista de la propiedad pública de la ventana
                
                List<Cobertura> seleccionadas = ventanaSeleccion.CoberturasSeleccionadas;

                // Añadir las coberturas seleccionadas a la grilla de la ventana principal
                foreach (var cobertura in seleccionadas)
                {
                    // (Opcional) Comprobar si ya existe para no añadir duplicados
                    if (!CoberturasDePropuesta.Any(c => c.codigo == cobertura.codigo))
                    {
                        CoberturasDePropuesta.Add(cobertura);
                    }
                }
            }
        }


        //VENTANA DE PLAN DE PAGO
        private void TxtRutCliente1_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadClienteDatosPago(TxtRutCliente1.Text.Trim());
            RebuildPlanGrid();
        }
        public class PlanPagoRow
        {
            public int CuotaNro { get; set; }
            public DateTime FechaVencimiento { get; set; }
            public DateTime? FechaPago { get; set; } = null;
            public string NumeroDocumento { get; set; } = "";
            public string NroTarjetaCuenta { get; set; } = "";
            public string TipoTarjeta { get; set; } = "";
            public string ValidezTarjeta { get; set; } = "";
            public string Banco { get; set; } = "";
            public decimal Monto { get; set; }
            public string FormaPago { get; set; } = "PAC";
            public string FechaVencimientoStr => FechaVencimiento.ToString("dd-MM-yyyy");
            public string FechaPagoStr => FechaPago?.ToString("dd-MM-yyyy") ?? "";
        }

        private void HookEvents()
        {
            // si el usuario cambia los datos en Carátula, sincronizar cuando se seleccione la pestaña Plan de Pago
            txtCuotaDesde.TextChanged += (s, e) => RebuildPlanGrid();
            txtCuotaHasta.TextChanged += (s, e) => RebuildPlanGrid();
            dpFechaIngresoPlan.SelectedDateChanged += (s, e) => RebuildPlanGrid();

            // cuando cambia el rut del cliente en Carátula TxtRutCliente1 recargar los datos de pago 
            TxtRutCliente1.TextChanged += (s, e) => { LoadClienteDatosPago(TxtRutCliente1.Text.Trim()); RebuildPlanGrid(); };
        }

        private void SyncFromCaratula()
        {
            try
            {
                // copiar valores desde Carátula 
                txtMontoAseguradoValor.Text = TxtMontoAsegurado?.Text ?? "0";
                txtPrimaNetaAfectaValor.Text = TxtPrimaNetaAfecta?.Text ?? "0";
                txtPrimaNetaExentaValor.Text = TxtPrimaNetaExenta?.Text ?? "0";
                txtPrimaNetaTotalValor.Text = TxtPrimaNetaTotal?.Text ?? "0";
                txtIVAValor.Text = TxtIva?.Text ?? "0";
                var primaBruta = TxtPrimaBrutaTotal?.Text ?? "0";
                txtPrimaBrutaTotalValor.Text = primaBruta;
                txtMontoTotalStatic.Text = primaBruta;
                txtTotalCuotasStatic.Text = primaBruta;
                txtMontoTotalCuotaStatic.Text = primaBruta;

                if (string.IsNullOrWhiteSpace(txtMontoPactar.Text))
                    txtMontoPactar.Text = primaBruta;

                // actualizar grid
                RebuildPlanGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al sincronizar Carátula -> Plan de Pago: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        private void LoadClienteDatosPago(string clienteID)
        {
            if (string.IsNullOrWhiteSpace(clienteID))
            {
                clienteDatosPagoCache = new List<ClienteDatosPago>();
                return;
            }

            clienteDatosPagoCache = clienteDatosPagoRep.GetDatosPagoByClienteID(clienteID);
        }

       
        private void RebuildPlanGrid()
        {
           
            if (!int.TryParse(txtCuotaDesde.Text?.Trim(), out int cuotaDesde))
            {
                
                dataGridPlanPagos.ItemsSource = new List<PlanPagoRow>();
                return;
            }
            if (!int.TryParse(txtCuotaHasta.Text?.Trim(), out int cuotaHasta))
            {
                dataGridPlanPagos.ItemsSource = new List<PlanPagoRow>();
                return;
            }
            if (cuotaHasta < cuotaDesde)
            {
                dataGridPlanPagos.ItemsSource = new List<PlanPagoRow>();
                return;
            }

            // fecha de ingreso plan
            if (!dpFechaIngresoPlan.SelectedDate.HasValue)
            {
                dataGridPlanPagos.ItemsSource = new List<PlanPagoRow>();
                return;
            }
            DateTime fechaIngreso = dpFechaIngresoPlan.SelectedDate.Value.Date;

            // obtener monto total a repartir
            decimal montoTotal;
            var montoTotalText = txtMontoTotalStatic.Text?.Trim();
            if (!TryParseDecimalInvariant(montoTotalText, out montoTotal))
            {
                
                if (!TryParseDecimalInvariant(txtMontoPactar.Text?.Trim(), out montoTotal))
                {
                    montoTotal = 0m;
                }
            }

            // cantidad de cuotas
            int cantidadCuotas = cuotaHasta - cuotaDesde + 1;

            // monto por cuota 
            decimal montoPorCuota = 0m;
            if (cantidadCuotas > 0)
            {
                montoPorCuota = Math.Floor((montoTotal / cantidadCuotas) * 100m) / 100m; 
            }

            // construir filas
            var filas = new List<PlanPagoRow>();
            for (int nro = cuotaDesde; nro <= cuotaHasta; nro++)
            {
                int offset = nro - cuotaDesde + 1; 
                DateTime fechaVenc = SafeAddMonthsKeepingDay(fechaIngreso, offset);

                // datos del cliente de pago 
                var datosPago = clienteDatosPagoCache.FirstOrDefault();

                filas.Add(new PlanPagoRow
                {
                    CuotaNro = nro,
                    FechaVencimiento = fechaVenc,
                    FechaPago = null,
                    NumeroDocumento = datosPago?.NumeroDocumento ?? "",
                    NroTarjetaCuenta = datosPago?.NroTarjetaCuenta ?? "",
                    TipoTarjeta = datosPago?.TipoTarjeta ?? "",
                    ValidezTarjeta = datosPago?.ValidezTarjeta ?? "",
                    Banco = datosPago?.Banco ?? "",
                    FormaPago = "PAC",
                    Monto = montoPorCuota
                });
            }

            decimal sumaAsignada = filas.Sum(f => f.Monto);
            decimal diferencia = Math.Round(montoTotal - sumaAsignada, 2);
            if (filas.Count > 0 && Math.Abs(diferencia) >= 0.01m)
            {
                filas[filas.Count - 1].Monto += diferencia;
            }

            // mostrar en grid
            dataGridPlanPagos.ItemsSource = filas;
            dataGridPlanPagos.Items.Refresh();

            // actualizar campos estáticos de totales y total cuotas
            txtMontoTotalStatic.Text = montoTotal.ToString("N2", CultureInfo.InvariantCulture);
            txtTotalCuotasStatic.Text = cantidadCuotas.ToString();
            txtMontoTotalCuotaStatic.Text = montoTotal.ToString("N2", CultureInfo.InvariantCulture);
        }

        private bool TryParseDecimalInvariant(string s, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim().Replace(".", "").Replace(",", "."); // "1.234,56" -> "1234.56"
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        // añadir meses manteniendo el día cuando sea posible 
        private DateTime SafeAddMonthsKeepingDay(DateTime date, int months)
        {
            var target = date.AddMonths(months);
            int day = Math.Min(date.Day, DateTime.DaysInMonth(target.Year, target.Month));
            return new DateTime(target.Year, target.Month, day);
        }


        private void BtnSyncPlanPago_Click(object sender, RoutedEventArgs e)
        {
            SyncFromCaratula();
        }


        private async void Guaradar_Click_Pagos(object sender, RoutedEventArgs e)
        {
            // validaciones 
            if (!dpFechaIngresoPlan.SelectedDate.HasValue)
            {
                MessageBox.Show("Debe elegir Fecha de ingreso plan de pago.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCuotaDesde.Text?.Trim(), out int cuotaDesde))
            {
                MessageBox.Show("Cuota desde inválida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCuotaHasta.Text?.Trim(), out int cuotaHasta))
            {
                MessageBox.Show("Cuota hasta inválida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cuotaDesde >= cuotaHasta)
            {
                MessageBox.Show("Cuota desde debe ser menor que Cuota hasta.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var items = dataGridPlanPagos.ItemsSource as IEnumerable<PlanPagoRow>;
            if (items == null || !items.Any())
            {
                MessageBox.Show("No hay filas en el Plan de Pago para guardar.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtNumeroPoliza.Text?.Trim(), out int numeroPoliza))
            {
                MessageBox.Show("Número de Póliza inválido. Debe guardar la Carátula primero.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var propuesta = propuestaRep.GetPropuestaByNumeroPoliza(numeroPoliza);
            if (propuesta == null || propuesta.ID <= 0)
            {
                MessageBox.Show("No se encontró la propuesta en la base de datos. Por favor guarde la Carátula antes de guardar los pagos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // mapear PlanPagoRow a PagoPropuesta 
            var pagos = new List<PagoPropuesta>();
            foreach (var row in items)
            {
                // row es PlanPagoRow (definido en esta clase). Asegurar casting correcto.
                if (row is PlanPagoRow pRow)
                {
                    pagos.Add(new PagoPropuesta
                    {
                        PropuestaID = propuesta.ID,
                        NumeroPoliza = numeroPoliza,
                        CuotaNro = pRow.CuotaNro,
                        Monto = pRow.Monto,
                        FechaVencimiento = pRow.FechaVencimiento,
                        FormaPago = pRow.FormaPago ?? "PAC",
                        NumeroDocumento = pRow.NumeroDocumento ?? "",
                        NroTarjCtaCte = pRow.NroTarjetaCuenta ?? "",
                        TipoTarj = pRow.TipoTarjeta ?? "",
                        ValidezTarj = pRow.ValidezTarjeta ?? "",
                        Banco = pRow.Banco ?? ""
                    });
                }
            }

            if (pagos.Count == 0)
            {
                MessageBox.Show("No hay pagos válidos para guardar.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Insertar pagos
            var pagosRepo = new PagosPropuestaRep();
            var (success, errorMsg) = await pagosRepo.CreatePagosForPropuestaAsync(propuesta.ID, numeroPoliza, pagos);
            if (success)
            {
                MessageBox.Show("Pagos guardados correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No se pudieron guardar los pagos: " + (errorMsg ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       

        private void BtnBuscarItem_Click(object sender, RoutedEventArgs e)
        {
            string rut = TxtRutItem.Text.Trim();

            if (string.IsNullOrWhiteSpace(rut))
            {
                MessageBox.Show("Por favor, ingrese un RUT para buscar el Ítem.", "Error de Entrada", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                List<Item> itemsEncontrados = itemRep.GetItemsByRut(rut);

                if (itemsEncontrados == null || !itemsEncontrados.Any())
                {
                    MessageBox.Show($"No se encontró ningún Ítem asociado al RUT: {rut}.", "Sin Resultados", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarCamposItem();
                    return;
                }

                Item item = itemsEncontrados.First();

                
                TxtIdItem.Text = item.IdItem.ToString();

                TxtMateriaAsegurada1.Text = item.MateriaAsegurada;

                TxtAnno.Text = item.Anno;
                TxtPatente.Text = item.Patente;
                TxtMinutaItem.Text = item.MinutaItem;
                TxtCarroceria.Text = item.Carroceria;
                TxtPropietario.Text = item.Propietario;
                TxtTipo.Text = item.Tipo;
                TxtNumeroMotor.Text = item.NumeroMotor;
                TxtColor.Text = item.Color;
                TxtChasis.Text = item.Chasis;
                TxtValorComercial.Text = item.ValorComercial;
                TxtModelo.Text = item.Modelo;
                TxtNumeroChasis.Text = item.NumeroChasis;
                TxtUso.Text = item.Uso;

                TxtFechaDesde.Text = item.FechaDesde.HasValue ? item.FechaDesde.Value.ToShortDateString() : string.Empty;
                TxtFechaHasta.Text = item.FechaHasta.HasValue ? item.FechaHasta.Value.ToShortDateString() : string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al buscar el Ítem: {ex.Message}", "Error de Búsqueda", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarCamposItem()
        {
           
            
            TxtMateriaAsegurada1.Text = string.Empty;

            TxtIdItem.Text = string.Empty;
            TxtAnno.Text = string.Empty;
            TxtPatente.Text = string.Empty;
            TxtMinutaItem.Text = string.Empty;
            TxtCarroceria.Text = string.Empty;
            TxtPropietario.Text = string.Empty;
            TxtTipo.Text = string.Empty;
            TxtNumeroMotor.Text = string.Empty;
            TxtColor.Text = string.Empty;
            TxtChasis.Text = string.Empty;
            TxtValorComercial.Text = string.Empty;
            TxtModelo.Text = string.Empty;
            TxtNumeroChasis.Text = string.Empty;
            TxtUso.Text = string.Empty;
            TxtFechaDesde.Text = string.Empty;
            TxtFechaHasta.Text = string.Empty;
        }


        private static bool TryParseDecimal(string s, out decimal value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            // Esto es CRÍTICO para manejar números chilenos (miles con punto, decimales con coma, o al revés)
            s = s.Trim().Replace(".", "").Replace(",", "."); // "1.234,56" -> "1234.56"
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }
        

        private async void BtnGuardarItemCoberturas_Click(object sender, RoutedEventArgs e)
        {
            // --- 1. CAPTURA Y VALIDACIÓN DE CLAVES ---
            // NOTA: Debes asegurarte de que currentPropuestaId y currentItemId 
            // se actualicen correctamente cuando se busca el Ítem y se crea/carga la Propuesta.
            // Usaremos variables placeholder si no tienes las variables de instancia declaradas:
            int idPropuesta = 1; // REEMPLAZAR con el valor real (ej: this.currentPropuestaId)
            int idItem = 1;      // REEMPLAZAR con el valor real (ej: this.currentItemId)

            if (idPropuesta <= 0 || idItem <= 0)
            {
                MessageBox.Show("Debe seleccionar un Ítem y una Propuesta válida para guardar coberturas.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- 2. CAPTURA DE VALORES GLOBALES (Monto y Prima) ---

            // Obtener Monto Asegurado (TxtMontoAsegurado)
            if (!TryParseDecimal(TxtMontoAsegurado.Text, out decimal montoAseguradoGlobal))
            {
                MessageBox.Show("El Monto Asegurado es inválido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Obtener Prima Neta Total (TxtPrimaNetaTotal)
            if (!TryParseDecimal(TxtPrimaNetaTotal.Text, out decimal primaNetaTotalGlobal))
            {
                MessageBox.Show("La Prima Neta Total es inválida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- 3. FILTRAR Y MAPEAR LAS COBERTURAS SELECCIONADAS ---

            // Asumo que CoberturasDePropuesta es la ObservableCollection<Cobertura> enlazada a la grilla
            var coberturasSeleccionadas = CoberturasDePropuesta
                .Where(c => c.IsSelected) // Asumo que tienes una propiedad IsSelected en el modelo Cobertura
                .ToList();

            if (!coberturasSeleccionadas.Any())
            {
                MessageBox.Show("No hay coberturas seleccionadas para guardar.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mapear Cobertura al Modelo ItemCobertura, inyectando los valores globales de Monto y Prima
            var coberturasAModelos = coberturasSeleccionadas.Select(c => new ItemCobertura
            {
                IdPropuesta = idPropuesta,
                IdItem = idItem,
                CodCobertura = c.codigo,

                // Asumiendo que afectaExtenta y sumaMonto son strings en el modelo Cobertura
                AfectaExenta = c.afectaExtenta,
                SumaAlMonto = c.sumaMonto,

                // INYECTAMOS los valores globales capturados de los TextBox de la Carátula
                Monto = montoAseguradoGlobal,
                Prima = primaNetaTotalGlobal
            }).ToList();

            // --- 4. GUARDAR EN LA BASE DE DATOS ---

            var rep = new ItemCoberturaRep();
            var (success, errorMsg) = await rep.CreateItemCoberturasAsync(coberturasAModelos);

            if (success)
            {
                // **********************************************
                // LÓGICA NUEVA: RECARGAR DESDE LA BASE DE DATOS
                // **********************************************

                // 1. Obtener la lista actualizada de la base de datos
                // Asegúrate de usar el ID de la propuesta actual.
                var resumenDesdeDB = await rep.GetResumenItemsPorPropuestaAsync(idPropuesta);

                // 2. Limpiar la colección actual enlazada a la grilla
                // Esto borra los datos anteriores y prepara la actualización
                ItemsConCobertura.Clear();

                // 3. Rellenar la colección con los datos frescos de la base de datos
                foreach (var item in resumenDesdeDB)
                {
                    ItemsConCobertura.Add(item);
                }

                // **********************************************

                MessageBox.Show("Coberturas de Ítem guardadas correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Error al guardar coberturas: " + errorMsg, "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DpDesde_DateChanged(object sender, RoutedEventArgs e)
        {
            DpHasta.IsEnabled = true;
            if (DpHasta.SelectedDate == null)
            {
                DateTime fechaSuma = (DateTime)DpDesde.SelectedDate;                
                DpHasta.SelectedDate = fechaSuma.AddYears(1); ;
                CalcularDiasFechaCaratula();
            }
            CalcularDiasFechaCaratula();
        }
        private void DpHasta_DateChanged(object sender, RoutedEventArgs e)
        {
            CalcularDiasFechaCaratula();
        }
        private void CalcularDiasFechaCaratula()
        {
            DateTime fechaDesde = (DateTime)DpDesde.SelectedDate;
            DateTime fechaHasta = (DateTime)DpHasta.SelectedDate;

            var dias = (fechaHasta - fechaDesde).Days;
            TxtNDias.Text = dias.ToString();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show(
            "Estás seguro que quieres salir? perderás todo el progreso de tu propuesta de carátula",
            "Confirmación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );

            if (result == MessageBoxResult.No)
                e.Cancel = true;
        }

    }

}

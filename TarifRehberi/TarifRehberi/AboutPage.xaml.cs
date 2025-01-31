using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TarifRehberi
{
    /// <summary>
    /// AboutPage.xaml etkileşim mantığı
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ResimYukle_Click(object sender, RoutedEventArgs e)
        {
           
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"; 

            
            if (openFileDialog.ShowDialog() == true)
            {
                
                string selectedFilePath = openFileDialog.FileName;

                MessageBox.Show("Seçilen Dosya: " + selectedFilePath);

                
            }
        }

        private void BtnEkle_Click(object sender, RoutedEventArgs e)
        {
            string malzemeAdi = txtMalzemeAdi.Text;
            string toplamMiktar = txtToplamMiktar.Text;
            string malzemeBirim = (cmbMalzemeBirim.SelectedItem as ComboBoxItem)?.Content.ToString();
            decimal birimFiyat;
            if (!decimal.TryParse(txtBirimFiyat.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out birimFiyat))
            {
                MessageBox.Show("Lütfen geçerli bir birim fiyatı girin.");
                return;
            }

            VeritabaniIslemleri veriIslem = new VeritabaniIslemleri();

            //duplicaate
            if (veriIslem.MalzemeVarMi(malzemeAdi))
            {
                MessageBox.Show("Bu malzeme zaten mevcut. Lütfen farklı bir malzeme ekleyin.");
                return;
            }

            
            veriIslem.MalzemeEkle(malzemeAdi, toplamMiktar, malzemeBirim, birimFiyat);

            
            DataGridMalzemeler.ItemsSource = veriIslem.TumMalzemeleriGetir().DefaultView;
        }


        private void BtnAra_Click(object sender, RoutedEventArgs e)
        {
            string malzemeAdi = txtArama.Text.Trim();

            VeritabaniIslemleri veriIslem = new VeritabaniIslemleri();
            DataTable dt;

            if (string.IsNullOrEmpty(malzemeAdi))
            {

                dt = veriIslem.TumMalzemeleriGetir();
            }
            else
            {
                
                dt = veriIslem.MalzemeAra(malzemeAdi);
            }

            DataGridMalzemeler.ItemsSource = dt.DefaultView;
        }



        private void DataGridMalzemeler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridMalzemeler.SelectedItem is DataRowView selectedRow)
            {
                txtMalzemeAdi.Text = selectedRow["MalzemeAdi"].ToString();
                txtToplamMiktar.Text = selectedRow["ToplamMiktar"].ToString();
                txtBirimFiyat.Text = selectedRow["BirimFiyat"].ToString();

                string malzemeBirim = selectedRow["MalzemeBirim"].ToString();
                foreach (ComboBoxItem item in cmbMalzemeBirim.Items)
                {
                    if (item.Content.ToString() == malzemeBirim)
                    {
                        cmbMalzemeBirim.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void BtnGuncelle_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridMalzemeler.SelectedItem is DataRowView selectedRow)
            {
                int malzemeID = Convert.ToInt32(selectedRow["MalzemeID"]);
                string malzemeAdi = txtMalzemeAdi.Text;
                string toplamMiktar = txtToplamMiktar.Text;
                string malzemeBirim = (cmbMalzemeBirim.SelectedItem as ComboBoxItem)?.Content.ToString();
                decimal birimFiyat;
                if (!decimal.TryParse(txtBirimFiyat.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out birimFiyat))
                {
                    MessageBox.Show("Lütfen geçerli bir birim fiyatı girin.");
                    return;
                }

                VeritabaniIslemleri veriIslem = new VeritabaniIslemleri();
                veriIslem.MalzemeGuncelle(malzemeID, malzemeAdi, toplamMiktar, malzemeBirim, birimFiyat);

                
                DataGridMalzemeler.ItemsSource = veriIslem.TumMalzemeleriGetir().DefaultView;
            }
        }

        private void BtnSil_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridMalzemeler.SelectedItem is DataRowView selectedRow)
            {
                int malzemeID = Convert.ToInt32(selectedRow["MalzemeID"]);

                var result = MessageBox.Show("Seçili malzemeyi silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    VeritabaniIslemleri veriIslem = new VeritabaniIslemleri();
                    veriIslem.MalzemeSil(malzemeID);

                    
                    DataGridMalzemeler.ItemsSource = veriIslem.TumMalzemeleriGetir().DefaultView;
                }
            }
        }
    }
}

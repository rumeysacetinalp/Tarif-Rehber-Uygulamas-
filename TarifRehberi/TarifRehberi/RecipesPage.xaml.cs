using Microsoft.Win32;
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

namespace TarifRehberi
{
    /// <summary>
    /// RecipesPage.xaml etkileşim mantığı
    /// </summary>
    public partial class RecipesPage : Page
    {

        
        private List<(string malzemeAdi, string miktar, string birim)> malzemeler = new List<(string, string, string)>();
      
        private string ResimYolu;
        public RecipesPage()
        {
            InitializeComponent();
        }

        private void ResimYukle_Click(object sender, RoutedEventArgs e)
        {
           
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"; 

           
            if (openFileDialog.ShowDialog() == true)
            {
               
                ResimYolu = openFileDialog.FileName;

                
                MessageBox.Show("Seçilen Dosya: " + ResimYolu);
            }
        }

        private void MalzemeSayisi_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            malzemeler.Clear();
            foreach (StackPanel item in MalzemePanel.Children)
            {
                if (item.Children[1] is ComboBox malzemeComboBox && item.Children[3] is TextBox miktarTextBox && item.Children[5] is ComboBox birimComboBox)
                {
                    malzemeler.Add((malzemeComboBox.SelectedItem?.ToString() ?? string.Empty, miktarTextBox.Text, birimComboBox.SelectedItem?.ToString() ?? string.Empty));
                }
            }

            
            if (int.TryParse((sender as TextBox).Text, out int malzemeSayisi))
            {
                
                while (MalzemePanel.Children.Count < malzemeSayisi)
                {
                    StackPanel malzemeItem = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                    Label malzemeLabel = new Label { Content = $"Malzeme {MalzemePanel.Children.Count + 1}:", VerticalAlignment = VerticalAlignment.Center };
                    ComboBox malzemeComboBox = new ComboBox { Width = 150, Margin = new Thickness(5, 0, 5, 0) };

                    
                    VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                    DataTable malzemelerDt = veritabaniIslemleri.TumMalzemeleriGetir();
                    foreach (DataRow row in malzemelerDt.Rows)
                    {
                        malzemeComboBox.Items.Add(row["MalzemeAdi"].ToString());
                    }
                    malzemeComboBox.Items.Add("Yeni Malzeme Ekle");
                    malzemeComboBox.SelectionChanged += MalzemeComboBox_SelectionChanged;

                    Label miktarLabel = new Label { Content = "Miktar:", VerticalAlignment = VerticalAlignment.Center };
                    TextBox miktarTextBox = new TextBox { Width = 75, Margin = new Thickness(5, 0, 5, 0) };

                    Label birimLabel = new Label { Content = "Birim:", VerticalAlignment = VerticalAlignment.Center };
                    ComboBox birimComboBox = new ComboBox { Width = 75, Margin = new Thickness(5, 0, 5, 0) };

                    malzemeItem.Children.Add(malzemeLabel);
                    malzemeItem.Children.Add(malzemeComboBox);
                    malzemeItem.Children.Add(miktarLabel);
                    malzemeItem.Children.Add(miktarTextBox);
                    malzemeItem.Children.Add(birimLabel);
                    malzemeItem.Children.Add(birimComboBox);

                    MalzemePanel.Children.Add(malzemeItem);
                }
            }
        }

        private void MalzemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem != null)
            {
                string secilenMalzeme = comboBox.SelectedItem.ToString();

                if (secilenMalzeme == "Yeni Malzeme Ekle")
                {
                    
                    YeniMalzemeEklePopUp(comboBox);
                }
                else
                {
                    
                    VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                    DataTable dt = veritabaniIslemleri.MalzemeAra(secilenMalzeme);
                    if (dt.Rows.Count > 0)
                    {
                        string birim = dt.Rows[0]["MalzemeBirim"].ToString();
                        StackPanel parentStackPanel = (StackPanel)comboBox.Parent;
                        if (parentStackPanel.Children[5] is ComboBox birimComboBox)
                        {
                            birimComboBox.Items.Clear();
                            birimComboBox.Items.Add(birim);
                            birimComboBox.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        private void YeniMalzemeEklePopUp(ComboBox malzemeComboBox)
        {
          
            Window popUp = new Window
            {
                Title = "Yeni Malzeme Ekle",
                Width = 300,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            StackPanel stackPanel = new StackPanel { Margin = new Thickness(10) };
            TextBox malzemeAdiTextBox = new TextBox { Text = "Malzeme Adı", Foreground = Brushes.Gray, Margin = new Thickness(0, 5, 0, 5) };
            malzemeAdiTextBox.GotFocus += (s, e) =>
            {
                if (malzemeAdiTextBox.Text == "Malzeme Adı")
                {
                    malzemeAdiTextBox.Text = "";
                    malzemeAdiTextBox.Foreground = Brushes.Black;
                }
            };
            malzemeAdiTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(malzemeAdiTextBox.Text))
                {
                    malzemeAdiTextBox.Text = "Malzeme Adı";
                    malzemeAdiTextBox.Foreground = Brushes.Gray;
                }
            };

            ComboBox birimComboBox = new ComboBox { Width = 150, Margin = new Thickness(0, 5, 0, 5) };
            birimComboBox.Items.Add("Gram");
            birimComboBox.Items.Add("Litre");
            birimComboBox.Items.Add("Kilo");
            TextBox birimFiyatTextBox = new TextBox { Text = "Birim Fiyatı", Foreground = Brushes.Gray, Margin = new Thickness(0, 5, 0, 5) };
            birimFiyatTextBox.GotFocus += (s, e) =>
            {
                if (birimFiyatTextBox.Text == "Birim Fiyatı")
                {
                    birimFiyatTextBox.Text = "";
                    birimFiyatTextBox.Foreground = Brushes.Black;
                }
            };
            birimFiyatTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(birimFiyatTextBox.Text))
                {
                    birimFiyatTextBox.Text = "Birim Fiyatı";
                    birimFiyatTextBox.Foreground = Brushes.Gray;
                }
            };

            Button ekleButton = new Button { Content = "Ekle", Width = 100, Margin = new Thickness(0, 10, 0, 5) };
            ekleButton.Click += (s, e) =>
            {
                string malzemeAdi = malzemeAdiTextBox.Text;
                string birim = birimComboBox.SelectedItem?.ToString();
                if (decimal.TryParse(birimFiyatTextBox.Text, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out decimal birimFiyat))
                {
                    if (!string.IsNullOrWhiteSpace(malzemeAdi) && !string.IsNullOrWhiteSpace(birim) && malzemeAdi != "Malzeme Adı" && birimFiyatTextBox.Text != "Birim Fiyatı")
                    {
                        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                        veritabaniIslemleri.MalzemeEkle(malzemeAdi, "0", birim, birimFiyat);
                        MessageBox.Show("Yeni malzeme eklendi!");
                        popUp.Close();

                       
                        malzemeComboBox.Items.Insert(malzemeComboBox.Items.Count - 1, malzemeAdi);
                        malzemeComboBox.SelectedItem = malzemeAdi;
                    }
                    else
                    {
                        MessageBox.Show("Lütfen tüm alanları doldurun.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir birim fiyatı girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            stackPanel.Children.Add(malzemeAdiTextBox);
            stackPanel.Children.Add(birimComboBox);
            stackPanel.Children.Add(birimFiyatTextBox);
            stackPanel.Children.Add(ekleButton);

            popUp.Content = stackPanel;
            popUp.ShowDialog();
        }

        private void KacKisilik_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
           
        }

        private void AraButton_Click(object sender, RoutedEventArgs e)
        {
            
            string arananTarifAdi = AramaTextBox.Text;

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            DataTable dt;

            
            if (string.IsNullOrWhiteSpace(arananTarifAdi))
            {
               
                dt = veritabaniIslemleri.TumTarifleriGetir();
            }
            else
            {
               
                dt = veritabaniIslemleri.TarifAra(arananTarifAdi);
            }

            // DataGrid'in ItemsSource özelliğini DataTable'ın DefaultView'i ile bağla
            DataGrid.ItemsSource = dt.DefaultView;
        }

        private void EkleButton_Click(object sender, RoutedEventArgs e)

        {
           
            string tarifAdi = TarifAdiTextBox.Text;
            string kategori = ((ComboBoxItem)KategoriComboBox.SelectedItem)?.Content.ToString();
            int hazirlamaSuresi = int.TryParse(HazirlamaSuresiTextBox.Text, out int suresi) ? suresi : 0;
            string talimatlar = TalimatlarTextBox.Text;
            string kisiSayisi = KisiSayisiTextBox.Text;

           
            if (string.IsNullOrWhiteSpace(ResimYolu))
            {
                MessageBox.Show("Lütfen bir resim seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }



            List<(string malzemeAdi, string miktar, string birim)> malzemeler = new List<(string, string, string)>();
            foreach (StackPanel item in MalzemePanel.Children)
            {
                if (item.Children[1] is ComboBox malzemeComboBox && item.Children[3] is TextBox miktarTextBox && item.Children[5] is ComboBox birimComboBox)
                {
                    string malzemeAdi = malzemeComboBox.SelectedItem?.ToString();
                    string miktar = miktarTextBox.Text;
                    string birim = birimComboBox.SelectedItem?.ToString();

                 
                    if (!string.IsNullOrWhiteSpace(malzemeAdi) && !string.IsNullOrWhiteSpace(miktar) && !string.IsNullOrWhiteSpace(birim))
                    {
                        malzemeler.Add((malzemeAdi, miktar, birim));
                    }
                }
            }

            
            if (string.IsNullOrWhiteSpace(tarifAdi))
            {
                MessageBox.Show("Tarif adı boş olamaz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Veritabanına kaydet
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            veritabaniIslemleri.TarifEkle(tarifAdi, kategori, hazirlamaSuresi, talimatlar, ResimYolu, kisiSayisi, malzemeler);
            AraButton_Click(sender, e);
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedItem is DataRowView rowView)
            {
                
                TarifAdiTextBox.Text = rowView["TarifAdi"].ToString();
                KategoriComboBox.SelectedItem = KategoriComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == rowView["Kategori"].ToString());
                HazirlamaSuresiTextBox.Text = rowView["HazirlamaSuresi"].ToString();
                TalimatlarTextBox.Text = rowView["Talimatlar"].ToString();
                KisiSayisiTextBox.Text = rowView["KisiSayisi"].ToString();
                ResimYolu = rowView["ResimYolu"].ToString();


                int tarifID = Convert.ToInt32(rowView["TarifID"]);
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                List<(string malzemeAdi, string miktar, string birim)> malzemeler = veritabaniIslemleri.TarifMalzemeleriGetir(tarifID);

                
                MalzemePanel.Children.Clear();
                foreach (var malzeme in malzemeler)
                {
                    StackPanel malzemeItem = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                    Label malzemeLabel = new Label { Content = "Malzeme:", VerticalAlignment = VerticalAlignment.Center };
                    ComboBox malzemeComboBox = new ComboBox { Width = 150, Margin = new Thickness(5, 0, 5, 0) };

                    
                    DataTable malzemelerDt = veritabaniIslemleri.TumMalzemeleriGetir();
                    foreach (DataRow row in malzemelerDt.Rows)
                    {
                        malzemeComboBox.Items.Add(row["MalzemeAdi"].ToString());
                    }
                    malzemeComboBox.SelectedItem = malzeme.malzemeAdi;

                    Label miktarLabel = new Label { Content = "Miktar:", VerticalAlignment = VerticalAlignment.Center };
                    TextBox miktarTextBox = new TextBox { Text = malzeme.miktar, Width = 75, Margin = new Thickness(5, 0, 5, 0) };

                    Label birimLabel = new Label { Content = "Birim:", VerticalAlignment = VerticalAlignment.Center };
                    ComboBox birimComboBox = new ComboBox { Width = 75, Margin = new Thickness(5, 0, 5, 0) };
                    birimComboBox.Items.Add("Gram");
                    birimComboBox.Items.Add("Litre");
                    birimComboBox.Items.Add("Kilo");

                    birimComboBox.SelectedItem = malzeme.birim;

                    malzemeItem.Children.Add(malzemeLabel);
                    malzemeItem.Children.Add(malzemeComboBox);
                    malzemeItem.Children.Add(miktarLabel);
                    malzemeItem.Children.Add(miktarTextBox);
                    malzemeItem.Children.Add(birimLabel);
                    malzemeItem.Children.Add(birimComboBox);

                    MalzemePanel.Children.Add(malzemeItem);
                }
            }
        }

        private void GuncelleButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem is DataRowView rowView)
            {
                int tarifID = Convert.ToInt32(rowView["TarifID"]);
                string tarifAdi = TarifAdiTextBox.Text;
                string kategori = ((ComboBoxItem)KategoriComboBox.SelectedItem)?.Content.ToString();
                int hazirlamaSuresi = int.TryParse(HazirlamaSuresiTextBox.Text, out int suresi) ? suresi : 0;
                string talimatlar = TalimatlarTextBox.Text;
                string kisiSayisi = KisiSayisiTextBox.Text;


                // Yeni malzemeler listesini oluşturur
           
                List<(string malzemeAdi, string miktar, string birim)> malzemeler = new List<(string, string, string)>();
                foreach (StackPanel item in MalzemePanel.Children)
                {
                    if (item.Children[1] is ComboBox malzemeComboBox && item.Children[3] is TextBox miktarTextBox && item.Children[5] is ComboBox birimComboBox)
                    {
                        string malzemeAdi = malzemeComboBox.SelectedItem?.ToString();
                        string miktar = miktarTextBox.Text;
                        string birim = birimComboBox.SelectedItem?.ToString();

                        
                        if (!string.IsNullOrWhiteSpace(malzemeAdi) && !string.IsNullOrWhiteSpace(miktar) && !string.IsNullOrWhiteSpace(birim))
                        {
                            malzemeler.Add((malzemeAdi, miktar, birim));
                        }
                    }
                }

               
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                veritabaniIslemleri.TarifGuncelle(tarifID, tarifAdi, kategori, hazirlamaSuresi, talimatlar, ResimYolu, kisiSayisi, malzemeler);

                
                AraButton_Click(sender, e);
            }
        }



        private void SilButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem is DataRowView rowView)
            {
                int tarifID = Convert.ToInt32(rowView["TarifID"]);

                
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                veritabaniIslemleri.TarifSil(tarifID);

               
                AraButton_Click(sender, e);
            }
        }










    }
}


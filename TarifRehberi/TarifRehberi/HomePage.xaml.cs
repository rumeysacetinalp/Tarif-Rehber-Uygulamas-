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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static TarifRehberi.HomePage;



namespace TarifRehberi
{
    /// <summary>
    /// HomePage.xaml etkileşim mantığı
    /// </summary>
    public partial class HomePage : Page
    {
        private List<Tarif> filteredTarifler;


        public HomePage()
        {
            InitializeComponent();
            
            TarifIsmiTextBox.GotFocus += RemovePlaceholderText;
            TarifIsmiTextBox.LostFocus += AddPlaceholderText;
            AddPlaceholderText(null, null); // Başlangıçta placeholder ekle
            TarifListView.ItemTemplate = CreateTarifListViewItemTemplate();

            AraButton_Click(null, null);
        }

  
        public class Tarif
        {
            public int TarifID { get; set; }
            public string TarifAdi { get; set; }
            public string Kategori { get; set; }
            public int HazirlamaSuresi { get; set; }
            public string KisiSayisi { get; set; }
            public string ResimYolu { get; set; }
            public int EslesmeYuzdesi { get; set; }
            public decimal ToplamMaliyet { get; set; }
            public List<string> EksikMalzemeler { get; set; }
            public bool EksikMalzemeVarMi => EksikMalzemeler != null && EksikMalzemeler.Count > 0;
            public decimal EksikMalzemeMaliyeti { get; set; }
            public int MalzemeSayisi { get; set; }
        }

        
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T tChild)
                {
                    return tChild;
                }

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        private void RemovePlaceholderText(object sender, RoutedEventArgs e)
        {
            if (TarifIsmiTextBox.Text == "Tarif ismi giriniz...")
            {
                TarifIsmiTextBox.Text = string.Empty;
                TarifIsmiTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void AddPlaceholderText(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TarifIsmiTextBox.Text))
            {
                TarifIsmiTextBox.Text = "Tarif ismi giriniz...";
                TarifIsmiTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void AraButton_Click(object sender, RoutedEventArgs e)
        {
            
            string arananTarifAdi = TarifIsmiTextBox.Text == "Tarif ismi giriniz..." ? string.Empty : TarifIsmiTextBox.Text;

           
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            DataTable dt = veritabaniIslemleri.TarifArama(arananTarifAdi, null);

           
            filteredTarifler = dt?.AsEnumerable().Select(row =>
            {
                int tarifID = row.Field<int>("TarifID"); 
                return new Tarif
                {
                    TarifID = tarifID,
                    TarifAdi = row.Field<string>("TarifAdi"),
                    Kategori = row.Field<string>("Kategori"),
                    HazirlamaSuresi = row.Field<int>("HazirlamaSuresi"),
                    KisiSayisi = row.Field<string>("KisiSayisi"),
                    ResimYolu = row.Field<string>("ResimYolu"),
                    EslesmeYuzdesi = 0,
                    ToplamMaliyet = veritabaniIslemleri.GetTarifMaliyeti(tarifID),
                    EksikMalzemeler = veritabaniIslemleri.EksikMalzemeleriKontrolEt(tarifID),
                    EksikMalzemeMaliyeti = veritabaniIslemleri.EksikMalzemelerinMaliyetiniHesapla(tarifID),
                    MalzemeSayisi = veritabaniIslemleri.TarifMalzemeSayisiGetir(tarifID)
                };
            }).ToList();

           
            TarifListView.ItemsSource = filteredTarifler;


          


        }

        
        private void TarifListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TarifListView.SelectedItem is not null)
            {
                Tarif selectedTarif = (Tarif)TarifListView.SelectedItem;
                int tarifID = selectedTarif.TarifID;


                TarifAdiTextBlock.Text = selectedTarif.TarifAdi;

                
                string resimYolu = selectedTarif.ResimYolu;
                if (!string.IsNullOrEmpty(resimYolu) && System.IO.File.Exists(resimYolu))
                {
                    TarifResmi.Source = new BitmapImage(new Uri(resimYolu, UriKind.Absolute));
                }
                else
                {
                    
                    TarifResmi.Source = null;
                }

                
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                DataTable detaylarTable = veritabaniIslemleri.GetTarifDetaylari(tarifID);

                if (detaylarTable?.Rows.Count > 0)
                {
                    
                    DataRow tarifBilgisi = detaylarTable.Rows[0];
                    YapilisTextBlock.Text = tarifBilgisi["Talimatlar"].ToString();

                   
                    var malzemeler = detaylarTable.AsEnumerable().Select(row => new
                    {
                        MalzemeAdi = row.Field<string>("MalzemeAdi"),
                        Miktar = row.Field<double>("MalzemeMiktar"),
                        Birim = row.Field<string>("malzemeBirim"),
                        EksikMi = selectedTarif.EksikMalzemeler.Contains(row.Field<string>("MalzemeAdi"))
                    }).ToList();

                    MalzemeListView.ItemsSource = malzemeler;

                   
                    decimal eksikMalzemeMaliyeti = veritabaniIslemleri.EksikMalzemelerinMaliyetiniHesapla(tarifID);
                    if (eksikMalzemeMaliyeti > 0)
                    {
                        YapilisTextBlock.Inlines.Add(new LineBreak());
                        YapilisTextBlock.Inlines.Add(new LineBreak());
                        YapilisTextBlock.Inlines.Add(new Run($"Eksik Malzemelerin Toplam Maliyeti: { eksikMalzemeMaliyeti:C}") { Foreground = Brushes.Red });
                    }
                }
                else
                {
                    MessageBox.Show("Tarif detayları bulunamadı.");
                }
            }
            else
            {
                
                TarifAdiTextBlock.Text = string.Empty;
                YapilisTextBlock.Text = string.Empty;
                MalzemeListView.ItemsSource = null;
                TarifResmi.Source = null;
            }
        }





        private void MalzemeSecimiButton_Click(object sender, RoutedEventArgs e)
        {
            // Veritabanından tüm malzemeleri al
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            DataTable dt = veritabaniIslemleri.TumMalzemeleriGetir();

            
            var malzemeler = dt?.AsEnumerable().Select(row => new
            {
                MalzemeAdi = row.Field<string>("MalzemeAdi")
            }).ToList();

            
            MalzemeSecimiListView.ItemsSource = malzemeler;

           
            MalzemeSecimiPopup.IsOpen = true;
        }

        private void MalzemeSecimiTamam_Click(object sender, RoutedEventArgs e)
        {
            
            var seciliMalzemeler = new List<string>();

            foreach (var item in MalzemeSecimiListView.Items)
            {
                ListViewItem listViewItem = (ListViewItem)MalzemeSecimiListView.ItemContainerGenerator.ContainerFromItem(item);
                if (listViewItem != null)
                {
                    CheckBox checkBox = FindVisualChild<CheckBox>(listViewItem);
                    if (checkBox != null && checkBox.IsChecked == true)
                    {
                        seciliMalzemeler.Add(checkBox.Content.ToString());
                    }
                }
            }

            if (seciliMalzemeler.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir malzeme seçiniz.");
                return;
            }

            
            var filtrelenmisTarifler = filteredTarifler.Select(t =>
            {
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                var tarifMalzemeleri = veritabaniIslemleri.TarifMalzemeleriGetir(t.TarifID);
                int eslesmeSayisi = seciliMalzemeler.Count(m => tarifMalzemeleri.Any(tm => tm.malzemeAdi == m));
                t.EslesmeYuzdesi = (eslesmeSayisi * 100) / seciliMalzemeler.Count;
                return t;
            }).Where(t => t.EslesmeYuzdesi > 0)
              .OrderByDescending(t => t.EslesmeYuzdesi).ToList();

           
            filteredTarifler = filtrelenmisTarifler;

            TarifListView.ItemsSource = filteredTarifler;

            MalzemeSecimiPopup.IsOpen = false;
        }

        private DataTemplate CreateTarifListViewItemTemplate()
        {
            var dataTemplate = new DataTemplate(typeof(Tarif));

            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            //borderFactory.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Colors.LightGray));

            borderFactory.SetValue(Border.BorderBrushProperty, new Binding("EksikMalzemeVarMi")
            {
                Converter = new EksikMalzemeToBorderBrushConverter()
            });

            borderFactory.SetValue(Border.PaddingProperty, new Thickness(15)); 
            borderFactory.SetValue(Border.MarginProperty, new Thickness(10)); 


           

            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            stackPanelFactory.SetValue(StackPanel.MarginProperty, new Thickness(10));

            var imageFactory = new FrameworkElementFactory(typeof(Image));
            imageFactory.SetBinding(Image.SourceProperty, new Binding("ResimYolu")
            {
                Converter = new UriToImageSourceConverter(),
                ConverterParameter = "Absolute"
            });
            imageFactory.SetValue(Image.HeightProperty, 150.0); 
            imageFactory.SetValue(Image.WidthProperty, 200.0);
            imageFactory.SetValue(Image.MarginProperty, new Thickness(0, 0, 10, 0));
            stackPanelFactory.AppendChild(imageFactory);

            var detailsStackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            detailsStackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);

            var textBlockFactory1 = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory1.SetBinding(TextBlock.TextProperty, new Binding("TarifAdi"));
            textBlockFactory1.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            detailsStackPanelFactory.AppendChild(textBlockFactory1);

            var textBlockFactory2 = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory2.SetBinding(TextBlock.TextProperty, new Binding("Kategori") { StringFormat = "Kategori: {0}" });
            detailsStackPanelFactory.AppendChild(textBlockFactory2);

            var textBlockFactory3 = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory3.SetBinding(TextBlock.TextProperty, new Binding("HazirlamaSuresi") { StringFormat = "Hazırlama Süresi: {0} dakika" });
            detailsStackPanelFactory.AppendChild(textBlockFactory3);

            var textBlockFactory4 = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory4.SetBinding(TextBlock.TextProperty, new Binding("KisiSayisi") { StringFormat = "{0} kişilik" });
            detailsStackPanelFactory.AppendChild(textBlockFactory4);

            var textBlockFactory5 = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory5.SetBinding(TextBlock.TextProperty, new Binding("EslesmeYuzdesi") { StringFormat = "Eşleşme Yüzdesi: {0}%" });
            detailsStackPanelFactory.AppendChild(textBlockFactory5);

           
            var textBlockFactory6 = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory6.SetBinding(TextBlock.TextProperty, new Binding("ToplamMaliyet") { StringFormat = "Maliyet: ₺{0}" }); 
            detailsStackPanelFactory.AppendChild(textBlockFactory6);


            
            var textBlockFactory7 = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory7.AppendChild(new FrameworkElementFactory(typeof(Run)) { Text = "Eksik Malzemelerin Maliyeti:" });
            textBlockFactory7.SetValue(TextBlock.ForegroundProperty, Brushes.Red);
            textBlockFactory7.SetValue(TextBlock.VisibilityProperty, new Binding("EksikMalzemeVarMi")
            {
                Converter = new BooleanToVisibilityConverter()
            });
            detailsStackPanelFactory.AppendChild(textBlockFactory7);

            var textBlockFactory8 = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory8.SetBinding(TextBlock.TextProperty, new Binding("EksikMalzemeMaliyeti")
            {
                StringFormat = "₺{0}",
                TargetNullValue = string.Empty
            });
            textBlockFactory8.SetValue(TextBlock.ForegroundProperty, Brushes.Red);
            textBlockFactory8.SetValue(TextBlock.VisibilityProperty, new Binding("EksikMalzemeVarMi")
            {
                Converter = new BooleanToVisibilityConverter()
            });
            detailsStackPanelFactory.AppendChild(textBlockFactory8);


            stackPanelFactory.AppendChild(detailsStackPanelFactory);

            borderFactory.AppendChild(stackPanelFactory);
            dataTemplate.VisualTree = borderFactory;
            return dataTemplate;
        }




        private void FiltrelemeButton_Click(object sender, RoutedEventArgs e)
        {
            FiltrelemePopup.IsOpen = true;
        }

        private void FiltrelemeTamam_Click(object sender, RoutedEventArgs e)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            
            FiltrelemePopup.IsOpen = false;

            
            int? minMalzemeSayisi = int.TryParse(MalzemeSayisiMinTextBox.Text, out int parsedMinMalzemeSayisi) ? parsedMinMalzemeSayisi : (int?)null;
            int? maxMalzemeSayisi = int.TryParse(MalzemeSayisiMaxTextBox.Text, out int parsedMaxMalzemeSayisi) ? parsedMaxMalzemeSayisi : (int?)null;

            string kategori = KategoriComboBox.SelectedItem is ComboBoxItem selectedItem ? selectedItem.Content.ToString() : null;

            int? minKisiSayisi = int.TryParse(KisiSayisiMinTextBox.Text, out int parsedMinKisiSayisi) ? parsedMinKisiSayisi : (int?)null;
            int? maxKisiSayisi = int.TryParse(KisiSayisiMaxTextBox.Text, out int parsedMaxKisiSayisi) ? parsedMaxKisiSayisi : (int?)null;
            decimal? minMaliyet = decimal.TryParse(MaliyetMinTextBox.Text, out decimal parsedMinMaliyet) ? parsedMinMaliyet : (decimal?)null;
            decimal? maxMaliyet = decimal.TryParse(MaliyetMaxTextBox.Text, out decimal parsedMaxMaliyet) ? parsedMaxMaliyet : (decimal?)null;

           
            var filtrelenmisTarifler = filteredTarifler.Where(t =>
                (string.IsNullOrEmpty(kategori) || t.Kategori == kategori) &&
                (!minKisiSayisi.HasValue || int.Parse(t.KisiSayisi) >= minKisiSayisi) &&
                (!maxKisiSayisi.HasValue || int.Parse(t.KisiSayisi) <= maxKisiSayisi) &&
                   (!minMalzemeSayisi.HasValue || t.MalzemeSayisi >= minMalzemeSayisi) && 
                   (!maxMalzemeSayisi.HasValue || t.MalzemeSayisi <= maxMalzemeSayisi) &&

                (!minMaliyet.HasValue || t.ToplamMaliyet >= minMaliyet) && 
                (!maxMaliyet.HasValue || t.ToplamMaliyet <= maxMaliyet)    
            ).ToList();

            
            filteredTarifler = filtrelenmisTarifler;

           
            TarifListView.ItemsSource = filteredTarifler;
        }


        private void FiltrelemeTemizle_Click(object sender, RoutedEventArgs e)
        {
            
            MaliyetMinTextBox.Text = string.Empty;
            MaliyetMaxTextBox.Text = string.Empty;
            MalzemeSayisiMinTextBox.Text = string.Empty;
            MalzemeSayisiMaxTextBox.Text = string.Empty;
            KisiSayisiMinTextBox.Text = string.Empty;
            KisiSayisiMaxTextBox.Text = string.Empty;

           
            KategoriComboBox.SelectedIndex = -1;
        }

        private void SiralamaButton_Click(object sender, RoutedEventArgs e)
        {
            
            SiralamaPopup.IsOpen = !SiralamaPopup.IsOpen;
        }

        private void SiralamaTamam_Click(object sender, RoutedEventArgs e)
        {
            
            MessageBox.Show("Sıralama seçenekleri uygulandı.");
        }



        private void SiralamaSecenek_Checked(object sender, RoutedEventArgs e)
        {
            
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            RadioButton selectedOption = sender as RadioButton;
            if (selectedOption != null)
            {
                string sortingCriteria = selectedOption.Tag.ToString();

              
                switch (sortingCriteria)
                {
                    case "MalzemeSayisiArtan":
                       
                        filteredTarifler = filteredTarifler?.OrderBy(t => veritabaniIslemleri.TarifMalzemeSayisiGetir(t.TarifID)).ToList();
                        break;
                    case "MalzemeSayisiAzalan":
                        
                        filteredTarifler = filteredTarifler?.OrderByDescending(t => veritabaniIslemleri.TarifMalzemeSayisiGetir(t.TarifID)).ToList();
                        break;
                    case "HazirlamaSureHizlidanYavasa":
                       
                        filteredTarifler = filteredTarifler?.OrderBy(t => t.HazirlamaSuresi).ToList();
                        break;
                    case "HazirlamaSureYavastanHizliya":
                        
                        filteredTarifler = filteredTarifler?.OrderByDescending(t => t.HazirlamaSuresi).ToList();
                        break;
                    case "TarifMaliyetArtan":
                        
                        filteredTarifler = filteredTarifler?.OrderBy(t => t.ToplamMaliyet).ToList();
                        break;

                    case "TarifMaliyetAzalan":
                       
                        filteredTarifler = filteredTarifler?.OrderByDescending(t => t.ToplamMaliyet).ToList();
                        break;
                }

                
                TarifListView.ItemsSource = filteredTarifler;
              MessageBox.Show("Seçilen sıralama: " + sortingCriteria);
            }
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "En Az" || textBox.Text == "En Çok")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.Foreground = Brushes.Gray;
            }
        }







    }

    public class UriToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            string uriString = value.ToString();
            if (!string.IsNullOrEmpty(uriString) && System.IO.File.Exists(uriString))
            {
                return new BitmapImage(new Uri(uriString, UriKind.Absolute));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class EksikMalzemeToBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool eksikMalzemeVarMi = (bool)value;
            
            return eksikMalzemeVarMi ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EksikMalzemeToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool eksikMi = (bool)value;
            return eksikMi ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool boolValue = (bool)value;
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}



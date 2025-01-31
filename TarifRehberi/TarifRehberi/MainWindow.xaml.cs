using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }




        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            HomeImageBorder.Visibility = Visibility.Collapsed;
            ContentArea.Navigate(new HomePage());
        }

        private void RecipesButton_Click(object sender, RoutedEventArgs e)
        {
            HomeImageBorder.Visibility = Visibility.Collapsed;
            ContentArea.Navigate(new RecipesPage());
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            HomeImageBorder.Visibility = Visibility.Collapsed;
            ContentArea.Navigate(new AboutPage());


        }
    }
}
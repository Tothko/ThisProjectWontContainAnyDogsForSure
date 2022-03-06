using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ThisProjectWontContainAnyDogsForSure
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Dictionary<string, int> GetPrimeType = new Dictionary<string, int>
            {
                { "Sequential", 0 },
                { "Parallel", 1 }
            };

            ModeSelectionComboBox.ItemsSource = GetPrimeType;
            ModeSelectionComboBox.DisplayMemberPath = "Key";
            ModeSelectionComboBox.SelectedValuePath = "Value";
        }

        private void ModeSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void TextBox_OnBeforeTextChanging(TextBox sender,
                                          TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private List<long> GetPrimesSequential(long first, long last)
        {
            List<long> SortedPrimes = new List<long>();
            for (long i = 0; i < last; i++)
            {
                if (IsPrime(i)) SortedPrimes.Add(i);

            }
            return SortedPrimes;
        }
        private List<long> GetPrimesParallel(long first, long last)
        {
            List<long> SortedPrimes = new List<long>();
            for (long i = 0; i < last; i++)
            {
                bool isPrime = false;
                Task.Factory.StartNew(() => isPrime = IsPrime(i));
                if (isPrime) SortedPrimes.Add(i);
            }
            return SortedPrimes;
        }
        public bool IsPrime(long n)
        {
            if(n == 2 || n == 3)
        return true;

            if (n <= 1 || n % 2 == 0 || n % 3 == 0)
                return false;

            for (int i = 5; i * i <= n; i += 6)
            {
                if (n % i == 0 || n % (i + 2) == 0)
                    return false;
            }

            return true;
        }
        

        private void MasterBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (ModeSelectionComboBox.SelectedValue)
            {
                case 0:
                    OutputDataGrid.ItemsSource = GetPrimesSequential(long.Parse(FirstInput.Text), long.Parse(SecondInput.Text));
                    break;
                case 1:
                    OutputDataGrid.ItemsSource = GetPrimesParallel(long.Parse(FirstInput.Text), long.Parse(SecondInput.Text));
                    break;
                default:
                    break;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    /// 
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        Stopwatch stopWatch;
        private string firstInputText;
        public int SelectedValue { get; set; }
        public string FirstInputText
        {
            get { return firstInputText; }
            set { firstInputText = value; }
        }
        CancellationToken token;
        CancellationTokenSource tokenSource;
        private string secondInputText;

        public event PropertyChangedEventHandler PropertyChanged;
        protected async void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High,
          () =>
          {
              PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
          });
        }

        public string SecondInputText
        {
            get { return secondInputText; }
            set { secondInputText = value; }
        }

        private List<long> outcomeList;

        private double maximumProgress;

        public double MaximumProgress
        {
            get { return maximumProgress; }
            set
            {
                maximumProgress = value;
                OnPropertyChanged("MaximumProgress");

            }
        }
        private double valueProgress;

        public double ValueProgress
        {
            get { return valueProgress; }
            set
            {
                valueProgress = value;
                OnPropertyChanged("ValueProgress");

            }
        }


        public List<long> OutcomeList
        {
            get { return outcomeList; }
            set
            {
                outcomeList = value;
            }
        }

        private string timerText;

        public string TimerText
        {
            get { return timerText; }
            set { timerText = value;
                OnPropertyChanged("TimerText");

            }
        }


        public MainPage()
        {
            this.InitializeComponent();
            Dictionary<string, int> GetPrimeType = new Dictionary<string, int>
            {
                { "Sequential", 0 },
                { "Parallel", 1 },
                { "ParallelAsync", 2 },
                { "ParallelAsyncAwait", 3 },
                { "Framework thing doesnt work", 4 },
                { "Manual multiple tasks", 5 }


            };

            ModeSelectionComboBox.ItemsSource = GetPrimeType;
            ModeSelectionComboBox.DisplayMemberPath = "Key";
            ModeSelectionComboBox.SelectedValuePath = "Value";
            stopWatch = new Stopwatch();
            tokenSource = new CancellationTokenSource();
           token = tokenSource.Token;

        }

        private void ModeSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedValue = (int)ModeSelectionComboBox.SelectedValue;
        }
        private void TextBox_OnBeforeTextChanging(TextBox sender,
                                          TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private List<long> GetPrimesSequential(long first, long last)
        {
            MaximumProgress = last;
            List<long> SortedPrimes = new List<long>();
            for (long i = first; i < last; i++)
            {
                if (i % (last / 100) == 0)  valueProgress = i;

                if (IsPrime(i)) SortedPrimes.Add(i);

            }
      

            return SortedPrimes;
        }
        private List<long> GetPrimesParallel(long first, long last)
        {
            MaximumProgress = last;

            List<long> SortedPrimes = new List<long>();
            for (long i = first; i < last; i++)
            {
                if (i % (last / 100) == 0) valueProgress = i;

                bool isPrime = false;
                Task.Factory.StartNew(() => isPrime = IsPrime(i));
                if (isPrime) SortedPrimes.Add(i);
            }
           

            return SortedPrimes;
        }
        private async Task<List<long>> GetPrimesParallelAsync(long first, long last)
        {
            MaximumProgress = last;

            List<long> SortedPrimes = new List<long>();
            for (long i = first; i < last; i++)
            {
                if (i % (last / 100) == 0) ValueProgress = i;

                bool isPrime = false;
                isPrime = IsPrime(i);
                if (isPrime) SortedPrimes.Add(i);
            }
          
            return SortedPrimes;
        }
        private async Task<List<long>> GetPrimesParallelAsyncAwait(long first, long last)
        {
            MaximumProgress = last;

            List<long> SortedPrimes = new List<long>();
            for (long i = first; i < last; i++)
            {
                
               if(i%(last/100)==0) ValueProgress = i;
                bool isPrime = false;
                isPrime = IsPrime(i);
                if (isPrime) SortedPrimes.Add(i);
            }
            

            await Task.CompletedTask;

            return SortedPrimes;
        }
        public bool IsPrime(long n)
        {
            if (n == 2 || n == 3)
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
            stopWatch.Restart();
            switch (SelectedValue)
            {
                case 0:
                    OutputDataGrid.ItemsSource = GetPrimesSequential(long.Parse(FirstInputText), long.Parse(SecondInputText));
                    DetermineTime();

                    break;
                case 1:
                    OutputDataGrid.ItemsSource = GetPrimesParallel(long.Parse(FirstInputText), long.Parse(SecondInputText));
                    DetermineTime();

                    break;
                case 2:
                    OutputDataGrid.ItemsSource = GetPrimesParallelAsync(long.Parse(FirstInputText), long.Parse(SecondInputText)).Result;
                    DetermineTime();

                    break;
                case 3:
                    OutputDataGrid.ItemsSource = GetPrimesParallelAsyncAwait(long.Parse(FirstInputText), long.Parse(SecondInputText)).Result;
                    DetermineTime();

                    break;
                case 4:

                    // Probably .Net Framework thing
                    // OutputDataGrid.ItemsSource = System.Threading.Dispatcher.InvokeAsync(() => GetPrimesSequential(long.Parse(FirstInput.Text), long.Parse(SecondInput.Text)));
                    break;
                case 5:

                    break;
                default:
                    break;
            }

        }

        private async void MasterBtnAsync_Click(object sender, RoutedEventArgs e)
        {
            stopWatch.Restart();
            switch (SelectedValue)
            {
                case 0:
                    await Task.Factory.StartNew(() => OutcomeList = GetPrimesSequential(long.Parse(FirstInputText), long.Parse(SecondInputText)));
                    OutputDataGrid.ItemsSource = OutcomeList;
                    DetermineTime();

                    break;
                case 1:
                    await Task.Factory.StartNew(() => OutcomeList = GetPrimesParallel(long.Parse(FirstInputText), long.Parse(SecondInputText)));
                    OutputDataGrid.ItemsSource = OutcomeList;
                    DetermineTime();

                    break;
                case 2:
                    await Task.Factory.StartNew(() => OutcomeList = GetPrimesParallelAsync(long.Parse(FirstInputText), long.Parse(SecondInputText)).Result);
                    DetermineTime();

                    break;
                case 3:
                    await Task.Factory.StartNew(() => OutcomeList = GetPrimesParallelAsyncAwait(long.Parse(FirstInputText), long.Parse(SecondInputText)).Result, token);
                    OutputDataGrid.ItemsSource = OutcomeList;
                    DetermineTime();
                    break;
                case 4:

                    // Probably .Net Framework thing
                    // OutputDataGrid.ItemsSource = System.Threading.Dispatcher.InvokeAsync(() => GetPrimesSequential(long.Parse(FirstInput.Text), long.Parse(SecondInput.Text)));
                    break;
                case 5:
                    long totalNumberOFTasks = long.Parse(SecondInputText) / 1000;
                    List<List<long>> ListOfOutcomeLists = new List<List<long>>();
                    for (int i = 0; i < totalNumberOFTasks; i++)
                    {
                        ListOfOutcomeLists.Add(new List<long>());
                    }
                    for (int i = 0; i < totalNumberOFTasks; i++)
                    {
                        
                        await Task.Factory.StartNew(() => ListOfOutcomeLists[i] = GetPrimesParallelAsyncAwait(long.Parse(SecondInputText) / totalNumberOFTasks * i, long.Parse(SecondInputText) / totalNumberOFTasks * (i+1)).Result, token);
                    }
                    List<long> SemiOutcomeLIst = new List<long>();
                    foreach (var item in ListOfOutcomeLists)
                    {
                        SemiOutcomeLIst.AddRange(item);
                    }
                    OutcomeList = SemiOutcomeLIst;
                    OutputDataGrid.ItemsSource = OutcomeList;
                    DetermineTime();
                    break;

                    
                default:
                    break;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
        }
        private void DetermineTime()
        {
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            TimerText = elapsedTime;
        }
    }
}

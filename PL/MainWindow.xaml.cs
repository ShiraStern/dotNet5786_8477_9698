using BO;
using PL.Order;
using System;
using System.Windows;
using System.Windows.Input;
using PL.Helpers;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // -------- Stage 7 Mutex --------
        private readonly ObserverMutex _clockMutex = new();   // stage 7
        private readonly ObserverMutex _configMutex = new();  // stage 7

        static int[] sums = s_bl.Order.GetOrdersStatusCounts(PL.Tools.UserContext.UserId);

        public int Open { get; set; } = sums[(int)OrderStatus.Open];
        public int InTreatment { get; set; } = sums[(int)OrderStatus.InTreatment];

        public static int Closed { get; set; } =
            sums[(int)OrderStatus.Delivered] +
            sums[(int)OrderStatus.Refused] +
            sums[(int)OrderStatus.Cancelled];

        // ---------------- Dependency Properties ----------------

        public DateTime CurrentTime
        {
            get => (DateTime)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }

        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(
                "CurrentTime",
                typeof(DateTime),
                typeof(MainWindow));

        public BO.Config Configuration
        {
            get => (BO.Config)GetValue(ConfigurationProperty);
            set => SetValue(ConfigurationProperty, value);
        }

        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register(
                "Configuration",
                typeof(BO.Config),
                typeof(MainWindow));

        // ---------------- Constructor ----------------

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += Window_Loaded;
            this.Closing += Window_Closed;
        }

        // ---------------- Clock Buttons ----------------

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
            => s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
            => s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
            => s_bl.Admin.ForwardClock(BO.TimeUnit.Day);

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
            => s_bl.Admin.ForwardClock(BO.TimeUnit.Month);

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
            => s_bl.Admin.ForwardClock(BO.TimeUnit.Year);

        // ---------------- Config ----------------

        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Configuration saved successfully!");
        }

        // ---------------- DB ----------------

        private void Button_InitializeDB(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to initialize the database?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            CloseOtherWindows();

            Mouse.OverrideCursor = Cursors.Wait;
            s_bl.Admin.InitializeDB();
            Mouse.OverrideCursor = null;

            MessageBox.Show("Database initialized!");
        }

        private void Button_ResetDB(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to reset the database?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            CloseOtherWindows();

            Mouse.OverrideCursor = Cursors.Wait;
            s_bl.Admin.ResetDB();
            Mouse.OverrideCursor = null;

            MessageBox.Show("Database reset!");
        }

        private static void CloseOtherWindows()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is not MainWindow)
                    window.Close();
            }
        }

        // ---------------- Navigation ----------------

        private void btnHandleOrders(object sender, RoutedEventArgs e)
            => new OrderListWindow().Show();

        private void btnHandleCourier(object sender, RoutedEventArgs e)
            => new Courier.CourierListWindow().Show();

        // ---------------- Observers (Stage 7) ----------------

        private void clockObserver()
        {
            if (_clockMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;

            _ = Dispatcher.BeginInvoke(async () =>
            {
                CurrentTime = s_bl.Admin.GetClock();

                if (await _clockMutex.UnsetLoadInProgressAndCheckRestartRequested())
                    clockObserver();
            });
        }

        private void configObserver()
        {
            if (_configMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;

            _ = Dispatcher.BeginInvoke(async () =>
            {
                Configuration = s_bl.Admin.GetConfig();

                if (await _configMutex.UnsetLoadInProgressAndCheckRestartRequested())
                    configObserver();
            });
        }

        // ---------------- Window Events ----------------

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            Configuration = s_bl.Admin.GetConfig();

            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }
    }
}

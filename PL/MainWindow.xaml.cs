using BO;
using PL.Helpers;
using PL.Order;
using System;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // -------- Stage 7 Mutex --------
        private readonly ObserverMutex _clockMutex = new();
        private readonly ObserverMutex _configMutex = new();

        // ---------------- Dependency Properties ----------------

        // Clock
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

        // Config
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

        // Simulator Interval (minutes)
        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval",
                typeof(int),
                typeof(MainWindow),
                new PropertyMetadata(1));

        // Is Simulator Running
        public bool IsSimulatorRunning
        {
            get => (bool)GetValue(IsSimulatorRunningProperty);
            set => SetValue(IsSimulatorRunningProperty, value);
        }

        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register(
                "IsSimulatorRunning",
                typeof(bool),
                typeof(MainWindow),
                new PropertyMetadata(false));

        // ---------------- Constructor ----------------

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            Interval = 1;
            IsSimulatorRunning = false;

            Loaded += Window_Loaded;
            Closing += Window_Closed;
        }

        // ---------------- Simulator ----------------

        private void btnStartStopSimulator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsSimulatorRunning)
                {
                    // Start
                    s_bl.Admin.StartSimulator(Interval);
                    IsSimulatorRunning = true;
                }
                else
                {
                    // Stop
                    s_bl.Admin.StopSimulator();
                    IsSimulatorRunning = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ---------------- Clock Buttons ----------------

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
            => s_bl.Admin.ForwardClock(TimeUnit.Minute);

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
            => s_bl.Admin.ForwardClock(TimeUnit.Hour);

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
            => s_bl.Admin.ForwardClock(TimeUnit.Day);

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
            => s_bl.Admin.ForwardClock(TimeUnit.Month);

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
            => s_bl.Admin.ForwardClock(TimeUnit.Year);

        // ---------------- Config ----------------

        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Configuration saved!");
        }

        // ---------------- DB ----------------

        private void Button_InitializeDB(object sender, RoutedEventArgs e)
        {
            if (IsSimulatorRunning)
                return;

            var result = MessageBox.Show(
                "Initialize database?",
                "Confirm",
                MessageBoxButton.YesNo);

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
            if (IsSimulatorRunning)
                return;

            var result = MessageBox.Show(
                "Reset database?",
                "Confirm",
                MessageBoxButton.YesNo);

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
            // Stop simulator on exit
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }

            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }
    }
}

using BlApi;
using BO;
using PL.Order;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PL.Helpers;

namespace PL.Courier
{
    public partial class CourierListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int _applicantId = PL.Tools.UserContext.UserId;

        private readonly ObserverMutex _courierListMutex = new(); // stage 7

        public BO.FilterCouriersByProperty FilterCouriers { get; set; }
            = BO.FilterCouriersByProperty.All;

        public BO.CourierInList? selectedCourier { get; set; }

        public IEnumerable<BO.CourierInList> CourierInList
        {
            get => (IEnumerable<BO.CourierInList>)GetValue(CourierInListProperty);
            set => SetValue(CourierInListProperty, value);
        }

        public static readonly DependencyProperty CourierInListProperty =
            DependencyProperty.Register(
                "CourierInList",
                typeof(IEnumerable<BO.CourierInList>),
                typeof(CourierListWindow),
                new PropertyMetadata(null));

        // ---------------- Observer ----------------

        private void CourierListObserver()
        {
            if (_courierListMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;

            _ = Dispatcher.BeginInvoke(async () =>
            {
                queryCourierList();

                if (await _courierListMutex
                    .UnsetLoadInProgressAndCheckRestartRequested())
                {
                    CourierListObserver();
                }
            });
        }

        // ---------------- Ctor ----------------

        public CourierListWindow()
        {
            InitializeComponent();

            this.Loaded += Window_Loaded;
            this.Closing += Window_Closed;
        }

        // ---------------- Window events ----------------

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Courier.AddObserver(CourierListObserver);
            queryCourierList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Courier.RemoveObserver(CourierListObserver);
        }

        // ---------------- Logic ----------------

        private void queryCourierList()
        {
            CourierInList =
                s_bl.Courier.GetCourierList(
                    _applicantId,
                    true,
                    FilterCouriers)!;
        }

        // ---------------- UI events ----------------

        private void CourierFilterComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            queryCourierList();
        }

        private void AddCourier_Click(object sender, RoutedEventArgs e)
        {
            new CourierWindow(_applicantId).Show();
        }

        private void selectCourier_MouseDoubleClick(
            object sender,
            MouseButtonEventArgs e)
        {
            if (selectedCourier == null)
            {
                MessageBox.Show("Please select a courier first.");
                return;
            }

            new CourierWindow(
                _applicantId,
                selectedCourier.ID).Show();
        }

        private void DeleteCourier_Click(object sender, RoutedEventArgs e)
        {
            var courier =
                ((FrameworkElement)sender).DataContext
                as BO.CourierInList;

            if (courier == null)
                return;

            if (MessageBox.Show(
                "Are you sure you want to delete this courier?",
                "Confirm Delete",
                MessageBoxButton.YesNo)
                != MessageBoxResult.Yes)
                return;

            try
            {
                int managerId = s_bl.Admin.GetConfig().ManagerID;

                s_bl.Courier.Delete(managerId, courier.ID);

                MessageBox.Show("Courier deleted successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

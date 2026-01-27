using BO;
using PL.Courier;
using PL.Order;
using System;
using System.Windows;
using PL.Helpers;
using DO;

namespace PL
{
    public partial class CourierMainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private bool flagAddedObserver = false;

        private readonly ObserverMutex _courierMutex = new(); // stage 7
        private readonly ObserverMutex _orderMutex = new();   // stage 7

        private int _currentOrderId = 0;

        // ---------------- Properties ----------------

        public BO.Courier CurrentCourier
        {
            get => (BO.Courier)GetValue(CurrentCourierProperty);
            set => SetValue(CurrentCourierProperty, value);
        }

        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register(
                "CurrentCourier",
                typeof(BO.Courier),
                typeof(CourierMainWindow),
                new PropertyMetadata(null));

        public BO.OrderInProgress? OrderInProgress { get; set; }

        public BO.DeliveryTerminationType
            deliveryTerminationSelectedItem
        { get; set; }

        public string CourierName { get; set; }

        // ---------------- Ctor ----------------

        public CourierMainWindow()
        {
            InitializeComponent();

            int id = PL.Tools.UserContext.UserId;

            CurrentCourier =
                s_bl.Courier.GetDetails(id, id);

            CourierName = CurrentCourier.FullName;

            OrderInProgress = CurrentCourier.OrderInProgress;

            _currentOrderId = OrderInProgress?.orderId ?? 0;

            DataContext = this;

            Loaded += Window_Loaded;
            Closing += Window_Closed;
        }

        // ---------------- Observers ----------------

        private void RefreshCourierObserver()
        {
            if (_courierMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;

            _ = Dispatcher.BeginInvoke(async () =>
            {
                int id = PL.Tools.UserContext.UserId;

                CurrentCourier =
                    s_bl.Courier.GetDetails(id, id);

                OrderInProgress =
                    CurrentCourier.OrderInProgress;

                // handle order observer change
                UpdateOrderObserver();

                if (await _courierMutex
                    .UnsetLoadInProgressAndCheckRestartRequested())
                {
                    RefreshCourierObserver();
                }
            });
        }

        private void RefreshOrderObserver()
        {
            if (_orderMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;

            _ = Dispatcher.BeginInvoke(async () =>
            {
                OrderInProgress =
                    CurrentCourier.OrderInProgress;

                if (await _orderMutex
                    .UnsetLoadInProgressAndCheckRestartRequested())
                {
                    RefreshOrderObserver();
                }
            });
        }

        // ---------------- Observer Management ----------------

        private void UpdateOrderObserver()
        {
            int newOrderId =
                CurrentCourier.OrderInProgress?.orderId ?? 0;

            if (newOrderId == _currentOrderId)
                return;

            // remove old
            if (_currentOrderId != 0)
                s_bl.Order.RemoveObserver(
                    _currentOrderId,
                    RefreshOrderObserver);

            _currentOrderId = newOrderId;

            // add new
            if (_currentOrderId != 0)
                s_bl.Order.AddObserver(
                    _currentOrderId,
                    RefreshOrderObserver);
        }

        // ---------------- Window Events ----------------

        private void Window_Loaded(object sender, EventArgs e)
        {
            int id = PL.Tools.UserContext.UserId;

            s_bl.Courier.AddObserver(id, RefreshCourierObserver);

            if (CurrentCourier.OrderInProgress is not null)
            {
                flagAddedObserver= true;
                s_bl.Order.AddObserver(
                    _currentOrderId,
                    RefreshOrderObserver);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            int id = PL.Tools.UserContext.UserId;

            s_bl.Courier.RemoveObserver(id, RefreshCourierObserver);

            if (flagAddedObserver)
            {
                s_bl.Order.RemoveObserver(
                    _currentOrderId,
                    RefreshOrderObserver);
                flagAddedObserver= false;
            }
        }

        // ---------------- Buttons ----------------

        private void viewAndUpdateDetails(object sender, RoutedEventArgs e)
        {
            new CourierWindow(
                CurrentCourier.ID,
                CurrentCourier.ID).Show();
        }

        private void Button_EndOrderHandle(object sender, RoutedEventArgs e)
        {
            if (OrderInProgress is null)
                return;

            s_bl.Order.EndOrderHandle(
                PL.Tools.UserContext.UserId,
                PL.Tools.UserContext.UserId,
                OrderInProgress.orderId,
                OrderInProgress.DeliveryId,
                (DO.DeliveryTermintionType)
                    deliveryTerminationSelectedItem);
            MessageBox.Show($"Order delivered");

        }

        private void Button_OrderSelection(object sender, RoutedEventArgs e)
        {
            if (CurrentCourier.Active)
                new OpenOrderList_Window().Show();
            else
            MessageBox.Show($"Only active courier can choose an order");
        }

        private void Button_DeliveriesHisrory(object sender, RoutedEventArgs e)
        {
            new DeliveriesPerCourier_Window().Show();
        }
    }
}

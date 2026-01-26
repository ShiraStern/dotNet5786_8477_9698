using BlApi;
using BO;
using System;
using System.Windows;
using PL.Helpers;

namespace PL.Courier
{
    public partial class CourierWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        int _applicantId;

        private bool IsAddMode;

        private readonly ObserverMutex _courierMutex = new(); // stage 7

        public string ButtonText { get; set; }

        // Dependency Property
        public BO.Courier CurrentCourier
        {
            get => (BO.Courier)GetValue(CurrentCourierProperty);
            set => SetValue(CurrentCourierProperty, value);
        }

        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register(
                "CurrentCourier",
                typeof(BO.Courier),
                typeof(CourierWindow),
                new PropertyMetadata(null));

        // ---------------- Ctor ----------------

        public CourierWindow(int userId, int courierId = 0)
        {
            InitializeComponent();

            _applicantId = userId;

            if (courierId == 0)
            {
                IsAddMode = true;

                CurrentCourier = new BO.Courier
                {
                    ID = 0
                };

                ButtonText = "Add";
            }
            else
            {
                IsAddMode = false;

                CurrentCourier =
                    s_bl.Courier.GetDetails(_applicantId, courierId);

                ButtonText = "Update";
            }

            DataContext = this;

            Loaded += CourierWindow_Loaded;
            Closing += CourierWindow_Closing;
        }

        // ---------------- Observer ----------------

        private void RefreshCourier()
        {
            if (_courierMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;

            _ = Dispatcher.BeginInvoke(async () =>
            {
                int id = CurrentCourier!.ID;

                CurrentCourier =
                    s_bl.Courier.GetDetails(_applicantId, id);

                if (await _courierMutex
                    .UnsetLoadInProgressAndCheckRestartRequested())
                {
                    RefreshCourier();
                }
            });
        }

        // ---------------- Window events ----------------

        private void CourierWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsAddMode)
                s_bl.Courier.AddObserver(CurrentCourier.ID, RefreshCourier);
        }

        private void CourierWindow_Closing(
            object? sender,
            System.ComponentModel.CancelEventArgs e)
        {
            if (CurrentCourier!.ID != 0)
                s_bl.Courier.RemoveObserver(CurrentCourier.ID, RefreshCourier);
        }

        // ---------------- Buttons ----------------

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentCourier.FullName))
                {
                    MessageBox.Show("Full name is required.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentCourier.PhoneNember))
                {
                    MessageBox.Show("Phone number is required.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentCourier.Email))
                {
                    MessageBox.Show("Email is required.");
                    return;
                }

                if (!(PL.Tools.ValidateFullName(CurrentCourier.FullName) &&
                      PL.Tools.IsValidPhone(CurrentCourier.PhoneNember) &&
                      PL.Tools.IsValidEmail(CurrentCourier.Email)))
                {
                    return;
                }

                if (IsAddMode)
                {
                    CurrentCourier.EmploymentStartDate = DateTime.Now;

                    s_bl.Courier.AddCourier(
                        _applicantId,
                        CurrentCourier);

                    MessageBox.Show("Courier added successfully!");
                }
                else
                {
                    s_bl.Courier.UpdateDetails(
                        _applicantId,
                        CurrentCourier);

                    MessageBox.Show("Courier updated successfully!");
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

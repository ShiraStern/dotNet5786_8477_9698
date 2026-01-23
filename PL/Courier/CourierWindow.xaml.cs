using BlApi;
using BO;
using System;
using System.Windows;

namespace PL.Courier
{
    public partial class CourierWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int _applicantId = UserContext.UserId;

        private bool IsAddMode;   //  מצב המסך: true = הוספה, false = עדכון

        public string ButtonText { get; set; }

        // Dependency Property עבור כל האובייקט
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

        //public CourierWindow(int applicantId, BO.Courier courier)
        //{
        //    InitializeComponent();

        //    _applicantId = applicantId;

        //    // הכנסת השליח לתוך ה־DependencyProperty
        //    CurrentCourier = courier;

        //    // קביעת טקסט הכפתור לפי מצב המסך
        //    ButtonText = courier.ID == 0 ? "Add" : "Update";

        //    DataContext = this;
        //}
        private void CourierWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCourier!.ID != 0)
                s_bl.Courier.AddObserver(CurrentCourier.ID, RefreshCourier);
        }
        public CourierWindow(int userId, int courierId = 0)
        {
            InitializeComponent();
            _applicantId = userId;
            if (courierId == 0)
            {
                // מצב הוספה
                IsAddMode = true;

                CurrentCourier = new BO.Courier
                {
                    ID = 0
                };
                ButtonText = "Add";
            }
            else
            {
                // מצב עדכון
                IsAddMode = false;

                CurrentCourier = s_bl.Courier.GetDetails(managerId, courierId);
                ButtonText = "Update";
            }

            DataContext = this;

            this.Loaded += CourierWindow_Loaded;
            this.Closing += CourierWindow_Closing;
        }



        private void RefreshCourier()
        {
            int id = CurrentCourier!.ID;
            CurrentCourier = s_bl.Courier.GetDetails(_applicantId, id);
        }


        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // בדיקות בסיסיות
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

                if (IsAddMode)
                {
                    // הוספה
                    s_bl.Courier.AddCourier(_applicantId, CurrentCourier);
                    MessageBox.Show("Courier added successfully!");
                }
                else
                {
                    // עדכון
                    BlApi.Factory.Get().Courier.UpdateDetails(_applicantId, CurrentCourier);
                    MessageBox.Show("Courier updated successfully!");
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CourierWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CurrentCourier!.ID != 0)
                s_bl.Courier.RemoveObserver(CurrentCourier.ID, RefreshCourier);
        }

    }
}

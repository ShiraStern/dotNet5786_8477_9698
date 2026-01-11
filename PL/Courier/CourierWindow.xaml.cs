using BlApi;
using BO;
using System;
using System.Windows;

namespace PL.Courier
{
    public partial class CourierWindow : Window
    {
        private int _applicantId;

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

        public CourierWindow(int applicantId, BO.Courier courier)
        {
            InitializeComponent();

            _applicantId = applicantId;

            // הכנסת השליח לתוך ה־DependencyProperty
            CurrentCourier = courier;

            // קביעת טקסט הכפתור לפי מצב המסך
            ButtonText = courier.ID == 0 ? "Add" : "Update";

            DataContext = this;
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

                if (CurrentCourier.ID == 0)
                {
                    // הוספה
                    BlApi.Factory.Get().courier.AddCourier(_applicantId, CurrentCourier);
                    MessageBox.Show("Courier added successfully!");
                }
                else
                {
                    // עדכון
                    BlApi.Factory.Get().courier.UpdateDetails(_applicantId, CurrentCourier);
                    MessageBox.Show("Courier updated successfully!");
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

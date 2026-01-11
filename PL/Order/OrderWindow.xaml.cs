using BlApi;
using BO;
using System;
using System.Windows;

namespace PL.Order
{
    public partial class OrderWindow : Window
    {
        private int _applicantId;

        public string ButtonText { get; set; }

        // DependencyProperty עבור אובייקט ההזמנה
        public BO.Order CurrentOrder
        {
            get => (BO.Order)GetValue(CurrentOrderProperty);
            set => SetValue(CurrentOrderProperty, value);
        }

        public static readonly DependencyProperty CurrentOrderProperty =
            DependencyProperty.Register(
                "CurrentOrder",
                typeof(BO.Order),
                typeof(OrderWindow),
                new PropertyMetadata(null));

        public OrderWindow(int applicantId, BO.Order order)
        {
            InitializeComponent();

            _applicantId = applicantId;

            // הכנסת ההזמנה לתוך ה־DependencyProperty
            CurrentOrder = order;

            // קובע טקסט לכפתור
            ButtonText = order.ID == 0 ? "Add" : "Update";

            DataContext = this;
        }



        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // בדיקות בסיסיות
                if (string.IsNullOrWhiteSpace(CurrentOrder.FullNameOfTheInviter))
                {
                    MessageBox.Show("Full name of inviter is required.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentOrder.OrderersPhoneNumber))
                {
                    MessageBox.Show("Phone number is required.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentOrder.FullAddressOfTheOrder))
                {
                    MessageBox.Show("Address is required.");
                    return;
                }

                if (CurrentOrder.ID == 0)
                {
                    // הוספת הזמנה חדשה
                    BlApi.Factory.Get().order.AddOrder(_applicantId, CurrentOrder);

                    MessageBox.Show("Order added successfully!");
                }
                else
                {
                    // עדכון הזמנה קיימת
                    BlApi.Factory.Get().order.UpdateDetails(_applicantId, CurrentOrder);

                    MessageBox.Show("Order updated successfully!");
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

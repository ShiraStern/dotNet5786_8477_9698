using BlApi;
using BO;
using System;
using System.Windows;

namespace PL.Order
{
    public partial class OrderWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int _applicantId = s_bl.Admin.GetConfig().ManagerID;
        public BO.CourierInList selectedOrder { get; set; }


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

        

        public OrderWindow(int orderId = 0)
        {
            InitializeComponent();

            if (orderId == 0)
            {
                // מצב הוספה
                CurrentOrder = new BO.Order
                {
                    ID = 0
                };
                ButtonText = "Add";
            }
            else
            {
                // מצב עדכון
                CurrentOrder = s_bl.Order.GetDetails(_applicantId ,orderId);
                ButtonText = "Update";
            }

            DataContext = this;
            this.Loaded += OrderWindow_Loaded;
            this.Closing += OrderWindow_Closing;

        }

        private void RefreshOrder()
        {
            int id = CurrentOrder!.ID;
            CurrentOrder = s_bl.Order.GetDetails(_applicantId, id);
        }

        private void OrderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentOrder!.ID != 0)
                s_bl.Order.AddObserver(CurrentOrder.ID, RefreshOrder);
        }
        private void OrderWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CurrentOrder!.ID != 0)
                s_bl.Order.RemoveObserver(CurrentOrder.ID, RefreshOrder);
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
                    BlApi.Factory.Get().Order.AddOrder(_applicantId, CurrentOrder);

                    MessageBox.Show("Order added successfully!");
                }
                else
                {
                    // עדכון הזמנה קיימת
                    BlApi.Factory.Get().Order.UpdateDetails(_applicantId, CurrentOrder);

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

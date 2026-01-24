using BlApi;
using BO;
using System;
using System.Windows;

namespace PL.Order
{
    public partial class OrderWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();
        private int _applicantId = UserContext.UserId;
       
        public BO.CourierInList selectedOrder { get; set; }
        public BO.OrderProperties OrderProperties { get; set; } = BO.OrderProperties.None;

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
            ButtonText = orderId == 0 ? "Add" : "Update";

            DataContext = this;
            InitializeComponent();

            if (orderId == 0)
            {
                CurrentOrder = new BO.Order
                {
                    ID = 0,
                    OrderOpenDate = s_bl.Admin.GetClock(),
                    OrderStatus = BO.OrderStatus.Open
                };
            }
            else
            {
                // update mode
                CurrentOrder = s_bl.Order.GetDetails(_applicantId, orderId);
            }

            this.Loaded += OrderWindow_Loaded; // single subscription
            this.Closing += OrderWindow_Closing;
          
        }

        private void RefreshOrder()
            =>CurrentOrder = s_bl.Order.GetDetails(_applicantId, CurrentOrder!.ID);
        

        private void OrderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //if (CurrentOrder!.ID != 0)
                s_bl.Order.AddObserver(CurrentOrder.ID, RefreshOrder);
        }
        private void OrderWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (CurrentOrder!.ID != 0)
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
                tool

                if (CurrentOrder.ID == 0 )
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

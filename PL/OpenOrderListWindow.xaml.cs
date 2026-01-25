using BO;
using PL.Courier;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;

namespace PL.Order
{
    /// <summary>
    /// Interaction logic for OrderListWindow.xaml
    /// </summary>
    public partial class OpenOrderListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int _applicantId = PL.Tools.UserContext.UserId;
        public BO.OrderInList selectedOrder { get; set; }

        public BO.OrderType? filterOrderType { get; set; } = null;
        public BO.OrderStatus? filterOrderProperties { get; set; }=null;


        public IEnumerable<BO.OpenOrderInList> OrderInList
        {
            get { return (IEnumerable<BO.OpenOrderInList>)GetValue(OrderInListProperty); }
            set { SetValue(OrderInListProperty, value); }
        }

        public static readonly DependencyProperty OrderInListProperty =
            DependencyProperty.Register("OrderInList", typeof(IEnumerable<BO.OpenOrderInList>),
                typeof(OpenOrderListWindow), new PropertyMetadata(null));


        
        public OpenOrderListWindow()
        {
            try
            {
                 InitializeComponent();
                OrderInList = s_bl.Order.GetList_OpenOrderInList();
            }
            catch(BlDoesNotExistException)
            {
                MessageBoxResult result = MessageBox.Show(
                "Failed to load data." ,
                " Would you like to try again?",
                MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    s_bl.Admin.InitializeDB(); // retry logic
                }
                this.Close();
            }
            this.Loaded += Window_Loaded; // single subscription
            this.Closing += Window_Closed;
        }


       
        private void queryOrderList()
        {
            if (filterOrderType is not null)
            {
                OrderInList = s_bl.Order.GetList_OpenOrderInList().Where(x=> x.OrderType== filterOrderType);
            }
            else if (filterOrderProperties is not null)
            {
                OrderInList = s_bl.Order.GetList_OpenOrderInList().Where(x => x.OrderProperties == filterOrderProperties);
            }
            else
            {
                OrderInList = s_bl.Order.GetList_OpenOrderInList();
            }
        }


        private void OrderListObserver()
            => queryOrderList();


        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Order.AddObserver(OrderListObserver);


        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Order.RemoveObserver(OrderListObserver);

       

        private void selectOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (selectedOrder == null)
            {
                MessageBox.Show("Please select a courier first.");
                return;
            }

            new OrderWindow(selectedOrder.OrderId).Show();
        }


        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var order = ((FrameworkElement)sender).DataContext as BO.OrderInList;

            if (order == null)

                return;

            if (MessageBox.Show("Are you sure you want to delete this order?",
                                "Confirm Delete",
                                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Order.CancelOrder(_applicantId, order.OrderId);
                    
                    MessageBox.Show("Order deleted successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OrderTypeFilterComboBox(object sender, SelectionChangedEventArgs e)
        {
            filterOrderStatus = null;
            queryOrderList();
        }

        private void OrderStatusFilterComboBox(object sender, SelectionChangedEventArgs e)
        {
            filterOrderType = null;
            queryOrderList();
        }
    }

}

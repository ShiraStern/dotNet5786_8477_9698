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
    public partial class OrderListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int _applicantId = UserContext.UserId;
        public BO.OrderInList selectedOrder { get; set; }

        public BO.OrderType? filterOrderType { get; set; } = null;
        public BO.OrderStatus? filterOrderStatus { get; set; }=null;


        public IEnumerable<BO.OrderInList> OrderInList
        {
            get { return (IEnumerable<BO.OrderInList>)GetValue(OrderInListProperty); }
            set { SetValue(OrderInListProperty, value); }
        }

        public static readonly DependencyProperty OrderInListProperty =
            DependencyProperty.Register("OrderInList", typeof(IEnumerable<BO.OrderInList>),
                typeof(OrderListWindow), new PropertyMetadata(null));



        public OrderListWindow()
        {
            try
            {
                
                InitializeComponent();
                OrderInList = s_bl.Order.GetOrderList(managerId);

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
        }


        private void queryOrderList()
        {
            if (filterOrderType is not null)
                OrderInList = s_bl.Order.GetOrderList(managerId, filterOrdersByProperty.OrderType, filterOrderType);
            if (filterOrderStatus is not null)
                OrderInList = s_bl.Order.GetOrderList(managerId, filterOrdersByProperty.OrderStatus, filterOrderStatus);
        }


        private void courseListObserver()
            => queryOrderList();


        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Order.AddObserver(courseListObserver);


        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Order.RemoveObserver(courseListObserver);

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().Show();
        }


        //private void EditOrder_Click(object sender, RoutedEventArgs e)
        //{
        //    // שליפת פריט נבחר
        //    var selected = OrderListView.SelectedItem as BO.OrderInList;

        //    if (selected == null)
        //    {
        //        MessageBox.Show("Please select an order first.");
        //        return;
        //    }

        //    // פתיחת החלון במצב 'עדכון'
        //    new OrderWindow().Show();
        //}

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
                    s_bl.Order.Delete(managerId, order.OrderId);
                    queryOrderList(); // רענון הרשימה
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

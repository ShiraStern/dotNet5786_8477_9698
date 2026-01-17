using BO;
using PL.Courier;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PL.Order
{
    /// <summary>
    /// Interaction logic for OrderListWindow.xaml
    /// </summary>
    public partial class OrderListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int managerId = s_bl.Admin.GetConfig().ManagerID;

        public BO.OrderInList selectedOrder { get; set; }


        public BO.filterOrdersByProperty filterOrdersByProp { get; set; } = BO.filterOrdersByProperty.All;
        public object filterPropertyKeys { get; set; }

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
            InitializeComponent();
        }


        private void queryOrderList()
        {
            int managerId = s_bl.Admin.GetConfig().ManagerID;

            OrderInList = s_bl.Order.GetOrderList(managerId, filterOrdersByProp)!;
        }


        private void courseListObserver()
            => queryOrderList();


        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Order.AddObserver(courseListObserver);


        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Order.RemoveObserver(courseListObserver);


        private void OrderFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryOrderList();
        }

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

        private void selectCourier_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                    int managerId = s_bl.Admin.GetConfig().ManagerID;

                    s_bl.Order.Delete(managerId, order.OrderId);

                    MessageBox.Show("Order deleted successfully!");

                    queryOrderList(); // רענון הרשימה
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }

}

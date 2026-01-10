using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Order
{
    /// <summary>
    /// Interaction logic for OrderListWindow.xaml
    /// </summary>
    public partial class OrderListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.filterOrdersByProperty filterOrdersByProp  { get; set; } = BO.filterOrdersByProperty.OrderStatus;

        public IEnumerable<BO.OrderInList> OrderInList
        {
            get { return (IEnumerable<BO.OrderInList>)GetValue(OrderInListProperty); }
            set { SetValue(OrderInListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OrderInList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrderInListProperty =
            DependencyProperty.Register("OrderInList", typeof(IEnumerable<BO.OrderInList>),
                typeof(OrderListWindow), new PropertyMetadata(null));



        public OrderListWindow()
        {
            InitializeComponent();
        }


        private void queryOrderList()
        {
            int managetID = s_bl.Admin.GetConfig().ManagerID;
            OrderInList = (filterOrdersByProp == BO.filterOrdersByProperty.OrderStatus) ?
                s_bl?.order!.GetOrderList(managetID, null, filterOrdersByProperty.OrderStatus)!
                : filterOrdersByProp == BO.filterOrdersByProperty.OrderType ?
                s_bl?.order!.GetOrderList(managetID, null, BO.filterOrdersByProperty.OrderType)! :
                s_bl?.order!.GetOrderList(managetID, null, BO.filterOrdersByProperty.DeliveryType)!;
        }
        private void courseListObserver()
            => queryOrderList();
 
private void Window_Loaded(object sender, RoutedEventArgs e)
    => s_bl.order.AddObserver(courseListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.order.RemoveObserver(courseListObserver);

        private void OrderFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryOrderList();
        }
    }
}

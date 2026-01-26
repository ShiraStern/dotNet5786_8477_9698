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

namespace PL
{
    /// <summary>
    /// Interaction logic for OpenOrderList_Window.xaml
    /// </summary>
    public partial class OpenOrderList_Window : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int _applicantId = PL.Tools.UserContext.UserId;
        public BO.OrderInList selectedOrder { get; set; }

        public BO.OrderType? filterOrderType { get; set; } = null;
        public BO.OrderProperties? filterOrderProperties { get; set; } = null;

        public IEnumerable<BO.OpenOrderInList> OpenOrderInList
        {
            get { return (IEnumerable<BO.OpenOrderInList>)GetValue(OrderInListProperty); }
            set { SetValue(OrderInListProperty, value); }
        }

        public static readonly DependencyProperty OrderInListProperty =
            DependencyProperty.Register("OpenOrderInList", typeof(IEnumerable<BO.OpenOrderInList>),
                typeof(OpenOrderList_Window), new PropertyMetadata(null));




        public OpenOrderList_Window()
        { 
            InitializeComponent();
            try
            {
               
                OpenOrderInList = s_bl.Order.GetList_OpenOrderInList();
            }
             catch(BlDoesNotExistException)
            {
                MessageBoxResult result = MessageBox.Show(
                "Failed to load data.",
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
                OpenOrderInList = s_bl.Order.GetList_OpenOrderInList().Where(x => x.OrderType == filterOrderType);
            }
            else if (filterOrderProperties is not null)
            {
                OpenOrderInList = s_bl.Order.GetList_OpenOrderInList().Where(x => x.OrderProperties == filterOrderProperties);
            }
            else
            {
                OpenOrderInList = s_bl.Order.GetList_OpenOrderInList();
            }
        }


        private void OrderListObserver()
            => queryOrderList();


        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Order.AddObserver(OrderListObserver);


        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Order.RemoveObserver(OrderListObserver);





        private void ChooseOrder_Click(object sender, RoutedEventArgs e)
        {
            var order = ((FrameworkElement)sender).DataContext as BO.OpenOrderInList;

            if (order == null)

                return;

            if (MessageBox.Show("Are you sure you want to ship this order?",
                                "Confirm Choice",
                                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Order.HandleOrder(_applicantId, _applicantId, order.OrderId);

                    MessageBox.Show("Order deleted successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OrderTypeFilterComboBox(object sender, SelectionChangedEventArgs e)
        => queryOrderList();

    }
}

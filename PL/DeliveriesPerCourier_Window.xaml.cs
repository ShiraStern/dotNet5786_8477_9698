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
    /// Interaction logic for DeliveriesPerCourier_Window.xaml
    /// </summary>
    public partial class DeliveriesPerCourier_Window : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int _applicantId = PL.Tools.UserContext.UserId;

        public BO.OrderType? filterOrderType { get; set; } = null;

        public IEnumerable<BO.DeliveryPerOrderInList> DeliveriesPerOrderInList
        {
            get { return (IEnumerable<BO.DeliveryPerOrderInList>)GetValue(OrderInListProperty); }
            set { SetValue(OrderInListProperty, value); }
        }

        public static readonly DependencyProperty OrderInListProperty =
            DependencyProperty.Register("DeliveriesPerOrderInList", typeof(IEnumerable<BO.DeliveryPerOrderInList>),
                typeof(DeliveriesPerCourier_Window), new PropertyMetadata(null));


        public DeliveriesPerCourier_Window()
        {
            InitializeComponent();
            try
            {
                DeliveriesPerOrderInList = s_bl.Courier.GetDeliveryPerOrderInLists(_applicantId);
            }
            catch (BlDoesNotExistException)
            {
                MessageBoxResult result = MessageBox.Show(

                " Would you like to reset DB?", "Failed to load data.",
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
                DeliveriesPerOrderInList = s_bl.Courier.GetDeliveryPerOrderInLists(_applicantId);
            }
            
            else
            {
                DeliveriesPerOrderInList = s_bl.Courier.GetDeliveryPerOrderInLists(_applicantId);
            }
        }


        private void OrderListObserver()
            => queryOrderList();


        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Order.AddObserver(OrderListObserver);


        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Order.RemoveObserver(OrderListObserver);


        private void OrderTypeFilterComboBox(object sender, SelectionChangedEventArgs e)
        => queryOrderList();

    }

}

using BO;
using PL.Courier;
using PL.Order;
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
    /// Interaction logic for CourierMainWindow.xaml
    /// </summary>
    public partial class CourierMainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Courier CurrentCourier
        {
            get => (BO.Courier)GetValue(CurrentCourierProperty);
            set => SetValue(CurrentCourierProperty, value);
        }
        
        public BO.DeliveryTerminationType deliveryTerminationSelectedItem { get; set; }


        public string CourierName { get; set; }

        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register(
                "CurrentCourier",
                typeof(BO.Courier),
                typeof(CourierMainWindow),
                new PropertyMetadata(null));

        public  BO.OrderInProgress? OrderInProgress { get; set; } 
        public CourierMainWindow()
        {
            CurrentCourier = s_bl.Courier.GetDetails(PL.Tools.UserContext.UserId, PL.Tools.UserContext.UserId);
            CourierName = CurrentCourier.FullName;
            OrderInProgress=CurrentCourier.OrderInProgress;
            DataContext = this;
            InitializeComponent();
            this.Loaded += Window_Loaded;
            this.Closing += Window_Closed;
        }

        private void viewAndUpdateDetails(object sender, RoutedEventArgs e)
        {
            new CourierWindow(CurrentCourier.ID, CurrentCourier.ID).Show();    
        }

        private void RefreshCoureirObserver()
        { 
            CurrentCourier = s_bl.Courier.GetDetails(PL.Tools.UserContext.UserId, CurrentCourier.ID);
            OrderInProgress = CurrentCourier.OrderInProgress;
        }
        private void RefreshOrderObserver()
            =>OrderInProgress = CurrentCourier.OrderInProgress;
      
        

        private void Button_OpenOrderListWindow(object sender, RoutedEventArgs e)
        {
          new OpenOrderListWindow().Show();  
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            s_bl.Courier.AddObserver(PL.Tools.UserContext.UserId, RefreshCoureirObserver);
            if (CurrentCourier.OrderInProgress is not null)
                s_bl.Order.AddObserver(CurrentCourier.OrderInProgress!.orderId, RefreshOrderObserver);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Courier.RemoveObserver(PL.Tools.UserContext.UserId, RefreshCoureirObserver);
            s_bl.Order.RemoveObserver(PL.Tools.UserContext.UserId, RefreshOrderObserver);
        }

        private void Button_EndOrderHandle(object sender, RoutedEventArgs e)
        {
            s_bl.Order.EndOrderHandle(PL.Tools.UserContext.UserId, PL.Tools.UserContext.UserId,
             OrderInProgress!.orderId, OrderInProgress!.DeliveryId, (DO.DeliveryTermintionType)deliveryTerminationSelectedItem);
        
        }

        private void Button_OrderSelection(object sender, RoutedEventArgs e)
        {
            new OpenOrderListWindow().Show();
        }
    }
}

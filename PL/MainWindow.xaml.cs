using PL.Courier;
using PL.Order;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace PL
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

       static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public MainWindow()
        {
        }
        public DateTime CurrentTime //תכונת תלות  שמיצגת את ערכו של התאריך המוצג על המסך.
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));
        public BO.Config Configuration//תכונת תלות שמייצגת  את אובייקט התצורה הלוגי Config.BO.
        {
            get { return (BO.Config)GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
        }
        //כפתור לשמירת הקונגפינג
        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register(
                "Configuration",
                typeof(BO.Config),
                typeof(MainWindow)
            );

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
        }
   
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
        }

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
        }

        private void btnHandleOrders(object sender, RoutedEventArgs e)
        {
            new OrderListWindow().Show();

        }

        private void btnHandleCourier(object sender, RoutedEventArgs e)
        {
            new CourierListWindow().Show();

        }
    }
}
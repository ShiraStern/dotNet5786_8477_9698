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
            InitializeComponent();
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
        //כפתורי עדכון השעה  והתאריך במערכת
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
        }
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
        }
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
        }
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
        }
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
        }

        //מטודות לטיפול בכפתורי אתחול ואיפוס מסד הנתונים
        private void Button_InitializeDB(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
        "Are you sure you want to initialize the database?",
        "Confirmation",
        MessageBoxButton.YesNo,
        MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                    window.Close();
            }

            Mouse.OverrideCursor = Cursors.Wait;
            s_bl.Admin.InitializeDB();
            Mouse.OverrideCursor = null;
            MessageBox.Show("Database initialization completed successfully!");

        }
        private void Button_ResetDB(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                    "Are you sure you want to reset the database?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                    window.Close();
            }

            Mouse.OverrideCursor = Cursors.Wait;
            s_bl.Admin.ResetDB();
            Mouse.OverrideCursor = null;
            MessageBox.Show("Database reset completed successfully!");                                  
        }


        //מטודות לטיפול בכפתורי ניהול משלוחים והזמנות
        private void btnHandleOrders(object sender, RoutedEventArgs e)
        {
            new OrderListWindow().Show();

        }

        private void btnHandleCourier(object sender, RoutedEventArgs e)
        {
            new CourierListWindow().Show();
        }

        //מטודות התצפית על השעון והקונפיגורציה
        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }
        private void configObserver()
        {
            Configuration = s_bl.Admin.GetConfig();
        }
        //מטודות שטוענות את השעון ואת הקונפיגורציה בעת טעינת החלון וסגירתו
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            Configuration = s_bl.Admin.GetConfig();

            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }

    }
}


using BlApi;
using BO;
using PL.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for CourierListWindow.xaml
    /// </summary>
    public partial class CourierListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int _applicantId = UserContext.UserId;
        public BO.FilterCouriersByProperty FilterCouriers { get; set; } = BO.FilterCouriersByProperty.All;

        public BO.CourierInList? selectedCourier { get; set; }
        public IEnumerable<BO.CourierInList> CourierInList
        {
            get { return (IEnumerable<BO.CourierInList> )GetValue(CourierInListProperty); }
            set { SetValue(CourierInListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CourierInList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CourierInListProperty =
            DependencyProperty.Register("CourierInList", typeof(IEnumerable<BO.CourierInList> ), 
                typeof(CourierListWindow), new PropertyMetadata(null));



        public CourierListWindow()
        {
            InitializeComponent();
            s_bl.Courier.AddObserver(courseListObserver);
        }

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
            =>s_bl.Courier.AddObserver(courseListObserver);
      

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Courier.RemoveObserver(courseListObserver);

        private void queryCourierList()
        {
            //int managerId = s_bl.Admin.GetConfig().ManagerID;
            CourierInList = s_bl.Courier.GetCourierList(UserContext.UserId /*_applicantId*/, true, FilterCouriers)!;
        }
        private void courseListObserver()
            => queryCourierList();
 
        private void CourierFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCourierList();
        }
        private void AddCourier_Click(object sender, RoutedEventArgs e)
        {
           
            new CourierWindow(UserContext.UserId).Show();
        }


        private void selectCourier_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (selectedCourier == null)
            {
                MessageBox.Show("Please select a courier first.");
                return;
            }
            new CourierWindow(UserContext.UserId, selectedCourier.ID).Show();
        }

        private void DeleteCourier_Click(object sender, RoutedEventArgs e)
        {
            var courier = ((FrameworkElement)sender).DataContext as BO.CourierInList;

            if (courier == null)
                return;

            if (MessageBox.Show("Are you sure you want to delete this courier?",
                                "Confirm Delete",
                                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    int managerId = s_bl.Admin.GetConfig().ManagerID;

                    s_bl.Courier.Delete(managerId, courier.ID);

                    MessageBox.Show("Courier deleted successfully!");

                    queryCourierList(); // רענון הרשימה 
                }
                catch (BlInvalidStatusException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            }

    }
}

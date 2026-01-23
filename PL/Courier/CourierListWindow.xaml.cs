

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
        }

        private void queryCourierList()
        {
            //int managetID = s_bl.Admin.GetConfig().ManagerID;
            //CourierInList = (FilterCouriers == BO.FilterCouriersByProperty.All) ?
            //    s_bl?.Courier!.GetCourierList(managetID, null, FilterCouriersByProperty.All)!
            //    : FilterCouriers == BO.FilterCouriersByProperty.IsActive ?
            //    s_bl?.Courier!.GetCourierList(managetID, null, BO.FilterCouriersByProperty.IsActive)! :
            //    s_bl?.Courier!.GetCourierList(managetID, null, BO.FilterCouriersByProperty.IsNotActive)!;

            int managerId = s_bl.Admin.GetConfig().ManagerID;

            CourierInList = s_bl.Courier.GetCourierList(UserContext.UserId /*_applicantId*/, true, FilterCouriers)!;//GetCourierList הוא מקבל שלוש פרמטרים ? ככה זה אמור להיות ? כי במקבילה באורר זה לא ככה שם הו אמקבל רק 2 פרמרטרים לבדוק אם זה נכון שבהגדרה של 
        }
        private void courseListObserver()
            => queryCourierList();
 
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Courier.AddObserver(courseListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Courier.RemoveObserver(courseListObserver);

        private void CourierFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCourierList();
        }
        private void AddCourier_Click(object sender, RoutedEventArgs e)
        {
           
            new CourierWindow(UserContext.UserId).Show();
        }

        //private void EditCourier_Click(object sender, RoutedEventArgs e)
        //{
        //    var selected = CourierListView.SelectedItem as BO.CourierInList;

        //    if (selected == null)
        //    {
        //        MessageBox.Show("Please select a courier first.");
        //        return;
        //    }

        //    var fullCourier = s_bl.courier.GetDetails(_applicantId, selected.ID);

        //    new CourierWindow(selectedCourier.ID).Show();
        //}

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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}

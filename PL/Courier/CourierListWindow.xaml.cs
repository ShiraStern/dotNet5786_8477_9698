

using BlApi;
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

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for CourierListWindow.xaml
    /// </summary>
    public partial class CourierListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.FilterCouriersByProperty FilterCouriers { get; set; } = BO.FilterCouriersByProperty.All;


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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int managetID = s_bl.Admin.GetConfig().ManagerID;
            CourierInList = (FilterCouriers == BO.FilterCouriersByProperty.All) ?
                s_bl?.courier!.GetCourierList(managetID, null, FilterCouriersByProperty.All)!
                : FilterCouriers == BO.FilterCouriersByProperty.IsActive ?
                s_bl?.courier!.GetCourierList(managetID, null, BO.FilterCouriersByProperty.IsActive)! :
                s_bl?.courier!.GetCourierList(managetID, null, BO.FilterCouriersByProperty.IsNotActive)!;
                

        }
    }
}

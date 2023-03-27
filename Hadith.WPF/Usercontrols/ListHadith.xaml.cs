using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hadith.WPF.Usercontrols
{
    /// <summary>
    /// Interaction logic for ListHadith.xaml
    /// </summary>
    public partial class ListHadith : UserControl
    {
        private ObservableCollection<DAL.hadith> data { set; get; }
        public delegate void HadithSelectedEventHandler(DAL.hadith Hadith);
        public event HadithSelectedEventHandler HadithSelectEvent;
        private void CallHadithEvent(DAL.hadith Hadith)
        {
            // Event will be null if there are no subscribers
            if (HadithSelectEvent != null)
            {
                HadithSelectEvent(Hadith);
            }
        }
        public ListHadith(bool LoadFirstItem)
        {
            InitializeComponent();
            PopulateHadith();
            ListData.PreviewMouseLeftButtonUp += ListData_PreviewMouseLeftButtonUp;
            if (LoadFirstItem)
            {
                GoToFirstItem();
            }
        }
        void ListData_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selitem = (sender as ListView).SelectedItem;
            if (selitem != null)
            {
                CallHadithEvent((DAL.hadith)selitem);
            }
        }
        void PopulateHadith()
        {
            data = BLL.HadithData.getHadiths();
            ListData.ItemsSource = data;
        }
        public void GoToFirstItem()
        {
            if (ListData.ItemsSource != null)
            {
                ListData.SelectedIndex = 0;
                var selitem = ListData.SelectedItem;
                if (selitem != null)
                {
                    CallHadithEvent((DAL.hadith)selitem);
                }
            }
        }
        public void GoToSpecificItem(int HadithId)
        {
            var sel = from c in data
                      where c.HadithID == HadithId
                      select c;
            if (sel != null)
                CallHadithEvent(sel.SingleOrDefault());
        }
    }
}

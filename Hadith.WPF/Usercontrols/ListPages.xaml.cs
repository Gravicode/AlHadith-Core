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
    /// Interaction logic for ListPages.xaml
    /// </summary>
    public partial class ListPages : UserControl
    {
        private static ObservableCollection<DAL.hadithindex> data { set; get; }
        public static int selHadith { set; get; }
        public delegate void PagesSelectedEventHandler(DAL.hadithindex Pages);
        public event PagesSelectedEventHandler PagesSelectEvent;
        private void CallPagesEvent(DAL.hadithindex Pages)
        {
            // Event will be null if there are no subscribers
            if (PagesSelectEvent != null)
            {
                PagesSelectEvent(Pages);
            }
        }
        public ListPages(int HadithId,bool LoadFirstItem)
        {
            InitializeComponent();
            PopulatePages(HadithId);
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
                CallPagesEvent((DAL.hadithindex)selitem);
            }
        }
        public void PopulatePages(int HadithId)
        {
            selHadith = HadithId;
            data = BLL.HadithData.getIndex(selHadith);
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
                    CallPagesEvent((DAL.hadithindex)selitem);
                }
            }
        }
        public void GoToSpecificItem(int PageNo)
        {
            var sel = from c in data
                      where c.No == PageNo
                      select c;
            if (sel != null)
                CallPagesEvent(sel.SingleOrDefault());
        }
    }
}

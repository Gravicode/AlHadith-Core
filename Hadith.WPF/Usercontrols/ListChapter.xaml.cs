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
    public partial class ListChapter : UserControl
    {

        public static ObservableCollection<DAL.hadithchapter> data { set; get; }
        public int MaxChapter { set; get; }
        public int MinChapter { set; get; }
        public static int selHadith { set; get; }
        public static int selPage { set; get; }
        public delegate void ChapterSelectedEventHandler(DAL.hadithchapter Chapter);
        public event ChapterSelectedEventHandler ChapterSelectEvent;
        private void CallChapterEvent(DAL.hadithchapter Chapter)
        {
            // Event will be null if there are no subscribers
            if (ChapterSelectEvent != null)
            {
                ChapterSelectEvent(Chapter);
            }
        }
        public ListChapter(int HadithId, int PageNo, bool LoadFirstItem)
        {

            InitializeComponent();
            PopulateChapter(HadithId, PageNo);
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
                CallChapterEvent((DAL.hadithchapter)selitem);
            }
        }
        public void PopulateChapter(int HadithId, int PageNo)
        {
            selHadith = HadithId;
            selPage = PageNo;
            data = BLL.HadithData.getChapters(selHadith, selPage);
            MinChapter = 1;
            MaxChapter = data.Count;
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
                    CallChapterEvent((DAL.hadithchapter)selitem);
                }
            }
        }
        public void GoToSpecificItem(int ChapterNo)
        {
            var sel = from c in data
                      where c.ChapterNo == ChapterNo
                      select c;
            if (sel != null)
                CallChapterEvent(sel.SingleOrDefault());
        }
    }
}

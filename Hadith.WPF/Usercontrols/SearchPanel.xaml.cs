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
    /// Interaction logic for SearchPanel.xaml
    /// </summary>
    public partial class SearchPanel : UserControl
    {
        private ObservableCollection<BLL.SearchItem> data { set; get; }
        public delegate void SearchSelectedEventHandler(BLL.SearchItem Search);
        public event SearchSelectedEventHandler SearchSelectEvent;
        private void CallSearchEvent(BLL.SearchItem Search)
        {
            // Event will be null if there are no subscribers
            if (SearchSelectEvent != null)
            {
                SearchSelectEvent(Search);
            }
        }
        public SearchPanel()
        {
            InitializeComponent();
            BtnSearch.Click += BtnSearch_Click;
            ListData.PreviewMouseLeftButtonUp += ListData_PreviewMouseLeftButtonUp;
            CmbHadith.SelectionChanged += CmbHadith_SelectionChanged;
            CmbPage.SelectionChanged += CmbPage_SelectionChanged;
            CmbChapter.SelectionChanged += CmbChapter_SelectionChanged;
            PopulateHadith();
            
            
        }

        void PopulateHadith()
        {
            var data = BLL.HadithData.getHadiths();
            data.Insert(0, new DAL.hadith() { Title="-- All Hadith --", HadithID=-999 });
            CmbHadith.DisplayMemberPath = "Title";
            CmbHadith.SelectedValuePath = "HadithID";
            CmbHadith.ItemsSource = data;
            if (data != null && data.Count > 0)
            {
                CmbHadith.SelectedIndex = 0;
            }
        }

        void CmbChapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void CmbPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbHadith.SelectedItem != null && CmbPage.SelectedItem != null)
            {
                var selHadith = (DAL.hadith)CmbHadith.SelectedItem;
                var selPage = (DAL.hadithindex)CmbPage.SelectedItem;
                CmbChapter.DisplayMemberPath = "Title";
                CmbChapter.SelectedValuePath = "ChapterNo";
                if (selPage.No == -999)
                {
                    CmbChapter.ItemsSource = new DAL.hadithchapter[] { new DAL.hadithchapter() {ChapterNo = -999, Title = "-- All Chapter --" } };
                }
                else
                {
                    var data = BLL.HadithData.getChapters(selHadith.HadithID, selPage.No);
                    data.Insert(0, new DAL.hadithchapter() { ChapterNo = -999, Title = "-- All Chapter --" });
                  
                    CmbChapter.ItemsSource = data;
                }
             
                CmbChapter.SelectedIndex = 0;
                
            }
        }

        void CmbHadith_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbHadith.SelectedItem != null)
            {
                var item = (DAL.hadith)CmbHadith.SelectedItem;
                CmbPage.DisplayMemberPath = "Name";
                CmbPage.SelectedValuePath = "No";
                if (item.HadithID == -999)
                {
                    CmbPage.ItemsSource = new DAL.hadithindex[] { new DAL.hadithindex() { No = -999, Name = "-- All Kitab --" } };
                }
                else
                {
                    var data = BLL.HadithData.getIndex(item.HadithID);
                    data.Insert(0, new DAL.hadithindex() { Name = "-- All Kitab --", No = -999 });
                   
                    CmbPage.ItemsSource = data;
                }
                CmbPage.SelectedIndex = 0;
            }
        }

        void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            GoSearch();
        }

        void ListData_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selitem = (sender as ListView).SelectedItem;
            if (selitem != null)
            {
                CallSearchEvent((BLL.SearchItem)selitem);
            }
        }

        void GoSearch()
        {
            string KeyWord = SearchTxt.Text;
            var selHadith = (DAL.hadith)CmbHadith.SelectedItem;
            var selPage = (DAL.hadithindex)CmbPage.SelectedItem;
            var selChapter = (DAL.hadithchapter)CmbChapter.SelectedItem;

            if (string.IsNullOrEmpty(KeyWord))
            {
                MessageBox.Show("Please type a keyword","Info");
                return;
            }
            data = BLL.HadithData.searchByKeyword(KeyWord,selHadith.HadithID,selPage.No,selChapter.ChapterNo);
            ListData.ItemsSource = data;
        }
        
    }
}

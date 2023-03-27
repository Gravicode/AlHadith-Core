using Hadith.WPF.Tools;
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
    /// Interaction logic for ListContent.xaml
    /// </summary>
    public partial class ListContent : UserControl
    {
        private static BLL.HadithPerChapter HadithContent { set; get; } 
        public static int selHadith { set; get; }
        public int selBefore { set; get; }
        public static int selChapter { set; get; }
        public static int selPage { set; get; }
       
        public delegate void ContentSelectedEventHandler(BLL.HadithContentExt Content,int Index);
        public event ContentSelectedEventHandler ContentSelectEvent;
        private void CallContentEvent(BLL.HadithContentExt Content,int Index)
        {
            // Event will be null if there are no subscribers
            if (ContentSelectEvent != null)
            {
                ContentSelectEvent(Content,Index);
            }
        }

        public delegate void BookmarkSelectedEventHandler(BLL.BookmarkExt Bookmark);
        public event BookmarkSelectedEventHandler BookmarkSelectEvent;
        private void CallBookmarkEvent(BLL.BookmarkExt Bookmark)
        {
            // Event will be null if there are no subscribers
            if (BookmarkSelectEvent != null)
            {
                BookmarkSelectEvent(Bookmark);
            }
        }
        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }

        public void OnScrollUp(object sender, RoutedEventArgs e)
        {
            var scrollViwer = GetScrollViewer(ListData) as ScrollViewer;

            if (scrollViwer != null)
            {
                // Logical Scrolling by Item
                scrollViwer.LineUp();
                // Physical Scrolling by Offset
                //scrollViwer.ScrollToVerticalOffset(scrollViwer.VerticalOffset + 30);
            }
        }

        public void OnScrollDown(object sender, RoutedEventArgs e)
        {
            var scrollViwer = GetScrollViewer(ListData) as ScrollViewer;

            if (scrollViwer != null)
            {
                // Logical Scrolling by Item
                scrollViwer.LineDown();
                // Physical Scrolling by Offset
                //scrollViwer.ScrollToVerticalOffset(scrollViwer.VerticalOffset - 30);
            }
        }

        public ListContent(int HadithId, int PageNo, int ChapterNo,bool LoadFirstItem,int VerseSize,int LangId)
        {
            InitializeComponent();
            PopulateContent(HadithId, PageNo, ChapterNo,VerseSize,LangId);
            ListData.PreviewMouseLeftButtonUp += ListData_PreviewMouseLeftButtonUp;
            if (LoadFirstItem)
            {
                GoToFirstItem();
            }
        }
        
        public void PopulateContent(int HadithId, int PageNo, int ChapterNo,int VerseSize,int LangId)
        {
            selChapter = ChapterNo;
            selPage = PageNo;
            selHadith = HadithId;
            HadithContent = BLL.HadithData.getHadithInChapter(selHadith, selPage, selChapter,VerseSize,(BLL.HadithData.Languages)LangId);
            NoChapterLbl.Text = HadithContent.Chapter.ChapterNo.ToString();
            NoChapter2Lbl.Text = HadithContent.Chapter.ChapterNo.ToString();
            ChapterContentLbl.Text = HadithContent.Chapter.Title;
            ChapterContentArabicLbl.Text = HadithContent.Chapter.TitleArabic;
            if (string.IsNullOrEmpty(HadithContent.Chapter.Intro))
            {
                BorderIntro.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                BorderIntro.Visibility = System.Windows.Visibility.Visible;
                ChapterIntroLbl.Text = HadithContent.Chapter.Intro;
            }
            ListData.ItemsSource = HadithContent.Contents;
        }

        public void ChangeLanguage(int langId)
        {
            
            for (int i = 0; i < HadithContent.Contents.Count; i++)
            {
                var item = HadithContent.Contents[i];
                switch ((BLL.HadithData.Languages)langId)
                {
                    case BLL.HadithData.Languages.English:
                        item.Translation = item.ContentEnglish;
                        break;
                    case BLL.HadithData.Languages.Indonesia:
                        item.Translation = item.ContentIndonesia;
                        break;
                    case BLL.HadithData.Languages.Urdu:
                        item.Translation = item.ContentUrdu;
                        break;
                }
            }
        }

        public void ChangeVerseSize(int SizeVerse)
        {
            for (int i = 0; i < HadithContent.Contents.Count; i++)
            {
                var item = HadithContent.Contents[i];
                item.VerseSize = SizeVerse;
            }
        }

        void ListData_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selBefore > -1 && selBefore != (sender as ListView).SelectedIndex)
            {
                SetItemHadith(selBefore, Brushes.White, false, false);
            }
            if (selBefore != (sender as ListView).SelectedIndex)
            {
                SetItemHadith((sender as ListView).SelectedIndex, Brushes.Orange, true, true);
            }
        }

        public void SetItemHadith(int index, SolidColorBrush warna, bool Choose, bool Play)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (Choose)
                {
                    ListData.SelectedIndex = index;
                    selBefore = ListData.SelectedIndex;
                    var selitem = ListData.SelectedItem;
                    ListData.ScrollIntoView(ListData.SelectedItem);
                    ListData.UpdateLayout();
                }

                ListViewItem myListBoxItem = (ListViewItem)(ListData.ItemContainerGenerator.ContainerFromIndex(index));
                if (myListBoxItem != null)
                {
                    ContentPresenter myContentPresenter = Hadith.WPF.Tools.VisualHelper.FindVisualChild<ContentPresenter>(myListBoxItem);
                    DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                    TextBlock target = (TextBlock)myDataTemplate.FindName("ContentLbl", myContentPresenter);
                    //ContentArabicLbl
                    TextBlock target2 = (TextBlock)myDataTemplate.FindName("ContentArabicLbl", myContentPresenter);
            
                    if (target != null)
                    {
                        target.Foreground = warna;
                    }
                    if (target2 != null)
                    {
                        target2.Foreground = warna;
                    }
                }

                if (Choose && ListData.SelectedItem != null && Play)
                {
                    CallContentEvent((BLL.HadithContentExt)ListData.SelectedItem, ListData.SelectedIndex+1);
                }

            }));
        }

        public void GoToFirstItem()
        {
            if (ListData.ItemsSource != null)
            {
                SetItemHadith(0, Brushes.Orange, true, false);
                /*
                ListData.SelectedIndex = 0;
                var selitem = ListData.SelectedItem;
                if (selitem != null)
                {
                    CallContentEvent((BLL.HadithContentExt)ListData.SelectedItem, ListData.SelectedIndex+1);
                }*/
            }
        }

        public int GoToSpecificItem(int ContentID)
        {
            if (ListData.ItemsSource != null)
            {
                var temp = (from c in HadithContent.Contents
                           where c.ContentID == ContentID
                           select c).SingleOrDefault();
                if (temp != null)
                {
                    ListData.SelectedItem = temp;
                    SetItemHadith(ListData.SelectedIndex, Brushes.Orange, true, false);
                    return ListData.SelectedIndex;
                }
            }
            return -1;
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                if (btn.DataContext is BLL.HadithContentExt)
                {
                    var item = btn.DataContext as BLL.HadithContentExt;
                    if (item != null)
                    {
                        string LinkStr = string.IsNullOrWhiteSpace(item.UrlRef) ? "-" : item.UrlRef.Replace("|", ", ");
                        string DataStr = string.Format("{0}\r\n{1}\r\n{2}\r\n\r\n{3}\r\n{4}\r\n\r\nGrade:{5}\r\nReference:{6}\r\nIn-book reference:{7}\r\nUSC-MSA web (English) reference:{8}\r\nQuran/Hadith Url Ref.:{9}", item.SanadTop, item.ContentArabic, item.SanadBottom, item.Narated, item.Translation, item.Grade, item.Reference, item.BookRef, item.USCRef, LinkStr);
                        Clipboard.SetText(DataStr, TextDataFormat.UnicodeText);
                        //MessageBox.Show(item.Narated);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot copy hadith","Info");
                Logs.WriteLog("failed to copy hadith:"+ex.Message + " - " + ex.StackTrace);
            }
        }

        private void BtnBookmark_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                if (btn.DataContext is BLL.HadithContentExt)
                {
                    var temp = btn.DataContext as BLL.HadithContentExt;
                    var item = new BLL.BookmarkExt()
                    {
                        HadithId = temp.HadithID,
                        PageNo = temp.PageNo,
                        ChapterNo = temp.ChapterNo,
                        HadithNo = temp.HadithOrder
                    };
                    if (item != null)
                    {
                        CallBookmarkEvent(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot add bookmark", "Info");
                Logs.WriteLog("failed to add bookmark:" + ex.Message + " - " + ex.StackTrace);
            }
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Hadith.DAL;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web;
using HtmlAgilityPack;
using System.Data;
using Hadith.BLL.Tools;

namespace Hadith.BLL
{
    public class HadithData
    {
        public enum Languages { English=0, Indonesia=1, Urdu=2}
        public static string Conn { set; get; }
        #region Hadith
        public static ObservableCollection<hadith> getHadiths()
        {
            ObservableCollection<hadith> data=null;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    data = (from a in context.hadiths
                               select a).ToObservableCollection();

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        public static hadith getHadith(int HadithId)
        {
            var data = default(hadith);

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    data = (from a in context.hadiths
                            where a.HadithID == HadithId
                            select a).SingleOrDefault();

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        #endregion
        #region Language
        public static ObservableCollection<language> getLanguage()
        {
            ObservableCollection<language> data = new ObservableCollection<language>();
            for (int i = 0; i < 3;i++ )
            {
                data.Add(new language() { langid = i, lang = ((Languages)i).ToString() });
            }
            return data;
        }
        public static language getLanguage(int LangId)
        {
            return new language() { langid = LangId, lang = ((Languages)LangId).ToString() };
        }
        #endregion
        #region Index
        public static ObservableCollection<hadithindex> getIndex(int HadithId)
        {
            ObservableCollection<hadithindex> data = null;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    data = (from a in context.hadithindexs
                            where a.HadithID == HadithId
                            orderby a.No ascending
                            select a).ToObservableCollection();

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        public static hadithindex getIndexbyId(int indexId)
        {
            var data = default(hadithindex);

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    data = (from a in context.hadithindexs
                            where a.IndexID == indexId
                            select a).SingleOrDefault();

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        #endregion
        #region Page
        public static ObservableCollection<hadithpage> getPages(int HadithId)
        {
            ObservableCollection<hadithpage> data = null;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    data = (from a in context.hadithpages
                            where a.HadithID==HadithId
                            orderby a.PageNo ascending
                            select a).ToObservableCollection();

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        public static hadithpage getPage(int PageId)
        {
            var data = default(hadithpage);

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    data = (from a in context.hadithpages
                            where a.PageID == PageId
                            select a).SingleOrDefault();

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        public static hadithpage getPage(int HadithId,int PageNo)
        {
            var data = default(hadithpage);

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    data = (from a in context.hadithpages
                            where a.PageNo == PageNo && a.HadithID==HadithId
                            select a).SingleOrDefault();

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        #endregion
        #region Content & Chapter
        public static hadithchapter getChapter(int HadithId,int PageNo,int ChapterNo)
        {
            var data = default(hadithchapter);
            if (ChapterNo == -1)
            {
                return new hadithchapter() { ChapterID = 99999, ChapterNo = -1, HadithID = HadithId, PageNo = PageNo, Intro = "-", Title = "Hadith without chapter", TitleArabic = "-" };
            }
            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    var temp = (from a in context.hadithchapters
                            where a.HadithID == HadithId && a.PageNo == PageNo && a.ChapterNo==ChapterNo
                            orderby a.ChapterNo ascending
                            select a).ToList();
                    if (temp!=null &&  temp.Count() > 0)
                    {
                        data = temp.Take(1).SingleOrDefault();
                    }

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        public static ObservableCollection<hadithchapter> getChapters(int HadithId,int PageNo)
        {
            ObservableCollection<hadithchapter> data = null;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    data = (from a in context.hadithchapters
                            where a.HadithID == HadithId && a.PageNo == PageNo
                            orderby a.ChapterNo ascending
                            select a).ToObservableCollection();
                    data.Add(new hadithchapter() { ChapterID = 99999, ChapterNo = -1, HadithID = HadithId, PageNo = PageNo, Intro = "-", Title = "Hadith without chapter", TitleArabic = "-" });
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        public static ObservableCollection<HadithContentExt> getContent(int HadithId, int PageNo, Languages selLang, int ChapterNo = -1)
        {
            ObservableCollection<HadithContentExt> data = null;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    var items = (from a in context.hadithcontents
                                 where a.HadithID == HadithId && a.PageNo == PageNo && a.ChapterNo == ChapterNo 
                                 orderby a.HadithOrder ascending
                                 select a);
                    foreach (var item in items)
                    {
                        data = new ObservableCollection<HadithContentExt>();
                        HadithContentExt newNode = (HadithContentExt) item;
                        switch (selLang)
                        {
                            case Languages.English:
                                newNode.Translation = newNode.ContentEnglish;
                                break;
                            case Languages.Indonesia:
                                newNode.Translation = newNode.ContentIndonesia;
                                break;
                            case Languages.Urdu:
                                newNode.Translation = newNode.ContentUrdu;
                                break;
                        }
                        data.Add(newNode);

                    }

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        public static HadithPerChapter getHadithInChapter(int HadithId, int PageNo,int ChapterNo, int VerseSize, Languages selLang = Languages.English)
        {
            HadithPerChapter data = new HadithPerChapter();

            try
            {
                data.Chapter = getChapter(HadithId, PageNo, ChapterNo);
                if (data.Chapter == null)
                {
                    data.Chapter = new hadithchapter() { ChapterNo= ChapterNo, HadithID=HadithId, PageNo=PageNo, Title="Chapter not found", ChapterID=-1, TitleArabic="...", Intro="..." };
                }
                data.Contents = new ObservableCollection<HadithContentExt>();
                      
                var connection = new SQLiteConnection(Conn);
                //get all hadith with existing chapter
                using (var context = new DataContext())
                {
                    //CleanUpHtml()
                    var items = (from a in context.hadithcontents
                                 where a.HadithID == HadithId && a.PageNo == PageNo && a.ChapterNo == ChapterNo
                                 orderby a.HadithOrder ascending
                                 select new HadithContentExt { BookRef=a.BookRef, ChapterNo=a.ChapterNo, ContentArabic=CleanUpHtml(a.ContentArabic), ContentEnglish=a.ContentEnglish.Trim(), ContentID=a.ContentID, ContentIndonesia=a.ContentIndonesia.Trim(), ContentUrdu=a.ContentUrdu.Trim(),
                                  Grade=a.Grade.Trim(), HadithID=a.HadithID, HadithOrder=a.HadithOrder, Narated=a.Narated.Trim(), OtherRef=a.OtherRef, PageNo=a.PageNo, Reference=a.Reference, SanadBottom=CleanUpHtml(a.SanadBottom), SanadTop=CleanUpHtml(a.SanadTop), USCRef=a.USCRef, VerseSize=VerseSize, UrlRef=a.UrlRef});
                    foreach (var newNode in items)
                    {
                        //HadithContentExt newNode = item;
                        switch (selLang)
                        {
                            case Languages.English:
                                newNode.Translation = newNode.ContentEnglish;
                                break;
                            case Languages.Indonesia:
                                newNode.Translation = newNode.ContentIndonesia;
                                break;
                            case Languages.Urdu:
                                newNode.Translation = newNode.ContentUrdu;
                                break;
                        }
                        data.Contents.Add(newNode);

                    }
                }
            }
            catch(Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        public static int getTotalHadithInChapter(int HadithId, int PageNo, int ChapterNo)
        {
            
            try
            {
                
                var connection = new SQLiteConnection(Conn);
                //get all hadith with existing chapter
                using (var context = new DataContext())
                {

                    var items = (from a in context.hadithcontents
                                 where a.HadithID == HadithId && a.PageNo == PageNo && a.ChapterNo == ChapterNo 
                                select a).Count();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return -1;
        }
        
        public static ObservableCollection<HadithPerChapter> getHadithInPage(int HadithId, int PageNo, Languages selLang=Languages.English)
        {
            ObservableCollection<HadithPerChapter> data = new ObservableCollection<HadithPerChapter> ();

            try
            {
                var connection = new SQLiteConnection(Conn);
                //get all hadith with existing chapter
                using (var context = new DataContext())
                {

                    var chapters = getChapters(HadithId, PageNo);
                    foreach (var chap in chapters)
                    {
                        var newNode = new HadithPerChapter();
                        newNode.Chapter = chap;
                        newNode.Contents = getContent(HadithId, PageNo, selLang, chap.ChapterNo);
                        data.Add(newNode);
                    }
                }
                //add all hadith without chapter
                {
                    var newNode = new HadithPerChapter();
                    newNode.Chapter = new hadithchapter() { ChapterNo=999, Intro="These hadith have not been mapped to specific chapter", Title ="Hadith without chapters", TitleArabic="-", HadithID=HadithId, PageNo=PageNo };
                    newNode.Contents = getContent(HadithId, PageNo, selLang, -1);
                    data.Add(newNode);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        #endregion
        #region Search
        public static ObservableCollection<SearchItem> searchByKeyword(string Keyword,int HadithId, int PageNo, int ChapterNo, int Limit=100)
        {
            ObservableCollection<SearchItem> data = null;

            try
            {
                var connection = new SQLiteConnection(Conn);
                //get all hadith with existing chapter
                using (var context = new DataContext())
                {
                    string AddQuery = HadithId == -999 ? "" : string.Format(" and a.hadithid = {0}", HadithId);
                    AddQuery += PageNo == -999 ? "" : string.Format(" and a.pageno = {0}", PageNo);
                    AddQuery += ChapterNo == -999 ? "" : string.Format(" and a.chapterno = {0}", ChapterNo);

                    FungsiDB.KoneksiStr = Conn;
                    string Qry = string.Format(@"select a.ChapterNo, d.Title as ChapterTitle, a.ContentID, b.HadithID, a.HadithOrder, b.Title as HadithTitle, c.PageNo, c.Title as PageTitle, a.Reference,  c.TitleArabic as PageArabic from hadithcontent a
left outer join hadith b on a.hadithid = b.hadithid
left outer join hadithpage c on a.hadithid = c.hadithid and a.pageno = c.pageno
left outer join hadithchapter d on a.hadithid = d.hadithid and a.pageno = d.pageno and a.chapterno = d.chapterno
where ( a.ContentEnglish like '%{0}%' or a.ContentIndonesia like '%{0}%' or a.ContentUrdu like '%{0}%' or a.ContentArabic like '%{0}%'){2} limit {1}", Keyword, Limit, AddQuery);
                    System.Data.DataTable dt = FungsiDB.RetrieveData(Qry);
                    //var xx = dt.Rows.Count;
                    dt.TableName = "data";
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int counter = 0;
                        data = (from a in dt.AsEnumerable()
                                select new SearchItem
                                {

                                    ChapterNo = (int)a.Field<long>("ChapterNo"),
                                    HadithOrder = (int)a.Field<long>("HadithOrder"),
                                    PageNo = (int)a.Field<long>("PageNo"),
                                    Reference = a.Field<string>("Reference"),
                                    ChapterTitle = a.Field<string>("ChapterTitle"),
                                    ContentId = (int)a.Field<long>("ContentId"),
                                    HadithId = (int)a.Field<long>("HadithId"),
                                    HadithTitle = a.Field<string>("HadithTitle"),
                                    IndexNo = ++counter,
                                    PageArabic = a.Field<string>("PageArabic"),
                                    PageTitle = a.Field<string>("PageTitle")

                                }).ToObservableCollection();
                        /*
                        if (items != null)
                        {
                            int counter = 0;
                            data = (from a in items
                                    join b in context.hadiths.DefaultIfEmpty() on a.HadithID equals b.HadithID
                                    join c in context.hadithpages.DefaultIfEmpty() on new { a.HadithID, a.PageNo } equals new { c.HadithID, c.PageNo }
                                    join d in context.hadithchapters.DefaultIfEmpty() on new { a.HadithID, a.PageNo, a.ChapterNo } equals new { d.HadithID, d.PageNo, d.ChapterNo }
                                    select new SearchItem { ChapterNo = d.ChapterNo, ChapterTitle = d.Title.Trim(), ContentId = a.ContentID, HadithId = b.HadithID, HadithOrder = a.HadithOrder, HadithTitle = b.Title, PageNo = c.PageNo, PageTitle = c.Title, IndexNo = ++counter, Reference=a.Reference,  PageArabic=c.TitleArabic }).ToObservableCollection();
                        }*/
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex.Message + " - " + ex.StackTrace);
            }
            return data;
        }
        #endregion
        #region bookmark
        public static void DeleteBookmark(BookmarkExt item)
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                connection.Open();
                string sql = string.Format("delete from bookmark where idx = {0}", item.idx);
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
                connection.Close();

            }
            catch
            {
                throw;
            }
        }
        public static void InsertBookmark(BookmarkExt item)
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                connection.Open();
                string sql = string.Format("insert into bookmark (title, hadithid, pageno, chapterno, hadithno) values ('{0}',{1},{2},{3},{4})", item.Title, item.HadithId, item.PageNo, item.ChapterNo,item.HadithNo);
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
                connection.Close();

            }
            catch
            {
                throw;
            }
        }
        public static IList<BookmarkExt> getBookmark()
        {
            dynamic Data;

            try
            {
                int Counter = 1;
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    var Temp = from a in context.Bookmarks
                               orderby a.idx ascending
                               select new BookmarkExt { idx=a.idx, HadithId=a.HadithId, PageNo=a.PageNo, ChapterNo=a.ChapterNo, HadithNo=a.HadithNo, Title=a.Title };
                    var data = new List<BookmarkExt>();
                    foreach (var item in Temp)
                    {
                        item.Counter = Counter++;
                        item.HadithName = BLL.HadithData.getHadith(item.HadithId).Name;
                        item.PageName = BLL.HadithData.getPage(item.HadithId, item.PageNo).Title;
                        item.ChapterName = BLL.HadithData.getChapter(item.HadithId, item.PageNo, item.ChapterNo).Title;
                        data.Add(item);
                    }
                    Data = data;
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }
        #endregion
        #region Tool
        static string CleanUpHtml(string HTMLString)
        {
            if (string.IsNullOrEmpty(HTMLString)) return string.Empty;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HTMLString);
            string s = doc.DocumentNode.InnerText.Trim();
            return s;
        }
        #endregion
    }
    #region Data Structure
    public class BookmarkExt : Bookmark
    {
        public string HadithName { set; get; }
        public string PageName { set; get; }
        public string ChapterName { set; get; }
        public int Counter { set; get; }
    }
    public class SearchItem
    {
        public int IndexNo { set; get; }
        public string HadithTitle { set; get; }
        public string PageTitle { set; get; }
        public string ChapterTitle { set; get; }
        public string Reference { set; get; }
        public string PageArabic { set; get; }
        public int HadithOrder { set; get; }
        public int HadithId { set; get; }
        public int PageNo { set; get; }
        public int? ChapterNo { set; get; }
        public int ContentId { set; get; }
    }

    public class HadithPerChapter
    {
        public hadithchapter Chapter { set; get; }
        public ObservableCollection<HadithContentExt> Contents { set; get; }
    }
    public class HadithContentExt : hadithcontent, INotifyPropertyChanged
    {
        private int _VerseSize;
        public int VerseSize
        {
            set { _VerseSize = value; NotifyPropertyChanged(); }
            get { return _VerseSize; }
        }
        private string _Translation;
        public string Translation
        {
            set { _Translation = value; NotifyPropertyChanged(); }
            get { return _Translation; }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    #endregion
    public static class MyExtensions
    {
        #region Extension
        public static ObservableCollection<T> ToObservableCollection<T>
    (this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new ObservableCollection<T>(source);
        }
        #endregion
    }
}

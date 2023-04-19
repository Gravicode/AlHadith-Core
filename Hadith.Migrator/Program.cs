using Hadith.Migrator.Models;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Hadith.Migrator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //migrate sql server to sqlite
            Migrate();
        }

        static void Migrate()
        {
            //read config
            string PathDB = "C:\\Experiment\\AlHadith-Core\\Hadith.WPF\\bin\\Debug\\net7.0-windows\\DB\\Hadith.db";
            System.IO.FileInfo ConFile = new System.IO.FileInfo(PathDB);
            if (!ConFile.Exists)
            {
                Console.WriteLine("Database not found.");
                return;
            }
            string ConStr = string.Format("Data Source={0};", ConFile.FullName);
            BLL.HadithData.Conn = ConStr;
            BLL.DataContext litedb = new BLL.DataContext();
            int ID_Hadith = 22;
            HadithDbContext oridb = new HadithDbContext();
            var hadith = oridb.Hadiths.Where(x => x.HadithId == ID_Hadith).FirstOrDefault();
            var newHadith = new DAL.hadith();
            newHadith.About = Regex.Replace(hadith.About, @"<[^>]+>|&nbsp;", "").Trim();
            newHadith.Arabic = hadith.Arabic;
            newHadith.HadithID = hadith.HadithId;
            newHadith.TotalHadith = oridb.HadithContents.Count(x => x.HadithId == ID_Hadith);
            newHadith.TotalPage = oridb.HadithPages.Count(x => x.HadithId == ID_Hadith);
            newHadith.Name = hadith.Name;
            newHadith.Title = hadith.Title;
            litedb.hadiths.Add(newHadith);
            litedb.SaveChanges();


            var indexes = oridb.HadithIndices.Where(x => x.HadithId == ID_Hadith).ToList();
            foreach (var index in indexes)
            {
                var newIndex = new DAL.hadithindex();
                newIndex.HadithID = index.HadithId.Value;
                newIndex.ArabicName = index.ArabicName;
                newIndex.IndexFrom = index.IndexFrom.Value;
                newIndex.IndexID = index.IndexId;
                newIndex.No = index.No.Value;
                newIndex.IndexTo = index.IndexTo.Value;
                newIndex.Name = index.Name;
                litedb.hadithindexs.Add(newIndex);
            }
            litedb.SaveChanges();

            var chapters = oridb.HadithChapters.Where(x => x.HadithId == ID_Hadith).ToList();
            foreach (var chapter in chapters)
            {
                var newChap = new DAL.hadithchapter();
                newChap.HadithID = chapter.HadithId.Value;
                newChap.ChapterID = chapter.ChapterId;
                newChap.Intro = chapter.Intro;
                newChap.ChapterNo = chapter.ChapterNo.Value;
                newChap.ChapterNoStr = chapter.ChapterNoStr;
                newChap.PageNo = chapter.PageNo.Value;
                newChap.Title = chapter.Title;
                newChap.TitleArabic = chapter.TitleArabic;
                litedb.hadithchapters.Add(newChap);
            }
            litedb.SaveChanges();

            var pages = oridb.HadithPages.Where(x => x.HadithId == ID_Hadith).ToList();
            foreach (var page in pages)
            {
                var newPage = new DAL.hadithpage();
                newPage.HadithID = page.HadithId.Value;
                newPage.TitleArabic = page.TitleArabic;
                newPage.PageNo = page.PageNo.Value;
                newPage.PageID = page.PageId;
                newPage.Title = Regex.Replace(page.Title, @"<[^>]+>|&nbsp;", "").Trim();

                litedb.hadithpages.Add(newPage);
            }
            litedb.SaveChanges();

            var contents = oridb.HadithContents.Where(x => x.HadithId == ID_Hadith).ToList();
            foreach (var content in contents)
            {
                var newContent = new DAL.hadithcontent();
                newContent.ContentID = content.ContentId;
                newContent.USCRef = string.IsNullOrEmpty(content.Uscref) ? "" : content.Uscref;
                newContent.Reference = string.IsNullOrEmpty(content.Reference) ? "" : content.Reference;
                newContent.PageNo = content.PageNo.HasValue ? content.PageNo.Value: 1;
                newContent.BookRef = string.IsNullOrEmpty(content.BookRef) ? "" : content.BookRef;
                newContent.ChapterNo = content.ChapterNo.HasValue ? content.ChapterNo.Value : -1;
                newContent.ContentArabic = string.IsNullOrEmpty(content.ContentArabic) ? "" : content.ContentArabic;
                newContent.ContentEnglish = string.IsNullOrEmpty(content.ContentEnglish) ? "" : content.ContentEnglish;
                newContent.ContentIndonesia = string.IsNullOrEmpty(content.ContentIndonesia) ? "" : content.ContentIndonesia;
                newContent.ContentUrdu = string.IsNullOrEmpty(content.ContentUrdu) ? "" : content.ContentUrdu;
                newContent.SanadBottom = string.IsNullOrEmpty(content.SanadBottom) ? "" : content.SanadBottom;
                newContent.SanadTop = string.IsNullOrEmpty(content.SanadTop) ? "" : content.SanadTop;
                newContent.Grade = string.IsNullOrEmpty(content.Grade) ? "" : content.Grade;
                newContent.HadithOrder = content.HadithOrder.HasValue ? content.HadithOrder.Value : 0;
                newContent.Narated = string.IsNullOrEmpty(content.Narated) ? "" : Regex.Replace(content.Narated, @"<[^>]+>|&nbsp;", "").Trim(); 
                newContent.OtherRef = string.IsNullOrEmpty(content.OtherRef) ? "" : content.OtherRef;
                newContent.UrlRef = string.IsNullOrEmpty(content.UrlRef)?"":content.UrlRef;
                newContent.HadithID = content.HadithId.Value;

                litedb.hadithcontents.Add(newContent);
            }
            litedb.SaveChanges();
            Console.WriteLine("ok");
        }
    }
}
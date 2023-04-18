using Hadith.Migrator.Models;
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

            HadithDbContext oridb = new HadithDbContext();
            //var hadith = oridb.Hadiths.Where(x => x.HadithID == 18).FirstOrDefault();
            //var newHadith = new Models.hadith();
            //newHadith.About = hadith.About;
            //newHadith.Arabic = hadith.Arabic;
            //newHadith.HadithID = hadith.HadithID;
            //newHadith.TotalHadith = 1;
            //newHadith.TotalPage = 1;
            //newHadith.Name = hadith.Name;
            //newHadith.Title = hadith.Title;
            //litedb.hadiths.Add(newHadith);
            //litedb.SingleInsert(newHadith, options => options.IncludeGraph = true);
            //var data = litedb.hadiths.ToList();

            //var index = oridb.HadithIndexes.Where(x => x.HadithID == 18).FirstOrDefault();
            //var newIndex = new Models.hadithindex();
            //newIndex.HadithID = index.HadithID.Value;
            //newIndex.ArabicName = index.ArabicName;
            //newIndex.IndexFrom = index.IndexFrom.Value;
            //newIndex.IndexID = index.IndexID;
            //newIndex.No = index.No.Value;
            //newIndex.IndexTo = index.IndexTo.Value;
            //newIndex.Name = index.Name;
            //litedb.hadithindexs.Add(newIndex);
            //litedb.SaveChanges();

            //var chapters = oridb.HadithChapters.Where(x => x.HadithId == 18).ToList();
            //foreach (var chapter in chapters)
            //{
            //    var newChap = new DAL.hadithchapter();
            //    newChap.HadithID = chapter.HadithId.Value;
            //    newChap.ChapterID = chapter.ChapterId;
            //    newChap.Intro = chapter.Intro;
            //    newChap.ChapterNo = chapter.ChapterNo.Value;
            //    newChap.ChapterNoStr = chapter.ChapterNoStr;
            //    newChap.PageNo = chapter.PageNo.Value;
            //    newChap.Title = chapter.Title;
            //    newChap.TitleArabic = chapter.TitleArabic;
            //    litedb.hadithchapters.Add(newChap);
            //}
            //litedb.SaveChanges();
            var contents = oridb.HadithContents.Where(x => x.HadithId == 18).ToList();
            foreach (var content in contents)
            {
                var newContent = new DAL.hadithcontent();
                newContent.ContentID = content.ContentId;
                newContent.USCRef = content.Uscref;
                newContent.Reference = content.Reference;
                newContent.PageNo = content.PageNo.Value;
                newContent.BookRef = content.BookRef;
                newContent.ChapterNo = content.ChapterNo.Value;
                newContent.ContentArabic = content.ContentArabic;
                newContent.ContentEnglish = content.ContentEnglish;
                newContent.ContentIndonesia = content.ContentIndonesia;
                newContent.ContentUrdu = content.ContentUrdu;
                newContent.SanadBottom = content.SanadBottom;
                newContent.SanadTop = content.SanadTop;
                newContent.Grade = content.Grade;
                newContent.HadithOrder = content.HadithOrder.Value;
                newContent.Narated = content.Narated;
                newContent.OtherRef = content.OtherRef;
                newContent.UrlRef = content.UrlRef;
                newContent.HadithID = content.HadithId.Value;

                litedb.hadithcontents.Add(newContent);
            }
            litedb.SaveChanges();
            Console.WriteLine("ok");
        }
    }
}
using HtmlAgilityPack;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using WatiN.Core;
using static System.Net.Mime.MediaTypeNames;

namespace Hadith.Parser
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            //getHadith();
            //GetIndex();
            //GetIndex(22); 
            //GetWaliullahContent();
            //GetContentNawawi();
            //GetChapters();
            //GetChaptersHisn();
            //GetHisnContent();
            //GetContent(22);
            GetContentMishkat();
            //GetContent();
            //CopyHadith();
            //GetSpecificContentWithWatin();
            //CleanHTML();
            //GetChapters();
            //CleanUpIndex();
            //CleanHTML();
            //RepairReference();
            //RepairOrder();
            //GetSpecificContentWithWatin();
            //GetSpecificContent();
            //GetIndoContent();
            //GetBulughContent();
            //GetSpecialContent();

            /*
             string original =
                "";
            string fix =
    Regex.Replace(original, @"^\s*$\n", string.Empty, RegexOptions.Multiline).TrimEnd();
             */

        }

        
        static void CopyHadith()
        {
            int counter = 0;
            HadithDBEntities ctxOri = new HadithDBEntities();
            HadithDB2Entities ctx = new HadithDB2Entities();
            var hadist = (from c in ctx.HadithContents
                          where c.HadithID==4
                          select c).ToList();
            foreach (var item in hadist)
            {
                HadithContent newItem = new HadithContent()
                {
                    BookRef = item.BookRef,
                 ContentIndonesia=item.ContentIndonesia, HadithID=item.HadithID,
                 ChapterNo=item.ChapterNo, ContentArabic=item.ContentArabic,
                 ContentEnglish=item.ContentEnglish, ContentUrdu=item.ContentUrdu, Grade=item.Grade
                , HadithOrder=item.HadithOrder, Narated=item.Narated, OtherRef=item.OtherRef,
                 PageNo=item.PageNo, Reference=item.Reference, SanadBottom=item.SanadBottom,
                 SanadTop=item.SanadTop, UrlRef=item.UrlRef , USCRef=item.USCRef};
                ctxOri.HadithContents.Add(newItem);
                if (++counter > 200)
                {
                    counter=0;
                    ctxOri.SaveChanges();
                }
            }
            ctxOri.SaveChanges();

        }

        static void CleanUpIndex()
        {
            HadithDBEntities ctx = new HadithDBEntities();
            var indeks = (from c in ctx.HadithPages
                          select c).ToList();
            int counter = 0;
            foreach (var item in indeks)
            {
                counter++;
                item.Title = item.Title.Trim();
                if (counter > 100)
                {
                    counter = 0;
                    ctx.SaveChanges();
                }
            }
        }

        static void CleanHTML()
        {
            HtmlDocument doc = new HtmlDocument();
            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var content = from c in ctx.HadithContents
                              where c.HadithID == selHadith.HadithID
                              orderby c.ContentID
                              select c;
                int counter = 0;
                foreach (var item in content)
                {
                    if (!string.IsNullOrEmpty(item.Grade))
                    {
                        counter++;
                        item.Grade = item.Grade.Trim();
                        
                    }
                    /*
                    if (!string.IsNullOrEmpty(item.Reference))
                    {
                        item.Reference = item.Reference.Replace(":", string.Empty).Trim();
                        //doc.LoadHtml(item.Reference);
                        //item.Reference = doc.DocumentNode.InnerText;
                    }
                    if (!string.IsNullOrEmpty(item.BookRef))
                    {
                        item.BookRef = item.BookRef.Replace(":",string.Empty).Trim();
                        //doc.LoadHtml(item.BookRef);
                        //item.BookRef = doc.DocumentNode.InnerText;
                    }
                    if (!string.IsNullOrEmpty(item.USCRef))
                    {
                        item.USCRef = item.USCRef.Replace(":", string.Empty).Trim();
                        //doc.LoadHtml(item.USCRef);
                        //item.USCRef = doc.DocumentNode.InnerText;
                    }
                    
                    if (!string.IsNullOrEmpty(item.ContentArabic))
                    {
                        counter = 0;
                        //item.Reference = HttpUtility.HtmlDecode(item.Reference);
                        doc.LoadHtml(item.ContentArabic);
                        var nodes = doc.DocumentNode.SelectNodes("//a");
                        string RefURL = string.Empty;
                        if (nodes != null)
                        {
                            foreach (HtmlNode itm in nodes)
                            {
                                if (counter > 0)
                                {
                                    RefURL += "|";
                                }
                                var tmp = itm.Attributes["href"].Value.Replace("javascript:openquran", string.Empty);
                                tmp = tmp.Replace("(", string.Empty).Replace(")", string.Empty);
                                string[] tmpStr = tmp.Split(',');
                                if (tmpStr.Length > 2)
                                {
                                    RefURL += string.Format("http://quran.com/{0}/{1}-{2}", Convert.ToInt32(tmpStr[0]) + 1, tmpStr[1], tmpStr[2]);
                                    counter++;
                                }
                                else if (tmpStr.Length <= 1 && !tmpStr[0].Contains("comment"))
                                {
                                    RefURL += string.Format("http://sunnah.com{0}", tmpStr[0]);
                                    counter++;
                                }
                                
                            }
                            if (!string.IsNullOrEmpty(RefURL))
                            {
                                item.UrlRef = RefURL;
                            }
                        }
                        //item.ContentArabic = doc.DocumentNode.InnerText.Trim();
                    }
                    if (!string.IsNullOrEmpty(item.Grade))
                    {
                        item.Grade = item.Grade.Replace("Grade", string.Empty).Replace(":",string.Empty);

                        //item.Grade = HttpUtility.HtmlDecode(item.Grade);
                        //doc.LoadHtml(item.Grade);
                        //item.Grade = doc.DocumentNode.InnerText.Trim();
                    }
                    if (!string.IsNullOrEmpty(item.ContentEnglish))
                    {
                        item.ContentEnglish = item.ContentEnglish.Replace("(?)", string.Empty);
                    }
                    if (!string.IsNullOrEmpty(item.Narated))
                    {
                        //item.Reference = HttpUtility.HtmlDecode(item.Reference);
                        doc.LoadHtml(item.Narated);
                        item.Narated = doc.DocumentNode.InnerText.Trim();
                    }
                    if (!string.IsNullOrEmpty(item.SanadBottom))
                    {
                        //item.Reference = HttpUtility.HtmlDecode(item.Reference);
                        doc.LoadHtml(item.SanadBottom);
                        item.SanadBottom = doc.DocumentNode.InnerText.Trim();
                    }
                     if (!string.IsNullOrEmpty(item.SanadTop))
                    {
                        //item.Reference = HttpUtility.HtmlDecode(item.Reference);
                        doc.LoadHtml(item.SanadTop);
                        item.SanadTop = doc.DocumentNode.InnerText.Trim();
                    }
                    if (!string.IsNullOrEmpty(item.ContentArabic))
                    {
                        //item.Reference = HttpUtility.HtmlDecode(item.Reference);
                        doc.LoadHtml(item.ContentArabic);
                        item.ContentArabic = doc.DocumentNode.InnerText.Trim();
                    }
                    
                    
                    
                    if (!string.IsNullOrEmpty(item.ContentEnglish))
                    {
                        //item.Reference = HttpUtility.HtmlDecode(item.Reference);
                        doc.LoadHtml(item.ContentEnglish);
                        item.ContentEnglish = doc.DocumentNode.InnerText.Trim();
                    }
                    if (!string.IsNullOrEmpty(item.ContentIndonesia))
                    {
                        //item.Reference = HttpUtility.HtmlDecode(item.Reference);
                        doc.LoadHtml(item.ContentIndonesia);
                        item.ContentIndonesia = doc.DocumentNode.InnerText.Trim();
                    }
                    if (!string.IsNullOrEmpty(item.ContentUrdu))
                    {
                        //item.Reference = HttpUtility.HtmlDecode(item.Reference);
                        doc.LoadHtml(item.ContentUrdu);
                        item.ContentUrdu = doc.DocumentNode.InnerText.Trim();
                    }
                    
                    if (!string.IsNullOrEmpty(item.Reference))
                    {
                        item.Reference = HttpUtility.HtmlDecode(item.Reference);
                        //doc.LoadHtml(item.Reference);
                        //item.Reference = doc.DocumentNode.InnerText;
                    }
                    if (!string.IsNullOrEmpty(item.BookRef))
                    {
                        item.BookRef = HttpUtility.HtmlDecode(item.BookRef);
                        //doc.LoadHtml(item.BookRef);
                        //item.BookRef = doc.DocumentNode.InnerText;
                    }
                    if (!string.IsNullOrEmpty(item.USCRef))
                    {
                        item.USCRef = HttpUtility.HtmlDecode(item.USCRef);
                        //doc.LoadHtml(item.USCRef);
                        //item.USCRef = doc.DocumentNode.InnerText;
                    }*/
                }
                ctx.SaveChanges();
            }
           
        }

        static void RepairReference()
        {
             HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var content = from c in ctx.HadithContents
                              where c.HadithID == selHadith.HadithID
                              orderby c.ContentID
                              select c;
                foreach (var item in content)
                {
                    if (!string.IsNullOrEmpty(item.Reference))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(item.Reference);
                        int counter = 0;
                        
                        var nodes = doc.DocumentNode.SelectNodes("//td");
                        if (nodes != null)
                        {
                            List<string> RefArray = new List<string>();
                            foreach (HtmlNode node in nodes)
                            {
                                counter++;
                                string RefContent = node.InnerText;
                                Console.WriteLine(counter + " - " + RefContent);
                                if (counter % 2 == 0)
                                {
                                    RefArray.Add(RefContent);
                                }
                            }
                            if (RefArray.Count > 0)
                                item.Reference = RefArray[0];
                            if (RefArray.Count > 1)
                                item.BookRef = RefArray[1];
                            if (RefArray.Count > 2)
                                item.USCRef = RefArray[2];
                        }
                        
                    }
                }
                ctx.SaveChanges();
            }
        }

        static void RepairOrder()
        {
            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.HadithID==13
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   && c.No!=null
                                   orderby c.No
                                   select c.No).ToArray();
                foreach(var j in hadistIndex)
                {
                    int Counter = 0;
                    var selContents = from c in ctx.HadithContents
                                      where c.HadithID == selHadith.HadithID && c.PageNo == j
                                      orderby c.ContentID ascending
                                      select c;
                    foreach (var item in selContents)
                    {
                        item.HadithOrder = ++Counter;
                    }
                    ctx.SaveChanges();
                }
            }
        }

        static string ScrapWebWatin()
        {
            Process[] processes = Process.GetProcessesByName("iexplore");

            foreach (Process process in processes)
            {
                process.Kill();
            }
            // Create an instance of Internet Explorer browser 
            using (IE ieInstance = new IE("http://sunnah.com/bukhari/1"))
            {
                // This will open Internet Explorer browser in maximized mode 
                ieInstance.ShowWindow(WatiN.Core.Native.Windows.NativeMethods.WindowShowStyle.ShowMaximized);
                ieInstance.Visible = false;
                
                ieInstance.WaitForComplete();
                // This will store page source in categoryPageSource variable 

                var RdBtn = ieInstance.RadioButton(Find.ById("ch_indonesian"));
                if (RdBtn != null)
                {
                    RdBtn.Click();
                    // This will wait for the browser to complete loading of the page 

                    string HtmlPage = ieInstance.Html;
                    return HtmlPage;
                }
            }
            return string.Empty;
        }

        static void GetSpecificContentWithWatin()
        {
            //Kill all ie
            Process[] processes = Process.GetProcessesByName("iexplore");

            foreach (Process process in processes)
            {
                process.Kill();
            }

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.HadithID == 1 || c.HadithID == 3 || c.HadithID == 4
                          select c).ToList();
            using (IE ieInstance = new IE())
            {
                // This will open Internet Explorer browser in maximized mode 
                ieInstance.ShowWindow(WatiN.Core.Native.Windows.NativeMethods.WindowShowStyle.ShowMaximized);
                ieInstance.Visible = false;

                ieInstance.WaitForComplete();
                // This will store page source in categoryPageSource variable 

                

                for (int i = 0; i < hadist.Count; i++)
                {
                    var selHadith = hadist[i];
                    var hadistIndex = (from c in ctx.HadithIndexes
                                       where c.HadithID == selHadith.HadithID
                                       && c.No != null
                                       orderby c.No
                                       select c).ToList();
                    for (int j = 0; j < hadistIndex.Count; j++)
                    {
                        var selIndex = hadistIndex[j];
                        var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, selIndex.No);

                        try
                        {
                            int HadithOrder = 0;
                            bool isIndonesian = false;
                            bool isUrdu = false;
                            ieInstance.GoTo(selURL);
                            var RdBtn = ieInstance.RadioButton(Find.ById("ch_indonesian"));
                            var RdBtn2 = ieInstance.RadioButton(Find.ById("ch_urdu"));

                            if (ieInstance.Elements.Exists("ch_indonesian") && RdBtn != null)
                            {
                                try
                                {
                                    RdBtn.Click();
                                    ieInstance.WaitForComplete();
                                    Thread.Sleep(500);
                                    // This will wait for the browser to complete loading of the page 
                                    isIndonesian = true;
                                }
                                catch { }
                            }
                            if (ieInstance.Elements.Exists("ch_urdu") && RdBtn2 != null)
                            {
                                try
                                {
                                    RdBtn2.Click();
                                    ieInstance.WaitForComplete();
                                    Thread.Sleep(500);

                                    // This will wait for the browser to complete loading of the page 
                                    isUrdu = true;
                                }
                                catch { }
                            }
                            string HtmlPage = ieInstance.Html;
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(HtmlPage);
                            HadithChapter selChapter = null;
                            int ContentCounter = 0;

                            HadithPage selPage = new HadithPage();
                            selPage.PageNo = selIndex.No;
                            selPage.HadithID = selHadith.HadithID;
                            //get title
                            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                            {
                                if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                                {
                                    switch (node.Attributes["class"].Value)
                                    {
                                        case "book_page_english_name":
                                            selPage.Title = node.InnerText.Trim();
                                            break;
                                        case "book_page_arabic_name arabic":
                                            selPage.TitleArabic = node.InnerText.Trim();
                                            //ctx.HadithPages.Add(selPage);
                                            break;
                                        case "chapter":
                                            selChapter = new HadithChapter();
                                            selChapter.HadithID = selHadith.HadithID;
                                            selChapter.PageNo = selPage.PageNo;
                                            //iterate every chapter
                                            var chapterNode = node;
                                            {
                                                var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                                if (subnode != null)
                                                {
                                                    try
                                                    {
                                                        selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Trim().Replace("(", "").Replace(")", ""));
                                                    }
                                                    catch
                                                    {
                                                        var Parsed = subnode.InnerText.Trim().Replace("(", "").Replace(")", "");
                                                        if (Parsed.Contains(','))
                                                        {
                                                            selChapter.ChapterNo = Convert.ToInt32(Parsed.Split(',')[0]);
                                                        }
                                                        else
                                                        {
                                                            for (int z = 0; z < Parsed.Length; z++)
                                                            {
                                                                if (!(Parsed[z] >= '0' && Parsed[z] <= '9'))
                                                                {
                                                                    Parsed = Parsed.Replace(Parsed[z].ToString(), " ");
                                                                }
                                                            }
                                                            selChapter.ChapterNo = Convert.ToInt32(Parsed.Trim());
                                                        }
                                                    }
                                                }
                                            }
                                            {
                                                var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                                if (subnode != null)
                                                {
                                                    selChapter.Title = subnode.InnerText.Trim();
                                                }
                                            }
                                            {
                                                var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                                if (subnode != null)
                                                {
                                                    selChapter.TitleArabic = subnode.InnerText.Trim();
                                                }
                                            }
                                            //ctx.HadithChapters.Add(selChapter);
                                            break;
                                        case "arabic achapintro":
                                            {
                                                if (selChapter != null)
                                                {
                                                    selChapter.Intro = node.InnerText.Trim();
                                                }
                                            }
                                            break;
                                        case "actualHadithContainer":
                                            HadithContent selContent = new HadithContent();
                                            selContent.HadithID = selHadith.HadithID;
                                            selContent.PageNo = selPage.PageNo;
                                            selContent.HadithOrder = ++HadithOrder;
                                            if (selChapter != null)
                                            {
                                                selContent.ChapterNo = selChapter.ChapterNo;
                                            }
                                            {
                                                var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                                if (subnode != null)
                                                {
                                                    selContent.Narated = subnode.InnerText.Trim();
                                                }
                                            }
                                            {
                                                var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                                if (subnode != null)
                                                {
                                                    selContent.ContentEnglish = subnode.InnerText.Trim();
                                                }
                                            }
                                            if(isIndonesian)
                                            {
                                                var subnode = node.SelectSingleNode(".//div[@class='indonesian_hadith_full']");
                                                if (subnode != null)
                                                {
                                                    selContent.ContentIndonesia = subnode.InnerText.Trim();
                                                }
                                            }
                                            if (isUrdu)
                                            {
                                                var subnode = node.SelectSingleNode(".//div[@class='urdu_hadith_full']");
                                                if (subnode != null)
                                                {
                                                    selContent.ContentUrdu = subnode.InnerText.Trim();
                                                }
                                            }
                                            {
                                                var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                                if (subnode != null)
                                                {
                                                    selContent.Grade = subnode.InnerText.Trim();
                                                }
                                            }
                                            {
                                                var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                                if (subnode != null)
                                                {
                                                    selContent.Reference = subnode.InnerText.Trim();
                                                }
                                            }
                                            {
                                                var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                                if (subnode != null)
                                                {
                                                    selContent.SanadTop = subnode[0].InnerText.Trim();
                                                    selContent.SanadBottom = subnode[1].InnerText.Trim();
                                                }

                                            }
                                            {
                                                var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                                if (subnode != null)
                                                {
                                                    selContent.ContentArabic = subnode.InnerText.Trim();
                                                }
                                            }
                                            ctx.HadithContents.Add(selContent);
                                            ContentCounter++;
                                            if (ContentCounter > 100)
                                            {
                                                ctx.SaveChanges();
                                                ContentCounter = 0;
                                            }
                                            break;
                                        default: break;
                                    }
                                }


                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(selURL + ":" + ex.Message + "-" + ex.StackTrace);
                        }

                        ctx.SaveChanges();
                    }
                }
            }
            Console.WriteLine("selesai");
            Console.ReadLine();
        }

        static void GetSpecificContent()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.HadithID==3
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, selIndex.No);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        //ctx.HadithPages.Add(selPage);
                                        break;
                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            if (subnode != null)
                                            {
                                                try
                                                {
                                                    selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                                }
                                                catch
                                                {
                                                    var Parsed = subnode.InnerText.Replace("(", "").Replace(")", "");
                                                    if (Parsed.Contains(','))
                                                    {
                                                        selChapter.ChapterNo = Convert.ToInt32(Parsed.Split(',')[0]);
                                                    }
                                                    else
                                                    {
                                                        for (int z = 0; z < Parsed.Length; z++)
                                                        {
                                                            if (!(Parsed[z] >= '0' && Parsed[z] <= '9'))
                                                            {
                                                                Parsed = Parsed.Replace(Parsed[z].ToString(), " ");
                                                            }
                                                        }
                                                        selChapter.ChapterNo = Convert.ToInt32(Parsed.Trim());
                                                    }
                                                }
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            if (subnode != null)
                                            {
                                                selChapter.Title = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            if (subnode != null)
                                            {
                                                selChapter.TitleArabic = subnode.InnerText;
                                            }
                                        }
                                        //ctx.HadithChapters.Add(selChapter);
                                        break;
                                    case "arabic achapintro":
                                        {
                                            if (selChapter != null)
                                            {
                                                selChapter.Intro = node.InnerText;
                                            }
                                        }
                                        break;
                                    case "actualHadithContainer":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        if (selChapter != null)
                                        {
                                            selContent.ChapterNo = selChapter.ChapterNo;
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentEnglish = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            if (subnode != null)
                                            {
                                                selContent.Reference = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.SanadTop = subnode[0].InnerHtml;
                                                selContent.SanadBottom = subnode[1].InnerHtml;
                                            }

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentArabic = subnode.InnerHtml;
                                            }
                                        }
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;
                                    default: break;
                                }
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(selURL +":"+ ex.Message+"-"+ex.StackTrace);
                    }

                    ctx.SaveChanges();
                }
            }
            Console.WriteLine("selesai");
            Console.ReadLine();
        }

        static void GetIndoContent()
        {
            List<HadithContent> IndoContent = new List<HadithContent>();
            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.Name == "bukhari"
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, selIndex.No);

                    try
                    {
                        //CookieAwareWebClient client = new CookieAwareWebClient ();
                        //var Webget = new HtmlWeb();
                        MyWebClient client = new MyWebClient();
                        var doc = client.GetPage(selURL); //Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        //ctx.HadithPages.Add(selPage);
                                        break;

                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            if (subnode != null)
                                            {
                                                selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            if (subnode != null)
                                            {
                                                selChapter.Title = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            if (subnode != null)
                                            {
                                                selChapter.TitleArabic = subnode.InnerText;
                                            }
                                        }
                                        //ctx.HadithChapters.Add(selChapter);
                                        break;
                                    case "arabic achapintro":
                                        {
                                            if (selChapter != null)
                                            {
                                                selChapter.Intro = node.InnerText;
                                            }
                                            //ctx.SaveChanges();
                                        }
                                        break;
                                    case "actualHadithContainer":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        //selContent.ChapterNo = selPage.PageNo;
                                        if (selChapter != null)
                                            selContent.ChapterNo = selChapter.ChapterNo;
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentEnglish = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='indonesian_hadith_full']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentIndonesia = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");

                                            selContent.Reference = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            selContent.SanadTop = subnode[0].InnerHtml;
                                            selContent.SanadBottom = subnode[1].InnerHtml;

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            selContent.ContentArabic = subnode.InnerHtml;
                                        }
                                        IndoContent.Add(selContent);
                                        //ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            //ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;
                                    default: break;
                                }
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    Console.WriteLine(IndoContent.Count);
                    Console.ReadLine();
                    //ctx.SaveChanges();
                    break;
                }
            }
        }

        static void GetBulughContent()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.Name == "bulugh"
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, selIndex.No);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        //ctx.HadithPages.Add(selPage);
                                        break;

                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            if (subnode != null)
                                            {
                                                selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            if (subnode != null)
                                            {
                                                selChapter.Title = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            if (subnode != null)
                                            {
                                                selChapter.TitleArabic = subnode.InnerText;
                                            }
                                        }
                                        ctx.HadithChapters.Add(selChapter);
                                        break;
                                case "arabic achapintro":
                                    {
                                        selChapter.Intro = node.InnerText;
                                        ctx.SaveChanges();
                                    }
                                    break;
                                    case "actualHadithContainer":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        //selContent.ChapterNo = selPage.PageNo;
                                        if(selChapter!=null)
                                            selContent.ChapterNo = selChapter.ChapterNo;
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            selContent.ContentEnglish = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            selContent.Reference = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            selContent.SanadTop = subnode[0].InnerHtml;
                                            selContent.SanadBottom = subnode[1].InnerHtml;

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            selContent.ContentArabic = subnode.InnerHtml;
                                        }
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;
                                    default: break;
                                }
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }

        static void RepairShamailChapter()
        {
            HadithDBEntities ctx = new HadithDBEntities();
            var data = from c in ctx.HadithPages
                       where c.HadithID == 12
                       select c;
            foreach (var item in data)
            {
                HadithChapter chap = new HadithChapter();
                chap.PageNo = item.PageNo;
                chap.HadithID = item.HadithID;
                chap.Title = item.Title;
                chap.TitleArabic = item.TitleArabic;
                chap.ChapterNo = item.PageNo;
                ctx.HadithChapters.Add(chap);
            }
            ctx.SaveChanges();
        }

        static void GetShamailContent()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.Name == "shamail"
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, selIndex.No);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        //HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        //ctx.HadithPages.Add(selPage);
                                        break;
                                    /*
                                case "chapter":
                                    selChapter = new HadithChapter();
                                    selChapter.HadithID = selHadith.HadithID;
                                    selChapter.PageNo = selPage.PageNo;
                                    //iterate every chapter
                                    var chapterNode = node;
                                    {
                                        var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                        selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                    }
                                    {
                                        var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                        selChapter.Title = subnode.InnerText;
                                    }
                                    {
                                        var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                        selChapter.TitleArabic = subnode.InnerText;
                                    }
                                    ctx.HadithChapters.Add(selChapter);
                                    break;
                                case "arabic achapintro":
                                    {
                                        selChapter.Intro = node.InnerText;
                                        ctx.SaveChanges();
                                    }
                                    break;*/
                                    case "actualHadithContainer":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        selContent.ChapterNo = selPage.PageNo;
                                        //selContent.ChapterNo = selChapter.ChapterNo;
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            selContent.ContentEnglish = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            selContent.Reference = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            selContent.SanadTop = subnode[0].InnerHtml;
                                            selContent.SanadBottom = subnode[1].InnerHtml;

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            selContent.ContentArabic = subnode.InnerHtml;
                                        }
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;
                                    default: break;
                                }
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }

        static void GetQudsi40Content()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.Name == "qudsi40"
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];


                var selURL = string.Format("https://sunnah.com/{0}", selHadith.Name);

                try
                {
                    var Webget = new HtmlWeb();

                    var doc = Webget.Load(selURL);
                    //HadithChapter selChapter = null;
                    int ContentCounter = 0;

                    //HadithPage selPage = new HadithPage();
                    //selPage.PageNo = selIndex.No;
                    //selPage.HadithID = selHadith.HadithID;
                    //get title
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                    {
                        if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                        {
                            switch (node.Attributes["class"].Value)
                            {
                                /*
                                case "book_page_english_name":
                                    selPage.Title = node.InnerHtml;
                                    break;
                                case "book_page_arabic_name arabic":
                                    selPage.TitleArabic = node.InnerHtml;
                                    //ctx.HadithPages.Add(selPage);
                                    break;
                                    
                            case "chapter":
                                selChapter = new HadithChapter();
                                selChapter.HadithID = selHadith.HadithID;
                                selChapter.PageNo = selPage.PageNo;
                                //iterate every chapter
                                var chapterNode = node;
                                {
                                    var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                    selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                }
                                {
                                    var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                    selChapter.Title = subnode.InnerText;
                                }
                                {
                                    var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                    selChapter.TitleArabic = subnode.InnerText;
                                }
                                ctx.HadithChapters.Add(selChapter);
                                break;
                            case "arabic achapintro":
                                {
                                    selChapter.Intro = node.InnerText;
                                    ctx.SaveChanges();
                                }
                                break;*/
                                case "actualHadithContainer":
                                    HadithContent selContent = new HadithContent();
                                    selContent.HadithID = selHadith.HadithID;
                                    selContent.PageNo = 1;
                                    selContent.ChapterNo = 1;
                                    //selContent.PageNo = selPage.PageNo;
                                    //selContent.ChapterNo = selChapter.ChapterNo;
                                    {
                                        var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                        if (subnode != null)
                                        {
                                            selContent.Narated = subnode.InnerHtml;
                                        }
                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                        selContent.ContentEnglish = subnode.InnerHtml;
                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                        if (subnode != null)
                                        {
                                            selContent.Grade = subnode.InnerText;
                                        }
                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                        selContent.Reference = subnode.InnerHtml;
                                    }
                                    {
                                        var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                        selContent.SanadTop = subnode[0].InnerHtml;
                                        selContent.SanadBottom = subnode[1].InnerHtml;

                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                        selContent.ContentArabic = subnode.InnerHtml;
                                    }
                                    ctx.HadithContents.Add(selContent);
                                    ContentCounter++;
                                    if (ContentCounter > 100)
                                    {
                                        ctx.SaveChanges();
                                        ContentCounter = 0;
                                    }
                                    break;
                                default: break;
                            }
                        }


                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                ctx.SaveChanges();

            }
        }

        static void GetAdabContent()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.Name == "adab"
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, selIndex.No);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        //ctx.HadithPages.Add(selPage);
                                        break;

                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            if (subnode != null)
                                            {
                                                selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            if (subnode != null)
                                            {
                                                selChapter.Title = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            if (subnode != null)
                                            {
                                                selChapter.TitleArabic = subnode.InnerText;
                                            }
                                        }
                                        ctx.HadithChapters.Add(selChapter);
                                        
                                        break;
                                    case "arabic achapintro":
                                        {
                                            selChapter.Intro = node.InnerText;
                                            ctx.SaveChanges();
                                        }
                                        break;
                                        /*
                                    case "actualHadithContainer":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        selContent.ChapterNo = selChapter.ChapterNo;
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            selContent.ContentEnglish = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            selContent.Reference = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            selContent.SanadTop = subnode[0].InnerHtml;
                                            selContent.SanadBottom = subnode[1].InnerHtml;

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            selContent.ContentArabic = subnode.InnerHtml;
                                        }
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;*/
                                    default: break;
                                }
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }

        static void GetRyadSalihinContent()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.Name == "riyadussaliheen"
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, selIndex.No);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        //HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        //ctx.HadithPages.Add(selPage);
                                        break;
                                    /*
                                case "chapter":
                                    selChapter = new HadithChapter();
                                    selChapter.HadithID = selHadith.HadithID;
                                    selChapter.PageNo = selPage.PageNo;
                                    //iterate every chapter
                                    var chapterNode = node;
                                    {
                                        var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                        selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                    }
                                    {
                                        var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                        selChapter.Title = subnode.InnerText;
                                    }
                                    {
                                        var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                        selChapter.TitleArabic = subnode.InnerText;
                                    }
                                    ctx.HadithChapters.Add(selChapter);
                                    break;
                                case "arabic achapintro":
                                    {
                                        selChapter.Intro = node.InnerText;
                                        ctx.SaveChanges();
                                    }
                                    break;*/
                                    case "actualHadithContainer":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        selContent.ChapterNo = selPage.PageNo;
                                        //selContent.ChapterNo = selChapter.ChapterNo;
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            selContent.ContentEnglish = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            selContent.Reference = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            selContent.SanadTop = subnode[0].InnerHtml;
                                            selContent.SanadBottom = subnode[1].InnerHtml;

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            selContent.ContentArabic = subnode.InnerHtml;
                                        }
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;
                                    default: break;
                                }
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }

        static void RepairRyadSalihinChapter()
        {
            HadithDBEntities ctx = new HadithDBEntities();
            var data = from c in ctx.HadithPages
                       where c.HadithID == 9
                       select c;
            foreach (var item in data)
            {
                HadithChapter chap = new HadithChapter();
                chap.PageNo = item.PageNo;
                chap.HadithID = item.HadithID;
                chap.Title = item.Title;
                chap.TitleArabic = item.TitleArabic;
                chap.ChapterNo = item.PageNo;
                ctx.HadithChapters.Add(chap);
            }
            ctx.SaveChanges();
        }

        static void Test()
        {
            var Webget = new HtmlWeb();

            var doc = Webget.Load("https://sunnah.com/muslim/1");
            Console.WriteLine("start");
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
            {
                Console.WriteLine("class:" + (node.Attributes["class"] == null ? "-" : node.Attributes["class"].Value));
            }
            Console.ReadLine();
        }

        static void RepairMalikChapter()
        {
            HadithDBEntities ctx = new HadithDBEntities();
            var data = from c in ctx.HadithPages
                       where c.HadithID == 7
                       select c;
            foreach (var item in data)
            {
                HadithChapter chap = new HadithChapter();
                chap.PageNo = item.PageNo;
                chap.HadithID = item.HadithID;
                chap.Title = item.Title;
                chap.TitleArabic = item.TitleArabic;
                chap.ChapterNo = item.PageNo;
                ctx.HadithChapters.Add(chap);
            }
            ctx.SaveChanges();
        }

        static void GetNawawi40Content()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.Name == "nawawi40"
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];


                var selURL = string.Format("https://sunnah.com/{0}", selHadith.Name);

                try
                {
                    var Webget = new HtmlWeb();

                    var doc = Webget.Load(selURL);
                    //HadithChapter selChapter = null;
                    int ContentCounter = 0;

                    //HadithPage selPage = new HadithPage();
                    //selPage.PageNo = selIndex.No;
                    //selPage.HadithID = selHadith.HadithID;
                    //get title
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                    {
                        if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                        {
                            switch (node.Attributes["class"].Value)
                            {
                                /*
                                case "book_page_english_name":
                                    selPage.Title = node.InnerHtml;
                                    break;
                                case "book_page_arabic_name arabic":
                                    selPage.TitleArabic = node.InnerHtml;
                                    //ctx.HadithPages.Add(selPage);
                                    break;
                                    
                            case "chapter":
                                selChapter = new HadithChapter();
                                selChapter.HadithID = selHadith.HadithID;
                                selChapter.PageNo = selPage.PageNo;
                                //iterate every chapter
                                var chapterNode = node;
                                {
                                    var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                    selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                }
                                {
                                    var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                    selChapter.Title = subnode.InnerText;
                                }
                                {
                                    var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                    selChapter.TitleArabic = subnode.InnerText;
                                }
                                ctx.HadithChapters.Add(selChapter);
                                break;
                            case "arabic achapintro":
                                {
                                    selChapter.Intro = node.InnerText;
                                    ctx.SaveChanges();
                                }
                                break;*/
                                case "actualHadithContainer":
                                    HadithContent selContent = new HadithContent();
                                    selContent.HadithID = selHadith.HadithID;
                                    //selContent.PageNo = selPage.PageNo;
                                    //selContent.ChapterNo = selChapter.ChapterNo;
                                    {
                                        var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                        if (subnode != null)
                                        {
                                            selContent.Narated = subnode.InnerHtml;
                                        }
                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                        selContent.ContentEnglish = subnode.InnerHtml;
                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                        if (subnode != null)
                                        {
                                            selContent.Grade = subnode.InnerText;
                                        }
                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                        selContent.Reference = subnode.InnerHtml;
                                    }
                                    {
                                        var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                        selContent.SanadTop = subnode[0].InnerHtml;
                                        selContent.SanadBottom = subnode[1].InnerHtml;

                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                        selContent.ContentArabic = subnode.InnerHtml;
                                    }
                                    ctx.HadithContents.Add(selContent);
                                    ContentCounter++;
                                    if (ContentCounter > 100)
                                    {
                                        ctx.SaveChanges();
                                        ContentCounter = 0;
                                    }
                                    break;
                                default: break;
                            }
                        }


                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                ctx.SaveChanges();

            }
        }

        static void GetMalikContent()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.Name == "malik"
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, selIndex.No);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        //HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        //ctx.HadithPages.Add(selPage);
                                        break;
                                        /*
                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            selChapter.Title = subnode.InnerText;
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            selChapter.TitleArabic = subnode.InnerText;
                                        }
                                        ctx.HadithChapters.Add(selChapter);
                                        break;
                                    case "arabic achapintro":
                                        {
                                            selChapter.Intro = node.InnerText;
                                            ctx.SaveChanges();
                                        }
                                        break;*/
                                    case "actualHadithContainer":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        //selContent.ChapterNo = selChapter.ChapterNo;
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            selContent.ContentEnglish = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            selContent.Reference = subnode.InnerHtml;
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            selContent.SanadTop = subnode[0].InnerHtml;
                                            selContent.SanadBottom = subnode[1].InnerHtml;

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            selContent.ContentArabic = subnode.InnerHtml;
                                        }
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;
                                    default: break;
                                }
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }

        static void GetContent()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                      
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, selIndex.No);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        ctx.HadithPages.Add(selPage);
                                        break;
                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            if (subnode != null)
                                            {
                                                selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            if (subnode != null)
                                            {
                                                selChapter.Title = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            if (subnode != null)
                                            {
                                                selChapter.TitleArabic = subnode.InnerText;
                                            }
                                        }
                                        ctx.HadithChapters.Add(selChapter);
                                        break;
                                    case "arabic achapintro":
                                        {
                                            selChapter.Intro = node.InnerText;
                                            ctx.SaveChanges();
                                        }
                                        break;
                                    case "actualHadithContainer":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        if (selChapter != null)
                                        {
                                            selContent.ChapterNo = selChapter.ChapterNo;
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentEnglish = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            if (subnode != null)
                                            {
                                                selContent.Reference = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.SanadTop = subnode[0].InnerHtml;
                                                selContent.SanadBottom = subnode[1].InnerHtml;
                                            }

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentArabic = subnode.InnerHtml;
                                            }
                                        }
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;
                                    default: break;
                                }
                                if (node.Attributes["class"].Value.Contains("actualHadithContainer"))
                                {
                                    HadithContent selContent = new HadithContent();
                                    selContent.HadithID = selHadith.HadithID;
                                    selContent.PageNo = selPage.PageNo;
                                    if (selChapter != null)
                                    {
                                        selContent.ChapterNo = selChapter.ChapterNo;
                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                        if (subnode != null)
                                        {
                                            selContent.Narated = subnode.InnerHtml;
                                        }
                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                        if (subnode != null)
                                        {
                                            selContent.ContentEnglish = subnode.InnerHtml;
                                        }
                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                        if (subnode != null)
                                        {
                                            selContent.Grade = subnode.InnerText;
                                        }
                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                        if (subnode != null)
                                        {
                                            selContent.Reference = subnode.InnerHtml;
                                        }
                                    }
                                    {
                                        var subnode = node.SelectNodes(".//span[@class='arabic_sanad']");
                                        if (subnode != null)
                                        {
                                            selContent.SanadTop = subnode[0].InnerHtml;
                                            selContent.SanadBottom = string.Empty;// subnode[1].InnerHtml;
                                        }

                                    }
                                    {
                                        var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details']");
                                        if (subnode != null)
                                        {
                                            selContent.ContentArabic = subnode.InnerHtml;
                                        }
                                    }
                                    ctx.HadithContents.Add(selContent);
                                    ContentCounter++;
                                    if (ContentCounter > 100)
                                    {
                                        ctx.SaveChanges();
                                        ContentCounter = 0;
                                    }
                                }
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }
        static void GetContentMishkat()
        {
            int HadithId = 22;
            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.HadithID == HadithId
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                int OrderHadith = 0;
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    OrderHadith = 1;
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, (selIndex.No == 0 ? "introduction" : selIndex.No.ToString()));

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        var hadithnode = doc.DocumentNode.SelectSingleNode(".//div[@class='AllHadith']");
                        int ChapterCounter = 1;
                        //get title
                        foreach (HtmlNode node in hadithnode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        ctx.HadithPages.Add(selPage);
                                        break;
                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            if (subnode != null)
                                            {
                                                selChapter.ChapterNo = ChapterCounter++; //Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            if (subnode != null)
                                            {
                                                selChapter.Title = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            if (subnode != null)
                                            {
                                                selChapter.TitleArabic = subnode.InnerText;
                                            }
                                        }
                                        ctx.HadithChapters.Add(selChapter);
                                        break;
                                    case "arabic achapintro":
                                        {
                                            selChapter.Intro = node.InnerText;
                                            ctx.SaveChanges();
                                        }
                                        break;
                                    case "actualHadithContainer hadith_container_mishkat":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        selContent.HadithOrder = OrderHadith++;
                                        if (selChapter != null)
                                        {
                                            selContent.ChapterNo = selChapter.ChapterNo;
                                        }
                                        else
                                        {
                                            selContent.ChapterNo = -1;
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentEnglish = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                                selContent.Grade = Regex.Replace(selContent.Grade, @"<[^>]+>|&nbsp;", "").Trim();
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            if (subnode != null)
                                            {
                                                selContent.Reference = subnode.InnerHtml;
                                            }
                                            if (!string.IsNullOrEmpty(selContent.Reference))
                                            {
                                                HtmlDocument doc1 = new HtmlDocument();
                                                doc1.LoadHtml(selContent.Reference);
                                                int counter = 0;

                                                var nodes1 = doc1.DocumentNode.SelectNodes("//td");
                                                if (nodes1 != null)
                                                {
                                                    List<string> RefArray = new List<string>();
                                                    foreach (HtmlNode node1 in nodes1)
                                                    {
                                                        counter++;
                                                        string RefContent = node1.InnerText;
                                                        Console.WriteLine(counter + " - " + RefContent);
                                                        if (counter % 2 == 0)
                                                        {
                                                            RefArray.Add(RefContent);
                                                        }
                                                    }
                                                    if (RefArray.Count > 0)
                                                        selContent.Reference = Regex.Replace(RefArray[0], @"<[^>]+>|&nbsp;", "").Trim();

                                                    if (RefArray.Count > 1)
                                                        selContent.BookRef = Regex.Replace(RefArray[1], @"<[^>]+>|&nbsp;", "").Trim();

                                                    if (RefArray.Count > 2)
                                                        selContent.USCRef = Regex.Replace(RefArray[2], @"<[^>]+>|&nbsp;", "").Trim();
                                                }

                                            }
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.SanadTop = subnode[0].InnerHtml;
                                                if (subnode.Count > 1)
                                                    selContent.SanadBottom = subnode[1].InnerHtml;
                                            }

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentArabic = subnode.InnerHtml;
                                            }
                                        }
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;
                                    default: break;
                                }
                             
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }
        static void GetContent(int HadithId)
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.HadithID == HadithId
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                int OrderHadith = 0;
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    OrderHadith = 1;
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, (selIndex.No==0? "introduction" : selIndex.No.ToString()));

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        
                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        ctx.HadithPages.Add(selPage);
                                        break;
                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            if (subnode != null)
                                            {
                                                selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            if (subnode != null)
                                            {
                                                selChapter.Title = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            if (subnode != null)
                                            {
                                                selChapter.TitleArabic = subnode.InnerText;
                                            }
                                        }
                                        ctx.HadithChapters.Add(selChapter);
                                        break;
                                    case "arabic achapintro":
                                        {
                                            selChapter.Intro = node.InnerText;
                                            ctx.SaveChanges();
                                        }
                                        break;
                                    case "actualHadithContainer hadith_container_mishkat":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        selContent.HadithOrder = OrderHadith++;
                                        if (selChapter != null)
                                        {
                                            selContent.ChapterNo = selChapter.ChapterNo;
                                        }
                                        else
                                        {
                                            selContent.ChapterNo = -1;
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentEnglish = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                                selContent.Grade = Regex.Replace(selContent.Grade, @"<[^>]+>|&nbsp;", "").Trim();
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            if (subnode != null)
                                            {
                                                selContent.Reference = subnode.InnerHtml;
                                            }
                                            if (!string.IsNullOrEmpty(selContent.Reference))
                                            {
                                                HtmlDocument doc1 = new HtmlDocument();
                                                doc1.LoadHtml(selContent.Reference);
                                                int counter = 0;

                                                var nodes1 = doc1.DocumentNode.SelectNodes("//td");
                                                if (nodes1 != null)
                                                {
                                                    List<string> RefArray = new List<string>();
                                                    foreach (HtmlNode node1 in nodes1)
                                                    {
                                                        counter++;
                                                        string RefContent = node1.InnerText;
                                                        Console.WriteLine(counter + " - " + RefContent);
                                                        if (counter % 2 == 0)
                                                        {
                                                            RefArray.Add(RefContent);
                                                        }
                                                    }
                                                    if (RefArray.Count > 0)
                                                        selContent.Reference = Regex.Replace(RefArray[0], @"<[^>]+>|&nbsp;", "").Trim();
                                                       
                                                    if (RefArray.Count > 1)
                                                        selContent.BookRef = Regex.Replace(RefArray[1], @"<[^>]+>|&nbsp;", "").Trim();
                                                  
                                                    if (RefArray.Count > 2)
                                                        selContent.USCRef = Regex.Replace(RefArray[2], @"<[^>]+>|&nbsp;", "").Trim();
                                                }

                                            }
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.SanadTop = subnode[0].InnerHtml;
                                                if(subnode.Count>1)
                                                    selContent.SanadBottom = subnode[1].InnerHtml;
                                            }

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentArabic = subnode.InnerHtml;
                                            }
                                        }
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;
                                    default: break;
                                }
                                //if (node.Attributes["class"].Value.Contains("actualHadithContainer"))
                                //{
                                //    HadithContent selContent = new HadithContent();
                                //    selContent.HadithID = selHadith.HadithID;
                                //    selContent.PageNo = selPage.PageNo;
                                //    if (selChapter != null)
                                //    {
                                //        selContent.ChapterNo = selChapter.ChapterNo;
                                //    }
                                //    {
                                //        var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                //        if (subnode != null)
                                //        {
                                //            selContent.Narated = subnode.InnerHtml;
                                //        }
                                //    }
                                //    {
                                //        var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                //        if (subnode != null)
                                //        {
                                //            selContent.ContentEnglish = subnode.InnerHtml;
                                //        }
                                //    }
                                //    {
                                //        var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                //        if (subnode != null)
                                //        {
                                //            selContent.Grade = subnode.InnerText;
                                //        }
                                //    }
                                //    {
                                //        var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                //        if (subnode != null)
                                //        {
                                //            selContent.Reference = subnode.InnerHtml;
                                //        }
                                //    }
                                //    {
                                //        var subnode = node.SelectNodes(".//span[@class='arabic_sanad']");
                                //        if (subnode != null)
                                //        {
                                //            selContent.SanadTop = subnode[0].InnerHtml;
                                //            selContent.SanadBottom = string.Empty;// subnode[1].InnerHtml;
                                //        }

                                //    }
                                //    {
                                //        var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details']");
                                //        if (subnode != null)
                                //        {
                                //            selContent.ContentArabic = subnode.InnerHtml;
                                //        }
                                //    }
                                //    ctx.HadithContents.Add(selContent);
                                //    ContentCounter++;
                                //    if (ContentCounter > 100)
                                //    {
                                //        ctx.SaveChanges();
                                //        ContentCounter = 0;
                                //    }
                                //}
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }
        static void GetContentNawawi()
        {
            int HadithId = 20;
            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.HadithID == HadithId
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                int OrderHadith = 0;
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    OrderHadith = 1;
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}", selHadith.Name);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;

                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        ctx.HadithPages.Add(selPage);
                                        break;
                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            if (subnode != null)
                                            {
                                                selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            if (subnode != null)
                                            {
                                                selChapter.Title = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            if (subnode != null)
                                            {
                                                selChapter.TitleArabic = subnode.InnerText;
                                            }
                                        }
                                        ctx.HadithChapters.Add(selChapter);
                                        break;
                                    case "arabic achapintro":
                                        {
                                            selChapter.Intro = node.InnerText;
                                            ctx.SaveChanges();
                                        }
                                        break;
                                    case "actualHadithContainer hadith_container_forty":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        selContent.HadithOrder = OrderHadith++;
                                        if (selChapter != null)
                                        {
                                            selContent.ChapterNo = selChapter.ChapterNo;
                                        }
                                        else
                                        {
                                            selContent.ChapterNo = -1;
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            if (subnode != null)
                                            {
                                                selContent.Narated = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentEnglish = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            if (subnode != null)
                                            {
                                                selContent.Grade = subnode.InnerText;
                                                selContent.Grade = Regex.Replace(selContent.Grade, @"<[^>]+>|&nbsp;", "").Trim();
                                            }
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            if (subnode != null)
                                            {
                                                selContent.Reference = subnode.InnerHtml;
                                            }
                                            if (!string.IsNullOrEmpty(selContent.Reference))
                                            {
                                                HtmlDocument doc1 = new HtmlDocument();
                                                doc1.LoadHtml(selContent.Reference);
                                                int counter = 0;

                                                var nodes1 = doc1.DocumentNode.SelectNodes("//td");
                                                if (nodes1 != null)
                                                {
                                                    List<string> RefArray = new List<string>();
                                                    foreach (HtmlNode node1 in nodes1)
                                                    {
                                                        counter++;
                                                        string RefContent = node1.InnerText;
                                                        Console.WriteLine(counter + " - " + RefContent);
                                                        if (counter % 2 == 0)
                                                        {
                                                            RefArray.Add(RefContent);
                                                        }
                                                    }
                                                    if (RefArray.Count > 0)
                                                        selContent.Reference = Regex.Replace(RefArray[0], @"<[^>]+>|&nbsp;", "").Trim();

                                                    if (RefArray.Count > 1)
                                                        selContent.BookRef = Regex.Replace(RefArray[1], @"<[^>]+>|&nbsp;", "").Trim();

                                                    if (RefArray.Count > 2)
                                                        selContent.USCRef = Regex.Replace(RefArray[2], @"<[^>]+>|&nbsp;", "").Trim();
                                                }

                                            }
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.SanadTop = subnode[0].InnerHtml;
                                                if (subnode.Count > 1)
                                                    selContent.SanadBottom = subnode[1].InnerHtml;
                                            }

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='arabic_hadith_full arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentArabic = subnode.InnerHtml;
                                            }
                                        }
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        if (ContentCounter > 100)
                                        {
                                            ctx.SaveChanges();
                                            ContentCounter = 0;
                                        }
                                        break;
                                    default: break;
                                }
                              
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }
        static void GetHisnContent()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.Name == "hisn"
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}", selHadith.Name);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        var hadithnode = doc.DocumentNode.SelectSingleNode(".//div[@class='AllHadith']");
                        //get title
                        foreach (HtmlNode node in hadithnode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        //ctx.HadithPages.Add(selPage);
                                        break;

                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            if (subnode != null)
                                            {
                                                selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            if (subnode != null)
                                            {
                                                selChapter.Title = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            if (subnode != null)
                                            {
                                                selChapter.TitleArabic = subnode.InnerText;
                                            }
                                        }
                                        ctx.HadithChapters.Add(selChapter);

                                        break;
                                    case "arabic achapintro":
                                        {
                                            selChapter.Intro = node.InnerText;
                                            ctx.SaveChanges();
                                        }
                                        break;
                                    case "actualHadithContainer hadith_container_hisn":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        if (selChapter != null)
                                        {
                                            selContent.ChapterNo = selChapter.ChapterNo;
                                        }
                                        {
                                            //var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            //if (subnode != null)
                                            //{
                                            //    selContent.Narated = subnode.InnerHtml;
                                            //}
                                            selContent.Narated = "-";
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='translation']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentEnglish = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            //var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            //if (subnode != null)
                                            //{
                                            //    selContent.Grade = subnode.InnerText;
                                            //}
                                            selContent.Grade = "-";
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='hisn_english_reference']");
                                            if (subnode != null)
                                            {
                                                selContent.Reference = Regex.Replace(subnode.InnerHtml, @"<[^>]+>|&nbsp;", "").Trim() ;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.SanadTop = subnode[0].InnerHtml;
                                                if(subnode.Count>1)
                                                    selContent.SanadBottom = subnode[1].InnerHtml;
                                            }

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentArabic = subnode.InnerHtml;
                                            }
                                        }
                                        selContent.OtherRef = "";
                                        selContent.BookRef = "";
                                        selContent.UrlRef = $"https://sunnah.com/hisn:{selContent.ChapterNo}";
                                        selContent.USCRef = "";
                                        selContent.ContentIndonesia = "";
                                        selContent.ContentUrdu = "";
                                        selContent.HadithOrder = ContentCounter;
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;
                                        
                                        break;
                                    default: break;

                                }
                            }


                        }
                        if (ContentCounter > 100)
                        {
                            ctx.SaveChanges();
                            ContentCounter = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }
        static void GetWaliullahContent()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.HadithID == 21
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}", selHadith.Name);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int ContentCounter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        var hadithnode = doc.DocumentNode.SelectSingleNode(".//div[@class='AllHadith']");
                        //get title
                        int counter = 0;
                        foreach (HtmlNode node in hadithnode.SelectNodes("//div"))
                        {
                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                switch (node.Attributes["class"].Value)
                                {
                                    case "book_page_english_name":
                                        selPage.Title = node.InnerHtml;
                                        break;
                                    case "book_page_arabic_name arabic":
                                        selPage.TitleArabic = node.InnerHtml;
                                        //ctx.HadithPages.Add(selPage);
                                        break;

                                    case "chapter":
                                        selChapter = new HadithChapter();
                                        selChapter.HadithID = selHadith.HadithID;
                                        selChapter.PageNo = selPage.PageNo;
                                        //iterate every chapter
                                        var chapterNode = node;
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                            if (subnode != null)
                                            {
                                                selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                            if (subnode != null)
                                            {
                                                selChapter.Title = subnode.InnerText;
                                            }
                                        }
                                        {
                                            var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                            if (subnode != null)
                                            {
                                                selChapter.TitleArabic = subnode.InnerText;
                                            }
                                        }
                                        ctx.HadithChapters.Add(selChapter);

                                        break;
                                    case "arabic achapintro":
                                        {
                                            selChapter.Intro = node.InnerText;
                                            ctx.SaveChanges();
                                        }
                                        break;
                                    case "actualHadithContainer hadith_container_forty":
                                        HadithContent selContent = new HadithContent();
                                        selContent.HadithID = selHadith.HadithID;
                                        selContent.PageNo = selPage.PageNo;
                                        if (selChapter != null)
                                        {
                                            selContent.ChapterNo = selChapter.ChapterNo;
                                        }
                                        else
                                        {
                                            selContent.ChapterNo = -1;
                                        }
                                        {
                                            //var subnode = node.SelectSingleNode(".//div[@class='hadith_narrated']");
                                            //if (subnode != null)
                                            //{
                                            //    selContent.Narated = subnode.InnerHtml;
                                            //}
                                            selContent.Narated = "-";
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//div[@class='text_details']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentEnglish = subnode.InnerHtml;
                                            }
                                        }
                                        {
                                            //var subnode = node.SelectSingleNode(".//table[@class='gradetable']");
                                            //if (subnode != null)
                                            //{
                                            //    selContent.Grade = subnode.InnerText;
                                            //}
                                            selContent.Grade = "-";
                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//table[@class='hadith_reference']");
                                            if (subnode != null)
                                            {
                                                selContent.Reference = Regex.Replace(subnode.InnerText, @"<[^>]+>|&nbsp;", "").Trim() ;
                                            }
                                        }
                                        {
                                            var subnode = node.SelectNodes(".//span[@class='arabic_sanad arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.SanadTop = subnode[0].InnerHtml;
                                                if (subnode.Count > 1)
                                                    selContent.SanadBottom = subnode[1].InnerHtml;
                                            }

                                        }
                                        {
                                            var subnode = node.SelectSingleNode(".//span[@class='arabic_text_details arabic']");
                                            if (subnode != null)
                                            {
                                                selContent.ContentArabic = subnode.InnerHtml;
                                            }
                                        }
                                        counter++;
                                        selContent.OtherRef = "";
                                        selContent.BookRef = "";
                                        selContent.UrlRef = $"https://sunnah.com/shahwaliullah40:{counter}";
                                        selContent.USCRef = "";
                                        selContent.ContentIndonesia = "";
                                        selContent.ContentUrdu = "";
                                        selContent.HadithOrder = ContentCounter;
                                        ctx.HadithContents.Add(selContent);
                                        ContentCounter++;

                                        break;
                                    default: break;

                                }
                            }


                        }
                        if (ContentCounter > 100)
                        {
                            ctx.SaveChanges();
                            ContentCounter = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    ctx.SaveChanges();
                }
            }
        }
        static void GetChaptersHisn()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.HadithID == 18
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}", selHadith.Name);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int counter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        //get title
                        var nodes = doc.DocumentNode.SelectNodes("//div[@class='chapter_index_container']//div[contains(@class,'chapter_link title')]");
                        foreach (HtmlNode node in nodes)//doc.DocumentNode.SelectNodes("//div")
                        {
                            try
                            {
                                selChapter = new HadithChapter();
                                selChapter.Intro = "-";
                                selChapter.HadithID = selHadith.HadithID;
                                selChapter.PageNo = selPage.PageNo;
                                {
                                    var subnode = node.SelectSingleNode(".//div[@class='chapter_number title_number']");
                                    selChapter.ChapterNo = int.Parse(subnode.InnerText);
                                    selChapter.ChapterNoStr = subnode.InnerText;
                                }
                                {
                                    var subnode = node.SelectSingleNode(".//div[@class='english_chapter_name english']");
                                    selChapter.Title = subnode.InnerText;
                                }
                                {
                                    var subnode = node.SelectSingleNode(".//div[@class='arabic_chapter_name arabic']");
                                    selChapter.TitleArabic = subnode.InnerText;
                                }
                                ctx.HadithChapters.Add(selChapter);
                                counter++;
                            
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                           
                         


                        }
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("error luar:" + ex.Message + "-" + ex.StackTrace);
                    }

                    ctx.SaveChanges();
                }
            }
            Console.WriteLine("selesai baca chapter");
            Console.ReadLine();
        }
        static void GetChapters()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          select c).ToList();

            for (int i = 0; i < hadist.Count; i++)
            {
                var selHadith = hadist[i];
                var hadistIndex = (from c in ctx.HadithIndexes
                                   where c.HadithID == selHadith.HadithID
                                   orderby c.No
                                   select c).ToList();
                for (int j = 0; j < hadistIndex.Count; j++)
                {
                    var selIndex = hadistIndex[j];
                    var selURL = string.Format("https://sunnah.com/{0}/{1}", selHadith.Name, selIndex.No);

                    try
                    {
                        var Webget = new HtmlWeb();

                        var doc = Webget.Load(selURL);
                        HadithChapter selChapter = null;
                        int counter = 0;

                        HadithPage selPage = new HadithPage();
                        selPage.PageNo = selIndex.No;
                        selPage.HadithID = selHadith.HadithID;
                        //get title
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div"))
                        {

                            if (node.Attributes["class"] != null && !string.IsNullOrEmpty(node.Attributes["class"].Value))
                            {
                                try
                                {
                                    switch (node.Attributes["class"].Value)
                                    {
                                        case "book_page_english_name":
                                            selPage.Title = node.InnerHtml;
                                            break;
                                        case "book_page_arabic_name arabic":
                                            selPage.TitleArabic = node.InnerHtml;
                                            //ctx.HadithPages.Add(selPage);
                                            break;
                                        case "chapter":
                                            selChapter = new HadithChapter();
                                            selChapter.HadithID = selHadith.HadithID;
                                            selChapter.PageNo = selPage.PageNo;
                                            //iterate every chapter
                                            var chapterNode = node;
                                            {
                                                var subnode = chapterNode.SelectSingleNode(".//div[@class='echapno']");
                                                {
                                                    try
                                                    {
                                                        selChapter.ChapterNo = Convert.ToInt32(subnode.InnerText.Replace("(", "").Replace(")", ""));
                                                    }
                                                    catch
                                                    {
                                                        selChapter.ChapterNoStr = subnode.InnerText.Trim();
                                                        var Parsed = subnode.InnerText.Replace("(", "").Replace(")", "");
                                                        if (Parsed.Contains(','))
                                                        {
                                                            selChapter.ChapterNo = Convert.ToInt32(Parsed.Split(',')[0]);
                                                        }
                                                        else
                                                        {
                                                            for (int z = 0; z < Parsed.Length; z++)
                                                            {
                                                                if (!(Parsed[z] >= '0' && Parsed[z] <= '9'))
                                                                {
                                                                    Parsed = Parsed.Replace(Parsed[z].ToString(), " ");
                                                                }
                                                            }
                                                            selChapter.ChapterNo = Convert.ToInt32(Parsed.Trim());
                                                        }
                                                    }
                                                }
                                            }
                                            {
                                                var subnode = chapterNode.SelectSingleNode(".//div[@class='englishchapter']");
                                                if (subnode != null)
                                                {
                                                    selChapter.Title = subnode.InnerText.Trim();
                                                }
                                            }
                                            {
                                                var subnode = chapterNode.SelectSingleNode(".//div[@class='arabicchapter arabic']");
                                                if (subnode != null)
                                                {
                                                    selChapter.TitleArabic = subnode.InnerText.Trim();
                                                }
                                            }
                                            ctx.HadithChapters.Add(selChapter);
                                            counter++;
                                            if (counter > 100)
                                            {
                                                ctx.SaveChanges();
                                                counter = 0;
                                            }
                                            break;
                                        case "arabic achapintro":
                                            {
                                                selChapter.Intro = node.InnerText;

                                            }
                                            break;

                                        default: break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("error dalam:" + ex.Message + "-" + ex.StackTrace);
                                    continue;
                                }
                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("error luar:"+ex.Message + "-" + ex.StackTrace);
                    }

                    ctx.SaveChanges();
                }
            }
            Console.WriteLine("selesai baca chapter");
            Console.ReadLine();
        }

        static void GetIndex(int HadithID)
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                          where c.HadithID == HadithID
                          select c).ToList();

            for (int i = 0; i < hadist.Count(); i++)
            {
                var item = hadist[i];


                try
                {
                    var Webget = new HtmlWeb();

                    var doc = Webget.Load(string.Format("https://sunnah.com/{0}", item.Name));
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='colindextitle incomplete']//div[@class='arabic']"))
                    {
                        item.Arabic = node.InnerHtml;
                        continue;
                    }
                    var nodes = doc.DocumentNode.SelectNodes("//div[@class='book_titles titles']//div[contains(@class,'book_title title')]");
                    foreach (HtmlNode node in nodes)
                    {
                        HadithIndex newNode = new HadithIndex();
                        newNode.HadithID = item.HadithID;
                        try
                        {
                            {
                                var subnode = node.SelectSingleNode(".//div[@class='book_number title_number']");
                                newNode.No = Convert.ToInt32(string.IsNullOrEmpty(subnode.InnerText)?"0": subnode.InnerText);
                            }
                            {
                                var subnode = node.SelectSingleNode(".//div[@class='english english_book_name']");
                                newNode.Name = subnode.InnerText;
                            }
                            {
                                var subnode = node.SelectSingleNode(".//div[@class='arabic arabic_book_name']");
                                newNode.ArabicName = subnode.InnerText;
                            }
                            {
                                var subnode = node.SelectNodes(".//div[@class='book_range']");
                                var rangestr = subnode.FirstOrDefault().InnerText;

                                newNode.IndexFrom = Convert.ToInt32(Regex.Split(rangestr, "to")[0].Trim());
                                newNode.IndexTo = Convert.ToInt32(Regex.Split(rangestr, "to")[1].Trim());
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        ctx.HadithIndexes.Add(newNode);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                ctx.SaveChanges();

            }
        }

        static void GetIndex()
        {

            HadithDBEntities ctx = new HadithDBEntities();
            var hadist = (from c in ctx.Hadiths
                         select c).ToList();

            for (int i = 0; i < hadist.Count();i++ )
            {
                var item = hadist[i];


                try
                {
                    var Webget = new HtmlWeb();
                   
                    var doc = Webget.Load(string.Format("https://sunnah.com/{0}", item.Name));
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='colindextitle incomplete']//div[@class='arabic']"))
                    {
                        item.Arabic = node.InnerHtml;
                        continue;
                    }
                    var nodes =  doc.DocumentNode.SelectNodes("//div[@class='book_titles titles']//div[contains(@class,'book_title title')]");
                    foreach (HtmlNode node in nodes)
                    {
                        HadithIndex newNode = new HadithIndex();
                        newNode.HadithID = item.HadithID;
                        try
                        {
                            {
                                var subnode = node.SelectSingleNode(".//div[@class='book_number title_number']");
                                newNode.No = Convert.ToInt32(subnode.InnerText);
                            }
                            {
                                var subnode = node.SelectSingleNode(".//div[@class='english english_book_name']");
                                newNode.Name = subnode.InnerText;
                            }
                            {
                                var subnode = node.SelectSingleNode(".//div[@class='arabic arabic_book_name']");
                                newNode.ArabicName = subnode.InnerText;
                            }
                            {
                                var subnode = node.SelectNodes(".//div[@class='book_range']");
                                var rangestr = subnode.FirstOrDefault().InnerText;

                                newNode.IndexFrom = Convert.ToInt32(Regex.Split(rangestr,"to")[0].Trim());
                                newNode.IndexTo = Convert.ToInt32(Regex.Split(rangestr, "to")[1].Trim());
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message); 
                        }
                        ctx.HadithIndexes.Add(newNode);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message); 
                }
                
                ctx.SaveChanges();

            }
        }
       
        static void getHadith()
        {
            
            HadithDBEntities ctx = new HadithDBEntities();
            var perowih = new string[] { "bukhari", "muslim", "nasai", "abudawud", "tirmidhi", "ibnmajah", "malik",  "ahmad", "darimi",  "forty", "nawawi40", "riyadussalihin", "mishkat", "adab", "qudsi40", "shamail", "bulugh", "hisn" };
            foreach (var item in perowih)
            {
                Hadith newNode = new Hadith();
                newNode.Name = item;
         
                try
                {

                    var Webget = new HtmlWeb();
                    
                    var doc = Webget.Load(string.Format("https://sunnah.com/{0}/about", item));
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='abouttitle']"))
                    {
                        newNode.Title = node.InnerHtml;
                    }
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='about']"))
                    {
                        newNode.About = node.InnerHtml;
                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);

                }
                ctx.Hadiths.Add(newNode);
                ctx.SaveChanges();
            }
        }

        static void ParseData(string URL)
        {
            List<string> arabic = new List<string>();
            var Webget = new HtmlWeb();
            var doc = Webget.Load(URL);
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='arabic_hadith_full arabic']"))
            {
                arabic.Add(node.ChildNodes[0].InnerHtml);
                Console.WriteLine(node.ChildNodes[0].InnerHtml);
            }
            Console.ReadLine();
            /*
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//li[@class='tel']//a"))
            {
                phones.Add(node.ChildNodes[0].InnerHtml);
            }*/
        }
    }
}

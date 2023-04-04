using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot2.Crawler;
using Abot2.Poco;
using Abot2.Core;
using System.Net;

namespace Hadith.Crawler
{
    class Program
    {
        //static HadithDBEntities ctx;
        static List<Crawled> CrawledItems { set; get; }
        static string[] hadith = { "bukhari", "muslim", "nasai", "abudawud", "tirmidhi", "ibnmajah", "malik", "nawawi40", "riyadussaliheen", "adab", "qudsi40", "shamail", "bulugh" };
        //static Dictionary<int, string> DictHadith { set; get; }
        static void Main(string[] args)
        {
            HadithDBEntities ctx = new HadithDBEntities();
            var res = ctx.Database.CreateIfNotExists();
            Console.WriteLine(res);
            //Initiate logging based on web.config file
            log4net.Config.XmlConfigurator.Configure();
            //DictHadith = new Dictionary<int, string>();
            //int count = 0;
            //foreach (var item in hadith)
            //{
            //    DictHadith.Add(count++, item);
            //}
            CrawledItems = new List<Crawled>();
            DoCrawl();
            //Console.WriteLine("Crawling beres");
            Console.ReadLine();
        }

        static async void DoCrawl()
        {
            CrawlConfiguration crawlConfig = new CrawlConfiguration();//AbotConfigurationSectionHandler.LoadFromXml().Convert();
            crawlConfig.CrawlTimeoutSeconds = 100;
            crawlConfig.MaxConcurrentThreads = 100;
            crawlConfig.MaxPagesToCrawl = 10000;
            crawlConfig.UserAgentString = "abot v1.0 http://code.google.com/p/abot";
            crawlConfig.IsSendingCookiesEnabled = true;
            //crawlConfig.ConfigurationExtensions.Add("SomeCustomConfigValue1", "1111");
            //crawlConfig.ConfigurationExtensions.Add("SomeCustomConfigValue2", "2222");

            //Will use app.config for confguration
            PoliteWebCrawler crawler = new PoliteWebCrawler();

            crawler.PageCrawlStarting += crawler_ProcessPageCrawlStarting;
            crawler.PageCrawlCompleted += crawler_ProcessPageCrawlCompleted;
            crawler.PageCrawlDisallowed += crawler_PageCrawlDisallowed;
            crawler.PageLinksCrawlDisallowed += crawler_PageLinksCrawlDisallowed;

            CrawlResult result =await crawler.CrawlAsync(new Uri("http://sunnah.com/"));
            Console.WriteLine("jumlah crawled content :" + result.CrawlContext.CrawledCount);
            if (result.ErrorOccurred)
                Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
            else
                Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);


        }

        static void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        static bool isContains(string URL)
        {
            if (string.IsNullOrEmpty(URL)) return false;
            foreach (var item in hadith)
            {
                if (URL.Contains(item)) return true;
            }
            return false;
        }

        static void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            if (isContains(crawledPage.Uri.AbsoluteUri))
            {
                // Create a logger for use in this class
                //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

                //log.Info("Page URL : " + crawledPage.Uri.AbsoluteUri);
                CrawledItems.Add(new Crawled() { Url = crawledPage.Uri.AbsoluteUri, Description = crawledPage.Content.Text });
                if (CrawledItems.Count >= 10)
                {
                    AddToDatabase(CrawledItems.ToArray());
                    CrawledItems.Clear();
                    Console.WriteLine("Submit 10 new records");
                }
            }
            /*
            int count = 0;
            
            foreach (var item in crawledPage.ParsedLinks)
            {
                log.Info("link :"+ ++count +item.AbsoluteUri+", "+(item.IsFile?"ini file":"ini bukan file"));
            }*/
            //log.Info(crawledPage.Content.Text);
            if (crawledPage.HttpRequestException != null || crawledPage.HttpResponseMessage.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);

            //log.Info(string.Format("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri));

            Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
        }

        static void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        static void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }

        static void AddToDatabase(Crawled[] items)
        {
            try
            {
                //if (ctx == null) ctx = new HadithDBEntities();
                HadithDBEntities ctx = new HadithDBEntities();
                foreach (var item in items)
                {
                    ctx.Crawleds.Add(item);
                }
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }
        }
    }
}

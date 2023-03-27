using System;
using System.Collections.Generic;
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
using Hadith.WPF.Usercontrols;
using Hadith.WPF.Tools;
using System.Speech.Recognition;
using System.Threading;
using System.Globalization;
using System.Speech.Synthesis;

namespace Hadith.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct HadithState
        {
            public int HadithId { set; get; }
            public int PageNo { set; get; }
            public int ChapterNo { set; get; }
            public int HadithNo { set; get; }

            public int TotalHadith { set; get; }
            public int LanguageId { set; get; }

            public bool isPlaying { set; get; }
            public ListContent CtlContent { set; get; }
            public ListLanguage CtlLanguage { set; get; }
            public ListHadith CtlHadith { set; get; }
            public ListPages CtlPage { set; get; }
            public ListChapter CtlChapter { set; get; }
            public SearchPanel CtlSearch { set; get; }
            public ListBookmark CtlBookmark { set; get; }
            public Konfigurasi config { set; get; }
        }

        #region constants
        public const double MaxVolume = 1.0;
        public const double MinVolume = 0.0;
        public const int MaxVerseSize = 30;
        public const int MinVerseSize = 12;
        #endregion

        public static HadithState CurrentState;
        public static bool InternetState { set; get; }

        //Speech recognition & synthetizer
        static SpeechRecognitionEngine _recognizer = null;
        static ManualResetEvent manualResetEvent = null;
        public static bool isRecognizing { set; get; }
        public enum BossCommand { Stop, Read, Exit, NextChapter, PrevChapter, NoCommand, NextItem, PrevItem, GoToChapter, GoToItem, ZoomIn, ZoomOut, VolumeUp, VolumeDown, AddBookmark, LoadBookmark, OpenBookmark };
        public static Dictionary<string, BossCommand> Perintah { set; get; }
        static SpeechSynthesizer speechSynthesizer;
        //static System.Windows.Threading.DispatcherTimer dispatcherTimer;
        //Realsense
        //static ICamera cam;

        //timer shutdown
        //static int FaceOffCount = 0;
        #region Init App
        public MainWindow()
        {
            //var col = Color.FromRgb(32, 55,37);
            //MessageBox.Show(col.ToString());
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //read config
            string PathDB = Logs.getPath() + "\\DB\\Hadith.db";
            System.IO.FileInfo ConFile = new System.IO.FileInfo(PathDB);
            if (!ConFile.Exists)
            {
                MessageBox.Show("Database not found.");
                Application.Current.Shutdown();

            }
            string ConStr = string.Format("Data Source={0};", ConFile.FullName);
            BLL.HadithData.Conn = ConStr;

            //string ConStr = ConfigurationManager.ConnectionStrings["Hadith_context"].ConnectionString;
            //UrlRecitation = ConfigurationManager.AppSettings["UrlRecitation"];

            //player events
            HadithPlayer.MediaFailed += media_MediaFailed;
            HadithPlayer.MediaEnded += HadithPlayer_MediaEnded;

            //button events
            StopBtn.Click += StopBtn_Click;
            PlayBtn.Click += PlayBtn_Click;
            SpeechBtn.Click += SpeechBtn_Click;
            GestureBtn.Click += GestureBtn_Click;
            VoiceBtn.Click += VoiceBtn_Click;




            //QFE.WPF.Tools.CacheManager<string> Cache = new Tools.CacheManager<string>();
            //Cache["hosni"] = "gendut";
            //MessageBox.Show(Cache["hosni"]);

            InitHadith();

            //Init speech recognizer
            manualResetEvent = new ManualResetEvent(false);

            ListenToBoss();

            //slider event
            FontSlider.ValueChanged += FontSlider_ValueChanged;
            VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;

            /*
            //real sense start
            cam = Camera.Create(); //autodiscovers your sdk (perceptual or realsense)

            //cam.Face.Visible += Face_Visible;
            //cam.Face.NotVisible += Face_NotVisible;
            cam.Gestures.SwipeLeft += Gestures_SwipeLeft;
            cam.Gestures.SwipeRight += Gestures_SwipeRight;
            //cam.Gestures.SwipeUp += Gestures_SwipeUp;
            //cam.Gestures.SwipeDown += Gestures_SwipeDown;
            cam.RightHand.Moved += RightHand_Moved;
            cam.LeftHand.Moved += LeftHand_Moved;
            cam.Start();


            //timer for human detection
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, CurrentState.config.ShutdownTime, 0);
            dispatcherTimer.Start();
            */
        }


        #endregion

        /*
        #region Real Sense
        void LeftHand_Moved(Position obj)
        {
            if (!CurrentState.CtlSetting.isGestureEnable) return;
            if (obj.Image.Y < 200)
            {

                Dispatcher.BeginInvoke(
                   new ThreadStart(() =>
                   {
                       CurrentState.CtlContent.OnScrollDown(null, null);
                   }
                   ));

            }
            else
            {

                Dispatcher.BeginInvoke(
                   new ThreadStart(() =>
                   {
                       CurrentState.CtlContent.OnScrollUp(null, null);
                   }
                   ));

            }
        }
        void RightHand_Moved(Position obj)
        {
            if (!CurrentState.CtlSetting.isGestureEnable) return;
            if (obj.Image.Y < 200)
            {

                Dispatcher.BeginInvoke(
                   new ThreadStart(() =>
                   {
                       CurrentState.CtlContent.OnScrollDown(null, null);
                   }
                   ));

            }
            else
            {

                Dispatcher.BeginInvoke(
                   new ThreadStart(() =>
                   {
                       CurrentState.CtlContent.OnScrollUp(null, null);
                   }
                   ));

            }
        }
        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!cam.Face.IsVisible)
            {
                FaceOffCount++;
            }
            else
            {
                FaceOffCount = 0;
            }
            if (FaceOffCount > 1 && CurrentState.CtlSetting.isAutoShutdownEnable)
            {
                Dispatcher.BeginInvoke(
                     new ThreadStart(() =>
                     {
                         speechSynthesizer.Speak("I will shutdown this application");
                         Application.Current.Shutdown();
                     }
                     ));
            }
        }

        void Gestures_SwipeRight(Hand obj)
        {
            if (CurrentState.CtlSetting.isGestureEnable)
            {
                Dispatcher.BeginInvoke(
                    new ThreadStart(() =>
                    {
                        GoToNextChapter(null, null);
                    }
                    ));
            }
            //throw new NotImplementedException();
        }

        void Gestures_SwipeLeft(Hand obj)
        {
            if (CurrentState.CtlSetting.isGestureEnable)
            {
                Dispatcher.BeginInvoke(
                      new ThreadStart(() =>
                      {
                          GoToPrevChapter(null, null);
                      }
                      ));
            }
            //throw new NotImplementedException();
        }

        void Face_NotVisible()
        {

            //throw new NotImplementedException();
        }

        void Face_Visible()
        {
            //FaceOffCount = 0;
            //throw new NotImplementedException();
        }
        #endregion
        */

        #region Speech
        void ListenToBoss()
        {
            CultureInfo ci = new CultureInfo("en-US");
            _recognizer = new SpeechRecognitionEngine(ci);
            // Select a voice that matches a specific gender.  
            Perintah = new Dictionary<string, BossCommand>();


            Perintah.Add("read now", BossCommand.Read);
            Perintah.Add("stop read", BossCommand.Stop);
            Perintah.Add("next chapter", BossCommand.NextChapter);
            Perintah.Add("previous chapter", BossCommand.PrevChapter);
            Perintah.Add("next item", BossCommand.NextItem);
            Perintah.Add("previous item", BossCommand.PrevItem);
            Perintah.Add("zoom in", BossCommand.ZoomIn);
            Perintah.Add("zoom out", BossCommand.ZoomOut);
            Perintah.Add("volume up", BossCommand.VolumeUp);
            Perintah.Add("volume down", BossCommand.VolumeDown);
            Perintah.Add("add bookmark", BossCommand.AddBookmark);
            Perintah.Add("open bookmark", BossCommand.OpenBookmark);
            Perintah.Add("please turn off", BossCommand.Exit);

            foreach (KeyValuePair<string, BossCommand> entry in Perintah)
            {
                _recognizer.LoadGrammar(new Grammar(new GrammarBuilder(entry.Key)));
            }
            //special grammar
            _recognizer.LoadGrammar(specialGrammar());

            isRecognizing = false;
            _recognizer.SpeechRecognized += _recognizer_SpeechRecognized; // if speech is recognized, call the specified method
            _recognizer.SpeechRecognitionRejected += _recognizer_SpeechRecognitionRejected;
            _recognizer.SetInputToDefaultAudioDevice(); // set the input to the default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous
        }

        private static Grammar specialGrammar()
        {
            Choices navigations = new Choices(new string[] { "chapter", "item", "bookmark" });
            SemanticResultKey navigationKeys = new SemanticResultKey("navigation", navigations);

            Choices values = new Choices();

            for (int i = 1; i <= 300; i++)
                values.Add(i.ToString());
            SemanticResultKey valueKeys = new SemanticResultKey("values", values);

            // 2. navigation commands.
            GrammarBuilder navigationGrammarBuilder = new GrammarBuilder();
            navigationGrammarBuilder.Append(navigationKeys);
            navigationGrammarBuilder.Append(valueKeys);
            navigationGrammarBuilder.Culture = CultureInfo.GetCultureInfo("en-US");
            navigationGrammarBuilder.Append("go");

            // Create a Grammar object from the GrammarBuilder.
            return new Grammar(navigationGrammarBuilder);
        }

        void _recognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (!CurrentState.config.isVoiceEnable) return;
            if (e.Result.Alternates.Count == 0)
            {
                Console.WriteLine("Perintah tidak dikenal.");
                return;
            }
            Console.WriteLine("Perintah tidak dikenali, mungkin maksud tuan ini:");
            foreach (RecognizedPhrase r in e.Result.Alternates)
            {
                Console.WriteLine("    " + r.Text);
            }
        }

        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (!CurrentState.config.isVoiceEnable) return;
            try
            {
                if (isRecognizing) return;
                if (e.Result.Confidence < 0.85) return;
                int NavigationNum = -1;

                isRecognizing = true;

                BossCommand selCmd = BossCommand.NoCommand;
                if (Perintah.ContainsKey(e.Result.Text))
                {
                    selCmd = Perintah[e.Result.Text];
                }
                if (e.Result.Semantics != null && e.Result.Semantics.Count != 0)
                {
                    if (e.Result.Semantics.ContainsKey("navigation"))
                    {
                        if (e.Result.Semantics["navigation"].Value.ToString().Contains("chapter"))
                            selCmd = BossCommand.GoToChapter;
                        else if (e.Result.Semantics["navigation"].Value.ToString().Contains("item"))
                            selCmd = BossCommand.GoToItem;
                        else selCmd = BossCommand.LoadBookmark;
                        // Checks whether a step has been specified.
                        if (e.Result.Semantics.ContainsKey("values"))
                        {
                            if (!int.TryParse(e.Result.Semantics["values"].Value.ToString(), out NavigationNum)) return;
                        }
                    }
                }
                if (selCmd == BossCommand.NoCommand) return;

                switch (selCmd)
                {
                    case BossCommand.Read:
                        //speechSynthesizer.SpeakAsync("Ok sir");
                        PlayBtn_Click(null, null);
                        break;
                    case BossCommand.Stop:
                        StopBtn_Click(null, null);
                        //speechSynthesizer.SpeakAsync("I have stopped the reciter");
                        break;
                    case BossCommand.Exit:
                        StopBtn_Click(null, null);
                        speechSynthesizer.Speak("Turning off the application");
                        Application.Current.Shutdown();
                        break;
                    case BossCommand.NextChapter:
                        GoToNextChapter(null, null);
                        break;
                    case BossCommand.PrevChapter:
                        GoToPrevChapter(null, null);
                        break;
                    case BossCommand.NextItem:
                        if (CurrentState.isPlaying)
                            StopBtn_Click(null, null);
                        NextItem(false);
                        break;
                    case BossCommand.PrevItem:
                        if (CurrentState.isPlaying)
                            StopBtn_Click(null, null);
                        PreviousItem(false);
                        break;
                    case BossCommand.GoToItem:
                        if (NavigationNum > 0 && NavigationNum <= CurrentState.TotalHadith)
                        {
                            CurrentState.HadithNo = NavigationNum;
                            StopBtn_Click(null, null);
                            if (CurrentState.CtlContent.selBefore > -1)
                                CurrentState.CtlContent.SetItemHadith(CurrentState.CtlContent.selBefore, Brushes.White, false, false);

                            CurrentState.CtlContent.SetItemHadith(CurrentState.HadithNo - 1, Brushes.Orange, true, false);
                        }
                        break;
                    case BossCommand.GoToChapter:
                        if (NavigationNum >= CurrentState.CtlChapter.MinChapter && NavigationNum <= CurrentState.CtlChapter.MaxChapter)
                        {
                            if (CurrentState.isPlaying) StopBtn_Click(null, null);
                            CurrentState.CtlChapter.GoToSpecificItem(NavigationNum);
                            //LoadChapterByIndex(NavigationNum);
                        }
                        break;
                    case BossCommand.VolumeUp:
                        if (CurrentState.config.Volume + 0.2 <= MaxVolume)
                        {
                            VolumeSlider.Value = CurrentState.config.Volume + 0.2;
                        }
                        else
                        {
                            VolumeSlider.Value = MaxVolume;
                        }
                        break;
                    case BossCommand.VolumeDown:
                        if (CurrentState.config.Volume - 0.2 >= MinVolume)
                        {
                            VolumeSlider.Value = (CurrentState.config.Volume - 0.2);
                        }
                        else
                        {
                            VolumeSlider.Value = (MinVolume);
                        }
                        break;
                    case BossCommand.ZoomIn:
                        if (CurrentState.config.VerseSize + 2 <= MaxVerseSize)
                        {
                            FontSlider.Value = CurrentState.config.VerseSize + 2;
                        }
                        else
                        {
                            FontSlider.Value = (MaxVerseSize);
                        }
                        break;
                    case BossCommand.ZoomOut:
                        if (CurrentState.config.VerseSize - 2 >= MinVerseSize)
                        {
                            FontSlider.Value = (CurrentState.config.VerseSize - 2);
                        }
                        else
                        {
                            FontSlider.Value = (MinVerseSize);
                        }
                        break;
                    case BossCommand.AddBookmark:
                        AddBookmark(false);
                        speechSynthesizer.SpeakAsync("bookmark added");
                        break;
                    case BossCommand.LoadBookmark:
                        var res = CurrentState.CtlBookmark.LoadBookmark(NavigationNum);
                        if (!res)
                        {
                            speechSynthesizer.SpeakAsync(string.Format("bookmark {0} not found", NavigationNum));
                        }
                        break;
                    case BossCommand.OpenBookmark:
                        if (!ExpanderBookmark.IsExpanded)
                        {
                            ExpanderBookmark.IsExpanded = true;
                        }
                        break;
                }
                //speechSynthesizer.Dispose();
            }
            catch
            {
            }
            finally
            {
                isRecognizing = false;
            }
        }
        #endregion

        #region Button Handler
        void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentState.config.Volume = VolumeSlider.Value;
            speechSynthesizer.Volume = Convert.ToInt32(VolumeSlider.Value * 100);
        }

        void FontSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentState.CtlContent.ChangeVerseSize((int)FontSlider.Value);
            CurrentState.config.VerseSize = (int)FontSlider.Value;

        }

        void VoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentState.config.isVoiceEnable = !CurrentState.config.isVoiceEnable;
            //change icon
            if (CurrentState.config.isVoiceEnable)
            {
                changeBtnImage(VoiceBtn, "", "Images/Icons/voice-command-active.png");
            }
            else
            {
                changeBtnImage(VoiceBtn, "", "Images/Icons/voice-command-inactive.png");
            }
        }

        void GestureBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentState.config.isGestureEnable = !CurrentState.config.isGestureEnable;
            //change icon
            if (CurrentState.config.isGestureEnable)
            {
                changeBtnImage(GestureBtn, "", "Images/Icons/gesture-active.png");
            }
            else
            {
                changeBtnImage(GestureBtn, "", "Images/Icons/gesture-inactive.png");
            }
        }

        void SpeechBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentState.config.isAutoSpeech = !CurrentState.config.isAutoSpeech;
            //change icon
            if (CurrentState.config.isAutoSpeech)
            {
                changeBtnImage(SpeechBtn, "", "Images/Icons/auto-speech-active.png");
            }
            else
            {
                changeBtnImage(SpeechBtn, "", "Images/Icons/auto-speech-inactive.png");
            }

        }

        void changeBtnImage(Button btn, string ObjName, string ImgSrc)
        {
            StackPanel stk = (StackPanel)btn.Content;
            if (stk != null)
            {
                foreach (UIElement ctl in stk.Children)
                {
                    if (ctl is Image)
                    {
                        Image Selimg = ctl as Image;
                        Selimg.Source = ImageHelper.BitmapFromUri(new Uri(ImgSrc, UriKind.Relative));
                    }
                }
            }
        }

        void GoToPrevChapter(object sender, RoutedEventArgs e)
        {
            int NewChapter = CurrentState.ChapterNo - 1;
            if (NewChapter < CurrentState.CtlChapter.MinChapter) NewChapter = CurrentState.CtlChapter.MaxChapter;
            StopBtn_Click(null, null);
            CurrentState.CtlChapter.GoToSpecificItem(NewChapter);
            //LoadChapterByIndex(NewChapter);
        }

        void GoToNextChapter(object sender, RoutedEventArgs e)
        {
            int NewChapter = CurrentState.ChapterNo + 1;
            if (NewChapter > CurrentState.CtlChapter.MaxChapter) NewChapter = CurrentState.CtlChapter.MinChapter;
            StopBtn_Click(null, null);
            CurrentState.CtlChapter.GoToSpecificItem(NewChapter);
            //LoadChapterByIndex(NewChapter);
        }

        void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentState.isPlaying)
            {
                if (CurrentState.CtlContent.selBefore > -1)
                    CurrentState.CtlContent.SetItemHadith(CurrentState.CtlContent.selBefore, Brushes.White, false, false);
                CurrentState.CtlContent.SetItemHadith(CurrentState.HadithNo - 1, Brushes.Orange, true, true);
            }
        }

        void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            //isSpeaking = false;
            CurrentState.isPlaying = false;
            speechSynthesizer.SpeakAsyncCancelAll();
        }
        #endregion

        #region Forms Controls & Handler
        void AddBookmark(bool Prompt, BLL.BookmarkExt item = null)
        {
            if (item == null)
            {
                item = new BLL.BookmarkExt()
                {
                    HadithId = CurrentState.HadithId,
                    PageNo = CurrentState.PageNo,
                    ChapterNo = CurrentState.ChapterNo,
                    HadithNo = CurrentState.HadithNo
                };
            }
            string Judul = DateTime.Now.ToString("dd-MMM-yy HH:mm");
            if (Prompt)
            {
                item.HadithName = BLL.HadithData.getHadith(item.HadithId).Name;
                item.PageName = BLL.HadithData.getPage(item.HadithId, item.PageNo).Title;
                item.ChapterName = BLL.HadithData.getChapter(item.HadithId, item.PageNo, item.ChapterNo).Title;

                string NewJudul = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Please type bookmark title for {0} - {1} - {2} - {3}", item.HadithName, item.PageName, item.ChapterName, item.HadithNo), "Add Bookmark", Judul);
                if (string.IsNullOrEmpty(NewJudul)) return;
                Judul = NewJudul;
            }
            item.Title = Judul;
            BLL.HadithData.InsertBookmark(item);
            CurrentState.CtlBookmark.LoadBookmark();
        }

        async void InitHadith()
        {

            //setup state
            CurrentState = new HadithState();

            //setup data from config
            CurrentState.config = new Konfigurasi();
            CurrentState.HadithNo = CurrentState.config.HadithNoLastOpen;
            CurrentState.ChapterNo = CurrentState.config.ChapterLastOpen;
            CurrentState.LanguageId = CurrentState.config.LanguageLastOpen;
            CurrentState.PageNo = CurrentState.config.PageLastOpen;
            CurrentState.HadithId = CurrentState.config.HadithLastOpen;

            //setup button state

            if (CurrentState.config.isVoiceEnable)
            {
                changeBtnImage(VoiceBtn, "", "Images/Icons/voice-command-active.png");
            }
            else
            {
                changeBtnImage(VoiceBtn, "", "Images/Icons/voice-command-inactive.png");
            }

            if (CurrentState.config.isGestureEnable)
            {
                changeBtnImage(GestureBtn, "", "Images/Icons/gesture-active.png");
            }
            else
            {
                changeBtnImage(GestureBtn, "", "Images/Icons/gesture-inactive.png");
            }

            if (CurrentState.config.isAutoSpeech)
            {
                changeBtnImage(SpeechBtn, "", "Images/Icons/auto-speech-active.png");
            }
            else
            {
                changeBtnImage(SpeechBtn, "", "Images/Icons/auto-speech-inactive.png");
            }

            //set up slider
            VolumeSlider.Value = CurrentState.config.Volume;
            FontSlider.Value = CurrentState.config.VerseSize;

            //speech synth
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SelectVoiceByHints(VoiceGender.Female);
            speechSynthesizer.Volume = Convert.ToInt32(CurrentState.config.Volume * 100);
            speechSynthesizer.SpeakCompleted += speechSynthesizer_SpeakCompleted;

            //setup setting
            //CurrentState.CtlSetting = new ListSetting();
            //ExpanderSetting.Content = CurrentState.CtlSetting;


            //setup expander
            var Hd = BLL.HadithData.getHadith(CurrentState.HadithId);
            setExpanderTitle(ExpanderHadith, "HadithLbl", Hd.Title);

            var Pg = BLL.HadithData.getPage(CurrentState.HadithId, CurrentState.PageNo);
            setExpanderTitle(ExpanderPage, "PageLbl", Pg.Title);

            var Cp = BLL.HadithData.getChapter(CurrentState.HadithId, CurrentState.PageNo, CurrentState.ChapterNo);
            setExpanderTitle(ExpanderChapter, "ChapterLbl", Cp.Title);

            var Lng = BLL.HadithData.getLanguage(CurrentState.LanguageId);
            setExpanderTitle(ExpanderLanguage, "Langlbl", Lng.lang);

            //set caption
            NowLatinLbl.Text = Hd.Title.Trim() + " - " + Pg.Title.Trim();
            NowArabicLbl.Text = Pg.TitleArabic.Trim();

            //player state
            CurrentState.isPlaying = false;
            HadithPlayer.LoadedBehavior = MediaState.Manual;
            HadithPlayer.UnloadedBehavior = MediaState.Stop;
            //HadithPlayer.Volume = CurrentState.config.Volume;
            HadithPlayer.Stop();

            //setup language
            CurrentState.CtlLanguage = new ListLanguage();
            CurrentState.CtlLanguage.Height = 250;
            CurrentState.CtlLanguage.LanguageSelectEvent += CtlLanguage_LanguageSelectEvent;
            ExpanderLanguage.Content = CurrentState.CtlLanguage;

            //setup hadith
            CurrentState.CtlHadith = new ListHadith(false);
            CurrentState.CtlHadith.Height = 250;
            CurrentState.CtlHadith.HadithSelectEvent += CtlHadith_HadithSelectEvent;
            ExpanderHadith.Content = CurrentState.CtlHadith;

            //setup page
            CurrentState.CtlPage = new ListPages(CurrentState.HadithId, false);
            CurrentState.CtlPage.Height = 250;
            CurrentState.CtlPage.PagesSelectEvent += CtlPage_PagesSelectEvent;
            ExpanderPage.Content = CurrentState.CtlPage;

            //setup chapter
            CurrentState.CtlChapter = new ListChapter(CurrentState.HadithId, CurrentState.PageNo, false);
            CurrentState.CtlChapter.ChapterSelectEvent += CtlChapter_ChapterSelectEvent;
            CurrentState.CtlChapter.Height = 250;
            ExpanderChapter.Content = CurrentState.CtlChapter;
            //CurrentState.CtlChapter.LoadSurah(CurrentState.HadithId);

            //setup ayah
            CurrentState.CtlContent = new ListContent(CurrentState.HadithId, CurrentState.PageNo, CurrentState.ChapterNo, false, CurrentState.config.VerseSize, CurrentState.LanguageId);
            CurrentState.CtlContent.ContentSelectEvent += CtlContent_ContentSelectEvent;
            CurrentState.CtlContent.BookmarkSelectEvent += CtlContent_BookmarkSelectEvent;

            //setup search
            CurrentState.CtlSearch = new SearchPanel();
            CurrentState.CtlSearch.Height = 300;
            CurrentState.CtlSearch.SearchSelectEvent += CtlSearch_SearchSelectEvent;
            ExpanderSearch.Content = CurrentState.CtlSearch;

            HDPanel.Children.Add(CurrentState.CtlContent);

            //select last opened ayah
            //LoadChapterByIndex(CurrentState.ChapterNo, CurrentState.HadithNo);
            CurrentState.CtlContent.SetItemHadith(CurrentState.HadithNo - 1, Brushes.Orange, true, false);

            //load bookmark
            CurrentState.CtlBookmark = new ListBookmark();
            CurrentState.CtlBookmark.BookmarkSelectEvent += CtlBookmark_BookmarkSelectEvent;
            CurrentState.CtlBookmark.Height = 250;
            ExpanderBookmark.Content = CurrentState.CtlBookmark;

            //Internet check
            InternetState = await Hadith.WPF.Tools.Internet.CheckConnection(CurrentState.config.UrlRecitation);
            if (!InternetState) MessageBox.Show("No Internet connection.", "Warning");

        }

        void CtlContent_BookmarkSelectEvent(BLL.BookmarkExt Bookmark)
        {
            AddBookmark(true, Bookmark);
        }

        void CtlBookmark_BookmarkSelectEvent(BLL.BookmarkExt Bookmark)
        {
            CurrentState.HadithId = Bookmark.HadithId;

            CurrentState.PageNo = Bookmark.PageNo;
            CurrentState.CtlPage.PopulatePages(CurrentState.HadithId);

            CurrentState.ChapterNo = Bookmark.ChapterNo;
            CurrentState.CtlChapter.PopulateChapter(CurrentState.HadithId, CurrentState.PageNo);

            CurrentState.HadithNo = Bookmark.HadithNo;
            CurrentState.CtlContent.PopulateContent(CurrentState.HadithId, CurrentState.PageNo, CurrentState.ChapterNo, CurrentState.config.VerseSize, CurrentState.LanguageId);
            CurrentState.TotalHadith = BLL.HadithData.getTotalHadithInChapter(CurrentState.HadithId, CurrentState.PageNo, CurrentState.ChapterNo);

            //setup expander
            var Hd = BLL.HadithData.getHadith(CurrentState.HadithId);
            setExpanderTitle(ExpanderHadith, "HadithLbl", Hd.Title);

            var Pg = BLL.HadithData.getPage(CurrentState.HadithId, CurrentState.PageNo);
            setExpanderTitle(ExpanderPage, "PageLbl", Pg.Title);

            var Cp = BLL.HadithData.getChapter(CurrentState.HadithId, CurrentState.PageNo, CurrentState.ChapterNo);
            setExpanderTitle(ExpanderChapter, "ChapterLbl", Cp.Title);

            //set caption
            NowLatinLbl.Text = Hd.Title.Trim() + " - " + Pg.Title.Trim();
            NowArabicLbl.Text = Pg.TitleArabic.Trim();

            CurrentState.CtlContent.SetItemHadith(CurrentState.HadithNo - 1, Brushes.Orange, true, false);

        }

        void CtlSearch_SearchSelectEvent(BLL.SearchItem Search)
        {
            if (CurrentState.HadithId != Search.HadithId)
            {
                CurrentState.HadithId = Search.HadithId;
                setExpanderTitle(ExpanderHadith, "HadithLbl", Search.HadithTitle);
            }
            if (CurrentState.PageNo != Search.PageNo)
            {
                NowLatinLbl.Text = Search.HadithTitle.Trim() + " - " + Search.PageTitle.Trim();
                NowArabicLbl.Text = Search.PageArabic.Trim();

                CurrentState.CtlPage.PopulatePages(Search.HadithId);
                CurrentState.PageNo = Search.PageNo;
                setExpanderTitle(ExpanderPage, "PageLbl", Search.PageTitle);
            }
            if (CurrentState.ChapterNo != Search.ChapterNo.Value)
            {
                CurrentState.CtlChapter.PopulateChapter(Search.HadithId, Search.PageNo);
                CurrentState.ChapterNo = Search.ChapterNo.Value;
                CurrentState.CtlContent.PopulateContent(Search.HadithId, Search.PageNo, Search.ChapterNo.Value, CurrentState.config.VerseSize, CurrentState.LanguageId);
                setExpanderTitle(ExpanderChapter, "ChapterLbl", Search.ChapterTitle);
            }

            int HdNo = CurrentState.CtlContent.GoToSpecificItem(Search.ContentId);
            if (HdNo > -1)
            {
                CurrentState.HadithNo = HdNo;
            }
        }

        void speechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            /*
            if (e.Cancelled)
            {
                //isSpeaking = false;
            }
            else if(e.Prompt.IsCompleted)
            {
                NextItem(true);
            }*/
        }



        void CtlHadith_HadithSelectEvent(DAL.hadith item)
        {
            //hadith click
            CurrentState.HadithId = item.HadithID;
            setExpanderTitle(ExpanderHadith, "HadithLbl", item.Title);
            CurrentState.CtlPage.PopulatePages(CurrentState.HadithId);
            CurrentState.CtlPage.GoToFirstItem();
            //LoadChapterByIndex(CurrentState.CtlChapter.MinChapter, CurrentState.CtlChapter.MinAyah);
            //CurrentState.CtlContent.SetItemHadith(CurrentState.HadithNo - 1, Brushes.Orange, true, false);
        }

        void CtlPage_PagesSelectEvent(DAL.hadithindex page)
        {
            //page click
            CurrentState.PageNo = page.No;
            var selHadith = BLL.HadithData.getHadith(CurrentState.HadithId);
            NowLatinLbl.Text = selHadith.Title.Trim() + " - " + page.Name.Trim();
            NowArabicLbl.Text = page.ArabicName.Trim();
            setExpanderTitle(ExpanderPage, "PageLbl", page.Name);

            CurrentState.CtlChapter.PopulateChapter(CurrentState.HadithId, CurrentState.PageNo);
            CurrentState.CtlChapter.GoToFirstItem();
        }

        void CtlLanguage_LanguageSelectEvent(DAL.language Language)
        {
            //language click
            CurrentState.LanguageId = Language.langid;
            setExpanderTitle(ExpanderLanguage, "Langlbl", Language.lang);
            CurrentState.CtlContent.ChangeLanguage(CurrentState.LanguageId);
        }

        void setExpanderTitle(Expander exp, string LabelName, string Val)
        {
            StackPanel panel1 = (StackPanel)exp.Header;
            foreach (UIElement item in panel1.Children)
            {
                if (item is Label)
                {
                    Label lbl1 = (Label)item;
                    if (lbl1.Name == LabelName)
                    {
                        lbl1.Content = Val;
                    }
                }
            }
        }

        void NextItem(bool PlayState)
        {
            if (CurrentState.HadithNo + 1 > CurrentState.TotalHadith || (CurrentState.ChapterNo == CurrentState.CtlChapter.MaxChapter && CurrentState.HadithNo + 1 > CurrentState.TotalHadith))
            {
                if (CurrentState.ChapterNo + 1 > CurrentState.CtlChapter.MaxChapter)
                {
                    CurrentState.ChapterNo = CurrentState.CtlChapter.MinChapter;
                    CurrentState.HadithNo = 1;
                }
                else
                {
                    CurrentState.ChapterNo += 1;
                    CurrentState.HadithNo = 1;
                }
                CurrentState.CtlChapter.GoToSpecificItem(CurrentState.ChapterNo);
                //LoadChapterByIndex(CurrentState.ChapterNo, CurrentState.HadithNo);
            }
            else
            {
                CurrentState.HadithNo += 1;
            }

            if (CurrentState.CtlContent.selBefore > -1)
                CurrentState.CtlContent.SetItemHadith(CurrentState.CtlContent.selBefore, Brushes.White, false, false);
            CurrentState.CtlContent.SetItemHadith(CurrentState.HadithNo - 1, Brushes.Orange, true, PlayState);


        }

        void PreviousItem(bool PlayState)
        {

            if (CurrentState.HadithNo - 1 < 1 || (CurrentState.ChapterNo == CurrentState.CtlChapter.MinChapter && CurrentState.HadithNo - 1 < 1))
            {
                if (CurrentState.ChapterNo - 1 < CurrentState.CtlChapter.MinChapter)
                {
                    CurrentState.ChapterNo = CurrentState.CtlChapter.MaxChapter;
                    CurrentState.HadithNo = 1;
                }
                else
                {
                    CurrentState.ChapterNo -= 1;
                    CurrentState.HadithNo = 1;
                }
                CurrentState.CtlChapter.GoToSpecificItem(CurrentState.ChapterNo);
            }
            else
            {
                CurrentState.HadithNo -= 1;
            }

            if (CurrentState.CtlContent.selBefore > -1)
                CurrentState.CtlContent.SetItemHadith(CurrentState.CtlContent.selBefore, Brushes.White, false, false);
            CurrentState.CtlContent.SetItemHadith(CurrentState.HadithNo - 1, Brushes.Orange, true, PlayState);


        }

        void HadithPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            NextItem(true);
        }

        void CtlChapter_ChapterSelectEvent(DAL.hadithchapter chap)
        {
            CurrentState.ChapterNo = chap.ChapterNo;
            setExpanderTitle(ExpanderChapter, "ChapterLbl", chap.Title);

            CurrentState.CtlContent.PopulateContent(CurrentState.HadithId, CurrentState.PageNo, CurrentState.ChapterNo, CurrentState.config.VerseSize, CurrentState.LanguageId);
            CurrentState.TotalHadith = BLL.HadithData.getTotalHadithInChapter(CurrentState.HadithId, CurrentState.PageNo, CurrentState.ChapterNo);
            CurrentState.CtlContent.GoToFirstItem();
        }
        /*
        void LoadChapterByIndex(int NoSurah, int NoAyah = 1)
        {
            var item = BLL.HadithData.getChapter(NoSurah);
            CurrentState.ChapterNo = NoSurah;
            CurrentState.TotalAyah = item.totalayah;
            CurrentState.HadithNo = NoAyah;
            CurrentState.CtlContent.LoadAyah(NoSurah, CurrentState.LanguageId, CurrentState.CtlSetting.VerseSize);
            SurahLbl.Content = BLL.HadithData.AyahInArabic[item.idx] + ". " + item.name + " - " + item.latin;
            setExpanderTitle(ExpanderChapter, "Surahlbl", "Now Reciting: " + item.latin);
        }*/

        void CtlContent_ContentSelectEvent(BLL.HadithContentExt content, int Index)
        {
            CurrentState.HadithNo = Index;
            if (CurrentState.config.isAutoSpeech)
                Recite();

        }

        void Recite()
        {

            if (CurrentState.LanguageId == (int)BLL.HadithData.Languages.English)
            {
                var item = (BLL.HadithContentExt)CurrentState.CtlContent.ListData.SelectedItem;
                speechSynthesizer.SpeakAsync(item.Translation);
            }

        }

        void media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CurrentState.config.HadithNoLastOpen = CurrentState.HadithNo;
            CurrentState.config.ChapterLastOpen = CurrentState.ChapterNo;
            CurrentState.config.HadithLastOpen = CurrentState.HadithId;
            CurrentState.config.PageLastOpen = CurrentState.PageNo;
            CurrentState.config.LanguageLastOpen = CurrentState.LanguageId;
            CurrentState.config.Volume = VolumeSlider.Value;

            CurrentState.config.VerseSize = (int)FontSlider.Value;
            //CurrentState.config.ClickMode = CurrentState.CtlSetting.ClickMode;
            //CurrentState.config.PlayMode = CurrentState.CtlSetting.PlayMode;
            //CurrentState.config.isVoiceEnable = CurrentState.CtlSetting.isVoiceEnable;
            //CurrentState.config.isGestureEnable = CurrentState.CtlSetting.isGestureEnable;
            //CurrentState.config.isAutoShutdownEnable = CurrentState.CtlSetting.isAutoShutdownEnable;

            CurrentState.config.WriteSettings();
            //dispatcherTimer.Stop();

            //Dispose speech engine
            manualResetEvent.Set();

            if (_recognizer != null)
            {
                _recognizer.Dispose();
            }

            if (speechSynthesizer != null)
                speechSynthesizer.Dispose();
            /*
            try
            {
                if (cam != null)
                    cam.Dispose();
            }
            catch
            {
            }
            */

        }
        #endregion
    }
}

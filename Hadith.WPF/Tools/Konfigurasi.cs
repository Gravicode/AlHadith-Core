using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadith.WPF.Tools
{
    public class Konfigurasi
    {
        public int ShutdownTime { set; get; }
        public int VerseSize { set; get; }
        public bool isAutoSpeech { set; get; }
        public bool isVoiceEnable { set; get; }
        public bool isGestureEnable { set; get; }
        public bool isAutoShutdownEnable { set; get; }
        public double Volume { set; get; }
        public int HadithNoLastOpen { set; get; }
        public int ChapterLastOpen { set; get; }
        public int PageLastOpen { set; get; }
        public int LanguageLastOpen { set; get; }
        public int HadithLastOpen { set; get; }
        public string UrlRecitation { set; get; }

        public string TargetFile { set; get; }
        const string FileConfig = "hadith.ini";

        public Konfigurasi()
        {
            TargetFile = Logs.getPath() + "\\" + FileConfig;
            if (!File.Exists(TargetFile))
            {
                DefaultSetting();
                WriteSettings();
            }
            else
            {
                ReadSettings();
            }
        }

        ~Konfigurasi()
        {
            WriteSettings();
        }

        private void ReadSettings()
        {
            try
            {
                IniFile ini = new IniFile(TargetFile);
                Volume = double.Parse(ini.IniReadValue("config", "Volume"));
                HadithNoLastOpen = int.Parse(ini.IniReadValue("config", "HadithNoLastOpen"));
                ChapterLastOpen = int.Parse(ini.IniReadValue("config", "ChapterLastOpen"));
                PageLastOpen = int.Parse(ini.IniReadValue("config", "PageLastOpen"));
                LanguageLastOpen = int.Parse(ini.IniReadValue("config", "LanguageLastOpen"));
                HadithLastOpen = int.Parse(ini.IniReadValue("config", "HadithLastOpen"));
                UrlRecitation = ini.IniReadValue("config", "UrlRecitation");
                
                isAutoSpeech = bool.Parse(ini.IniReadValue("config", "isAutoSpeech"));
                VerseSize = int.Parse(ini.IniReadValue("config", "VerseSize"));
                //ClickMode = int.Parse(ini.IniReadValue("config", "ClickMode"));
                //PlayMode = int.Parse(ini.IniReadValue("config", "PlayMode"));
                isVoiceEnable = bool.Parse(ini.IniReadValue("config", "isVoiceEnable"));
                isGestureEnable = bool.Parse(ini.IniReadValue("config", "isGestureEnable"));
                isAutoShutdownEnable = bool.Parse(ini.IniReadValue("config", "isAutoShutdownEnable"));
                 ShutdownTime = int.Parse(ini.IniReadValue("config", "ShutdownTime"));
                
               
            }
            catch
            {
                throw;
            }
        }

        public void WriteSettings()
        {
            try
            {
                IniFile ini = new IniFile(TargetFile);
                ini.IniWriteValue("config", "Volume", Volume.ToString());
                ini.IniWriteValue("config", "HadithNoLastOpen", HadithNoLastOpen.ToString());
                ini.IniWriteValue("config", "ChapterLastOpen", ChapterLastOpen.ToString());
                ini.IniWriteValue("config", "PageLastOpen", PageLastOpen.ToString());
                ini.IniWriteValue("config", "LanguageLastOpen", LanguageLastOpen.ToString());
                ini.IniWriteValue("config", "HadithLastOpen", HadithLastOpen.ToString());
                ini.IniWriteValue("config", "UrlRecitation", UrlRecitation);

                ini.IniWriteValue("config", "VerseSize", VerseSize.ToString());
                //ini.IniWriteValue("config", "ClickMode", ClickMode.ToString());
                //ini.IniWriteValue("config", "PlayMode", PlayMode.ToString());
               
                ini.IniWriteValue("config", "isAutoSpeech", isAutoSpeech.ToString());
                ini.IniWriteValue("config", "isVoiceEnable", isVoiceEnable.ToString());
                ini.IniWriteValue("config", "isGestureEnable", isGestureEnable.ToString());
                ini.IniWriteValue("config", "isAutoShutdownEnable", isAutoShutdownEnable.ToString());
                ini.IniWriteValue("config", "ShutdownTime", ShutdownTime.ToString());

            }
            catch
            {
                throw;
            }
        }

        private void DefaultSetting()
        {
            Volume = 0.5;
            HadithNoLastOpen = 1;
            ChapterLastOpen = 1;
            PageLastOpen = 1;
            LanguageLastOpen = 0;
            HadithLastOpen = 1;
            UrlRecitation = "http://www.everyayah.com/";
            isAutoSpeech = false;
            ShutdownTime = 30;
            VerseSize = 16;
            //ClickMode = 0;
            //PlayMode = 2;
            isVoiceEnable = true;
            isGestureEnable = false;
            isAutoShutdownEnable = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Hadith.WPF.Tools
{
    public static class LinkHelper
    {

        public static readonly DependencyProperty LinkUrlProperty = DependencyProperty.RegisterAttached(
            "LinkUrl", typeof(string), typeof(LinkHelper), new PropertyMetadata(null, OnLinkUrlChanged));

        public static string GetLinkUrl(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(LinkUrlProperty);
        }

        public static void SetLinkUrl(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(LinkUrlProperty, value);
        }

        private static void OnLinkUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var teks = d as TextBlock;

            if (teks == null)
                return;

            var LinkUrl = e.NewValue.ToString();
            if (string.IsNullOrWhiteSpace(LinkUrl))
                teks.Visibility = Visibility.Collapsed;
            else
            {
                teks.Visibility = Visibility.Visible;
                string[] lnks = LinkUrl.Split('|');
                int counter = 1;
                foreach (var item in lnks)
                {
                    if (counter > 1)
                    {
                        teks.Inlines.Add(new Run(", "));
                    }
                    Hyperlink lnk = new Hyperlink(new Run("Link " + counter));
                    lnk.Foreground = Brushes.White;
                    lnk.NavigateUri = new Uri(item, UriKind.Absolute);
                    lnk.RequestNavigate += lnk_RequestNavigate;


                    teks.Inlines.Add(lnk);

                    counter++;
                }
            }
        }

        static void lnk_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
       
    }
}
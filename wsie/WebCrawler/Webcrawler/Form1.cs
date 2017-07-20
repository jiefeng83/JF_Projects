using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoScraper;
using AutoScraper.Attributes;
using AutoScraper.Events;
using AutoScraper.Schedulers;
using AutoScraper.TransformFilters;
using AutoScraper.WebClients;
using CsQuery;
using SilverBulletUtility;

namespace webcrawler2
{
    public partial class Form1 : Form
    {
        List<string> myFoodList = new List<string>();

        public Form1()
        {
            InitializeComponent();

            string x = "<span xcasfcasd> <span asfasfa fasfa> 12345 </span> </span>";
            x.cleanSpan();
            string y = x;


            //using (var scraper = new Crawler<InMemoryScheduler, DefaultWebClient>("http://sethlui.com"))
            //using (var scraper = new Crawler<InMemoryScheduler, DefaultWebClient>("http://danielfooddiary.com"))
            //using (var scraper = new Crawler<InMemoryScheduler, DefaultWebClient>("http://ieatishootipost.sg"))
            //using (var scraper = new Crawler<InMemoryScheduler, DefaultWebClient>("https://www.misstamchiak.com"))
            using (var scraper = new Crawler<InMemoryScheduler, DefaultWebClient>("http://sethlui.com/populus-cafe-singapore-cafe-review/"))
            {
                scraper.OnPageCrawlComplete += SethLui;
                //scraper.OnPageCrawlComplete += DanielFoodDairy;
                //scraper.OnPageCrawlComplete += ieatishootipost;
                //scraper.OnPageCrawlComplete += MissTamChiak;
                scraper.OnPageCrawlError += Scraper_OnPageCrawlError;

                // These are IMBD specific, not needed most of the time.
                //scraper.TransformFilters.Add(new RemoveQueryTranform());
                //scraper.TransformFilters.Add(new RemoveLastSlashTransform());

                scraper.Run();
            }
        }

        private void Scraper_OnPageCrawlError(Crawler<InMemoryScheduler, DefaultWebClient> sender, PageCrawlErrorEventArgs e)
        {
            Console.WriteLine($"Error: {e.Url}");
            SBUtility.LogDebug($"Error: {e.Url}");
        }

        private void SethLui(Crawler<InMemoryScheduler, DefaultWebClient> sender, PageCrawlCompleteEventArgs e)
        {
            try
            {
                if (e.Url.ToString().Contains("/feed/"))
                {
                    Console.WriteLine($"* IsFeed: {e.Url}");
                    return;
                }

                if (e.Url.ToString().Contains("/category/"))
                {
                    Console.WriteLine($"* IsCategory: {e.Url}");
                    return;
                }

                if (e.Html.Contains2("is closed"))
                {
                    Console.WriteLine($"* IsClosed: {e.Url}");
                    return;
                }

                string extract = e.Html.GetBetwString("<h3>", "</h3>").cleanString();

                if (extract == "")
                    extract = e.Html.GetBetwString("<em>", "</em>").cleanString();

                if (extract == "")
                {
                    Console.WriteLine($"* Cannot extract: {e.Url}");
                    SBUtility.LogDebug($"* Cannot extract: {e.Url}");
                    return;
                }

                if (extract == "")
                {
                    Console.WriteLine($"* Cannot extract: {e.Url}");
                    SBUtility.LogDebug($"* Cannot extract: {e.Url}");
                    return;
                }

                string[] strArr = extract.Split(new string[] { ":", "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (strArr.Length >= 2)
                {
                    string food = strArr[0];
                    string addr = strArr[1];

                    if (food != "" && !addr.Contains2("singapore") && !addr.Contains2("s("))
                    {
                        var s = food.Split(',');
                        food = s[0];
                        addr = food.Replace(food + ",", "");
                    }

                    if (addr != "" && food != "")
                    {
                        SBUtility.LogDebug($"* IsGood: {e.Url}^{food.cleanString()}^{addr.cleanString()}^-");
                        Console.WriteLine($"* IsGood: {e.Url}^{food.cleanString()}^{addr.cleanString()}^-");
                    }
                    else
                    {
                        SBUtility.LogDebug($"* Cant get info: {e.Url}");
                        Console.WriteLine($"* Cant get info: {e.Url}");
                    }
                }
                else
                {
                    SBUtility.LogDebug($"* Cant get info2: {e.Url}");
                    Console.WriteLine($"* Cant get info2: {e.Url}");
                }
            }
            catch
            {
                SBUtility.LogDebug($"* Parsing err: {e.Url}");
                Console.WriteLine($"* Parsing err: {e.Url}");
            }
        }

        private void MissTamChiak(Crawler<InMemoryScheduler, DefaultWebClient> sender, PageCrawlCompleteEventArgs e)
        {
            try
            {
                if (e.Url.ToString().IsProblemURL()) return;

                string addr = "";
                string food = "";
                if (e.Url.ToString().Contains("/feed/"))
                {
                    Console.WriteLine($"* IsFeed: {e.Url}");
                    return;
                }

                if (e.Url.ToString().Contains("/category/"))
                {
                    Console.WriteLine($"* IsCategory: {e.Url}");
                    return;
                }

                if (e.Html.Contains2("(Closed)"))
                {
                    Console.WriteLine($"* IsClosed: {e.Url}");
                    return;
                }

                int idx = -1;
                string extract = "";

                try
                {
                    idx = e.Html.IndexOf2("company-name__label");
                    extract = idx < 0 ? "" : e.Html.Substring(idx, 500).Replace("\n", "");

                    string[] strArr = extract.Split(new string[] { "<span>", "</span>", "<span class=\"company-address\">", "<spanclass=\"company-address\">" }, StringSplitOptions.RemoveEmptyEntries);

                    if (strArr.Length >= 4)
                    {
                        food = strArr[1];
                        addr = strArr[3];
                    }
                }
                catch { }

                try
                {
                    if (extract == "" || food == "" || addr == "")
                    {
                        food = "";
                        addr = "";
                        string addStr = "address:";
                        idx = e.Html.IndexOf2(addStr);
                        extract = idx < 0 ? "" : e.Html.Substring(idx - 200, 700).Replace("\n", "");

                        string kw = "<spanstyle=\"color: rgb(0, 0, 0); text-decoration: underline;\">";
                        idx = extract.IndexOf2(kw);
                        if (idx <= 0)
                        {
                            kw = "<spanstyle=\"color: #0000ff;\">";
                            idx = extract.IndexOf2(kw);
                        }
                        if (idx <= 0)
                        {
                            kw = "<spanstyle=\"text-decoration: underline; color: #000000;\">";
                            idx = extract.IndexOf2(kw);
                        }

                        if (idx <= 0)
                        {
                            kw = "<spanstyle=\"color: #000000; text-decoration: underline;\">";
                            idx = extract.IndexOf2(kw);
                        }
                        if (idx <= 0)
                        {
                            kw = "<spanstyle=\"color: #3366ff;\">";
                            idx = extract.IndexOf2(kw);
                        }
                        if (idx <= 0)
                        {
                            kw = "<spanstyle=\"color: #000000;\">";
                            idx = extract.IndexOf2(kw);
                        }
                        if (idx <= 0)
                        {
                            kw = "<h1 style=\"text-align: center;\">";
                            idx = extract.IndexOf2(kw);
                        }
                        if (idx <= 0)
                        {
                            kw = "<h2 style=\"text-align: center;\">";
                            idx = extract.IndexOf2(kw);
                        }

                        string extract2 = idx < 0 ? "" : extract.Substring(idx).Replace("\n", "").Replace(kw, "");

                        idx = extract2.IndexOf2(addStr);
                        string extract3 = idx < 0 ? "" : extract2.Substring(idx).Replace("\n", "").Replace(addStr, "");

                        idx = extract2.IndexOf2("</span>");
                        if (idx <= 0) idx = extract2.IndexOf2("</h1>");
                        if (idx <= 0) idx = extract2.IndexOf2("</h2>");
                        food = extract2.Substring(0, idx);

                        idx = extract3.IndexOf2("</span>");
                        if (idx <= 0) idx = extract3.IndexOf2("<br/>");
                        addr = extract3.Substring(0, idx);
                    }
                }
                catch { }

                try
                {
                    if (extract == "" || food == "" || addr == "")
                    {
                        food = "";
                        addr = "";
                        string kw = "<em>";
                        string kw2 = "</em>";
                        extract = e.Html.GetBetwString(kw, kw2).Replace("\n", "");

                        if (extract == "")
                        {
                            Console.WriteLine($"* Cant get info: {e.Url}");
                            return;
                        }

                        string br = "<br/>";
                        idx = extract.IndexOf2(br);
                        food = extract.Substring(0, idx);

                        int idx2 = extract.IndexOf2("tel:");
                        if (idx2 <= 0)
                            idx2 = extract.IndexOf("电话：");
                        if (idx2 <= 0)
                            idx2 = extract.IndexOf("ç”µè¯\u009dï¼š");

                        string x = "åœ°å\u009d€ï¼š";
                        if (idx2 > 0)
                            addr = extract.Substring(idx + br.Length, idx2 - idx - br.Length).Replace(br, " ").Replace(x, "");
                        else
                            addr = extract.Substring(idx + br.Length).Replace(br, " ").Replace(x, "");
                    }
                }
                catch { }

                if (addr != "" && food != "")
                {
                    SBUtility.LogDebug($"* IsGood: {e.Url}^{food.cleanString()}^{addr.cleanString()}");
                    Console.WriteLine($"* IsGood: {e.Url}^{food.cleanString()}^{addr.cleanString()}");
                }
                else
                {
                    SBUtility.LogDebug($"* Cant get info: {e.Url}");
                    Console.WriteLine($"* Cant get info: {e.Url}");
                }
            }
            catch
            { 
                SBUtility.LogDebug($"* Parsing err: {e.Url}");
                Console.WriteLine($"* Parsing err: {e.Url}");
            }
        }

        private void DanielFoodDairy(Crawler<InMemoryScheduler, DefaultWebClient> sender, PageCrawlCompleteEventArgs e)
        {
            try
            {
                if (e.Url.ToString().Contains("/feed/"))
                {
                    Console.WriteLine($"* IsFeed: {e.Url}");
                    return;
                }

                if (e.Url.ToString().Contains("/category/"))
                {
                    Console.WriteLine($"* IsCategory: {e.Url}");
                    return;
                }

                if (e.Html.Contains2("[Closed]"))
                {
                    Console.WriteLine($"* IsClosed: {e.Url}");
                    return;
                }
                int idx = e.Html.IndexOf2("Tel:");
                if (idx < 0) idx = e.Html.IndexOf("Opening Hours");
                if (idx < 0) idx = e.Html.IndexOf2("Opening Hours");
                if (idx < 0) idx = e.Html.IndexOf2("<a href=\"https://www.google.com/maps");

                string extract = idx < 0 ? "" : e.Html.Substring(idx - 200, 200).Replace("\n","");
                string[] strArr = extract.Split(new string[] { "<br />", "<br/>", "<Strong>", "<strong>"}, StringSplitOptions.RemoveEmptyEntries);
                if (strArr.Length >= 3)
                {
                    string food = strArr[1];
                    string addr = strArr[2];

                    if (addr != "" && food != "")
                    {
                        SBUtility.LogDebug($"* IsGood: {e.Url}^{food.cleanString()}^{addr.cleanString()}^-");
                        Console.WriteLine($"* IsGood: {e.Url}^{food.cleanString()}^{addr.cleanString()}^-");
                    }
                    else
                    {
                        SBUtility.LogDebug($"* Cant get info: {e.Url}");
                        Console.WriteLine($"* Cant get info: {e.Url}");
                    }
                }
                else
                {
                    SBUtility.LogDebug($"* Cant get info: {e.Url}");
                    Console.WriteLine($"* Cant get info: {e.Url}");
                }
            }
            catch
            {
                SBUtility.LogDebug($"* Parsing err: {e.Url}");
                Console.WriteLine($"* Parsing err: {e.Url}");
            }
        }

        void ieatishootipost(Crawler<InMemoryScheduler, DefaultWebClient> sender, PageCrawlCompleteEventArgs e)
        {
            try
            {
                CQ dom = e.Html;

                if (e.Url.ToString().Contains("/category/"))
                {
                    Console.WriteLine($"* IsCategory: {e.Url}");
                    return;
                }

                if (e.Html.Contains2("is closed"))
                {
                    Console.WriteLine($"* IsClosed: {e.Url}");
                    return;
                }

                if (e.Html.CheckIsClosed())
                {
                    Console.WriteLine($"* IsClosed1: {e.Url}");
                    return;
                }

                string food = dom["[class~=restaurant-name]"].Text();
                if (food == "") food = dom["[class~=article-title]"].Text();

                string extract = dom["[class=info]"].Text().Replace("\n", "").Replace("\t", "").Replace("Address:", "");
                string addr = "";

                if (extract.Contains2("View Map"))
                    addr = extract.Length == 0 ? "" : extract.Substring(0, extract.IndexOf2("View Map"));
                else if (extract.Contains2("Opening"))
                    addr = extract.Length == 0 ? "" : extract.Substring(0, extract.IndexOf2("Opening"));

                if (addr != "" && food != "")
                {
                    SBUtility.LogDebug($"* IsGood: {e.Url}^{food.cleanString()}^{addr.cleanString()}");
                    Console.WriteLine($"* IsGood: {e.Url}^{food.cleanString()}^{addr.cleanString()}");
                }
                else
                {
                    SBUtility.LogDebug($"* Cant get info: {e.Url}");
                    Console.WriteLine($"* Cant get info: {e.Url}");
                }
            }
            catch
            {
                SBUtility.LogDebug($"* Parsing err: {e.Url}");
                Console.WriteLine($"* Parsing err: {e.Url}");
            }
        }
    }

    public static class Utility
    {
        public static bool CheckIsClosed(this string source)
        {
            int index = source.IndexOf2("color: #ff0000;");
            if (index < 0) return false;

            string extract = source.Substring(index, 100);
            if (extract.Contains2("close")) return true;

            return false;
        }

        public static int IndexOf2(this string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase);
        }

        public static bool Contains2(this string source, string toCheck)
        {
            return source.IndexOf2(toCheck) >= 0;

        }

        public static string GetBetwString(this string str, string startStr, string endStr)
        {
            try
            {
                if (!str.Contains(startStr) || !str.Contains(endStr)) return "";
                int start = str.IndexOf(startStr) + startStr.Length;
                string tempStr = str.Substring(start, str.Length - start);
                int end = tempStr.IndexOf(endStr);
                return tempStr.Substring(0, end);
            }
            catch { return ""; }
        }

        public static string GetBetwStringInc(this string str, string startStr, string endStr)
        {
            try
            {
                if (!str.Contains(startStr) || !str.Contains(endStr)) return "";
                int start = str.IndexOf(startStr);
                string tempStr = str.Substring(start, str.Length - start);
                int end = tempStr.IndexOf(endStr) + endStr.Length;
                return tempStr.Substring(0, end);
            }
            catch { return ""; }
        }

        public static bool IsProblemURL(this string url)
        {
            switch(url)
            {
                case "https://www.misstamchiak.com/celebrating-iftar-at-aquamarine-marina-mandarin/":
                case "https://www.misstamchiak.com/le-chasseur/":
                case "https://www.misstamchiak.com/wgs-celebratory-set-menu-at-li-bai-sheraton-towers/":
                case "https://www.misstamchiak.com/kyushu-gourmet-experience-at-japanese-dining-sun/":
                case "https://www.misstamchiak.com/casa-verde/":
                case "https://www.misstamchiak.com/mediterranean-sunday-brunch-21-on-rajah/":
                case "":
                    return true;
            }
            return false;
        }

        public static string cleanString(this string name)
        {
            name = name.cleanSpan();
            name = name.cleanA();
            name = name.cleanImg();
            return name.Replace("&#8211;", "-").Replace("&#038;", "&").Replace("&#8217;", "'").Replace("&amp;", "&").Replace("<strong>", "").Replace("</strong>", "").Replace("&#8230;", "").Replace("<b>", "").Replace("</b>", "").Replace("<sup>TM</sup>", "").Replace("<em>", "").Replace("</em>", "").Replace("\n", "");
        }

        public static string cleanSpan(this string str)
        {
            try
            {
                str = str.Replace("</span>", "");
                string a = "-";

                while (a != "")
                {
                    a = str.GetBetwStringInc("<span", ">");
                    if(a != "") str = str.Replace(a, "");
                }

                str = str.Replace("<span", "");
                return str;
            }
            catch { return str; }
        }

        public static string cleanA(this string str)
        {
            try
            {
                str = str.Replace("</a>", "");
                string a = "-";

                while (a != "")
                {
                    a = str.GetBetwStringInc("<a", ">");
                    if (a != "") str = str.Replace(a, "");
                }

                str = str.Replace("<a", "");
                return str;
            }
            catch { return str; }
        }

        public static string cleanImg(this string str)
        {
            try
            {
                str = str.Replace("</img>", "");
                string a = "-";

                while (a != "")
                {
                    a = str.GetBetwStringInc("<img", ">");
                    if (a != "") str = str.Replace(a, "");
                }

                str = str.Replace("<img", "");
                return str;
            }
            catch { return str; }
        }
    }
}

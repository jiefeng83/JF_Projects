using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using SilverBulletUtility;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace FoodInfoParser
{
    public partial class Form1 : Form
    {
        const string blogCode = "ieisip";
        const string path = @"C:\Users\GenkCapital\Desktop\wsie\";
        public Form1()
        {
            InitializeComponent();
            //combine();
            Testing();

            var lines = File.ReadAllLines($@"{path}{blogCode}.log");

            Dictionary<string, FoodInfo> foodInfoList = new Dictionary<string, FoodInfo>();
            List<string> errLines = new List<string>();
            int i = 0;
            Regex re = new Regex(@"(?<!\d)\d{6}(?!\d)");

            var WB = new XLWorkbook();
            var WS = WB.Worksheets.Add($"{blogCode}");

            WS.Cell(1, 1).Value = "Name";
            WS.Cell(1, 2).Value = "URL";
            WS.Cell(1, 3).Value = "Lat";
            WS.Cell(1, 4).Value = "Lng";
            WS.Cell(1, 5).Value = "Address";

            List<string> alrAdded = new List<string>();
            int xx = 0;
            int a = 0;
            int b = 0;
            int c = 0;
            
            foreach (var line in lines)
            {
                xx++;
                string extract = "";
                try
                {
                    int index = line.IndexOf("* IsGood: ");
                    if (index < 0) {  continue; }

                    extract = line.Substring(index).Replace("* IsGood: ", "");
                    var arr = extract.Split('^');
                    if (arr.Length < 3) { WriteLine($"length < 3: {line}"); continue; }
                    if (arr[1].Contains2("address")) { WriteLine($"Contains address: {line}"); continue; }
                    if (arr[0].IsProblemURL()) { WriteLine($"IsProbURL: {line}"); continue; }

                    string uniqueName = arr[1].cleanString().Replace(" ", "");
                    if (alrAdded.Contains(uniqueName)) { WriteLine($"IsNotUnique: {line}"); continue; }

                    string address = arr[2].cleanString();
                    if (!address.Contains2("singapore") && !address.Contains2("s(")) { WriteLine($"Does not contain Singapore: {line}"); continue; }

                    string match = re.Match(address).Value;
                    Tuple<double, double, string> geo = null;

                    if (match != "")
                        geo = extractLatLong($"Singapore {match}");

                    if (geo == null)
                    {
                        if (match != "") address = address.Replace(match, "");
                        address = address.Replace("Singapore", "").Replace("singapore", "");

                        string road = "";
                        if (address.Contains('#')) road = address.Substring(0, address.IndexOf('#'));
                        geo = extractLatLong($"{road}, Singapore");
                    }

                    if (geo == null)
                    {
                        geo = extractLatLongBaseOnRules(address);
                    }

                    if (geo == null)
                    {
                        string[] addArr;

                        var unit = address.GetBetwString("#", " ");
                        if (unit == "") unit = address.GetBetwString("#", ",");

                        if (string.IsNullOrWhiteSpace(unit))
                            addArr = address.Split(new char[] { ',', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                        else
                            addArr = address.Split(new string[] { ",", "(", ")", $"#{unit}"}, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var part in addArr)
                        {
                            if (string.IsNullOrWhiteSpace(part) || Utility.IsDigitsOnly(part))  continue;

                            geo = extractLatLong($"{part}, Singapore");

                            if (geo != null)
                                break;
                        }
                    }
                    
                    if (geo == null)
                    {
                        WriteLine("ProblemAddress: " + arr[2]);
                        continue;
                    }

                    WriteLine($"GoodAddress: UniqueName={uniqueName} GoodPart={geo.Item3} ExtractFrom={arr[2]}");

                    alrAdded.Add(uniqueName);

                    var info = new FoodInfo();
                    info.name = arr[1].cleanString();
                    info.url = arr[0];
                    info.lat = geo.Item1;
                    info.lng = geo.Item2;
                    //info.blog = blogName;

                    WS.Cell(i + 2, 1).Value = info.name;
                    WS.Cell(i + 2, 2).Value = info.url;
                    WS.Cell(i + 2, 3).Value = info.lat;
                    WS.Cell(i + 2, 4).Value = info.lng;
                    WS.Cell(i + 2, 5).Value = arr[2].cleanString();

                    foodInfoList.Add(i.ToString(), info);
                    i++;
                }
                catch {
                    WriteLine("error: " + extract);
                }
            }
            int yy = xx;
            WB.SaveAs($@"{path}{blogCode}.xlsx");
            Dictionary<string, Dictionary<string, FoodInfo>> dict = new Dictionary<string, Dictionary<string, FoodInfo>>();
            dict.Add("items", foodInfoList);
            var json = JsonConvert.SerializeObject(dict);

            File.WriteAllText($@"{path}{blogCode}.json", json);

        }

        void combine()
        {
            string json1 = File.ReadAllText($@"{path}dfd.json");
            string json2 = File.ReadAllText($@"{path}ieisip.json");
            string json3 = File.ReadAllText($@"{path}mtc.json");
            string json4 = File.ReadAllText($@"{path}sl.json");

            var dict1 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, FoodInfo>>>(json1);
            var dict2 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, FoodInfo>>>(json2);
            var dict3 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, FoodInfo>>>(json3);
            var dict4 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, FoodInfo>>>(json4);

            List<FoodInfo> list1 = dict1["items"].Values.ToList();
            List<FoodInfo> list2 = dict2["items"].Values.ToList();
            List<FoodInfo> list3 = dict3["items"].Values.ToList();
            List<FoodInfo> list4 = dict4["items"].Values.ToList();

            List<FoodInfo> combineList = new List<FoodInfo>();
            combineList.AddRange(list1);
            combineList.AddRange(list2);
            combineList.AddRange(list3);
            combineList.AddRange(list4);

            //var sortedList = combineList.OrderBy(a => Guid.NewGuid());
            var sortedList = combineList.OrderBy(a => a.lat).ThenBy(a=>a.lng);
            Dictionary<string, FoodInfo> newFoodList = new Dictionary<string, FoodInfo>();
            int i = 0;
            foreach(var f in sortedList)
            {
                if (f.url.Contains("/feed/") || f.name.Contains("href=")) continue;
                newFoodList.Add(i.ToString(), f);
                i++;
            }

            Dictionary<string, Dictionary<string, FoodInfo>> dict = new Dictionary<string, Dictionary<string, FoodInfo>>();
            dict.Add("items", newFoodList);
            var json = JsonConvert.SerializeObject(dict);
            File.WriteAllText($@"{path}wsieDB.json", json);
        }


        Tuple<double, double, string> extractLatLongBaseOnRules(string address)
        {
            string newAdd = address.Replace(" ", "").ToLower();

            if (!newAdd.Contains("foodcentre") && !newAdd.Contains("hawkercentre") && !newAdd.Contains("market") && !newAdd.Contains("fc")) return null;

            if (newAdd.Contains("oldairport")) return new Tuple<double, double, string>(1.308201, 103.885793, address);
            else if (newAdd.Contains("honglim")) return new Tuple<double, double, string>(1.285278, 103.845961, address);
            else if (newAdd.Contains("amoy")) return new Tuple<double, double, string>(1.279204, 103.846636, address);
            else if (newAdd.Contains("circuit")) return new Tuple<double, double, string>(1.327811, 103.887280, address);
            else if (newAdd.Contains("maxwell")) return new Tuple<double, double, string>(1.280438, 103.844829, address);
            else if (newAdd.Contains("abcbrickworks")) return new Tuple<double, double, string>(1.286869, 103.808055, address);
            else if (newAdd.Contains("alexandravillage")) return new Tuple<double, double, string>(1.286235, 103.804447, address);
            else if (newAdd.Contains("albert")) return new Tuple<double, double, string>(1.300902, 103.854114, address);
            else if (newAdd.Contains("haigroad")) return new Tuple<double, double, string>(1.315232, 103.895518, address);
            else if (newAdd.Contains("chinatown") || newAdd.Contains("smith")) return new Tuple<double, double, string>(1.282579, 103.843095, address);
            else if (newAdd.Contains("whampoa")) return new Tuple<double, double, string>(1.323326, 103.854105, address);
            else if (newAdd.Contains("bukitmerahview")) return new Tuple<double, double, string>(1.284685, 103.822162, address);
            else if (newAdd.Contains("bukittimah")) return new Tuple<double, double, string>(1.340775, 103.775813, address);
            else if (newAdd.Contains("chompchomp")) return new Tuple<double, double, string>(1.364205, 103.866557, address);
            else if (newAdd.Contains("commonwealthcresecent")) return new Tuple<double, double, string>(1.306959, 103.800374, address);
            else if (newAdd.Contains("dunman")) return new Tuple<double, double, string>(1.309362, 103.901839, address);
            else if (newAdd.Contains("geylangeast")) return new Tuple<double, double, string>(1.320432, 103.886934, address);
            else if (newAdd.Contains("geylangserai")) return new Tuple<double, double, string>(1.316810, 103.898293, address);
            else if (newAdd.Contains("ghimmoh")) return new Tuple<double, double, string>(1.311145, 103.788250, address);
            else if (newAdd.Contains("goldenmile")) return new Tuple<double, double, string>(1.303122, 103.863853, address);
            else if (newAdd.Contains("goldenshoe")) return new Tuple<double, double, string>(1.283888, 103.849936, address);
            else if (newAdd.Contains("marineparade")) return new Tuple<double, double, string>(1.302172, 103.906320, address);
            else if (newAdd.Contains("meiling")) return new Tuple<double, double, string>(1.293187, 103.802908, address);
            else if (newAdd.Contains("newton")) return new Tuple<double, double, string>(1.311992, 103.839578, address);
            else if (newAdd.Contains("peoplespark")) return new Tuple<double, double, string>(1.285437, 103.843686, address);
            else if (newAdd.Contains("tekka")) return new Tuple<double, double, string>(1.306272, 103.850659, address);
            else if (newAdd.Contains("simlim")) return new Tuple<double, double, string>(1.303017, 103.852957, address);
            else if (newAdd.Contains("bersih")) return new Tuple<double, double, string>(1.307463, 103.856730, address);
            else if (newAdd.Contains("tanglinhalt")) return new Tuple<double, double, string>(1.299663, 103.797882, address);
            else if (newAdd.Contains("tiongbahru")) return new Tuple<double, double, string>(1.284861, 103.832320, address);
            else if (newAdd.Contains("toapayohlor1")) return new Tuple<double, double, string>(1.337823, 103.844762, address);
            else if (newAdd.Contains("toapayohlor4")) return new Tuple<double, double, string>(1.334473, 103.851966, address);
            else if (newAdd.Contains("toapayohlor5")) return new Tuple<double, double, string>(1.336103, 103.852996, address);
            else if (newAdd.Contains("toapayohlor7")) return new Tuple<double, double, string>(1.335541, 103.857080, address);
            return null;
        }

        Tuple<double, double, string> extractLatLong(string address)
        {
            try
            {
                var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyCsJAK5pem81Fka-9Bcob8FmuC72OsE98s&address={0}&sensor=false", Uri.EscapeDataString(address));

                var request = WebRequest.Create(requestUri);
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());

                var result = xdoc.Element("GeocodeResponse").Element("result");
                if (result == null)
                {
                    SBUtility.LogDebug(xdoc.ToString());
                    return null;
                }
                var locationElement = result.Element("geometry").Element("location");

                var tuple = new Tuple<double, double, string>((double)locationElement.Element("lat"), (double)locationElement.Element("lng"), address);

                if (tuple.Item1.ToString() == "1.352083" && tuple.Item2.ToString() == "103.819836")
                    return null;
                else
                    return tuple;
            }
            catch { return null; }
        }

        void getProblemAddress()
        {
            var lines2 = File.ReadAllLines(@"C:\Users\GenkCapital\Desktop\probadd.txt");
            List<string> lll = new List<string>();
            foreach (var l in lines2)
            {
                lll.Add(l.Substring(59));
            }

            string xxx = "";
            foreach (var l in lll)
            {
                string ll = l.Replace(",", "");
                xxx += ll + "," + ll.Replace(" ", "").ToLower() + "\n";
            }
            File.WriteAllText(@"C:\Users\GenkCapital\Desktop\probadd2.csv", xxx);
        }

        void WriteLine(string text)
        {
            Console.WriteLine(text);
            SBUtility.LogDebug(text);
        }

        void Testing()
        {
            var a = File.ReadAllLines($"{path}sl.log");
            var b = File.ReadAllLines($"{path}logfile.log");

            var a1 = new List<Info>();
            var b1 = new List<Info>();

            string x;
            int idx;
            string xx = "http://sethlui.com//";

            foreach (var s in a)
            {
                x = "Cant get info2:";
                idx = s.IndexOf(x);
                if (idx >= 0) { a1.Add(new Info(s.Substring(idx + x.Length + xx.Length), x)); continue; }

                x = "IsGood:";
                idx = s.IndexOf(x);
                if (idx >= 0) { a1.Add(new Info(s.Substring(idx + x.Length + xx.Length), x)); continue; }

                x = "Cannot extract:";
                idx = s.IndexOf(x);
                if (idx >= 0) { a1.Add(new Info(s.Substring(idx + x.Length + xx.Length), x)); continue; }
            }

            foreach (var s in b)
            {
                x = "Cant get info2:";
                idx = s.IndexOf(x);
                if (idx >= 0) { b1.Add(new Info(s.Substring(idx + x.Length + xx.Length), x)); continue; }

                x = "IsGood:";
                idx = s.IndexOf(x);
                if (idx >= 0) { b1.Add(new Info(s.Substring(idx + x.Length + xx.Length), x)); continue; }

                x = "Cannot extract:";
                idx = s.IndexOf(x);
                if (idx >= 0) { b1.Add(new Info(s.Substring(idx + x.Length + xx.Length), x)); continue; }
            }

            a1 = a1.OrderBy(y => y.url).ToList();
            b1 = b1.OrderBy(y => y.url).ToList();

            string aa = "";
            string bb = "";
            foreach (var s in a1)
                aa += $"{s.url} - {s.res}\n";
            foreach (var s in b1)
                bb += $"{s.url} - {s.res}\n";

            File.WriteAllText(path + "aa.txt", aa);
            File.WriteAllText(path + "bb.txt", bb);
        }
    }

    public class Info
    {
        public string url = "";
        public string res = "";

        public Info(string a, string b)
        {
            url = a;
            res = b;
        }

    }

    public class FoodInfo
    {
        public string name = "";
        public string url = "";
        public double lat = 0;
        public double lng = 0;
        public string placeType = "";
    }

    public static class Utility
    {
        public static string CheckIsClosed(this string source, string toCheck)
        {
            int index = source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase);
            if (index < 0) return "-";
            else return source.Substring(index - 15, 25);

        }

        public static bool Contains2(this string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
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

        public static bool IsDigitsOnly(string str)
        {
            string toCheck = str.Replace("Blk", "").Replace("Block", "");

            if (toCheck.Contains("Japan") || toCheck.Contains("Malaysia") || toCheck.Contains("Indonesia")) return true;

            foreach (char c in toCheck)
            {
                if (char.IsWhiteSpace(c)) continue;
                if (c < '0' || c > '9' || c == ' ')
                    return false;
            }

            return true;
        }

        public static bool IsProblemURL(this string url)
        {
            switch (url)
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
            return name.Replace("&#8211;", "-").Replace("&#038;", "&").Replace("&#8217;", "'").Replace("&amp;", "&").Replace("<strong>", "").Replace("</strong>", "").Replace("&#8230;", "").Replace("<b>", "").Replace("</b>", "").Replace("<sup>TM</sup>", "").Replace("<em>", "").Replace("</em>", "").Replace("\n", "").Replace("<i>", "").Replace("</i>", "");
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
                    if (a != "") str = str.Replace(a, "");
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

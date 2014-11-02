using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Globalization;

namespace StockFundamentalStudy
{

    public partial class Form1 : Form
    {
        bool isFirstLoad = true;
        bool startExtract = false;
        bool extractSuccess = false;
        int retries = 0;
        string extractingSymbol = "";

        int totalStockNum = 0;
        int currStockNum = 0;

        string FindNetEarnings = " Net Earnings</STRONG> </TH>";
        string FindRevenue = " Revenue</STRONG> </TH>";
        string FindShareHolderEquity = " Shareholders' Equity</STRONG> </TH>";
        string FindLTLiabilities = " Long Term Liabilities</STRONG> </TH>";
        string FindEPS = "(EPS)</SPAN></STRONG> <I>- Historical</I>";
        string FindCash = "Cash And Cash Equivalents At End</STRONG> </TH>";
        string FindMargin = "(Net Earnings/Revenue) </TH>";
        string FindDebtToEquity = "Debt To Equity</SPAN></STRONG><BR> ((Long Term Debt + Short Term Debt)";
        string FindDateUpdate = "Full Year<BR>";

        Queue<string> stockAddressQueue = new Queue<string>();

        Regex regex = new Regex(@"^-?\d+(?:\.\d+)?");

        //string contents = "";
        string tempString = "";
        string tempString2 = "";

        DataSet ds = new DataSet("New_DataSet");
        DataTable dt = new DataTable("New_DataTable");

        int secCounter = 0;


        Dictionary<string, StockInfo> StockList = new Dictionary<string, StockInfo>();

        public Form1()
        {
            InitializeComponent();

            //stockAddressQueue.Enqueue("http://www.shareinvestor.com/fundamental/financials.html?counter=5DA.SI&period=fy&cols=10");
            //stockAddressQueue.Enqueue("http://www.shareinvestor.com/fundamental/financials.html?counter=568.SI&period=fy&cols=10");
            //stockAddressQueue.Enqueue("http://www.shareinvestor.com/fundamental/financials.html?counter=U96.SI&period=fy&cols=10");
            //stockAddressQueue.Enqueue("http://www.shareinvestor.com/fundamental/financials.html?counter=U11.SI&period=fy&cols=10");
            //stockAddressQueue.Enqueue("http://www.shareinvestor.com/fundamental/financials.html?counter=S53.SI&period=fy&cols=10");

            LogTextbox.Text += "Loading...\n";

            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.ScrollBarsEnabled = false;
            webBrowser1.Navigate("http://www.shareinvestor.com/");

        }

        private void calculation()
        {
            LogTextbox.Text += "Doing Calculation... \n";
            LogTextbox.SelectionStart = LogTextbox.Text.Length;
            LogTextbox.ScrollToCaret();

            StudyResult studyResult = new StudyResult(StockList);
            studyResult.Show();

            LogTextbox.Text += "Calculation Completed! \n";
            LogTextbox.SelectionStart = LogTextbox.Text.Length;
            LogTextbox.ScrollToCaret();
        }

       

        private bool extractInfo(string contents)
        {
            StockInfo stockInfo = new StockInfo();
            double tempDouble = 0;
            bool parseSuccess = false;

            stockInfo.symbol = extractingSymbol;
            LogTextbox.Text += "Extracting: " + extractingSymbol + "\n";
            LogTextbox.SelectionStart = LogTextbox.Text.Length;
            LogTextbox.ScrollToCaret();

            //Get stockName
            tempString = getBetween2(contents, "<TITLE>", "</TITLE>");
            string[] tempName = tempString.Split('-');
            stockInfo.stockName = Regex.Match(tempName[0], @"[0-9a-zA-Z\s^.]+").Value;
            stockInfo.stockName = stockInfo.stockName.Replace(" ", "");

            //Get lastPrice
            tempString = getBetween2(contents, "Last (SGD):", "</TD>");
            tempString = getBetween2(tempString, "<STRONG>", "</STRONG>");

            parseSuccess = double.TryParse(tempString, NumberStyles.Any, CultureInfo.InvariantCulture, out tempDouble);
            if (parseSuccess)
                stockInfo.lastPrice = tempDouble;
            else
            {
                secCounter = 0;
                return false;
            }

            //Get dateUpdate
            tempString = getBetween2(contents, FindDateUpdate, "</TH>");
            stockInfo.dateUpdate = tempString;

            //Get netEarnings 
            tempString = getBetween(contents, FindNetEarnings, "</TR>");

            for (; ; )
            {
                tempString2 = getBetween2(tempString, "<TD>", "</TD>");

                if (tempString2 == "EMPTY")
                    break;

                if (tempString2.Length < 15)
                {
                    parseSuccess = double.TryParse(tempString2, NumberStyles.Any, CultureInfo.InvariantCulture, out tempDouble);
                    if (parseSuccess)
                        stockInfo.netEarnings.Add(tempDouble);
                    else
                        stockInfo.netEarnings.Add(-99999999);
                }
                tempString = trimFront(tempString, tempString2);
            }

            //Get revenue
            tempString = getBetween(contents, FindRevenue, "</TR>");

            for (; ; )
            {
                tempString2 = getBetween2(tempString, "<TD>", "</TD>");

                if (tempString2 == "EMPTY")
                    break;

                if (tempString2.Length < 15)
                {
                    parseSuccess = double.TryParse(tempString2, NumberStyles.Any, CultureInfo.InvariantCulture, out tempDouble);
                    if (parseSuccess)
                        stockInfo.revenue.Add(tempDouble);
                    else
                        stockInfo.revenue.Add(-99999999);
                }
                tempString = trimFront(tempString, tempString2);
            }

            //Get shareHolderEquity
            tempString = getBetween(contents, FindShareHolderEquity, "</TR>");

            for (; ; )
            {
                tempString2 = getBetween2(tempString, "<TD>", "</TD>");

                if (tempString2 == "EMPTY")
                    break;

                if (tempString2.Length < 15)
                {
                    parseSuccess = double.TryParse(tempString2, NumberStyles.Any, CultureInfo.InvariantCulture, out tempDouble);
                    if (parseSuccess)
                        stockInfo.shareHolderEquity.Add(tempDouble);
                    else
                        stockInfo.shareHolderEquity.Add(0);
                }
                tempString = trimFront(tempString, tempString2);
            }

            //Get ltLiabilities
            tempString = getBetween(contents, FindLTLiabilities, "</TR>");

            for (; ; )
            {
                tempString2 = getBetween2(tempString, "<TD>", "</TD>");

                if (tempString2 == "EMPTY")
                    break;

                if (tempString2.Length < 15)
                {
                    parseSuccess = double.TryParse(tempString2, NumberStyles.Any, CultureInfo.InvariantCulture, out tempDouble);
                    if (parseSuccess)
                        stockInfo.ltLiabilities.Add(tempDouble);
                    else
                        stockInfo.ltLiabilities.Add(0);
                }
                tempString = trimFront(tempString, tempString2);
            }

            //Get eps
            tempString = getBetween(contents, FindEPS, "</TR>");

            for (; ; )
            {
                tempString2 = getBetween2(tempString, "<TD>", "</TD>");

                if (tempString2 == "EMPTY")
                    break;

                if (tempString2.Length < 15)
                {
                    parseSuccess = double.TryParse(tempString2, NumberStyles.Any, CultureInfo.InvariantCulture, out tempDouble);
                    if (parseSuccess)
                        stockInfo.eps.Add(tempDouble);
                    else
                        stockInfo.eps.Add(-99999999);
                }
                tempString = trimFront(tempString, tempString2);
            }

            //Get cash
            tempString = getBetween(contents, FindCash, "</TR>");

            for (; ; )
            {
                tempString2 = getBetween2(tempString, "<TD>", "</TD>");

                if (tempString2 == "EMPTY")
                    break;

                if (tempString2.Length < 15)
                {
                    parseSuccess = double.TryParse(tempString2, NumberStyles.Any, CultureInfo.InvariantCulture, out tempDouble);
                    if (parseSuccess)
                        stockInfo.cash.Add(tempDouble);
                    else
                        stockInfo.cash.Add(-99999999);
                }
                tempString = trimFront(tempString, tempString2);
            }

            //Get cash
            tempString = getBetween(contents, FindMargin, "</TR>");

            for (; ; )
            {
                tempString2 = getBetween2(tempString, "<TD>", "</TD>");

                if (tempString2 == "EMPTY")
                    break;

                if (tempString2.Length < 15)
                {
                    parseSuccess = double.TryParse(tempString2, NumberStyles.Any, CultureInfo.InvariantCulture, out tempDouble);
                    if (parseSuccess)
                        stockInfo.margin.Add(tempDouble);
                    else
                        stockInfo.margin.Add(-99999999);
                }
                tempString = trimFront(tempString, tempString2);
            }

            //Get debtToEquity
            tempString = getBetween(contents, FindDebtToEquity, "</TR>");

            for (; ; )
            {
                tempString2 = getBetween2(tempString, "<TD>", "</TD>");

                if (tempString2 == "EMPTY")
                    break;

                if (tempString2.Length < 15)
                {
                    parseSuccess = double.TryParse(tempString2, NumberStyles.Any, CultureInfo.InvariantCulture, out tempDouble);
                    if (parseSuccess)
                        stockInfo.debtToEquity = tempDouble;
                    else
                        stockInfo.debtToEquity = -99999999;

                    break;
                }
                tempString = trimFront(tempString, tempString2);
            }

            if (!StockList.ContainsKey(extractingSymbol))
                StockList.Add(extractingSymbol, stockInfo);
            else
                StockList[extractingSymbol] = stockInfo;

            ReadStockFromStockQueue();
            return true;
        }



        private void Start_Click(object sender, EventArgs e)
        {
            if (stockAddressQueue.Count == 0)
            {
                StringReader strReader = new StringReader(stockTextbox.Text);
                string str;

                for (; ; )
                {
                    str = strReader.ReadLine();
                    if (str != null && str != "")
                        stockAddressQueue.Enqueue(str);
                    else
                        break;
                }

                totalStockNum = stockAddressQueue.Count;
            }

            ReadStockFromStockQueue();
        }

        private void ReadStockFromStockQueue()
        {
            if (stockAddressQueue.Count > 0)
            {
                extractingSymbol = stockAddressQueue.Dequeue();
                webBrowser1.Navigate("http://www.shareinvestor.com/fundamental/financials.html?counter=" + extractingSymbol + ".SI&period=fy&cols=10");
                startExtract = true;
            }
            else
                calculation();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //label1.Text = "OK";
            secCounter = 0;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (secCounter > 2)
            {
                if (startExtract)
                {
                    webBrowser1.Document.Window.ScrollTo(30, 470);
                    extractSuccess = extractInfo(webBrowser1.DocumentText);

                    if (extractSuccess)
                    {
                        extractSuccess = false;
                        timer1.Enabled = false;
                        retries = 0;

                        currStockNum++;
                        LogTextbox.Text += "Extract Success! (" + currStockNum + "/" + totalStockNum + ")\n";
                        LogTextbox.SelectionStart = LogTextbox.Text.Length;
                        LogTextbox.ScrollToCaret();

                        
                    }
                    else
                    {
                        if (retries > 3)
                        {
                            currStockNum++;
                            errorStockTextBox.Text += extractingSymbol + "\n";
                            ReadStockFromStockQueue();
                            timer1.Enabled = false;
                            retries = 0;
                        }
                        else
                        {
                            retries++;
                            LogTextbox.Text += "Extract Fail. Retry #: " + retries + "\n";
                            LogTextbox.SelectionStart = LogTextbox.Text.Length;
                            LogTextbox.ScrollToCaret();
                        }

                    }
                }
                else
                {
                    if (webBrowser1.DocumentText.Contains("Welcome,"))
                    {
                        StartButton.Enabled = true;
                        LogTextbox.Text = "Login Success!\n\nPress READ to read Stock List from file.\nPress START to extract data of stocks in Stock List.\n";
                        LogTextbox.SelectionStart = LogTextbox.Text.Length;
                        LogTextbox.ScrollToCaret();
                    }
                    else
                    {
                        if (isFirstLoad)
                        {
                            isFirstLoad = false;
                            LogTextbox.Text = "Welcome!\n";
                        }
                        LogTextbox.Text += "Please log into Share Investor!\n";
                        LogTextbox.SelectionStart = LogTextbox.Text.Length;
                        LogTextbox.ScrollToCaret();
                    }

                    webBrowser1.Document.Window.ScrollTo(3000, 0);
                    timer1.Enabled = false;
                }

                secCounter = 0;
            }
            else
                secCounter++;
        }

        private void ReadInstFromFile(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(@filePath))
                {
                    return;
                }

                string[] inst = System.IO.File.ReadAllLines(@filePath);

                foreach (string str in inst)
                {
                    stockTextbox.Text += str + "\n";
                    
                }
            }
            catch { MessageBox.Show("Error reading file!", "Error"); }
        }

        private void ReadInstFromFile()
        {
            try
            {
                if (!System.IO.File.Exists(@".\DefaultStocks.txt"))
                {
                    return;
                }

                string[] inst = System.IO.File.ReadAllLines(@".\DefaultStocks.txt");

                foreach (string str in inst)
                {
                    stockTextbox.Text += str + "\n";
                    stockAddressQueue.Enqueue("http://www.shareinvestor.com/fundamental/financials.html?counter=" + str + ".SI&period=fy&cols=10");
                }
            }
            catch { MessageBox.Show("Error reading file!", "Error"); }
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0);
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "EMPTY";
            }
        }

        public static string getBetween2(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "EMPTY";
            }
        }

        public static string trimFront(string strSource, string strStart)
        {
            int Start;
            if (strSource.Contains(strStart))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                return strSource.Substring(Start, strSource.Length - Start);
            }
            else
            {
                return strSource;
            }
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = @".\";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    ReadInstFromFile(file);
                }
                catch (IOException)
                {
                }
            }
            
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = @".\";
            
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK) // Test result.
            {
                string file = saveFileDialog1.FileName;
                try
                {
                    WriteStockToFile(file);
                }
                catch (IOException)
                {
                }
            }
            
        }

        private void WriteStockToFile(string filePath)
        {
            if (!System.IO.File.Exists(@filePath))
            {
                var myFile = System.IO.File.Create(@filePath);
                myFile.Close();
            }

            System.IO.File.WriteAllText(@filePath, stockTextbox.Text, Encoding.UTF8);
        }
    }


    public class StockInfo
    {
        public string symbol = "";
        public string stockName = "";
        public double lastPrice = 0;
        public string dateUpdate = "";
        public List<double> netEarnings = new List<double>();
        public List<double> revenue = new List<double>();
        public List<double> shareHolderEquity = new List<double>();
        public List<double> ltLiabilities = new List<double>();
        public List<double> eps = new List<double>();
        public List<double> cash = new List<double>();
        public List<double> margin = new List<double>();
        public List<double> roc = new List<double>();
        public double debtToEquity = 0;


        public double[] avgRoc = new double[4];
        public double[] equityGrowth = new double[4];
        public double[] epsGrowth = new double[4];
        public double[] revenueGrowth = new double[4];
        public double[] cashGrowth = new double[4];

        public double rule1Growth = 0;
        public double rule1PE = 0;
        public double epsIn5Yrs = 0;
        public double futurePriceIn5yrs = 0;
        public double stickerPrice = 0;
        public double priceToPurchase = 0;
        public double priceToPurchaseOverLast = 0;

        public int score = 0;

    }
}


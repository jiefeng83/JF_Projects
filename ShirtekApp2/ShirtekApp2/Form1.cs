using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Threading;

namespace ShirtekApp2
{
    public partial class Form1 : Form
    {
        List<string> fileList = new List<string>();
        List<WorkOrderData> woDataList = new List<WorkOrderData>();

        string fileName = @"C:\Users\Jiefeng\Desktop\data\McD 10 OCT  WO 2001x.xls";

        public Form1()
        {
            InitializeComponent();

            statusStrip1.Text = "Select folder and press the Read button.";
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                pathTextBox.Text = folderBrowserDialog1.SelectedPath + @"\";
            }
        }

        private void invoiceTotalButton_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;
            readFiles();
            invoiceFormat();
            statusStrip1.Text = woDataList.Count + " files read. Data copied to clipboard. Paste directly to Excel ;)";
        }

        private void doTotalButton_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;
            readFiles();
            doFormat();
            statusStrip1.Text = woDataList.Count + " files read. Data copied to clipboard. Paste directly to Excel ;)";
        }

        void readFiles()
        {
            fileList.Clear();
            woDataList.Clear();
            dataGridView1.Rows.Clear();

            if (!Directory.Exists(@pathTextBox.Text))
            {
                statusStrip1.Text = "Directory not found.";
                return;
            }

            foreach (string s in Directory.GetFiles(@pathTextBox.Text, "*.xls").Select(Path.GetFileName))
            {
                fileList.Add(s);
            }

            int count = 0;
            foreach (string fileName in fileList)
            {
                count++;
                statusStrip1.Text = "Reading " + count + "/" + fileList.Count;

                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source='" + @pathTextBox.Text + fileName + "';" + "Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1;\"";
                //var connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", fileName, "Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1;\"");

                var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                var ds = new DataSet();

                adapter.Fill(ds, "Sheet1");

                DataTable dataTable = ds.Tables["Sheet1"];

                string[] strArray = dataTable.Rows[2][0].ToString().Split(':');
                string invoiceDate = strArray.Length >= 2 ? changeMonth(strArray[1].Replace("-", "")) : "";
                string[] strArray2 = fileName.Replace("  ", " ").Replace(".xls", "").Split(' ');
                string spNumber = strArray2.Length >= 6 ? strArray2[5] : "";

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i][3] != null && dataTable.Rows[i][3].ToString() != "" && !dataTable.Rows[i][3].ToString().Contains("ACCOUNT"))
                    {

                        string a = dataTable.Rows[i][3].ToString();
                        WorkOrderData woData = new WorkOrderData();

                        woData.date = (dataTable.Rows[i][2] != null) ? dataTable.Rows[i][2].ToString() : "";
                        string[] strArray3 = woData.date.Split(' ');

                        if (strArray3.Length >= 3)
                        {
                            woData.date_month = strArray3[1];
                            woData.date_day = strArray3[0];
                        }
                        else
                        {
                            woData.date_month = "";
                            woData.date_day = "";
                        }

                        woData.storeCode1 = (dataTable.Rows[i][4] != null) ? dataTable.Rows[i][4].ToString() : "";
                        woData.storeCode2 = (dataTable.Rows[i][5] != null) ? dataTable.Rows[i][5].ToString() : "";
                        woData.doNumber = (dataTable.Rows[i][13] != null) ? dataTable.Rows[i][13].ToString() : "";
                        woData.woNumber = (dataTable.Rows[i][6] != null) ? dataTable.Rows[i][6].ToString().Substring(1, 6) : "";
                        woData.netAmount = (dataTable.Rows[i][7] != null) ? dataTable.Rows[i][7].ToString() : "";
                        woData.invoiceDate = invoiceDate;
                        woData.invoiceNo = (dataTable.Rows[i][1] != null) ? dataTable.Rows[i][1].ToString() : "";
                        woData.fileName = fileName.Replace(".xls", "");
                        woData.spNumber = spNumber;
                        woDataList.Add(woData);
                    }
                }
            }
        }

        void doFormat()
        {
            List<WorkOrderData> woDataListSorted = woDataList.OrderBy(o => o.date_month).ThenBy(o=> o.doNumber).ToList();

            dataGridView2.SuspendLayout();
            dataGridView2.Rows.Add(woDataList.Count + (woDataList.Count * 2 / 25) + 5);

            int rowIndex = 0;

            dataGridView2.Rows[rowIndex].Cells[0].Value = "Date";
            dataGridView2.Rows[rowIndex].Cells[1].Value = "DO Number";
            dataGridView2.Rows[rowIndex].Cells[2].Value = "Total Price";
            dataGridView2.Rows[rowIndex].Cells[3].Value = "SP Number";
            dataGridView2.Rows[rowIndex].Cells[4].Value = "Invoice Date";
            dataGridView2.Rows[rowIndex].Cells[5].Value = "Invoice No";
            dataGridView2.Rows[rowIndex].Cells[6].Value = "Date Paid";
            dataGridView2.Rows[rowIndex].Cells[7].Value = "Amount Paid";
            rowIndex++;

            string lastMth = woDataListSorted[0].date_month;

            foreach (WorkOrderData wod in woDataListSorted)
            {
                if (wod.date_month != lastMth)
                {
                    lastMth = wod.date_month;

                    dataGridView2.Rows[rowIndex].Cells[0].Value = "";
                    dataGridView2.Rows[rowIndex].Cells[1].Value = "";
                    dataGridView2.Rows[rowIndex].Cells[2].Value = "";
                    dataGridView2.Rows[rowIndex].Cells[3].Value = "";
                    dataGridView2.Rows[rowIndex].Cells[4].Value = "";
                    dataGridView2.Rows[rowIndex].Cells[5].Value = "";
                    dataGridView2.Rows[rowIndex].Cells[6].Value = "";
                    dataGridView2.Rows[rowIndex].Cells[7].Value = "";
                    rowIndex++;

                    dataGridView2.Rows[rowIndex].Cells[0].Value = "Date";
                    dataGridView2.Rows[rowIndex].Cells[1].Value = "DO Number";
                    dataGridView2.Rows[rowIndex].Cells[2].Value = "Total Price";
                    dataGridView2.Rows[rowIndex].Cells[3].Value = "SP Number";
                    dataGridView2.Rows[rowIndex].Cells[4].Value = "Invoice Date";
                    dataGridView2.Rows[rowIndex].Cells[5].Value = "Invoice No";
                    dataGridView2.Rows[rowIndex].Cells[6].Value = "Date Paid";
                    dataGridView2.Rows[rowIndex].Cells[7].Value = "Amount Paid";
                    rowIndex++;

                }

                dataGridView2.Rows[rowIndex].Cells[0].Value = wod.date;
                dataGridView2.Rows[rowIndex].Cells[1].Value = wod.doNumber;
                dataGridView2.Rows[rowIndex].Cells[2].Value = wod.netAmount;
                dataGridView2.Rows[rowIndex].Cells[3].Value = "S/P " + wod.spNumber;
                dataGridView2.Rows[rowIndex].Cells[4].Value = wod.invoiceDate;
                dataGridView2.Rows[rowIndex].Cells[5].Value = wod.invoiceNo;
                dataGridView2.Rows[rowIndex].Cells[6].Value = "";
                dataGridView2.Rows[rowIndex].Cells[7].Value = "";

                rowIndex++;
            }

            dataGridView2.ResumeLayout();
        }

        void invoiceFormat()
        {
            List<WorkOrderData> woDataListSorted = woDataList.OrderBy(o => o.invoiceNo).ToList();

            dataGridView1.SuspendLayout();
            dataGridView1.Rows.Add(woDataList.Count + (woDataList.Count * 2 / 25) + 5);

            int rowIndex = 0;
            int number = 0;

            dataGridView1.Rows[rowIndex].Cells[0].Value = "DO DATE";
            dataGridView1.Rows[rowIndex].Cells[1].Value = "DELIVERY ORDER";
            dataGridView1.Rows[rowIndex].Cells[2].Value = "OUTLET";
            dataGridView1.Rows[rowIndex].Cells[3].Value = "TOTAL COST";
            dataGridView1.Rows[rowIndex].Cells[4].Value = "WORK ORDER";
            dataGridView1.Rows[rowIndex].Cells[5].Value = "INVOICE";
            dataGridView1.Rows[rowIndex].Cells[6].Value = "INVOICE DATE";
            dataGridView1.Rows[rowIndex].Cells[7].Value = "PAY DATE";
            dataGridView1.Rows[rowIndex].Cells[8].Value = "BANK";
            dataGridView1.Rows[rowIndex].Cells[9].Value = "CHEQUE NO";

            rowIndex++;

            foreach (WorkOrderData wod in woDataListSorted)
            {
                dataGridView1.Rows[rowIndex].Cells[0].Value = wod.date;
                dataGridView1.Rows[rowIndex].Cells[1].Value = wod.doNumber;
                dataGridView1.Rows[rowIndex].Cells[2].Value = "McDonald's Restaurant";
                dataGridView1.Rows[rowIndex].Cells[3].Value = wod.netAmount;
                dataGridView1.Rows[rowIndex].Cells[4].Value = wod.woNumber;
                dataGridView1.Rows[rowIndex].Cells[5].Value = wod.invoiceNo;
                dataGridView1.Rows[rowIndex].Cells[6].Value = wod.invoiceDate;
                dataGridView1.Rows[rowIndex].Cells[7].Value = "";
                dataGridView1.Rows[rowIndex].Cells[8].Value = "";
                dataGridView1.Rows[rowIndex].Cells[9].Value = "";

                rowIndex++;
                number++;

                if (number % 25 == 0)
                {
                    dataGridView1.Rows[rowIndex].Cells[0].Value = "";
                    dataGridView1.Rows[rowIndex].Cells[1].Value = "";
                    dataGridView1.Rows[rowIndex].Cells[2].Value = "";
                    dataGridView1.Rows[rowIndex].Cells[3].Value = "";
                    dataGridView1.Rows[rowIndex].Cells[4].Value = "";
                    dataGridView1.Rows[rowIndex].Cells[5].Value = "";
                    dataGridView1.Rows[rowIndex].Cells[6].Value = "";
                    dataGridView1.Rows[rowIndex].Cells[7].Value = "";
                    dataGridView1.Rows[rowIndex].Cells[8].Value = "";
                    dataGridView1.Rows[rowIndex].Cells[9].Value = "";

                    rowIndex++;

                    dataGridView1.Rows[rowIndex].Cells[0].Value = "DO DATE";
                    dataGridView1.Rows[rowIndex].Cells[1].Value = "DELIVERY ORDER";
                    dataGridView1.Rows[rowIndex].Cells[2].Value = "OUTLET";
                    dataGridView1.Rows[rowIndex].Cells[3].Value = "TOTAL COST";
                    dataGridView1.Rows[rowIndex].Cells[4].Value = "WORK ORDER";
                    dataGridView1.Rows[rowIndex].Cells[5].Value = "INVOICE";
                    dataGridView1.Rows[rowIndex].Cells[6].Value = "INVOICE DATE";
                    dataGridView1.Rows[rowIndex].Cells[7].Value = "PAY DATE";
                    dataGridView1.Rows[rowIndex].Cells[8].Value = "BANK";
                    dataGridView1.Rows[rowIndex].Cells[9].Value = "CHEQUE NO";

                    rowIndex++;
                }

            }

            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

            dataGridView1.ResumeLayout();
        }

        string changeMonth(string str)
        {
            return str.Replace("MAY", "05").Replace("JUNE", "06").Replace("JULY", "07").Replace("AUG", "08").Replace("SEP", "09").Replace("OCT", "10").Replace("NOV", "11").Replace("DEC", "12"); 
        }

        private void testing(DataTable dataTable)
        {
            int x = 0, y = 0;
            string temp = "";
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (dataTable.Rows[i][j] != null)
                    {
                        temp = dataTable.Rows[i][j].ToString();

                        if (temp.Contains("12345"))
                        {
                            x = i; y = j;
                            statusStrip1.Text = x + "   " + y;
                        }
                    }
                }
            }
        }

 
    }

    public class WorkOrderData
    {
        public string date;
        public string date_month;
        public string date_day;
        public string storeCode1;
        public string storeCode2;
        public string netAmount;
        public string doNumber;
        public string woNumber;
        public string fileName;
        public string invoiceDate;
        public string invoiceNo;
        public string spNumber;
    }
}

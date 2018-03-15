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
using System.Text.RegularExpressions;

namespace ShirtekApp1
{
    public partial class Form1 : Form
    {
        List<string> fileList = new List<string>();
        List<WorkOrderData> woDataList = new List<WorkOrderData>();

        string fileName = @"C:\Users\Jiefeng\Desktop\data\McD 10 OCT  WO 2001x.xls";

        private void Form1_Load(object sender, EventArgs e)
        {
			pathTextBox.Text = Properties.Settings.Default.FilePath;
			
            setDataGridCol(dataGridView1, 0, "NO.");
            setDataGridCol(dataGridView1, 1, "INVOICE NO.");
            setDataGridCol(dataGridView1, 2, "INVOICE DATE", 1);
            setDataGridCol(dataGridView1, 3, "WO NO.");
            setDataGridCol(dataGridView1, 4, "ACCT NO.");
            setDataGridCol(dataGridView1, 5, "STR ABB");
            setDataGridCol(dataGridView1, 6, "STR NO.");
            setDataGridCol(dataGridView1, 7, "DESCRIPTION", 1);
            setDataGridCol(dataGridView1, 8, "NET AMT");
            setDataGridCol(dataGridView1, 9, "TAX AMT");
            setDataGridCol(dataGridView1, 10, "TOTAL AMT");
            setDataGridCol(dataGridView1, 11, "GST RATE");
            setDataGridCol(dataGridView1, 12, "JOB STATUS", 1);
            setDataGridCol(dataGridView1, 13, "REMARK", 1);
            setDataGridCol(dataGridView1, 14, "WO DATE");
            setDataGridCol(dataGridView1, 15, "DO NO.");
            setDataGridCol(dataGridView1, 16, "FILENAME");


            setDataGridCol(dataGridView2, 0, "NO.");
            setDataGridCol(dataGridView2, 1, "WORK ORDER");
            setDataGridCol(dataGridView2, 2, "SERVICE ORDER");
            setDataGridCol(dataGridView2, 3, "DATE");
            setDataGridCol(dataGridView2, 4, "STR ABB");
            setDataGridCol(dataGridView2, 5, "STR NO.");
            setDataGridCol(dataGridView2, 6, "MODEL");
            setDataGridCol(dataGridView2, 7, "LOCATION"); 
            setDataGridCol(dataGridView2, 8, "LABOUR CHARGE");
            setDataGridCol(dataGridView2, 9, "PART CHARGE");
            setDataGridCol(dataGridView2, 10, "TOTAL");
            setDataGridCol(dataGridView2, 11, "CAUSE");
            setDataGridCol(dataGridView2, 12, "ACTION");
            setDataGridCol(dataGridView2, 13, "PARTS");
            setDataGridCol(dataGridView2, 14, "FRESER");
            setDataGridCol(dataGridView2, 15, "TOWER");
            setDataGridCol(dataGridView2, 16, "BACKROOM");
            setDataGridCol(dataGridView2, 17, "ORANGE JUICE");
            setDataGridCol(dataGridView2, 18, "OTHER");

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

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
                Properties.Settings.Default.FilePath = pathTextBox.Text;
                Properties.Settings.Default.Save();
            }
        }

        KeywordType convertToKT(KeywordType kt, ref string key)
        {
            if (key.ContainsCI("CAUSE:") || key.ContainsCI("CAUSES:"))
            {
                key = "";
                return KeywordType.CAUSE;
            }
            else if (key.ContainsCI("ACTION:") || key.ContainsCI("ACTIONS:"))
            {
                key = "";
                return KeywordType.ACTION;
            }
            else if (key.ContainsCI("PART:") || key.ContainsCI("PARTS:"))
            {
                key = "";
                return KeywordType.PARTS;
            }
            else
                return kt;
        }

        private void readButton_Click(object sender, EventArgs e)
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
                KeywordType keywordType = KeywordType.UNKNOWN;

                if (dataTable.Rows.Count >= 55 && dataTable.Columns.Count >= 13)
                {
                    string storeCode = (dataTable.Rows[16][5] != null) ? dataTable.Rows[16][5].ToString().Replace(" ", "") : "#";
                    string[] array = storeCode.Split('#');

                    WorkOrderData woData = new WorkOrderData();
                    woData.date = (dataTable.Rows[17][5] != null) ? dataTable.Rows[17][5].ToString() : "";
                    woData.storeCode1 = (array.Length > 0) ? array[0] : "";
                    woData.storeCode2 = (array.Length > 1) ? array[1] : "";
                    woData.doNumber = (dataTable.Rows[17][8] != null) ? dataTable.Rows[17][8].ToString() : "";
                    woData.woNumber = (dataTable.Rows[16][8] != null) ? "B" + dataTable.Rows[16][8].ToString() + dataTable.Rows[50][1].ToString(): "";
                    woData.netAmount = (dataTable.Rows[54][12] != null) ? dataTable.Rows[54][12].ToString() : "";
                    woData.fileName = fileName;
                    woData.invNumber = (dataTable.Rows[5][10] != null) ? dataTable.Rows[5][10].ToString() : "";
                    woData.labourCharge = (dataTable.Rows[19][12] != null) ? dataTable.Rows[19][12].ToString() : "";

                    for (int i = 19; i<dataTable.Rows.Count; i++)
                    {
                        string extract = (dataTable.Rows[i][4] != null) ? dataTable.Rows[i][4].ToString() : "";
                        keywordType = convertToKT(keywordType, ref extract);
                        if (extract == "") continue;

                        switch(keywordType)
                        {
                            case KeywordType.UNKNOWN:
                                if (extract.ContainsCI("Freser")) woData.isFreser = "1";
                                if (extract.ContainsCI("Backroom")) woData.isBackroom = "1";
                                if (extract.ContainsCI("Tower")) woData.isTower = "1";
                                if (extract.ContainsCI("Orange")) woData.isOJ = "1";
                                if (extract.ContainsCI("Other")) woData.isOther = "1";
                                break;
                            case KeywordType.CAUSE:
                                if (woData.cause != "") extract = "\n" + extract;
                                woData.cause += extract;
                                break;
                            case KeywordType.ACTION:
                                if (woData.action != "") extract = "\n" + extract;
                                woData.action += extract;
                                break;
                            case KeywordType.PARTS:
                                string partQty = (dataTable.Rows[i][2] != null) ? dataTable.Rows[i][2].ToString() : "";
                                extract = partQty + ": " + extract;
                                if (woData.parts != "") extract = "\n" + extract;
                                woData.parts += extract;
                                string partCharges = (dataTable.Rows[i][12] != null) ? dataTable.Rows[i][12].ToString() : "";
                                if (woData.partCharge != "") partCharges = "\n" + partCharges;
                                woData.partCharge += partCharges;
                                break;
                        }
                    }

                    string result = Regex.Replace(woData.fileName, @"[^\d]", "");
                    long.TryParse(result, out woData.fileNumber);

                    woDataList.Add(woData);
                }
            }

            List<WorkOrderData> woDataListSorted = woDataList.OrderBy(o => o.fileNumber).ToList();

            dataGridView1.SuspendLayout();
            dataGridView1.Rows.Add(woDataList.Count);
            
            int rowIndex = 0;
            foreach (WorkOrderData wod in woDataListSorted)
            {
                dataGridView1.Rows[rowIndex].Cells[0].Value = rowIndex + 1;
                dataGridView1.Rows[rowIndex].Cells[1].Value = wod.invNumber;
                dataGridView1.Rows[rowIndex].Cells[2].Value = ""; //invoice date
                dataGridView1.Rows[rowIndex].Cells[3].Value = wod.woNumber;
                dataGridView1.Rows[rowIndex].Cells[4].Value = "7415-044";
                dataGridView1.Rows[rowIndex].Cells[5].Value = wod.storeCode1;
                dataGridView1.Rows[rowIndex].Cells[6].Value = wod.storeCode2;
                dataGridView1.Rows[rowIndex].Cells[7].Value = "";
                dataGridView1.Rows[rowIndex].Cells[8].Value = wod.netAmount;
                dataGridView1.Rows[rowIndex].Cells[9].Value = "$0.00";
                dataGridView1.Rows[rowIndex].Cells[10].Value = wod.netAmount; //total amt
                dataGridView1.Rows[rowIndex].Cells[11].Value = "0%"; //GST
                dataGridView1.Rows[rowIndex].Cells[12].Value = "";
                dataGridView1.Rows[rowIndex].Cells[13].Value = "";
                dataGridView1.Rows[rowIndex].Cells[14].Value = wod.date;
                dataGridView1.Rows[rowIndex].Cells[15].Value = wod.doNumber;
                dataGridView1.Rows[rowIndex].Cells[16].Value = wod.fileName;
                rowIndex++;
            }

            dataGridView2.SuspendLayout();
            dataGridView2.Rows.Add(woDataList.Count);

            rowIndex = 0;
            foreach (WorkOrderData wod in woDataListSorted)
            {
                dataGridView2.Rows[rowIndex].Cells[0].Value = rowIndex + 1;
                dataGridView2.Rows[rowIndex].Cells[1].Value = wod.woNumber;
                dataGridView2.Rows[rowIndex].Cells[2].Value = wod.doNumber;
                dataGridView2.Rows[rowIndex].Cells[3].Value = wod.date;
                dataGridView2.Rows[rowIndex].Cells[4].Value = wod.storeCode1;
                dataGridView2.Rows[rowIndex].Cells[5].Value = wod.storeCode2;

                dataGridView2.Rows[rowIndex].Cells[6].Value = "";
                dataGridView2.Rows[rowIndex].Cells[7].Value = "";

                dataGridView2.Rows[rowIndex].Cells[8].Value = wod.labourCharge;
                dataGridView2.Rows[rowIndex].Cells[9].Value = wod.partCharge;
                dataGridView2.Rows[rowIndex].Cells[10].Value = wod.netAmount;

                dataGridView2.Rows[rowIndex].Cells[11].Value = wod.cause;
                dataGridView2.Rows[rowIndex].Cells[12].Value = wod.action;
                dataGridView2.Rows[rowIndex].Cells[13].Value = wod.parts;

                dataGridView2.Rows[rowIndex].Cells[14].Value = wod.isFreser;
                dataGridView2.Rows[rowIndex].Cells[15].Value = wod.isTower;
                dataGridView2.Rows[rowIndex].Cells[16].Value = wod.isBackroom;
                dataGridView2.Rows[rowIndex].Cells[17].Value = wod.isOJ;
                dataGridView2.Rows[rowIndex].Cells[18].Value = wod.isOther;
                
                
                rowIndex++;
            }


            copyAll(dataGridView1, 1);
            statusStrip1.Text = woDataListSorted.Count + " files read. Data copied to clipboard. Paste directly to Excel.";
        }

        void copyAll(DataGridView dgv, int lessCol = 0)
        {
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dgv.ResumeLayout();
            dgv.ClearSelection();
            for (int i = 0; i < dgv.ColumnCount - lessCol; i++)
            {
                for (int r = 0; r < dgv.RowCount; r++)
                    dgv[i, r].Selected = true;
            }

            DataObject dataObj = dgv.GetClipboardContent();
            Clipboard.SetDataObject(dataObj, true);

            statusStrip1.Text = "Data copied to clipboard. Paste directly to Excel.";
        }

        void setDataGridCol(DataGridView dgv, int index, string header, int widthFillWeight = 100)
        {
            dgv.Columns[index].HeaderText = header;
            dgv.Columns[index].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns[index].FillWeight = widthFillWeight;
            dgv.Columns[index].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[index].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        }

        private void copyBut1_Click(object sender, EventArgs e)
        {
            copyAll(dataGridView1, 1);
        }

        private void copyBut2_Click(object sender, EventArgs e)
        {
            copyAll(dataGridView2);
        }
    }

    public static class StringExtensions
    {
        public static bool ContainsCI(this string source, string toCheck)
        {
            return source?.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    public enum KeywordType { UNKNOWN, CAUSE, ACTION, PARTS }

    public class WorkOrderData
    {
        public string date;
        public string storeCode1;
        public string storeCode2;
        public string netAmount;
        public string doNumber;
        public string woNumber;
        public string fileName;
        public string invNumber;
        public long fileNumber = 0;
        public string cause = "";
        public string action = "";
        public string parts = "";
        public string partCharge = "";
        public string labourCharge = "";
        public string isFreser = "";
        public string isTower = "";
        public string isBackroom = "";
        public string isOJ = "";
        public string isOther = "";

    }
}

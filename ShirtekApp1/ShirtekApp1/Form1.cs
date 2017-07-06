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
			
            setDataGridCol(0, "NO.");
            setDataGridCol(1, "INVOICE NO.");
            setDataGridCol(2, "INVOICE DATE", 1);
            setDataGridCol(3, "WO NO.");
            setDataGridCol(4, "ACCT NO.");
            setDataGridCol(5, "STR ABB");
            setDataGridCol(6, "STR NO.");
            setDataGridCol(7, "DESCRIPTION", 1);
            setDataGridCol(8, "NET AMT");
            setDataGridCol(9, "TAX AMT");
            setDataGridCol(10, "TOTAL AMT");
            setDataGridCol(11, "GST RATE");
            setDataGridCol(12, "JOB STATUS", 1);
            setDataGridCol(13, "REMARK", 1);
            setDataGridCol(14, "WO DATE");
            setDataGridCol(15, "DO NO.");
            setDataGridCol(16, "FILENAME");
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

                if (dataTable.Rows.Count >= 55 && dataTable.Columns.Count >= 13)
                {
                    string storeCode = (dataTable.Rows[16][5] != null) ? dataTable.Rows[16][5].ToString().Replace(" ", "") : "#";
                    string[] array = storeCode.Split('#');

                    WorkOrderData woData = new WorkOrderData();
                    woData.date = (dataTable.Rows[17][5] != null) ? dataTable.Rows[17][5].ToString() : "";
                    woData.storeCode1 = (array.Length > 0) ? array[0] : "";
                    woData.storeCode2 = (array.Length > 1) ? array[1] : "";
                    woData.doNumber = (dataTable.Rows[17][8] != null) ? dataTable.Rows[17][8].ToString() : "";
                    woData.woNumber = (dataTable.Rows[17][8] != null) ? "B" + dataTable.Rows[16][8].ToString() + dataTable.Rows[50][1].ToString(): "";
                    woData.netAmount = (dataTable.Rows[54][12] != null) ? dataTable.Rows[54][12].ToString() : "";
                    woData.fileName = fileName.Replace(".xls", "");
                    woData.invNumber = (dataTable.Rows[5][10] != null) ? dataTable.Rows[5][10].ToString() : "";

                    woDataList.Add(woData);
                }
            }

            List<WorkOrderData> woDataListSorted = woDataList.OrderBy(o => o.fileName).ToList();

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

            //dataGridView1.Sort(setDataGridCol(14], ListSortDirection.Ascending);
            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

            dataGridView1.ResumeLayout();
            dataGridView1.ClearSelection();
            for (int i = 0; i < dataGridView1.ColumnCount - 1; i++)
            {
                for (int r = 0; r < dataGridView1.RowCount; r++)
                    dataGridView1[i, r].Selected = true;
            }
            //dataGridView1.SelectAll();

            
            DataObject dataObj = dataGridView1.GetClipboardContent();
            Clipboard.SetDataObject(dataObj, true);

            statusStrip1.Text = woDataListSorted.Count + " files read. Data copied to clipboard. Paste directly to Excel ;)";
        }

        void setDataGridCol(int index, string header, int widthFillWeight = 100)
        {
            dataGridView1.Columns[index].HeaderText = header;
            dataGridView1.Columns[index].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[index].FillWeight = widthFillWeight;
        }

    }

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
    }
}

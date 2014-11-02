namespace StockFundamentalStudy
{
    partial class StudyResult
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StudyResult));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.NameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Symbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvgROA1Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvgROA3Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvgROA5Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvgROA9Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EquityGrowth1Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EquityGrowth3Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EquityGrowth5Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EquityGrowth9Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EPSGrowth1Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EPSGrowth3Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EPSGrowth5Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EPSGrowth9Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RevGrowth1Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RevGrowth3Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RevGrowth5Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RevGrowth9Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CashGrowth1Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CashGrowth3Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CashGrowth5Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CashGrowth9Yr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DebtToEquity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TargetPurchasePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PurchasePriceOverLast = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Score = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateUpdated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameCol,
            this.Symbol,
            this.AvgROA1Yr,
            this.AvgROA3Yr,
            this.AvgROA5Yr,
            this.AvgROA9Yr,
            this.EquityGrowth1Yr,
            this.EquityGrowth3Yr,
            this.EquityGrowth5Yr,
            this.EquityGrowth9Yr,
            this.EPSGrowth1Yr,
            this.EPSGrowth3Yr,
            this.EPSGrowth5Yr,
            this.EPSGrowth9Yr,
            this.RevGrowth1Yr,
            this.RevGrowth3Yr,
            this.RevGrowth5Yr,
            this.RevGrowth9Yr,
            this.CashGrowth1Yr,
            this.CashGrowth3Yr,
            this.CashGrowth5Yr,
            this.CashGrowth9Yr,
            this.DebtToEquity,
            this.TargetPurchasePrice,
            this.LastPrice,
            this.PurchasePriceOverLast,
            this.Score,
            this.DateUpdated});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(1322, 461);
            this.dataGridView1.TabIndex = 4;
            // 
            // NameCol
            // 
            this.NameCol.HeaderText = "Name";
            this.NameCol.Name = "NameCol";
            this.NameCol.ReadOnly = true;
            // 
            // Symbol
            // 
            this.Symbol.HeaderText = "Sym";
            this.Symbol.Name = "Symbol";
            this.Symbol.ReadOnly = true;
            this.Symbol.Width = 40;
            // 
            // AvgROA1Yr
            // 
            this.AvgROA1Yr.HeaderText = "Avg ROA 1Yr";
            this.AvgROA1Yr.Name = "AvgROA1Yr";
            this.AvgROA1Yr.ReadOnly = true;
            this.AvgROA1Yr.Width = 45;
            // 
            // AvgROA3Yr
            // 
            this.AvgROA3Yr.HeaderText = "Avg ROA 3Yr";
            this.AvgROA3Yr.Name = "AvgROA3Yr";
            this.AvgROA3Yr.ReadOnly = true;
            this.AvgROA3Yr.Width = 45;
            // 
            // AvgROA5Yr
            // 
            this.AvgROA5Yr.HeaderText = "Avg ROA 5Yr";
            this.AvgROA5Yr.Name = "AvgROA5Yr";
            this.AvgROA5Yr.ReadOnly = true;
            this.AvgROA5Yr.Width = 45;
            // 
            // AvgROA9Yr
            // 
            this.AvgROA9Yr.HeaderText = "Avg ROA 9Yr";
            this.AvgROA9Yr.Name = "AvgROA9Yr";
            this.AvgROA9Yr.ReadOnly = true;
            this.AvgROA9Yr.Width = 45;
            // 
            // EquityGrowth1Yr
            // 
            this.EquityGrowth1Yr.HeaderText = "Equity Growth % 1Yr";
            this.EquityGrowth1Yr.Name = "EquityGrowth1Yr";
            this.EquityGrowth1Yr.ReadOnly = true;
            this.EquityGrowth1Yr.Width = 45;
            // 
            // EquityGrowth3Yr
            // 
            this.EquityGrowth3Yr.HeaderText = "Equity Growth % 3Yr";
            this.EquityGrowth3Yr.Name = "EquityGrowth3Yr";
            this.EquityGrowth3Yr.ReadOnly = true;
            this.EquityGrowth3Yr.Width = 45;
            // 
            // EquityGrowth5Yr
            // 
            this.EquityGrowth5Yr.HeaderText = "Equity Growth % 5Yr";
            this.EquityGrowth5Yr.Name = "EquityGrowth5Yr";
            this.EquityGrowth5Yr.ReadOnly = true;
            this.EquityGrowth5Yr.Width = 45;
            // 
            // EquityGrowth9Yr
            // 
            this.EquityGrowth9Yr.HeaderText = "Equity Growth % 9Yr";
            this.EquityGrowth9Yr.Name = "EquityGrowth9Yr";
            this.EquityGrowth9Yr.ReadOnly = true;
            this.EquityGrowth9Yr.Width = 45;
            // 
            // EPSGrowth1Yr
            // 
            this.EPSGrowth1Yr.HeaderText = "EPS Growth % 1Yr";
            this.EPSGrowth1Yr.Name = "EPSGrowth1Yr";
            this.EPSGrowth1Yr.ReadOnly = true;
            this.EPSGrowth1Yr.Width = 45;
            // 
            // EPSGrowth3Yr
            // 
            this.EPSGrowth3Yr.HeaderText = "EPS Growth % 3Yr";
            this.EPSGrowth3Yr.Name = "EPSGrowth3Yr";
            this.EPSGrowth3Yr.ReadOnly = true;
            this.EPSGrowth3Yr.Width = 45;
            // 
            // EPSGrowth5Yr
            // 
            this.EPSGrowth5Yr.HeaderText = "EPS Growth % 5Yr";
            this.EPSGrowth5Yr.Name = "EPSGrowth5Yr";
            this.EPSGrowth5Yr.ReadOnly = true;
            this.EPSGrowth5Yr.Width = 45;
            // 
            // EPSGrowth9Yr
            // 
            this.EPSGrowth9Yr.HeaderText = "EPS Growth % 9Yr";
            this.EPSGrowth9Yr.Name = "EPSGrowth9Yr";
            this.EPSGrowth9Yr.ReadOnly = true;
            this.EPSGrowth9Yr.Width = 45;
            // 
            // RevGrowth1Yr
            // 
            this.RevGrowth1Yr.HeaderText = "Rev Growth % 1Yr";
            this.RevGrowth1Yr.Name = "RevGrowth1Yr";
            this.RevGrowth1Yr.ReadOnly = true;
            this.RevGrowth1Yr.Width = 45;
            // 
            // RevGrowth3Yr
            // 
            this.RevGrowth3Yr.HeaderText = "Rev Growth % 3Yr";
            this.RevGrowth3Yr.Name = "RevGrowth3Yr";
            this.RevGrowth3Yr.ReadOnly = true;
            this.RevGrowth3Yr.Width = 45;
            // 
            // RevGrowth5Yr
            // 
            this.RevGrowth5Yr.HeaderText = "Rev Growth % 5Yr";
            this.RevGrowth5Yr.Name = "RevGrowth5Yr";
            this.RevGrowth5Yr.ReadOnly = true;
            this.RevGrowth5Yr.Width = 45;
            // 
            // RevGrowth9Yr
            // 
            this.RevGrowth9Yr.HeaderText = "Rev Growth % 9Yr";
            this.RevGrowth9Yr.Name = "RevGrowth9Yr";
            this.RevGrowth9Yr.ReadOnly = true;
            this.RevGrowth9Yr.Width = 45;
            // 
            // CashGrowth1Yr
            // 
            this.CashGrowth1Yr.HeaderText = "Cash Growth % 1Yr";
            this.CashGrowth1Yr.Name = "CashGrowth1Yr";
            this.CashGrowth1Yr.ReadOnly = true;
            this.CashGrowth1Yr.Width = 45;
            // 
            // CashGrowth3Yr
            // 
            this.CashGrowth3Yr.HeaderText = "Cash Growth % 3Yr";
            this.CashGrowth3Yr.Name = "CashGrowth3Yr";
            this.CashGrowth3Yr.ReadOnly = true;
            this.CashGrowth3Yr.Width = 45;
            // 
            // CashGrowth5Yr
            // 
            this.CashGrowth5Yr.HeaderText = "Cash Growth % 5Yr";
            this.CashGrowth5Yr.Name = "CashGrowth5Yr";
            this.CashGrowth5Yr.ReadOnly = true;
            this.CashGrowth5Yr.Width = 45;
            // 
            // CashGrowth9Yr
            // 
            this.CashGrowth9Yr.HeaderText = "Cash Growth % 9Yr";
            this.CashGrowth9Yr.Name = "CashGrowth9Yr";
            this.CashGrowth9Yr.ReadOnly = true;
            this.CashGrowth9Yr.Width = 45;
            // 
            // DebtToEquity
            // 
            this.DebtToEquity.HeaderText = "Debt to Equity";
            this.DebtToEquity.Name = "DebtToEquity";
            this.DebtToEquity.ReadOnly = true;
            this.DebtToEquity.Width = 45;
            // 
            // TargetPurchasePrice
            // 
            this.TargetPurchasePrice.HeaderText = "Target Buy Price";
            this.TargetPurchasePrice.Name = "TargetPurchasePrice";
            this.TargetPurchasePrice.ReadOnly = true;
            this.TargetPurchasePrice.Width = 45;
            // 
            // LastPrice
            // 
            this.LastPrice.HeaderText = "Last Price";
            this.LastPrice.Name = "LastPrice";
            this.LastPrice.ReadOnly = true;
            this.LastPrice.Width = 45;
            // 
            // PurchasePriceOverLast
            // 
            this.PurchasePriceOverLast.HeaderText = "Buy Price / Last";
            this.PurchasePriceOverLast.Name = "PurchasePriceOverLast";
            this.PurchasePriceOverLast.ReadOnly = true;
            this.PurchasePriceOverLast.Width = 45;
            // 
            // Score
            // 
            this.Score.HeaderText = "Score";
            this.Score.Name = "Score";
            this.Score.ReadOnly = true;
            this.Score.Width = 35;
            // 
            // DateUpdated
            // 
            this.DateUpdated.HeaderText = "Update Date";
            this.DateUpdated.Name = "DateUpdated";
            this.DateUpdated.ReadOnly = true;
            this.DateUpdated.Width = 62;
            // 
            // StudyResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1351, 489);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StudyResult";
            this.Text = "StudyResult";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn Symbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvgROA1Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvgROA3Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvgROA5Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvgROA9Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn EquityGrowth1Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn EquityGrowth3Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn EquityGrowth5Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn EquityGrowth9Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn EPSGrowth1Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn EPSGrowth3Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn EPSGrowth5Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn EPSGrowth9Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn RevGrowth1Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn RevGrowth3Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn RevGrowth5Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn RevGrowth9Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn CashGrowth1Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn CashGrowth3Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn CashGrowth5Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn CashGrowth9Yr;
        private System.Windows.Forms.DataGridViewTextBoxColumn DebtToEquity;
        private System.Windows.Forms.DataGridViewTextBoxColumn TargetPurchasePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn PurchasePriceOverLast;
        private System.Windows.Forms.DataGridViewTextBoxColumn Score;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateUpdated;
    }
}
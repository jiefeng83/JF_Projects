using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockFundamentalStudy
{
    public partial class StudyResult : Form
    {
        public StudyResult(Dictionary<string, StockInfo> StockList)
        {
            InitializeComponent();

            dataGridView1.SuspendLayout();
            foreach (StockInfo si in StockList.Values)
            {
                for (int i = 0; i < si.netEarnings.Count; i++)
                {
                    if (si.netEarnings[i] != -99999999 && i < si.ltLiabilities.Count && i < si.shareHolderEquity.Count)
                    {
                        si.roc.Add(si.netEarnings[i] * 100 / (si.ltLiabilities[i] + si.shareHolderEquity[i]));
                    }
                    else
                    {
                        si.roc.Add(-99999999);
                    }
                }

                si.avgRoc[0] = findAvgRoc(1.0, si.roc);
                si.avgRoc[1] = findAvgRoc(3.0, si.roc);
                si.avgRoc[2] = findAvgRoc(5.0, si.roc);
                si.avgRoc[3] = findAvgRoc(9.0, si.roc);

                si.epsGrowth[0] = findGrowth(1, si.eps);
                si.epsGrowth[1] = findGrowth(3, si.eps);
                si.epsGrowth[2] = findGrowth(5, si.eps);
                si.epsGrowth[3] = findGrowth(9, si.eps);

                si.equityGrowth[0] = findGrowth(1, si.shareHolderEquity);
                si.equityGrowth[1] = findGrowth(3, si.shareHolderEquity);
                si.equityGrowth[2] = findGrowth(5, si.shareHolderEquity);
                si.equityGrowth[3] = findGrowth(9, si.shareHolderEquity);

                si.revenueGrowth[0] = findGrowth(1, si.revenue);
                si.revenueGrowth[1] = findGrowth(3, si.revenue);
                si.revenueGrowth[2] = findGrowth(5, si.revenue);
                si.revenueGrowth[3] = findGrowth(9, si.revenue);

                si.cashGrowth[0] = findGrowth(1, si.cash);
                si.cashGrowth[1] = findGrowth(3, si.cash);
                si.cashGrowth[2] = findGrowth(5, si.cash);
                si.cashGrowth[3] = findGrowth(9, si.cash);

                if (si.equityGrowth[2] != -99999999)
                {
                    si.rule1Growth = si.equityGrowth[2];
                }
                else if (si.equityGrowth[1] != -99999999)
                {
                    si.rule1Growth = si.equityGrowth[1];
                }
                else
                {
                    si.rule1Growth = si.equityGrowth[0];
                }

                if (si.rule1Growth != -99999999)
                    si.rule1PE = si.rule1Growth * 2;
                else
                    si.rule1PE = -99999999;

                if (si.eps.Count >= 1)
                {
                    si.epsIn5Yrs = si.eps[0] * Math.Pow((1 + (si.rule1Growth / 100)), 5);
                    si.futurePriceIn5yrs = si.epsIn5Yrs * si.rule1PE;
                    si.stickerPrice = si.futurePriceIn5yrs / Math.Pow(1.15, 5); //assuming min rate of return to be 15%
                    si.priceToPurchase = si.stickerPrice / 2;
                    si.priceToPurchaseOverLast = si.priceToPurchase / si.lastPrice;
                }

                dataGridView1.Rows.Add();
                int rowIndex = dataGridView1.RowCount - 1;
                dataGridView1.Rows[rowIndex].Cells[0].Value = si.stockName;
                dataGridView1.Rows[rowIndex].Cells[1].Value = si.symbol;
                dataGridView1.Rows[rowIndex].Cells[2].Value = si.avgRoc[0] != -99999999 ? si.avgRoc[0].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[3].Value = si.avgRoc[1] != -99999999 ? si.avgRoc[1].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[4].Value = si.avgRoc[2] != -99999999 ? si.avgRoc[2].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[5].Value = si.avgRoc[3] != -99999999 ? si.avgRoc[3].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[6].Value = si.equityGrowth[0] != -99999999 ? si.equityGrowth[0].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[7].Value = si.equityGrowth[1] != -99999999 ? si.equityGrowth[1].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[8].Value = si.equityGrowth[2] != -99999999 ? si.equityGrowth[2].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[9].Value = si.equityGrowth[3] != -99999999 ? si.equityGrowth[3].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[10].Value = si.epsGrowth[0] != -99999999 ? si.epsGrowth[0].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[11].Value = si.epsGrowth[1] != -99999999 ? si.epsGrowth[1].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[12].Value = si.epsGrowth[2] != -99999999 ? si.epsGrowth[2].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[13].Value = si.epsGrowth[3] != -99999999 ? si.epsGrowth[3].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[14].Value = si.revenueGrowth[0] != -99999999 ? si.revenueGrowth[0].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[15].Value = si.revenueGrowth[1] != -99999999 ? si.revenueGrowth[1].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[16].Value = si.revenueGrowth[2] != -99999999 ? si.revenueGrowth[2].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[17].Value = si.revenueGrowth[3] != -99999999 ? si.revenueGrowth[3].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[18].Value = si.cashGrowth[0] != -99999999 ? si.cashGrowth[0].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[19].Value = si.cashGrowth[1] != -99999999 ? si.cashGrowth[1].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[20].Value = si.cashGrowth[2] != -99999999 ? si.cashGrowth[2].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[21].Value = si.cashGrowth[3] != -99999999 ? si.cashGrowth[3].ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[22].Value = si.debtToEquity != -99999999 ? si.debtToEquity.ToString("0.##") : "-";
                dataGridView1.Rows[rowIndex].Cells[23].Value = si.priceToPurchase.ToString("0.##");
                dataGridView1.Rows[rowIndex].Cells[24].Value = si.lastPrice.ToString("0.##");
                dataGridView1.Rows[rowIndex].Cells[25].Value = si.priceToPurchaseOverLast.ToString("0.##");

                si.score = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (si.avgRoc[i] > 10)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 2].Style.BackColor = Color.LimeGreen;
                        si.score += 2;
                    }
                    else if (si.avgRoc[i] > 5)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 2].Style.BackColor = Color.Yellow;
                        si.score += 1;
                    }
                    else if (si.avgRoc[i] <= 0 && si.avgRoc[i] != -99999999)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 2].Style.BackColor = Color.IndianRed;
                        si.score -= 2;
                    }

                    if (si.equityGrowth[i] > 10)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 6].Style.BackColor = Color.LimeGreen;
                        si.score += 2;
                    }
                    else if (si.equityGrowth[i] > 5)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 6].Style.BackColor = Color.Yellow;
                        si.score += 1;
                    }
                    else if (si.equityGrowth[i] <= 0 && si.equityGrowth[i] != -99999999)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 6].Style.BackColor = Color.IndianRed;
                        si.score -= 2;
                    }

                    if (si.epsGrowth[i] > 10)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 10].Style.BackColor = Color.LimeGreen;
                        si.score += 2;
                    }
                    else if (si.epsGrowth[i] > 5)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 10].Style.BackColor = Color.Yellow;
                        si.score += 1;
                    }
                    else if (si.epsGrowth[i] <= 0 && si.epsGrowth[i] != -99999999)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 10].Style.BackColor = Color.IndianRed;
                        si.score -= 2;
                    }

                    if (si.revenueGrowth[i] > 10)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 14].Style.BackColor = Color.LimeGreen;
                        si.score += 2;
                    }
                    else if (si.revenueGrowth[i] > 5)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 14].Style.BackColor = Color.Yellow;
                        si.score += 1;
                    }
                    else if (si.revenueGrowth[i] <= 0 && si.revenueGrowth[i] != -99999999)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 14].Style.BackColor = Color.IndianRed;
                        si.score -= 2;
                    }

                    if (si.cashGrowth[i] > 10)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 18].Style.BackColor = Color.LimeGreen;
                        si.score += 2;
                    }
                    else if (si.cashGrowth[i] > 5)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 18].Style.BackColor = Color.Yellow;
                        si.score += 1;
                    }
                    else if (si.cashGrowth[i] <= 0 && si.cashGrowth[i] != -99999999)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i + 18].Style.BackColor = Color.IndianRed;
                        si.score -= 2;
                    }

                }

                if (si.priceToPurchaseOverLast > 1)
                {
                    dataGridView1.Rows[rowIndex].Cells[25].Style.BackColor = Color.LimeGreen;
                    si.score += 2;
                }
                else if (si.priceToPurchaseOverLast > 0.8)
                {
                    dataGridView1.Rows[rowIndex].Cells[25].Style.BackColor = Color.Yellow;
                    si.score += 1;
                }
                else if (si.priceToPurchaseOverLast < 0.5)
                {
                    dataGridView1.Rows[rowIndex].Cells[25].Style.BackColor = Color.IndianRed;
                    si.score -= 2;
                }


                dataGridView1.Rows[rowIndex].Cells[26].Value = si.score;
                dataGridView1.Rows[rowIndex].Cells[27].Value = si.dateUpdate;
            }
            dataGridView1.ResumeLayout();

        }

        double findGrowth(int num, List<double> value)
        {
            if (value.Count > num && value[0] != -99999999 && value[num] != -99999999)
            {
                if (value[num] != 0)
                    return (Math.Pow((value[0] / value[num]), 1 / (double)num) - 1) * 100;
                else
                    return -99999999;
            }
            else
                return -99999999;
        }

        double findAvgRoc(double num, List<double> roc)
        {
            double temp = 0;
            if (roc.Count >= num)
            {
                for (int j = 0; j < num; j++)
                {
                    if (roc[j] != -99999999)
                        temp += roc[j];
                    else
                        return -99999999;
                }

                return temp / num;
            }
            else
                return -99999999;
        }
    }
}

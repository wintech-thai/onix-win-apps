using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Onix.Client.Helper;
using Onix.Client.Model;
using Onix.Client.Report;
using Onix.ClientCenter.UI.HumanResource.OTDocument;
using Onix.ClientCenter.UI.HumanResource.PayrollDocument;
using Onix.OnixHttpClient;

namespace Onix.ClientCenter.Forms.AcDesign.HROtDetails
{
    public partial class ULeaveSummary : UFormBase
    {
        private MVEployeeLeaveSummary leaveSummary = new MVEployeeLeaveSummary(new CTable(""));
        private double[] totals = new double[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public ULeaveSummary(MBaseModel model, int page, int totalPage, MReportConfig cfg, CReportPageParam param)
        {
            leaveSummary = (MVEployeeLeaveSummary) model;
          
            pageNo = page;
            pageCount = totalPage;
            pageParam = param;
            rptConfig = cfg;

            init();
            
            DataContext = model;
            InitializeComponent();

            populateDataGrid(leaveSummary);
        }

        private void populateDataGrid(MVEployeeLeaveSummary leaveSummary)
        {
            ColumnDefinition gridCol1 = new ColumnDefinition();
            ColumnDefinition gridCol2 = new ColumnDefinition();
            ColumnDefinition gridCol3 = new ColumnDefinition();
            ColumnDefinition gridCol4 = new ColumnDefinition();
            ColumnDefinition gridCol5 = new ColumnDefinition();
            ColumnDefinition gridCol6 = new ColumnDefinition();
            ColumnDefinition gridCol7 = new ColumnDefinition();
            ColumnDefinition gridCol8 = new ColumnDefinition();
            ColumnDefinition gridCol9 = new ColumnDefinition();
            ColumnDefinition gridCol10 = new ColumnDefinition();
            ColumnDefinition gridCol11 = new ColumnDefinition();
            ColumnDefinition gridCol12 = new ColumnDefinition();
            ColumnDefinition gridCol13 = new ColumnDefinition();

            gridCol1.Width = new GridLength(70, GridUnitType.Star);
            gridCol2.Width = new GridLength(35, GridUnitType.Star);
            gridCol3.Width = new GridLength(35, GridUnitType.Star);
            gridCol4.Width = new GridLength(35, GridUnitType.Star);
            gridCol5.Width = new GridLength(35, GridUnitType.Star);
            gridCol6.Width = new GridLength(35, GridUnitType.Star);
            gridCol7.Width = new GridLength(35, GridUnitType.Star);
            gridCol8.Width = new GridLength(35, GridUnitType.Star);
            gridCol9.Width = new GridLength(35, GridUnitType.Star);
            gridCol10.Width = new GridLength(35, GridUnitType.Star);
            gridCol11.Width = new GridLength(35, GridUnitType.Star);
            gridCol12.Width = new GridLength(35, GridUnitType.Star);
            gridCol13.Width = new GridLength(35, GridUnitType.Star);

            grdSummary.ColumnDefinitions.Add(gridCol1);
            grdSummary.ColumnDefinitions.Add(gridCol2);
            grdSummary.ColumnDefinitions.Add(gridCol3);
            grdSummary.ColumnDefinitions.Add(gridCol4);
            grdSummary.ColumnDefinitions.Add(gridCol5);
            grdSummary.ColumnDefinitions.Add(gridCol6);
            grdSummary.ColumnDefinitions.Add(gridCol7);
            grdSummary.ColumnDefinitions.Add(gridCol8);
            grdSummary.ColumnDefinitions.Add(gridCol9);
            grdSummary.ColumnDefinitions.Add(gridCol10);
            grdSummary.ColumnDefinitions.Add(gridCol11);
            grdSummary.ColumnDefinitions.Add(gridCol12);
            grdSummary.ColumnDefinitions.Add(gridCol13);

            //Header
            putDataRow(0, 16, HorizontalAlignment.Center, "วันที่", "ขาด (t)", "ไม่ครบ (t)", "สาย (t)", "ลากิจ (t)", "อื่นๆ (t)", "ขาด", "ไม่ครบ", "สาย", "ลากิจ", "อื่นๆ", "หักสาย", "ไม่หักสาย");

            var keyMap = leaveSummary.DateDeductionTypeHashMap;
            var i = 1;
            var total1 = 0.00;
            var total2 = 0.00;

            foreach (DateTime otDocDate in leaveSummary.DistinctDatesList)
            {
                var displayDate = CUtil.DateTimeToDateString2YY(otDocDate);

                var docDateStr = CUtil.DateTimeToDateStringInternal(otDocDate);
                var deductFlag = getDeductFlag(docDateStr, keyMap);

                var v1 = getDurationMin("1", docDateStr, keyMap); //ขาด
                var v2 = getDurationMin("2", docDateStr, keyMap); //ทำงานไม่ครบ
                var v3 = getDurationMin("3", docDateStr, keyMap); //สาย
                var v4 = getDurationMin("4", docDateStr, keyMap); //อื่น ๆ
                var v5 = getDurationMin("5", docDateStr, keyMap); //ลากิจ

                var m1 = getDeductionAmount("1", v1); //ขาด
                var m2 = getDeductionAmount("2", v2); //ทำงานไม่ครบ
                var m3 = getDeductionAmount("3", v3); //สาย
                var m4 = getDeductionAmount("4", v4); //อื่น ๆ
                var m5 = getDeductionAmount("5", v5); //ลากิจ
                var totMoney = CUtil.StringToDouble(m1) + 
                    CUtil.StringToDouble(m2) + 
                    CUtil.StringToDouble(m3) +
                    CUtil.StringToDouble(m4) + 
                    CUtil.StringToDouble(m5);

                var f1 = "";
                var f2 = "";
                if (deductFlag.Equals("Y"))
                {
                    f1 = CUtil.FormatNumber(totMoney.ToString());
                    total1 = total1 + totMoney;
                }
                else if (deductFlag.Equals("N"))
                {
                    f2 = CUtil.FormatNumber(totMoney.ToString());
                    total2 = total2 + totMoney;
                }

                putDataRow(i, 16, HorizontalAlignment.Right, displayDate, 
                    CUtil.FormatNumber(v1), CUtil.FormatNumber(v2), CUtil.FormatNumber(v3), CUtil.FormatNumber(v5),
                    CUtil.FormatNumber(v4), CUtil.FormatNumber(m1), CUtil.FormatNumber(m2), CUtil.FormatNumber(m3),
                    CUtil.FormatNumber(m5), CUtil.FormatNumber(m4), f1, f2);

                addTotal(v1, v2, v3, v4, v5, m1, m2, m3, m4, m5);
                i++;
            }

            putDataRow(i, 16, HorizontalAlignment.Right, "รวม",
                CUtil.FormatNumber(totals[1].ToString()), CUtil.FormatNumber(totals[2].ToString()), 
                CUtil.FormatNumber(totals[3].ToString()), CUtil.FormatNumber(totals[5].ToString()),
                CUtil.FormatNumber(totals[4].ToString()), CUtil.FormatNumber(totals[6].ToString()), 
                CUtil.FormatNumber(totals[7].ToString()), CUtil.FormatNumber(totals[8].ToString()),
                CUtil.FormatNumber(totals[10].ToString()), CUtil.FormatNumber(totals[9].ToString()),
                CUtil.FormatNumber(total1.ToString()), CUtil.FormatNumber(total2.ToString()));
        }

        private string getDeductFlag(string docDateStr, Hashtable keyMap)
        {
            int[] arr = { 1, 2, 3, 4, 5 };

            foreach (int t in arr)
            {
                var key1 = $"{docDateStr}:{t}";
                var obj1 = (MVPayrollDeductionItem) keyMap[key1];

                if (obj1 != null)
                {
                    return obj1.LeaveDeductionFlag;
                }
            }
            
            return "Y";
        }

        private void addTotal(string v1, string v2, string v3, string v4, string v5, 
            string v6, string v7, string v8, string v9, string v10)
        {
            totals[1] = totals[1] + CUtil.StringToDouble(v1);
            totals[2] = totals[2] + CUtil.StringToDouble(v2);
            totals[3] = totals[3] + CUtil.StringToDouble(v3);
            totals[4] = totals[4] + CUtil.StringToDouble(v4);
            totals[5] = totals[5] + CUtil.StringToDouble(v5);
            totals[6] = totals[6] + CUtil.StringToDouble(v6);
            totals[7] = totals[7] + CUtil.StringToDouble(v7);
            totals[8] = totals[8] + CUtil.StringToDouble(v8);
            totals[9] = totals[9] + CUtil.StringToDouble(v9);
            totals[10] = totals[10] + CUtil.StringToDouble(v10);
        }

        private string getDurationMin(string deductionType, string docDateStr, Hashtable keyMap)
        {
            var key1 = $"{docDateStr}:{deductionType}";
            var obj1 = (MVPayrollDeductionItem) keyMap[key1];

            var v1 = "";
            if (obj1 != null)
            {
                v1 = obj1.DurationMin;
            }

            return v1;
        }

        private double roundHour(double num)
        {
            double floor = Math.Floor(num);
            double diff = num - floor;

            if (diff == 0.00)
            {
                return num;
            }
            else if (diff >= 0.50)
            {
                double midpoint = floor + 0.50;
                return midpoint;
            }

            return floor;
        }

        private string getDeductionAmount(string deductionType, string totalMin)
        {
            double multiplier = 1.0;
            double rate = CUtil.StringToDouble(leaveSummary.HiringRate);

            double totalMinute = CUtil.StringToDouble(totalMin);
            double roundedHour = roundHour(totalMinute / 60.00);
            if (roundedHour < 1.00)
            {
                //ถ้าไม่ถึง 1 ให้ปัดลง
                roundedHour = 0.00;
            }

            if (deductionType.Equals("3") || deductionType.Equals("4"))
            {
                //สายกับขาด เท่านั้นที่คิดตัวคูณที่ 1.5
                multiplier = 1.5;
            }

            double amt = roundedHour * rate * multiplier;

            return amt.ToString();
        }

        private void putDataRow(int i, int headerFontSize, HorizontalAlignment halign,
            string v1, string v2, string v3, string v4, string v5, string v6, string v7, 
            string v8, string v9, string v10, string v11, string v12, string v13)
        {
            RowDefinition tmpRowDev = new RowDefinition();
            tmpRowDev.Height = new GridLength(20);
            grdSummary.RowDefinitions.Add(tmpRowDev);

            Border row1_1 = new Border();
            row1_1.BorderBrush = Brushes.Black;
            row1_1.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_1 = new TextBlock();
            data1_1.Foreground = Brushes.Black;
            data1_1.FontSize = headerFontSize;
            data1_1.Text = v1;
            data1_1.HorizontalAlignment = HorizontalAlignment.Center;
            data1_1.VerticalAlignment = VerticalAlignment.Center;
            row1_1.Child = data1_1;
            Grid.SetRow(row1_1, i);
            Grid.SetColumn(row1_1, 0);

            Border row1_2 = new Border();
            row1_2.BorderBrush = Brushes.Black;
            row1_2.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_2 = new TextBlock();
            data1_2.Foreground = Brushes.Black;
            data1_2.FontSize = headerFontSize;
            data1_2.Text = v2;
            data1_2.HorizontalAlignment = halign;
            data1_2.VerticalAlignment = VerticalAlignment.Center;
            data1_2.Margin = new Thickness(0, 0, 5, 0);
            row1_2.Child = data1_2;
            Grid.SetRow(row1_2, i);
            Grid.SetColumn(row1_2, 1);

            Border row1_3 = new Border();
            row1_3.BorderBrush = Brushes.Black;
            row1_3.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_3 = new TextBlock();
            data1_3.Foreground = Brushes.Black;
            data1_3.FontSize = headerFontSize;
            data1_3.Text = v3;
            data1_3.HorizontalAlignment = halign;
            data1_3.VerticalAlignment = VerticalAlignment.Center;
            data1_3.Margin = new Thickness(0, 0, 5, 0);
            row1_3.Child = data1_3;
            Grid.SetRow(row1_3, i);
            Grid.SetColumn(row1_3, 2);

            Border row1_4 = new Border();
            row1_4.BorderBrush = Brushes.Black;
            row1_4.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_4 = new TextBlock();
            data1_4.Foreground = Brushes.Black;
            data1_4.FontSize = headerFontSize;
            data1_4.Text = v4;
            data1_4.HorizontalAlignment = halign;
            data1_4.VerticalAlignment = VerticalAlignment.Center;
            data1_4.Margin = new Thickness(0, 0, 5, 0);
            row1_4.Child = data1_4;
            Grid.SetRow(row1_4, i);
            Grid.SetColumn(row1_4, 3);

            Border row1_5 = new Border();
            row1_5.BorderBrush = Brushes.Black;
            row1_5.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_5 = new TextBlock();
            data1_5.Foreground = Brushes.Black;
            data1_5.FontSize = headerFontSize;
            data1_5.Text = v5;
            data1_5.HorizontalAlignment = halign;
            data1_5.VerticalAlignment = VerticalAlignment.Center;
            data1_5.Margin = new Thickness(0, 0, 5, 0);
            row1_5.Child = data1_5;
            Grid.SetRow(row1_5, i);
            Grid.SetColumn(row1_5, 4);

            Border row1_6 = new Border();
            row1_6.BorderBrush = Brushes.Black;
            row1_6.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_6 = new TextBlock();
            data1_6.Foreground = Brushes.Black;
            data1_6.FontSize = headerFontSize;
            data1_6.Text = v6;
            data1_6.HorizontalAlignment = halign;
            data1_6.VerticalAlignment = VerticalAlignment.Center;
            data1_6.Margin = new Thickness(0, 0, 5, 0);
            data1_6.Margin = new Thickness(0, 0, 5, 0);
            row1_6.Child = data1_6;
            Grid.SetRow(row1_6, i);
            Grid.SetColumn(row1_6, 5);

            Border row1_7 = new Border();
            row1_7.BorderBrush = Brushes.Black;
            row1_7.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_7 = new TextBlock();
            data1_7.Foreground = Brushes.Black;
            data1_7.FontSize = headerFontSize;
            data1_7.Text = v7;
            data1_7.HorizontalAlignment = halign;
            data1_7.VerticalAlignment = VerticalAlignment.Center;
            data1_7.Margin = new Thickness(0, 0, 5, 0);
            data1_7.Margin = new Thickness(0, 0, 5, 0);
            row1_7.Child = data1_7;
            Grid.SetRow(row1_7, i);
            Grid.SetColumn(row1_7, 6);

            Border row1_8 = new Border();
            row1_8.BorderBrush = Brushes.Black;
            row1_8.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_8 = new TextBlock();
            data1_8.Foreground = Brushes.Black;
            data1_8.FontSize = headerFontSize;
            data1_8.Text = v8;
            data1_8.HorizontalAlignment = halign;
            data1_8.VerticalAlignment = VerticalAlignment.Center;
            data1_8.Margin = new Thickness(0, 0, 5, 0);
            data1_8.Margin = new Thickness(0, 0, 5, 0);
            row1_8.Child = data1_8;
            Grid.SetRow(row1_8, i);
            Grid.SetColumn(row1_8, 7);

            Border row1_9 = new Border();
            row1_9.BorderBrush = Brushes.Black;
            row1_9.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_9 = new TextBlock();
            data1_9.Foreground = Brushes.Black;
            data1_9.FontSize = headerFontSize;
            data1_9.Text = v9;
            data1_9.HorizontalAlignment = halign;
            data1_9.VerticalAlignment = VerticalAlignment.Center;
            data1_9.Margin = new Thickness(0, 0, 5, 0);
            data1_9.Margin = new Thickness(0, 0, 5, 0);
            row1_9.Child = data1_9;
            Grid.SetRow(row1_9, i);
            Grid.SetColumn(row1_9, 8);

            Border row1_10 = new Border();
            row1_10.BorderBrush = Brushes.Black;
            row1_10.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_10 = new TextBlock();
            data1_10.Foreground = Brushes.Black;
            data1_10.FontSize = headerFontSize;
            data1_10.Text = v10;
            data1_10.HorizontalAlignment = halign;
            data1_10.VerticalAlignment = VerticalAlignment.Center;
            data1_10.Margin = new Thickness(0, 0, 5, 0);
            data1_10.Margin = new Thickness(0, 0, 5, 0);
            row1_10.Child = data1_10;
            Grid.SetRow(row1_10, i);
            Grid.SetColumn(row1_10, 9);

            Border row1_11 = new Border();
            row1_11.BorderBrush = Brushes.Black;
            row1_11.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_11 = new TextBlock();
            data1_11.Foreground = Brushes.Black;
            data1_11.FontSize = headerFontSize;
            data1_11.Text = v11;
            data1_11.HorizontalAlignment = halign;
            data1_11.VerticalAlignment = VerticalAlignment.Center;
            data1_11.Margin = new Thickness(0, 0, 5, 0);
            data1_11.Margin = new Thickness(0, 0, 5, 0);
            row1_11.Child = data1_11;
            Grid.SetRow(row1_11, i);
            Grid.SetColumn(row1_11, 10);

            Border row1_12 = new Border();
            row1_12.BorderBrush = Brushes.Black;
            row1_12.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_12 = new TextBlock();
            data1_12.Foreground = Brushes.Black;
            data1_12.FontSize = headerFontSize;
            data1_12.Text = v12;
            data1_12.HorizontalAlignment = HorizontalAlignment.Right;
            data1_12.VerticalAlignment = VerticalAlignment.Center;
            data1_12.Margin = new Thickness(0, 0, 5, 0);
            data1_12.Margin = new Thickness(0, 0, 5, 0);
            row1_12.Child = data1_12;
            Grid.SetRow(row1_12, i);
            Grid.SetColumn(row1_12, 11);

            Border row1_13 = new Border();
            row1_13.BorderBrush = Brushes.Black;
            row1_13.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_13 = new TextBlock();
            data1_13.Foreground = Brushes.Black;
            data1_13.FontSize = headerFontSize;
            data1_13.Text = v13;
            data1_13.HorizontalAlignment = HorizontalAlignment.Right;
            data1_13.VerticalAlignment = VerticalAlignment.Center;
            data1_13.Margin = new Thickness(0, 0, 5, 0);
            data1_13.Margin = new Thickness(0, 0, 5, 0);
            row1_13.Child = data1_13;
            Grid.SetRow(row1_13, i);
            Grid.SetColumn(row1_13, 12);

            grdSummary.Children.Add(row1_1);
            grdSummary.Children.Add(row1_2);
            grdSummary.Children.Add(row1_3);
            grdSummary.Children.Add(row1_4);
            grdSummary.Children.Add(row1_5);
            grdSummary.Children.Add(row1_6);
            grdSummary.Children.Add(row1_7);
            grdSummary.Children.Add(row1_8);
            grdSummary.Children.Add(row1_9);
            grdSummary.Children.Add(row1_10);
            grdSummary.Children.Add(row1_11);
            grdSummary.Children.Add(row1_12);
            grdSummary.Children.Add(row1_13);
        }

        public MEmployee EmployeeObj
        {
            get
            {
                if (leaveSummary == null)
                {
                    return null;
                }

                return (leaveSummary.EmployeeObj);
            }

            set
            {
            }
        }

        public String Filler1Height
        {
            get
            {
                string p = rptConfig.GetConfigValue("Filler1Height");
                return (p);
            }
        }

        public String Filler2Height
        {
            get
            {
                string p = rptConfig.GetConfigValue("Filler2Height");
                return (p);
            }
        }
    }
}

using System;
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
    public partial class UFormOtDetailsP1 : UFormBase
    {
        private MVOTDocument otDoc = null;
        private MVPayrollDocumentItem item = null;

        public UFormOtDetailsP1(MBaseModel model, int page, int totalPage, MReportConfig cfg, CReportPageParam param)
        {
            dataSource = model;
            otDoc = (MVOTDocument) model;

            pageNo = page;
            pageCount = totalPage;
            pageParam = param;
            rptConfig = cfg;

            init();

            if (otDoc != null)
            {
                otDoc.InitializeAfterLoaded();
            }

            DataContext = model;
            InitializeComponent();


            if (otDoc != null)
            {
                populateOtGrid(otDoc);
                populateDeductionGrid(otDoc);
            }
        }

        private void populateDeductionGrid(MVOTDocument otDoc)
        {
            ColumnDefinition gridCol1 = new ColumnDefinition();
            ColumnDefinition gridCol2 = new ColumnDefinition();
            ColumnDefinition gridCol3 = new ColumnDefinition();

            gridCol1.Width = new GridLength(110, GridUnitType.Star);
            gridCol2.Width = new GridLength(35, GridUnitType.Star);
            gridCol3.Width = new GridLength(45, GridUnitType.Star);

            grdDeduction.ColumnDefinitions.Add(gridCol1);
            grdDeduction.ColumnDefinitions.Add(gridCol2);
            grdDeduction.ColumnDefinitions.Add(gridCol3);

            //Header
            putDeductionDataRow(0, 16, "รายการ", "นาที", "รวมหัก");

            var i = 1;
            var totalMin = 0.00;
            foreach (var item in otDoc.DeductionItems)
            {
                putDeductionDataRow(i, 16, $"{item.DeductionDate2YYFmt} ({item.DeductionTypeDesc})", item.DurationMinFmt, "");
                i++;

                totalMin = totalMin + CUtil.StringToDouble(item.DurationMin);
            }

            var totalMinFmt = CUtil.FormatNumber(totalMin.ToString());

            putDeductionDataRow(i, 16, "ปรับยอด", "", $"-{otDoc.AdjustAmount}");
            putDeductionDataRow(i + 1, 16, "หักสุทธิ", totalMinFmt, otDoc.DeductionAmountFmt);
        }


        private void populateOtGrid(MVOTDocument otDoc)
        {
            ColumnDefinition gridCol1 = new ColumnDefinition();
            ColumnDefinition gridCol2 = new ColumnDefinition();
            ColumnDefinition gridCol3 = new ColumnDefinition();
            ColumnDefinition gridCol4 = new ColumnDefinition();
            ColumnDefinition gridCol5 = new ColumnDefinition();
            ColumnDefinition gridCol6 = new ColumnDefinition();
            ColumnDefinition gridCol7 = new ColumnDefinition();

            gridCol1.Width = new GridLength(110, GridUnitType.Star);
            gridCol2.Width = new GridLength(35, GridUnitType.Star);
            gridCol3.Width = new GridLength(35, GridUnitType.Star);
            gridCol4.Width = new GridLength(35, GridUnitType.Star);
            gridCol5.Width = new GridLength(35, GridUnitType.Star);
            gridCol6.Width = new GridLength(45, GridUnitType.Star);
            gridCol7.Width = new GridLength(45, GridUnitType.Star);

            grdOT.ColumnDefinitions.Add(gridCol1);
            grdOT.ColumnDefinitions.Add(gridCol2);
            grdOT.ColumnDefinitions.Add(gridCol3);
            grdOT.ColumnDefinitions.Add(gridCol4);
            grdOT.ColumnDefinitions.Add(gridCol5);
            grdOT.ColumnDefinitions.Add(gridCol6);
            grdOT.ColumnDefinitions.Add(gridCol7);

            //Header
            putOtDataRow(0, 16, "รายการ", "ค่าแรง", "ชม ทำงาน", "ชม OT", "ตัวคูณ", "ค่าแรง", "OT");

            var i = 1;
            foreach (var item in otDoc.OTItems)
            {
                var displayDate = item.FromWorkDate2YYFmt;
                if (otDoc.EmployeeType == "2")
                {
                    //รายเดือน
                    displayDate = item.FromOtDate2YYFmt;
                }

                putOtDataRow(i, 16, displayDate, item.OtRateFmt, item.WorkAdjustedTotalHourFmt, item.OtAdjustedTotalHourFmt, item.MultiplierType, item.WorkAmountFmt, item.OtAmountFmt);
                i++;
            }

            putOtDataRow(i, 16, "ปรับยอด", "", "", "", "", "", $"-{otDoc.OtAdjustAmount}");
            putOtDataRow(i+1, 16, "รวม (ปัดเศษ)", "", "", "", "", otDoc.WorkedAmountFmt, otDoc.ReceiveAmountFmt);
        }

        private void putDeductionDataRow(int i, int headerFontSize, string v1, string v2, string v3)
        {
            RowDefinition tmpRowDev = new RowDefinition();
            tmpRowDev.Height = new GridLength(20);
            grdDeduction.RowDefinitions.Add(tmpRowDev);

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
            data1_2.HorizontalAlignment = HorizontalAlignment.Center;
            data1_2.VerticalAlignment = VerticalAlignment.Center;
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
            data1_3.HorizontalAlignment = HorizontalAlignment.Center;
            data1_3.VerticalAlignment = VerticalAlignment.Center;
            row1_3.Child = data1_3;
            Grid.SetRow(row1_3, i);
            Grid.SetColumn(row1_3, 2);

            grdDeduction.Children.Add(row1_1);
            grdDeduction.Children.Add(row1_2);
            grdDeduction.Children.Add(row1_3);
        }

        private void putOtDataRow(int i, int headerFontSize, string v1, string v2, string v3, string v4, string v5, string v6, string v7)
        {
            RowDefinition tmpRowDev = new RowDefinition();
            tmpRowDev.Height = new GridLength(20);
            grdOT.RowDefinitions.Add(tmpRowDev);

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

            //"ค่าแรง/hr"
            Border row1_2 = new Border();
            row1_2.BorderBrush = Brushes.Black;
            row1_2.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_2 = new TextBlock();
            data1_2.Foreground = Brushes.Black;
            data1_2.FontSize = headerFontSize;
            data1_2.Text = v2;
            data1_2.HorizontalAlignment = HorizontalAlignment.Center;
            data1_2.VerticalAlignment = VerticalAlignment.Center;
            row1_2.Child = data1_2;
            Grid.SetRow(row1_2, i);
            Grid.SetColumn(row1_2, 1);

            //"ชม ทำงาน"
            Border row1_3 = new Border();
            row1_3.BorderBrush = Brushes.Black;
            row1_3.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_3 = new TextBlock();
            data1_3.Foreground = Brushes.Black;
            data1_3.FontSize = headerFontSize;
            data1_3.Text = v3;
            data1_3.HorizontalAlignment = HorizontalAlignment.Center;
            data1_3.VerticalAlignment = VerticalAlignment.Center;
            row1_3.Child = data1_3;
            Grid.SetRow(row1_3, i);
            Grid.SetColumn(row1_3, 2);

            //"ชม OT";
            Border row1_4 = new Border();
            row1_4.BorderBrush = Brushes.Black;
            row1_4.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_4 = new TextBlock();
            data1_4.Foreground = Brushes.Black;
            data1_4.FontSize = headerFontSize;
            data1_4.Text = v4;
            data1_4.HorizontalAlignment = HorizontalAlignment.Center;
            data1_4.VerticalAlignment = VerticalAlignment.Center;
            row1_4.Child = data1_4;
            Grid.SetRow(row1_4, i);
            Grid.SetColumn(row1_4, 3);

            // ตัวคูณ
            Border row1_5 = new Border();
            row1_5.BorderBrush = Brushes.Black;
            row1_5.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_5 = new TextBlock();
            data1_5.Foreground = Brushes.Black;
            data1_5.FontSize = headerFontSize;
            data1_5.Text = v5;
            data1_5.HorizontalAlignment = HorizontalAlignment.Center;
            data1_5.VerticalAlignment = VerticalAlignment.Center;
            row1_5.Child = data1_5;
            Grid.SetRow(row1_5, i);
            Grid.SetColumn(row1_5, 4);

            // ค่าแรง
            Border row1_6 = new Border();
            row1_6.BorderBrush = Brushes.Black;
            row1_6.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_6 = new TextBlock();
            data1_6.Foreground = Brushes.Black;
            data1_6.FontSize = headerFontSize;
            data1_6.Text = v6;
            data1_6.HorizontalAlignment = HorizontalAlignment.Right;
            data1_6.VerticalAlignment = VerticalAlignment.Center;
            data1_6.Margin = new Thickness(0, 0, 5, 0);
            row1_6.Child = data1_6;
            Grid.SetRow(row1_6, i);
            Grid.SetColumn(row1_6, 5);

            // OT
            Border row1_7 = new Border();
            row1_7.BorderBrush = Brushes.Black;
            row1_7.BorderThickness = new Thickness(0, 0, 1, 1);
            TextBlock data1_7 = new TextBlock();
            data1_7.Foreground = Brushes.Black;
            data1_7.FontSize = headerFontSize;
            data1_7.Text = v7;
            data1_7.HorizontalAlignment = HorizontalAlignment.Right;
            data1_7.VerticalAlignment = VerticalAlignment.Center;
            data1_7.Margin = new Thickness(0, 0, 5, 0);
            row1_7.Child = data1_7;
            Grid.SetRow(row1_7, i);
            Grid.SetColumn(row1_7, 6);

            grdOT.Children.Add(row1_1);
            grdOT.Children.Add(row1_2);
            grdOT.Children.Add(row1_3);
            grdOT.Children.Add(row1_4);
            grdOT.Children.Add(row1_5);
            grdOT.Children.Add(row1_6);
            grdOT.Children.Add(row1_7);
        }

        public MEmployee EmployeeObj
        {
            get
            {
                if (otDoc == null)
                {
                    return null;
                }

                return (otDoc.EmployeeObj);
            }

            set
            {
            }
        }

        public MVPayrollDocumentItem ItemObj
        {
            get
            {
                return (item);
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

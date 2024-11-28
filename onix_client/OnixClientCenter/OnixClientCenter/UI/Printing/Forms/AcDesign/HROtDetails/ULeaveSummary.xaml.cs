using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        public ULeaveSummary(MBaseModel model, int page, int totalPage, MReportConfig cfg, CReportPageParam param)
        {
            dataSource = model;

            if (model is MVOTDocument)
            {
                var otDoc = (MVOTDocument) model;
                leaveSummary.EmployeeObj = otDoc.EmployeeObj;
            }
            else if (model is MVPayrollDocument)
            {
                int idx = pageNo - 1;
                var payrollDoc = (MVPayrollDocument)model;

                var item = payrollDoc.GetItemByIndex(idx);
                if (item == null)
                {
                    item = new MVPayrollDocumentItem(new CTable(""));
                }

                leaveSummary.EmployeeObj = item.EmployeeObj;
            }

            pageNo = page;
            pageCount = totalPage;
            pageParam = param;
            rptConfig = cfg;

            init();

            leaveSummary.InitializeAfterLoaded();
            
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

            //Header
            putDataRow(0, 16, HorizontalAlignment.Center, "วันที่", "ขาด(t)", "ไม่ครบ(t)", "สาย(t)", "ลากิจ(t)", "อื่นๆ(t)", "ขาด", "ไม่ครบ", "สาย", "ลากิจ", "อื่นๆ");
            /*
            var i = 1;
            foreach (var item in otDoc.OTItems)
            {
                var displayDate = item.FromWorkDateFmt;
                if (otDoc.EmployeeType == "2")
                {
                    //รายเดือน
                    displayDate = item.FromOtDateFmt;
                }

                putDataRow(i, 16, displayDate, item.OtRateFmt, item.WorkAdjustedTotalHourFmt, item.OtAdjustedTotalHourFmt, item.MultiplierType, item.WorkAmountFmt, item.OtAmountFmt);
                i++;
            }

            putDataRow(i, 16, "ปรับยอด", "", "", "", "", "", $"-{otDoc.OtAdjustAmount}");
            putDataRow(i + 1, 16, "รวม (ปัดเศษ)", "", "", "", "", otDoc.WorkedAmountFmt, otDoc.ReceiveAmountFmt);
            */
        }


        private void putDataRow(int i, int headerFontSize, HorizontalAlignment halign,
            string v1, string v2, string v3, string v4, string v5, string v6, string v7, 
            string v8, string v9, string v10, string v11)
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
            row1_11.Child = data1_11;
            Grid.SetRow(row1_11, i);
            Grid.SetColumn(row1_11, 10);

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

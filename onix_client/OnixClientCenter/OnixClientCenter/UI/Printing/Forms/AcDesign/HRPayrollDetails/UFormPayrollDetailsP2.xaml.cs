using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Onix.Client.Model;
using Onix.Client.Report;
using Onix.ClientCenter.UI.HumanResource.PayrollDocument;
using Onix.OnixHttpClient;

namespace Onix.ClientCenter.Forms.AcDesign.HRPayrollSlip
{
    public partial class UFormPayrollDetailsP2 : UFormBase
    {
        private MVPayrollDocument payrollDoc = null;
        private MVPayrollDocumentItem item = null;

        public UFormPayrollDetailsP2(MBaseModel model, int page, int totalPage, MReportConfig cfg, CReportPageParam param)
        {
            if (model == null)
            {
                model = new MVPayrollDocument(new CTable(""));
            }

            dataSource = model;
            payrollDoc = (MVPayrollDocument) model;

            pageNo = page;
            pageCount = totalPage;
            pageParam = param;
            rptConfig = cfg;

            init();

            int idx = pageNo - 1;

            item = payrollDoc.GetItemByIndex(0);
            if (item == null)
            {
                item = new MVPayrollDocumentItem(new CTable(""));
            }

            item.InitializeAfterLoaded();
            
            DataContext = model;
            InitializeComponent();

            populateAllowanceGrid(payrollDoc);
            populateExpenseGrid(payrollDoc);
        }

        private void populateExpenseGrid(MVPayrollDocument payrollDoc)
        {
            var otDoc = payrollDoc.OtDoc;

            ColumnDefinition gridCol1 = new ColumnDefinition();
            ColumnDefinition gridCol2 = new ColumnDefinition();
            ColumnDefinition gridCol3 = new ColumnDefinition();
            ColumnDefinition gridCol4 = new ColumnDefinition();

            gridCol1.Width = new GridLength(110, GridUnitType.Star);
            gridCol2.Width = new GridLength(35, GridUnitType.Star);
            gridCol3.Width = new GridLength(35, GridUnitType.Star);
            gridCol4.Width = new GridLength(45, GridUnitType.Star);

            grdExpense.ColumnDefinitions.Add(gridCol1);
            grdExpense.ColumnDefinitions.Add(gridCol2);
            grdExpense.ColumnDefinitions.Add(gridCol3);
            grdExpense.ColumnDefinitions.Add(gridCol4);


            //Header
            putExpenseDataRow(0, 16, "รายการ (สำรองจ่าย)", "จำนวน", "ราคา", "มูลค่า");

            var i = 1;
            foreach (var item in otDoc.Expensetems)
            {
                var itemDesc = $"{item.ExpenseDateFmt} ({item.ExpenseTypeDesc})";
                putExpenseDataRow(i, 16, itemDesc, item.ExpenseQuantityFmt, item.ExpensePriceFmt, item.ExpenseAmountFmt);
                i++;
            }

            putExpenseDataRow(i, 16, "รวม", "", "", otDoc.ExpenseAmountFmt);
        }


        private void populateAllowanceGrid(MVPayrollDocument payrollDoc)
        {
            var otDoc = payrollDoc.OtDoc;

            ColumnDefinition gridCol1 = new ColumnDefinition();
            ColumnDefinition gridCol2 = new ColumnDefinition();
            ColumnDefinition gridCol3 = new ColumnDefinition();
            ColumnDefinition gridCol4 = new ColumnDefinition();

            gridCol1.Width = new GridLength(110, GridUnitType.Star);
            gridCol2.Width = new GridLength(35, GridUnitType.Star);
            gridCol3.Width = new GridLength(35, GridUnitType.Star);
            gridCol4.Width = new GridLength(45, GridUnitType.Star);

            grdAllowance.ColumnDefinitions.Add(gridCol1);
            grdAllowance.ColumnDefinitions.Add(gridCol2);
            grdAllowance.ColumnDefinitions.Add(gridCol3);
            grdAllowance.ColumnDefinitions.Add(gridCol4);


            //Header
            putAllowanceDataRow(0, 16, "รายการ (สวัสดิการ)", "จำนวน", "ราคา", "มูลค่า");

            var i = 1;
            foreach (var item in otDoc.AllowanceItems)
            {
                var itemDesc = $"{item.AllowanceDateFmt} ({item.AllowanceTypeDesc})";
                putAllowanceDataRow(i, 16, itemDesc, item.AllowanceQuantityFmt, item.AllowancePriceFmt, item.AllowanceAmountFmt);
                i++;
            }

            putAllowanceDataRow(i, 16, "รวม", "", "", otDoc.AllowanceAmountFmt);
        }

        private void putExpenseDataRow(int i, int headerFontSize, string v1, string v2, string v3, string v4)
        {
            RowDefinition tmpRowDev = new RowDefinition();
            tmpRowDev.Height = new GridLength(20);
            grdExpense.RowDefinitions.Add(tmpRowDev);

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


            grdExpense.Children.Add(row1_1);
            grdExpense.Children.Add(row1_2);
            grdExpense.Children.Add(row1_3);
            grdExpense.Children.Add(row1_4);
        }

        private void putAllowanceDataRow(int i, int headerFontSize, string v1, string v2, string v3, string v4)
        {
            RowDefinition tmpRowDev = new RowDefinition();
            tmpRowDev.Height = new GridLength(20);
            grdAllowance.RowDefinitions.Add(tmpRowDev);

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


            grdAllowance.Children.Add(row1_1);
            grdAllowance.Children.Add(row1_2);
            grdAllowance.Children.Add(row1_3);
            grdAllowance.Children.Add(row1_4);
        }

        public MEmployee EmployeeObj
        {
            get
            {
                return (item.EmployeeObj);
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

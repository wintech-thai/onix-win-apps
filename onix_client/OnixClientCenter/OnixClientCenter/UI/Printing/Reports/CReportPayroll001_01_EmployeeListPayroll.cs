﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using Onix.OnixHttpClient;
using Onix.Client.Controller;
using Onix.Client.Helper;
using Onix.Client.Report;
using Onix.ClientCenter.Commons.Utils;

namespace Onix.ClientCenter.Reports
{
    public class CReportPayroll001_01_EmployeeListPayroll : CBaseReport
    {
        private Hashtable rowdef = new Hashtable();
        private String prevEntityId = "";
        private ArrayList entitySums = new ArrayList();
        private ArrayList sums = new ArrayList();

        private string[] empReceives = { "RECEIVE_INCOME", "RECEIVE_OT", "RECEIVE_POSITION", "RECEIVE_BONUS", "RECEIVE_TRANSPORTATION", "RECEIVE_TELEPHONE", "RECEIVE_ALLOWANCE", "RECEIVE_COMMISSION" };
        private string[] empDeducts = { "DEDUCT_TAX", "DEDUCT_SOCIAL_SECURITY", "DEDUCT_PENALTY", "DEDUCT_BORROW" };
        private string[] empDeductsTaxSsc = { "DEDUCT_TAX", "DEDUCT_SOCIAL_SECURITY" };
        private string[] empBorrowCoverage = { "DEDUCT_BORROW", "DEDUCT_COVERAGE" };

        public CReportPayroll001_01_EmployeeListPayroll() : base()
        {
        }

        private void configReport()
        {
            addConfig("L1", 10, "number", HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, "", "RN", false);
            addConfig("L1", 23, "date", HorizontalAlignment.Center, HorizontalAlignment.Left, HorizontalAlignment.Center, "FROM_SALARY_DATE", "DT", false);
            addConfig("L1", 20, "code", HorizontalAlignment.Center, HorizontalAlignment.Left, HorizontalAlignment.Center, "EMPLOYEE_CODE", "S", false);
            addConfig("L1", 30, "employee_name", HorizontalAlignment.Center, HorizontalAlignment.Left, HorizontalAlignment.Center, "EMPLOYEE_NAME_LASTNAME", "S", false);
            // addConfig("L1", 30, "bank_name", HorizontalAlignment.Center, HorizontalAlignment.Left, HorizontalAlignment.Center, "BANK_NAME", "S", false);
            addConfig("L1", 30, "account", HorizontalAlignment.Center, HorizontalAlignment.Left, HorizontalAlignment.Center, "ACCOUNT_NO", "S", false);

            addConfig("L1", 20, "salary", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "RECEIVE_INCOME", "D", true);
            addConfig("L1", 20, "ot", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "RECEIVE_OT", "D", true);
            addConfig("L1", 20, "revenue_position", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "RECEIVE_POSITION", "D", true);
            addConfig("L1", 20, "revenue_bonus", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "RECEIVE_BONUS", "D", true);
            addConfig("L1", 20, "revenue_transportation", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "RECEIVE_TRANSPORTATION", "D", true);
            addConfig("L1", 20, "revenue_telephone", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "RECEIVE_TELEPHONE", "D", true);
            addConfig("L1", 20, "revenue_allowance", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "RECEIVE_ALLOWANCE", "D", true);
            addConfig("L1", 20, "revenue_commission", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "RECEIVE_COMMISSION", "D", true);
            addConfig("L1", 20, "absent_late", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "DEDUCT_PENALTY", "D", true);
            addConfig("L1", 20, "revenue_total", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "EMP_RECEIVED_TOTAL_WITH_PENALTY", "D", true);
            
            addConfig("L1", 20, "social_security", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "DEDUCT_SOCIAL_SECURITY", "D", true);
            addConfig("L1", 20, "tax", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "DEDUCT_TAX", "D", true);
            addConfig("L1", 20, "รายได้สุทธิ", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "EMP_RECEIVED_TOTAL", "D", true);

            addConfig("L1", 20, "employee_borrow", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "DEDUCT_BORROW", "D", true); //ของเดิม คือ EMP_DEDUCT_TOTAL แต่ลูกค้าบอกว่าไม่ต้องแสดงหักรวม
            //addConfig("L1", 20, "return_advance", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "RECEIVE_REFUND", "D", true);
            addConfig("L1", 20, "coverage", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "DEDUCT_COVERAGE", "D", true);

            addConfig("L1", 20, "total_receive_emp", HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, "EMP_AMOUNT_TOTAL", "D", true);
        }

        protected override ArrayList getRecordSet()
        {
            ArrayList arr = OnixWebServiceAPI.GetListAPI("GetEmployeePayrollByDateList", "PAYROLL_EMPLOYEE_LIST", Parameter);
            return (arr);
        }

        #region Create Layout Report

        protected override void createRowTemplates()
        {
            String nm = "";
            Thickness defMargin = new Thickness(3, 1, 3, 1);

            configReport();

            nm = "HEADER_LEVEL1";
            CRow h2 = new CRow(nm, 30, getColumnCount("L1"), defMargin);
            h2.SetFont(null, FontStyles.Normal, 0, FontWeights.Bold);
            rowdef[nm] = h2;

            configRow("L1", h2, "H");

            //== START BODY ===

            nm = "DATA_LEVEL1";
            CRow r01 = new CRow(nm, 30, getColumnCount("L1"), defMargin);
            r01.SetFont(null, FontStyles.Normal, 0, FontWeights.Normal);
            rowdef[nm] = r01;

            configRow("L1", r01, "B");
            //== END BODY ===

            nm = "FOOTER_LEVEL1";
            CRow f1 = new CRow(nm, 30, getColumnCount("L1"), defMargin);
            f1.SetFont(null, FontStyles.Normal, 0, FontWeights.Bold);
            rowdef[nm] = f1;

            configRow("L1", f1, "F");

            baseTemplateName = "DATA_LEVEL1";
        }

        private void createDataHeaderRow2(UReportPage page)
        {
            CRow r = (CRow)rowdef["HEADER_LEVEL1"];

            r.FillColumnsText(getColumnHederTexts("L1", "H"));

            ConstructUIRow(page, r);
            AvailableSpace = AvailableSpace - r.GetHeight();
        }

        protected override UReportPage initNewArea(Size areaSize)
        {
            UReportPage page = new UReportPage();

            CreateGlobalHeaderRow(page);
            createDataHeaderRow2(page);

            page.Width = areaSize.Width;
            page.Height = areaSize.Height;

            page.Measure(areaSize);

            return (page);
        }

        #endregion

        public override Boolean IsNewVersion()
        {
            return (true);
        }

        private double getSumArray(CTable o, string[] arr)
        {
            double sum = 0.00;

            foreach (String field in arr)
            {
                String tmp = o.GetFieldValue(field);
                double amt = CUtil.StringToDouble(tmp);

                sum = sum + amt;
            }

            return (sum);
        }

        private void populateTempFields(CTable o)
        {
            String name = o.GetFieldValue("EMPLOYEE_NAME");
            String lastName = o.GetFieldValue("EMPLOYEE_LASTNAME");
            double refund = CUtil.StringToDouble(o.GetFieldValue("RECEIVE_REFUND"));
            double coverage = CUtil.StringToDouble(o.GetFieldValue("DEDUCT_COVERAGE"));

            o.SetFieldValue("EMPLOYEE_NAME_LASTNAME", name + " " + lastName);
            double latePenalty = CUtil.StringToDouble(o.GetFieldValue("DEDUCT_PENALTY"));
            double taxSscDeduct = getSumArray(o, empDeductsTaxSsc);
            double borrowCoverage = getSumArray(o, empBorrowCoverage);
            
            double received = getSumArray(o, empReceives);
            double total1 = received - latePenalty;
            double total2 = received - latePenalty - taxSscDeduct;
            double total3 = received - latePenalty - taxSscDeduct - borrowCoverage;

            double deduct = getSumArray(o, empDeducts);

            //o.SetFieldValue("EMP_DEDUCT_TOTAL", deduct.ToString());
            o.SetFieldValue("EMP_RECEIVED_TOTAL_WITH_PENALTY", total1.ToString());
            o.SetFieldValue("EMP_RECEIVED_TOTAL", total2.ToString());
            o.SetFieldValue("EMP_AMOUNT_TOTAL", total3.ToString());
        }

        public override CReportDataProcessingProperty DataToProcessingProperty(CTable o, ArrayList rows, int row)
        {
            String tmpPrevKey = prevEntityId;
            int rowcount = rows.Count;
            CReportDataProcessingProperty rpp = new CReportDataProcessingProperty();

            ArrayList keepTotal1 = copyTotalArray(entitySums);
            ArrayList keepTotal2 = copyTotalArray(sums);

            CRow r = (CRow)rowdef["DATA_LEVEL1"];
            CRow nr = r.Clone();
           
            double newh = AvailableSpace - nr.GetHeight();
            populateTempFields(o);

            if (newh > 0)
            {
                ArrayList temps = getColumnDataTexts("L1", row + 1, o);
                nr.FillColumnsText(temps);
                rpp.AddReportRow(nr);

                sums = sumDataTexts("L1", sums, temps);
                entitySums = sumDataTexts("L1", entitySums, temps);

                if (row == rowcount - 1)
                {
                    //=== End row
                    CRow ft1 = (CRow)rowdef["FOOTER_LEVEL1"];
                    CRow ftr1 = ft1.Clone();

                    ArrayList totals = displayTotalTexts("L1", sums, 1, "total");
                    ftr1.FillColumnsText(totals);

                    rpp.AddReportRow(ftr1);
                    newh = newh - ftr1.GetHeight();
                }
            }

            if (newh < 1)
            {
                rpp.IsNewPageRequired = true;

                //พวก sum ทั้งหลายจะถูกคืนค่ากลับไปด้วย เพราะถูกบวกไปแล้ว
                entitySums = keepTotal1;
                sums = keepTotal2;
                prevEntityId = tmpPrevKey;
            }
            else
            {
                AvailableSpace = newh;
            }

            return (rpp);
        }

        private void InitCombo(ComboBox cbo)
        {
            cbo.SelectedItem = "ObjSelf";
            cbo.DisplayMemberPath = "Description";
        }

        private void LoadDocumentTypeCombo(ComboBox cbo, String id)
        {
            CUtil.LoadComboFromCollection(cbo, false, id, CMasterReference.Instance.PayrollDocTypes);
        }

        private void LoadDocStatusCombo(ComboBox cbo, String id)
        {
            CUtil.LoadInventoryDocStatus(cbo, true, id);
        }

        public override ArrayList GetReportInputEntries()
        {
            CEntry entry = null;
            ArrayList entries = new ArrayList();

            entry = new CEntry("from_date", EntryType.ENTRY_DATE_MIN, 200, true, "FROM_DOCUMENT_DATE");
            entries.Add(entry);

            entry = new CEntry("to_date", EntryType.ENTRY_DATE_MAX, 200, true, "TO_DOCUMENT_DATE");
            entries.Add(entry);

            entry = new CEntry("employee_code", EntryType.ENTRY_TEXT_BOX, 200, true, "EMPLOYEE_CODE");
            entries.Add(entry);

            entry = new CEntry("employee_name", EntryType.ENTRY_TEXT_BOX, 350, true, "EMPLOYEE_NAME");
            entries.Add(entry);

            entry = new CEntry("document_type", EntryType.ENTRY_COMBO_BOX, 200, true, "EMPLOYEE_TYPE");
            entry.SetComboLoadAndInit(LoadDocumentTypeCombo, InitCombo, ObjectToIndex);
            entries.Add(entry);

            entry = new CEntry("inventory_doc_status", EntryType.ENTRY_COMBO_BOX, 200, true, "DOCUMENT_STATUS");
            entry.SetComboLoadAndInit(LoadDocStatusCombo, InitCombo, ObjectToIndex);
            entries.Add(entry);

            return (entries);
        }
    }
}

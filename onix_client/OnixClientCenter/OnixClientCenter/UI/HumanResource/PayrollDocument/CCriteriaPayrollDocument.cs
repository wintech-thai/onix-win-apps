﻿using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Onix.Client.Model;
using Onix.Client.Controller;
using Onix.Client.Helper;
using Onix.ClientCenter.Commons.UControls;
using Onix.ClientCenter.Commons.CriteriaConfig;
using Onix.OnixHttpClient;
using Onix.ClientCenter.Commons.Utils;
using Onix.ClientCenter.Commons.Windows;
using Onix.ClientCenter.Commons.Factories;

namespace Onix.ClientCenter.UI.HumanResource.PayrollDocument
{
    public class CCriteriaPayrollDocument : CCriteriaBase
    {
        private ArrayList items = new ArrayList();
        private CTable lastObjectReturned = null;
        private ObservableCollection<MBaseModel> itemSources = new ObservableCollection<MBaseModel>();
        private MBaseModel currentObj = null;

        public CCriteriaPayrollDocument() : base(new MVPayrollDocument(new CTable("")), "CCriteriaPayrollDocument")
        {
        }

        public override void Init(String type)
        {
            createCriteriaEntries();
            createGridColumns();
            loadRelatedReferences();
        }

        #region Criteria Configure

        private void loadRelatedReferences()
        {
            if (!CMasterReference.IsCashAccountLoad())
            {
                CMasterReference.LoadCashAccount(OnixWebServiceAPI.GetCashAccountList);
            }
        }

        private ArrayList createActionContextMenu()
        {
            ArrayList contexts = new ArrayList();

            CCriteriaContextMenu ct1 = new CCriteriaContextMenu("mnuEdit", "ADMIN_EDIT", new RoutedEventHandler(mnuContextMenu_Click), 1);
            contexts.Add(ct1);
            
            return (contexts);
        }

        private void createGridColumns()
        {
            ArrayList ctxMenues = createActionContextMenu();

            CCriteriaColumnCheckbox c0 = new CCriteriaColumnCheckbox("colChecked", "", "", 5, HorizontalAlignment.Left);
            c0.RegisterCheckboxBindingVariable(CheckBox.IsEnabledProperty, "IsEditable");
            AddGridColumn(c0);

            CCriteriaColumnButton c0_1 = new CCriteriaColumnButton("colAction", "action", ctxMenues, 5, HorizontalAlignment.Center, cmdAction_Click);
            AddGridColumn(c0_1);

            CCriteriaColumnText c4 = new CCriteriaColumnText("colDate", "inventory_doc_date", "DocumentDateNormalized", 20, HorizontalAlignment.Left);
            AddGridColumn(c4);

            CCriteriaColumnText c2 = new CCriteriaColumnText("colMonth", "inventory_doc_desc", "Note", 30, HorizontalAlignment.Left);
            AddGridColumn(c2);

            CCriteriaColumnText c5 = new CCriteriaColumnText("colStatus", "inventory_doc_status", "DocumentStatusDesc", 20, HorizontalAlignment.Left);
            AddGridColumn(c5);

            CCriteriaColumnText c6 = new CCriteriaColumnText("colType", "document_type", "EmployeeTypeDesc", 20, HorizontalAlignment.Left);
            AddGridColumn(c6);
        }

        private void createCriteriaEntries()
        {
            AddCriteriaControl(new CCriteriaEntry(CriteriaEntryType.ENTRY_LABEL, "", "year"));
            AddCriteriaControl(new CCriteriaEntry(CriteriaEntryType.ENTRY_TEXT_BOX, "PayrollYear", ""));

            AddCriteriaControl(new CCriteriaEntry(CriteriaEntryType.ENTRY_LABEL, "", "month"));
            CCriteriaEntry monthEntry = new CCriteriaEntry(CriteriaEntryType.ENTRY_COMBO_BOX, "PayrollMonthObj", "");
            monthEntry.SetComboItemSources("Months", "Description");
            AddCriteriaControl(monthEntry);

            AddCriteriaControl(new CCriteriaEntry(CriteriaEntryType.ENTRY_LABEL, "", "document_type"));
            CCriteriaEntry docTypeEntry = new CCriteriaEntry(CriteriaEntryType.ENTRY_COMBO_BOX, "EmployeeTypeObj", "");
            docTypeEntry.SetComboItemSources("PayrollDocTypes", "Description");
            AddCriteriaControl(docTypeEntry);

            AddCriteriaControl(new CCriteriaEntry(CriteriaEntryType.ENTRY_LABEL, "", "inventory_doc_status"));
            CCriteriaEntry statusEntry = new CCriteriaEntry(CriteriaEntryType.ENTRY_COMBO_BOX, "DocumentStatusObj", "");
            statusEntry.SetComboItemSources("DocumentStatuses", "Description");
            AddCriteriaControl(statusEntry);

            //===
            AddTopCriteriaControl(new CCriteriaEntry(CriteriaEntryType.ENTRY_LABEL, "", "from_date"));
            AddTopCriteriaControl(new CCriteriaEntry(CriteriaEntryType.ENTRY_DATE_MIN, "FromDocumentDate", ""));

            AddTopCriteriaControl(new CCriteriaEntry(CriteriaEntryType.ENTRY_LABEL, "", "to_date"));
            AddTopCriteriaControl(new CCriteriaEntry(CriteriaEntryType.ENTRY_DATE_MAX, "ToDocumentDate", ""));
        }

        private ArrayList createAddContextMenu()
        {
            ArrayList contexts = new ArrayList();

            CCriteriaContextMenu ct0 = new CCriteriaContextMenu("mnuBalance", "balance_document", new RoutedEventHandler(cmdAdd_Click), 1);
            contexts.Add(ct0);

            CCriteriaContextMenu ct1 = new CCriteriaContextMenu("mnuDaily", "daily", new RoutedEventHandler(cmdAdd_Click), 1);
            contexts.Add(ct1);

            CCriteriaContextMenu ct2 = new CCriteriaContextMenu("mnuMonthly", "monthly", new RoutedEventHandler(cmdAdd_Click), 2);
            contexts.Add(ct2);

            return (contexts);
        }

        #endregion;

        #region Private        

        public void ShowEditWindowEx(MVPayrollDocument actDoc)
        {
            if (!CHelper.VerifyAccessRight("HR_PAYROLL_VIEW"))
            {
                return;
            }

            CWinLoadParam param = new CWinLoadParam();
            PayrollDocType dt = (PayrollDocType) CUtil.StringToInt(actDoc.EmployeeType);
            int dtInt = (int)dt;

            if (dt == PayrollDocType.PayrollDaily)
            {
                param.Caption = CUtil.PayrollDocTypeToString(dt);
                param.Mode = "E";
                param.GenericType = dtInt.ToString();
                param.ActualView = actDoc;
                FactoryWindow.ShowWindow("WinAddEditPayrollDoc", param);
            }
            else if (dt == PayrollDocType.PayrollMonthly)
            {
                param.Caption = CUtil.PayrollDocTypeToString(dt);
                param.Mode = "E";
                param.GenericType = dtInt.ToString();
                param.ActualView = actDoc;
                FactoryWindow.ShowWindow("WinAddEditPayrollDoc", param);
            }
            else
            {
                param.Caption = CUtil.PayrollDocTypeToString(dt);
                param.Mode = "E";
                param.GenericType = dtInt.ToString();
                param.ActualView = actDoc;
                FactoryWindow.ShowWindow("WinAddEditPayrollDoc", param);
            }
        }
        
        #endregion

        public void SetDocumentType(AccountDocumentType dt)
        {
        }

        private void cmdTaxDocAdd_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsOpen = true;
        }

        public override Button GetButton()
        {
            ArrayList menues = createAddContextMenu();
            CCriteriaButton btn = new CCriteriaButton("add", true, menues, cmdTaxDocAdd_Click);

            return (btn.GetButton());
        }

        public override Tuple<CTable, ObservableCollection<MBaseModel>> QueryData()
        {
            MVPayrollDocument ad = (model as MVPayrollDocument);

            items = OnixWebServiceAPI.GetListAPI("GetPayrollDocList", "PAYROLL_DOC_LIST", ad.GetDbObject());
            lastObjectReturned = OnixWebServiceAPI.GetLastObjectReturned();

            itemSources.Clear();
            int idx = 0;
            foreach (CTable o in items)
            {
                MVPayrollDocument v = new MVPayrollDocument(o);

                v.RowIndex = idx;
                itemSources.Add(v);
                idx++;
            }

            Tuple<CTable, ObservableCollection<MBaseModel>> tuple = new Tuple<CTable, ObservableCollection<MBaseModel>>(lastObjectReturned, itemSources);
            return (tuple);
        }

        private String getAccessRightDelete()
        {
            String acr = "HR_PAYROLL_DELETE";
            return (acr);
        }

        public override int DeleteData(int rc)
        {

            if (!CHelper.VerifyAccessRight(getAccessRightDelete()))
            {
                return(rc);
            }

            int rCount = CHelper.DeleteSelectedItems(itemSources, OnixWebServiceAPI.DeleteAPI, rc.ToString(), "DeletePayrollDoc");

            return (rCount);
        }

        public override void DoubleClickData(MBaseModel m)
        {
            currentObj = m;
            ShowEditWindowEx((MVPayrollDocument) currentObj);
        }

        #region Event Handler

        private void cmdAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!CHelper.VerifyAccessRight("HR_PAYROLL_ADD"))
            {
                return;
            }

            MenuItem mnu = (MenuItem)sender;
            CWinLoadParam param = new CWinLoadParam();

            if (mnu.Name.Equals("mnuDaily"))
            {
                param.Caption = mnu.Header.ToString();
                param.Mode = "A";
                param.GenericType = "1";
                param.ParentItemSources = itemSources;
                FactoryWindow.ShowWindow("WinAddEditPayrollDoc", param);
            }
            else if (mnu.Name.Equals("mnuMonthly"))
            {
                param.Caption = mnu.Header.ToString();
                param.Mode = "A";
                param.GenericType = "2";
                param.ParentItemSources = itemSources;
                FactoryWindow.ShowWindow("WinAddEditPayrollDoc", param);
            }
            else if (mnu.Name.Equals("mnuBalance"))
            {
                param.Caption = mnu.Header.ToString();
                param.Mode = "A";
                param.GenericType = "0";
                param.ParentItemSources = itemSources;
                FactoryWindow.ShowWindow("WinAddEditPayrollDoc", param);
            }
        }

        private void cmdAction_Click(object sender, RoutedEventArgs e)
        {
            currentObj = (MBaseModel)(sender as UActionButton).Tag;
        }       

        private void mnuContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = (sender as MenuItem);
            string name = mnu.Name;

            if (name.Equals("mnuEdit"))
            {
                ShowEditWindowEx((MVPayrollDocument) currentObj);
            }
        }
#endregion
    }
}

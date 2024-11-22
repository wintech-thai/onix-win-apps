using System;
using System.Windows;
using Onix.OnixHttpClient;
using Onix.Client.Helper;
using Onix.Client.Model;
using Onix.ClientCenter.Commons.Windows;
using Onix.ClientCenter.Commons.Utils;
using System.Windows.Controls;

namespace Onix.ClientCenter.UI.HumanResource.OTDocument
{  
    public partial class WinAddEditPayrollAllowanceItem : WinBase
    {
        private MVPayrollAllowanceItem mv = null;
        private MVOTDocument mvParent = null;

        public WinAddEditPayrollAllowanceItem(CWinLoadParam param) : base(param)
        {
            accessRightName = "HR_ALLOWANCE_EDIT";
            mvParent = (MVOTDocument)loadParam.ActualParentView;

            InitializeComponent();

            //Need to be after InitializeComponent
            registerValidateControls(lblExpense, cboExpense, false);
            registerValidateControls(lblQuantity, txtQuantity, false);
            registerValidateControls(lblPrice, txtPrice, false);
        }

        protected override MBaseModel createObject()
        {
            mv = new MVPayrollAllowanceItem(new CTable(""));
            mv.AllowanceDate = DateTime.Now;
            mv.CreateDefaultValue();
            mv.AllowanceQuantity = "1.00";

            return (mv);
        }        

        protected override void addSubItem()
        {
            MVOTDocument parent = (MVOTDocument)loadParam.ActualParentView;
            parent.AddAllowanceItem(mv);
        }

        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            Boolean r = saveDataItem();
            if (r)
            {
                vw.IsModified = false;
                IsOKClick = true;
                CUtil.EnableForm(true, this);
                this.Close();
            }
        }
    }
}

using System;
using System.Collections.ObjectModel;
using Onix.Client.Helper;
using Onix.OnixHttpClient;
using Onix.Client.Model;
using System.Windows.Media;
using System.Collections;

namespace Onix.ClientCenter.UI.HumanResource.OTDocument
{
    public class MVEployeeLeaveSummary : MBaseModel
    {
        //หักเงินเนื่องจากการ leave ต่าง ๆ
        private ObservableCollection<MVPayrollDeductionItem> deductionItems = new ObservableCollection<MVPayrollDeductionItem>();
        private ArrayList dateArr = new ArrayList();
        private Hashtable deducAccumulates = new Hashtable();

        public MVEployeeLeaveSummary(CTable obj) : base(obj)
        {

        }

        public override void InitializeAfterLoaded()
        {
            initDeductionItem();
            accumulateDeductionItem();
        }

        public ObservableCollection<MVPayrollDeductionItem> DeductionItems
        {
            get
            {
                return (deductionItems);
            }
        }

        public ArrayList DistinctDatesList
        {
            get
            {
                return (dateArr);
            }
        }

        public Hashtable DateDeductionTypeHashMap
        {
            get
            {
                return (deducAccumulates);
            }
        }

        private void accumulateDeductionItem()
        {
            deducAccumulates.Clear();

            foreach (MVPayrollDeductionItem t in deductionItems)
            {
                var deductType = t.DeductionType;
                var docDate = t.DocumentDate;
                var docDateStr = CUtil.DateTimeToDateStringInternal(docDate);

                var key = $"{docDateStr}:{deductType}";

                if (!dateArr.Contains(docDate))
                {
                    dateArr.Add(docDate);
                }

                if (deducAccumulates.ContainsKey(key))
                {
                    MVPayrollDeductionItem currentAccum = (MVPayrollDeductionItem) deducAccumulates[key]; //Got object reference

                    var amt1 = sumString(currentAccum.DeductionAmount, t.DeductionAmount);
                    var amt2 = sumString(currentAccum.Duration, t.Duration);

                    currentAccum.DeductionAmount = amt1;
                    currentAccum.Duration = amt2;
                }
                else
                {
                    CTable dat = new CTable("");
                    dat.CopyFrom(t.GetDbObject());

                    MVPayrollDeductionItem firstAccum = new MVPayrollDeductionItem(dat);
                    deducAccumulates.Add(key, firstAccum);
                }
            }
        }

        private string sumString(string numStr1, string numStr2)
        {
            string str = (CUtil.StringToDouble(numStr1) + CUtil.StringToDouble(numStr2)).ToString();
            return str;
        }

        private void initDeductionItem()
        {
            CTable o = GetDbObject();
            if (o == null)
            {
                return;
            }

            ArrayList arr = o.GetChildArray("EMPLOYEE_PAYROLL_DEDUCTION_LIST");

            if (arr == null)
            {
                deductionItems.Clear();
                return;
            }

            deductionItems.Clear();
            foreach (CTable t in arr)
            {
                MVPayrollDeductionItem v = new MVPayrollDeductionItem(t);

                deductionItems.Add(v);
                v.ExtFlag = "I";
            }
        }

        public String HiringRate //อัตราค่าจ้าง/ชั่วโมง
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("HIRING_RATE"));
            }

            set
            {
                GetDbObject().SetFieldValue("HIRING_RATE", value);
                NotifyPropertyChanged();
            }
        }

        #region Employee Type

        public String EmployeeType
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("EMPLOYEE_TYPE"));
            }

            set
            {
                GetDbObject().SetFieldValue("EMPLOYEE_TYPE", value);
                NotifyPropertyChanged();
            }
        }

        public String EmployeeTypeDesc
        {
            get
            {
                if (EmployeeType.Equals(""))
                {
                    return ("");
                }

                PayrollDocType et = (PayrollDocType)Int32.Parse(EmployeeType);
                String str = CUtil.PayrollDocTypeToString(et);

                return (str);
            }

            set
            {

            }
        }

        public MMasterRef EmployeeTypeObj
        {
            set
            {
                MMasterRef m = value as MMasterRef;
                EmployeeType = m.MasterID;
            }
        }

        #endregion

        #region Employee

        public MEmployee EmployeeObj
        {
            set
            {
                if (value == null)
                {
                    EmployeeID = "";
                    EmployeeCode = "";
                    EmployeeName = "";
                    EmployeeLastName = "";

                    return;
                }

                MEmployee ii = (value as MEmployee);

                EmployeeID = ii.EmployeeID;
                EmployeeCode = ii.EmployeeCode;
                EmployeeName = ii.EmployeeName;
                EmployeeLastName = ii.EmployeeLastName;
                EmployeeNamePrefixDesc = ii.NamePrefixDesc;

                updateFlag();
            }

            get
            {
                if (GetDbObject() == null)
                {
                    return (null);
                }

                MEmployee m = new MEmployee(new CTable(""));
                m.EmployeeID = EmployeeID;
                m.EmployeeCode = EmployeeCode;
                m.EmployeeName = EmployeeName;
                m.EmployeeLastName = EmployeeLastName;
                m.EmployeeNameLastname = EmployeeNameLastName;
                m.NamePrefixDesc = EmployeeNamePrefixDesc;

                return (m);
            }
        }

        public String EmployeeID
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("EMPLOYEE_ID"));
            }

            set
            {
                GetDbObject().SetFieldValue("EMPLOYEE_ID", value);
            }
        }

        public String EmployeeCode
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("EMPLOYEE_CODE"));
            }

            set
            {
                GetDbObject().SetFieldValue("EMPLOYEE_CODE", value);
            }
        }

        public String EmployeeName
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("EMPLOYEE_NAME"));
            }

            set
            {
                GetDbObject().SetFieldValue("EMPLOYEE_NAME", value);
            }
        }

        public String EmployeeLastName
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("EMPLOYEE_LASTNAME"));
            }

            set
            {
                GetDbObject().SetFieldValue("EMPLOYEE_LASTNAME", value);
            }
        }

        public String EmployeeNameLastName
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (EmployeeName + " " + EmployeeLastName);
            }

            set
            {
            }
        }

        public String EmployeeNamePrefixDesc
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("NAME_PREFIX_DESC"));
            }

            set
            {
                GetDbObject().SetFieldValue("NAME_PREFIX_DESC", value);
            }
        }
        
        #endregion Employee

        #region DocumentDate

        public DateTime? FromDocumentDate
        {
            set
            {
                String str = CUtil.DateTimeToDateStringInternalMin(value);

                GetDbObject().SetFieldValue("FROM_DOCUMENT_DATE", str);
            }
        }

        public DateTime? ToDocumentDate
        {
            set
            {
                String str = CUtil.DateTimeToDateStringInternalMax(value);

                GetDbObject().SetFieldValue("TO_DOCUMENT_DATE", str);
            }
        }

        public DateTime DocumentDate
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return (DateTime.Now);
                }

                String str = GetDbObject().GetFieldValue("DOCUMENT_DATE");
                DateTime dt = CUtil.InternalDateToDate(str);

                return (dt);
            }

            set
            {
                String str = CUtil.DateTimeToDateStringInternal(value);

                GetDbObject().SetFieldValue("DOCUMENT_DATE", str);
                NotifyPropertyChanged();
            }
        }

        public DateTime StartDate
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return (DateTime.Now);
                }

                String str = GetDbObject().GetFieldValue("START_DATE");
                DateTime dt = CUtil.InternalDateToDate(str);

                return (dt);
            }

            set
            {
                String str = CUtil.DateTimeToDateStringInternal(value);

                GetDbObject().SetFieldValue("START_DATE", str);
                NotifyPropertyChanged();
            }
        }

        public String DocumentDateFmt
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                String str = GetDbObject().GetFieldValue("DOCUMENT_DATE");
                DateTime dt = CUtil.InternalDateToDate(str);
                String str2 = CUtil.DateTimeToDateString(dt);

                return (str2);
            }

            set
            {
            }
        }
        
        #endregion
    }
}

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
        public MVEployeeLeaveSummary(CTable obj) : base(obj)
        {

        }

        public override void InitializeAfterLoaded()
        {
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

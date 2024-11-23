using System;
using Onix.Client.Helper;
using Onix.OnixHttpClient;
using Onix.Client.Model;
using System.Collections.ObjectModel;

namespace Onix.ClientCenter.UI.HumanResource.OTDocument
{
    public class MVPayrollAllowanceItem : MBaseModel
    {
        private Boolean forDelete = false;
        private String oldFlag = "";
        private int seq = 0;

        public MVPayrollAllowanceItem(CTable obj) : base(obj)
        {
        }

        public void CreateDefaultValue()
        {
        }

        public int Seq
        {
            get
            {
                return (seq);
            }

            set
            {
                GetDbObject().SetFieldValue("INTERNAL_SEQ", value.ToString());
                seq = value;
            }
        }

        public bool IsDeleted
        {
            get
            {
                return (forDelete);
            }

            set
            {
                forDelete = value;
                if (forDelete)
                {
                    oldFlag = ExtFlag;
                    ExtFlag = "D";
                }
                else
                {
                    ExtFlag = oldFlag;
                }
            }
        }

        #region

        public MMasterRef AllowanceTypeObj
        {
            set
            {
                MMasterRef m = value as MMasterRef;
                if (m != null)
                {
                    AllowanceType = m.MasterID;
                    AllowanceTypeDesc = m.Description;
                    NotifyPropertyChanged();
                    updateFlag();
                }
            }

            get
            {
                if (GetDbObject() == null)
                {
                    return (null);
                }

                ObservableCollection<MMasterRef> items = CMasterReference.Instance.PayrollAllowanceTypes;
                if (items == null)
                {
                    return (null);
                }

                return ((MMasterRef)CUtil.IDToObject(items, "MasterID", AllowanceType));
            }
        }
        
        public String AllowanceTypeDesc
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (AllowanceTypeObj.Description);
            }

            set
            {
                GetDbObject().SetFieldValue("ALLOWANCE_TYPE_DESC", value);
                NotifyPropertyChanged();
            }
        }

        public String AllowanceType
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("0");
                }

                return (GetDbObject().GetFieldValue("ALLOWANCE_TYPE"));
            }

            set
            {
                GetDbObject().SetFieldValue("ALLOWANCE_TYPE", value);
                NotifyPropertyChanged();
            }
        }
        
        #endregion

        public String OTDocID
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("OT_DOC_ID"));
            }

            set
            {
                GetDbObject().SetFieldValue("OT_DOC_ID", value);
            }
        }
        
        public String AllowanceAmount
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("ALLOWANCE_AMOUNT"));
            }

            set
            {
                GetDbObject().SetFieldValue("ALLOWANCE_AMOUNT", value);
                NotifyPropertyChanged();
                NotifyPropertyChanged("AllowanceAmountFmt");

                updateFlag();
            }
        }

        public String AllowanceAmountFmt
        {
            get
            {
                String fmt = CUtil.FormatNumber(AllowanceAmount);
                return (fmt);
            }

            set
            {
            }
        }        

        public String AllowanceNote
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("ALLOWANCE_NOTE"));
            }

            set
            {
                GetDbObject().SetFieldValue("ALLOWANCE_NOTE", value);
                updateFlag();
            }
        }

        public DateTime AllowanceDate
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return (DateTime.Now);
                }

                String str = GetDbObject().GetFieldValue("ALLOWANCE_DATE");
                DateTime dt = CUtil.InternalDateToDate(str);

                return (dt);
            }

            set
            {
                String str = CUtil.DateTimeToDateStringInternal(value);

                GetDbObject().SetFieldValue("ALLOWANCE_DATE", str);
                NotifyPropertyChanged();
            }
        }

        public String AllowanceDateFmt
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                String str2 = CUtil.DateTimeToDateString(AllowanceDate);
                return (str2);
            }

            set
            {
            }
        }        

        public String AllowanceQuantity
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("ALLOWANCE_QUANTITY"));
            }

            set
            {
                GetDbObject().SetFieldValue("ALLOWANCE_QUANTITY", value);
                CalculateAllowanceAmount();
                NotifyPropertyChanged("AllowanceQuantityFmt");
                updateFlag();
            }
        }

        public String AllowanceQuantityFmt
        {
            get
            {
                String fmt = CUtil.FormatNumber(AllowanceQuantity);
                return (fmt);
            }

            set
            {
            }
        }

        public String AllowancePrice
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("ALLOWANCE_PRICE"));
            }

            set
            {
                GetDbObject().SetFieldValue("ALLOWANCE_PRICE", value);
                CalculateAllowanceAmount();
                NotifyPropertyChanged("AllowancePriceFmt");
                updateFlag();
            }
        }

        public String AllowancePriceFmt
        {
            get
            {
                String fmt = CUtil.FormatNumber(AllowancePrice);
                return (fmt);
            }
        }
        
        public void CalculateAllowanceAmount()
        {
            double price = CUtil.StringToDouble(AllowancePrice);
            double quantity = CUtil.StringToDouble(AllowanceQuantity);
            double amt = price * quantity;

            AllowanceAmount = amt.ToString();
        }
    }
}

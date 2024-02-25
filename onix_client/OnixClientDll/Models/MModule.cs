using System;
using System.Collections.ObjectModel;
using Onix.OnixHttpClient;

namespace Onix.Client.Model
{
    public class MModule : MBaseModel
    {
        private ObservableCollection<MGroupPermission> items = new ObservableCollection<MGroupPermission>();

        public MModule(CTable obj) : base(obj)
        {
        }

        public String ModuleName
        {
            get
            {
                if (GetDbObject() == null)
                {
                    return ("");
                }

                return (GetDbObject().GetFieldValue("MODULE_NAME"));
            }

            set
            {
                GetDbObject().SetFieldValue("MODULE_NAME", value);
            }
        }

        public ObservableCollection<MGroupPermission> Permissions
        {
            get
            {
                return (items);
            }

            set
            {
            }
        }

        public void AddGroupPermission(MGroupPermission pm)
        {
            items.Add(pm);
        }
    }
}

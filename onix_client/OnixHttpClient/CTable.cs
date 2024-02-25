using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.OnixHttpClient
{
    public class CTable
    {
        private Hashtable fieldsHash = (Hashtable) null;
        private ArrayList fieldsArr = (ArrayList) null;
        private string tbn = "";
        private Hashtable childHash = (Hashtable) null;

        public CTable(string table)
        {
            this.fieldsHash = new Hashtable();
            this.fieldsArr = new ArrayList();
            this.childHash = new Hashtable();
            this.tbn = table;
        }

        public void AddChildArray(string itemName, ArrayList items)
        {
            this.childHash.Add((object)itemName, (object)items);
        }

        public void RemoveChildArray(string itemName)
        {
            if (!this.childHash.ContainsKey((object)itemName))
                return;
            this.childHash.Remove((object)itemName);
        }

        public ArrayList GetChildArray(string itemName)
        {
            return (ArrayList)this.childHash[(object)itemName];
        }

        public Hashtable GetChildHash() => this.childHash;

        protected void AddField(string fldName, string type, string value)
        {
            CField cfield = new CField(type, value, fldName);
            this.fieldsHash.Add((object)fldName, (object)cfield);
            this.fieldsArr.Add((object)cfield);
        }

        public void SetFieldValue(string fldName, string value)
        {
            if (this.fieldsHash.Contains((object)fldName))
                ((CField)this.fieldsHash[(object)fldName]).setValue(value);
            else
                this.AddField(fldName, "S", value);
        }

        public string GetFieldValue(string fldName)
        {
            return this.fieldsHash.Contains((object)fldName) ? ((CField)this.fieldsHash[(object)fldName]).getValue() : "";
        }

        public string GetTableName() => this.tbn;

        public ArrayList GetTableFields() => this.fieldsArr;

        public void CopyFrom(CTable t)
        {
            foreach (CField tableField in t.GetTableFields())
                this.SetFieldValue(tableField.getName(), tableField.getValue());
        }

        public CTable Clone()
        {
            CTable ctable1 = new CTable(this.tbn);
            foreach (CField cfield in this.fieldsArr)
                ctable1.AddField(cfield.getName(), "S", cfield.getValue());
            Hashtable childHash = this.GetChildHash();
            foreach (string key in (IEnumerable)childHash.Keys)
            {
                ArrayList arrayList = (ArrayList)childHash[(object)key];
                ArrayList items = new ArrayList();
                ctable1.AddChildArray(key, items);
                foreach (CTable ctable2 in arrayList)
                {
                    CTable ctable3 = new CTable(ctable2.GetTableName());
                    items.Add((object)ctable3);
                    foreach (CField tableField in ctable2.GetTableFields())
                        ctable3.AddField(tableField.getName(), "S", tableField.getValue());
                }
            }
            return ctable1;
        }

        public CTable CloneAll()
        {
            CTable ctable1 = new CTable(this.tbn);
            foreach (CField cfield in this.fieldsArr)
                ctable1.AddField(cfield.getName(), "S", cfield.getValue());
            Hashtable childHash = this.GetChildHash();
            foreach (string key in (IEnumerable)childHash.Keys)
            {
                ArrayList arrayList = (ArrayList)childHash[(object)key];
                ArrayList items = new ArrayList();
                ctable1.AddChildArray(key, items);
                foreach (CTable ctable2 in arrayList)
                {
                    CTable ctable3 = ctable2.CloneAll();
                    items.Add((object)ctable3);
                }
            }
            return ctable1;
        }

    }
}

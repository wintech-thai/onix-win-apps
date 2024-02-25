using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.OnixHttpClient
{
    public class CRoot
    {
        private CTable param = (CTable)null;
        private CTable data = (CTable)null;
        private Hashtable hashOfArray = new Hashtable();

        public CRoot(CTable prm, CTable dta)
        {
            this.param = prm;
            this.data = dta;
        }

        public void AddChildArray(string arrName, List<CTable> items)
        {
            this.hashOfArray.Add((object)arrName, (object)items);
        }

        public List<CTable> GetChildArray(string arrName)
        {
            return (List<CTable>)this.hashOfArray[(object)arrName];
        }

        public Hashtable GetChildHash() => this.hashOfArray;

        public CTable Param
        {
            get => this.param;
            set => this.param = value;
        }

        public CTable Data
        {
            get => this.data;
            set => this.data = value;
        }
    }
}

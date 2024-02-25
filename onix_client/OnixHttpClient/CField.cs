using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.OnixHttpClient
{
    public class CField
    {
        private string type = "";
        private string value = "";
        private string name = "";

        public CField(string t, string v, string fldName)
        {
            this.type = t;
            this.value = v;
            this.name = fldName;
        }

        public string getValue() => this.value;

        public string getName() => this.name;

        public void setValue(string v) => this.value = v;
    }
}

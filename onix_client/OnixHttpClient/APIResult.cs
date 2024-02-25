using System.Collections.Generic;

namespace Onix.OnixHttpClient
{
    public class APIResult
    {
        public string DLL_VERSION = "1.0.4 built on 09/13/2017 (MM/DD/YYYY)";
        private CTable resultObject = (CTable)null;
        private CTable paramObject = (CTable)null;
        private string xmlSubmit = "";
        private string xmlReturn = "";
        private CRoot rootObj = (CRoot)null;

        public APIResult(CRoot root, string inXML, string outXML)
        {
            if (root != null)
            {
                root.Param.SetFieldValue(nameof(DLL_VERSION), this.DLL_VERSION);
                this.paramObject = root.Param;
                this.resultObject = root.Data;
                this.rootObj = root;
            }
            this.xmlSubmit = inXML;
            this.xmlReturn = outXML;
        }

        public bool IsSuccess() => this.paramObject.GetFieldValue("ERROR_CODE").Equals("0");

        public string GetErrorDesc() => this.paramObject.GetFieldValue("ERROR_DESC");

        public CTable GetResultObject() => this.resultObject;

        public string GetSubmitedXML() => this.xmlSubmit;

        public string GetResultXML() => this.xmlReturn;

        public void SetResultObject(CTable obj) => this.resultObject = obj;

        public void SetParamObject(CTable obj) => this.paramObject = obj;

        public CTable GetParamObject() => this.paramObject;

        public List<CTable> GetChildArray(string arrName) => this.rootObj.GetChildArray(arrName);
    }
}

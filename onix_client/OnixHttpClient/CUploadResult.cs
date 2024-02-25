using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.OnixHttpClient
{
    public class CUploadResult
    {
        public string Token { get; set; }

        public int ErrCode { get; set; }

        public string ErrDesc { get; set; }

        public string ReturnString { get; set; }

        public string StatusCode { get; set; }

        public string MimeType { get; set; }

        public CUploadResult(string uploadResultString)
        {
            string[] strArray = uploadResultString.Split('|');
            this.ErrCode = 0;
            this.ErrDesc = "";
            this.Token = "";
            this.ReturnString = uploadResultString;
            try
            {
                this.StatusCode = strArray[0];
                this.ErrCode = int.Parse(strArray[1]);
                this.MimeType = strArray[3];
                this.ErrDesc = strArray[4];
                this.Token = strArray[4];
            }
            catch
            {
            }
        }
    }
}

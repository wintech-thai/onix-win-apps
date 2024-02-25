using Onix.OnixHttpClient;
using System;
using System.Collections;
using System.Net;
using System.Threading;

namespace Onix.OnixHttpClient
{
    public class OnixConnectionXML : OnixConnection
    {
        private Thread sseThread = (Thread)null;
        private string loginID = "";
        private int downloadingFileSize = 0;

        public OnixConnectionXML(string symKey, string url, string vt) : base(symKey, url, vt)
        {
        }

        public override APIResult SubmitCommand(CTable cmd, string functionName)
        {
            return this.SubmitCommand(cmd, functionName, false);
        }

        public APIResult SubmitCommand(CTable cmd, string functionName, bool debug)
        {
            CTable prm = new CTable("PARAM");
            prm.SetFieldValue("FUNCTION_NAME", functionName);
            prm.SetFieldValue("SESSION", this.session);
            prm.SetFieldValue("LOGIN_ID", this.loginID);
            string str1 = "N";
            if (debug)
                str1 = "Y";
            prm.SetFieldValue("DEBUG_FLAG", str1);
            prm.SetFieldValue("WisWsClientAPI_VERSION", this.Version);
            ArrayList variableNames = this.GetVariableNames();
            Hashtable variableHash = this.GetVariableHash();
            foreach (string str2 in variableNames)
            {
                string str3 = (string)variableHash[(object)str2];
                prm.SetFieldValue(str2, str3);
            }
            string xmlString = this.CreateXMLString(new CRoot(prm, cmd));
            string str4 = this.submitRequest(xmlString);
            return new APIResult(this.XMLToRootObject(str4), xmlString, str4);
        }

        public override APIResult Login(CTable user)
        {
            APIResult apiResult = this.SubmitCommand(user, nameof(Login));
            CTable paramObject = apiResult.GetParamObject();
            this.session = paramObject.GetFieldValue("SESSION");
            this.loginID = paramObject.GetFieldValue("LOGIN_ID");
            this.userName = apiResult.GetResultObject().GetFieldValue("USER_NAME");
            return apiResult;
        }

        public override int GetDownloadingFileSize() => this.downloadingFileSize;

        public void KillSSEThread()
        {
            if (this.sseThread == null)
                return;
            this.sseThread.Abort();
        }

        public override bool SubmitCommandSSE(
          CTable cmd,
          string functionName,
          SSEMessageUpdate prog,
          SSEMessageCopleted comp)
        {
            if (this.sseThread != null)
                return false;
            CTable prm = new CTable("PARAM");
            prm.SetFieldValue("FUNCTION_NAME", functionName);
            prm.SetFieldValue("SESSION", this.session);
            prm.SetFieldValue("LOGIN_ID", this.loginID);
            string xmlString = this.CreateXMLString(new CRoot(prm, cmd));
            HttpWebResponse sseResp = (HttpWebResponse)null;
            try
            {
                sseResp = this.submitSSERequest(xmlString);
                if (sseResp == null)
                    return false;
            }
            catch
            {
                return false;
            }
            this.sseThread = new Thread((ThreadStart)(() =>
            {
                CStreamReaderChunk cstreamReaderChunk = new CStreamReaderChunk(sseResp.GetResponseStream(), "BEGIN", "END");
                int num = 0;
                bool flag = false;
                CRoot croot = (CRoot)null;
                while (!cstreamReaderChunk.EndOfStream)
                {
                    croot = this.XMLToRootObject(this.DecryptString(cstreamReaderChunk.ReadChunk().Replace(Environment.NewLine, "")));
                    if (num == 0 && !croot.Param.GetFieldValue("STAGE").Equals("START"))
                    {
                        flag = true;
                        break;
                    }
                    checked { ++num; }
                    prog(croot.Param, croot.Data);
                }
                if (flag)
                {
                    croot.Param.SetFieldValue("STATUS", "ERROR");
                    croot.Param.SetFieldValue("ERROR_DESC1", "No START found!!!");
                    comp(croot.Param, croot.Data);
                }
                else
                {
                    cstreamReaderChunk.ReadChunk();
                    comp(croot.Param, croot.Data);
                }
                this.sseThread = (Thread)null;
            }));
            this.sseThread.Start();
            return true;
        }
    }
}

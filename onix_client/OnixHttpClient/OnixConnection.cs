using Onix.OnixHttpClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Onix.OnixHttpClient
{
    public class OnixConnection : IOnixConnection
    {
        private string key;
        private string webUrl;
        private string downloaderScript = "downloader.php";
        private string uploaderScript = "uploader.php";
        private const string initVector = "pemgail9uzpgzl88";
        private string vector = "pemgail9uzpgzl88";
        private Hashtable variables = new Hashtable();
        private ArrayList varNames = new ArrayList();
        private bool imageUpload = true;
        private bool fileUploadPublic = false;
        protected string Version = "1.0.5";
        protected string session = "";
        protected string userName = "";

        public OnixConnection(string symKey, string url, string vt)
        {
            this.key = symKey;
            this.webUrl = url;
            this.vector = vt;
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)((_param1, _param2, _param3, _param4) => true);
        }

        public OnixConnection(string symKey, string url, string vt, string downloader)
        {
            this.key = symKey;
            this.webUrl = url;
            this.vector = vt;
            this.downloaderScript = downloader;
        }

        public void RegisterVariable(string paramName, string value)
        {
            if (!this.variables.ContainsKey((object)paramName))
            {
                this.variables.Add((object)paramName, (object)value);
                this.varNames.Add((object)paramName);
            }
            else
                this.variables[(object)paramName] = (object)value;
        }

        public bool IsUploadImage
        {
            set => this.imageUpload = value;
            get => this.imageUpload;
        }

        public bool IsUploadPublic
        {
            set => this.fileUploadPublic = value;
            get => this.fileUploadPublic;
        }

        public string Sesion => this.session;

        public ArrayList GetVariableNames() => this.varNames;

        public Hashtable GetVariableHash() => this.variables;

        public virtual APIResult SubmitCommand(CTable cmd, string functionName) => (APIResult)null;

        public virtual APIResult Login(CTable user) => (APIResult)null;

        public virtual int GetDownloadingFileSize() => 0;

        public virtual CTable Download(
          CTable user,
          UploadProgressChangedEventHandler prog,
          UploadValuesCompletedEventHandler comp)
        {
            return (CTable)null;
        }

        public string EncryptString(string plainText)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            rijndaelManaged.Key = Encoding.UTF8.GetBytes(this.key);
            rijndaelManaged.IV = Encoding.UTF8.GetBytes("pemgail9uzpgzl88");
            ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] array = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(array);
        }

        private void copyTo(Stream src, Stream dest)
        {
            byte[] buffer = new byte[4096];
            int count;
            while ((count = src.Read(buffer, 0, buffer.Length)) != 0)
                dest.Write(buffer, 0, count);
        }

        private byte[] zip(string str)
        {
            using (MemoryStream src = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (GZipStream dest = new GZipStream((Stream)memoryStream, CompressionMode.Compress))
                        this.copyTo((Stream)src, (Stream)dest);
                    return memoryStream.ToArray();
                }
            }
        }

        private byte[] unzip(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (MemoryStream dest = new MemoryStream())
                {
                    using (GZipStream src = new GZipStream((Stream)memoryStream, CompressionMode.Decompress))
                        this.copyTo((Stream)src, (Stream)dest);
                    return dest.ToArray();
                }
            }
        }

        public string DecryptString(string cipherText)
        {
            byte[] buffer = this.unzip(Convert.FromBase64String(cipherText));
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            rijndaelManaged.Key = Encoding.UTF8.GetBytes(this.key);
            rijndaelManaged.IV = Encoding.UTF8.GetBytes("pemgail9uzpgzl88");
            ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
            MemoryStream memoryStream = new MemoryStream(buffer);
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] numArray = new byte[buffer.Length];
            int count = cryptoStream.Read(numArray, 0, numArray.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(numArray, 0, count);
        }

        private string EscapeDataString(string value)
        {
            int length = 2000;
            StringBuilder stringBuilder = new StringBuilder();
            int num1 = value.Length / length;
            int num2 = 0;
            while (num2 <= num1)
            {
                if (num2 < num1)
                    stringBuilder.Append(Uri.EscapeDataString(value.Substring(checked(length * num2), length)));
                else
                    stringBuilder.Append(Uri.EscapeDataString(value.Substring(checked(length * num2))));
                checked { ++num2; }
            }
            return stringBuilder.ToString();
        }

        protected string submitRequest(string d)
        {
            var responseText = "";
            long num = 0;
            var str = "";

            try
            {
                var username = "user1";
                var password = this.key;
                string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));

                byte[] bytes = Encoding.ASCII.GetBytes(string.Format("DBOSOBJ={0}", (object) this.EscapeDataString(d)));
                HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(this.webUrl);

                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = (long) bytes.Length;
                httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);

                num = (long) bytes.Length;
                httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);

                var httpResponse = httpWebRequest.GetResponse();  
                responseText = new StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                str = responseText;
            }
            catch (Exception ex)
            {
                str = string.Format("{0}\n{1}\nLen={2}", (object) responseText, (object) ex.Message, (object) num);
            }

            return str;
        }

        protected HttpWebResponse submitSSERequest(string d)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(string.Format("DBOSOBJ={0}", (object)Uri.EscapeDataString(this.EncryptString(d))));
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.webUrl);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = (long)bytes.Length;
            httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
            return (HttpWebResponse)httpWebRequest.GetResponse();
        }

        private XmlElement createElementFromTable(XmlDocument doc, CTable tb)
        {
            ArrayList tableFields = tb.GetTableFields();
            XmlElement element1 = doc.CreateElement("OBJECT");
            element1.SetAttribute("name", tb.GetTableName());
            foreach (CField cfield in tableFields)
            {
                XmlElement element2 = doc.CreateElement("FIELD");
                element2.SetAttribute("name", cfield.getName());
                string str = cfield.getValue();
                element2.InnerText = str;
                element1.AppendChild((XmlNode)element2);
            }
            Hashtable childHash = tb.GetChildHash();
            foreach (string key in (IEnumerable)childHash.Keys)
            {
                ArrayList arrayList = (ArrayList)childHash[(object)key];
                XmlElement element3 = doc.CreateElement("ITEMS");
                element3.SetAttribute("name", key);
                element1.AppendChild((XmlNode)element3);
                foreach (CTable tb1 in arrayList)
                {
                    XmlElement elementFromTable = this.createElementFromTable(doc, tb1);
                    element3.AppendChild((XmlNode)elementFromTable);
                }
            }
            return element1;
        }

        public string CreateXMLString(CRoot root)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "");
            XmlElement documentElement = doc.DocumentElement;
            doc.InsertBefore((XmlNode)xmlDeclaration, (XmlNode)documentElement);
            XmlElement element1 = doc.CreateElement("API");
            XmlElement elementFromTable1 = this.createElementFromTable(doc, root.Param);
            XmlElement elementFromTable2 = this.createElementFromTable(doc, root.Data);
            element1.AppendChild((XmlNode)elementFromTable1);
            element1.AppendChild((XmlNode)elementFromTable2);
            Hashtable childHash = root.GetChildHash();
            foreach (string key in (IEnumerable)childHash.Keys)
            {
                ArrayList arrayList = (ArrayList)childHash[(object)key];
                XmlElement element2 = doc.CreateElement("ITEMS");
                element2.SetAttribute("name", key);
                element1.AppendChild((XmlNode)element2);
                foreach (CTable tb in arrayList)
                {
                    XmlElement elementFromTable3 = this.createElementFromTable(doc, tb);
                    element2.AppendChild((XmlNode)elementFromTable3);
                }
            }
            doc.AppendChild((XmlNode)element1);
            return doc.OuterXml;
        }

        private CTable populateTableObject(XmlNode node)
        {
            CTable ctable1 = new CTable(node.Attributes["name"].Value);
            foreach (XmlNode childNode1 in node.ChildNodes)
            {
                if (childNode1.Name.Equals("FIELD"))
                    ctable1.SetFieldValue(childNode1.Attributes["name"].Value, childNode1.InnerText);
                else if (childNode1.Name.Equals("ITEMS"))
                {
                    string itemName = childNode1.Attributes["name"].Value;
                    ArrayList items = new ArrayList();
                    ctable1.AddChildArray(itemName, items);
                    foreach (XmlNode childNode2 in childNode1.ChildNodes)
                    {
                        if (childNode2.Name.Equals("OBJECT"))
                        {
                            CTable ctable2 = this.populateTableObject(childNode2);
                            items.Add((object)ctable2);
                        }
                    }
                }
            }
            return ctable1;
        }

        private CRoot getRootObject(XmlNode node)
        {
            CTable ctable1 = (CTable)null;
            CTable ctable2 = (CTable)null;
            CRoot rootObject = new CRoot((CTable)null, (CTable)null);
            int num = 0;
            foreach (XmlNode childNode1 in node.ChildNodes)
            {
                switch (num)
                {
                    case 0:
                        ctable1 = this.populateTableObject(childNode1);
                        break;
                    case 1:
                        ctable2 = this.populateTableObject(childNode1);
                        break;
                    default:
                        if (childNode1.Name.Equals("ITEMS"))
                        {
                            string arrName = childNode1.Attributes["name"].Value;
                            List<CTable> items = new List<CTable>();
                            rootObject.AddChildArray(arrName, items);
                            foreach (XmlNode childNode2 in childNode1.ChildNodes)
                            {
                                if (childNode2.Name.Equals("OBJECT"))
                                {
                                    CTable ctable3 = this.populateTableObject(childNode2);
                                    items.Add(ctable3);
                                }
                            }
                        }
                        break;
                }
                checked { ++num; }
            }
            rootObject.Data = ctable2;
            rootObject.Param = ctable1;
            return rootObject;
        }

        public CRoot XMLToRootObject(string xml)
        {
            CRoot rootObject;
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                rootObject = this.getRootObject(xmlDocument.ChildNodes[1]);
            }
            catch (Exception ex)
            {
                CTable prm = new CTable("");
                CTable dta = new CTable("");
                prm.SetFieldValue("ERROR_CODE", "1");
                prm.SetFieldValue("ERROR_DESC", ex.Message);
                prm.SetFieldValue("ERROR_DESC2", xml);
                rootObject = new CRoot(prm, dta);
            }
            return rootObject;
        }

        public virtual void DownloadFileAsync(
          string srcFile,
          string outputFile,
          DownloadProgressChangedEventHandler prog,
          AsyncCompletedEventHandler comp)
        {
            Uri uri = new Uri(this.webUrl);
            string uriString = string.Format("{0}?name={1}", (object)this.webUrl.Replace(Path.GetFileName(this.webUrl), this.downloaderScript), (object)srcFile);
            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += prog;
            webClient.DownloadFileCompleted += comp;
            webClient.DownloadFileAsync(new Uri(uriString), outputFile);
            webClient.Dispose();
        }

        public virtual void UploadFileAsync(
          string srcFile,
          UploadProgressChangedEventHandler prog,
          UploadFileCompletedEventHandler comp)
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            string str1 = "N";
            if (this.imageUpload)
                str1 = "Y";
            string str2 = "N";
            if (this.fileUploadPublic)
                str2 = "Y";
            nameValueCollection.Add("PublicShare", str2);
            nameValueCollection.Add("ImageUpload", str1);
            nameValueCollection.Add("Session", this.session);
            nameValueCollection.Add("UserName", this.userName);
            Uri uri = new Uri(this.webUrl);
            string uriString = this.webUrl.Replace(Path.GetFileName(this.webUrl), this.uploaderScript);
            WebClient webClient = new WebClient();
            webClient.QueryString = nameValueCollection;
            webClient.UploadProgressChanged += prog;
            webClient.UploadFileCompleted += comp;
            webClient.UploadFileAsync(new Uri(uriString), srcFile);
            webClient.Dispose();
        }

        public string GetUploadedFileUrl(string fileName, string area)
        {
            return string.Format("{0}?name={1}&area={2}&session={3}", (object)this.webUrl.Replace(Path.GetFileName(this.webUrl), "content.php"), (object)fileName, (object)area, (object)this.session);
        }

        public virtual bool SubmitCommandSSE(
          CTable cmd,
          string functionName,
          SSEMessageUpdate prog,
          SSEMessageCopleted comp)
        {
            return false;
        }
    }
}

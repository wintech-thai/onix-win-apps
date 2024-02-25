using System;
using System.IO;


namespace Onix.OnixHttpClient
{
    public class CStreamReaderChunk
    {
        private StreamReader srr = (StreamReader)null;
        private string startKey = "BEGIN";
        private string stopKey = "END";

        public CStreamReaderChunk(Stream s, string start, string stop)
        {
            this.srr = new StreamReader(s);
        }

        public bool EndOfStream => this.srr.EndOfStream;

        public string ReadChunk()
        {
            string str1 = "";
            while (!this.srr.EndOfStream)
            {
                string str2 = this.srr.ReadLine();
                if (str2.Equals(this.startKey))
                {
                    str1 = "";
                }
                else
                {
                    if (str2.Equals(this.stopKey))
                        return str1;
                    str1 = str1 + str2 + Environment.NewLine;
                }
            }
            return str1;
        }
    }
}

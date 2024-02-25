using System;
using Onix.OnixHttpClient;

namespace Onix.Client.Model
{
    public class MChunkNavigate : MBaseModel
    {
        private String pageno = "";

        public MChunkNavigate(CTable obj) : base(obj)
        {

        }

        public void CreateDefaultValue()
        {
        }

        public String PageNo
        {
            get
            {
                return (pageno);
            }

            set
            {
                pageno = value;
            }
        }
    }
}

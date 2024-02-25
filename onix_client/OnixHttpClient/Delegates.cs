using Onix.OnixHttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.OnixHttpClient
{
    public delegate void SSEMessageCopleted(CTable param, CTable data);
    public delegate void SSEMessageUpdate(CTable param, CTable data);
}

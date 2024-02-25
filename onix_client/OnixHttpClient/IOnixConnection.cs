using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Onix.OnixHttpClient
{
    public interface IOnixConnection
    {
        APIResult SubmitCommand(CTable cmd, string functionName);

        APIResult Login(CTable user);

        int GetDownloadingFileSize();

        CTable Download(
          CTable user,
          UploadProgressChangedEventHandler prog,
          UploadValuesCompletedEventHandler comp);

        void DownloadFileAsync(
          string srcFile,
          string outputFile,
          DownloadProgressChangedEventHandler prog,
          AsyncCompletedEventHandler comp);

        void UploadFileAsync(
          string srcFile,
          UploadProgressChangedEventHandler prog,
          UploadFileCompletedEventHandler comp);

        bool SubmitCommandSSE(
          CTable cmd,
          string functionName,
          SSEMessageUpdate prog,
          SSEMessageCopleted comp);
    }
}

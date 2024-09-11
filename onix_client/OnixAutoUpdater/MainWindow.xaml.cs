using System;
using System.Net.Security;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;

namespace OnixAutoUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IComponentConnector
    {
        private string webUrl = "";
        private Exception lastError = (Exception)null;
        private string cwd = Directory.GetCurrentDirectory();
        private bool isDownloadDone = false;
        private string dlFile = "";
        private Thread dlThread = (Thread)null;
        private Thread mnThread = (Thread)null;
        private string postProgram = "";

        public MainWindow()
        {
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)((_param1, _param2, _param3, _param4) => true);
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length < 3)
            {
                int num = (int)MessageBox.Show("Argument error !!!", "Updater", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                Close();

                return;
            }

            webUrl = commandLineArgs[1];
            postProgram = commandLineArgs[2];

            InitializeComponent();
        }

        public virtual void DownloadFileAsync(string outputFile, DownloadProgressChangedEventHandler prog, AsyncCompletedEventHandler comp)
        {
            this.lastError = (Exception)null;
            Uri address = new Uri(this.webUrl);
            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += prog;
            webClient.DownloadFileCompleted += comp;
            webClient.DownloadFileAsync(address, outputFile);
        }


        private void downloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100.0;
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.Title = string.Format("Downloading... {0,0:0,000} from {1,0:0,000} ({2,0:#.00}%)", (object)bytesIn, (object)totalBytes, (object)percentage);
                this.prgValue.Value = (double)int.Parse(Math.Truncate(percentage).ToString());
            }));
        }

        private void downloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.lastError = e.Error;
                this.prgValue.Value = 100.0;
                this.isDownloadDone = true;
            }));
        }

        private void extractZipFile(string zipPath)
        {
/*
            ZipArchive zipArchive = ZipFile.OpenRead(zipPath);
            int cnt = 0;
            foreach (ZipArchiveEntry entry1 in zipArchive.Entries)
            {
                ZipArchiveEntry entry = entry1;
                cnt++;
                this.Dispatcher.Invoke((Action)(() => this.txtStatus.Text = string.Format("Extracting file [{0}]...", (object)entry.FullName)));
                string str = Path.Combine(this.cwd, Path.GetFileName(entry.FullName));
                if (System.IO.File.Exists(str))
                    System.IO.File.Delete(str);
                entry.ExtractToFile(str);
            }
            this.Dispatcher.Invoke((Action)(() => this.txtStatus.Text = string.Format("Done extracted {0} file(s)", (object)cnt)));
*/
        }

        private void fileDownload()
        {
            this.dlFile = string.Format("{0}\\{1}", (object)this.cwd, (object) System.IO.Path.GetFileName(this.webUrl));
            this.DownloadFileAsync(this.dlFile, new DownloadProgressChangedEventHandler(this.downloadProgressChanged), new AsyncCompletedEventHandler(this.downloadFileCompleted));
        }

        private void jobMonitor()
        {
            this.isDownloadDone = false;
            this.dlThread = new Thread(new ThreadStart(this.fileDownload));
            this.dlThread.Start();
            do
                ;
            while (!this.isDownloadDone);

            if (this.lastError != null)
            {
                int num = (int)MessageBox.Show(string.Format("Download error [{0}]!!!", (object)this.lastError.ToString()), "Updater", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                this.extractZipFile(this.dlFile);
                this.Dispatcher.Invoke((Action)(() => this.Title = "Program successfully updated"));
                Process.Start(this.postProgram);
                Process.GetCurrentProcess().Kill();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.dlThread != null)
                this.dlThread.Abort();

            if (this.mnThread == null)
                return;

            this.mnThread.Abort();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.mnThread = new Thread(new ThreadStart(this.jobMonitor));
            this.mnThread.Start();
        }
    }
}

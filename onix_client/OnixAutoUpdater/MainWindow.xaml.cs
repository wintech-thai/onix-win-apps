using System;
using System.Net;
using System.Windows;
using System.Windows.Markup;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Windows.Threading;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace OnixAutoUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window, IComponentConnector
    {
        private string webUrl = "";
        private string env = "";
        private Exception lastError = (Exception)null;
        private string cwd = Directory.GetCurrentDirectory();
        private bool isDownloadDone = false;
        private string dlFile = "";
        private Thread dlThread = (Thread)null;
        private Thread mnThread = (Thread)null;
        private string postProgram = "";

        private double totalFileSize = 15 * 1024 * 1024; // 15 MB
        private double totalDownloaded = 0;
        private bool isLocalMode = true;

        public MainWindow()
        {
            //ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)((_param1, _param2, _param3, _param4) => true);
            webUrl = "https://storage.googleapis.com/public-software-download/onix";
            postProgram = "OnixClientCenter.exe";
            env = "dev";

            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length >= 4)
            {
                webUrl = commandLineArgs[1];
                postProgram = commandLineArgs[2];
                env = commandLineArgs[3];

                isLocalMode = false;
            }

            InitializeComponent();
        }

        public virtual void DoDownloadFileAsync(string fileUrl, string outputFile, DownloadProgressChangedEventHandler prog, AsyncCompletedEventHandler comp)
        {
            this.lastError = (Exception) null;
            Uri address = new Uri(fileUrl);

            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += prog;
            webClient.DownloadFileCompleted += comp;
            webClient.DownloadFileAsync(address, outputFile);
        }

        private void downloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            totalDownloaded = totalDownloaded + bytesIn;

            double totalBytes = totalFileSize; // double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100.0;

            this.Dispatcher.Invoke((Action)(() =>
            {
                Title = string.Format("Downloading... {0,0:0,000} from {1,0:0,000} ({2,0:#.00}%)", (object)bytesIn, (object)totalBytes, (object)percentage);
                var value = (double) int.Parse(Math.Truncate(percentage).ToString());

                prgValue.Dispatcher.Invoke(() => prgValue.Value = value, DispatcherPriority.Background);
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

        [Obsolete]
        private void extractZipFile(string zipPath)
        {
            var targetDir = this.cwd;
            if (isLocalMode)
            {
                targetDir = @"D:\tmp";
            }

            try
            {
                FileInfo tarFileInfo = new FileInfo(zipPath);
                DirectoryInfo targetDirectory = new DirectoryInfo(targetDir);

                if (!targetDirectory.Exists)
                {
                    targetDirectory.Create();
                }

                using (Stream sourceStream = new GZipInputStream(tarFileInfo.OpenRead()))
                {
                    using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(sourceStream, TarBuffer.DefaultBlockFactor))
                    {
                        tarArchive.ExtractContents(targetDirectory.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} - [{zipPath}]", "Updater", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void fileDownload()
        {
            var latestRelease = getLatestRelease();
            var fileUrl = $"{webUrl}/{env}/OnixClientCenter-x64_{latestRelease}.tar.gz"; ;

            dlFile = string.Format("{0}\\{1}", (object) this.cwd, (object) System.IO.Path.GetFileName(fileUrl));
            DoDownloadFileAsync(fileUrl, dlFile, 
                new DownloadProgressChangedEventHandler(downloadProgressChanged), 
                new AsyncCompletedEventHandler(downloadFileCompleted));
        }

        private string getLatestRelease()
        {
            //https://storage.googleapis.com/public-software-download/onix/dev/latest-release.txt
            var randomStr = Guid.NewGuid().ToString("n").Substring(0, 8); //To escape cache

            HttpClient httpClient = new HttpClient();
            var releaseInfoUrl = $"{webUrl}/{env}/latest-release.txt?{randomStr}";

            var t = httpClient.GetStringAsync(releaseInfoUrl);
            var releaseVersion = t.Result;


            return releaseVersion.Trim();
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
                MessageBox.Show(string.Format("Download error [{0}]!!!", (object)this.lastError.ToString()), "Updater", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                //this.extractZipFile(this.dlFile);
                this.Dispatcher.Invoke((Action)(() => this.Title = "Program successfully updated"));

                try
                {
                    Process.Start(postProgram);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message} - [{postProgram}]", "Updater", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }

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

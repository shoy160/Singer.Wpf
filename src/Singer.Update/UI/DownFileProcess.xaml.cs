using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using ICCEmbedded.SharpZipLib.Zip;

namespace Singer.Update.UI
{
    public partial class DownFileProcess
    {
        private readonly string _updateFileDir;//更新文件存放的文件夹
        private readonly string _appDir;
        private readonly string _downloadUrl;

        public DownFileProcess(string callExeName, string downloadUrl,
            string appVersion, string desc, string md5)
        {
            InitializeComponent();
            _appDir = AppDomain.CurrentDomain.BaseDirectory;
            _updateFileDir = Path.Combine(_appDir, "Update", md5);
            _downloadUrl = downloadUrl;

            YesButton.Focus();

            Loaded += (sl, el) =>
            {

                YesButton.Click += (sender, e) =>
                {
                    YesButton.IsEnabled = false;
                    NoButton.IsEnabled = false;

                    var processes = Process.GetProcessesByName(callExeName);

                    if (processes.Length > 0)
                    {
                        foreach (var p in processes)
                        {
                            p.Kill();
                        }
                    }

                    DownloadUpdateFile();
                };

                NoButton.Click += (sender, e) => Close();

                TxtProcess.Text = $"发现新的版本({appVersion}) 是否现在更新?";
                TxtDes.Text = desc;
            };
            MouseLeftButtonDown += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            };
        }

        /// <summary> 下载文件 </summary>
        private void DownloadUpdateFile()
        {
            var fileName = Path.GetFileName(_downloadUrl);
            if (string.IsNullOrWhiteSpace(fileName))
                return;
            var client = new WebClient();
            client.DownloadProgressChanged += (sender, e) => UpdateProcess(e.BytesReceived, e.TotalBytesToReceive);
            client.DownloadDataCompleted += (sender, e) =>
            {
                if (!Directory.Exists(_updateFileDir))
                {
                    Directory.CreateDirectory(_updateFileDir);
                }
                var installPath = Path.Combine(_updateFileDir, fileName);
                var data = e.Result;
                var writer = new BinaryWriter(new FileStream(installPath, FileMode.OpenOrCreate));
                writer.Write(data);
                writer.Flush();
                writer.Close();

                ThreadPool.QueueUserWorkItem(s =>
                {
                    Dispatcher.Invoke(new Action(() => { TxtProcess.Text = "开始更新程序..."; }));
                    Thread.Sleep(300);
                    try
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            //覆盖安装软件
                            var info = new ProcessStartInfo(installPath)
                            {
                                UseShellExecute = true,
                                WorkingDirectory = _appDir
                            };
                            Process.Start(info);
                            Close();
                        }));
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                });

            };
            client.DownloadDataAsync(new Uri(_downloadUrl));
            BProcess.Background = Brushes.White;
        }

        private static void UnZipFile(string zipFilePath, string targetDir)
        {
            var fz = new FastZip(new FastZipEvents());
            fz.ExtractZip(zipFilePath, targetDir, "");
        }

        public void UpdateProcess(long current, long total)
        {
            var percent = (int)((float)current * 100 / total);
            TxtProcess.Text = $"{percent}%";
            RectProcess.Width = ((float)current / total) * BProcess.ActualWidth;
            if (percent >= 48)
                TxtProcess.Foreground = Brushes.White;
        }

        public void CopyDirectory(string sourceDirName, string destDirName)
        {
            try
            {
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                    File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
                }
                if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
                    destDirName = destDirName + Path.DirectorySeparatorChar;
                var files = Directory.GetFiles(sourceDirName);
                foreach (var file in files)
                {
                    File.Copy(file, destDirName + Path.GetFileName(file), true);
                    File.SetAttributes(destDirName + Path.GetFileName(file), FileAttributes.Normal);
                }
                var dirs = Directory.GetDirectories(sourceDirName);
                foreach (var dir in dirs)
                {
                    CopyDirectory(dir, destDirName + Path.GetFileName(dir));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("复制文件错误");
            }
        }
    }
}

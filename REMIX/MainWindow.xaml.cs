using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using REMIX.Laeng;

namespace REMIX
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private String BaseCamp;
        private LocalEnvironmentFile LEF;

        public MainWindow()
        {
            LEF = new LocalEnvironmentFile(this);
            BaseCamp = LEF.Get("local");
            String conf = Path.Combine(BaseCamp, "server.conf");

            if (File.Exists(conf)) File.Delete(conf);
            File.Move(Path.Combine(Directory.GetCurrentDirectory(), "server.conf"), conf);

            if (!String.IsNullOrEmpty(BaseCamp) || File.Exists(BaseCamp))
            {
                Hide();
                Process.Start(Path.Combine(BaseCamp, "MiNET.Service.exe"));
                Task.Run(() =>
                {
                    while (true)
                    {
                        Process[] process = Process.GetProcessesByName("MiNET.Service");
                        if (process.Length == 0)
                        {
                            Process.GetCurrentProcess().Kill();
                        }

                        Thread.Sleep(200);
                    }
                });

            }

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(BaseCamp))
            {
                SendSingleMessage("Specify the folder that contains the MiNET.Service.exe");
                return;
            }
            Process.Start(Path.Combine(LEF.Get("local"), "MiNET.Service.exe"));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog d = new System.Windows.Forms.FolderBrowserDialog();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BaseCamp = d.SelectedPath;
                
                LEF.Set("local", BaseCamp);
                
            }
        }

        internal void SendSingleMessage(String message, MessageBoxImage mbi = MessageBoxImage.Error)
        {
            MessageBox.Show(message, "Project REMiX", MessageBoxButton.OK, mbi);
        }
    }
}

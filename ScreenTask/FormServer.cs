
using ExtendCSharp.Classes;
using ExtendCSharp.ExtendedClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendCSharp;
using ExtendCSharp.Services;
using System.Reflection;
using ScreenTask.Classes;
using CommonLib;
using CommonLib.Enums;

namespace ScreenTask
{


    public partial class FormServer : Form
    {
        private bool isWorking;             //flag -> gestione del thread del server
        private bool isTakingScreenshots;   //flag -> se deve fare gli screen
        private bool isPreview;             //flag -> visualizzare la preview nella GUI
        private bool isMouseCapture;        //flag -> cattura del mouse negli screen

        

        Bitmap LastBitmap = null;           //Ultima immagine ottenuta
        JPG LastJpeg = null;                
        bool JpegToSend = false;            //flag per ottimizzare l'invio ( se l'immagine è duplicata -> false = NON INVIO )

        uint JPGQuality;                    //parametri per l'invio dell'immagine
        uint SleepMSecond;                  

        ISenderServices sender;             //Servizio per la gestione degli invii  TCP o MULTICAST
        HTTP_Downloader http;               //server per la gestione del download del client

        public FormServer()
        {
            InitializeComponent();
            
            isPreview = false;
            isMouseCapture = true;


            ServicesManager.RegistService(new NetworkService());
            ServicesManager.RegistService(new SystemService());  
            ServicesManager.RegistService(new ResourcesService(System.Reflection.Assembly.GetExecutingAssembly()));

            Common.Log = Log;
            if(CommonSetting.sendingProtocol==SendingProtocol.Multicast)
            {
                sender = new Sender_Multicast();
                this.Text += " - Multicast";
            }
            else if(CommonSetting.sendingProtocol==SendingProtocol.TCP)
            {
                sender = new Sender_TCP();
                this.Text += " - TCP";
            }
            http = new HTTP_Downloader();
        }

        private async void btnStartServer_Click(object sender, EventArgs e)
        {
            if (btnStartServer.Tag.ToString() != "start")           //STOP
            {
                btnStartServer.Tag = "start";
                btnStartServer.Text = "Start Server"; 
                StopServer();
                return;
            }

            try
            {
                isTakingScreenshots = true;
                isWorking = true;
                Log("Starting Server, Please Wait...");
                SleepMSecond = (uint)numShotEvery.Value;
                await AddFirewallRule((int)numPort.Value);
                Task.Factory.StartNew(() => CaptureScreenEvery()).Wait();
                http.Start();
                btnStartServer.Tag = "stop";
                btnStartServer.Text = "Stop Server";
                await SendingLoop();

            }
            catch (Exception ex)
            {
                Log("Error! : " + ex.Message);
            }
        }


     
        private async Task SendingLoop()
        {
            JPGQuality = (uint)trackBar1.Value;
            String selectedIP = comboIPs.SelectedItem._Cast<ComboIp>().Address;
            int Port = (int)numPort.Value;

            if (CommonSetting.sendingProtocol == SendingProtocol.Multicast)
            {
                sender.Setup(0, Port, selectedIP);
            }
            else if (CommonSetting.sendingProtocol == SendingProtocol.TCP)
            {
                sender.Setup(Port, 0, selectedIP);
            }


            sender.Start();
            while (isWorking)
            {
                if (LastJpeg == null)
                {
                    await Task.Delay(1);
                    continue;
                }
                else if (!JpegToSend)
                {
                    await Task.Delay((int)SleepMSecond / 2);
                    continue;
                }
                sender.Send(LastJpeg.data);
                await Task.Delay((int)SleepMSecond);
            }

        }


       
        private void StopServer()
        {
            isWorking = false;
            isTakingScreenshots = false;
            http.Stop();
            sender.Stop();
            Log("Server Stoped.");
        }
       


        private async Task CaptureScreenEvery()
        {
            while (isWorking)
            {
                if (isTakingScreenshots)
                {
                    TakeScreenshot(isMouseCapture);
                    await Task.Delay((int)SleepMSecond);
                }
            }
        }
        
        private void TakeScreenshot(bool captureMouse)
        {
            Bitmap newBitmap;

            if (captureMouse)
            {
                newBitmap = ScreenCapturePInvoke.CapturePrimaryScreen(true);
                
            }
            else
            {
                
                Rectangle bounds = Screen.GetBounds(Point.Empty);
                newBitmap = new Bitmap(bounds.Width,bounds.Height);
                using (Graphics g = Graphics.FromImage(newBitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
            }

            if(!newBitmap.EqualMemCmp(LastBitmap))           
            {
                if (LastBitmap != null)
                    LastBitmap.Dispose();
                LastBitmap = newBitmap;
                JpegToSend = true;

                LastJpeg = new JPG(LastBitmap, JPGQuality);
                if (isPreview)
                {
                    jpgPreview.jpg = LastJpeg;
                }
            }
            else
            {
                JpegToSend = false;
            }

        }
    
        private Task AddFirewallRule(int port)
        {
            return Task.Run(() =>
            {

                string cmd = RunCMD("netsh advfirewall firewall show rule \"Screen Task\"");
                if (cmd.StartsWith("\r\nNo rules match the specified criteria."))
                {
                    cmd = RunCMD("netsh advfirewall firewall add rule name=\"Screen Task\" dir=in action=allow remoteip=localsubnet protocol=tcp localport=" + port);
                    if (cmd.Contains("Ok."))
                    {
                        Log("Screen Task Rule added to your firewall");
                    }
                }
                else
                {
                    cmd = RunCMD("netsh advfirewall firewall delete rule name=\"Screen Task\"");
                    cmd = RunCMD("netsh advfirewall firewall add rule name=\"Screen Task\" dir=in action=allow remoteip=localsubnet protocol=tcp localport=" + port);
                    if (cmd.Contains("Ok."))
                    {
                        Log("Screen Task Rule updated to your firewall");
                    }
                }
            });

        }
        private string RunCMD(string cmd)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = "/C " + cmd;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();
            string res = proc.StandardOutput.ReadToEnd();
            proc.StandardOutput.Close();

            proc.Close();
            return res;
        }
        private void Log(string text)
        {
            txtLog.AppendTextInvoke(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " : " + text + "\r\n");
        }

        private void cbPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPreview.Checked == true)
            {
                isPreview = true;
            }
            else
            {
                isPreview = false;
            }
        }

        private void cbCaptureMouse_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCaptureMouse.Checked)
            {
                isMouseCapture = true;
            }
            else
            {
                isMouseCapture = false;
            }
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            List<ComboIp> _ips = ServicesManager.Get< NetworkService>().GetAllIPv4Addresses();
            foreach (var ip in _ips)
                comboIPs.Items.Add(ip);
            
            comboIPs.SelectedIndex = comboIPs.Items.Count - 1;
        }



        private void cbScreenshotEvery_CheckedChanged(object sender, EventArgs e)
        {
            if (cbScreenshotEvery.Checked)
            {
                isTakingScreenshots = true;
            }
            else
            {
                isTakingScreenshots = false;
            }
        }





        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopServer();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            JPGQuality = (uint)trackBar1.Value;
        }

        private void numShotEvery_ValueChanged(object sender, EventArgs e)
        {
            SleepMSecond = (uint)numShotEvery.Value;
        }


        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start("https://github.com/Rarder44/ScreenTask");
        }

        private void jpgPreview_DoubleClick(object sender, EventArgs e)
        {
            if(jpgPreview.Dock == DockStyle.None)
            {
                jpgPreview.Dock = DockStyle.Fill;
            }
            else
            {
                jpgPreview.Dock = DockStyle.None;
            }
        }
    }
}

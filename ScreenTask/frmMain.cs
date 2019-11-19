using Common;
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
namespace ScreenTask
{
    public partial class frmMain : Form
    {
        private bool isWorking;
        private bool isTakingScreenshots;
        private bool isPrivateTask;
        private bool isPreview;
        private bool isMouseCapture;

        private object locker = new object();
        private ReaderWriterLock rwl = new ReaderWriterLock();
        private MemoryStream img;
        private List<Tuple<string, string>> _ips;
        HttpListener serv;


        List<TcpClientPlus> Clients = new List<TcpClientPlus>();
        TcpListenerPlus Listener = null;


        Bitmap LastBitmap = null;
        JPG LastJpeg = null;
        bool JpegToSend = false;

        uint JPGQuality;
        uint SleepMSecond;

        public frmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false; // For Visual Studio Debuging Only !
            serv = new HttpListener();
            serv.IgnoreWriteExceptions = true; // Seems Had No Effect :(
            img = new MemoryStream();
            isPrivateTask = false;
            isPreview = false;
            isMouseCapture = false;
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


                serv.IgnoreWriteExceptions = true;
                isTakingScreenshots = true;
                isWorking = true;
                Log("Starting Server, Please Wait...");
                SleepMSecond = (uint)numShotEvery.Value;
                await AddFirewallRule((int)numPort.Value);
                Task.Factory.StartNew(() => CaptureScreenEvery()).Wait();
                btnStartServer.Tag = "stop";
                btnStartServer.Text = "Stop Server";
                await StartServer();

            }
            catch (ObjectDisposedException disObj)
            {
                serv = new HttpListener();
                serv.IgnoreWriteExceptions = true;
            }
            catch (Exception ex)
            {
                Log("Error! : " + ex.Message);
            }
        }
    
        private async Task StartServer()
        {
           
            String selectedIP = _ips.ElementAt(comboIPs.SelectedIndex).Item2;
            int Port = (int)numPort.Value;

            Clients.Clear();

            Listener = new TcpListenerPlus(selectedIP, Port);
            Listener.ClientConnected += Listener_ClientConnected;
            Listener.Start();

            JPGQuality = (uint)trackBar1.Value;

            Log("Server Started Successfuly!");
            Log("Private Network Socket : " + selectedIP+":"+ Port);
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

                DataPacket dp = new DataPacket();
                dp.Data = LastJpeg.data;

                

                foreach (TcpClientPlus client in Clients.ToArray())
                {
                    if (!client.Connected)
                        Client_disconnected(client);
                    else
                    {
                        try
                        {
                            await dp.SerializeToStream(client.GetStream());
                        }
                        catch(Exception ex)
                        {
                            //errore nel'invio, client disconnesso -> rimuovo il client 
                            Client_disconnected(client);
                        }
                    }
                                 

                    //INVIARE I DATI IN MANIERA ASINCRONA!!!
                }
                await Task.Delay((int)SleepMSecond);
            }

        }
        private void StopServer()
        {
            isWorking = false;
            isTakingScreenshots = false;
            if (Listener != null)
                Listener.Stop();
            foreach (TcpClientPlus client in Clients)
                client.Close();
            Log("Server Stoped.");
        }
        private void Listener_ClientConnected(TcpClientPlus client)
        {
            Clients.Add(client);
        }

        private void Client_disconnected(TcpClientPlus client)
        {
            Clients.Remove(client);
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
                newBitmap = ScreenCapturePInvoke.CaptureFullScreen(true);
                
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
                if (isPreview)
                {
                    imgPreview.Image = LastBitmap;
                }
            }
            else
            {
                JpegToSend = false;
            }


            

            LastJpeg = new JPG(LastBitmap, JPGQuality);
            



            
        }
        private string GetIPv4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }
        private List<Tuple<string, string>> GetAllIPv4Addresses()
        {
            List<Tuple<string, string>> ipList = new List<Tuple<string, string>>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {

                foreach (var ua in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipList.Add(Tuple.Create(ni.Name, ua.Address.ToString()));
                    }
                }
            }
            return ipList;
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
            txtLog.Text += DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " : " + text + "\r\n";

        }



        private void cbPrivate_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPrivate.Checked == true)
            {
                txtUser.Enabled = true;
                txtPassword.Enabled = true;
                isPrivateTask = true;
            }
            else
            {
                txtUser.Enabled = false;
                txtPassword.Enabled = false;
                isPrivateTask = false;
            }
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
                imgPreview.Image = imgPreview.InitialImage;
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
            _ips = GetAllIPv4Addresses();
            foreach (var ip in _ips)
            {
                comboIPs.Items.Add(ip.Item2 + " - " + ip.Item1);
            }
            comboIPs.SelectedIndex = comboIPs.Items.Count - 1;
        }

        private void imgPreview_Click(object sender, EventArgs e)
        {
            if (imgPreview.Dock == DockStyle.None)
            {
                imgPreview.Dock = DockStyle.Fill;
            }
            else
            {
                imgPreview.Dock = DockStyle.None;
            }
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




        private void lblGithub_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Rarder44/ScreenTask");
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            isWorking = false;
            if(Listener!=null)
                Listener.Stop();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            JPGQuality = (uint)trackBar1.Value;
        }

        private void numShotEvery_ValueChanged(object sender, EventArgs e)
        {
            SleepMSecond = (uint)numShotEvery.Value;
        }
    }
}

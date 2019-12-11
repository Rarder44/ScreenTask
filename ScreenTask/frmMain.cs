
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

namespace ScreenTask
{

    /* 
     TODO:
     - la porta non è decisa dalla textbox 
     - dividere meglio quello che fa il bottone start e la StartServer
     - creare task dedicati per la gestione dell'invio dei dati e per il web server

         */
    public partial class frmMain : Form
    {
        private bool isWorking;
        private bool isTakingScreenshots;
        private bool isPreview;
        private bool isMouseCapture;

        
        private MemoryStream img;
        private List<Tuple<string, string>> _ips;
        HttpListener serv;
        String MulticastAddress = "224.168.100.2";

        Bitmap LastBitmap = null;
        JPG LastJpeg = null;
        bool JpegToSend = false;

        uint JPGQuality;
        uint SleepMSecond;

        public frmMain()
        {
            InitializeComponent();
            img = new MemoryStream();
            isPreview = false;
            isMouseCapture = true;


            ServicesManager.RegistService(new NetworkService());
            ServicesManager.RegistService(new SystemService());
            
            ServicesManager.RegistService(new ResourcesService(System.Reflection.Assembly.GetExecutingAssembly()));
        }

        MulticastClient c;
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

                serv = new HttpListener();
                serv.IgnoreWriteExceptions = true;
                isTakingScreenshots = true;
                isWorking = true;
                Log("Starting Server, Please Wait...");
                SleepMSecond = (uint)numShotEvery.Value;
                await AddFirewallRule((int)numPort.Value);
                Task.Factory.StartNew(() => CaptureScreenEvery()).Wait();
                Task.Factory.StartNew(() => WebServer()).Wait();
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
            c = new MulticastClient(MulticastAddress, Port, selectedIP, false);
            c.JoinMulticast(true);
            
            JPGQuality = (uint)trackBar1.Value;

            Log("Server Started Successfuly!");
            Log("Multicast socket : " + MulticastAddress+":"+ Port);
            Log("Output interface: " + selectedIP);
            
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

               
                c.SendGroup(LastJpeg.data,(ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds());

                
                 
                await Task.Delay((int)SleepMSecond);
            }

        }


        private async Task WebServer(int portWebServer = 80)
        {
            serv.Prefixes.Clear();
            string Address = "http://*:" + portWebServer + "/";
            serv.Prefixes.Add(Address);
            Log("Download address "+ Address);
            serv.Start();

           

            while (isWorking)
            {
                var ctx = await serv.GetContextAsync();


                //qualsiasi richiesta la ridireziono sul download del file
                FilePlus client = ServicesManager.Get<ResourcesService>().GetObject<FilePlus>("Client.exe");
                client.Extension = ".exe";
                client.Name = "Client";


                ctx.Response.ContentType = "application/octet-stream";
                ctx.Response.AddHeader("Content-disposition", "attachment; filename=Client.exe");

                ctx.Response.StatusCode = 200;
                try
                {

                    //TODO: mi sa che non devo fare await
                    await ctx.Response.OutputStream.WriteAsync(client.data, 0,  client.data.Length);
                }
                catch (Exception ex)
                {
                  
                        //Do Nothing !!! this is the Only Effective Solution for this Exception : the specified network name is no longer available
                }

                ctx.Response.Close();
            }

            serv.Stop();
        }
        private void StopServer()
        {
            isWorking = false;
            isTakingScreenshots = false;
            if(serv!=null)
                serv.Close();
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
            }
            else
            {
                JpegToSend = false;
            }



            LastJpeg = new JPG(LastBitmap, JPGQuality);
            if (isPreview)
            {
                jpgPreview.jpg = LastJpeg;
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
            _ips = ServicesManager.Get< NetworkService>().GetAllIPv4Addresses();
            foreach (var ip in _ips)
            {
                comboIPs.Items.Add(ip.Item2 + " - " + ip.Item1);
            }
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




        private void lblGithub_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Rarder44/ScreenTask");
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

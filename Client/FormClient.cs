
using ExtendCSharp.ExtendedClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendCSharp;
using ExtendCSharp.Classes;
using System.Diagnostics;
using ExtendCSharp.Services;
using Client.Classes;

namespace Client
{

    /* TODO:
     -Creo una classe per gestire gli indirizzi IP visualizzati nella combo box, 
     in modo da recuperare il selectedItem e prendere direttamente da li l'indirizzo ( senza usare la _ips) 
     

     */
    public partial class FormClient : Form
    {
        bool Connesso = false;
        int Frame = 0;

        Control IpControl;
            
        


        IReceiverServices Receiver;

        public FormClient()
        {
            InitializeComponent();
            ExtendCSharp.Services.ServicesManager.RegistService(new ExtendCSharp.Services.NetworkService());

            if (CommonLib.CommonSetting.sendingProtocol == CommonLib.Enums.SendingProtocol.Multicast)
            {
                Receiver = new Receiver_Multicast();
                this.Text += " - Multicast";
                IpControl = comboIPs;
                textBoxIP.Hide();
            }
            else if (CommonLib.CommonSetting.sendingProtocol == CommonLib.Enums.SendingProtocol.TCP)
            {
                Receiver = new Receiver_TCP();
                this.Text += " - TCP";
                IpControl = textBoxIP;
                comboIPs.Hide();
                textBoxIP.Location = comboIPs.Location;
                textBoxIP.Text = "127.0.0.1";
            }

            Receiver.OnDataReceived += C_onReceivedByte;
            Receiver.OnError += Receiver_OnError;
           Common.Log = (String s) =>
            {
                toolStripStatusLabel1.SetTextInvoke(s);
            };

        }

        private void Receiver_OnError(object sender, EventArgs e)
        {
            StopConnection();
        }

        private void C_onReceivedByte(byte[] data, System.Net.EndPoint remoteEP)
        {
            JPG j = new JPG(data);
            jpgPanel1.jpg = j;
            Frame++;    
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            if (Connesso)
                StopConnection();
            else
                StartConnection();
        }

        private void StartConnection()
        {
            try
            {
                Common.Log("Connessione in corso...");
                EnableGUI(false);
              

                if (CommonLib.CommonSetting.sendingProtocol == CommonLib.Enums.SendingProtocol.Multicast)
                {
                    string intfIP = ((ComboIp)comboIPs.SelectedItem).Address;
                    int Port = (int)numeric_Port.Value;
                   
                    Receiver.Setup(intfIP, Port, null, null);
                }
                else if (CommonLib.CommonSetting.sendingProtocol == CommonLib.Enums.SendingProtocol.TCP)
                {
                    string intfIP = textBoxIP.Text;
                    int Port = (int)numeric_Port.Value;

                    Receiver.Setup(null,null,intfIP, Port);
                }
                Connesso = true;


                Receiver.Start();

                new Task(TaskFPS).Start();
                Common.Log("Connessione effettuata");

            }
            catch (Exception ex)
            {
                Common.Log("Impossibile stabilire la connessione");
                EnableGUI(true);
            }
        }

        bool IsAlreadyStopped = false;
        
        private void StopConnection()
        {
            if (IsAlreadyStopped)
            {
                IsAlreadyStopped = false;
            }
            else
            {
                IsAlreadyStopped = true;
                Connesso = false;
                Receiver.Stop();
                Common.Log("Connessione interrotta");
                EnableGUI(true);
            }
        }





        private void TaskFPS()
        {
            System.Timers.Timer tt = new System.Timers.Timer();
            tt.Elapsed += async (sender, e) =>
            {
                if (!Connesso)
                    tt.Stop();


                this.SetTextInvoke(Frame + " FPS");
                Frame = 0;
            };
            tt.Interval = 1000;
            tt.Start();       
        }

     

        private void EnableGUI(bool enable)
        {
            if (enable)
                btnConnection.SetTextInvoke( "Connect");
            else
                btnConnection.SetTextInvoke("Disconnect");

            numeric_Port.SetEnableInvoke(enable);
            IpControl.SetEnableInvoke(enable);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopConnection();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            if (CommonLib.CommonSetting.sendingProtocol == CommonLib.Enums.SendingProtocol.Multicast)
            {
                List<ComboIp> _ips = ServicesManager.Get<NetworkService>().GetAllIPv4Addresses();
                foreach (var ip in _ips)
                    comboIPs.Items.Add(ip);

                comboIPs.SelectedIndex = comboIPs.Items.Count - 1;
            }
        }


     
        bool OnFullScreen = false;

        private void jpgPanel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (OnFullScreen)
                SetOldState();
            else
                FullScreen();  
        }


        private void FullScreen()
        {
            //salvo lo stato corrente
            jpgPanel1.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
            statusStrip1.Hide();
            IpControl.Hide();
            OnFullScreen = true;
        }
        private void SetOldState()
        {
            this.TopMost = false;
            statusStrip1.Show();
            IpControl.Show();
            this.FormBorderStyle = FormBorderStyle.Sizable;
            jpgPanel1.Dock = DockStyle.None;
            ResetJpgPanelSize();
            jpgPanel1.Anchor=AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
  
            OnFullScreen = false;
        }

        private void ResetJpgPanelSize()
        {
            int BaseHeight = 35;
            jpgPanel1.Location = new Point(0, BaseHeight);
            jpgPanel1.Height = ClientSize.Height - BaseHeight - statusStrip1.Height;
            jpgPanel1.Width = ClientSize.Width;
        }


        bool Moviment = false;
        Point LastPoint ;
        private void jpgPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (OnFullScreen)
            {
                Moviment = true;
                LastPoint = this.PointToScreen(e.Location);
            }
        }

        private void jpgPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            Moviment = false;
        }

        private void jpgPanel1_MouseLeave(object sender, EventArgs e)
        {
            Moviment = false;
        }

        private void jpgPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if(Moviment)
            {
                Point AbsPoint = this.PointToScreen(e.Location);
                Point delta = AbsPoint.Sub(LastPoint);
                //Console.WriteLine(AbsPoint + " "+delta);
                this.Location = this.Location.Add(delta);
                LastPoint = AbsPoint;
            }
        }
    }
}

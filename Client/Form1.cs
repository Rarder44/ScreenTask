
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

namespace Client
{

    /* TODO:
     -Creo una classe per gestire gli indirizzi IP visualizzati nella combo box, 
     in modo da recuperare il selectedItem e prendere direttamente da li l'indirizzo ( senza usare la _ips) 
     

     */
    public partial class Form1 : Form
    {
        bool Connesso = false;
        int Frame = 0;
        String MulticastAddress = "224.168.100.2";
        MulticastClient c;
        List<Tuple<string, string>> _ips;


        public Form1()
        {
            InitializeComponent();
            ExtendCSharp.Services.ServicesManager.RegistService(new ExtendCSharp.Services.NetworkService());

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


        private void StopConnection()
        {
            Connesso = false;
            if (c != null)
            {
                c.StopListener();
                c.Dispose();
            }
            toolStripStatusLabel1.SetTextInvoke("Connessione interrotta");
            EnableGUI(true);
        }

        private void StartConnection()
        {
            try
            {
                toolStripStatusLabel1.SetTextInvoke("Connessione in corso...");
                EnableGUI(false);

                string intfIP = _ips.ElementAt(comboIPs.SelectedIndex).Item2;
                int Port = (int)numeric_Port.Value;
                Connesso = true;

                if (c != null)
                    c.Dispose();

                c = new MulticastClient(MulticastAddress, Port, intfIP);
                c.onReceivedByte += C_onReceivedByte;
                c.StartListen();

                new Task(TaskFPS).Start();
                toolStripStatusLabel1.SetTextInvoke("Connessione effettuata");
               
            }
            catch(Exception ex)
            {
                toolStripStatusLabel1.SetTextInvoke("Impossibile stabilire la connessione");
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
                btnConnection.Text = "Connect";
            else
                btnConnection.Text = "Disconnect";

            //btnConnection.SetEnableInvoke(enable);
            numeric_Port.SetEnableInvoke(enable);
            comboIPs.SetEnableInvoke(enable);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopConnection();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            _ips = ServicesManager.Get<NetworkService>().GetAllIPv4Addresses();
            foreach (var ip in _ips)
            {
                comboIPs.Items.Add(ip.Item2 + " - " + ip.Item1);
            }
            comboIPs.SelectedIndex = comboIPs.Items.Count - 1;
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

            //this.TopMost = true;
            jpgPanel1.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
            statusStrip1.Hide();
            comboIPs.Hide();
            

            OnFullScreen = true;
        }
        private void SetOldState()
        {
            this.TopMost = false;
            statusStrip1.Show();
            comboIPs.Show();
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

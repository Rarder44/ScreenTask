using Common;
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

namespace Client
{
    public partial class Form1 : Form
    {
        TcpClientPlus connection;
        bool Connesso = false;

        int Frame = 0;


       
        public Form1()
        {
            InitializeComponent();

            //TEST
            MulticastClient c = new MulticastClient("224.168.100.2", 11000);
            c.JoinMulticast();
            c.onReceivedByte += C_onReceivedByte;
            c.StartListen();
            DataPacket.onDeserializationComplete += DataPacket_onDeserializationComplete;
            //TEST
        }

        private void DataPacket_onDeserializationComplete(DataPacket dp)
        {
            
        }

        private void C_onReceivedByte(byte[] data, System.Net.EndPoint remoteEP)
        {
            DataPacket.DeserializeAddPacket(data);
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO: implemento  ricerca in rete
            StartConnection();

            

        }


        private void CheckConnection()
        {
            if( !connection.Connected)
            {
                StopConnection();       //se non c'è piu connessione, interrompo il ciclo di lettura
            }
        }

        private void StopConnection()
        {
            Connesso = false;
            toolStripStatusLabel1.SetTextInvoke("Connessione interrotta");
            EnableGUI(true);
        }
        private async void StartConnection()
        {
            try
            {
                //TODO: implemento le textbox | ricerca in rete
                toolStripStatusLabel1.SetTextInvoke("Connessione in corso...");
                EnableGUI(false);
                connection = await TcpClientPlus.Create(textBox_IP.Text, (int)numeric_Port.Value);
                connection.Closed += Connection_Closed;
                connection.StartCheckClose();
                Connesso = true;
                new Task(TaskGetImage).Start();
                new Task(TaskFPS).Start();
                toolStripStatusLabel1.SetTextInvoke("Connessione effettuata");
               
            }
            catch(Exception ex)
            {
                toolStripStatusLabel1.SetTextInvoke("Impossibile stabilire la connessione");
                EnableGUI(true);
            }
            

           
        }

        private void Connection_Closed(TcpClientPlus client, out bool ToDispose)
        {
            StopConnection();
            ToDispose = true;
        }

        /// <summary>
        /// Task che viene ripetuto per ottenere le immagini
        /// </summary>
        private void TaskGetImage()
        {
            while (Connesso)
            {
                DataPacket p = DataPacket.DeserializeFromStream(connection.GetStream());
                if (p == null)  //in caso ritorni null, ci puo essere un errore di comunicazione o la connessione è saltata
                {
                    CheckConnection(); //controllo la connessione
                }
                else
                {
                    JPG j = new JPG(p.Data);
                    jpgPanel1.jpg = j;
                    Frame++;
                }
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
            button1.SetEnableInvoke(enable);
            textBox_IP.SetEnableInvoke(enable);
            numeric_Port.SetEnableInvoke(enable);
            button2.SetEnableInvoke(!enable);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (connection != null && connection.Connected)
                connection.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.Connected)
                connection.Close();
        }
    }
}

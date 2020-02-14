using ScreenTask.Forms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendCSharp;
using System.Net;

namespace ScreenTask.Forms
{
    public partial class ConnectionsLog : Form
    {
        List<ConnectionControl> conns = new List<ConnectionControl>();
        public ConnectionsLog()
        {
            InitializeComponent();
        }

        private void ConnectionsLog_Load(object sender, EventArgs e)
        {

        }


        public void AddConnection(Connection c)
        {
            var conn = new ConnectionControl(c);
            conns.Add(conn);
            AddConnectionControlToPanel(conn);

           
        }
        private void AddConnectionControlToPanel(ConnectionControl cc)
        {
            int Y = panel1.Controls.Count * cc.Height;

            cc.SetLocationInvoke(0, Y);
            cc.SetWidthInvoke( panel1.Width - 25); //grandezza scroll bar
            panel1.AddControlInvoke(cc);
        }
        public void UpdateConnection(Connection c)
        {
            var ToEdit = conns.Where((cc) => { return cc.details==c; });
            if (ToEdit.Count() == 0)
            {
                AddConnection(c);
            }
            else
            {
                foreach (ConnectionControl con in ToEdit)
                {
                    con.details = c;
                    con.UpdateGUI();
                }
            }
        }

       
        public void RemoveConnection(Connection c)
        {
            var ToRemove=new List<ConnectionControl>(conns.Where((cc) => { return cc.details == c; }));
            //panel1.RemoveControlsInvoke(ToRemove);
            conns.Remove(ToRemove);
            CompactConnections();
            ToRemove.Clear();
            /*List<ConnectionControl> toRemove = new List<ConnectionControl>();
            foreach(ConnectionControl cc in panel1.Controls)
            {
                if (cc.details == c)
                    toRemove.Add(cc);
            }
            foreach(ConnectionControl cc in toRemove)
            {
                panel1.RemoveControlInvoke(cc);
            }

            conns.RemoveAll((cc) => { return cc.details == c; });
            
    */
        
        }

        public void CompactConnections()
        {
            panel1.ClearControlsInvoke();
            foreach(ConnectionControl cc in conns)
            {
                AddConnectionControlToPanel(cc);
            }
        }

        public void UpdateGUI()
        {
            foreach (ConnectionControl cc in conns)
                cc.UpdateGUI();
        }
    }

    public class Connection
    {
        public String IP { get; set; }
        public int Port { get; set; }

        public String Speed { get; set; }

        public Connection(IPEndPoint endPoint,string speed=""):this(endPoint.Address.ToString(), endPoint.Port,speed)
        {

        }

        public Connection(string iP, int port, string speed)
        {
            IP = iP;
            Port = port;
            Speed = speed;
        }

        public static bool operator==(Connection left,Connection right )
        {
            return left.IP == right.IP && left.Port == right.Port;
        }
        public static bool operator !=(Connection left, Connection right)
        {
            return !(left==right);
        }

    }
}

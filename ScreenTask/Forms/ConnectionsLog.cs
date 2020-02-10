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

namespace ScreenTask.Forms
{
    public partial class ConnectionsLog : Form
    {
        List<ConnectionControl> conns = new List<ConnectionControl>();
        ScrollBar vScrollBar1;
        public ConnectionsLog()
        {
            InitializeComponent();
        }

        private void ConnectionsLog_Load(object sender, EventArgs e)
        {
            vScrollBar1 = new VScrollBar();
            vScrollBar1.Dock = DockStyle.Right;
            vScrollBar1.Scroll += (sender1, e1) => { panel1.VerticalScroll.Value = vScrollBar1.Value; };
            panel1.Controls.Add(vScrollBar1);
        }


        public void AddConnection(Connection c)
        {
            var conn = new ConnectionControl(c);
            conns.Add(conn);

            int Y = panel1.Controls.Count * conn.Height;


            /*int Y = -1;
            panel1.Controls.Cast<Control>().ForEach((control) =>
            {
                if(Y==-1)
                {
                    Y=control.Location.Y*control.Height;
                }
                else
                {
                    int YTemp = control.Location.Y * control.Height;
                    if (YTemp > Y)
                        Y = YTemp;
                }
            });
            if (Y == -1)
                Y = 0;*/

            conn.Location = new Point(0, Y);
            conn.Width = panel1.Width - vScrollBar1.Width;
            panel1.AddControlInvoke(conn);

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
            List<ConnectionControl> toRemove = new List<ConnectionControl>();
            foreach(ConnectionControl cc in panel1.Controls)
            {
                if (cc.details == c)
                    toRemove.Add(cc);
            }
            foreach(ConnectionControl cc in toRemove)
            {
                panel1.Controls.Remove(cc);
            }
           

            toRemove.Clear();
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

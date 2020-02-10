using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenTask.Forms.Controls
{
    public partial class ConnectionControl : UserControl
    {
        public Connection details { get; set; }


        public ConnectionControl(Connection c)
        {
            InitializeComponent();
            details = c;
        }


        public void UpdateGUI()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(() =>{ UpdateGUI(); }));
            }
            else
            {
                IP_port_label.Text = details.IP + ":" + details.Port;
                Speed_label.Text = details.Speed;
            }
        }
    }
}

using ExtendCSharp.ExtendedClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        TcpClientPlus connection;
        public Form1()
        {
            InitializeComponent();

            //TODO: implemento le textbox | ricerca in rete
            connection = new TcpClientPlus("127.0.0.1", 7070);
            
            Task.
        }
    }
}

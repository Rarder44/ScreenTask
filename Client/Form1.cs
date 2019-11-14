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
namespace Client
{
    public partial class Form1 : Form
    {
        TcpClientPlus connection;
        bool Connesso = false;

        public Form1()
        {
            InitializeComponent();

            //TODO: implemento le textbox | ricerca in rete
            connection = new TcpClientPlus("127.0.0.1", 7070);
            Connesso = true;
            
            new Task(() => {
                while (Connesso)
                {
                    DataPacket p = DataPacket.DeserializeFromStream(connection.GetStream());
                    using (MemoryStream img = new MemoryStream())
                    {
                        img.Write(p.Data, 0, p.Data.Length);
                        Bitmap b = new Bitmap(img);
                        
                        try
                        {
                            Image old = pictureBox1.Image;
                            pictureBox1.SetImageInvoke(b);
                            old.Dispose();
                        }
                        catch(Exception e)
                        {

                        }
                        
                    }
                }
            
            }).Start();
        }
    }
}

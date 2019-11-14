using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendCSharp.Classes;

namespace Client.Controls
{
    public partial class JPGPanel : UserControl
    {
        //TODO: implemento Tiled, Stretched ecc
        public JPG _jpg;
        public JPG jpg
        {
            get { return _jpg; } 
            set
            {
                _jpg = value;
                this.Refresh();
            }
        }

        public JPGPanel()
        {
            InitializeComponent();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //TODO: stampo l'immagine
        }
    }
}

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
using ExtendCSharp;


namespace Client.Controls
{
    public partial class JPGPanel : UserControl
    {
        //TODO: implemento Tiled, Stretched ecc
        public ImageLayout imageLayout = ImageLayout.Stretch;
        private JPG _jpg;
        public JPG jpg
        {
            get { return _jpg; } 
            set
            {
                _jpg = value;
                this.Invalidate();
            }
        }

        public JPGPanel()
        {
            InitializeComponent();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //TODO: implemento tutti gli imageLayout
            
            if (_jpg == null)
                return;

            if(imageLayout==ImageLayout.Stretch)
            {
                using(Bitmap bitmap= _jpg.ToBitmap())
                {
                    e.Graphics.DrawImage(bitmap, this.Bounds);
                }
               
            }
            //base.OnPaint(e);
        }
    }
}

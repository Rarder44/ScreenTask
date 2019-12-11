namespace Client
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnConnection = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.numeric_Port = new System.Windows.Forms.NumericUpDown();
            this.comboIPs = new System.Windows.Forms.ComboBox();
            this.jpgPanel1 = new ExtendCSharp.Controls.JPGPanel();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_Port)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConnection
            // 
            this.btnConnection.Location = new System.Drawing.Point(12, 6);
            this.btnConnection.Name = "btnConnection";
            this.btnConnection.Size = new System.Drawing.Size(83, 23);
            this.btnConnection.TabIndex = 1;
            this.btnConnection.Text = "Connect";
            this.btnConnection.UseVisualStyleBackColor = true;
            this.btnConnection.Click += new System.EventHandler(this.button1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 426);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 24);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(18, 19);
            this.toolStripStatusLabel1.Text = "...";
            // 
            // numeric_Port
            // 
            this.numeric_Port.Location = new System.Drawing.Point(228, 7);
            this.numeric_Port.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numeric_Port.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numeric_Port.Name = "numeric_Port";
            this.numeric_Port.Size = new System.Drawing.Size(63, 20);
            this.numeric_Port.TabIndex = 4;
            this.numeric_Port.Value = new decimal(new int[] {
            7070,
            0,
            0,
            0});
            // 
            // comboIPs
            // 
            this.comboIPs.FormattingEnabled = true;
            this.comboIPs.Location = new System.Drawing.Point(101, 6);
            this.comboIPs.Name = "comboIPs";
            this.comboIPs.Size = new System.Drawing.Size(121, 21);
            this.comboIPs.TabIndex = 6;
            // 
            // jpgPanel1
            // 
            this.jpgPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.jpgPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.jpgPanel1.jpg = null;
            this.jpgPanel1.Location = new System.Drawing.Point(0, 35);
            this.jpgPanel1.Name = "jpgPanel1";
            this.jpgPanel1.Size = new System.Drawing.Size(800, 390);
            this.jpgPanel1.TabIndex = 5;
            this.jpgPanel1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.jpgPanel1_MouseDoubleClick);
            this.jpgPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jpgPanel1_MouseDown);
            this.jpgPanel1.MouseLeave += new System.EventHandler(this.jpgPanel1_MouseLeave);
            this.jpgPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.jpgPanel1_MouseMove);
            this.jpgPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jpgPanel1_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.comboIPs);
            this.Controls.Add(this.jpgPanel1);
            this.Controls.Add(this.numeric_Port);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnConnection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Client - Screen Task";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_Port)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnConnection;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.NumericUpDown numeric_Port;
        private ExtendCSharp.Controls.JPGPanel jpgPanel1;
        private System.Windows.Forms.ComboBox comboIPs;
    }
}


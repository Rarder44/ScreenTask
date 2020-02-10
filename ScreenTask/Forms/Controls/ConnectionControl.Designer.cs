namespace ScreenTask.Forms.Controls
{
    partial class ConnectionControl
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

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.IP_port_label = new System.Windows.Forms.Label();
            this.Speed_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // IP_port_label
            // 
            this.IP_port_label.AutoSize = true;
            this.IP_port_label.Location = new System.Drawing.Point(3, 4);
            this.IP_port_label.Name = "IP_port_label";
            this.IP_port_label.Size = new System.Drawing.Size(54, 15);
            this.IP_port_label.TabIndex = 0;
            this.IP_port_label.Text = "IP_Porta";
            // 
            // Speed_label
            // 
            this.Speed_label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Speed_label.Location = new System.Drawing.Point(97, 4);
            this.Speed_label.Name = "Speed_label";
            this.Speed_label.Size = new System.Drawing.Size(103, 15);
            this.Speed_label.TabIndex = 1;
            this.Speed_label.Text = "999,99 Gb/s";
            this.Speed_label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ConnectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Speed_label);
            this.Controls.Add(this.IP_port_label);
            this.Name = "ConnectionControl";
            this.Size = new System.Drawing.Size(205, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label IP_port_label;
        private System.Windows.Forms.Label Speed_label;
    }
}

namespace Sistem_Booking_Hotel
{
    partial class kamar_Ubah_Hapus
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Kamar_Ubah = new System.Windows.Forms.Button();
            this.btn_Kamar_Hapus = new System.Windows.Forms.Button();
            this.hapuskamar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Kamar_Ubah
            // 
            this.btn_Kamar_Ubah.Location = new System.Drawing.Point(18, 12);
            this.btn_Kamar_Ubah.Name = "btn_Kamar_Ubah";
            this.btn_Kamar_Ubah.Size = new System.Drawing.Size(87, 23);
            this.btn_Kamar_Ubah.TabIndex = 0;
            this.btn_Kamar_Ubah.Text = "Ubah";
            this.btn_Kamar_Ubah.UseVisualStyleBackColor = true;
            this.btn_Kamar_Ubah.Click += new System.EventHandler(this.btn_Kamar_Ubah_Click);
            // 
            // btn_Kamar_Hapus
            // 
            this.btn_Kamar_Hapus.Location = new System.Drawing.Point(18, 42);
            this.btn_Kamar_Hapus.Name = "btn_Kamar_Hapus";
            this.btn_Kamar_Hapus.Size = new System.Drawing.Size(87, 23);
            this.btn_Kamar_Hapus.TabIndex = 1;
            this.btn_Kamar_Hapus.Text = "Maintenance";
            this.btn_Kamar_Hapus.UseVisualStyleBackColor = true;
            this.btn_Kamar_Hapus.Click += new System.EventHandler(this.btn_Kamar_Hapus_Click);
            // 
            // hapuskamar
            // 
            this.hapuskamar.Location = new System.Drawing.Point(18, 71);
            this.hapuskamar.Name = "hapuskamar";
            this.hapuskamar.Size = new System.Drawing.Size(87, 23);
            this.hapuskamar.TabIndex = 2;
            this.hapuskamar.Text = "Hapus";
            this.hapuskamar.UseVisualStyleBackColor = true;
            this.hapuskamar.Click += new System.EventHandler(this.hapuskamar_Click);
            // 
            // kamar_Ubah_Hapus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(124, 106);
            this.Controls.Add(this.hapuskamar);
            this.Controls.Add(this.btn_Kamar_Hapus);
            this.Controls.Add(this.btn_Kamar_Ubah);
            this.Name = "kamar_Ubah_Hapus";
            this.Deactivate += new System.EventHandler(this.kamar_Ubah_Hapus_Deactivate);
            this.Load += new System.EventHandler(this.kamar_Ubah_Hapus_Load);
            this.MouseLeave += new System.EventHandler(this.kamar_Ubah_Hapus_MouseLeave);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Kamar_Ubah;
        private System.Windows.Forms.Button btn_Kamar_Hapus;
        private System.Windows.Forms.Button hapuskamar;
    }
}
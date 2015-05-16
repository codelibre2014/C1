namespace Sistem_Booking_Hotel
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.keluarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sistemBookingTabHotelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.inputIdentitas = new System.Windows.Forms.TextBox();
            this.inputSandi = new System.Windows.Forms.TextBox();
            this.btnConn = new System.Windows.Forms.Button();
            this.btnInputSerialNumber = new System.Windows.Forms.Button();
            this.statusStripBooking = new System.Windows.Forms.StatusStrip();
            this.hakcipta = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.statusStripBooking.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keluarToolStripMenuItem,
            this.sistemBookingTabHotelToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // keluarToolStripMenuItem
            // 
            this.keluarToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.keluarToolStripMenuItem.Name = "keluarToolStripMenuItem";
            this.keluarToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.keluarToolStripMenuItem.Text = "Keluar";
            this.keluarToolStripMenuItem.Click += new System.EventHandler(this.keluarToolStripMenuItem_Click);
            // 
            // sistemBookingTabHotelToolStripMenuItem
            // 
            this.sistemBookingTabHotelToolStripMenuItem.Enabled = false;
            this.sistemBookingTabHotelToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.sistemBookingTabHotelToolStripMenuItem.Name = "sistemBookingTabHotelToolStripMenuItem";
            this.sistemBookingTabHotelToolStripMenuItem.Size = new System.Drawing.Size(158, 20);
            this.sistemBookingTabHotelToolStripMenuItem.Text = "Sistem Booking tabHotel";
            // 
            // btnLogin
            // 
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.Location = new System.Drawing.Point(36, 62);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(38, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Identitas";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(38, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sandi";
            // 
            // inputIdentitas
            // 
            this.inputIdentitas.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputIdentitas.Location = new System.Drawing.Point(117, 7);
            this.inputIdentitas.Name = "inputIdentitas";
            this.inputIdentitas.Size = new System.Drawing.Size(156, 22);
            this.inputIdentitas.TabIndex = 5;
            // 
            // inputSandi
            // 
            this.inputSandi.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputSandi.Location = new System.Drawing.Point(117, 34);
            this.inputSandi.Name = "inputSandi";
            this.inputSandi.PasswordChar = '*';
            this.inputSandi.Size = new System.Drawing.Size(156, 22);
            this.inputSandi.TabIndex = 6;
            this.inputSandi.Enter += new System.EventHandler(this.inputSandi_Enter);
            // 
            // btnConn
            // 
            this.btnConn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConn.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConn.Location = new System.Drawing.Point(198, 62);
            this.btnConn.Name = "btnConn";
            this.btnConn.Size = new System.Drawing.Size(75, 23);
            this.btnConn.TabIndex = 7;
            this.btnConn.Text = "Koneksi DB";
            this.btnConn.UseVisualStyleBackColor = true;
            this.btnConn.Click += new System.EventHandler(this.btnConn_Click);
            // 
            // btnInputSerialNumber
            // 
            this.btnInputSerialNumber.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInputSerialNumber.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInputSerialNumber.Location = new System.Drawing.Point(117, 62);
            this.btnInputSerialNumber.Name = "btnInputSerialNumber";
            this.btnInputSerialNumber.Size = new System.Drawing.Size(75, 23);
            this.btnInputSerialNumber.TabIndex = 8;
            this.btnInputSerialNumber.Text = "Serial";
            this.btnInputSerialNumber.UseVisualStyleBackColor = true;
            this.btnInputSerialNumber.Click += new System.EventHandler(this.btnInputSerialNumber_Click);
            // 
            // statusStripBooking
            // 
            this.statusStripBooking.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hakcipta});
            this.statusStripBooking.Location = new System.Drawing.Point(0, 276);
            this.statusStripBooking.Name = "statusStripBooking";
            this.statusStripBooking.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStripBooking.Size = new System.Drawing.Size(311, 22);
            this.statusStripBooking.TabIndex = 9;
            this.statusStripBooking.Text = "statusStrip1";
            // 
            // hakcipta
            // 
            this.hakcipta.Name = "hakcipta";
            this.hakcipta.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.hakcipta.Size = new System.Drawing.Size(189, 17);
            this.hakcipta.Text = "BookIn © 2014 PT. Indo Surya Asia";
            this.hakcipta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.inputIdentitas);
            this.panel1.Controls.Add(this.btnLogin);
            this.panel1.Controls.Add(this.btnInputSerialNumber);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnConn);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.inputSandi);
            this.panel1.Location = new System.Drawing.Point(0, 182);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(311, 102);
            this.panel1.TabIndex = 10;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::Sistem_Booking_Hotel.Properties.Resources.bell_and_book;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(311, 187);
            this.panel2.TabIndex = 11;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 298);
            this.Controls.Add(this.statusStripBooking);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximumSize = new System.Drawing.Size(327, 336);
            this.Name = "Login";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BookIn Central Reservation System";
            this.Load += new System.EventHandler(this.Login_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Login_KeyDown);
            this.Resize += new System.EventHandler(this.Login_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStripBooking.ResumeLayout(false);
            this.statusStripBooking.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem keluarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sistemBookingTabHotelToolStripMenuItem;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox inputIdentitas;
        private System.Windows.Forms.TextBox inputSandi;
        private System.Windows.Forms.Button btnConn;
        private System.Windows.Forms.Button btnInputSerialNumber;
        private System.Windows.Forms.StatusStrip statusStripBooking;
        private System.Windows.Forms.ToolStripStatusLabel hakcipta;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}
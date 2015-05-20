namespace Sistem_Booking_Hotel
{
    partial class DatabaseConnectionForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.WindowsAuthen = new System.Windows.Forms.CheckBox();
            this.groupBoxUser = new System.Windows.Forms.GroupBox();
            this.PWD = new System.Windows.Forms.TextBox();
            this.UID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.serverName = new System.Windows.Forms.ComboBox();
            this.databaseNamee = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Komputer Database";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(135, 173);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(103, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Simpan Koneksi";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // WindowsAuthen
            // 
            this.WindowsAuthen.AutoSize = true;
            this.WindowsAuthen.Location = new System.Drawing.Point(135, 39);
            this.WindowsAuthen.Name = "WindowsAuthen";
            this.WindowsAuthen.Size = new System.Drawing.Size(141, 17);
            this.WindowsAuthen.TabIndex = 8;
            this.WindowsAuthen.Text = "Windows Authentication";
            this.WindowsAuthen.UseVisualStyleBackColor = true;
            this.WindowsAuthen.CheckedChanged += new System.EventHandler(this.WindowsAuthen_CheckedChanged);
            // 
            // groupBoxUser
            // 
            this.groupBoxUser.Controls.Add(this.PWD);
            this.groupBoxUser.Controls.Add(this.UID);
            this.groupBoxUser.Controls.Add(this.label4);
            this.groupBoxUser.Controls.Add(this.label3);
            this.groupBoxUser.Location = new System.Drawing.Point(63, 59);
            this.groupBoxUser.Name = "groupBoxUser";
            this.groupBoxUser.Size = new System.Drawing.Size(323, 75);
            this.groupBoxUser.TabIndex = 9;
            this.groupBoxUser.TabStop = false;
            // 
            // PWD
            // 
            this.PWD.Location = new System.Drawing.Point(97, 44);
            this.PWD.Name = "PWD";
            this.PWD.Size = new System.Drawing.Size(226, 20);
            this.PWD.TabIndex = 5;
            // 
            // UID
            // 
            this.UID.Location = new System.Drawing.Point(97, 16);
            this.UID.Name = "UID";
            this.UID.Size = new System.Drawing.Size(226, 20);
            this.UID.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Username";
            // 
            // serverName
            // 
            this.serverName.FormattingEnabled = true;
            this.serverName.Location = new System.Drawing.Point(135, 11);
            this.serverName.Name = "serverName";
            this.serverName.Size = new System.Drawing.Size(251, 21);
            this.serverName.TabIndex = 10;
            // 
            // databaseNamee
            // 
            this.databaseNamee.FormattingEnabled = true;
            this.databaseNamee.Location = new System.Drawing.Point(137, 144);
            this.databaseNamee.Name = "databaseNamee";
            this.databaseNamee.Size = new System.Drawing.Size(251, 21);
            this.databaseNamee.TabIndex = 13;
            this.databaseNamee.DropDown += new System.EventHandler(this.databaseNamee_DropDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Nama Database";
            // 
            // DatabaseConnectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 206);
            this.ControlBox = false;
            this.Controls.Add(this.databaseNamee);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverName);
            this.Controls.Add(this.groupBoxUser);
            this.Controls.Add(this.WindowsAuthen);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "DatabaseConnectionForm";
            this.Load += new System.EventHandler(this.DatabaseConnectionForm_Load);
            this.groupBoxUser.ResumeLayout(false);
            this.groupBoxUser.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox WindowsAuthen;
        private System.Windows.Forms.GroupBox groupBoxUser;
        private System.Windows.Forms.TextBox PWD;
        private System.Windows.Forms.TextBox UID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox serverName;
        private System.Windows.Forms.ComboBox databaseNamee;
        private System.Windows.Forms.Label label2;
    }
}
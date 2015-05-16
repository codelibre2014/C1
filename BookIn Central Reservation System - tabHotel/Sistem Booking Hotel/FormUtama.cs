using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using Microsoft.Reporting;
using System.Collections;
using System.Globalization;
using System.Threading;
using Microsoft.Reporting.WinForms;
using teboweb;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Net;

using AForge.Video;
using AForge.Video.DirectShow;

namespace Sistem_Booking_Hotel
{
    public partial class FormUtama : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        private VideoCapabilities[] videoCapabilities;
        private VideoCapabilities[] snapshotCapabilities;

        
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(out LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf =
                   Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public int dwTime;
        }

        SqlCommand cmd; SqlCommand cmd1;
        public string boxJbt = ""; public string boxKmrTersedia = ""; public string boxKalender = ""; public string boxStatus = ""; public string boxSelesai = "";
        public string boxDafTamu = ""; public string boxStaff = ""; public string boxHak = ""; public string boxResto = ""; public string boxAturKamar = "";
        public string boxInv = ""; public string boxAturHotel = ""; public string boxAturKhusus = ""; public string boxAturPeri = ""; public string boxLap = "";
        public string boxBkAngus = ""; public string boxAturItem = ""; public string boxUtang = ""; public string boxRekap = ""; public string boxLaporanRestoran = ""; 
        public string boxBatal = "";
        public int boxId;
        public DateTime tglcheck;
        public int noroom;
        public int opsistatusbookingkamar;
        public int noidtamu;

        DataTable dKamarPesan = new DataTable();
        ComboboxItem item = new ComboboxItem();
        Boolean cekPilih = false;
        Boolean cekPilihLaporan = false;
        Boolean cekPilihLaporanGrandTotal = false;
        
        int dataCustomer = 0; int rowSelect = 0; int columnSelect;
        int TglBulan = 0; int Tgltahun = 0;
        int totalBiaya = 0;
        DataSet ds;
        SqlDataAdapter da;
        
        int dataKamarCh = 0;
        int noIDdatatamu = 0;
        int dataKamarGabung = 0;

        int kamarStatus = 0;

        public FormUtama()
        {
            InitializeComponent();                    
        }

        SqlDataReader reader;
        int x = 0;
        configconn koneksi = new configconn();
        //SqlConnection conn = new SqlConnection();

        private int idStaff;
        public int getIdStaff
        {
            get
            {
                return idStaff;
            }
            set
            {
                idStaff = value;
            }
        }


        private string isAdmin;
        public string getAdmin
        {
            get
            {
                return isAdmin;
            }
            set
            {
                isAdmin = value;
                // do something with _theValue so that it
                // appears in the UI

            }
        }

        public void refresh_kamar()
        {
            panelKamarDibooking.Enabled = true;
            panelKamar.Controls.Clear();
            //conn.Open();
            cmd = new SqlCommand((@"select count(*) from Kamar"), koneksi.KoneksiDB());

            int jumKamar = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();

            ///button1.Text = jumKamar.ToString();
            Button[] Kamar;

            //command.Parameters.AddWithValue("@Username", username);
            //command.Parameters.AddWithValue("@Password", password);

            cmd = new SqlCommand(
            (@"
            select k.kamar_no,kk.kamar_kapasitas,kt.warna,
            case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga, k.smoking 
            from 
            (
	            select distinct kamar_no
	            from 
	            Reservasi r
                where 
	                (
                        (r.checkin >= @checkindate
	                    and
	                    r.checkout <=@checkoutdate
	                    )
	                    or
	                    (
	                    r.checkin <= @checkindate
	                    and
	                    r.checkout >=@checkoutdate
	                    )
	                    or 
	                    (
	                    r.checkin >= @checkindate
	                    and
	                    r.checkin < @checkoutdate
	                    )
	                    or 
	                    (
	                    r.checkout > @checkindate
	                    and
	                    r.checkout <=@checkoutdate
	                    )
                    )
                    and r.status in ('booking','checkin') 
            )a
            full join
            Kamar k
            on a.kamar_no = k.kamar_no 
            inner join
            kamar_kapasitas kk on k.kamar_kapasitas_id = kk.kamar_kapasitas_id
            inner join 
            kamar_tipe kt on k.kamar_tipe_id = kt.kamar_tipe_id
            inner join harga h on h.tanggal_id = '2014-1-1'
            and kt.kamar_tipe_id = h.kamar_tipe_id
			where a.kamar_no is null and (k.status is null or k.status = '1')  
            order by k.kamar_no desc
            "), koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@checkindate", checkinDate.Value.ToString("yyyy-M-d"));
            cmd.Parameters.AddWithValue("@checkoutdate", checkoutDate.Value.ToString("yyyy-M-d"));
            /*
             
             cmd = new SqlCommand(
            (@"select
            k.kamar_no,
            k.kamar_tipe_id,
            case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga
            from            
            Kamar k
            inner join kamar_tipe kt on k.kamar_tipe_id = kt.kamar_tipe_id 
            inner join harga h on h.tanggal_id = '2008-7-1'
            and kt.kamar_tipe_id = h.kamar_tipe_id"), koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@checkindate",checkinDate.Value.ToString("yyyy-M-d"));
            //cmd.Parameters.AddWithValue("@checkoutdate",checkoutDate.Value.ToString("yyyy-M-d"));
             */
            String baruString = "";
            reader = cmd.ExecuteReader();
            Kamar = new Button[jumKamar];
            x = 0;
            while (reader.Read())
            {
                Kamar[x] = new Button();
                Kamar[x].Text = reader.GetInt32(0).ToString() + "\n\r" + reader.GetString(1);
                Kamar[x].Name = reader.GetInt32(0).ToString();
                Kamar[x].Visible = true;
                Kamar[x].Height = 45;
                Kamar[x].Width = 95;
                Kamar[x].FlatStyle = FlatStyle.Flat;
                Kamar[x].Tag = reader.GetDouble(3).ToString();

                //Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                try
                {
                    Kamar[x].BackColor = Color.FromArgb(Int32.Parse(reader.GetString(2)));
                }
                catch
                {
                    Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                }
                if (reader["smoking"].ToString().Equals("1"))
                {
                    Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.smoke;
                }
                else
                {

                    Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.nosmoke;
                }
                Kamar[x].ImageAlign = btnBooking.ImageAlign;
                Kamar[x].Click += new EventHandler(tambah_kamar);
                Kamar[x].MouseEnter += new EventHandler(showhargakamar);
                Kamar[x].MouseLeave += new EventHandler(hidehargakamar);
                if (baruString.Equals(""))
                {
                    baruString = Kamar[x].Name.ToString().Substring(0, 1);
                }
                if (!Kamar[x].Name.ToString().Substring(0, 1).Equals(baruString))
                {
                    baruString = Kamar[x].Name.ToString().Substring(0, 1);
                    LinkLabel label1 = new LinkLabel();
                    label1.AutoSize = false;
                    label1.Height = 20;
                    label1.Width = panelKamar.Width;
                    label1.BorderStyle = BorderStyle.Fixed3D;

                    panelKamar.Controls.Add(label1);
                }
                panelKamar.Controls.Add(Kamar[x]);
                x += 1;
                //Kamar[x].MouseEnter += button1_MouseEnter_2;// Kamar_Tips;//new EventHandler(Kamar_Tips);

            }
            //conn.Close();
            koneksi.closeConnection();
        }

        public void showhargakamar(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            SqlCommand sqlC = new SqlCommand("select kamar_tipe_id from Kamar where kamar_no = @no", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@no", btn.Name);
            string nilai = sqlC.ExecuteScalar().ToString();
            koneksi.closeConnection();

            sqlC = new SqlCommand("select case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end hargaK from Harga where kamar_tipe_id = @tipe and tanggal_id = @chin", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@tipe", Int32.Parse(nilai));
            sqlC.Parameters.AddWithValue("@chin", checkinDate.Value);
            SqlDataReader readC = sqlC.ExecuteReader();
            while (readC.Read())
            {

                toolTip1.Show(
                    "---------------------------------------------\r\n" +
                    " Kamar No : " + btn.Name.ToString() + "\r\n" +
                    "---------------------------------------------\r\n" +
                    " Harga Kamar : Rp." + readC["hargaK"].ToString() + ",00\r\n" +
                    "---------------------------------------------\r\n"
                    , btn);

            }
            //Thread.Sleep(1000);
            koneksi.closeConnection();

        }
        public void hidehargakamar(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            toolTip1.Hide(btn);
        }
        public void refreshPengaturanKamar()
        {
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            DataKamar dataKamar = new DataKamar(this);
            dataKamar.TopLevel = false;
            dataKamar.Name = "panelPengaturanKamarInnerForm";
            //panelPengaturanKamar.BringToFront();
            //splitContainer2.Panel1.Controls.Clear();
            splitContainer2.Panel1.Controls.Add(dataKamar);
            dataKamar.Show();
            dataKamar.Dock = DockStyle.Fill;
            dataKamar.BringToFront();
        }

        public void refresh_pengaturankamar()
        {
            //configconn.conn.Open();
            SqlCommand cmd = new SqlCommand((@"select count(*) from Kamar"), koneksi.KoneksiDB());
            int jumKamar = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();

            ///button1.Text = jumKamar.ToString();
            Button[] Kamar;
            cmd = new SqlCommand("select kamar_no from Kamar", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            Kamar = new Button[jumKamar + 1];
            x = 0;
            while (reader.Read())
            {
                Kamar[x] = new Button();
                Kamar[x].Text = reader.GetInt32(0).ToString(); //+ "\n\r" + reader.GetString(1);
                Kamar[x].Name = reader.GetInt32(0).ToString();
                Kamar[x].Visible = true;
                //Kamar[x].Height = 35;
                //Kamar[x].Tag = reader.GetDouble(2).ToString();
                //Kamar[x].BackColor = Color.FromName(reader.GetString(1));
                Kamar[x].Height = 45;
                Kamar[x].Width = 95;
                Kamar[x].FlatStyle = FlatStyle.Flat;
                Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
                Kamar[x].ImageAlign = btnBooking.ImageAlign;
                
                Kamar[x].Click += new EventHandler(load_Ubah_Hapus);
                //Kamar[x].MouseEnter += new EventHandler(button1_MouseEnter_2);
                //Kamar[x].MouseLeave += new EventHandler(button1_MouseLeave_1);

                flowLayoutPanel1.Controls.Add(Kamar[x]);
                x += 1;
                //MessageBox.Show("ok");
                //Kamar[x].MouseEnter += button1_MouseEnter_2;// Kamar_Tips;//new EventHandler(Kamar_Tips);

            }
            koneksi.closeConnection();

            Kamar[x] = new Button();
            //Kamar[x].Text = reader.GetInt32(0).ToString() + "\n\r" + reader.GetString(1);
            //Kamar[x].Name = reader.GetInt32(0).ToString();
            Kamar[x].Text = "+";
            Kamar[x].Visible = true;
            //Kamar[x].Height = 35;
            //Kamar[x].Tag = reader.GetDouble(2).ToString();
            Kamar[x].Height = 45;
            Kamar[x].Width = 95;
            Kamar[x].FlatStyle = FlatStyle.Flat;
            Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
            Kamar[x].ImageAlign = btnBooking.ImageAlign;
                
            Kamar[x].BackColor = Color.Aqua;
            Kamar[x].Click += new EventHandler(tambah_Kamar);
            flowLayoutPanel1.Controls.Add(Kamar[x]);
            //conn.Close();
            //configconn.conn.Close();
        }
        //kamar_Ubah_Hapus loadUbahHapus = new kamar_Ubah_Hapus();
        protected void load_Ubah_Hapus(object sender, EventArgs e)
        {
            kamar_Ubah_Hapus loadUbahHapus = new kamar_Ubah_Hapus(this);
            Button btn = sender as Button;
            loadUbahHapus.passNoKamar(Convert.ToInt32(btn.Text));
            loadUbahHapus.ShowDialog();
            loadUbahHapus.Location = new Point(btn.Location.X + (btn.Width) + loadUbahHapus.Width, btn.Location.Y);
        }
        TambahKamar tambahKamar;
        protected void tambah_Kamar(object sender, EventArgs e)
        {
            tambahKamar = new TambahKamar(this);

            tambahKamar.ShowDialog();
        }
        public void hilangkan()
        {
        }
        private void infoHotelWelcome(){
            SqlCommand sql = new SqlCommand("select Nama_Hotel, Alamat, Kota, Telepon from IDHotel",koneksi.KoneksiDB());
            SqlDataReader reads = sql.ExecuteReader();
            while (reads.Read())
            {
                label105.Text = reads["Nama_Hotel"].ToString();
                label106.Text = reads["Alamat"].ToString();
                label107.Text = reads["Kota"].ToString();
                label108.Text = reads["Telepon"].ToString();
            }
            koneksi.closeConnection();
        }

        string lang = "En";

        private void NotifyIcon1_Click(object sender, System.EventArgs e)
        {
         //   string url = "https://www.dropbox.com/s/38etpu15rx6uza9/um.zip?dl=0";
            string url = "http://www.indosuryaasia.com/BookIn/";
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch
            {

            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'laporanWNA.Tamu' table. You can move, or remove it, as needed.
            this.tamuTableAdapter1.Fill(this.laporanWNA.Tamu);
            this.Refresh();

            bool cek = false;
            string urlBaru = "http://www.indosuryaasia.com/BookIn/";
            cek = update.getUpdateInfo(urlBaru, "UM.txt", Application.StartupPath + @"\", 1, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", ""));
            if (cek)
            {
                notifyIcon1.Icon = SystemIcons.Exclamation;
                notifyIcon1.Visible = true;
                notifyIcon1.Click += new System.EventHandler(NotifyIcon1_Click);
                notifyIcon1.BalloonTipClicked += new System.EventHandler(NotifyIcon1_Click);
                notifyIcon1.ShowBalloonTip(8000, "BookIn Versi Baru Tersedia", "Klik Untuk Melihat Update", ToolTipIcon.Info);
            }
            Hide();
            bool done = false;
            ThreadPool.QueueUserWorkItem((x) =>
            {
                using (var splashForm = new ProgressIndicator())
                {
                    splashForm.Show();
                    while (!done)
                        Application.DoEvents();
                    splashForm.Close();
                }
            });

            checkinDate.MinDate = DateTime.Today.AddDays(-1);
            checkoutDate.MinDate = checkinDate.Value.AddDays(1);
            checkoutDate.Value = checkinDate.Value.AddDays(1);
           
            //MessageBox.Show(Login.idS.ToString());
            panel5.BringToFront();
            infoHotelWelcome();
            dKamarPesan.Reset();
            dKamarPesan.Columns.Add("NO Kamar".ToString());
            dKamarPesan.Columns.Add("Checkin", typeof(DateTime));
            dKamarPesan.Columns.Add("Checkout", typeof(DateTime));
            dKamarPesan.Columns.Add("Tamu".ToString());
            dKamarPesan.Columns.Add("Harga".ToString());

            string strTersedia = ""; string strKal = ""; string strStat = ""; string strSelesai = ""; string strDafTamu = "";
            string strStaff = ""; string strHak = ""; string strResto = ""; string strAturKamar = ""; string strInv = "";
            string strAturHotel = ""; string strAturKhusus = ""; string strAturPeri = ""; string strLap = ""; string strBkAngus = "";
            string strAturItem = "";
            string strUtang = ""; string strRekap = ""; string strLaporanRestoran = ""; string strBatal = "";
            //MessageBox.Show(Login.idS.ToString());

            //MessageBox.Show(Login.idS.ToString());
            SqlCommand cmd = new SqlCommand("select * from jabatan a,staff b where a.id_jabatan=b.id_jabatan and b.staff_id=@idjab", koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@idjab", Login.idS);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                strTersedia = reader.GetValue(2).ToString();
                strKal = reader.GetValue(3).ToString();
                strStat = reader.GetValue(4).ToString();
                strSelesai = reader.GetValue(5).ToString();
                strDafTamu = reader.GetValue(6).ToString();
                strStaff = reader.GetValue(7).ToString();
                strHak = reader.GetValue(8).ToString();
                strResto = reader.GetValue(9).ToString();
                strAturKamar = reader.GetValue(10).ToString();
                strInv = reader.GetValue(11).ToString();
                strAturHotel = reader.GetValue(12).ToString();
                strAturKhusus = reader.GetValue(13).ToString();
                strAturPeri = reader.GetValue(14).ToString();
                strLap = reader.GetValue(15).ToString();
                strBkAngus = reader.GetValue(16).ToString();
                strAturItem = reader.GetValue(17).ToString();
                strUtang = reader.GetValue(18).ToString();
                strRekap = reader.GetValue(19).ToString();
                strLaporanRestoran = reader.GetValue(20).ToString();
                strBatal = reader.GetValue(21).ToString();
            }

            if (strTersedia == "On") btnBooking.Visible = true; else btnBooking.Visible = false;
            if (strTersedia == "On")
            { panelCheckinDate.Visible = true; panelCheckoutDate.Visible = true; }
            else { panelCheckinDate.Visible = false; panelCheckoutDate.Visible = false;  }
            if (strKal == "On") btnKalender.Visible = true; else btnKalender.Visible = false;
            if (strStat == "On") btnCheckInStatus.Visible = true; else btnCheckInStatus.Visible = false;
            if (strSelesai == "On") btnKamarMaintenance.Visible = true; else btnKamarMaintenance.Visible = false;
            if (strDafTamu == "On") btnDaftarTamu.Visible = true; else btnDaftarTamu.Visible = false;
            if (strStaff == "On") btnUser.Visible = true; else btnUser.Visible = false;
            if (strHak == "On") btnRights.Visible = true; else btnRights.Visible = false;
            if (strResto == "On") btn_restoran.Visible = true; else btn_restoran.Visible = false;
            if (strAturKamar == "On") btnPengaturanKamar.Visible = true; else btnPengaturanKamar.Visible = false;
            if (strInv == "On") btn_historis.Visible = true; else btn_historis.Visible = false;
            if (strAturHotel == "On") btnPengaturanHotel.Visible = true; else btnPengaturanHotel.Visible = false;
            //if (strAturKhusus == "On") btn_harga_khusus.Visible = true; else btn_harga_khusus.Visible = false;
            //if (strAturPeri == "On") btnPeriodik.Visible = true; else btnPeriodik.Visible = false;
            if (strAturItem == "On") btn_pengaturan_item.Visible = true; else btn_pengaturan_item.Visible = false;
            if (strAturKhusus == "On" || strAturPeri == "On") btnPengaturanHarga.Visible = true; else btnPengaturanHarga.Visible = false;
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            if (strLap == "On") { btnLaporanKeuangan.Visible = true; btnLaporanGrandTotal.Visible = true; btnLaporanTopCorp.Visible = true; } else { btnLaporanKeuangan.Visible = false; btnLaporanGrandTotal.Visible = false; btnLaporanTopCorp.Visible = false; }
            //if (strLap == "On") { btnLaporanGrandTotal.Visible = true; } else { btnLaporanGrandTotal.Visible = false; }
            
            if (strBkAngus == "On") btn_bookingHangus.Visible = true; else btn_bookingHangus.Visible = false;
            if (strUtang == "On")
            {
                btnUtng.Visible = true;
            }
            else
            {
                btnUtng.Visible = false;
            }
            if (strRekap == "On")
            {
                btnRekapHariIni.Visible = true;
             //   btnLaporanGrandTotal.Visible = true;
            }
            else
            {
                btnLaporanGrandTotal.Visible = false;
             //   btnRekapHariIni.Visible = false;
            }
            if (strLaporanRestoran == "On") btn_pendapatanRestoran.Visible = true; else btn_pendapatanRestoran.Visible = false;

            if(strBatal == "On"){
                //batalToolStripMenuItem1.Enabled = true;
                pembatalantoolStripMenuItem1.Enabled = true;
                BatalStripMenuItem1.Enabled = true;
                btn_hapusReservasi.Enabled = true;
                btn_hapusReservasiNPembayaran.Enabled = true;
                batalkanReservasiToolStripMenuItem.Enabled = true;
            }else{
                //batalToolStripMenuItem1.Enabled = false;
                pembatalantoolStripMenuItem1.Enabled = false;
                BatalStripMenuItem1.Enabled = false;
                btn_hapusReservasi.Enabled = false;
                btn_hapusReservasiNPembayaran.Enabled = false;
                batalkanReservasiToolStripMenuItem.Enabled = false;
            }
            //if (isAdmin == "admin") btnPengaturanKamar.Visible = true; else btnPengaturanKamar.Visible = false;
            //if (isAdmin == "admin") btnBooking.Visible = true;
            //if (isAdmin == "admin") btnPengaturanKamar.Visible = true;
            //if (isAdmin == "admin") btnKalender.Visible = true;
            //if (isAdmin == "admin") btnPesan.Visible = true;
            //if (isAdmin == "admin") button1.Visible = true;
            //if (isAdmin == "admin") btnDaftarTamu.Visible = true;
            //if (isAdmin == "admin") btnUser.Visible = true;
            //if (isAdmin == "admin") btnRights.Visible = true;
            //if (isAdmin == "admin") btn_restoran.Visible = true;
            //if (isAdmin == "admin") btn_historis.Visible = true;
            //if (isAdmin == "admin") btnPengaturanHotel.Visible = true;
            //if (isAdmin == "admin") btn_harga_khusus.Visible = true;
            //if (isAdmin == "admin") btnPeriodik.Visible = true;
            //if (isAdmin == "admin") btnLaporanKeuangan.Visible = true;
            //if (isAdmin == "admin") btn_bookingHangus.Visible = true;
            //if (isAdmin == "admin") btn_pengaturan_item.Visible = true;
            ////MessageBox.Show(isAdmin.ToString());
            // TODO: This line of code loads data into the 'tabHotelDataSet.Tamu' table. You can move, or remove it, as needed.
            //this.tamuTableAdapter.Fill(this.tabHotelDataSet.Tamu);
            // TODO: This line of code loads data into the 'tabHotelDataSet1.Tamu' table. You can move, or remove it, as needed.
            //this.tamuTableAdapter.Fill(this.tabHotelDataSet1.Tamu);
            // TODO: This line of code loads data into the 'tabHotelDataSet.Booking' table. You can move, or remove it, as needed.
            //this.bookingTableAdapter.Fill(this.tabHotelDataSet.Booking);
            //this.TopMost = true;
            btnBookingStatus.Visible = false;
            //btnCheckInStatus.Visible = true;
            //btnKamarMaintenance.Visible = false;
            //this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            //checkoutDate.MinDate = checkinDate.Value.AddDays(1);
            //panelKamar.BringToFront();
            //panelCheckinDate.Visible = true;
            //panelCheckoutDate.Visible = true;
            refresh_kamar();
            //irwan tambahkan
            isiCombobox();
            isCombobox3();
            // end irwan

            //connecting();
            //conn.Open();
            //koneksi.closeConnection();
            cmbJbtUsr.Refresh();
            //cmbJbtR.Refresh();
            cmd = new SqlCommand("select id_jabatan,jabatan from jabatan", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                boxId = reader.GetInt32(0);
                //cmbJbtR.Items.Add(reader.GetString(1));
                cmbJbtUsr.Items.Add(reader.GetString(1));
                //a++;
            }
            koneksi.closeConnection();
//            conn.Close();

  //          connecting();
    ////        conn.Open();
    //        cmd = new SqlCommand("select nama from staff", koneksi.KoneksiDB());
    //        reader = cmd.ExecuteReader();
    //        while (reader.Read())
    //        {
    //            cmbNmUsr.Items.Add(reader.GetString(0));
    //            //a++;
    //        }
    //        koneksi.closeConnection();
//            conn.Close();

            cmd = new SqlCommand("select b.nama from staff b where b.staff_id=@idjab", koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@idjab", Login.idS);
            userLogin.Text = "Selamat Datang, " + cmd.ExecuteScalar().ToString();
            koneksi.closeConnection();

            string strQ = "select * from jabatan";
            createTblNoParam(strQ);

            string strQUsr = "select nama,password,username,telp,email,Jabatan " +
                "from staff a,Jabatan b where a.id_jabatan=b.id_jabatan";
            createTblNoParamUsr(strQUsr);

//            this.reportViewer1.RefreshReport();
            //this.reportInvoice.RefreshReport();
            //this.reportViewer1.RefreshReport();
            checkouthariIni();

            Thread.Sleep(0); // Emulate hardwork
            done = true;
            Show();

            isiArrayJamLogout();
            AutoLogout = false;

            SqlCommand lg = new SqlCommand("select distinct ISNULL(bahasa, 'En') from IDHotel", koneksi.KoneksiDB());
            lang = lg.ExecuteScalar().ToString();
            koneksi.closeConnection();

            if(lang=="En"){
                btnBooking.Text = "Booking Registration";
                btnKalender.Text = "Booking Schedule";
                btnCheckInStatus.Text = "Occupied Room Management";
                btnKamarMaintenance.Text = "Room Status Management";
                btnDaftarTamu.Text = "Guest List";
                btnRights.Text = "Rights";
                btnUser.Text = "Staff";
                btn_restoran.Text = "Restaurant";
                btnPengaturanKamar.Text = "Room Configuration";
                btn_historis.Text = "Invoice Archive";
                btnPengaturanHotel.Text = "Hotel Configuration";
                btnPengaturanHarga.Text = "Price Management";
                btn_harga_khusus.Text = "Special Rate";
                btnPeriodik.Text = "Full Rate";
                btn_bookingHangus.Text = "Expiring Registration";
                btn_pengaturan_item.Text = "Inventory Management";
                btnLaporanKeuangan.Text = "Income Report";
                btnRekapHariIni.Text = "Daily Income Report";
                btnUtng.Text = "Pending Bill";
                btn_pendapatanRestoran.Text = "Restaurant Income Report";
                btnLaporanGrandTotal.Text = "Financial Control";    
                btn_Konfigurasi.Text = "Configuration";
                btnLaporan.Text = "Reports";
                btnLaporanWNA.Text = "Foreign Guest Report";
                btn_stockExpen.Text = "Warehouse Stock";
                btn_inventorykamar.Text = "Room Inventory";
                btnLaporanTopCorp.Text = "Top 10 Corporate";
                btnLaporanTandaTangan.Text = "Hand Signature Report";
            }
            else if (lang == "Ind")
            {
                btnBooking.Text = "Registrasi Booking";
                btnKalender.Text = "Kalender Booking";
                btnCheckInStatus.Text = "Kamar CheckIn";
                btnKamarMaintenance.Text = "Konfigurasi Status Kamar";
                btnDaftarTamu.Text = "Daftar Tamu";
                btnRights.Text = "Jabatan";
                btnUser.Text = "Karyawan";
                btn_restoran.Text = "Restoran";
                btnPengaturanKamar.Text = "Pengaturan Kamar";
                btn_historis.Text = "Daftar Invoice";
                btnPengaturanHotel.Text = "Pengaturan Hotel";
                btnPengaturanHarga.Text = "Pengaturan Harga";
                btn_harga_khusus.Text = "Harga Khusus";
                btnPeriodik.Text = "Harga Periodik";
                btn_bookingHangus.Text = "Booking Hangus";
                btn_pengaturan_item.Text = "Pengaturan Item";
                btnLaporanKeuangan.Text = "Laporan Pendapatan";
                btnRekapHariIni.Text = "Laporan Pendapatan Harian";
                btnUtng.Text = "Utang";
                btn_pendapatanRestoran.Text = "Laporan Pendapatan Restoran";
                btnLaporanGrandTotal.Text = "Kontrol Keuangan";
                btn_Konfigurasi.Text = "Konfigurasi";
                btnLaporan.Text = "Laporan";
                btnLaporanWNA.Text = "Laporan Tamu WNA";
                btn_stockExpen.Text = "Persediaan Barang";
                btn_inventorykamar.Text = "Inventori Kamar";
                btnLaporanTopCorp.Text = "Korporasi 10 Besar";
                btnLaporanTandaTangan.Text = "Laporan Tanda Tangan";
            }

            resetBtnKonfigurasi();
            resetBtnLaporan();

            // enumerate video devices
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count != 0)
            {
                // add all devices to combo
                foreach (FilterInfo device in videoDevices)
                {
                    devicesCombo.Items.Add(device.Name);
                }
            }
            else
            {
                devicesCombo.Items.Add("No DirectShow devices found");
            }

            EnableConnectionControls(true);
            hakcipt.Text = "BookIn Central Reservation System © " + DateTime.Now.Year + " PT. Indo Surya Asia";

        }


        private void resetBtnKonfigurasi()
        {
            btnPengaturanKamar.Size = new Size(1, 1);
            btnPengaturanKamar.Margin = new Padding(1);

            btnPengaturanHotel.Size = new Size(1, 1);
            btnPengaturanHotel.Margin = new Padding(1);

            btnPengaturanHarga.Size = new Size(1, 1);
            btnPengaturanHarga.Margin = new Padding(1);

            btn_harga_khusus.Size = new Size(1, 1);
            btn_harga_khusus.Margin = new Padding(1);

            btnPeriodik.Size = new Size(1, 1);
            btnPeriodik.Margin = new Padding(1);

            btn_pengaturan_item.Size = new Size(1, 1);
            btn_pengaturan_item.Margin = new Padding(1);        
        
            btnUser.Size = new Size(1, 1);
            btnUser.Margin = new Padding(1);

            btnRights.Size = new Size(1, 1);
            btnRights.Margin = new Padding(1);

            btn_stockExpen.Size = new Size(1, 1);
            btn_stockExpen.Margin = new Padding(1);

            btn_inventorykamar.Size = new Size(1, 1);
            btn_inventorykamar.Margin = new Padding(1);

            //resetBtnLaporan();
        }
        /*
         this.flowLayoutPanel2.Controls.Add(this.btnLaporanKeuangan);
            this.flowLayoutPanel2.Controls.Add(this.btnLaporanGrandTotal);
            this.flowLayoutPanel2.Controls.Add(this.btnUtng);
            this.flowLayoutPanel2.Controls.Add(this.btnRekapHariIni);
            this.flowLayoutPanel2.Controls.Add(this.btn_pendapatanRestoran);
            
         
         */

        private void resetBtnLaporan()
        {
            btnLaporanKeuangan.Size = new Size(1, 1);
            btnLaporanKeuangan.Margin = new Padding(1);

            btnLaporanGrandTotal.Size = new Size(1, 1);
            btnLaporanGrandTotal.Margin = new Padding(1);

            btnUtng.Size = new Size(1, 1);
            btnUtng.Margin = new Padding(1);

            btnRekapHariIni.Size = new Size(1, 1);
            btnRekapHariIni.Margin = new Padding(1);

            btn_pendapatanRestoran.Size = new Size(1, 1);
            btn_pendapatanRestoran.Margin = new Padding(1);

            btnLaporanWNA.Size = new Size(1, 1);
            btnLaporanWNA.Margin = new Padding(1);

            btnLaporanTandaTangan.Size = new Size(1, 1);
            btnLaporanTandaTangan.Margin = new Padding(1);

            btnLaporanTopCorp.Size = new Size(1, 1);
            btnLaporanTopCorp.Margin = new Padding(1);
            
        }
        
        private void createTblNoParam(string strQuery)
        {
//          connecting();
//          conn.Open();
            cmd = new SqlCommand(strQuery, koneksi.KoneksiDB());
            ds = new DataSet();
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, "User Rights");
            dgR.DataSource = ds;
            dgR.DataMember = "User Rights";
            dgR.Refresh();
            koneksi.closeConnection();
        }

        private void createTblNoParamUsr(string strQuery)
        {
            //connecting();
            //conn.Open();
            cmd = new SqlCommand(strQuery, koneksi.KoneksiDB());
            ds = new DataSet();
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, "User");
            dgUsr.DataSource = ds;
            dgUsr.DataMember = "User";
            dgUsr.Refresh();
            koneksi.closeConnection();

        }

        private void createTbl1Param(string strQuery, string strParam)
        {

            //connecting();
            //conn.Open();
            cmd = new SqlCommand(strQuery, koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@nm", strParam);
            ds = new DataSet();
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, "User Rights");
            dgR.DataSource = ds;
            dgR.DataMember = "User Rights";
            dgR.Refresh();
            koneksi.closeConnection();

        }

        private void createTbl1ParamUsr(string strQuery, string strParam)
        {

            //connecting();
            //conn.Open();
            cmd = new SqlCommand(strQuery, koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@nm", strParam);
            ds = new DataSet();
            da = new SqlDataAdapter(cmd);
            da.Fill(ds, "User");
            dgUsr.DataSource = ds;
            dgUsr.DataMember = "User";
            dgUsr.Refresh();
            koneksi.closeConnection();

        }

        private void clearJabatan()
        {
            //connecting();
            //conn.Open();
            cmbJbtUsr.Items.Clear();
            //cmbJbtUsr.ResetText();
            string strQ2 = "select jabatan from Jabatan";
            cmd = new SqlCommand(strQ2, koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@nm", cmbJbtUsr.SelectedItem.ToString());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // boxId = reader.GetInt32(0);
                //    lblHideJabUsr.Text = reader.GetInt32(0).ToString();
                //    //cmbJbtR.Items.Add(reader.GetString(1));
                cmbJbtUsr.Items.Add(reader.GetString(0));
                //    //a++;
            }
            koneksi.closeConnection();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Enter(object sender, EventArgs e)
        {

            
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Tooltip text goes here\r\nTestingtesting\r\n", btnBooking);
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(btnBooking);
        }

        private void keluarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        protected void Kamar_Tips(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            //toolTip1.Show(
            //    "----------------------------------------------------------------------------\r\n" +
            //    "                       Booking ##########\r\n Dipesan oleh ##########\r\n" +
            //    "----------------------------------------------------------------------------\r\n" +
            //    "Kamar ## Tamu ########### Checkin ##-##-## Checkout ##-##-##\r\n" +
            //    "Kamar ## Tamu ########### Checkin ##-##-## Checkout ##-##-##\r\n" +
            //    "----------------------------------------------------------------------------\r\n"
            //    , btn);

        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    //if (panelCheckinDate.Visible == false)
        //    //{
        //    //    panelCheckinDate.Visible = true;
        //    //    panelCheckoutDate.Visible = true;
        //    //    panelKamar.BringToFront();
        //    //}else
        //    //{
        //    //    panelCheckinDate.Visible = false;
        //    //    panelCheckoutDate.Visible = false;
        //    //}r


        //    refresh_kamar();
        //    panelCheckinDate.Visible = true;
        //    panelCheckoutDate.Visible = true;
        //    panelKamar.BringToFront();
        //    panelKamarDibooking.Controls.Clear();
        //    groupBukuTamu.SendToBack();
        //    panelDataTamu.Enabled = true;
        //}

        private void button1_MouseEnter_1(object sender, EventArgs e)
        {
            toolTip1.Show(
                "----------------------------------------------------------------------------\r\n" +
                "                       Booking ##########\r\n Dipesan oleh ##########\r\n" +
                "----------------------------------------------------------------------------\r\n" +
                "Kamar ## Tamu ########### Checkin ##-##-## Checkout ##-##-##\r\n" +
                "Kamar ## Tamu ########### Checkin ##-##-## Checkout ##-##-##\r\n" +
                "----------------------------------------------------------------------------\r\n"              
                , btnBooking);
            //Thread.Sleep(1000);
        }

        protected void button1_MouseLeave_1(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            toolTip1.Hide(btn);
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        //private void button1_Click_1(object sender, EventArgs e)
        //{
        //    if (button1.FlatAppearance.BorderSize == 1)
        //    {
        //        button1.FlatAppearance.BorderSize = 2;
        //        button1.Font = new Font(button1.Font, FontStyle.Bold);
        //    }else
        //    {
        //        button1.FlatAppearance.BorderSize = 1;
        //        button1.Font = new Font(button1.Font, FontStyle.Regular);

        //    }
        //}

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnKalender_Click(object sender, EventArgs e)
        {
            dataGridView3.Enabled = true;

            refreshActivatedButton();
            btnKalender.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnKalender.FlatAppearance.BorderSize = 2;

            HideBtnStatusKamar();
            panel4.Visible = false; //tambahBaru
            panelKamarDibooking.Controls.Clear();
            //irwan tambahkan
            DataTamuKalender.Visible = false;
            hidepanelPengaturanKamar();
            panelPengaturanKamar.SendToBack();
            //irwan tambahkan
            cekPilih = true;
            flowLayoutPanel1.Visible = true;
            flowLayoutPanel4.Visible = false;
            dataGridView3.BringToFront();
            //loadKalender(7, 2014);
            loadKalender(DateTime.Now.Month,DateTime.Now.Year);
            //panelKalender.BringToFront();
            //loadKalender(,);
            //comboBox2.Text = "Juli";//comboBox2.Items[0].ToString();
            //MessageBox.Show(DateTime.Now.Month.ToString());
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            switch (DateTime.Now.Month)
            {
                case 1: comboBox2.Text = "Januari"; break;
                case 2: comboBox2.Text = "Februari"; break;
                case 3: comboBox2.Text = "Maret"; break;
                case 4: comboBox2.Text = "April"; break;
                case 5: comboBox2.Text = "Mei"; break;
                case 6: comboBox2.Text = "Juni"; break;
                case 7: comboBox2.Text = "Juli"; break;
                case 8: comboBox2.Text = "Agustus"; break;
                case 9: comboBox2.Text = "September"; break;
                case 10: comboBox2.Text = "Oktober"; break;
                case 11: comboBox2.Text = "November"; break;
                default: comboBox2.Text = "Desember"; break;
            }
            comboBox3.Text = DateTime.Now.Year.ToString();
            dataGridView3.ReadOnly = true;
            dataGridView3.AllowUserToOrderColumns = false;

           //end irwan
            //panelCheckinDate.Visible = false;
            //panelCheckoutDate.Visible = false;
            //panelKamarDibooking.Visible = false; 
            hideBookingElement();
            flowLayoutPanel1.Visible = true;
            flowLayoutPanel4.Visible = false;
        }

        private void btnDaftarBooking_Click(object sender, EventArgs e)
        {
            panelDaftarBooking.BringToFront();
            panelCheckinDate.Visible = false;
            panelCheckoutDate.Visible = false;
        }
        private void HideBtnStatusKamar()
        {
            btnBookingStatus.Visible = false;

        }

        private void refreshActivatedButton()
        {
            btnLaporanWNA.FlatAppearance.BorderColor = Color.Black;
            btnLaporanWNA.FlatAppearance.BorderSize = 1;
                        
            btnBooking.FlatAppearance.BorderColor = Color.Black;
            btnBooking.FlatAppearance.BorderSize = 1;

            btnLaporan.FlatAppearance.BorderColor = Color.Black;
            btnLaporan.FlatAppearance.BorderSize = 1;
                   
            btn_Konfigurasi.FlatAppearance.BorderColor = Color.Black;
            btn_Konfigurasi.FlatAppearance.BorderSize = 1;
            //btn_pendapatanRestoran

            btnLaporanGrandTotal.FlatAppearance.BorderColor = Color.Black;
            btnLaporanGrandTotal.FlatAppearance.BorderSize = 1;
                            
            btn_pendapatanRestoran.FlatAppearance.BorderColor = Color.Black;
            btn_pendapatanRestoran.FlatAppearance.BorderSize = 1;
            //btn_pendapatanRestoran
            btnKalender.FlatAppearance.BorderColor = Color.Black;
            btnPesan.FlatAppearance.BorderColor = Color.Black;
            btnCheckInStatus.FlatAppearance.BorderColor = Color.Black;
            btnKamarMaintenance.FlatAppearance.BorderColor = Color.Black;
            btnDaftarTamu.FlatAppearance.BorderColor = Color.Black;
            btnUser.FlatAppearance.BorderColor = Color.Black;
            btnRights.FlatAppearance.BorderColor = Color.Black;
            btn_restoran.FlatAppearance.BorderColor = Color.Black;
            btnPengaturanKamar.FlatAppearance.BorderColor = Color.Black;
            btn_historis.FlatAppearance.BorderColor = Color.Black;
            btnPengaturanHotel.FlatAppearance.BorderColor = Color.Black;
            btnPengaturanHarga.FlatAppearance.BorderColor = Color.Black;
            btn_harga_khusus.FlatAppearance.BorderColor = Color.Black;
            btnPeriodik.FlatAppearance.BorderColor = Color.Black;
            btn_bookingHangus.FlatAppearance.BorderColor = Color.Black;
            btn_pengaturan_item.FlatAppearance.BorderColor = Color.Black;
            btnLaporanKeuangan.FlatAppearance.BorderColor = Color.Black;
            btnUtng.FlatAppearance.BorderColor = Color.Black;
            btnRekapHariIni.FlatAppearance.BorderColor = Color.Black;
            
            btnBooking.FlatAppearance.BorderSize = 1;
            btnKalender.FlatAppearance.BorderSize = 1;
            btnPesan.FlatAppearance.BorderSize = 1;
            btnCheckInStatus.FlatAppearance.BorderSize = 1;
            btnKamarMaintenance.FlatAppearance.BorderSize = 1;
            btnDaftarTamu.FlatAppearance.BorderSize = 1;
            btnUser.FlatAppearance.BorderSize = 1;
            btnRights.FlatAppearance.BorderSize = 1;
            btn_restoran.FlatAppearance.BorderSize = 1;
            btnPengaturanKamar.FlatAppearance.BorderSize = 1;
            btn_historis.FlatAppearance.BorderSize = 1;
            btnPengaturanHotel.FlatAppearance.BorderSize = 1;
            btnPengaturanHarga.FlatAppearance.BorderSize = 1;
            btn_harga_khusus.FlatAppearance.BorderSize = 1;
            btnPeriodik.FlatAppearance.BorderSize = 1;
            btn_bookingHangus.FlatAppearance.BorderSize = 1;
            btn_pengaturan_item.FlatAppearance.BorderSize = 1;
            btnLaporanKeuangan.FlatAppearance.BorderSize = 1;
            btnUtng.FlatAppearance.BorderSize = 1;
            btnLaporanTopCorp.FlatAppearance.BorderSize = 1;
            btnRekapHariIni.FlatAppearance.BorderSize = 1;

            flowLayoutPanel4.Visible = false;
            flowLayoutGrandTotalInput.Visible = false;

            btnLaporanTopCorp.FlatAppearance.BorderColor = Color.Black;
            btnLaporanTopCorp.FlatAppearance.BorderSize = 1;
            
        }

        private void refreshDaftartamu()
        {
            resetBtnLaporan();
            resetBtnKonfigurasi();
            refreshdataTamu();
            refreshActivatedButton();
            btnDaftarTamu.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnDaftarTamu.FlatAppearance.BorderSize = 2;

            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            panelDaftarTamu.BringToFront();
            //panelCheckinDate.Visible = false;
            //panelCheckoutDate.Visible = false;
            refreshGridDataTamu(GridViewDaftarTamu);
        
        }

        private void btnDaftarTamu_Click(object sender, EventArgs e)
        {
            refreshDaftartamu();
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        protected void button1_MouseEnter_2(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            toolTip1.Show(
                "----------------------------------------------------------------------------\r\n" +
                "                       Booking ########## Dipesan oleh ##########\r\n" +
                "----------------------------------------------------------------------------\r\n" +
                "Kamar ## Tamu ########### Checkin ##-##-## Checkout ##-##-##\r\n" +
                "Kamar ## Tamu ########### Checkin ##-##-## Checkout ##-##-##\r\n" +
                "----------------------------------------------------------------------------\r\n"
                , btn);

        }

        private void checkinDate_ValueChanged(object sender, EventArgs e)
        {
            //checkinDate.MinDate = DateTime.Today;
            checkoutDate.MinDate = checkinDate.Value.AddDays(1);
            cekBooking();
            refresh_kamar();
        }

        public void cancelKamar(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            List<DataRow> rd = new List<DataRow>();
            int index = 0;
            foreach (DataRow dr in dKamarPesan.Rows)
            {
                if (dr["NO Kamar"].ToString().Equals(btn.Name))
                {
                    rd.Add(dr);
                }

                index += 1;
            }

            foreach (var r in rd)
            {
                dKamarPesan.Rows.Remove(r);
            }
            dKamarPesan.AcceptChanges();
            comboBox4.Items.Clear();

            foreach (DataRow dr in dKamarPesan.Rows)
            {
                comboBox4.Items.Add("Down payment "+dr["NO Kamar"].ToString());
            }
            int hargaKamarcancel = 0;
            foreach (Control x in panelKamarDibooking.Controls)
            {
                if (x is Label)
                {
                    if (((Label)x).Name.ToString().Equals(btn.Name.ToString() + "Checkin"))
                    {
                        hargaKamarcancel = Int32.Parse(((Label)x).Tag.ToString());
                        panelKamarDibooking.Controls.Remove((Label)x);
                    }
                }
            }
            int biaya = 0;
            foreach (Control x in panelKamarDibooking.Controls)
            {
                if (x is Label)
                {
                    if (((Label)x).Name.ToString().Equals("lblTotalBooking"))
                    {
                        biaya = Int32.Parse((Int32.Parse(((Label)x).Tag.ToString()) - hargaKamarcancel).ToString());
                        ((Label)x).Tag = biaya.ToString();
                        totalBiaya = biaya;
                        ((Label)x).Text = "----------------------------------------------------\n\r"
                        + "Grand Total : Rp." + biaya.ToString() + ",00\n\r"
                        + "----------------------------------------------------";
                        if (biaya == 0)
                        {
                            panelKamarDibooking.Controls.Remove((Label)x);
                        }
                    }
                }
            }
            foreach (Control x in panelKamarDibooking.Controls)
            {
                if (x is Button)
                {
                    if (((Button)x).Name.ToString().Equals("addBooking"))
                    {
                        if (biaya == 0)
                        {
                            panelKamarDibooking.Controls.Remove((Button)x);
                        }
                    }
                }
            }

            panelKamarDibooking.Controls.Remove(btn);

        }
        private void tambah_kamar(object sender, EventArgs e)
        {
            Button btn = sender as Button;


            //
            btn.Visible = false;
            Button kamarDibooking = new Button();
            kamarDibooking.Name = btn.Name;
            kamarDibooking.Text = btn.Text;
            kamarDibooking.Margin = new Padding(3, 3, 3, 3);
            kamarDibooking.BackColor = btn.BackColor;
            kamarDibooking.Height = btn.Height;
            kamarDibooking.Width = btn.Width;
            kamarDibooking.Image = btn.Image;
            kamarDibooking.ImageAlign = btn.ImageAlign;
            kamarDibooking.FlatStyle = btn.FlatStyle;
            
            kamarDibooking.Click += new EventHandler(cancelKamar);

            panelKamarDibooking.Controls.Add(kamarDibooking);

            SqlCommand sqlC = new SqlCommand("select kamar_tipe_id from Kamar where kamar_no = @no", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@no", btn.Name);
            string nilai = sqlC.ExecuteScalar().ToString();
            koneksi.closeConnection();

            sqlC = new SqlCommand("select case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end hargaK from Harga where kamar_tipe_id = @tipe and tanggal_id >= @chin and tanggal_id< @chou", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@tipe", Int32.Parse(nilai));
            sqlC.Parameters.AddWithValue("@chin", checkinDate.Value.Date);
            sqlC.Parameters.AddWithValue("@chou", checkoutDate.Value.Date);
            SqlDataReader readC = sqlC.ExecuteReader();

            int biayaKamar = 0;
            int hargaPertama = 0; int ctrPertama = 0;
            while (readC.Read())
            {
                if (ctrPertama == 0)
                {
                    hargaPertama = Int32.Parse(readC["hargaK"].ToString());
                }
                biayaKamar += Int32.Parse(readC["hargaK"].ToString());
                ctrPertama += 1;
            }
            koneksi.closeConnection();
            //irwan tambahkan

            DataRow dr = dKamarPesan.NewRow();
            dr["NO Kamar"] = btn.Name;
            dr["Checkin"] = checkinDate.Value.Date;
            dr["Checkout"] = checkoutDate.Value.Date;
            dr["Tamu"] = "";
            dr["Harga"] = biayaKamar;
            dKamarPesan.Rows.Add(dr);
            //item.Text = "Down payment " + btn.Name;
            //item.Value = btn.Name;
            comboBox4.Items.Add("Down payment " + btn.Name);
            //comboBox4.Text = "Down payment " + btn.Name;
            //end irwan


            //
            btn.Visible = false;
            //Button kamarDibooking = new Button();

            Label lblcheckin = new Label();
            lblcheckin.Name = btn.Name + "Checkin";
            lblcheckin.Width = 200;
            lblcheckin.Height = 70;
            lblcheckin.Text =
                "Checkin                    : " + checkinDate.Value.ToString("yyyy-M-d") + "\n\r"
               + "Checkout                 : " + checkoutDate.Value.ToString("yyyy-M-d") + "\n\r"
               + "Jumlah hari               : " + (checkoutDate.Value.Date - checkinDate.Value.Date).TotalDays.ToString() + " hari\n\r"
               + "Harga kamar satuan : Rp. " + hargaPertama.ToString() + ",00\n\r"
               + "Harga kamar total     : Rp. " + biayaKamar.ToString() + ",00\n\r"
               + lblcheckin.Text;
            lblcheckin.Tag = biayaKamar.ToString();
            panelKamarDibooking.Controls.Add(lblcheckin);
            Int32 totalBooking = 0;

            try
            {
                Button removeaddBooking = ((Button)panelKamarDibooking.Controls.Find("addBooking", true)[0]);
                if (removeaddBooking != null) panelKamarDibooking.Controls.Remove(removeaddBooking);
            }
            catch
            {
            }


            try
            {
                Label removelblTotalBooking = ((Label)panelKamarDibooking.Controls.Find("lblTotalBooking", true)[0]);
                if (removelblTotalBooking != null)
                {
                    totalBooking = +Convert.ToInt32(removelblTotalBooking.Tag);
                    panelKamarDibooking.Controls.Remove(removelblTotalBooking);
                }
            }
            catch
            {
            }

            Label lblTotalBooking = new Label();
            lblTotalBooking.Name = "lblTotalBooking";
            lblTotalBooking.Height = 50;
            lblTotalBooking.Width = 300;
            lblTotalBooking.Margin = new Padding(3, 10, 3, 3);

            lblTotalBooking.Tag = (Int32.Parse(biayaKamar.ToString()) + totalBooking).ToString();
            lblTotalBooking.Text =
                "----------------------------------------------------\n\r"
                + "Grand Total : Rp." + lblTotalBooking.Tag.ToString() + ",00\n\r"
                + "----------------------------------------------------";
            panelKamarDibooking.Controls.Add(lblTotalBooking);
            lblTotalBooking.Text = lblTotalBooking.Text;
            //irwan tambahkan
            totalBiaya = Int32.Parse(lblTotalBooking.Tag.ToString());
            //end irwan

            Button addBooking = new Button();
            addBooking.Name = "addBooking";
            //panelKamarDibooking.Controls.Remove("addBooking");
            //LinkLabel lbls = (LinkLabel)sender;
            //this.Controls.Remove((LinkLabel)sender);
            //this.Controls.Remove((TextBox)lbls.Tag);
            addBooking.Width = 150;
            addBooking.Text = "Registrasi Kamar";
            addBooking.Click += new EventHandler(addBooking_DataTamu);
            panelKamarDibooking.Controls.Add(addBooking);

            //panelKamarDibooking.Controls.Find("addBooking").Visible = false;
            //check51.Visible = false;
            //check51.remov
            //((TextBox)frm.Controls.Find("controlName", true)[0]).Text = "yay";

        }

        private void hidepanelPengaturanKamar()
        {
            try
            {
                Form pengaturanKamarInner = ((Form)splitContainer2.Panel1.Controls.Find("panelPengaturanKamarInnerForm", true)[0]);
                pengaturanKamarInner.Visible = false;
                splitContainer2.Panel1.BringToFront();
            }
            catch
            { }
        }

        private void showpanelPengaturanKamar()
        {
            try
            {
                Form pengaturanKamarInner = ((Form)splitContainer2.Panel1.Controls.Find("panelPengaturanKamarInnerForm", true)[0]);
                pengaturanKamarInner.Visible = true;
            }
            catch
            { }

        }

        private void addBooking_DataTamu(object sender, EventArgs e)
        {
            Button addBooking = sender as Button;
            addBooking.Visible = false;
            //panelKamarDibooking.Enabled = false;
            panelDataTamu.BringToFront();
            panelKamarDibooking.Enabled = false;

            comboBox4.Items.Add("OTA - Tanpa Batas Waktu");
            
            comboBox4.Items.Add("Lunas");
            comboBox4.SelectedIndex = 0;
            //comboBox4.SelectedItem = "Lunas";
        }

        private void checkoutDate_ValueChanged(object sender, EventArgs e)
        {
            //checkoutDate.MinDate = DateTime.Today.AddDays(1);
            cekBooking();
            refresh_kamar();
        }

        public void cekBooking()
        {
            ////btnBooking.Text = (checkoutDate.Value.Date - checkinDate.Value.Date).ToString();//.ToString();
            //string query = "select kamar_no from Reservasi"; //where (@chIn <= checkin and @chOt >= checkin) or (@chIn <= checkout and @chOt >= checkout) ";
            //////Form3.
            //conn.Open();
            //cmd = new SqlCommand(query, conn);
            ////cmd.Parameters.AddWithValue("@chIn", checkinDate.Value.Date);
            ////cmd.Parameters.AddWithValue("@chOt", checkoutDate.Value.Date);
            //reader = cmd.ExecuteReader();
            //conn.Close();   
        }

        private void panelDataTamu_Paint(object sender, PaintEventArgs e)
        {

        }

        static string switchHari(int hari)
        {
            switch (hari)
            {
                case 0: return "Minggu"; break;
                case 1: return "Senin"; break;
                case 2: return "Selasa"; break;
                case 3: return "Rabu"; break;
                case 4: return "Kamis"; break;
                case 5: return "Jum'at"; break;
                default: return "Sabtu"; break;                
            }            
        }

        Boolean booking_notif = false;
        Boolean cek_kamarTersedia = true;
        configconn koneksi2 = new configconn();

        Boolean AutoLogout = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(DateTime.Now.Second % 59 == 0){
                for (int i = 0; i < 4;i++ )
                {
                    if(jamLogout[i].Hour == DateTime.Now.Hour && jamLogout[i].Minute == DateTime.Now.Minute){
                        AutoLogout = true;
                    }
                }
                //Console.Write(AutoLogout);
            }

            if ((DateTime.Now.Hour == 12 || DateTime.Now.Hour == 18)  && DateTime.Now.Minute == 00 && DateTime.Now.Second == 0
                )
            {
                refreshDaftartamu();
                cmd = new SqlCommand((@"select 
                                    count(*) jmlUlangtahun--a.spending,a.checkout_terakhir,t.*
                                    from 
                                    (
	                                    select 
	                                    *
	                                    from 
	                                    tamu t
	                                    where 
	                                    day(tanggallahir) = day(GETDATE())
	                                    and 
	                                    month(tanggallahir) = month(getdate())
                                    )a
            "), koneksi.KoneksiDB());

                int jumUlangTahun = (int)cmd.ExecuteScalar();
                koneksi.closeConnection();
            

                //DialogResult dialogResult = MessageBox.Show("Hari ini terdapat " + jumUlangTahun.ToString() + " tamu berulang tahun.", "Peringatan Ulang Tahun Tamu", MessageBoxButtons.OK);
                //if (dialogResult == DialogResult.OK)
                //{
                //    btnUlangtahun_Click(sender, e);
                //}
            }

            if (AutoLogout)
            {
                int idleTime = 0;
                LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
                lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
                lastInputInfo.dwTime = 0;

                int envTicks = Environment.TickCount;

                if (GetLastInputInfo(out lastInputInfo))
                {
                    int lastInputTick = lastInputInfo.dwTime;
                    idleTime = envTicks - lastInputTick;
                }

                int a;

                if (idleTime > 0)
                    a = idleTime / 1000;
                else
                    a = idleTime;

                if (a % 150 == 0 && a > 0)
                {
                    //MessageBox.Show("Sistem akan Logout otomatis dalam " + (600 - a) / 60 + " menit ");
                }

                if (a > 600)
                {
                    this.Close();
                }
            }


            statusTime.Text = "Hari " +
                switchHari((int)DateTime.Now.DayOfWeek)
                    + " Tanggal " +
                DateTime.Now.ToString("d-M-yyyy") + " Jam " + DateTime.Now.ToString("HH:mm:ss");

            int jumKamar = 0;
            if //(1==0)///blok ini tidak untuk dijalankan sebelum Data Access Layer selesai
                (DateTime.Now.Second % 27 == 0 && cek_kamarTersedia)
            {

                
                //Tanggal ##-##-#### Jam ##:##
                SqlCommand sql = new SqlCommand("select count(*) from Reservasi where GETDATE() between checkin and checkout", koneksi2.KoneksiDB());
                int jmlKamarTerbooking = Int32.Parse(sql.ExecuteScalar().ToString());
                koneksi2.KoneksiDB().Close();

                sql = new SqlCommand("SELECT count(*) FROM Kamar", koneksi2.KoneksiDB());
                int jmlKamarTersedia = Int32.Parse(sql.ExecuteScalar().ToString()) - jmlKamarTerbooking;
                koneksi2.KoneksiDB().Close();

                statusJumlahKamarTersedia.Text = "Jumlah Kamar Tersedia " + jmlKamarTersedia.ToString();

                statusJumlahKamarTerbooking.Text = "Jumlah Kamar Terbooking " + jmlKamarTerbooking.ToString();

                //sql = new SqlCommand("SELECT count(*) FROM Kamar where status is not null", koneksi.KoneksiDB());
                //int nomROR = Int32.Parse(sql.ExecuteScalar().ToString());
                //koneksi.closeConnection();
                decimal ROR = jmlKamarTerbooking * 100 / (jmlKamarTerbooking + jmlKamarTersedia);
                statusROR.Text = "Room Occupancy Rate " + ROR.ToString() + "%";                
            }

            if (DateTime.Now.Minute % 5 == 0 && DateTime.Now.Second == 30)
            {

                //Notif when Now-tgl_booking > 180 minutes
                //Check booking hangus
                SqlCommand sql = new SqlCommand("Select b.booking_id from reservasi r inner join booking b on r.booking_id=b.booking_id where r.status='booking' and (datediff(minute,b.tgl_booking,SYSDATETIME())>180) group by b.booking_id having SUM(r.downpayment)<=0", koneksi2.KoneksiDB());
                int bookinghangus = 0;
                reader = sql.ExecuteReader();
                while (reader.Read())
                {
                    bookinghangus++;
                }

                if (bookinghangus > 0)
                {
                    booking_notif = true;
                }
                else
                {
                    booking_notif = false;
                }
                koneksi2.closeConnection();

                sql = new SqlCommand("SELECT jam_checkout FROM IDHotel", koneksi2.KoneksiDB());
                String jamCheckout = sql.ExecuteScalar().ToString();
                koneksi2.closeConnection();
                DateTime dateTimeCheckout = DateTime.ParseExact(jamCheckout, "HH:mm:ss", CultureInfo.InvariantCulture);
                dateTimeCheckout -= new TimeSpan(0, 30, 0);
                jumKamar = 0;
                if (DateTime.Now.Hour == dateTimeCheckout.Hour && DateTime.Now.Minute >= dateTimeCheckout.Minute)
                {
                    SqlCommand cmd = new SqlCommand((@"select count(*) from Kamar, Reservasi where Kamar.kamar_no = Reservasi.kamar_no and Reservasi.status = 'checkin' and convert(date,Reservasi.checkout)=convert(date, SYSDATETIME())"), koneksi2.KoneksiDB());
                    jumKamar = (int)cmd.ExecuteScalar();
                    //Console.WriteLine(jumKamar);      
                    koneksi2.closeConnection();        
                }
                if (jumKamar > 0)
                {
                    //if (DateTime.Now.Second % 60 == 0)
                    //{
                        MessageBox.Show("Kamar yang harus checkout hari ini (" + jumKamar + ") kamar");
                    //}
                }
            }

            if (booking_notif)
            {
                if (DateTime.Now.Second % 2 == 0)
                {
                    btn_bookingHangus.BackColor = Color.Red;
                }
                else
                {
                    btn_bookingHangus.BackColor = Control.DefaultBackColor;
                }
            }
            else
            {
                btn_bookingHangus.BackColor = Control.DefaultBackColor;
            }
         
        }

        private void groupBukuTamu_Paint(object sender, PaintEventArgs e)
        {
            groupBukuTamu.Visible = false;
        }

        private void btnCariTamu_Click(object sender, EventArgs e)
        {
            groupBukuTamu.Visible = true;
            groupBukuTamu.Height = 500;
            //groupBukuTamu.Dock = DockStyle.Top;
            groupBukuTamu.BringToFront();
            //panelDataTamu.Enabled = false;
            btnKonfirmasiBooking.Enabled = false;
            inputCariNamaTamu.Text = "";

            refreshGridDataTamu(datagridTamu);
        }

        private void refreshGridDataTamu(DataGridView dg)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter("select tanggallahir, tamu, alamat, kota, telepon, email, perusahaan, sebutan, gelar,noidentitas,jenisidentitas,warganegara,month(tanggallahir) bulan_lahir,day(tanggallahir) hari_lahir,tamu_id from tamu where len(tamu+perusahaan) > 4 order by tamu", koneksi.KoneksiDB()); //c.con is the connection string
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            dg.ReadOnly = true;
            dg.DataSource = ds.Tables[0];
            
            koneksi.closeConnection();
        }

        private void panelDataTamu_MouseClick(object sender, MouseEventArgs e)
        {
            groupBukuTamu.Visible = false;
            groupBukuTamu.SendToBack();
            btnKonfirmasiBooking.Enabled = true;
        }

        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            ////if (groupBukuTamu.Visible == true)
            ////{
            //    inputNamaTamu.Text = datagridTamu[1, e.RowIndex].Value.ToString();
            ////}
            //groupBukuTamu.Visible = false;
            //groupBukuTamu.SendToBack();
            
            //    btnKonfirmasiBooking.Enabled = true;
        }

        private void KosongkanInput()
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(Controls);
        }

       // void this_StartupNextInstance(object sender,
       //StartupNextInstanceEventArgs e)
       // 
       //     FormUtama form = MainForm as Form1; //My derived form type
       //     form.ShowForm();
       // }
        
        private void btnBooking_Click(object sender, EventArgs e)
        {
            picIdentitas.Image = null;

            videoSourcePlayer.Visible = false;
            SqlCommand sqlttd = new SqlCommand("select tandatangan from IDHotel", koneksi.KoneksiDB());
            string b = sqlttd.ExecuteScalar().ToString();
            koneksi.closeConnection();

            if (b.Equals("Ya"))
            {
                btn_tandatangan.Visible = true;
            }
            else
            {
                btn_tandatangan.Visible = false;
            }


            txtWargaNegara.Text = "";
            resetBtnKonfigurasi();
            resetBtnLaporan();
            checkinDate.Value = DateTime.Today;
            checkoutDate.Value = checkinDate.Value.AddDays(1);
            comboboxPembayaranBooking.Enabled = true;
            txtNoIdentitas.Text = "";
            cbJnsIdentitas.Text = "";
            diskonAngka.Text = "0";
            DiskonPersen.Text = "0";
            panelKamarDibooking.AutoScroll = false;
            panelKamarDibooking.HorizontalScroll.Enabled = false;
            panelKamarDibooking.HorizontalScroll.Visible = false;
            panelKamarDibooking.AutoScroll = true;

            inputUlangTahun.Value = Convert.ToDateTime("01-01-1900");
            refreshActivatedButton();
            btnBooking.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnBooking.FlatAppearance.BorderSize = 2;
            
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            dKamarPesan.Clear(); //tambahBaru
            HideBtnStatusKamar();
            panelKamarDibooking.Visible = true;
            comboboxPembayaranBooking.Text = comboboxPembayaranBooking.Items[0].ToString();
            comboBox4.Items.Clear();
             
            refresh_kamar();
            panelCheckinDate.Visible = true;
            panelCheckoutDate.Visible = true;
            panelKamar.BringToFront();
            panelPengaturanKamar.SendToBack();
            panelKamarDibooking.Controls.Clear();
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            groupBukuTamu.SendToBack();
            //panelDataTamu.Enabled = true;
            groupBukuTamu.Refresh();
            panelDataTamu.Refresh();
            groupBukuTamu.Visible = false;
            //groupBox2.Refresh();
            //inputNamaTamu.Text = "";
            //groupBukuTamu.Invalidate();
            //groupBukuTamu.Update();
            //groupBukuTamu.Refresh();
            //Application.DoEvents();
            KosongkanInput();
            hidepanelPengaturanKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;

            cb_diskon.Checked = false;
        }

        private void datagridTamu_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void datagridTamu_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            //if (groupBukuTamu.Visible == true)
            //{
            inputNamaTamu.Text = datagridTamu[1, e.RowIndex].Value.ToString();
            inputEmail.Text = datagridTamu[5, e.RowIndex].Value.ToString();
            inputAlamat.Text = datagridTamu[2, e.RowIndex].Value.ToString();
            inputKota.Text = datagridTamu[3, e.RowIndex].Value.ToString();
            input_perusahaan.Text = datagridTamu[6, e.RowIndex].Value.ToString();
            inputTelepon.Text = datagridTamu[4, e.RowIndex].Value.ToString();

            inputSebutan.Text = datagridTamu[7, e.RowIndex].Value.ToString();
            inputGelar.Text = datagridTamu[8, e.RowIndex].Value.ToString();
     //       MessageBox.Show(datagridTamu[9, e.RowIndex].Value.ToString());
            txtNoIdentitas.Text = datagridTamu[9, e.RowIndex].Value.ToString();
            cbJnsIdentitas.Text = datagridTamu[10, e.RowIndex].Value.ToString();

            if (input_perusahaan.Text == "")
            {
                cb_diskon.Checked = false;
            }
            else
            {
                cb_diskon.Checked = true;
            }

            //irwan tambahkan
            dataCustomer = Int32.Parse(datagridTamu[14, e.RowIndex].Value.ToString());
            //end irwan
            //}
            groupBukuTamu.Visible = false;
            groupBukuTamu.SendToBack();

            btnKonfirmasiBooking.Enabled = true;

        }
        //irwan tambahkan
        private void isCombobox3()
        {
            ComboboxItem item = new ComboboxItem();
          //  MessageBox.Show("A");
            int tahunTampil = 2008;
            for (int i = 0; i < 10; i++)
            {
                item = new ComboboxItem();
                item.Text = tahunTampil.ToString();
                item.Value = tahunTampil;
                comboBox3.Items.Add(item);
                cb_tahunLaporan.Items.Add(item);
                comboBoxGrandTotalTahun.Items.Add(item);
                
                tahunTampil = tahunTampil + 1;
            }
            comboBox3.Text = comboBox3.Items[0].ToString();
            cb_tahunLaporan.Text = cb_tahunLaporan.Items[0].ToString();
            comboBoxGrandTotalTahun.Text = comboBoxGrandTotalTahun.Items[0].ToString();

        }
        private void isiCombobox()
        {

            ComboboxItem item = new ComboboxItem();

            item.Text = "Januari";
            item.Value = 1;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "Februari";
            item.Value = 2;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);
            
            item = new ComboboxItem();
            item.Text = "Maret";
            item.Value = 3;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item); 
            comboBoxGrandTotalBulan.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "April";
            item.Value = 4;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "Mei";
            item.Value = 5;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "Juni";
            item.Value = 6;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "Juli";
            item.Value = 7;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "Agustus";
            item.Value = 8;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "September";
            item.Value = 9;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "Oktober";
            item.Value = 10;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "November";
            item.Value = 11;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "Desember";
            item.Value = 12;
            comboBox2.Items.Add(item);
            cb_bulanLaporan.Items.Add(item);
            comboBoxGrandTotalBulan.Items.Add(item);

            cb_bulanLaporan.Text = cb_bulanLaporan.Items[0].ToString();
            comboBox2.Text = comboBox2.Items[0].ToString();
        }
        DataTable dt;
        //private void loadKalender(int bulan, int tahun)
        //{
        //    TglBulan = bulan;
        //    Tgltahun = tahun;
        //    dt = new DataTable();
        //    SqlCommand sql = new SqlCommand("select tanggal from Tanggal where bulan = @bln and tahun = @thn", koneksi.KoneksiDB());
        //    sql.Parameters.AddWithValue("@bln", bulan);
        //    sql.Parameters.AddWithValue("@thn", tahun);

        //    dataGridView3.DataSource = dt;
        //    dataGridView3.AllowUserToAddRows = false;
        //    dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        //    dt.Columns.Add("NO_KAMAR".ToString());
        //    reader = sql.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        dt.Columns.Add(reader["tanggal"].ToString());
        //    }

        //    dataGridView3.DataSource = dt;
        //    koneksi.closeConnection();

        //    sql = new SqlCommand("select kamar_no from Kamar", koneksi.KoneksiDB());
        //    reader = sql.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        DataRow dr = dt.NewRow();
        //        dr["NO_Kamar"] = reader["kamar_no"].ToString();
        //        dt.Rows.Add(dr);

        //    }
        //    koneksi.closeConnection();
        //    dataGridView3.Columns[0].Width = 50;
        //    DateTime tanggalPesan;

        //    ComboboxItem selectedCar = (ComboboxItem)comboBox2.SelectedItem;

        //    for (int i = 1; i <= dataGridView3.ColumnCount-1; i++)
        //    {
        //        dataGridView3.Columns[i].Width = 20;            
        //        tanggalPesan = Convert.ToDateTime(bulan + "/" + dataGridView3.Columns[i].Name.ToString() + "/" + tahun);
        //        sql = new SqlCommand("select kamar_no, status from Reservasi where checkin <@id and checkout >= @id and (status='booking' or status='checkin')", koneksi.KoneksiDB());
        //        sql.Parameters.AddWithValue("@id", tanggalPesan);
        //        reader = sql.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            int nilai = 0;
        //            foreach (DataGridViewRow row in this.dataGridView3.Rows)
        //            {
        //                if (row.Cells[0].Value.ToString().Equals(reader["kamar_no"].ToString()) && reader["status"].ToString().Equals("booking"))
        //                {
        //                    dataGridView3.Rows[nilai].Cells[i-1].Style.BackColor = Color.Red;
        //                }
        //                else if (row.Cells[0].Value.ToString().Equals(reader["kamar_no"].ToString()) && reader["status"].ToString().Equals("checkin"))
        //                {
        //                    dataGridView3.Rows[nilai].Cells[i-1].Style.BackColor = Color.Green;

        //                }
        //                nilai += 1;
        //            }
        //        }
        //        koneksi.closeConnection();
        //    }


        //}

        private void loadKalender(int bulan, int tahun)
        {
            TglBulan = bulan;
            Tgltahun = tahun;


            string sql = @"select
            *
            from
            (
	            select
	            combine.kamar_no no,DATEPART(dd,tanggal_id) tanggal,case when status = 'checkin' then 'OC-'+tamu when status = 'booking' then 'EA-'+tamu when status = 'checkout' then 'VD-'+tamu else '' end status
	            from
	            (
		            select 
		            kamar_no,tanggal_id
		            from 
		            Tanggal t
		            cross join 
		            Kamar
		            WHERE 
		            bulan  = " + bulan + @" and tahun = " + tahun + @"
	            )combine
	            left join 
	            (
		            select 
		            booking_id,kamar_no,DATEADD(dd,hari,convert(date,checkin)) tanggal, status, t.tamu
		            from
		            (
			            select
			            ROW_NUMBER() over(partition by reservasi_id,kamar_no order by checkin,tamu_id)-1 hari
			            ,booking_id,kamar_no,convert(date,checkin) checkin,r.status,r.tamu_id
			            from 
			            Reservasi r
			            join master..spt_values v on v.type='P'
			            and v.number between 1 
			            and datediff(dd, convert(date,checkin), convert(date,checkout))
			            where
			            (year(r.checkin) = " + tahun + @" and month(r.checkin) = " + bulan + @")
			            or
			            (year(r.checkout) = " + tahun + @" and month(r.checkout) = " + bulan + @")
		            )a, Tamu t
                    where a.tamu_id = t.tamu_id
	            )a
	            on 
	            combine.kamar_no = a.kamar_no
	            and
	            combine.tanggal_id = a.tanggal
            )s
            pivot
            (
	            max(status) for tanggal in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19],[20],[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31])
            )as piv
            order by no
            ";
            //sql.Parameters.AddWithValue("@bln", bulan);
            //sql.Parameters.AddWithValue("@thn", tahun);

            //SqlConnection connection = new SqlConnection(connectionString);
            SqlDataAdapter dataadapter = new SqlDataAdapter(sql, koneksi.KoneksiDB());
            DataSet ds = new DataSet();
            //connection.Open();
            dataadapter.Fill(ds, "reservasi");
            //connection.Close();
            koneksi.closeConnection();

            //dataGridView3.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            
            dataGridView3.DataSource = ds;
            dataGridView3.DataMember = "reservasi";
            dataGridView3.AllowUserToOrderColumns = false;

            dataGridView3.Columns[0].Frozen = true;

            int row = 0;
            //dataGridView3.AutoResizeColumns();
            //dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            int counter = 0;
            foreach (DataGridViewColumn cl in this.dataGridView3.Columns)
            {
                //dataGridView3.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                //int widthCol = dataGridView3.Columns[i].Width;
                cl.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cl.SortMode = DataGridViewColumnSortMode.NotSortable;
                //cl.Width = 35;
                //dataGridView3.Columns[i].Width = 200;
                //Console.Write(i);
                counter++;
                if(counter == 1){
                    cl.Width = 35;
                }
                if (counter == Convert.ToInt32(DateTime.Today.Day) && bulan == Convert.ToInt32(DateTime.Now.Month))
                {
                    dataGridView3.FirstDisplayedScrollingColumnIndex = counter;
                }
            }

            foreach (DataGridViewRow rw in this.dataGridView3.Rows)
            {
                
                //row++;
                for (int i = 0; i < rw.Cells.Count; i++)
                {
                    string[] ketKalender = Convert.ToString(rw.Cells[i].Value).Split('-');

                    if (Convert.ToString(rw.Cells[i].Value) != string.Empty && i > 0 && ketKalender[0] == "EA")

                    //if (dataGridView3.Rows[row].Cells[i].Value.ToString().Length >= 1)//rw.Cells[i].Value != null || rw.Cells[i].Value != DBNull.Value )
                    {
                        //rw.Cells[i].Style.BackColor = Color.Red;

                        DateTime tanggalPesan1;
                        //ComboboxItem selectedCar = (ComboboxItem)comboBox2.SelectedItem;
                        int NoKamarInfo = Int32.Parse(rw.Cells[0].Value.ToString());
                        tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[rw.Cells[i].ColumnIndex].Name.ToString());

                        SqlCommand sqlq = new SqlCommand("select max(Reservasi.reservasi_id) from Reservasi where convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking')", koneksi.KoneksiDB());
                        sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
                        sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
                        string reservasiKamar = "0";
                        
                        reservasiKamar = sqlq.ExecuteScalar().ToString();
                        
                            //MessageBox.Show(NoKamarInfo + ":" + tanggalPesan1);
                        
                        koneksi.closeConnection();

                        sqlq = new SqlCommand("select sum(jumlahpayment) from pembayaran where reservasi_id=@r_id", koneksi.KoneksiDB());
                        sqlq.Parameters.AddWithValue("r_id", reservasiKamar);
                        int jmlhPayment = 0;
                        try
                        {
                            jmlhPayment = Int32.Parse(sqlq.ExecuteScalar().ToString());
                        }catch
                        { }
                        
                        koneksi.closeConnection();

                        //dataGridView3.Rows[row].Cells[i].Value = jmlhPayment;

                        if(jmlhPayment > 1){

                            dataGridView3.Rows[row].Cells[i].Style.BackColor = Color.Pink;
                        }
                        else
                        {
                            dataGridView3.Rows[row].Cells[i].Style.BackColor = Color.Red;
                        }
                    }

                    if (Convert.ToString(rw.Cells[i].Value) != string.Empty && i > 0 && ketKalender[0] == "OC")

                    //if (dataGridView3.Rows[row].Cells[i].Value.ToString().Length >= 1)//rw.Cells[i].Value != null || rw.Cells[i].Value != DBNull.Value )
                    {
                        //rw.Cells[i].Style.BackColor = Color.Red;
                        dataGridView3.Rows[row].Cells[i].Style.BackColor = Color.Green;
                    }
                    //dataGridView3.Rows[row].Cells[i].Value = DBNull;
                    if (Convert.ToString(rw.Cells[i].Value) != string.Empty && i > 0 && ketKalender[0] == "VD")

                    //if (dataGridView3.Rows[row].Cells[i].Value.ToString().Length >= 1)//rw.Cells[i].Value != null || rw.Cells[i].Value != DBNull.Value )
                    {
                        //rw.Cells[i].Style.BackColor = Color.Red;
                        dataGridView3.Rows[row].Cells[i].Style.BackColor = Color.Yellow;
                    }

                } row++;
            }
            //for (int i = 1; i <= dataGridView3.ColumnCount - 1; i++)
            //{
            //    dataGridView3.Columns[i].Width = 20;
            //}
            //dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            dataGridView3.ReadOnly = true;
        }

        private void dataGridView3_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            
            try
            {
                DateTime tanggalPesan1;
                ComboboxItem selectedCar = (ComboboxItem)comboBox2.SelectedItem;
                int NoKamarInfo;
                //tanggalPesan1 = Convert.ToDateTime(TglBulan + "/" + dataGridView3.Columns[e.ColumnIndex].Name.ToString() + "/" + Tgltahun);
                //tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[e.ColumnIndex].Name.ToString());
                tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[e.ColumnIndex].Name.ToString());

                if (dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Red || dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Pink || dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Green || dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Yellow)
                {
                    NoKamarInfo = Int32.Parse(dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString());
                    SqlCommand sqlq = new SqlCommand("select top 1 Tamu.tamu, Reservasi.checkin, Reservasi.checkout,Tamu.alamat,Tamu.kota,Tamu.telepon,Tamu.email,Booking.note from Reservasi, Tamu, Booking where Booking.booking_id = Reservasi.booking_id and Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and Reservasi.checkout > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking' or Reservasi.status='checkin' or Reservasi.status='checkout') order by Reservasi.checkin Desc", koneksi.KoneksiDB());
                    sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
                    sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
                    reader = sqlq.ExecuteReader();
                       
                    while (reader.Read())
                    {
                        //MessageBox.Show(reader.GetString(0));

                        var cell = dataGridView3.CurrentCell;
                        var cellDisplayRect = dataGridView3.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                        //reader.GetString(0), reader.GetDateTime(1).ToString("dd/MMM/yyyy"), reader.GetDateTime(2).ToString("dd/MMM/yyyy")
                        if ((dataGridView3.Height / 2) > cellDisplayRect.Y)
                        {
                            //reader.GetString(0), reader.GetDateTime(1).ToString("dd/MMM/yyyy"), reader.GetDateTime(2).ToString("dd/MMM/yyyy")
                            toolTip1.Show("----------------------------------------------------------------------------\r\n" +
                                          "                        Dipesan oleh " + reader.GetString(0) + "\r\n" +
                                          "----------------------------------------------------------------------------\r\n" +
                                          " Alamat           = " + reader["alamat"].ToString() + "\r\n" +
                                          " Kota             = " + reader["kota"].ToString() + "\r\n" +
                                          " No Telepon       = " + reader["telepon"].ToString() + "\r\n" +
                                          " Email            = " + reader["email"].ToString() + "\r\n" +
                                //"Alamat " + reader.GetString(3) + " Kota " + reader.GetString(4) + " Email " + reader.GetString(6) + "\r\n" +
                                          "-----------------------------------------------------------------------------\r\n" +
                                          " Kamar            = " + NoKamarInfo.ToString() + "\r\n" +
                                          " Checkin          = " + reader.GetDateTime(1).ToString("dd/MMM/yyyy") + " Checkout =" + reader.GetDateTime(2).ToString("dd/MMM/yyyy") + "\r\n" +
                                          " Catatan          = " + reader["note"].ToString() + "\r\n"+ 
                                          "----------------------------------------------------------------------------\r\n"
                                           ,
                            dataGridView3 ,
                            0,
                            cellDisplayRect.Y + cell.Size.Height,
                            5000);

                        }
                        else
                        {
                            toolTip1.Show("----------------------------------------------------------------------------\r\n" +
                                          "                        Dipesan oleh " + reader.GetString(0) + "\r\n" +
                                          "----------------------------------------------------------------------------\r\n" +
                                          " Alamat           = " + reader["alamat"].ToString() + "\r\n" +
                                          " Kota             = " + reader["kota"].ToString() + "\r\n" +
                                          " No Telepon       = " + reader["telepon"].ToString() + "\r\n" +
                                          " Email            = " + reader["email"].ToString() + "\r\n" +
                                //"Alamat " + reader.GetString(3) + " Kota " + reader.GetString(4) + " Email " + reader.GetString(6) + "\r\n" +
                                          "-----------------------------------------------------------------------------\r\n" +
                                          " Kamar            = " + NoKamarInfo.ToString() + "\r\n" +
                                          " Checkin          = " + reader.GetDateTime(1).ToString("dd/MMM/yyyy") + " Checkout =" + reader.GetDateTime(2).ToString("dd/MMM/yyyy") + "\r\n" +
                                          " Catatan          = " + reader["note"].ToString() + "\r\n" +
                                          "----------------------------------------------------------------------------\r\n"
                                           ,
                            dataGridView3 ,
                            0,
                            10,
                            5000);

                        }
                        
          

                        dataGridView3.ShowCellToolTips = false;
                        //label95.Text = reader.GetString(0);
                        //label96.Text = reader["alamat"].ToString();
                        //label97.Text = reader["kota"].ToString();
                        //label98.Text = reader["telepon"].ToString();
                        //label99.Text = reader["email"].ToString();
                        //label100.Text = NoKamarInfo.ToString();
                        //label101.Text = reader.GetDateTime(1).ToString("dd/MMM/yyyy");
                        //label102.Text = reader.GetDateTime(2).ToString("dd/MMM/yyyy");

                        //PanelInfoKalender.Visible = true;
                        //PanelInfoKalender.Location = new Point(Cursor.Position.X,Cursor.Position.Y);
                        //PanelInfoKalender.BringToFront();
                        
                    }
                    koneksi.closeConnection();
                }
                else
                {
                    //hide
                    toolTip1.Hide(this);
                }
            }
            catch { }

            Thread.Sleep(100);
        }

        private void dataGridView3_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            
            // MessageBox.Show(e.ColumnIndex.ToString()+ " " + e.RowIndex.ToString());
            if (dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Red || dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Pink)
            {
               
                contextMenuStrip1.Show(Cursor.Position);
                //tes1
                //DateTime tanggalPesan1 = Convert.ToDateTime(TglBulan + "/" + dataGridView3.Columns[e.ColumnIndex].Name.ToString() + "/" + Tgltahun);
                DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[e.ColumnIndex].Name.ToString());

                SqlCommand sqlC = new SqlCommand("select checkin from Reservasi where kamar_no =@a and status='booking' and Reservasi.checkin <=@id and Reservasi.checkout > @id", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@a",Int32.Parse(dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString()));
                sqlC.Parameters.AddWithValue("@id", tanggalPesan1);
                SqlDataReader readC = sqlC.ExecuteReader();
                int inDexC = 0;
                while (readC.Read())
                {
                    if (readC["checkin"].ToString().Equals(DateTime.Now.Date.ToString()))
                    {
                        inDexC = 1;
                    }
                }
                //if (inDexC < 1){
                //    checkInToolStripMenuItem.Visible = false;
                //}
                //else
                //{ checkInToolStripMenuItem.Visible = true; }
                rowSelect = e.RowIndex;
                columnSelect = e.ColumnIndex;
                koneksi.closeConnection(); checkInBookingToolStripMenuItem.Visible = true;
                checkInToolStripMenuItem.Visible = true;
                //batalToolStripMenuItem1.Visible = true;
                ubahBookingToolStripMenuItem.Visible = true;
                bayarToolStripMenuItem3.Visible = true;
                tambahNoteToolStripMenuItem1.Visible = true;

                tToolStripMenuItem.Visible = true;

            }
            else if (dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Yellow)
            {
                contextMenuStrip1.Show(Cursor.Position);
                rowSelect = e.RowIndex;
                columnSelect = e.ColumnIndex;
                checkInBookingToolStripMenuItem.Visible = false;
                checkInToolStripMenuItem.Visible = false;
                //batalToolStripMenuItem1.Visible = false;
                ubahBookingToolStripMenuItem.Visible = false;
                bayarToolStripMenuItem3.Visible = true;
                tambahNoteToolStripMenuItem1.Visible = false;

                tToolStripMenuItem.Visible = true;
            }
            else if(dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Green)
            {
                contextMenuStrip1.Show(Cursor.Position);
                rowSelect = e.RowIndex;
                columnSelect = e.ColumnIndex;
                checkInBookingToolStripMenuItem.Visible = false;
                checkInToolStripMenuItem.Visible = false;
                //batalToolStripMenuItem1.Visible = false;
                ubahBookingToolStripMenuItem.Visible = false;
                bayarToolStripMenuItem3.Visible = false;
                tambahNoteToolStripMenuItem1.Visible = false;
                tToolStripMenuItem.Visible = true;
            }

            try
            {
                DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

                int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1
                SqlCommand sqlq = new SqlCommand("select max(Reservasi.booking_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok", koneksi.KoneksiDB());
                sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
                sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
                string bookingKamar = sqlq.ExecuteScalar().ToString();
                koneksi.closeConnection();

                sqlq = new SqlCommand("select statusbayar from Booking where booking_id=@a", koneksi.KoneksiDB());
                sqlq.Parameters.AddWithValue("@a", bookingKamar);
                string statusbayar = sqlq.ExecuteScalar().ToString();
                koneksi.closeConnection();

                if (statusbayar == "1")
                {
                    printInvoiceRoomToolStripMenuItem.Visible = false;
                }
                else
                {
                    printInvoiceRoomToolStripMenuItem.Visible = true;
                }
            }
            catch { }

        }

        private void btnKonfirmasiBooking_Click(object sender, EventArgs e)
        {
            if (comboboxPembayaranBooking.Text.Equals("") || inputSebutan.Text.Equals("") || inputPembayaran.Text.Equals("") || inputNamaTamu.Text.Equals("") || inputTelepon.Text.Equals(""))
            {
                MessageBox.Show("Pastikan Data Terisi");
            }
            else
            {
                SqlCommand sql;
                if (dataCustomer < 1)
                {
                    sql = new SqlCommand("insert into Tamu(tamu,alamat,kota,telepon,email,perusahaan,tanggallahir,sebutan,gelar,noidentitas,jenisidentitas,warganegara,snapshot) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,@j,@k,@l,@m)", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", inputNamaTamu.Text);
                    sql.Parameters.AddWithValue("@b", inputAlamat.Text);
                    sql.Parameters.AddWithValue("@c", inputKota.Text);
                    sql.Parameters.AddWithValue("@d", inputTelepon.Text);
                    sql.Parameters.AddWithValue("@e", inputEmail.Text);
                    sql.Parameters.AddWithValue("@f", input_perusahaan.Text);
                    sql.Parameters.AddWithValue("@g", inputUlangTahun.Value);
                    sql.Parameters.AddWithValue("@h", inputSebutan.Text);
                    sql.Parameters.AddWithValue("@i", inputGelar.Text);
                    sql.Parameters.AddWithValue("@j", txtNoIdentitas.Text);
                    sql.Parameters.AddWithValue("@k", cbJnsIdentitas.Text);
                    sql.Parameters.AddWithValue("@l", txtWargaNegara.Text);
                    sql.Parameters.AddWithValue("@m", filejpeg);
                                                

                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    sql = new SqlCommand("select max(tamu_id) from Tamu", koneksi.KoneksiDB());
                    dataCustomer = Int32.Parse(sql.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                    //btnKonfirmasiBooking.Text = "Booking Telah Dilakukan";
                    //btnKonfirmasiBooking.Enabled = false;
                }

                sql = new SqlCommand("update Tamu set tamu=@a,alamat=@b,kota=@c,telepon=@d,email=@e,perusahaan=@f,tanggallahir=@g,sebutan=@h,gelar=@i,noidentitas=@j, jenisidentitas=@k,warganegara=@l,snapshot=@m where tamu_id=@no", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", inputNamaTamu.Text);
                sql.Parameters.AddWithValue("@b", inputAlamat.Text);
                sql.Parameters.AddWithValue("@c", inputKota.Text);
                sql.Parameters.AddWithValue("@d", inputTelepon.Text);
                sql.Parameters.AddWithValue("@e", inputEmail.Text);
                sql.Parameters.AddWithValue("@f", input_perusahaan.Text);
                sql.Parameters.AddWithValue("@g", inputUlangTahun.Value);
                sql.Parameters.AddWithValue("@h", inputSebutan.Text);
                sql.Parameters.AddWithValue("@i", inputGelar.Text);
                sql.Parameters.AddWithValue("@j", txtNoIdentitas.Text);
                sql.Parameters.AddWithValue("@k", cbJnsIdentitas.Text);

                sql.Parameters.AddWithValue("@l", txtWargaNegara.Text);
                sql.Parameters.AddWithValue("@m", filejpeg);
                                                
                sql.Parameters.AddWithValue("@no", dataCustomer);

                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand("insert into Booking(tamu_id, tgl_booking, checkin, checkout, uang_muka, tag_kamar,tag_restoran,tag_transport,status,grand_total,payment,balance_due,note,booking_diskon_id,staff_id) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,@j,@k,@l,@m,@n,@o)", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", dataCustomer);
                sql.Parameters.AddWithValue("@b", DateTime.Now);
                sql.Parameters.AddWithValue("@c", DateTime.Now);
                sql.Parameters.AddWithValue("@d", DateTime.Now);
                sql.Parameters.AddWithValue("@e", 0);
                float diskon = 100;
                if (cb_diskon.Checked)
                {
                    sql.Parameters.AddWithValue("@n", 1);
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=1", koneksi.KoneksiDB());
                    diskon = float.Parse(s.ExecuteScalar().ToString());
                
                }
                else
                {   if(Int32.Parse(diskonAngka.Text)>0){
                        SqlCommand s = new SqlCommand("INSERT INTO Booking_diskon(booking_diskon, harga) VALUES('Custom Diskon', @hrg)", koneksi.KoneksiDB());
                        int diskonA = Int32.Parse(diskonAngka.Text);
                        float totalDiskon = (float)(diskonA * 100) / totalBiaya;
                        s.Parameters.AddWithValue("@hrg", ((float)(100-totalDiskon)/100));
                        s.ExecuteNonQuery();

                        s = new SqlCommand("select max(booking_diskon_id) from booking_diskon", koneksi.KoneksiDB());
                        int booking_diskon_id = Int32.Parse(s.ExecuteScalar().ToString());
                        sql.Parameters.AddWithValue("@n", booking_diskon_id);
                        
                        diskon = (float)(100-totalDiskon);

                    }else{
                        sql.Parameters.AddWithValue("@n", 2);
                        SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=2", koneksi.KoneksiDB());
                        diskon = float.Parse(s.ExecuteScalar().ToString());
                    }
                }

                sql.Parameters.AddWithValue("@f", totalBiaya);
                sql.Parameters.AddWithValue("@g", 0);
                sql.Parameters.AddWithValue("@h", 0);
                sql.Parameters.AddWithValue("@i", "NO");
                sql.Parameters.AddWithValue("@j", totalBiaya);
                sql.Parameters.AddWithValue("@k", 1);
                totalBiaya = Convert.ToInt32((totalBiaya * diskon) / 100);
                sql.Parameters.AddWithValue("@l", totalBiaya - Int32.Parse(inputPembayaran.Text));
                sql.Parameters.AddWithValue("@m", txtCatatanBooking.Text);
                //sql.Parameters.AddWithValue("@n", 1);
                sql.Parameters.AddWithValue("@o", 1);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();



                sql = new SqlCommand("select max(booking_id) from Booking", koneksi.KoneksiDB());
                int nilaimax = Int32.Parse(sql.ExecuteScalar().ToString());

                koneksi.closeConnection();
           
                //simpan reservasi
                List<DataRow> rd = new List<DataRow>();
                foreach (DataRow dr in dKamarPesan.Rows)
                {
                    sql = new SqlCommand("insert into Reservasi(booking_id, checkin, checkout, tamu_id, kamar_no, tag_kamar,tag_restoran,tag_transport,harga_id,status,downpayment,realcheckout) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,'booking',@j,@k)", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", nilaimax);
                    sql.Parameters.AddWithValue("@b", dr["Checkin"]);
                    sql.Parameters.AddWithValue("@c", dr["Checkout"]);
                    sql.Parameters.AddWithValue("@d", dataCustomer);
                    sql.Parameters.AddWithValue("@e", dr["NO Kamar"]);
                    sql.Parameters.AddWithValue("@f", dr["Harga"]);
                    sql.Parameters.AddWithValue("@g", 0);
                    sql.Parameters.AddWithValue("@h", 0);
                    sql.Parameters.AddWithValue("@i", 1);
                    if (comboBox4.Text.Equals("Down payment " + dr["NO Kamar"].ToString()))
                    {
                        /*if (comboBox4.Text.Equals("OTA - Tanpa Batas Waktu"))
                        {
                            sql.Parameters.AddWithValue("@j", 1);
                        }
                        else
                        {*/
                    //        sql.Parameters.AddWithValue("@j", Int32.Parse(inputPembayaran.Text));
                            sql.Parameters.AddWithValue("@j", 0);
                        //}
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@j", 0);
                    }
                    sql.Parameters.AddWithValue("@k", dr["checkout"]);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();
                }

                if (comboBox4.Text == "Lunas" || comboBox4.Text == "OTA - Tanpa Batas Waktu")
                {
                    rd = new List<DataRow>();
                    int ctr = 0;
                    foreach (DataRow dr in dKamarPesan.Rows)
                    {
                        ctr++;
                        sql = new SqlCommand("select max(reservasi_id) from Reservasi where kamar_no = @a and status = 'booking' ", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", dr["NO Kamar"].ToString());
                        int reservasiIDPayment = Int32.Parse(sql.ExecuteScalar().ToString());
                        koneksi.closeConnection();
                        if (ctr == 1)
                        {
                            sql = new SqlCommand("insert into pembayaran(booking_id, reservasi_id,payment,nopayment,jumlahpayment,tggalpayment,staff_id) values (@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@a", nilaimax);
                            sql.Parameters.AddWithValue("@b", reservasiIDPayment);
                            sql.Parameters.AddWithValue("@c", comboboxPembayaranBooking.Text);
                            sql.Parameters.AddWithValue("@d", inputCC1.Text);
                            sql.Parameters.AddWithValue("@e", 0);
                            sql.Parameters.AddWithValue("@f", DateTime.Now);
                            sql.Parameters.AddWithValue("@g", Login.idS.ToString());
                            sql.ExecuteNonQuery();
                            koneksi.closeConnection();
                        }
                        sql = new SqlCommand("insert into pembayaran(booking_id, reservasi_id,payment,nopayment,jumlahpayment,tggalpayment,staff_id) values (@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", nilaimax);
                        sql.Parameters.AddWithValue("@b", reservasiIDPayment);
                        sql.Parameters.AddWithValue("@c", comboboxPembayaranBooking.Text);
                        sql.Parameters.AddWithValue("@d", inputCC1.Text);
                        sql.Parameters.AddWithValue("@e", Convert.ToInt32((float.Parse(dr["Harga"].ToString()) * diskon) / 100));
                        sql.Parameters.AddWithValue("@f", DateTime.Now);

                        sql.Parameters.AddWithValue("@g", Login.idS.ToString());
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sql = new SqlCommand("update Reservasi set downpayment= downpayment+@a where reservasi_id =@b", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", Convert.ToInt32((float.Parse(dr["Harga"].ToString()) * diskon) / 100));
                        sql.Parameters.AddWithValue("@b", reservasiIDPayment);
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();

                    }
                }
                else
                {
                    sql = new SqlCommand("select max(reservasi_id) from Reservasi where kamar_no = @a and status = 'booking' ", koneksi.KoneksiDB());
                    if (comboBox4.Text.Equals("OTA - Tanpa Batas Waktu"))
                    {
                        sql.Parameters.AddWithValue("@a", Int32.Parse(comboBox4.Items[0].ToString().Replace("Down payment ", "")));
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@a", Int32.Parse(comboBox4.Text.Replace("Down payment ", "")));
                    }
                    int reservasiIDPayment = Int32.Parse(sql.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                    sql = new SqlCommand("insert into pembayaran(booking_id, reservasi_id,payment,nopayment,jumlahpayment,tggalpayment,staff_id) values (@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", nilaimax);
                    sql.Parameters.AddWithValue("@b", reservasiIDPayment);
                    sql.Parameters.AddWithValue("@c", comboboxPembayaranBooking.Text);
                    sql.Parameters.AddWithValue("@d", inputCC1.Text);
                    sql.Parameters.AddWithValue("@e", Int32.Parse(inputPembayaran.Text));
                    sql.Parameters.AddWithValue("@f", DateTime.Now);
                    sql.Parameters.AddWithValue("@g", Login.idS.ToString());
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    sql = new SqlCommand("update Reservasi set downpayment = @a where reservasi_id=@b", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", Int32.Parse(inputPembayaran.Text));
                    sql.Parameters.AddWithValue("@b", reservasiIDPayment);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();   
                }
                dKamarPesan.Clear();
                comboBox4.Items.Clear();
                dataCustomer = 0;

                //btnKonfirmasiBooking.Text = "Booking Telah Disimpan";
                //btnKonfirmasiBooking.Enabled = false;
                //panelKalender.BringToFront();
                btnKalender_Click(sender, e);

            }

        }

        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
        
        }

        private void comboBox3_SelectedValueChanged(object sender, EventArgs e)
        {
        
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView3.Enabled = true;
            if (cekPilih == true)
            {
                ComboboxItem selectedCar = (ComboboxItem)comboBox2.SelectedItem;
                loadKalender(Convert.ToInt32(selectedCar.Value), Int32.Parse(comboBox3.Text));
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView3.Enabled = true;
            if (cekPilih == true)
            {
                ComboboxItem selectedCar = (ComboboxItem)comboBox2.SelectedItem;
                loadKalender(Convert.ToInt32(selectedCar.Value), Int32.Parse(comboBox3.Text));
            }
        }

        private void comboBox3_TextChanged(object sender, EventArgs e)
        {
            if (cekPilih == true)
            {
                ComboboxItem selectedCar = (ComboboxItem)comboBox2.SelectedItem;
                loadKalender(Convert.ToInt32(selectedCar.Value), Int32.Parse(comboBox3.Text));
            }
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            if (cekPilih == true)
            {
                ComboboxItem selectedCar = (ComboboxItem)comboBox2.SelectedItem;
                loadKalender(Convert.ToInt32(selectedCar.Value), Int32.Parse(comboBox3.Text));
                
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void checkInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string nOKamar = dataGridView3.Rows[rowSelect].Cells[0].Value.ToString();
            //MessageBox.Show(nOKamar);
            //Boolean cekKiri=false;
            //int kiriGanti=columnSelect;

            //while(!cekKiri){
            //    if (dataGridView3.Rows[rowSelect].Cells[kiriGanti].Style.BackColor == Color.Red)
            //    {
            //        dataGridView3.Rows[rowSelect].Cells[kiriGanti].Style.BackColor = Color.Green;
            //        kiriGanti = kiriGanti - 1;
            //    }
            //    else
            //    {
            //        cekKiri = true;   
            //    }
            //}
            //cekKiri = false;
            //kiriGanti = columnSelect+1;
            //while (!cekKiri)
            //{
            //    if (dataGridView3.Rows[rowSelect].Cells[kiriGanti].Style.BackColor == Color.Red)
            //    {
            //        dataGridView3.Rows[rowSelect].Cells[kiriGanti].Style.BackColor = Color.Green;
            //        kiriGanti = kiriGanti + 1;
            //    }
            //    else
            //    {
            //        cekKiri = true;
            //    }
            //}
            DialogResult result = MessageBox.Show("Anda yakin untuk mencheckinkan reservasi kamar ini", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1

                DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

                int cekTgl = 0;
                SqlCommand sqlq = new SqlCommand("select Reservasi.booking_id, checkin, checkout from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking')", koneksi.KoneksiDB());
                sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
                sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
                SqlDataReader reader = sqlq.ExecuteReader();
                while (reader.Read())
                {
                    DateTime tgl_checkin = Convert.ToDateTime(reader.GetValue(1));
                    DateTime tgl_checkout = Convert.ToDateTime(reader.GetValue(2));
                    if (tgl_checkin.Date > DateTime.Now.Date || tgl_checkout.Date < DateTime.Now.Date)
                    {
                        cekTgl += 1;
                    }
                }
                koneksi.closeConnection();

                SqlCommand sqlCheckin = new SqlCommand("select reservasi_id from Reservasi where kamar_no = @no and status='checkin'", koneksi.KoneksiDB());
                sqlCheckin.Parameters.AddWithValue("@no", NoKamarInfo);
                SqlDataReader readCheckin = sqlCheckin.ExecuteReader();
                int CekData = 0;
                while (readCheckin.Read())
                {
                    CekData += 1;
                }
                koneksi.closeConnection();

                if (CekData == 0 && cekTgl == 0)
                {
                    panel4.Visible = true;
                    panel4.BringToFront();
                    DataTamuKalenderBaru.Enabled = false;

                    DataTamuKalender.Visible = true;
                    SqlDataAdapter da = new SqlDataAdapter("select tamu_id, tamu, alamat, kota, telepon from Tamu", koneksi.KoneksiDB());
                    DataTable dset = new DataTable();
                    da.Fill(dset);
                    dataGridView6.DataSource = dset;
                    koneksi.closeConnection();
                }
                else
                {
                    MessageBox.Show("Terdapat NoKamar yang Belum dichekout \n Atau Tanggal checkin Harus hari ini");
                }
            }
        }

        private void btnDaftarTamu_Click_1(object sender, EventArgs e)
        {
            panelPengaturanKamar.SendToBack();
        }
        DataTable dtPesan = new DataTable();
        
        private void hideBookingElement()
        {
            panelCheckinDate.Visible = false;
            panelCheckoutDate.Visible = false;
            //flowLayoutPanel1.Visible = true;
            panelKamarDibooking.Visible = false;
 
            
        }

        Button[] Kamar;
        int JumKamarHigh = 0;
        private void btnPesan_Click(object sender, EventArgs e)
        {
            resetBtnKonfigurasi();
            resetBtnLaporan();
            refreshActivatedButton();
            btnPesan.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnPesan.FlatAppearance.BorderSize = 2;
            
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            btnBookingStatus.Visible = true;
            btnCheckInStatus.Visible = true;
            btnKamarMaintenance.Visible = true;
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement(); 
            panelKamarDibooking.Controls.Clear();
            hidepanelPengaturanKamar();
            PanelPesan.BringToFront();
            PanelPesan.Controls.Clear();
            
}

        public void tooltipclosebookinghangus(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            toolTip1.Hide(btn);
        }

        public void tooltipshowbookinghangus(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            //SqlCommand cmd = new SqlCommand("Select b.booking_id from reservasi r inner join booking b on r.booking_id=b.booking_id where r.status='booking' and (datediff(minute,b.tgl_booking,SYSDATETIME())>180) group by b.booking_id having SUM(r.downpayment)<=0", koneksi.KoneksiDB());
            //reader = cmd.ExecuteReader();
            //Button[] Kamar;
            //while (reader.Read())
            //{

            SqlCommand sqlq = new SqlCommand("select t.tamu, r.checkin, r.checkout ,t.alamat,t.kota,t.telepon,t.email, b.note, b.tgl_booking " +
                "from Reservasi r,Booking b,Tamu t where r.booking_id=b.booking_id and t.tamu_id=b.tamu_id and r.status='booking' " +
                "and (datediff(minute,b.tgl_booking,SYSDATETIME())>180) and r.downpayment<=0 and r.kamar_no=@nok and b.booking_id=@b_id", koneksi.KoneksiDB());
            // sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
            sqlq.Parameters.AddWithValue("@nok", btn.Text);
            sqlq.Parameters.AddWithValue("@b_id", btn.Tag.ToString());
            reader = sqlq.ExecuteReader();

            while (reader.Read())
            {

                toolTip1.Show("----------------------------------------------------------------------------\r\n" +
                            "                        Dibooking oleh : " + reader.GetString(0) + "\r\n" +
                            "----------------------------------------------------------------------------\r\n" +
                            " Alamat     : " + reader["alamat"].ToString() + "\r\n" +
                            " Kota       : " + reader["kota"].ToString() + "\r\n" +
                            " No Telepon : " + reader["telepon"].ToString() + "\r\n" +
                            " Email      : " + reader["email"].ToString() + "\r\n" +
                                        //"Alamat " + reader.GetString(3) + " Kota " + reader.GetString(4) + " Email " + reader.GetString(6) + "\r\n" +
                                "-----------------------------------------------------------------------------\r\n" +
                            " Kamar      : " + btn.Text.ToString() + "\r\n" +
                            " Checkin    : " + reader.GetDateTime(1).ToString("dd/MMM/yyyy") + " | Checkout : " + reader.GetDateTime(2).ToString("dd/MMM/yyyy") + "\r\n" +
                            " Catatan    : " + reader["note"].ToString() + "\r\n" +
                            " Jam Booking : " + reader.GetDateTime(8).ToString("HH:mm:ss") + "  |  Jam Hangus : " + reader.GetDateTime(8).AddHours(3).ToString("HH:mm:ss") + "\r\n" +
                            "----------------------------------------------------------------------------\r\n"
                            ,
                btn);

            }
            koneksi.closeConnection();
        }

        public void tooltipclose(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            toolTip1.Hide(btn);
            for (int i = 0; i < JumKamarHigh; i++)
            {
                Kamar[i].FlatStyle = FlatStyle.Flat;
                Kamar[i].FlatAppearance.BorderColor = Color.Black;
                Kamar[i].FlatAppearance.BorderSize = 1;
            }
        }

        public void tooltipshow(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            SqlCommand sql = new SqlCommand("select reservasi_id,booking_id from Reservasi where kamar_no =@noKamar and status='checkin' ", koneksi.KoneksiDB());
            //SqlCommand sql = new SqlCommand("select reservasi_id,booking_id from Reservasi where kamar_no =@noKamar and convert(date,@tnggal)>=convert(date,checkin) and convert(date,@tnggal) <= convert(date,checkout) and status='checkin' ", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@noKamar", Int32.Parse(btn.Text));
            //sql.Parameters.AddWithValue("@tnggal", DateTime.Now);
            string booking_id = "-";
            SqlDataReader readZ = sql.ExecuteReader();
            while (readZ.Read())
            {
                booking_id = readZ["booking_id"].ToString();
            }

            koneksi.closeConnection();

            sql = new SqlCommand("select Tamu.tamu from Tamu, Booking where Tamu.tamu_id=Booking.tamu_id and Booking.booking_id = @a",koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", booking_id);
            string tamubook = sql.ExecuteScalar().ToString();
            koneksi.closeConnection();

            SqlCommand sqlq = new SqlCommand("select Tamu.tamu, Reservasi.checkin, Reservasi.checkout,Tamu.alamat,Tamu.kota,Tamu.telepon,Tamu.email,Reservasi.booking_id, Booking.note from Reservasi, Tamu, Booking where Booking.booking_id = Reservasi.booking_id and Tamu.tamu_id = Reservasi.tamu_id and Reservasi.kamar_no=@nok and ( Reservasi.status='checkin')", koneksi.KoneksiDB());
            // sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
            sqlq.Parameters.AddWithValue("@nok", btn.Text);
            reader = sqlq.ExecuteReader();

            while (reader.Read())
            {

                toolTip1.Show("----------------------------------------------------------------------------\r\n" +
        "                        Dibooking oleh : " + tamubook + "\r\n" +
        "                        Ditempati oleh : " + reader.GetString(0) + "\r\n" +
        "----------------------------------------------------------------------------\r\n" +
        " Alamat     : " + reader["alamat"].ToString() +"\r\n" +
        " Kota       : " + reader["kota"].ToString() + "\r\n" +
        " No Telepon : " + reader["telepon"].ToString() + "\r\n" +
        " Email      : " + reader["email"].ToString() + "\r\n" +
                    //"Alamat " + reader.GetString(3) + " Kota " + reader.GetString(4) + " Email " + reader.GetString(6) + "\r\n" +
          "-----------------------------------------------------------------------------\r\n" +
        " Kamar      : " + btn.Text.ToString() + "\r\n" +
        " Checkin    : " + reader.GetDateTime(1).ToString("dd/MMM/yyyy") + " | Checkout : " + reader.GetDateTime(2).ToString("dd/MMM/yyyy") + "\r\n" +
        " Catatan    : " + reader["note"].ToString() + "\r\n"+ 
        "----------------------------------------------------------------------------\r\n"
        ,
                btn);

            }
            koneksi.closeConnection();
            

            sql = new SqlCommand("select kamar_no from Reservasi where booking_id=@a and status='checkin'", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", booking_id);
            SqlDataReader readKamarno = sql.ExecuteReader();
            while (readKamarno.Read())
            {
                for (int i = 0; i < JumKamarHigh; i++)
                {
                    if (Kamar[i].Text.Equals(readKamarno["kamar_no"].ToString()))
                    {
                       
                        Kamar[i].FlatStyle = FlatStyle.Flat;
                        Kamar[i].FlatAppearance.BorderColor = Color.Yellow;
                        Kamar[i].FlatAppearance.BorderSize = 2;
                    }
                }
            }
            koneksi.closeConnection();
        
        }

        private void MunculKan(object sender, EventArgs e)
        {
            panelPembayaran.Visible = false;
            panelCatatanBook.SendToBack();

            Button btn = sender as Button;
            contextMenuStrip2.Show(Cursor.Position);
            dataKamarCh = Int32.Parse(btn.Text);
            SqlCommand sql = new SqlCommand("select reservasi_id,booking_id from Reservasi where kamar_no =@noKamar and status='checkin' ", koneksi.KoneksiDB());
            
            //SqlCommand sql = new SqlCommand("select reservasi_id,booking_id from Reservasi where kamar_no =@noKamar and convert(date,@tnggal)>=convert(date,checkin) and convert(date,@tnggal) <= convert(date,checkout) and status='checkin' ", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@noKamar", Int32.Parse(btn.Text));
            //sql.Parameters.AddWithValue("@tnggal", DateTime.Now);
            string idReservasi = "-";
            string booking_id = "-";
            SqlDataReader readZ = sql.ExecuteReader();

            while (readZ.Read())
            {
                try
                {
                    idReservasi = readZ["reservasi_id"].ToString();
                    booking_id = readZ["booking_id"].ToString();
                }
                catch
                {
                    idReservasi = "-";
                }
            }

            koneksi.closeConnection();
            sql = new SqlCommand("select statusbayar from Booking where booking_id=@a", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", booking_id);
            string statusA = sql.ExecuteScalar().ToString();
            koneksi.closeConnection();
            if (statusA.Equals(""))
            {
                checkOutKamarToolStripMenuItem.Visible = true;
                //pendingToolStripMenuItem.Visible = true;
                printInvoiceKamarToolStripMenuItem1.Visible = true;
            }
            else
            {
                //checkOutKamarToolStripMenuItem.Visible = false;
                //pendingToolStripMenuItem.Visible = false;
                printInvoiceKamarToolStripMenuItem1.Visible = false;

            }
            label22.Text = idReservasi;
            dtPesan = new DataTable();

            dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtPesan.Columns.Add("IdItem".ToString());
            dtPesan.Columns.Add("Pesanan".ToString());
            dtPesan.Columns.Add("Jumlah".ToString());
            dtPesan.Columns.Add("RESERVASI_ID".ToString());
            dtPesan.Columns.Add("Tanggal", typeof(DateTime));
            dtPesan.Columns.Add("Harga".ToString());
            dataGridView4.DataSource = dtPesan;

        }
        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

        }

        private void pesanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtPesan.Clear();
            label3.Text = "-";
            label15.Text = "-";
            xPenang.Text = "";

            SqlCommand sql = new SqlCommand("Select * from item_tipe", koneksi.KoneksiDB());
            cbKriteriaCari.Items.Clear();
            reader = sql.ExecuteReader();
            while (reader.Read())
            {
                ComboboxItem item = new ComboboxItem();
                item.Value = reader.GetValue(0).ToString();
                item.Text = reader.GetValue(1).ToString();
                cbKriteriaCari.Items.Add(item);
            }
            koneksi.closeConnection();
            //cbKriteriaCari.Text = "Makanan";
            
            txtHargaLaundry.Visible = false;
            panel2.Visible = false;
            panelPesanItem.BringToFront();
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            PanelPesan.BringToFront();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        //private void label21_Click(object sender, EventArgs e)
        //{
        //    panel2.BringToFront();
        //    SqlDataAdapter da = new SqlDataAdapter("select Item.item_id, Item.item, Item_Tipe.item_tipe, Item.harga from Item, Item_Tipe where Item.item_tipe_id = Item_Tipe.item_tipe_id", koneksi.KoneksiDB());
        //    DataTable ds = new DataTable();
        //    da.Fill(ds);
        //    dataGridView5.DataSource = ds;
        //    koneksi.closeConnection();
        //}

        //private void label26_Click(object sender, EventArgs e)
        //{

        //    SqlDataAdapter da = new SqlDataAdapter("select Item.item_id, Item.item, Item_Tipe.item_tipe, Item.harga from Item, Item_Tipe where Item.item_tipe_id = Item_Tipe.item_tipe_id and Item.item like @nama ", koneksi.KoneksiDB());
        //    da.SelectCommand.Parameters.Add(new SqlParameter("@nama", string.Format("%{0}%", TxtCust.Text)));
        //    DataTable ds = new DataTable();
        //    da.Fill(ds);
        //    dataGridView5.DataSource = ds;
        //    koneksi.closeConnection();
        //}

        //private void label25_Click(object sender, EventArgs e)
        //{
        //    panel1.BringToFront();
        //}

        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           xPenang.Text = dataGridView5.Rows[dataGridView5.CurrentRow.Index].Cells[0].Value.ToString();
           label15.Text = dataGridView5.Rows[dataGridView5.CurrentRow.Index].Cells[1].Value.ToString();
           label3.Text = dataGridView5.Rows[dataGridView5.CurrentRow.Index].Cells[3].Value.ToString();
           panelPesanItem.BringToFront();
        }

        private void label20_Click(object sender, EventArgs e)
        {
            DataRow dr = dtPesan.NewRow();
            dr["IdItem"] = xPenang.Text;
            dr["RESERVASI_ID"] = label22.Text;
            dr["Tanggal"] = DateTime.Now;
            dr["Harga"] = label3.Text;
            dtPesan.Rows.Add(dr);
            dataGridView1.DataSource = dtPesan;
        }

        private void lblClear_Click(object sender, EventArgs e)
        {
            dtPesan.Clear();
        }

        private void lblRemove_Click(object sender, EventArgs e)
        {
            List<DataRow> rd = new List<DataRow>();
            int index = 0;
            foreach (DataRow dr in dtPesan.Rows)
            {
                if (index == dataGridView4.CurrentRow.Index)
                {
                    rd.Add(dr);
                }

                index += 1;
            }

            foreach (var r in rd)
            {
                dtPesan.Rows.Remove(r);
            }
            dtPesan.AcceptChanges();
            dataGridView4.DataSource = dtPesan;

        }

        private void label19_Click(object sender, EventArgs e)
        {
            List<DataRow> rd = new List<DataRow>();
            foreach (DataRow dr in dtPesan.Rows)
            {

                SqlCommand sql = new SqlCommand("insert into Pemesanan(item_id, reservasi_id, tgl_pemesanan, harga) values (@a,@b,@c,@d)", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", dr["IdIem"]);
                sql.Parameters.AddWithValue("@b", dr["RESERVASI_ID"]);
                sql.Parameters.AddWithValue("@c", dr["Tanggal"]);
                sql.Parameters.AddWithValue("@d", dr["Harga"]);

                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand("select booking_id from Reservasi where reservasi_id = @id", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@id", dr["RESERVASI_ID"]);
                string databooking = (sql.ExecuteScalar().ToString());

                koneksi.closeConnection();

                sql = new SqlCommand("update Booking set grand_total=grand_total+@biaya, balance_due = balance_due+@biaya  where booking_id=@id", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@biaya", dr["Harga"]);
                sql.Parameters.AddWithValue("@id", Int32.Parse(databooking));

                sql.ExecuteNonQuery();
                koneksi.closeConnection();
                sql = new SqlCommand("update Reservasi set tag_restoran=tag_restoran+@biaya where booking_id=@id and reservasi_id=@idr", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@biaya", dr["Harga"]);
                sql.Parameters.AddWithValue("@idr", dr["RESERVASI_ID"]);
                sql.Parameters.AddWithValue("@id", Int32.Parse(databooking));

                sql.ExecuteNonQuery();
                koneksi.closeConnection();
            }
            dtPesan.Clear();
            label22.Text = "-";
            label3.Text = "-";
            label15.Text = "-";
            xPenang.Text = "";
            PanelPesan.BringToFront();
            
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            
        }

        private void btnPengaturanKamar_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            refreshActivatedButton();
            btnPengaturanKamar.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnPengaturanKamar.FlatAppearance.BorderSize = 2;
                        
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement(); 
            panelKamarDibooking.Controls.Clear();
            DataKamar dataKamar = new DataKamar(this);
            dataKamar.TopLevel = false;
            dataKamar.Name = "panelPengaturanKamarInnerForm";
            //panelPengaturanKamar.BringToFront();
            //splitContainer2.Panel1.Controls.Clear();
            splitContainer2.Panel1.Controls.Add(dataKamar);
            dataKamar.Show();
            dataKamar.Dock = DockStyle.Fill;
            dataKamar.BringToFront();
            //panelPengaturanKamarInner.Show();
            //panelPengaturanKamarInner.BringToFront();
            //refresh_pengaturankamar();
        }

        private void panelPengaturanKamarInner_Paint(object sender, PaintEventArgs e)
        {
            refresh_pengaturankamar();
        }

        /// <summary>
        //Suhendro Update
        /// </summary>
        /// 

        private void cmbJbtR_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSAllR_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveR_Click(object sender, EventArgs e)
        {
            string strTemp1 = ""; string strTemp2 = ""; string strTemp3 = ""; string strTemp4 = ""; string strTemp5 = "";
            string strTemp6 = ""; string strTemp7 = ""; string strTemp8 = ""; string strTemp9 = ""; string strTemp10 = "";
            string strTemp11 = ""; string strTemp12 = ""; string strTemp13 = ""; string strTemp14 = ""; string strTemp15 = "";
            string strTemp16 = ""; string checkNama = "";
            string strTemp17 = ""; string strTemp18 = ""; string strTemp19 = ""; string strTemp20 = "";
            // cmbJbtR.Items.Add(cmbJbtR.Text);
            //if (cmbJbtR.SelectedText == "")
            if (txtJbtR.Text != "")
            {
                string strQ1 = "select jabatan from Jabatan where jabatan = @nm";
                cmd1 = new SqlCommand(strQ1, koneksi.KoneksiDB());
                // cmd1.Parameters.AddWithValue("@nm", cmbJbtR.SelectedText.ToString());
                cmd1.Parameters.AddWithValue("@nm", txtJbtR.Text);
                reader = cmd1.ExecuteReader();
                while (reader.Read())
                {
                    checkNama = reader.GetString(0);
                }
                koneksi.closeConnection();
                if (checkNama == "")
                {
                    if (cKamarTersedia.Checked == true) strTemp1 = "On"; else strTemp1 = "Off";
                    if (cKalenderBooking.Checked == true) strTemp2 = "On"; else strTemp2 = "Off";
                    if (cStatusKamar.Checked == true) strTemp3 = "On"; else strTemp3 = "Off";
                    if (cSelesaiBersih.Checked == true) strTemp4 = "On"; else strTemp4 = "Off";
                    if (cDaftarTamu.Checked == true) strTemp5 = "On"; else strTemp5 = "Off";
                    if (cStaff.Checked == true) strTemp6 = "On"; else strTemp6 = "Off";
                    if (cHakAkses.Checked == true) strTemp7 = "On"; else strTemp7 = "Off";
                    if (cRestoran.Checked == true) strTemp8 = "On"; else strTemp8 = "Off";
                    if (cAturKamar.Checked == true) strTemp9 = "On"; else strTemp9 = "Off";
                    if (cInvoiceHistoris.Checked == true) strTemp10 = "On"; else strTemp10 = "Off";
                    if (cAturInfoHotel.Checked == true) strTemp11 = "On"; else strTemp11 = "Off";
                    if (cAturHargaKhusus.Checked == true) strTemp12 = "On"; else strTemp12 = "Off";
                    if (cAturHargaPeriodik.Checked == true) strTemp13 = "On"; else strTemp13 = "Off";
                    if (cLapKeuangan.Checked == true) strTemp14 = "On"; else strTemp14 = "Off";
                    if (cBookingHangus.Checked == true) strTemp15 = "On"; else strTemp15 = "Off";
                    if (cAturItem.Checked == true) strTemp16 = "On"; else strTemp16 = "Off";
                    if (cUtang.Checked == true) strTemp17 = "On"; else strTemp17 = "Off";
                    if (cRekap.Checked == true) strTemp18 = "On"; else strTemp18 = "Off";
                    if (cLaporanRestoran.Checked == true) strTemp19 = "On"; else strTemp19 = "Off";
                    if (cBatalR.Checked == true) strTemp20 = "On"; else strTemp20 = "Off";

                    // string strJabatan = cmbJbtR.Text;
                    string strJabatan = txtJbtR.Text;
                    //connecting();
                    //conn.Open();
                    string strQueryIns = "insert into jabatan(Jabatan,Rights_Kamar_Yang_Tersedia,Rights_Kalender_Booking,Rights_Status_Kamar,Rights_Kamar_Selesai_Dibersihkan," +
                        "Rights_Daftar_Tamu,Rights_Staff,Rights_Hak_Akses,Rights_Restoran,Rights_Pengaturan_Kamar,Rights_Invoice_Historis," +
                        "Rights_Pengaturan_Info_Hotel,Rights_Pengaturan_Harga_Khusus,Rights_Pengaturan_Harga_Periodik,Rights_Laporan_Keuangan," +
                        "Rights_Booking_Hangus,Rights_Pengaturan_Item, Rights_utang, Rights_RekapHariIni, Rights_Laporan_Restoran, Rights_Batal) " +
                        "values (@jab,@kmrtersedia,@kalender,@status,@selesai,@daftamu,@staff,@hak,@resto,@aturkamar,@inv,@aturhotel,@aturkhusus,@aturper,@lap,@bkangus,@aturitem,@utang,@rekap,@LaporanRestoran,@batal)";
                    SqlCommand cmd = new SqlCommand(strQueryIns, koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@jab", strJabatan);
                    cmd.Parameters.AddWithValue("@kmrtersedia", strTemp1);
                    cmd.Parameters.AddWithValue("@kalender", strTemp2);
                    cmd.Parameters.AddWithValue("@status", strTemp3);
                    cmd.Parameters.AddWithValue("@selesai", strTemp4);
                    cmd.Parameters.AddWithValue("@daftamu", strTemp5);
                    cmd.Parameters.AddWithValue("@staff", strTemp6);
                    cmd.Parameters.AddWithValue("@hak", strTemp7);
                    cmd.Parameters.AddWithValue("@resto", strTemp8);
                    cmd.Parameters.AddWithValue("@aturkamar", strTemp9);
                    cmd.Parameters.AddWithValue("@inv", strTemp10);
                    cmd.Parameters.AddWithValue("@aturhotel", strTemp11);
                    cmd.Parameters.AddWithValue("@aturkhusus", strTemp12);
                    cmd.Parameters.AddWithValue("@aturper", strTemp13);
                    cmd.Parameters.AddWithValue("@lap", strTemp14);
                    cmd.Parameters.AddWithValue("@bkangus", strTemp15);
                    cmd.Parameters.AddWithValue("@aturitem", strTemp16);
                    cmd.Parameters.AddWithValue("@utang", strTemp17);
                    cmd.Parameters.AddWithValue("@rekap", strTemp18);
                    cmd.Parameters.AddWithValue("@LaporanRestoran", strTemp19);
                    cmd.Parameters.AddWithValue("@batal", strTemp20);

                    cmd.ExecuteNonQuery();
                    koneksi.closeConnection();
                    //conn.Close();
                    MessageBox.Show("Hak Akses Telah Disimpan");
                    string strQ = "select * from jabatan";
                    createTblNoParam(strQ);
                }
                else
                {
                    MessageBox.Show("Hak Akses Tidak boleh Double");

                }
            }
        }

        private void btnUpdR_Click(object sender, EventArgs e)
        {
            
        }

        private void dgR_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panelKamarDibooking_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSaveR_Click_1(object sender, EventArgs e)
        {
            ////cmbJbtR.Items.Add(cmbJbtR.Text);
            //string strTemp1 = "";
            //string strTemp2 = "";
            //string strTemp3 = "";
            //string strTemp4 = "";
            //string strTemp5 = "";
            //string strTemp6 = "";
            //string strTemp7 = "";
            //string strTemp8 = "";
            //string strTemp9 = "";

            //if (cmbJbtR.SelectedText == "")
            //{
            //    if (cKamarR.Checked == true)
            //    {
            //        strTemp1 = "On";
            //    }
            //    else
            //    {
            //        strTemp1 = "Off";
            //    }

            //    if (cBookingR.Checked == true)
            //    {
            //        strTemp2 = "On";
            //    }
            //    else
            //    {
            //        strTemp2 = "Off";
            //    }

            //    if (cTamuR.Checked == true)
            //    {
            //        strTemp3 = "On";
            //    }
            //    else
            //    {
            //        strTemp3 = "Off";
            //    }

            //    if (cKalenderR.Checked == true)
            //    {
            //        strTemp4 = "On";
            //    }
            //    else
            //    {
            //        strTemp4 = "Off";
            //    }

            //    if (cDafBookingR.Checked == true)
            //    {
            //        strTemp5 = "On";
            //    }
            //    else
            //    {
            //        strTemp5 = "Off";
            //    }

            //    if (cUpdBookR.Checked == true)
            //    {
            //        strTemp6 = "On";
            //    }
            //    else
            //    {
            //        strTemp6 = "Off";
            //    }

            //    if (cPrintR.Checked == true)
            //    {
            //        strTemp7 = "On";
            //    }
            //    else
            //    {
            //        strTemp7 = "Off";
            //    }

            //    if (cUserR.Checked == true)
            //    {
            //        strTemp8 = "On";
            //    }
            //    else
            //    {
            //        strTemp8 = "Off";
            //    }

            //    if (cRights.Checked == true)
            //    {
            //        strTemp9 = "On";
            //    }
            //    else
            //    {
            //        strTemp9 = "Off";
            //    }
            //    string strJabatan = cmbJbtR.Text;
            //    //connecting();
            //    //conn.Open();
            //    string strQueryIns = "insert into jabatan(Jabatan,Rights_Check_Kamar,Rights_Booking_Kamar,Rights_Data_Tamu," +
            //        "Rights_Kalender,Rights_Daftar_Booking,Rights_Update_Booking,Rights_Print_Invoice,Rights_Daftar_User," +
            //        "Rights_Rights) values (@jab,@rcheck,@rbook,@rtamu,@rkal,@rdaf,@rupd,@rprint,@ruser,@rrig)";
            //    SqlCommand cmd = new SqlCommand(strQueryIns, koneksi.KoneksiDB());
            //    cmd.Parameters.AddWithValue("@jab", strJabatan);
            //    cmd.Parameters.AddWithValue("@rcheck", strTemp1);
            //    cmd.Parameters.AddWithValue("@rbook", strTemp2);
            //    cmd.Parameters.AddWithValue("@rtamu", strTemp3);
            //    cmd.Parameters.AddWithValue("@rkal", strTemp4);
            //    cmd.Parameters.AddWithValue("@rdaf", strTemp5);
            //    cmd.Parameters.AddWithValue("@rupd", strTemp6);
            //    cmd.Parameters.AddWithValue("@rprint", strTemp7);
            //    cmd.Parameters.AddWithValue("@ruser", strTemp8);
            //    cmd.Parameters.AddWithValue("@rrig", strTemp9);
            //    cmd.ExecuteNonQuery();
            //    koneksi.closeConnection();
            //    //conn.Close();
            //    MessageBox.Show("Data Rights Telah Disimpan");
            //    string strQ = "select * from jabatan";
            //    createTblNoParam(strQ);
            //}
        }

        private void btnUpdR_Click_1(object sender, EventArgs e)
        {
            string strTemp1 = ""; string strTemp2 = ""; string strTemp3 = ""; string strTemp4 = ""; string strTemp5 = "";
            string strTemp6 = ""; string strTemp7 = ""; string strTemp8 = ""; string strTemp9 = ""; string strTemp10 = "";
            string strTemp11 = ""; string strTemp12 = ""; string strTemp13 = ""; string strTemp14 = ""; string strTemp15 = "";
            string strTemp16 = ""; string checkNama = "";
            string strTemp17 = ""; string strTemp18 = ""; string strTemp19 = ""; string strTemp20 = "";

            //string strJabatan = cmbJbtR.Text;
            string strJabatan = txtJbtR.Text;

            string strQ1 = "select jabatan from Jabatan where jabatan = @nm";
            cmd = new SqlCommand(strQ1, koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@nm", cmbJbtR.SelectedText.ToString());
            cmd.Parameters.AddWithValue("@nm", txtJbtR.Text);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                checkNama = reader.GetString(0);
            }
            koneksi.closeConnection();
            if (checkNama == "")
            {
                MessageBox.Show("Masukkan Jabatan !");
            }
            else
            {
                if (cKamarTersedia.Checked == true) strTemp1 = "On"; else strTemp1 = "Off";
                if (cKalenderBooking.Checked == true) strTemp2 = "On"; else strTemp2 = "Off";
                if (cStatusKamar.Checked == true) strTemp3 = "On"; else strTemp3 = "Off";
                if (cSelesaiBersih.Checked == true) strTemp4 = "On"; else strTemp4 = "Off";
                if (cDaftarTamu.Checked == true) strTemp5 = "On"; else strTemp5 = "Off";
                if (cStaff.Checked == true) strTemp6 = "On"; else strTemp6 = "Off";
                if (cHakAkses.Checked == true) strTemp7 = "On"; else strTemp7 = "Off";
                if (cRestoran.Checked == true) strTemp8 = "On"; else strTemp8 = "Off";
                if (cAturKamar.Checked == true) strTemp9 = "On"; else strTemp9 = "Off";
                if (cInvoiceHistoris.Checked == true) strTemp10 = "On"; else strTemp10 = "Off";
                if (cAturInfoHotel.Checked == true) strTemp11 = "On"; else strTemp11 = "Off";
                if (cAturHargaKhusus.Checked == true) strTemp12 = "On"; else strTemp12 = "Off";
                if (cAturHargaPeriodik.Checked == true) strTemp13 = "On"; else strTemp13 = "Off";
                if (cLapKeuangan.Checked == true) strTemp14 = "On"; else strTemp14 = "Off";
                if (cBookingHangus.Checked == true) strTemp15 = "On"; else strTemp15 = "Off";
                if (cAturItem.Checked == true) strTemp16 = "On"; else strTemp16 = "Off";
                if (cUtang.Checked == true) strTemp17 = "On"; else strTemp17 = "Off";
                if (cRekap.Checked == true) strTemp18 = "On"; else strTemp18 = "Off";
                if (cLaporanRestoran.Checked == true) strTemp19 = "On"; else strTemp19 = "Off";
                if (cBatalR.Checked == true) strTemp20 = "On"; else strTemp20 = "Off";

                //connecting();
                //conn.Open();
                string strQueryUpd = "update jabatan set Rights_Kamar_Yang_Tersedia = @kmrtersedia,Rights_Kalender_Booking = @kalender,Rights_Status_Kamar = @status," +
                "Rights_Kamar_Selesai_Dibersihkan = @selesai,Rights_Daftar_Tamu = @daftamu,Rights_Staff = @staff,Rights_Hak_Akses = @hak," +
                "Rights_Restoran = @resto,Rights_Pengaturan_Kamar = @aturkamar,Rights_Invoice_Historis = @inv,Rights_Pengaturan_Info_Hotel = @aturhotel," +
                "Rights_Pengaturan_Harga_Khusus = @aturkhusus,Rights_Pengaturan_Harga_Periodik = @aturper,Rights_Laporan_Keuangan = @lap," +
                "Rights_Booking_Hangus = @bkangus,Rights_Pengaturan_Item = @aturitem, Rights_utang=@utang, Rights_RekapHariIni=@rekap, Rights_Laporan_Restoran=@LaporanRestoran, Rights_Batal=@batal where jabatan = @jab";
                SqlCommand cmd1 = new SqlCommand(strQueryUpd, koneksi.KoneksiDB());
                cmd1.Parameters.AddWithValue("@jab", strJabatan);
                cmd1.Parameters.AddWithValue("@kmrtersedia", strTemp1);
                cmd1.Parameters.AddWithValue("@kalender", strTemp2);
                cmd1.Parameters.AddWithValue("@status", strTemp3);
                cmd1.Parameters.AddWithValue("@selesai", strTemp4);
                cmd1.Parameters.AddWithValue("@daftamu", strTemp5);
                cmd1.Parameters.AddWithValue("@staff", strTemp6);
                cmd1.Parameters.AddWithValue("@hak", strTemp7);
                cmd1.Parameters.AddWithValue("@resto", strTemp8);
                cmd1.Parameters.AddWithValue("@aturkamar", strTemp9);
                cmd1.Parameters.AddWithValue("@inv", strTemp10);
                cmd1.Parameters.AddWithValue("@aturhotel", strTemp11);
                cmd1.Parameters.AddWithValue("@aturkhusus", strTemp12);
                cmd1.Parameters.AddWithValue("@aturper", strTemp13);
                cmd1.Parameters.AddWithValue("@lap", strTemp14);
                cmd1.Parameters.AddWithValue("@bkangus", strTemp15);
                cmd1.Parameters.AddWithValue("@aturitem", strTemp16);
                cmd1.Parameters.AddWithValue("@utang", strTemp17);
                cmd1.Parameters.AddWithValue("@rekap", strTemp18);
                cmd1.Parameters.AddWithValue("@LaporanRestoran", strTemp19);
                cmd1.Parameters.AddWithValue("@batal", strTemp20);

                cmd1.ExecuteNonQuery();
                koneksi.closeConnection();
                MessageBox.Show("Hak Akses Telah Diupdate");
                string strQ = "select * from jabatan";
                createTblNoParam(strQ);
            }
            //string strTemp1 = "";
            //string strTemp2 = "";
            //string strTemp3 = "";
            //string strTemp4 = "";
            //string strTemp5 = "";
            //string strTemp6 = "";
            //string strTemp7 = "";
            //string strTemp8 = "";
            //string strTemp9 = "";

            //if (cKamarR.Checked == true)
            //{
            //    strTemp1 = "On";
            //}
            //else
            //{
            //    strTemp1 = "Off";
            //}

            //if (cBookingR.Checked == true)
            //{
            //    strTemp2 = "On";
            //}
            //else
            //{
            //    strTemp2 = "Off";
            //}

            //if (cTamuR.Checked == true)
            //{
            //    strTemp3 = "On";
            //}
            //else
            //{
            //    strTemp3 = "Off";
            //}

            //if (cKalenderR.Checked == true)
            //{
            //    strTemp4 = "On";
            //}
            //else
            //{
            //    strTemp4 = "Off";
            //}

            //if (cDafBookingR.Checked == true)
            //{
            //    strTemp5 = "On";
            //}
            //else
            //{
            //    strTemp5 = "Off";
            //}

            //if (cUpdBookR.Checked == true)
            //{
            //    strTemp6 = "On";
            //}
            //else
            //{
            //    strTemp6 = "Off";
            //}

            //if (cPrintR.Checked == true)
            //{
            //    strTemp7 = "On";
            //}
            //else
            //{
            //    strTemp7 = "Off";
            //}

            //if (cUserR.Checked == true)
            //{
            //    strTemp8 = "On";
            //}
            //else
            //{
            //    strTemp8 = "Off";
            //}

            //if (cRights.Checked == true)
            //{
            //    strTemp9 = "On";
            //}
            //else
            //{
            //    strTemp9 = "Off";
            //}

            //string strJabatan = cmbJbtR.Text;
            ////connecting();
            ////conn.Open();
            //string strQueryUpd = "update jabatan set Rights_Check_Kamar = @rcheck,Rights_Booking_Kamar = @rbook," +
            //    "Rights_Data_Tamu = @rtamu,Rights_Kalender = @rkal,Rights_Daftar_Booking = @rdaf,Rights_Update_Booking = @rupd," +
            //    "Rights_Print_Invoice = @rprint,Rights_Daftar_User = @ruser,Rights_Rights = @rrig where jabatan = @jab";
            //SqlCommand cmd = new SqlCommand(strQueryUpd, koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@jab", strJabatan);
            //cmd.Parameters.AddWithValue("@rcheck", strTemp1);
            //cmd.Parameters.AddWithValue("@rbook", strTemp2);
            //cmd.Parameters.AddWithValue("@rtamu", strTemp3);
            //cmd.Parameters.AddWithValue("@rkal", strTemp4);
            //cmd.Parameters.AddWithValue("@rdaf", strTemp5);
            //cmd.Parameters.AddWithValue("@rupd", strTemp6);
            //cmd.Parameters.AddWithValue("@rprint", strTemp7);
            //cmd.Parameters.AddWithValue("@ruser", strTemp8);
            //cmd.Parameters.AddWithValue("@rrig", strTemp9);
            //cmd.ExecuteNonQuery();
            //koneksi.closeConnection();
            //MessageBox.Show("Data Rights Telah Diupdate");
            //string strQ = "select * from jabatan";
            //createTblNoParam(strQ);
        }

        private void btnDelR_Click_1(object sender, EventArgs e)
        {
            //string strJabatan = cmbJbtR.Text;
            ////connecting();
            ////conn.Open();
            //string strQueryDel = "delete from jabatan where jabatan = @jab";
            //SqlCommand cmd = new SqlCommand(strQueryDel, koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@jab", strJabatan);
            //cmd.ExecuteNonQuery();
            //koneksi.closeConnection();
            //MessageBox.Show("Data Rights Telah Dihapus");
            //string strQ = "select * from jabatan";
            //createTblNoParam(strQ);
        }

        private void dgR_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            //// boxId = dgR.Rows[dgR.CurrentRow.Index].Cells[0].Value.GetType(Int32());
            //boxJbt = dgR.Rows[dgR.CurrentRow.Index].Cells[1].Value.ToString();
            //boxCheck = dgR.Rows[dgR.CurrentRow.Index].Cells[2].Value.ToString();
            //boxBook = dgR.Rows[dgR.CurrentRow.Index].Cells[3].Value.ToString();
            //boxTamu = dgR.Rows[dgR.CurrentRow.Index].Cells[4].Value.ToString();
            //boxKalender = dgR.Rows[dgR.CurrentRow.Index].Cells[5].Value.ToString();
            //boxDafBook = dgR.Rows[dgR.CurrentRow.Index].Cells[6].Value.ToString();
            //boxUpdBook = dgR.Rows[dgR.CurrentRow.Index].Cells[7].Value.ToString();
            //boxPrint = dgR.Rows[dgR.CurrentRow.Index].Cells[8].Value.ToString();
            //boxUser = dgR.Rows[dgR.CurrentRow.Index].Cells[9].Value.ToString();
            //boxRights = dgR.Rows[dgR.CurrentRow.Index].Cells[10].Value.ToString();

            //cmbJbtR.Refresh();
            //cmbJbtR.Text = "";
            //cmbJbtR.SelectedText = boxJbt;
            //if (boxCheck == "On")
            //{
            //    cKamarR.Checked = true;
            //}
            //else
            //{
            //    cKamarR.Checked = false;
            //}

            //if (boxBook == "On")
            //{
            //    cBookingR.Checked = true;
            //}
            //else
            //{
            //    cBookingR.Checked = false;
            //}

            //if (boxTamu == "On")
            //{
            //    cTamuR.Checked = true;
            //}
            //else
            //{
            //    cTamuR.Checked = false;
            //}

            //if (boxKalender == "On")
            //{
            //    cKalenderR.Checked = true;
            //}
            //else
            //{
            //    cKalenderR.Checked = false;
            //}

            //if (boxDafBook == "On")
            //{
            //    cDafBookingR.Checked = true;
            //}
            //else
            //{
            //    cDafBookingR.Checked = false;
            //}

            //if (boxUpdBook == "On")
            //{
            //    cUpdBookR.Checked = true;
            //}
            //else
            //{
            //    cUpdBookR.Checked = false;
            //}

            //if (boxPrint == "On")
            //{
            //    cPrintR.Checked = true;
            //}
            //else
            //{
            //    cPrintR.Checked = false;
            //}

            //if (boxUser == "On")
            //{
            //    cUserR.Checked = true;
            //}
            //else
            //{
            //    cUserR.Checked = false;
            //}

            //if (boxRights == "On")
            //{
            //    cRights.Checked = true;
            //}
            //else
            //{
            //    cRights.Checked = false;
            //}
        }

        private void cmbJbtR_SelectedIndexChanged_1(object sender, EventArgs e)
        {


            //string tCheck = "";
            //string tBook = "";
            //string tTamu = "";
            //string tKalender = "";
            //string tDafBook = "";
            //string tUpdBook = "";
            //string tPrint = "";
            //string tUser = "";
            //string tRights = "";


            ////connecting();
            ////conn.Open();
            //string strQ = "select * from Jabatan where jabatan = @nm";
            //cmd = new SqlCommand(strQ, koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@nm", cmbJbtR.SelectedItem.ToString());
            //reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    tCheck = reader.GetString(2);
            //    tBook = reader.GetString(3);
            //    tTamu = reader.GetString(4);
            //    tKalender = reader.GetString(5);
            //    tDafBook = reader.GetString(6);
            //    tUpdBook = reader.GetString(7);
            //    tPrint = reader.GetString(8);
            //    tUser = reader.GetString(9);
            //    tRights = reader.GetString(10);
            //    //a++;
            //}
            //koneksi.closeConnection();

            //if (tCheck.Equals("On"))
            //{
            //    cKamarR.Checked = true;
            //}
            //else
            //{
            //    cKamarR.Checked = false;
            //}

            //if (tBook.Equals("On"))
            //{
            //    cBookingR.Checked = true;
            //}
            //else
            //{
            //    cBookingR.Checked = false;
            //}

            //if (tTamu.Equals("On"))
            //{
            //    cTamuR.Checked = true;
            //}
            //else
            //{
            //    cTamuR.Checked = false;
            //}

            //if (tKalender.Equals("On"))
            //{
            //    cKalenderR.Checked = true;
            //}
            //else
            //{
            //    cKalenderR.Checked = false;
            //}

            //if (tDafBook.Equals("On"))
            //{
            //    cDafBookingR.Checked = true;
            //}
            //else
            //{
            //    cDafBookingR.Checked = false;
            //}

            //if (tUpdBook.Equals("On"))
            //{
            //    cUpdBookR.Checked = true;
            //}
            //else
            //{
            //    cUpdBookR.Checked = false;
            //}

            //if (tPrint.Equals("On"))
            //{
            //    cPrintR.Checked = true;
            //}
            //else
            //{
            //    cPrintR.Checked = false;
            //}

            //if (tUser.Equals("On"))
            //{
            //    cUserR.Checked = true;
            //}
            //else
            //{
            //    cUserR.Checked = false;
            //}

            //if (tRights.Equals("On"))
            //{
            //    cRights.Checked = true;
            //}
            //else
            //{
            //    cRights.Checked = false;
            //}

            //createTbl1Param(strQ, cmbJbtR.SelectedItem.ToString());
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void panelRights1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            btnUser.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnUser.FlatAppearance.BorderSize = 2;
                        
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            panelUser.BringToFront();
            string strQUsr = "select nama,password,username,telp,email,Jabatan " +
                "from staff a,Jabatan b where a.id_jabatan=b.id_jabatan";
            createTblNoParamUsr(strQUsr);
            cmbJbtUsr.Refresh();
            cmbJbtUsr.Items.Clear();
            //cmbJbtR.Refresh();
            cmd = new SqlCommand("select id_jabatan,jabatan from jabatan", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                boxId = reader.GetInt32(0);
                //cmbJbtR.Items.Add(reader.GetString(1));
                cmbJbtUsr.Items.Add(reader.GetString(1));
                //a++;
            }
            koneksi.closeConnection();
            //panelCheckinDate.Visible = false;
            //panelCheckoutDate.Visible = false;

        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void btnSaveUsr_Click(object sender, EventArgs e)
        {
            //connecting();
            //conn.Open();

            // (cmbNmUsr.Text != "")
            if (txtNmUsr.Text != "")
            {
                // if (boxId == 0)
                // {
                //txtNmUsr.Items.Add(cmbNmUsr.Text);
                string strNama = txtNmUsr.Text;
                string strQueryIns = "insert into staff(nama,password,username,id_jabatan," +
                    "telp,email) values (@nm,@pass,@usrnm,@idjab,@telp,@email)";
                SqlCommand cmd = new SqlCommand(strQueryIns, koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@nm", strNama);
                cmd.Parameters.AddWithValue("@pass", txtPassUsr.Text);
                cmd.Parameters.AddWithValue("@usrnm", txtUserNmUsr.Text);
                cmd.Parameters.AddWithValue("@idjab", boxId);
                cmd.Parameters.AddWithValue("@telp", txtTelpUsr.Text);
                cmd.Parameters.AddWithValue("@email", txtEmailUsr.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();
                MessageBox.Show("Data User Telah Disimpan");
                string strQ = "select nama,password,username,telp,email,Jabatan " +
                   "from staff a,Jabatan b where a.id_jabatan=b.id_jabatan";
                createTblNoParamUsr(strQ);
                clearJabatan();
                // }
                // else
                // {
                //    MessageBox.Show("Masukkan Jabatan !");
                // }
            }
            else
            {
                MessageBox.Show("Masukkan Nama !");
            }
            resetInputDataUser();

        }

        private void resetInputDataUser()
        { 
            txtNmUsr.Text = "";
            txtUserNmUsr.Text = "";
            txtPassUsr.Text = "";
            txtTelpUsr.Text = "";
            txtEmailUsr.Text = "";
        
        }

        private void cmbJbtUsr_SelectedIndexChanged(object sender, EventArgs e)
        {
            //connecting();
            //conn.Open();
            //lblHideJabUsr.Refresh();
            //lblHideJabUsr.Text = "";
            cmbJbtUsr.Refresh();
            string strQ = "select id_jabatan from Jabatan where jabatan = @nm";
            cmd = new SqlCommand(strQ, koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@nm", cmbJbtUsr.SelectedItem.ToString());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                boxId = reader.GetInt32(0);
            }
            koneksi.closeConnection();
            //clearJabatan();

        }

        private void cmbNmUsr_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnUpdUsr_Click(object sender, EventArgs e)
        {
            if (txtNmUsr.Text != "")
            {
                //string strUserNm = cmbNmUsr.Text;
                string strUserNm = txtNmUsr.Text;
                //connecting();
                //koneksi.KoneksiDB().Open();
                string strQueryUpd = "update staff set password = @pass,username = @usrname," +
                    "id_jabatan = @idjabat,telp = @telp,email = @email where nama = @nama";
                SqlCommand cmd = new SqlCommand(strQueryUpd, koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@pass", txtPassUsr.Text);
                cmd.Parameters.AddWithValue("@usrname", txtUserNmUsr.Text);
                cmd.Parameters.AddWithValue("@idjabat", boxId);
                cmd.Parameters.AddWithValue("@telp", txtTelpUsr.Text);
                cmd.Parameters.AddWithValue("@email", txtEmailUsr.Text);
                cmd.Parameters.AddWithValue("@nama", strUserNm);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();
                MessageBox.Show("Data User Telah Diupdate");
                string strQ = "select nama,password,username,telp,email,Jabatan " +
                    "from staff a,Jabatan b where a.id_jabatan=b.id_jabatan";
                createTblNoParamUsr(strQ);
            }
            else
            {
                MessageBox.Show("Masukkan Nama !");
            }

            resetInputDataUser();
        }

        private void btnDelUsr_Click(object sender, EventArgs e)
        {
            if (txtNmUsr.Text != "")
            {
                //string strNmUsr = cmbNmUsr.Text;
                string strNmUsr = txtNmUsr.Text;
                //connecting();
                //conn.Open();
                string strQueryDel = "delete from staff where nama = @nm";
                SqlCommand cmd = new SqlCommand(strQueryDel, koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@nm", strNmUsr);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();
                MessageBox.Show("Data User Telah Dihapus");
                string strQ = "select nama,password,username,telp,email,Jabatan " +
                "from staff a,Jabatan b where a.id_jabatan=b.id_jabatan";
                createTblNoParamUsr(strQ);
            }
            else
            {
                MessageBox.Show("Masukkan Nama !");
            }

            resetInputDataUser();

        }

        private void btnSAllUsr_Click(object sender, EventArgs e)
        {
            string strQUsr = "select nama,password,username,telp,email,Jabatan " +
               "from staff a,Jabatan b where a.id_jabatan=b.id_jabatan";
            createTblNoParamUsr(strQUsr);


        }

        private void dgUsr_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // boxId = dgR.Rows[dgR.CurrentRow.Index].Cells[0].Value.GetType(Int32());
            txtNmUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[0].Value.ToString();
            txtUserNmUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[1].Value.ToString();
            txtPassUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[2].Value.ToString();
            txtTelpUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[3].Value.ToString();
            txtEmailUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[4].Value.ToString();
            cmbJbtUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[5].Value.ToString();
        }

        ///
        ///Suhendro Update
        ///

        private void btnRights_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            refreshActivatedButton();
            btnRights.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnRights.FlatAppearance.BorderSize = 2;
                        
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            hideBookingElement(); 
            panelKamarDibooking.Controls.Clear();
            hidepanelPengaturanKamar();
            panelRights1.BringToFront();
            string strQ = "select * from jabatan";
            createTblNoParam(strQ);
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            //panelCheckinDate.Visible = false;
            //panelCheckoutDate.Visible = false;

        }

        private void dgUsr_Click(object sender, EventArgs e)
        {
            // boxId = dgR.Rows[dgR.CurrentRow.Index].Cells[0].Value.GetType(Int32());
            txtNmUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[0].Value.ToString();
            txtPassUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[1].Value.ToString();
            txtUserNmUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[2].Value.ToString();
            txtTelpUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[3].Value.ToString();
            txtEmailUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[4].Value.ToString();
            cmbJbtUsr.Text = dgUsr.Rows[dgUsr.CurrentRow.Index].Cells[5].Value.ToString();

        }

        private void tambahToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string lmaHari = Interaction.InputBox("Lama Hari =");
            try
            {
                //MessageBox.Show( DateTime.Now.Date.AddDays(Int32.Parse(lmaHari)).ToString());
                SqlCommand sql1 = new SqlCommand("select checkout from Reservasi where kamar_no =@noKamar and status = 'checkin'", koneksi.KoneksiDB());
                sql1.Parameters.AddWithValue("@noKamar", dataKamarCh);
                //tambahBaru
                DateTime haricheck = Convert.ToDateTime(sql1.ExecuteScalar().ToString());
                //DATENAME(dw,tanggal_id) in ('Saturday','Sunday')
                koneksi.closeConnection();

                SqlCommand sql2 = new SqlCommand(@"select distinct kamar_no,r.checkin, checkout
	            from 
	            Reservasi r
                where 
	                (
                       (r.checkin>=@checkindate and r.checkout<=@checkoutdate and r.checkout>@checkindate)
                        or 
                       (r.checkin>=@checkindate and r.checkout>=@checkoutdate and r.checkin<@checkoutdate)
                    )
                    and r.status in ('booking','checkin') and r.kamar_no=@kamarno", koneksi.KoneksiDB());
                sql2.Parameters.AddWithValue("@checkindate", haricheck);//baruuuu
                sql2.Parameters.AddWithValue("@checkoutdate", haricheck.AddDays(Int32.Parse(lmaHari)));
                sql2.Parameters.AddWithValue("@kamarno", dataKamarCh);
                SqlDataReader readDataCheck = sql2.ExecuteReader();
                bool checkDataX = false;
                while (readDataCheck.Read())
                {
                    checkDataX = true;
                }

                koneksi.closeConnection();
                if (checkDataX == false)
                {//tambahBaru

                    SqlCommand sql = new SqlCommand(@"
                select case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga 
                from 
                Harga 
                inner join Kamar
                on Kamar.kamar_tipe_id = Harga.kamar_tipe_id and Kamar.kamar_no=@NOKAMAR 
                and Harga.tanggal_id >= convert(date,@chin) and Harga.tanggal_id< @chou
            ", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@NOKAMAR", dataKamarCh);
                    sql.Parameters.AddWithValue("@chin", haricheck);
                    sql.Parameters.AddWithValue("@chou", haricheck.AddDays(Int32.Parse(lmaHari)));

                    SqlDataReader sqlDataHarga = sql.ExecuteReader();
                    int jumHargaLama = 0;
                    while (sqlDataHarga.Read())
                    {
                        jumHargaLama += Int32.Parse(sqlDataHarga["harga"].ToString());
                    }
                    koneksi.closeConnection();

                    sql = new SqlCommand("update Reservasi set checkout = @tnggalcheck,realcheckout=@tnggalcheck, tag_kamar = tag_kamar + @nilai where status='checkin' and kamar_no=@kamarno", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@kamarno", dataKamarCh);
                    sql.Parameters.AddWithValue("@nilai", jumHargaLama);
                    sql.Parameters.AddWithValue("@tnggalcheck", haricheck.AddDays(Int32.Parse(lmaHari)));
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    sql = new SqlCommand("select booking_id from Reservasi where status='checkin' and kamar_no = @kamarno", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@kamarno", dataKamarCh);
                    int kodeid = Int32.Parse(sql.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    sql = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", kodeid);
                    int kodediskon = Int32.Parse(sql.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    sql = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", kodediskon);
                    float potongan = float.Parse(sql.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    sql = new SqlCommand("update Booking set grand_total = grand_total + @nilai,tag_kamar=tag_kamar+@nilai, balance_due=balance_due+@nilai2 where  booking_id=@id", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@nilai", jumHargaLama);
                    sql.Parameters.AddWithValue("@nilai2", (int)(jumHargaLama * potongan) / 100);

                    sql.Parameters.AddWithValue("@id", kodeid);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();
                }
                else
                {
                    MessageBox.Show("Maaf Anda Tidak Bisa Memperpanjang Hari\n Dikarenakan Kamar tersebut sudah dibooking");
                }
            }//tambahBaru
            catch
            {
                if (lmaHari != "")
                {
                    MessageBox.Show("Inputan harus dalam bentuk integer!");
                }
            }


            btnCheckInStatus_Click(sender, e);
        }

        private void btnCariItemPesan_Click(object sender, EventArgs e)
        {
        }

        private void btnRemovePesan_Click(object sender, EventArgs e)
        {
            List<DataRow> rd = new List<DataRow>();
            int index = 0;
            foreach (DataRow dr in dtPesan.Rows)
            {
                if (index == dataGridView4.CurrentRow.Index)
                {
                    rd.Add(dr);
                }

                index += 1;
            }

            foreach (var r in rd)
            {
                dtPesan.Rows.Remove(r);
            }
            dtPesan.AcceptChanges();
            dataGridView4.DataSource = dtPesan;
        }

        private void btnClearPesan_Click(object sender, EventArgs e)
        {

            dtPesan.Clear();
        }

        private void btnaddItemPesan_Click(object sender, EventArgs e)
        {
            if (!xPenang.Text.Equals(""))
            {
                DataRow dr = dtPesan.NewRow();
                dr["IdItem"] = xPenang.Text;
                dr["Pesanan"] = label15.Text;
                dr["Jumlah"] = txtJumlahPesanItem.Text;
                dr["RESERVASI_ID"] = label22.Text;
                dr["Tanggal"] = DateTime.Now.Date;
                if (cbKriteriaCari.Text.Equals("Laundry") || cbKriteriaCari.Text.Equals("Lainnya"))
                {
                    dr["Harga"] = (float.Parse(txtHargaLaundry.Text) * float.Parse(txtJumlahPesanItem.Text.Replace(',','.'))).ToString();
                }
                else
                {
                    dr["Harga"] = (Int32.Parse(label3.Text) * Int32.Parse(txtJumlahPesanItem.Text)).ToString();

                }
                dtPesan.Rows.Add(dr);
                dataGridView1.DataSource = dtPesan;
                xPenang.Text = "";
                label15.Text = "";
                txtHargaLaundry.Text = "";
                label3.Text = "";
                txtJumlahPesanItem.Text = "";
            }
        }

        private void btnSubmitPesan_Click(object sender, EventArgs e)
        {
            List<DataRow> rd = new List<DataRow>();
            foreach (DataRow dr in dtPesan.Rows)
            {

                SqlCommand sql = new SqlCommand("insert into Pemesanan(item_id, reservasi_id, tgl_pemesanan, harga) values (@a,@b,@c,@d)", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", dr["IdItem"]);
                sql.Parameters.AddWithValue("@b", dr["RESERVASI_ID"]);
                sql.Parameters.AddWithValue("@c", dr["Tanggal"]);
                sql.Parameters.AddWithValue("@d", dr["Harga"]);

                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand("select booking_id from Reservasi where reservasi_id = @id", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@id", dr["RESERVASI_ID"]);
                string databooking = (sql.ExecuteScalar().ToString());

                koneksi.closeConnection();

                sql = new SqlCommand("update Booking set grand_total=grand_total+@biaya, balance_due = balance_due+@biaya  where booking_id=@id", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@biaya", dr["Harga"]);
                sql.Parameters.AddWithValue("@id", Int32.Parse(databooking));

                sql.ExecuteNonQuery();
                koneksi.closeConnection();
                sql = new SqlCommand("update Reservasi set tag_restoran=tag_restoran+@biaya where booking_id=@id and reservasi_id=@idr", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@biaya", dr["Harga"]);
                sql.Parameters.AddWithValue("@idr", dr["RESERVASI_ID"]);
                sql.Parameters.AddWithValue("@id", Int32.Parse(databooking));

                sql.ExecuteNonQuery();
                koneksi.closeConnection();
            }
            dtPesan.Clear();
            label22.Text = "-";
            label3.Text = "-";
            label15.Text = "-";
            xPenang.Text = "";
            PanelPesan.BringToFront();
            cbKriteriaCari.Text = "Makanan";
        }

        private void TxtCust_TextChanged(object sender, EventArgs e)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = dataGridView5.DataSource;
            bs.Filter = "item like '%" + txtcariItemPesan.Text + "%'";
            dataGridView5.DataSource = bs;
        }

        private void dataGridView5_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            xPenang.Text = dataGridView5[0, e.RowIndex].Value.ToString();
            label15.Text = dataGridView5[1, e.RowIndex].Value.ToString();
            label3.Text = dataGridView5[3, e.RowIndex].Value.ToString();
            txtHargaLaundry.Text = dataGridView5[3, e.RowIndex].Value.ToString(); 
            panel2.Visible = false;
        }

        private void txtcariItemPesan_TextChanged(object sender, EventArgs e)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = dataGridView5.DataSource;
            bs.Filter = "item like '%" + txtcariItemPesan.Text + "%'";
            dataGridView5.DataSource = bs;
        }

        private void dataGridView3_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            //string cellValue = dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            //DBNull isnull;
            dataGridView3.ReadOnly = false;
           dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
           
        }

        private void dataGridView3_CellEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Red)
            {
                contextMenuStrip1.Show(Cursor.Position);
                rowSelect = e.RowIndex;
                columnSelect = e.ColumnIndex;
            }
        }

        private void dataGridView6_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        public void reloaddata()
        {
            SqlCommand sql;
            //SqlDataReader reader;
            Button[] JKamar;
            int jumKamar;

            panelRestoran.Controls.Clear();

            sql = new SqlCommand("select count(*) from MejaRestaurant", koneksi.KoneksiDB());
            jumKamar = (int)sql.ExecuteScalar();
            koneksi.closeConnection();

            sql = new SqlCommand("select NoMeja,StatusMeja from MejaRestaurant", koneksi.KoneksiDB());
            reader = sql.ExecuteReader();
            JKamar = new Button[jumKamar+2];
            int ctr = 0;
            while (reader.Read())
            {
                JKamar[ctr] = new Button();
                JKamar[ctr].Text = (ctr+1).ToString();
                JKamar[ctr].Name = reader.GetInt32(0).ToString();
                JKamar[ctr].Visible = true;
                JKamar[ctr].Height = 45;
                JKamar[ctr].Width = 95;
                JKamar[ctr].Image = Sistem_Booking_Hotel.Properties.Resources.resto;
                JKamar[ctr].ImageAlign = ContentAlignment.MiddleLeft;
                //JKamar[ctr].Click += new EventHandler(tambah_kamar);
                //JKamar[ctr].MouseEnter += new EventHandler(button1_MouseEnter_2);
                //JKamar[ctr].MouseLeave += new EventHandler(button1_MouseLeave_1);

                //JKamar[ctr] = new Label();
                //JKamar[ctr].Text = reader.GetInt32(0).ToString();
                //JKamar[ctr].Name = reader.GetInt32(0).ToString();
                //JKamar[ctr].Size = new Size(panjangLbl, lebarLbl);
                //JKamar[ctr].Font = new Font("Arial", 16);
                //JKamar[ctr].TextAlign = ContentAlignment.MiddleCenter;

                //JKamar[ctr].Location = new Point((panjangLbl * posX) + (20 * posX), (posY * lebarLbl) + (20 * posY));
                if (reader.GetString(1).ToString().Equals("S"))
                {
                    JKamar[ctr].BackColor = Color.Green;
                }
                else
                {
                    JKamar[ctr].BackColor = Color.Gray;

                }
                JKamar[ctr].Click += new EventHandler(JKClick);
                JKamar[ctr].MouseEnter += new EventHandler(JKEnter);
                JKamar[ctr].MouseLeave += new EventHandler(JKLeave);

                panelRestoran.Controls.Add(JKamar[ctr]);

                ctr += 1;

            }
            koneksi.closeConnection();
            JKamar[ctr] = new Button();
            JKamar[ctr].Text = "+";
            JKamar[ctr].Name = "+";
            JKamar[ctr].Visible = true;
            JKamar[ctr].Height  = 45;
            JKamar[ctr].Width = 95;
            
            JKamar[ctr].BackColor = Color.Snow;
            JKamar[ctr].Click += new EventHandler(JKPlusMeja);
            panelRestoran.Controls.Add(JKamar[ctr]);
            ctr += 1;
            JKamar[ctr] = new Button();
            JKamar[ctr].Text = "-";
            JKamar[ctr].Name = "-";
            JKamar[ctr].Visible = true;
            JKamar[ctr].Height = 45;
            JKamar[ctr].Width = 95;
            JKamar[ctr].BackColor = Color.Snow;
            JKamar[ctr].Click += new EventHandler(JKMinusMeja);
            panelRestoran.Controls.Add(JKamar[ctr]);
            ctr += 1;
        }
        private void JKPlusMeja(object sender, EventArgs e)
        {
            SqlCommand sql = new SqlCommand("Insert into MejaRestaurant(StatusMeja) values ('S')",koneksi.KoneksiDB());
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            reloaddata();
        }
        private void JKMinusMeja(object sender, EventArgs e)
        {
            SqlCommand sql = new SqlCommand("delete from MejaRestaurant where NoMeja = (select max(NoMeja) from MejaRestaurant)", koneksi.KoneksiDB());
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            reloaddata();
        }   
        private void JKEnter(object sender, EventArgs e)
        {
            Button enter = sender as Button;
            String noKamar = "-";
            String namaTamu = "-";
            try
            {
                SqlCommand sql = new SqlCommand("select h.NoKamar from HRestaurant h, MejaRestaurant m where m.StatusMeja = 'R' and h.flag='M' and m.NoMeja = @nMeja and h.NoMeja=m.NoMeja group by h.NoKamar ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@nMeja", enter.Name);
                noKamar = sql.ExecuteScalar().ToString();
                koneksi.closeConnection();

                sql = new SqlCommand("select Tamu.tamu from Reservasi, Tamu where Reservasi.kamar_no = @kmarno and Reservasi.status = 'checkin' and Reservasi.tamu_id = Tamu.tamu_id ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@kmarno", noKamar);
                namaTamu = sql.ExecuteScalar().ToString();
                koneksi.closeConnection();
            }
            catch
            {
            }

            toolTip1.Show(" No Meja : " + enter.Text + "\n No Kamar : " + noKamar + "\n Tamu : " + namaTamu, enter);
        }

        private void JKLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(sender as Button);
        }

        //DataTable dt;
        int noKamarRestoran = 0;
        Boolean tambahRestoran = false;
        int noMejaDiclick = 0;
        
        private void JKClick(object sender, EventArgs e)
        {
            HargaItem.Text = "";
            inputJumlahItem.Text = "1";
            inputIdItem.Text = "";
            namaItem.Text = "";
            totalHargaItem.Text = "0";

            Button lbl = sender as Button;
            dt = new DataTable();
            if (lbl.BackColor == Color.Green)
            {
                tambahRestoran = false;
                //panelPesanRestoran.Visible = true;
                noMeja.Text = lbl.Text;
                noMejaDiclick = Int32.Parse(lbl.Name.ToString());
        
                /*
                cb_inputNoKamar.Items.Clear();
                cb_inputNoKamar.Items.Add("-");
                SqlCommand sql = new SqlCommand("select kamar_no from reservasi where status='checkin'", koneksi.KoneksiDB());
                reader = sql.ExecuteReader();
                while (reader.Read())
                {
                    cb_inputNoKamar.Items.Add(reader.GetValue(0));
                }
                koneksi.closeConnection();
                cb_inputNoKamar.Enabled = true;

                cb_inputNoKamar.SelectedIndex = 0;
                */
                GridViewAddItem.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dt.Columns.Add("IdItem".ToString());
                dt.Columns.Add("Pesanan".ToString());
                dt.Columns.Add("Jumlah".ToString());
                dt.Columns.Add("Tanggal", typeof(DateTime));
                dt.Columns.Add("Harga".ToString());
                dt.Columns.Add("SubTotal".ToString());
                GridViewAddItem.DataSource = dt;

                panelPesanRestoran.BringToFront();
                //panelCariItem.Visible = false;
            }
            else
            {
                tambahRestoran = true;

                contextMenuRestoran.Show(Cursor.Position);

                noMejaPembayaran.Text = lbl.Name;
                noMeja.Text = lbl.Text;
                noMejaDiclick = Int32.Parse(lbl.Name.ToString());
        
                /*
                cb_inputNoKamar.Items.Clear();
                cb_inputNoKamar.Items.Add("-");
                SqlCommand sql = new SqlCommand("select kamar_no from reservasi where status='checkin'", koneksi.KoneksiDB());
                reader = sql.ExecuteReader();
                while (reader.Read())
                {
                    cb_inputNoKamar.Items.Add(reader.GetValue(0));
                }
                koneksi.closeConnection();
                cb_inputNoKamar.Enabled = false;

                try
                {
                    sql = new SqlCommand("select h.NoKamar from HRestaurant h, MejaRestaurant m where m.StatusMeja = 'R' and h.flag='M' and m.NoMeja = @nMeja and h.NoMeja=m.NoMeja group by h.NoKamar ", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@nMeja", lbl.Text);
                    noKamarRestoran = Int32.Parse(sql.ExecuteScalar().ToString());
                    cb_inputNoKamar.SelectedIndex = cb_inputNoKamar.FindStringExact(noKamarRestoran.ToString());
                    //MessageBox.Show(noKamarRestoran + "");
                    koneksi.closeConnection();
                }
                catch
                {
                    noKamarRestoran = 0;
                    cb_inputNoKamar.SelectedIndex = 0;
                }
                 */
                GridViewAddItem.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dt.Columns.Add("IdItem".ToString());
                dt.Columns.Add("Pesanan".ToString());
                dt.Columns.Add("Jumlah".ToString());
                dt.Columns.Add("Tanggal", typeof(DateTime));
                dt.Columns.Add("Harga".ToString());
                dt.Columns.Add("SubTotal".ToString());
                GridViewAddItem.DataSource = dt;

            }
        }

        private void btn_restoran_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            resetBtnKonfigurasi();
            refreshActivatedButton();
            btn_restoran.FlatAppearance.BorderColor = Color.DodgerBlue;
            btn_restoran.FlatAppearance.BorderSize = 2;
                        
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement(); 
            panelKamarDibooking.Controls.Clear();
            panelRestoran.BringToFront();
            //panelCheckinDate.Visible = false;
            //panelCheckoutDate.Visible = false;

            //panelPesanRestoran.Visible = false;
            reloaddata();

            //reportInvoice.BringToFront();
            //List<Microsoft.Reporting.WinForms.ReportParameter> list = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            //Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("NomorMeja", noMejaPembayaran.Text);
            //list.Add(param);
            //reportInvoice.ServerReport.SetParameters(list);
            //reportInvoice.ServerReport.Refresh();
            //reportInvoice.RefreshReport();
            panelPembayaranRestoran.Visible = false;        
            btn_cari_item_Click(sender, e);
        }

        private void tambahToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            panelPesanRestoran.BringToFront();
            //panelCariItem.Visible = false;
        }

        private void bayarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*if (noKamarRestoran == 0)
            {
                SqlCommand sql = new SqlCommand("update MejaRestaurant set StatusMeja ='S' where NoMeja=@a", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", noMejaPembayaran.Text);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand("select max(noPemesanan) from HRestaurant where noMeja =@a", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", noMejaPembayaran.Text);
                int nopesan = Int32.Parse(sql.ExecuteScalar().ToString());
                koneksi.closeConnection();

                sql = new SqlCommand("update HRestaurant set flag ='S' where NoMeja=@a and noPemesanan = @b ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", noMejaPembayaran.Text);
                sql.Parameters.AddWithValue("@b", nopesan);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                //panelPembayaranRestoran.Visible = true;
                //panelPembayaranRestoran.BringToFront();
                btn_submitPembayaranRestoran_Click(sender, e);

            }
            else
            {*/
                comboboxPembayaran.Items.Clear();
                ComboboxItem item = new ComboboxItem();
                item.Text = "Bayar Langsung";
                item.Value = 1;
                comboboxPembayaran.Items.Add(item);
                item = new ComboboxItem();
                item.Text = "Simpan";
                item.Value = 2;
                comboboxPembayaran.Items.Add(item);
                comboboxPembayaran.Text = "Bayar Langsung";


                cb_inputNoKamar.Items.Clear();
                cb_inputNoKamar.Items.Add("-");
                SqlCommand sql = new SqlCommand("select kamar_no from reservasi where status='checkin'", koneksi.KoneksiDB());
                reader = sql.ExecuteReader();
                while (reader.Read())
                {
                    cb_inputNoKamar.Items.Add(reader.GetValue(0));
                }
                cb_inputNoKamar.SelectedIndex = 0;
                koneksi.closeConnection();

                cb_jenisPembaynaranRestor.SelectedIndex = 0;

                panelBayarRestoran.BringToFront();
            //}
        }

        private void btn_cari_item_Click(object sender, EventArgs e)
        {
            panelCariItem.Visible = true;

            SqlDataAdapter da = new SqlDataAdapter("select Item.item_id, Item.item, Item_Tipe.item_tipe, Item.harga from Item, Item_Tipe where Item.item_tipe_id = Item_Tipe.item_tipe_id and Item_tipe.item_tipe='Makanan'", koneksi.KoneksiDB());
            DataTable ds = new DataTable();
            da.Fill(ds);
            GridViewItem.DataSource = ds;
            GridViewItem.Columns[0].Visible = false;
            GridViewItem.Columns[2].Visible = false;
            koneksi.closeConnection();

        }

        private void btn_addItem_Click(object sender, EventArgs e)
        {
            try
            {
                totalHargaItem.Text = (Int32.Parse(totalHargaItem.Text) + (Int32.Parse(HargaItem.Text) * Int32.Parse(inputJumlahItem.Text))).ToString();
                DataRow dr = dt.NewRow();
                dr["IdItem"] = inputIdItem.Text;
                dr["Pesanan"] = namaItem.Text;
                dr["Jumlah"] = inputJumlahItem.Text;
                dr["Tanggal"] = DateTime.Now;
                dr["Harga"] = HargaItem.Text;
                dr["SubTotal"] = (Int32.Parse(HargaItem.Text) * Int32.Parse(inputJumlahItem.Text)).ToString();
                dt.Rows.Add(dr);
                GridViewAddItem.DataSource = dt;
                inputJumlahItem.Text = "1";
            }
            catch
            {
                MessageBox.Show("Silakan lakukan pengecekan inputan");
            }
        
        }

        private void inputCariItem_TextChanged(object sender, EventArgs e)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = GridViewItem.DataSource;
            bs.Filter = "item like '%" + inputCariItem.Text + "%'";
            GridViewItem.DataSource = bs;
        
        }

        private void checkouthariIni()
        {
            listDataKamar.Items.Clear();
            SqlCommand queryCheck = new SqlCommand("select kamar_no from Reservasi where datediff(minute,checkout,@a)<5 and status='checkin'", koneksi.KoneksiDB());
            queryCheck.Parameters.AddWithValue("@a", DateTime.Now);
            SqlDataReader readCheck = queryCheck.ExecuteReader();
            while (readCheck.Read())
            {
                listDataKamar.Items.Add(readCheck["kamar_no"].ToString());
            }
            koneksi.closeConnection();
        }

        private void btn_submitItem_Click(object sender, EventArgs e)
        {
            //if (cb_inputNoKamar.Enabled)
            if(tambahRestoran == false)
            {
                //Console.WriteLine(noMeja.Text + "-" + cb_inputNoKamar.SelectedItem);
                //Console.WriteLine(noMeja.Text + "-" + cb_inputNoKamar.SelectedItem);
                SqlCommand sql;
                int reservasi_id = 0;
                try
                {
                    sql = new SqlCommand("Select reservasi_id from reservasi where kamar_no=@nkmr and status='checkin'", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@nkmr", cb_inputNoKamar.Text);
                    reservasi_id = Int32.Parse(sql.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }
                catch
                {
                }

                sql = new SqlCommand("insert into HRestaurant(TglPesan, NoKamar,NoMeja, Biaya, flag, reservasi_id) values (@a,@b,@c,@d,'M',@e)", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", DateTime.Now);
                /*if (cb_inputNoKamar.Text != "-")
                {
                    sql.Parameters.AddWithValue("@b", cb_inputNoKamar.Text);
                    sql.Parameters.AddWithValue("@e", reservasi_id);
                }
                else
                {*/
                    sql.Parameters.AddWithValue("@b", DBNull.Value);
                    sql.Parameters.AddWithValue("@e", DBNull.Value);
                //}
                sql.Parameters.AddWithValue("@c", noMejaDiclick);
                sql.Parameters.AddWithValue("@d", Int32.Parse(totalHargaItem.Text));

                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand("update MejaRestaurant set StatusMeja = 'R' where NoMeja = @no  ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@no", noMejaDiclick);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand("select MAX(NoPemesanan) from HRestaurant", koneksi.KoneksiDB());
                int nopesan = Int32.Parse(sql.ExecuteScalar().ToString());
                koneksi.closeConnection();

                List<DataRow> rd = new List<DataRow>();
                foreach (DataRow dr in dt.Rows)
                {
                    sql = new SqlCommand("insert into DRestaurant(NoPemesanan, NoItem,Jumlah, SubBiaya) values (@a,@b,@c,@d)", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", nopesan);
                    sql.Parameters.AddWithValue("@b", dr["IdItem"]);
                    sql.Parameters.AddWithValue("@c", dr["Jumlah"]);
                    sql.Parameters.AddWithValue("@d", dr["SubTotal"]);

                    sql.ExecuteNonQuery();

                    koneksi.closeConnection();
                }

            }
            else
            {
                try
                {
                    SqlCommand sql = new SqlCommand("Select NoPemesanan from HRestaurant where NoMeja=@nMeja and flag='M'", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@nMeja", noMejaDiclick);
                    int nopesan = Int32.Parse(sql.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    sql = new SqlCommand("update HRestaurant set Biaya=Biaya+@biaya where NoPemesanan=@nopesan", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@biaya", totalHargaItem.Text);
                    sql.Parameters.AddWithValue("@nopesan", nopesan);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    List<DataRow> rd = new List<DataRow>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        sql = new SqlCommand("insert into DRestaurant(NoPemesanan, NoItem,Jumlah, SubBiaya) values (@a,@b,@c,@d)", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", nopesan);
                        sql.Parameters.AddWithValue("@b", dr["IdItem"]);
                        sql.Parameters.AddWithValue("@c", dr["Jumlah"]);
                        sql.Parameters.AddWithValue("@d", dr["SubTotal"]);

                        sql.ExecuteNonQuery();

                        koneksi.closeConnection();
                    }

                }
                catch
                { }
            }
            btn_restoran_Click(sender, e);
        
        }

        private void btn_removeItem_Click(object sender, EventArgs e)
        {
            List<DataRow> rd = new List<DataRow>();
            int index = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (index == GridViewAddItem.CurrentRow.Index)
                {
                    totalHargaItem.Text = (Convert.ToInt32(totalHargaItem.Text) - Convert.ToInt32(dr["SubTotal"].ToString())).ToString();
                    rd.Add(dr);
                }

                index += 1;
            }

            foreach (var r in rd)
            {
                dt.Rows.Remove(r);
            }
            dt.AcceptChanges();
            GridViewAddItem.DataSource = dt;
        
        }

        private void btn_clearItem_Click(object sender, EventArgs e)
        {
            dt.Clear();
            totalHargaItem.Text = "0";
        }

        private void GridViewItem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void GridViewItem_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            inputIdItem.Text = GridViewItem[0, e.RowIndex].Value.ToString();
            namaItem.Text = GridViewItem[1, e.RowIndex].Value.ToString();
            HargaItem.Text = GridViewItem[3, e.RowIndex].Value.ToString();

            //panelCariItem.Visible = false;
        
        }

        private void _bayarRestoran_Click(object sender, EventArgs e)
        {
            SqlCommand sql;

            sql = new SqlCommand("select max(noPemesanan) from HRestaurant where noMeja =@a", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", noMejaDiclick);
            int nopesan = Int32.Parse(sql.ExecuteScalar().ToString());
            koneksi.closeConnection();

            if(cb_inputNoKamar.Text != "-"){

                sql = new SqlCommand("select reservasi_id from reservasi where kamar_no=@nokamar and status='checkin'", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@nokamar", cb_inputNoKamar.Text);
                String reservasi_id = sql.ExecuteScalar().ToString();
                koneksi.closeConnection();

                sql = new SqlCommand("update Hrestaurant set NoKamar=@NoKamar, reservasi_id=@r_id where NoPemesanan=@a", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", nopesan);
                sql.Parameters.AddWithValue("@NoKamar", cb_inputNoKamar.Text);
                sql.Parameters.AddWithValue("@r_id", reservasi_id);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                noKamarRestoran = Int32.Parse(cb_inputNoKamar.Text);
            }
            else{
                    noKamarRestoran = 0;
            }
            //
            sql = new SqlCommand("update MejaRestaurant set StatusMeja ='S' where NoMeja=@a", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", noMejaDiclick);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            

            if (comboboxPembayaran.Text.Equals("Bayar Langsung"))
            {
                

                sql = new SqlCommand("update HRestaurant set flag ='S' where NoMeja=@a and noPemesanan = @b ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", noMejaDiclick);
                sql.Parameters.AddWithValue("@b", nopesan);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                //panelPembayaranRestoran.Visible = true;
                //panelPembayaranRestoran.BringToFront();
                btn_submitPembayaranRestoran_Click(sender, e);
            }
            else
            {
                /*
                sql = new SqlCommand("select NoKamar from HRestaurant where NoPemesanan =@a", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", nopesan);
                int noKamarA = Int32.Parse(sql.ExecuteScalar().ToString());
                koneksi.closeConnection();
                */
                sql = new SqlCommand("update HRestaurant set flag ='B' where NoMeja=@a and noPemesanan = @b ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", noMejaDiclick);
                sql.Parameters.AddWithValue("@b", nopesan);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand("select Biaya from HRestaurant where NoPemesanan=@b ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@b", nopesan);
                int biayaBaru = Int32.Parse(sql.ExecuteScalar().ToString());
                koneksi.closeConnection();

                sql = new SqlCommand("update Reservasi set tag_restoran=tag_restoran+@a where kamar_no = @b and status='checkin'", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", biayaBaru);
                sql.Parameters.AddWithValue("@b", cb_inputNoKamar.Text);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand("select booking_id from Reservasi where kamar_no=@a and status='checkin'", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", cb_inputNoKamar.Text);
                int idbooking = Int32.Parse(sql.ExecuteScalar().ToString());
                koneksi.closeConnection();

                sql = new SqlCommand("update Booking set balance_due=balance_due+@a, grand_total=grand_total+@a where booking_id=@b", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", biayaBaru);
                sql.Parameters.AddWithValue("@b", idbooking);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                MessageBox.Show("Tag restoran telah ditambahkan!");
                btn_restoran_Click(sender, e);
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {

                if (!inputNamaDT.Text.Equals("") && !inputTlpnDT.Text.Equals("") && !inputSebutanDT.Text.Equals(""))
                {
                    SqlCommand cmd = new SqlCommand((@"update Tamu set tamu=@nama, alamat=@alamat, kota=@kota, telepon=@telepon, email=@email, perusahaan=@perusahaan, tanggallahir=@tgllhr, sebutan=@sebutan, gelar=@gelar, noidentitas=@noide, jenisidentitas =@jeniside, warganegara=@l where tamu_id=@tamu_id"), koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@tamu_id", tamu_id);
                    cmd.Parameters.AddWithValue("@nama", inputNamaDT.Text);
                    cmd.Parameters.AddWithValue("@alamat", inputAlamatDT.Text);
                    cmd.Parameters.AddWithValue("@kota", inputKotaDT.Text);
                    cmd.Parameters.AddWithValue("@telepon", inputTlpnDT.Text);
                    cmd.Parameters.AddWithValue("@email", inputEmailDT.Text);
                    cmd.Parameters.AddWithValue("@perusahaan", inputPerusahaanDT.Text);
                    if (inputTglLhrDT.Value.Date.Year != 1900)
                    {
                        cmd.Parameters.Add("@tgllhr", SqlDbType.DateTime).Value = inputTglLhrDT.Value.Date;
                    }
                    else
                    {
                        cmd.Parameters.Add("@tgllhr", SqlDbType.DateTime).Value = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue("@sebutan", inputSebutanDT.Text);
                    cmd.Parameters.AddWithValue("@gelar", inputGelarDT.Text);
                    cmd.Parameters.AddWithValue("@noide", txtNoIdentitasPanelTamu.Text);
                    cmd.Parameters.AddWithValue("@jeniside", cbJenisIdentitasPanelTamu.Text);
                    cmd.Parameters.AddWithValue("@l", txtwntambah.Text);
                    cmd.ExecuteNonQuery();

                    koneksi.closeConnection();

                    MessageBox.Show("Data Tamu telah diubah");
                    refreshdataTamu();
                    refreshGridDataTamu(GridViewDaftarTamu);
                }
                else
                {
                    MessageBox.Show("Pastikan data terisi");
                }
            }
            catch
            {
                MessageBox.Show("Mohon pilih data tamu terlebih dahulu.");
            }
        }

        private void inputSearchDT_TextChanged(object sender, EventArgs e)
        {
            inputSearchPerusahaanDT.Text = "";
            //if (inputSearchDT.Text.Length >= 3)
            //{
                BindingSource bs = new BindingSource();
                bs.DataSource = GridViewDaftarTamu.DataSource;
                bs.Filter = "tamu like '%" + inputSearchDT.Text + "%'";
                GridViewDaftarTamu.DataSource = bs;
            //}
        }

        private void inputSearchPerusahaanDT_TextChanged(object sender, EventArgs e)
        {
            //if (inputSearchPerusahaanDT.Text.Length >= 3)
            //{
            inputSearchDT.Text = "";
            BindingSource bs = new BindingSource();
            bs.DataSource = GridViewDaftarTamu.DataSource;
            //bs.Filter = "tanggallahir = '" + String.Format("{0:M/d/yyyy}", DateTime.Now.Date) +"' order by kota";
            bs.Filter = "perusahaan like '%" + inputSearchPerusahaanDT.Text + "%'";
            //bs.Filter = "bulan_lahir = " + DateTime.Now.Month.ToString();
            //bs.Filter += " and hari_lahir >= " + DateTime.Now.Day.ToString();
            //bs.Sort = "hari_lahir";
            GridViewDaftarTamu.DataSource = bs;
            //}
        }

        string tamu_id;
        private void GridViewDaftarTamu_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SqlCommand cmd = new SqlCommand((@"select * from Tamu where tamu_id=@tamu_id"), koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@tamu_id", GridViewDaftarTamu[14, e.RowIndex].Value.ToString());
            tamu_id = GridViewDaftarTamu[14, e.RowIndex].Value.ToString();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                inputNamaDT.Text = reader.GetValue(1).ToString();
                inputAlamatDT.Text = reader.GetValue(2).ToString();
                inputKotaDT.Text = reader.GetValue(3).ToString();
                inputTlpnDT.Text = reader.GetValue(4).ToString();
                inputEmailDT.Text = reader.GetValue(5).ToString();
                inputPerusahaanDT.Text = reader.GetValue(6).ToString();
                if (reader.GetValue(7) != DBNull.Value)
                {
                    //Console.WriteLine(reader.GetValue(7));
                    inputTglLhrDT.Value = Convert.ToDateTime(reader.GetValue(7));
                }
                else
                {
                    inputTglLhrDT.Value = Convert.ToDateTime("1900-1-1 16:58:00"); ;
                }
                inputSebutanDT.Text = reader.GetValue(8).ToString();
                inputGelarDT.Text = reader.GetValue(9).ToString();
                txtNoIdentitasPanelTamu.Text = reader["noidentitas"].ToString();
                cbJenisIdentitasPanelTamu.Text = reader["jenisidentitas"].ToString();
                txtwntambah.Text = reader["warganegara"].ToString();
             

            }
            koneksi.closeConnection();

            //GridViewDaftarTamu.Visible = false;
        
        }

        int total_data_tamu = 0;
        private void inputCariNamaTamu_TextChanged(object sender, EventArgs e)
        {

            //if (inputCariNamaTamu.Text.Length >= 3)
            //{
                BindingSource bs = new BindingSource();
                bs.DataSource = datagridTamu.DataSource;
                bs.Filter = "tamu like '%" + inputCariNamaTamu.Text + "%'";
                total_data_tamu = bs.Count;
                //MessageBox.Show(bs.Count.ToString());
                datagridTamu.DataSource = bs;
            //}
        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (opsistatusbookingkamar == 1)
            {
                DataTamuKalender.Visible = false;
                //noIDdatatamu = Int32.Parse(dataGridView6.Rows[dataGridView6.CurrentRow.Index].Cells[0].Value.ToString());
                //DateTime tanggalPesan1 = Convert.ToDateTime(TglBulan + "/" + dataGridView3.Columns[columnSelect].Name.ToString() + "/" + Tgltahun);
                //DateTime tanggalPesan1 = Convert.ToDateTime(TglBulan + "/" + dataGridView6.Columns[columnSelect].Name.ToString() + "/" + Tgltahun);

                // int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1
                //int NoKamarInfo = Int32.Parse(dataGridView6.Rows[rowSelect].Cells[0].Value.ToString());//tes1
                SqlCommand sqlq = new SqlCommand("select Reservasi.reservasi_id from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and Reservasi.checkin <=@id and Reservasi.checkout > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking')", koneksi.KoneksiDB());
                sqlq.Parameters.AddWithValue("@id", tglcheck);
                sqlq.Parameters.AddWithValue("@nok", noroom);

                string reservasiKamar = sqlq.ExecuteScalar().ToString();
                SqlCommand sql;
                if (noidtamu > 0)
                {
                    if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 7)
                    {
                        sql = new SqlCommand("update Reservasi set status= 'checkin', tamu_id=@a where reservasi_id =@id", koneksi.KoneksiDB());
                    }
                    else
                    {
                        SqlCommand qwe = new SqlCommand("select checkin from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                        qwe.Parameters.AddWithValue("r_id", reservasiKamar);
                        DateTime qwe_checkin = Convert.ToDateTime(qwe.ExecuteScalar().ToString());

                        if(qwe_checkin.Date < DateTime.Now.Date){
                            sql = new SqlCommand("update Reservasi set status= 'checkin', tamu_id=@a where reservasi_id =@id", koneksi.KoneksiDB());
                        }else{
                            sql = new SqlCommand("update Reservasi set status= 'checkin', checkin=SYSDATETIME(), tamu_id=@a where reservasi_id =@id", koneksi.KoneksiDB());
                        }
                        
                    }
                    sql.Parameters.AddWithValue("@a", noidtamu);
                }
                else
                {
                    if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 7)
                    {
                        sql = new SqlCommand("update Reservasi set status= 'checkin' where reservasi_id =@id", koneksi.KoneksiDB());
                    }
                    else
                    {
                        SqlCommand qwe = new SqlCommand("select checkin from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                        qwe.Parameters.AddWithValue("r_id", reservasiKamar);
                        DateTime qwe_checkin = Convert.ToDateTime(qwe.ExecuteScalar().ToString());

                        if (qwe_checkin.Date < DateTime.Now.Date)
                        {
                            sql = new SqlCommand("update Reservasi set status= 'checkin' where reservasi_id =@id", koneksi.KoneksiDB());
                        }
                        else {
                            sql = new SqlCommand("update Reservasi set status= 'checkin', checkin=SYSDATETIME() where reservasi_id =@id", koneksi.KoneksiDB());
                        }
                        
                    }

                }
                sql.Parameters.AddWithValue("@id", reservasiKamar);
                noIDdatatamu = 0;
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0 and ik.Tipe='Rec'", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@room", noroom);
                reader = sql.ExecuteReader();
                while (reader.Read())
                {
                    cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                    cmd.Parameters.AddWithValue("@b", "HK");
                    cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                    cmd.Parameters.AddWithValue("@d", "R");
                    cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                    cmd.Parameters.AddWithValue("@f", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
                koneksi.closeConnection();

                panel4.Visible = false;
                loadKalender(TglBulan, Tgltahun);
                opsistatusbookingkamar = 0;
                refresh_kamar_status_booking();
            }
            else
            {
                //MessageBox.Show(dataGridView6.Rows[e.RowIndex].Cells[0].Value.ToString());

                try
                {
                    
                    noIDdatatamu = Int32.Parse(dataGridView6.Rows[e.RowIndex].Cells[0].Value.ToString());
                    //DateTime tanggalPesan1 = Convert.ToDateTime(TglBulan + "/" + dataGridView3.Columns[columnSelect].Name.ToString() + "/" + Tgltahun);
                    DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

                    int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1
                    SqlCommand sqlq = new SqlCommand("select max(Reservasi.reservasi_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking')", koneksi.KoneksiDB());
                    sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
                    sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);

                    string reservasiKamar = sqlq.ExecuteScalar().ToString();
                    SqlCommand sql;
                    if (noIDdatatamu > 0)
                    {
                        if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 7)
                        {
                            sql = new SqlCommand("update Reservasi set status= 'checkin', tamu_id=@a where reservasi_id =@id", koneksi.KoneksiDB());

                        }
                        else
                        {
                            SqlCommand qwe = new SqlCommand("select checkin from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                            qwe.Parameters.AddWithValue("r_id", reservasiKamar);
                            DateTime qwe_checkin = Convert.ToDateTime(qwe.ExecuteScalar().ToString());

                            if (qwe_checkin.Date < DateTime.Now.Date)
                            {
                                sql = new SqlCommand("update Reservasi set status= 'checkin', tamu_id=@a where reservasi_id =@id", koneksi.KoneksiDB());
                            }
                            else
                            {
                                sql = new SqlCommand("update Reservasi set status= 'checkin', checkin=SYSDATETIME(), tamu_id=@a where reservasi_id =@id", koneksi.KoneksiDB());
                            }

                        }
                        sql.Parameters.AddWithValue("@a", noIDdatatamu);
                    }
                    else
                    {
                        if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 7)
                        {
                            sql = new SqlCommand("update Reservasi set status= 'checkin' where reservasi_id =@id", koneksi.KoneksiDB());
                        }
                        else
                        {
                            SqlCommand qwe = new SqlCommand("select checkin from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                            qwe.Parameters.AddWithValue("r_id", reservasiKamar);
                            DateTime qwe_checkin = Convert.ToDateTime(qwe.ExecuteScalar().ToString());

                            if (qwe_checkin.Date < DateTime.Now.Date)
                            {
                                sql = new SqlCommand("update Reservasi set status= 'checkin' where reservasi_id =@id", koneksi.KoneksiDB());
                            }
                            else
                            {
                                sql = new SqlCommand("update Reservasi set status= 'checkin', checkin=SYSDATETIME() where reservasi_id =@id", koneksi.KoneksiDB());
                            }
                        }

                    }
                    sql.Parameters.AddWithValue("@id", reservasiKamar);
                    noIDdatatamu = 0;
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0 and ik.Tipe='Rec'", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@room", NoKamarInfo);
                    reader = sql.ExecuteReader();
                    while (reader.Read())
                    {
                        cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                        cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                        cmd.Parameters.AddWithValue("@b", "HK");
                        cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                        cmd.Parameters.AddWithValue("@d", "R");
                        cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                        cmd.Parameters.AddWithValue("@f", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                    koneksi.closeConnection();

                    DataTamuKalender.Visible = false;
                    panel4.Visible = false;
                    opsistatusbookingkamar = 0;
                    loadKalender(TglBulan, Tgltahun);
                }
                catch { }
            }
        }

        private void txtNamaCust_TextChanged(object sender, EventArgs e)
        {
            if (txtNamaCust.Text.Length >= 3)
            {
                BindingSource bs = new BindingSource();
                bs.DataSource = dataGridView6.DataSource;
                bs.Filter = "tamu like '%" + txtNamaCust.Text + "%'";
                dataGridView6.DataSource = bs;
            }
        }

        private void checkOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkout_kamar(sender,e,1);
        }

        private void checkout_kamar(object sender, EventArgs e,int notcancelled)
        {
            SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@id", dataKamarCh);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            sql = new SqlCommand("update Reservasi set checkout=@tggal where kamar_no = @id and status='checkin' ", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@tggal", DateTime.Now);
            sql.Parameters.AddWithValue("@id", dataKamarCh);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            int tagihankamar = 0;
            int tagihanpesanan = 0;
            int reservasiid = 0;
            int bookingid = 0; int tamuid = 0;
            sql = new SqlCommand("select reservasi_id,tag_kamar,booking_id,tamu_id,tag_restoran from Reservasi where kamar_no = @id and (datediff(minute,checkout,@tggal)<2 and datediff(minute,checkout,@tggal)>=0)", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@tggal", DateTime.Now);
            sql.Parameters.AddWithValue("@id", dataKamarCh);
            SqlDataReader reader = sql.ExecuteReader();
            while (reader.Read())
            {
                tagihankamar = Convert.ToInt32(reader["tag_kamar"]);
                tagihanpesanan = Convert.ToInt32(reader["tag_restoran"]);
                reservasiid = Convert.ToInt32(reader["reservasi_id"]);
                bookingid = Convert.ToInt32(reader["booking_id"]);
                tamuid = Convert.ToInt32(reader["tamu_id"]);
                // MessageBox.Show(reader.GetInt32(1).ToString());
            }
            koneksi.closeConnection();


            if (notcancelled == 1)
            {
                sql = new SqlCommand("update Reservasi set status= 'checkout' where kamar_no = @id and (datediff(minute,checkout,@tggal)<2 and datediff(minute,checkout,@tggal)>=0) and status='checkin' ", koneksi.KoneksiDB());
            }
            else
            {
                sql = new SqlCommand("update Reservasi set status= '' where kamar_no = @id and (datediff(minute,checkout,@tggal)<2 and datediff(minute,checkout,@tggal)>=0) and status='checkin' ", koneksi.KoneksiDB());
            }

            sql.Parameters.AddWithValue("@tggal", DateTime.Now);
            sql.Parameters.AddWithValue("@id", dataKamarCh);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            SqlCommand sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", bookingid);
            int kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", kodediskon);
            float potongan = float.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sql = new SqlCommand("select downpayment from Reservasi where reservasi_id = @id", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@id", reservasiid);
            int diskon = 0;
            diskon = Int32.Parse(sql.ExecuteScalar().ToString());
            koneksi.closeConnection();

            string hargalebihreser = "";
            hargalebihreser = cekHargaLebih(bookingid);
            int totalbiayakamar = 0;

            if (Convert.ToInt32(hargalebihreser) > 0)
            {
                SqlCommand sqlLbh = new SqlCommand("select payment_id,payment,nopayment,tggalpayment,staff_id from pembayaran where reservasi_id = @a and jumlahpayment = (select max(jumlahpayment) from pembayaran where reservasi_id = @a)", koneksi.KoneksiDB());
                sqlLbh.Parameters.AddWithValue("@a", resIDLebih[0]);
                SqlDataReader readDaLbh = sqlLbh.ExecuteReader();
                string paymentLbh = ""; string paymentnamaLbh = ""; string nopaymentnamaLbh = "";
                string tglpaymentnamaLbh = ""; string staffpaymentnamaLbh = "";
                while (readDaLbh.Read())
                {
                    paymentLbh = readDaLbh["payment_id"].ToString();
                    paymentnamaLbh = readDaLbh["payment"].ToString();
                    nopaymentnamaLbh = readDaLbh["nopayment"].ToString();
                    tglpaymentnamaLbh = readDaLbh["tggalpayment"].ToString();
                    staffpaymentnamaLbh = readDaLbh["staff_id"].ToString();

                }
                koneksi.closeConnection();
                if (((int)((tagihankamar * potongan) / 100) + tagihanpesanan - diskon) > 0)
                {
                    if (Convert.ToInt32(hargalebihreser) - ((int)((tagihankamar * potongan) / 100) + tagihanpesanan - diskon) >= 0)
                    {
                        totalbiayakamar = 0;
                        sqlLbh = new SqlCommand("update pembayaran set jumlahpayment=jumlahpayment-@a where payment_id=@b", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", ((int)((tagihankamar * potongan) / 100) + tagihanpesanan - diskon));
                        sqlLbh.Parameters.AddWithValue("@b", paymentLbh);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment-@a where reservasi_id=@b", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", ((int)((tagihankamar * potongan) / 100) + tagihanpesanan - diskon));
                        sqlLbh.Parameters.AddWithValue("@b", resIDLebih[0]);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment+@a where reservasi_id=@b", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", ((int)((tagihankamar * potongan) / 100) + tagihanpesanan - diskon));
                        sqlLbh.Parameters.AddWithValue("@b", reservasiid);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sqlLbh = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment,staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", bookingid);
                        sqlLbh.Parameters.AddWithValue("@b", reservasiid);
                        sqlLbh.Parameters.AddWithValue("@c", paymentnamaLbh);
                        sqlLbh.Parameters.AddWithValue("@d", nopaymentnamaLbh);
                        sqlLbh.Parameters.AddWithValue("@e", ((int)((tagihankamar * potongan) / 100) + tagihanpesanan - diskon));
                        sqlLbh.Parameters.AddWithValue("@f", tglpaymentnamaLbh);
                        sqlLbh.Parameters.AddWithValue("@g", staffpaymentnamaLbh);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                    }
                    else
                    {
                        sqlLbh = new SqlCommand("update pembayaran set jumlahpayment=jumlahpayment-@a where payment_id=@b", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(hargalebihreser));
                        sqlLbh.Parameters.AddWithValue("@b", paymentLbh);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment-@a where reservasi_id=@b", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(hargalebihreser));
                        sqlLbh.Parameters.AddWithValue("@b", resIDLebih[0]);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment+@a where reservasi_id=@b", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(hargalebihreser));
                        sqlLbh.Parameters.AddWithValue("@b", reservasiid);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sqlLbh = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment,staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", bookingid);
                        sqlLbh.Parameters.AddWithValue("@b", reservasiid);
                        sqlLbh.Parameters.AddWithValue("@c", paymentnamaLbh);
                        sqlLbh.Parameters.AddWithValue("@d", nopaymentnamaLbh);
                        sqlLbh.Parameters.AddWithValue("@e", Convert.ToInt32(hargalebihreser));
                        sqlLbh.Parameters.AddWithValue("@f", tglpaymentnamaLbh);
                        sqlLbh.Parameters.AddWithValue("@g", staffpaymentnamaLbh);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                        totalbiayakamar = ((int)((tagihankamar * potongan) / 100) + tagihanpesanan - diskon) - Convert.ToInt32(hargalebihreser);
                    }
                }

            }
            else
            {
                totalbiayakamar = (int)((tagihankamar * potongan) / 100) + tagihanpesanan - diskon;
            }
            
            if (notcancelled == 0) { totalbiayakamar = 0; }
            sql = new SqlCommand("update Booking set balance_due = balance_due-@total where booking_id = @id", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@id", bookingid);
            sql.Parameters.AddWithValue("@total", totalbiayakamar);
            sql.ExecuteNonQuery();

            koneksi.closeConnection();

            sql = new SqlCommand("insert into pembayaran(booking_id, reservasi_id,payment,nopayment,jumlahpayment,tggalpayment,staff_id) values (@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", bookingid);
            sql.Parameters.AddWithValue("@b", reservasiid);
            sql.Parameters.AddWithValue("@c", "Kontan");
            sql.Parameters.AddWithValue("@d", "");
            sql.Parameters.AddWithValue("@e", totalbiayakamar);
            sql.Parameters.AddWithValue("@f", DateTime.Now);
            sql.Parameters.AddWithValue("@g", Login.idS.ToString());
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            sql = new SqlCommand("update Reservasi set downpayment = downpayment + @a where reservasi_id = @b", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", totalbiayakamar);
            sql.Parameters.AddWithValue("@b", reservasiid);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();
            btnCheckInStatus_Click(sender, e);
            /*
            List<Microsoft.Reporting.WinForms.ReportParameter> list = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("NomorMeja", noMejaPembayaran.Text);
            list.Add(param);
            Microsoft.Reporting.WinForms.ReportParameter param2 = new Microsoft.Reporting.WinForms.ReportParameter("NoPemesanan", nopesan.ToString());
            list.Add(param2);
            
            reportInvoice.ServerReport.SetParameters(list);
            reportInvoice.ServerReport.Refresh();
            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
            */

        }

        private void dgUsr_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void batalToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
            SqlCommand batalQuery = new SqlCommand("select booking_id,tag_kamar, downpayment from Reservasi where kamar_no=@a and status='booking'", koneksi.KoneksiDB());
            batalQuery.Parameters.AddWithValue("@a", dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());
            SqlDataReader bacaQuery = batalQuery.ExecuteReader();
            int kodeBookingId = 0;
            int tagKamarBatal = 0;
            int downPaymentBatal = 0;

            while (bacaQuery.Read())
            {
                kodeBookingId=Int32.Parse(bacaQuery["booking_id"].ToString());
                tagKamarBatal = Int32.Parse(bacaQuery["tag_kamar"].ToString());
                downPaymentBatal = Int32.Parse(bacaQuery["downpayment"].ToString());
            }

            koneksi.closeConnection();

            batalQuery = new SqlCommand("update Reservasi set status='checkout', checkout=@b where kamar_no=@a and status='booking'",koneksi.KoneksiDB());
            batalQuery.Parameters.AddWithValue("@a", dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());
            batalQuery.Parameters.AddWithValue("@b", DateTime.Now);
            batalQuery.ExecuteNonQuery();
            koneksi.closeConnection();


            batalQuery = new SqlCommand("update Booking set balance_due=balance_due-@b where booking_id=@a ", koneksi.KoneksiDB());
            batalQuery.Parameters.AddWithValue("@a", kodeBookingId);
            batalQuery.Parameters.AddWithValue("@b", tagKamarBatal - downPaymentBatal);
            batalQuery.ExecuteNonQuery();
            koneksi.closeConnection();

            loadKalender(TglBulan, Tgltahun);
        }

        private void setTampilanKamar()
        {
            panelKamar.BringToFront();
            panelKamar.Controls.Clear();
            SqlCommand cmd = new SqlCommand((@"select count(kamar_no)
                                from Kamar
                                "), koneksi.KoneksiDB());
            int jumKamar = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();
            ///button1.Text = jumKamar.ToString();
            Button[] Kamar;

            //command.Parameters.AddWithValue("@Username", username);
            //command.Parameters.AddWithValue("@Password", password);

            cmd = new SqlCommand(
            (@"
                select k.kamar_no , k.status,kt.warna
                from Kamar k
                inner join Kamar_Tipe kt
                on k.kamar_tipe_id = kt.kamar_tipe_id
                "), koneksi.KoneksiDB());
            /*

             cmd = new SqlCommand(
            (@"select
            k.kamar_no,
            k.kamar_tipe_id,
            case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga
            from            
            Kamar k
            inner join kamar_tipe kt on k.kamar_tipe_id = kt.kamar_tipe_id 
            inner join harga h on h.tanggal_id = '2008-7-1'
            and kt.kamar_tipe_id = h.kamar_tipe_id"), koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@checkindate",checkinDate.Value.ToString("yyyy-M-d"));
            //cmd.Parameters.AddWithValue("@checkoutdate",checkoutDate.Value.ToString("yyyy-M-d"));
             */
            String baruString = "";

            reader = cmd.ExecuteReader();
            Kamar = new Button[jumKamar];
            x = 0;
            while (reader.Read())
            {
                Kamar[x] = new Button();
                Kamar[x].Text = reader.GetInt32(0).ToString();
                Kamar[x].Name = reader.GetInt32(0).ToString();
                Kamar[x].Visible = true;
                Kamar[x].Height = 35;
                Kamar[x].Tag = reader.GetValue(1).ToString();
                //Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                try
                {
                    Kamar[x].BackColor = Color.FromArgb(Int32.Parse(reader.GetString(2)));
                }
                catch
                {
                    Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                }
                Kamar[x].Click += new EventHandler(UbahStatus);
                //Kamar[x].MouseEnter += new EventHandler(button1_MouseEnter_2);
                //Kamar[x].MouseLeave += new EventHandler(button1_MouseLeave_1);
                Kamar[x].Height = 45;
                Kamar[x].Width = 95;
                Kamar[x].FlatStyle = FlatStyle.Flat;
                if (reader.GetValue(1).ToString().Equals("1"))
                {
                    Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.housekeeping_2;
                }
                else if (reader.GetValue(1).ToString().Equals("2"))
                {
                    Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.maintenance;
                }
                else
                {
                    Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
                }
                
                Kamar[x].ImageAlign = btnBooking.ImageAlign;
                if (baruString.Equals(""))
                {
                    baruString = Kamar[x].Name.ToString().Substring(0, 1);
                }
                if (!Kamar[x].Name.ToString().Substring(0, 1).Equals(baruString))
                {
                    baruString = Kamar[x].Name.ToString().Substring(0, 1);
                    LinkLabel label1 = new LinkLabel();
                    label1.AutoSize = false;
                    label1.Height = 20;
                    label1.Width = PanelPesan.Width;
                    label1.BorderStyle = BorderStyle.Fixed3D;

                    panelKamar.Controls.Add(label1);
                }
                panelKamar.Controls.Add(Kamar[x]);
                x += 1;
                //Kamar[x].MouseEnter += button1_MouseEnter_2;// Kamar_Tips;//new EventHandler(Kamar_Tips);

            }
            //conn.Close();
            koneksi.closeConnection();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            resetBtnKonfigurasi();
            resetBtnLaporan();
            refreshActivatedButton();
            btnKamarMaintenance.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnKamarMaintenance.FlatAppearance.BorderSize = 2;

            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            hideBookingElement();
            setTampilanKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
        }

//        private void button1_Click(object sender, EventArgs e)
//        {
//          }
        private void UbahStatus(object sender, EventArgs e)
        {
            houseKeepingToolStripMenuItem.Enabled = true;
            maintenanceToolStripMenuItem.Enabled = true;
            availableToolStripMenuItem.Enabled = true;

            contextMenuStatusKamar.Show(Cursor.Position);
            Button lbl = sender as Button; 
            if(lbl.Tag.Equals("1")){
                houseKeepingToolStripMenuItem.Enabled = false;
            }
            else if (lbl.Tag.Equals("2"))
            {
                maintenanceToolStripMenuItem.Enabled = false;
            }
            else
            {
                availableToolStripMenuItem.Enabled = false;
            }
            kamarStatus = Int32.Parse(lbl.Text);
        }
        
        private void btn_historis_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            resetBtnKonfigurasi();
            refreshActivatedButton();
            btn_historis.FlatAppearance.BorderColor = Color.DodgerBlue;
            btn_historis.FlatAppearance.BorderSize = 2;
            
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            //gridView_historis.BringToFront();
            panelHistoris.BringToFront();

            DateTime lastDayOfMonth = new DateTime(
            DateTime.Now.Year,
            DateTime.Now.Month,
            DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            filter_checkout.Value = lastDayOfMonth;

            filter_checkin.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            input_tamu_historis.Text = "";
            reload_dataHistoris("", filter_checkin.Value, filter_checkout.Value);

        }
        
        private void reload_dataHistoris(string text_cari, DateTime checkin, DateTime checkout)
        {
            uniqueBooking_id.Clear();
            gridView_historis.Columns.Clear();

            SqlDataAdapter da;

            da = new SqlDataAdapter("select b.booking_id, r.reservasi_id, t.tamu, r.kamar_no, r.checkin, r.checkout,b.statusbayar, r.status from booking b, reservasi r, tamu t where b.booking_id=r.booking_id and b.tamu_id=t.tamu_id and t.tamu like '%" + text_cari + "%' and (convert(date,r.checkin)>='" + checkin.ToString("yyyy-MM-dd") + "' and convert(date,r.checkin)<='" + checkout.ToString("yyyy-MM-dd") + "' or convert(date,r.checkout)>='" + checkin.ToString("yyyy-MM-dd") + "' and convert(date,r.checkout)<='" + checkout.ToString("yyyy-MM-dd") + "') order by b.booking_id desc", koneksi.KoneksiDB());
            DataTable ds = new DataTable();
            da.Fill(ds);
            gridView_historis.DataSource = ds;
            koneksi.closeConnection();


            
            //gridView_historis.Columns[0].Visible = false;
            //gridView_historis.Columns[1].Visible = false;
            gridView_historis.Columns[6].Visible = false;
            gridView_historis.Columns[7].Visible = false;
            
            var buttonCol = new DataGridViewButtonColumn();
            buttonCol.Name = "InvoiceBooking";
            buttonCol.HeaderText = "";
            buttonCol.Text = "Invoice Booking";
            buttonCol.DefaultCellStyle.SelectionBackColor = Color.Gray;
            buttonCol.DefaultCellStyle.Font = btn_historis.Font;
            buttonCol.FlatStyle = btn_historis.FlatStyle;
            gridView_historis.Columns.Add(buttonCol);

            gridView_historis.Columns[8].DisplayIndex = 0;

            buttonCol = new DataGridViewButtonColumn();
            buttonCol.Name = "InvoiceReservasi";
            buttonCol.HeaderText = "";
            buttonCol.Text = "Invoice Reservasi";
            buttonCol.DefaultCellStyle.SelectionBackColor = Color.Gray;
            buttonCol.DefaultCellStyle.Font = btn_historis.Font;
            buttonCol.FlatStyle = btn_historis.FlatStyle;
            gridView_historis.Columns.Add(buttonCol);
            gridView_historis.Columns[9].DisplayIndex = 1;


            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "Chk";
            //dgvCmb.ReadOnly = false;
            dgvCmb.Width = 50;
            dgvCmb.HeaderText = "Gabung";
            gridView_historis.Columns.Add(dgvCmb);


            buttonCol = new DataGridViewButtonColumn();
            buttonCol.Name = "UbahBooking";
            buttonCol.HeaderText = "";
            buttonCol.Text = "Ubah Booking";
            buttonCol.DefaultCellStyle.SelectionBackColor = Color.Gray;
            buttonCol.DefaultCellStyle.Font = btn_historis.Font;
            buttonCol.FlatStyle = btn_historis.FlatStyle;
            gridView_historis.Columns.Add(buttonCol);


            foreach (DataGridViewRow row in gridView_historis.Rows)
            {
                row.Cells["InvoiceBooking"].Value = "Invoice Booking";
                row.Cells["InvoiceReservasi"].Value = "Invoice Reservasi";
                if (row.Cells[6].Value.ToString().Equals("1"))
                {
                    row.Cells["InvoiceReservasi"].Value = "-";
                }

                row.Cells["UbahBooking"].Value = "Ubah Booking";
                if (row.Cells[7].Value.ToString().Equals("checkout") || row.Cells[7].Value.ToString().Equals("cancel"))
                {
                    row.Cells["UbahBooking"].Value = "-";
                }
            }
            

            gridView_historis.ReadOnly = true;
            //gridView_historis.Columns[2].ReadOnly = true;
            //gridView_historis.Columns[3].ReadOnly = true;
            //gridView_historis.Columns[4].ReadOnly = true;
            //gridView_historis.Columns[5].ReadOnly = true;

            gridView_historis.Columns[3].Width = 40;

            gridView_historis.Columns[0].DisplayIndex = 11;
            gridView_historis.Columns[1].DisplayIndex = 11;
        }

        HashSet<int> uniqueBooking_id = new HashSet<int>();

        private void gridView_historis_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridView_historis.Columns[e.ColumnIndex].Name == "InvoiceBooking")
            {
                if (gridView_historis.SelectedCells.Count > 0)
                {
                    int selectedrowindex = gridView_historis.SelectedCells[0].RowIndex;

                    DataGridViewRow selectedRow = gridView_historis.Rows[selectedrowindex];

                    string a = Convert.ToString(selectedRow.Cells["booking_id"].Value);

                    /*
                    List<Microsoft.Reporting.WinForms.ReportParameter> parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
                    reportInvoice.ServerReport.SetParameters(parameter_reset);
                    List<Microsoft.Reporting.WinForms.ReportParameter> parameters = new List<Microsoft.Reporting.WinForms.ReportParameter>();
                    Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("booking_id", Convert.ToString(selectedRow.Cells["booking_id"].Value));
                    parameters.Add(param);
                    reportInvoice.ServerReport.ReportPath = "/Invoice/Invoice_Booking";
                    reportInvoice.ServerReport.SetParameters(parameters);
                    reportInvoice.RefreshReport();
                    reportInvoice.BringToFront();
                    reportInvoice.ServerReport.SetParameters(parameter_reset);
                    */

                    this.infoBooking.EnforceConstraints = false;

                    this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
                    this.newSelectLastPaymentTableAdapter.Fill(this.lastPayment.NewSelectLastPayment,Int32.Parse(Convert.ToString(selectedRow.Cells["booking_id"].Value)), null);
                    this.TamuTableAdapter.Fill(this.infoBooking.Tamu, Int32.Parse(Convert.ToString(selectedRow.Cells["booking_id"].Value)));
                    this.NewSelectCommandTableAdapter.Fill(this.infoReservasi.NewSelectCommand, Int32.Parse(Convert.ToString(selectedRow.Cells["booking_id"].Value)), null);
                    this.newSelectCommandTableAdapter1.Fill(this.pemesanan.NewSelectCommand, Int32.Parse(Convert.ToString(selectedRow.Cells["booking_id"].Value)), null);
                    this.NewSelectPembayaranTableAdapter.Fill(this.infoPembayaran.NewSelectPembayaran, Int32.Parse(Convert.ToString(selectedRow.Cells["booking_id"].Value)), null);
                    reportInvoice.LocalReport.EnableExternalImages = true;
                    string imagePath = "file://"+Directory.GetCurrentDirectory()+"\\gambar\\LogoC.png";
                    ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                    reportInvoice.LocalReport.SetParameters(parameter);
                    string imagePath2 = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(selectedRow.Cells["booking_id"].Value)+".png";
                    if (!File.Exists(Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(selectedRow.Cells["booking_id"].Value) + ".png"))
                    {
                        imagePath2 = "NULL";
                    }
                    ReportParameter parameter2 = new ReportParameter("ttd", imagePath2);
                    reportInvoice.LocalReport.SetParameters(parameter2);
    
                    reportInvoice.RefreshReport();
                    reportInvoice.BringToFront();
            

                }
            }
            else if (gridView_historis.Columns[e.ColumnIndex].Name == "InvoiceReservasi")
            {
                if (gridView_historis.SelectedCells.Count > 0 && gridView_historis.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Invoice Reservasi")
                {
                    int selectedrowindex = gridView_historis.SelectedCells[0].RowIndex;

                    DataGridViewRow selectedRow = gridView_historis.Rows[selectedrowindex];

                    string a = Convert.ToString(selectedRow.Cells["kamar_no"].Value);
                    /*List<Microsoft.Reporting.WinForms.ReportParameter> parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
                    List<Microsoft.Reporting.WinForms.ReportParameter> parameters = new List<Microsoft.Reporting.WinForms.ReportParameter>();
                    Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("booking_id", Convert.ToString(selectedRow.Cells["booking_id"].Value));
                    parameters.Add(param);
                    Microsoft.Reporting.WinForms.ReportParameter param2 = new Microsoft.Reporting.WinForms.ReportParameter("room", Convert.ToString(selectedRow.Cells["kamar_no"].Value));
                    parameters.Add(param2);
                    reportInvoice.ServerReport.ReportPath = "/Invoice/Invoice_Booking";
                    reportInvoice.ServerReport.SetParameters(parameters);
                    //reportInvoice.ServerReport.Refresh();
                    reportInvoice.RefreshReport();
                    reportInvoice.BringToFront();
                    reportInvoice.ServerReport.SetParameters(parameter_reset);*/

                    this.infoBooking.EnforceConstraints = false;
                    
                    this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
                    this.newSelectLastPaymentTableAdapter.Fill(this.lastPayment.NewSelectLastPayment,Int32.Parse(Convert.ToString(selectedRow.Cells["booking_id"].Value)), Int32.Parse(Convert.ToString(selectedRow.Cells["kamar_no"].Value)));
                    this.TamuTableAdapter.Fill(this.infoBooking.Tamu, Int32.Parse(Convert.ToString(selectedRow.Cells["booking_id"].Value)));
                    this.NewSelectCommandTableAdapter.Fill(this.infoReservasi.NewSelectCommand, Int32.Parse(Convert.ToString(selectedRow.Cells["booking_id"].Value)), Int32.Parse(Convert.ToString(selectedRow.Cells["kamar_no"].Value)));
                    this.newSelectCommandTableAdapter1.Fill(this.pemesanan.NewSelectCommand, Int32.Parse(Convert.ToString(selectedRow.Cells["booking_id"].Value)), Int32.Parse(Convert.ToString(selectedRow.Cells["kamar_no"].Value)));
                    this.NewSelectPembayaranTableAdapter.Fill(this.infoPembayaran.NewSelectPembayaran, Int32.Parse(Convert.ToString(selectedRow.Cells["booking_id"].Value)), Int32.Parse(Convert.ToString(selectedRow.Cells["kamar_no"].Value)));
                    reportInvoice.LocalReport.EnableExternalImages = true;
                    string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
                    ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                    reportInvoice.LocalReport.SetParameters(parameter);
                    string imagePath2 = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(selectedRow.Cells["booking_id"].Value) + ".png";
                    if (!File.Exists(Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(selectedRow.Cells["booking_id"].Value) + ".png"))
                    {
                        imagePath2 = "NULL";
                    }
                    ReportParameter parameter2 = new ReportParameter("ttd", imagePath2);
                    reportInvoice.LocalReport.SetParameters(parameter2);
    
                    reportInvoice.RefreshReport();
                    reportInvoice.BringToFront();
                }
            }
            else if (gridView_historis.Columns[e.ColumnIndex].Name == "Chk")
            {
                if (gridView_historis.SelectedCells.Count > 0)
                {
                    int selectedrowindex = gridView_historis.SelectedCells[0].RowIndex;

                    DataGridViewRow selectedRow = gridView_historis.Rows[selectedrowindex];

                    string a = Convert.ToString(selectedRow.Cells["booking_id"].Value);
                    //MessageBox.Show(a);
                    if (Convert.ToBoolean(selectedRow.Cells["Chk"].Value))
                    {
                        uniqueBooking_id.Remove(Convert.ToInt32(selectedRow.Cells["booking_id"].Value));
                    }
                    else
                    {
                        uniqueBooking_id.Add(Convert.ToInt32(selectedRow.Cells["booking_id"].Value));
                    }

                    foreach (DataGridViewRow row in gridView_historis.Rows)
                    {
                        //foreach (DataGridViewCell cell in row.Cells)
                        //{
                            if(Convert.ToString(row.Cells["booking_id"].Value) == a){
                                row.Cells["Chk"].Value = Convert.ToBoolean(row.Cells["Chk"].Value)==true?false:true;
                                //Console.WriteLine("a");
                            }
                            /*if (cell.ColumnIndex == 8) //Checkbox historis
                            {
                                if (Convert.ToBoolean(cell.Value) == true)
                                {
                                    Console.WriteLine(row.Cells[0].Value);
                                }
                            }*/
                        //}
                    }
                }
            }
            else if (gridView_historis.Columns[e.ColumnIndex].Name == "UbahBooking")
            {
                if (gridView_historis.SelectedCells.Count > 0 && gridView_historis.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Ubah Booking")
                {
                    
                    int selectedrowindex = gridView_historis.SelectedCells[0].RowIndex;
                    DataGridViewRow selectedRow = gridView_historis.Rows[selectedrowindex];

                    //MessageBox.Show(selectedRow.Cells["status"].Value.ToString());

                    if(selectedRow.Cells["status"].Value.ToString().Equals("checkin")){
                        //MessageBox.Show("In");
                        panelCariKamarTambahReservasi.Visible = false;
                        panelTambahReservasiCheckIn.BringToFront();

                        tambahReservasi_checkin.MinDate = DateTime.Today.AddDays(-1);
                        tambahreservasi_checkout.MinDate = DateTime.Today.AddDays(1);
                        tambahreservasi_checkout.Value = tambahReservasi_checkin.Value.AddDays(1);

                        string bookingKamar = selectedRow.Cells["booking_id"].Value.ToString();

                        SqlCommand sqlq = new SqlCommand("select b.tgl_booking, t.tamu_id, t.tamu, b.booking_diskon_id from booking b inner join tamu t on t.tamu_id=b.tamu_id where b.booking_id=@bokid", koneksi.KoneksiDB());
                        sqlq.Parameters.AddWithValue("@bokid", bookingKamar);
                        reader = sqlq.ExecuteReader();
                        while (reader.Read())
                        {
                            tamuCheckIn.Text = reader.GetString(2);
                            tgglBookingCheckIn.Text = Convert.ToDateTime(reader.GetValue(0)).ToString("dd/MM/yyyy HH:mm:ss");
                            if (reader.GetValue(3).ToString() == "1")
                            {
                                bookingCorporateCheckIn.Checked = true;
                            }
                            else
                            {
                                bookingCorporateCheckIn.Checked = false;
                            }
                        }
                        koneksi.closeConnection();
                        bookingIdCheckIn.Text = bookingKamar;

                        refreshDataGridView_datareservasi(bookingKamar);

                    }
                    else if(selectedRow.Cells["status"].Value.ToString().Equals("booking"))
                    {
                        //MessageBox.Show("Book");                       
                        string bookingKamar = selectedRow.Cells["booking_id"].Value.ToString();

                        SqlCommand sqlq = new SqlCommand("select b.tgl_booking, t.tamu_id, t.tamu, b.booking_diskon_id from booking b inner join tamu t on t.tamu_id=b.tamu_id where b.booking_id=@bokid", koneksi.KoneksiDB());
                        sqlq.Parameters.AddWithValue("@bokid", bookingKamar);
                        reader = sqlq.ExecuteReader();
                        while (reader.Read())
                        {
                            update_namaTamu.Text = reader.GetString(2);
                            update_tanggalBooking.Text = Convert.ToDateTime(reader.GetValue(0)).ToString("dd/MM/yyyy HH:mm:ss");
                            if (reader.GetValue(3).ToString() == "1")
                            {
                                update_bookingDiskon.Checked = true;
                            }
                            else
                            {
                                update_bookingDiskon.Checked = false;
                            }
                        }
                        koneksi.closeConnection();

                        //MessageBox.Show(bookingKamar);
                        update_bookingId.Text = bookingKamar;
                        refreshGridViewDataUpdateBooking(bookingKamar);
                        panelUpdateBooking.BringToFront();
                        panelUpdateReservasi.Visible = false;
                        panelCariKamarUpdateReservasi.Visible = false;
                        panelTambahReservasi.Visible = false;
                    }

                }
            }
        }

        private void btnPengaturanHotel_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            refreshActivatedButton();
            btnPengaturanHotel.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnPengaturanHotel.FlatAppearance.BorderSize = 2;
            
            
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            cmd = new SqlCommand("select * from IDHotel", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            reader.Read();
            inputNamaHotel.Text = reader.GetString(0);
            //nama_hotel = reader.GetString(0);
            inputAlamatHotel.Text = reader.GetString(1);
            inputTeleponHotel.Text = reader.GetString(2);
            inputKotaHotel.Text = reader.GetString(3);
            input_jamcheckout.Text = reader.GetValue(4).ToString();

            input_logout1.Text = reader.GetValue(6).ToString();
            input_logout2.Text = reader.GetValue(7).ToString();
            input_logout3.Text = reader.GetValue(8).ToString();
            input_logout4.Text = reader.GetValue(9).ToString();
            cb_bahasa.Text = reader.GetValue(10).ToString();
            cbtndatanganyesno.Text = reader.GetValue(11).ToString();
            koneksi.closeConnection();
            panelPengaturanHotel.BringToFront();


            logoHotelShow.ImageLocation = Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            openFD.FileName = "";

            cmd = new SqlCommand("select 100-harga*100 from booking_diskon where booking_diskon_id=1",koneksi.KoneksiDB());
            BookingCorporate.Text = cmd.ExecuteScalar().ToString();
            koneksi.closeConnection();
        }

        private void btnUpdateInfoHotel_Click(object sender, EventArgs e)
        {
            //using (SqlCommand dataCommand = koneksi.KoneksiDB().CreateCommand())
            //{
            //    //koneksi.KoneksiDB().Open();

            //    dataCommand.CommandText = "update IDHotel set Nama_Hotel = '" + inputNamaHotel.Text +
            //                                             "', Alamat = '" + inputAlamatHotel.Text +
            //                                             "', Telepon = '" + inputTeleponHotel.Text +
            //                                             "', Kota = '" + inputKotaHotel.Text +
            //                                          "' where Nama_Hotel = '" + inputNamaHotel.Text + "';";

            //    //dataCommand.Parameters.AddWithValue("@val1", Convert.ToInt32(textBox1.Text));
            //    //dataCommand.Parameters.AddWithValue("@param2", kamarTipeID);
            //    //dataCommand.Parameters.AddWithValue("@param3", kamarKapasitasID);


            //    dataCommand.ExecuteNonQuery();
            //    koneksi.closeConnection();
            //    MessageBox.Show("Data ID Hotel telah diubah");
            //    this.Close();
            //}


            SqlCommand cmd = new SqlCommand((@"update IDHotel set Nama_Hotel = '" + inputNamaHotel.Text +
                                                         "', Alamat = '" + inputAlamatHotel.Text +
                                                         "', Telepon = '" + inputTeleponHotel.Text +
                                                         "', Kota = '" + inputKotaHotel.Text +
                                                         "', Jam_checkout = '" + input_jamcheckout.Text +
                                                         "', Jam_logout1 = '" + input_logout1.Text +
                                                         "', Jam_logout2 = '" + input_logout2.Text +
                                                         "', Jam_logout3 = '" + input_logout3.Text +
                                                         "', Jam_logout4 = '" + input_logout4.Text +
                                                         "', bahasa = '" + cb_bahasa.Text +
                                                         "', tandatangan = '" +cbtndatanganyesno.Text +
                                                      //"' where Nama_Hotel = '" + inputNamaHotel.Text + 
                                                      "';"), koneksi.KoneksiDB());
            try
            {
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                cmd = new SqlCommand("update booking_diskon set harga=@hrg where booking_diskon_id=1", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@hrg", (100-Math.Round(float.Parse(BookingCorporate.Text),2))/100);
                cmd.ExecuteNonQuery();

                koneksi.closeConnection();

                isiArrayJamLogout();

                if (!openFD.FileName.Equals(""))
                {
                    string fName = Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
                    if (File.Exists(fName))
                        File.Delete(fName);
                    System.IO.File.Copy(openFD.FileName, fName);
                }
                openFD.FileName = "";
                MessageBox.Show("Data ID Hotel telah diubah");

                SqlCommand lg = new SqlCommand("select distinct ISNULL(bahasa, 'En') from IDHotel", koneksi.KoneksiDB());
                lang = lg.ExecuteScalar().ToString();
                koneksi.closeConnection();

                if (lang == "En")
                {
                    btnBooking.Text = "Booking Registration";
                    btnKalender.Text = "Booking Schedule";
                    btnCheckInStatus.Text = "Occupied Room Management";
                    btnKamarMaintenance.Text = "Vacant Room Management";
                    btnDaftarTamu.Text = "Guest List";
                    btnRights.Text = "Rights";
                    btn_restoran.Text = "Restaurant";
                    btnPengaturanKamar.Text = "Room Configuration";
                    btn_historis.Text = "Invoice Archive";
                    btnPengaturanHotel.Text = "Hotel Configuration";
                    btnPengaturanHarga.Text = "Price Management";
                    btn_harga_khusus.Text = "Special Rate";
                    btnPeriodik.Text = "Full Rate";
                    btn_bookingHangus.Text = "Expiring Registration";
                    btn_pengaturan_item.Text = "Inventory Management";
                    btnLaporanKeuangan.Text = "Income Report";
                    btnRekapHariIni.Text = "Daily Income Report";
                    btnUtng.Text = "Pending Bill";
                    btn_pendapatanRestoran.Text = "Restaurant Income Report";
                    btnLaporanGrandTotal.Text = "Daily GrandTotal";
                }
                else if (lang == "Ind")
                {
                    btnBooking.Text = "Registrasi Booking";
                    btnKalender.Text = "Kalender Booking";
                    btnCheckInStatus.Text = "Kamar CheckIn";
                    btnKamarMaintenance.Text = "Kamar Kosong";
                    btnDaftarTamu.Text = "Daftar Tamu";
                    btnRights.Text = "Jabatan";
                    btn_restoran.Text = "Restoran";
                    btnPengaturanKamar.Text = "Pengaturan Kamar";
                    btn_historis.Text = "Daftar Invoice";
                    btnPengaturanHotel.Text = "Pengaturan Hotel";
                    btnPengaturanHarga.Text = "Pengaturan Harga";
                    btn_harga_khusus.Text = "Harga Khusus";
                    btnPeriodik.Text = "Harga Priodik";
                    btn_bookingHangus.Text = "Booking Hangus";
                    btn_pengaturan_item.Text = "Pengaturan Item";
                    btnLaporanKeuangan.Text = "Laporan Pendapatan";
                    btnRekapHariIni.Text = "Laporan Pendapatan Harian";
                    btnUtng.Text = "Utang";
                    btn_pendapatanRestoran.Text = "Laporan Pendapatan Restoran";
                    btnLaporanGrandTotal.Text = "GrandTotal Harian";
                }
            }
            catch
            {
                MessageBox.Show("Input data tidak valid!");
            }

            
        }

        string[] resIDLebih;
        private string cekHargaLebih(int idbook)
        {
            resIDLebih = new string[999];
            int nilaiUang = 0;
            string nilaiLebih = "";
            SqlCommand sqlq = new SqlCommand("select reservasi_id from Reservasi where booking_id=@a ", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@a", idbook.ToString());
            SqlDataReader readLebih = sqlq.ExecuteReader();
            int jumLebih = 0;
            while (readLebih.Read())
            {
                string idReserLebih = readLebih["reservasi_id"].ToString();
                SqlCommand querybayar = new SqlCommand("select downpayment,tag_kamar,tag_restoran from Reservasi where reservasi_id = @id", koneksi.KoneksiDB());
                querybayar.Parameters.AddWithValue("@id", idReserLebih);
                SqlDataReader readD = querybayar.ExecuteReader();
                int tagihankamar = 0;
                int tagihanresto = 0;
                int diskon = 0;
                while (readD.Read())
                {
                    tagihankamar = Int32.Parse(readD["tag_kamar"].ToString());
                    diskon = Int32.Parse(readD["downpayment"].ToString());
                    tagihanresto = Int32.Parse(readD["tag_restoran"].ToString());
                }
                
                SqlCommand sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
                sql1.Parameters.AddWithValue("@a", idbook);
                int kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
                
                sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
                sql1.Parameters.AddWithValue("@a", kodediskon);
                float potongan = float.Parse(sql1.ExecuteScalar().ToString());
                //MessageBox.Show(potongan.ToString());

                if (((int)((tagihankamar *potongan) / 100) - diskon + tagihanresto) < 0)
                {
                    nilaiUang += ((int)(tagihankamar *potongan) / 100) - diskon + tagihanresto;
                    resIDLebih[jumLebih] = idReserLebih;
                    jumLebih += 1;
                }
                
            }
            koneksi.closeConnection();
            nilaiUang *= -1;

            return nilaiUang.ToString();
        }
     
        int idReservasi = 0;
        int idBooking = 0;
        
        private void bayarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            txtJumUang.Text = "0";
            lblduidLebih.Visible = true;
            lblUseDP.Visible = true;
            txtuseDp.Visible = true;
            //MessageBox.Show(dataKamarCh.ToString()); 
            //string JumyangDibayar = Interaction.InputBox("Masukkan Jumlah Uang = ");
            panelPembayaran.Visible = true;
            panelPembayaran.BringToFront();
            cbPembayaranReser.Text = cbPembayaranReser.Items[0].ToString();
            lblTanggalBayar.Text = DateTime.Now.Date.ToString("dd-MMM-yyyy");

            SqlCommand querybayar = new SqlCommand("select reservasi_id, booking_id from Reservasi where kamar_no = @a and status='checkin'",koneksi.KoneksiDB());
            querybayar.Parameters.AddWithValue("@a", dataKamarCh);
            SqlDataReader readData = querybayar.ExecuteReader();
            while (readData.Read())
            {
                idReservasi = Convert.ToInt32(readData["reservasi_id"].ToString());
                idBooking = Convert.ToInt32(readData["booking_id"].ToString());
            }
            koneksi.closeConnection();

            //addX
            txtuseDp.Text = "0";
            string hargaLebihA = "";
            hargaLebihA = cekHargaLebih(idBooking);
            lblduidLebih.Text = "DownPayment Booking : (Rp." + hargaLebihA + ",00)";
            if (hargaLebihA.Equals("0"))
            {
                txtuseDp.Enabled = false;
            }
            else
            {
                txtuseDp.Enabled = true;
            }
            //endX

            querybayar = new SqlCommand("select downpayment,tag_kamar,tag_restoran from Reservasi where reservasi_id = @id", koneksi.KoneksiDB());
            querybayar.Parameters.AddWithValue("@id", idReservasi);
            SqlDataReader readD = querybayar.ExecuteReader();
            int tagihankamar=0;
            int tagihanresto = 0;
            int diskon=0;
            while(readD.Read()){
                tagihankamar=Int32.Parse(readD["tag_kamar"].ToString());
                diskon = Int32.Parse(readD["downpayment"].ToString());
                tagihanresto = Int32.Parse(readD["tag_restoran"].ToString());
            }
            koneksi.closeConnection();

            SqlCommand sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", idBooking);
            int kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", kodediskon);
            float potongan = float.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            int totalbiayakamar = (int)((tagihankamar * potongan) / 100) - diskon + tagihanresto;

            koneksi.closeConnection();
            if (totalbiayakamar < 0)
            {
                lblBiayaTag.Text = "Tagihan : Rp.0,00";
                lblBiayaTag.Tag = "0";
            }
            else
            {
                lblBiayaTag.Text = "Tagihan : Rp." + totalbiayakamar.ToString() + ",00" +"  Diskon"+ diskon.ToString() +"  Potongan"+ potongan.ToString();
                lblBiayaTag.Tag = totalbiayakamar.ToString();
            }
        }

        private void btnBayarReservasi_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(Login.idS.ToString());
            dataGridView3.Enabled = true;
            try
            {
                //if ((Int32.Parse(txtJumUang.Text) + Int32.Parse(txtuseDp.Text)) <= (Int32.Parse(lblBiayaTag.Tag.ToString()) + 1) || txtuseDp.Visible == false)
                //{
                    if (!txtuseDp.Text.Equals("0"))
                    {
                        SqlCommand sqlLbh = new SqlCommand("select payment_id,payment,nopayment,tggalpayment,staff_id from pembayaran where reservasi_id = @a and jumlahpayment = (select max(jumlahpayment) from pembayaran where reservasi_id = @a)", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", resIDLebih[0]);
                        SqlDataReader readDaLbh = sqlLbh.ExecuteReader();
                        string paymentLbh = ""; string paymentnamaLbh = ""; string nopaymentnamaLbh = "";
                        string tglpaymentnamaLbh = ""; string staffpaymentnamaLbh = "";
                        while (readDaLbh.Read())
                        {
                            paymentLbh = readDaLbh["payment_id"].ToString();
                            paymentnamaLbh = readDaLbh["payment"].ToString();
                            nopaymentnamaLbh = readDaLbh["nopayment"].ToString();
                            tglpaymentnamaLbh = readDaLbh["tggalpayment"].ToString();
                            staffpaymentnamaLbh = readDaLbh["staff_id"].ToString();

                        }
                        koneksi.closeConnection();
                        sqlLbh = new SqlCommand("update pembayaran set jumlahpayment=jumlahpayment-@a where payment_id=@b", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(txtuseDp.Text));
                        sqlLbh.Parameters.AddWithValue("@b", paymentLbh);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment-@a where reservasi_id=@b", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(txtuseDp.Text));
                        sqlLbh.Parameters.AddWithValue("@b", resIDLebih[0]);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment+@a where reservasi_id=@b", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(txtuseDp.Text));
                        sqlLbh.Parameters.AddWithValue("@b", idReservasi);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sqlLbh = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment,staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                        sqlLbh.Parameters.AddWithValue("@a", idBooking);
                        sqlLbh.Parameters.AddWithValue("@b", idReservasi);
                        sqlLbh.Parameters.AddWithValue("@c", paymentnamaLbh);
                        sqlLbh.Parameters.AddWithValue("@d", nopaymentnamaLbh);
                        sqlLbh.Parameters.AddWithValue("@e", txtuseDp.Text.ToString());
                        sqlLbh.Parameters.AddWithValue("@f", Convert.ToDateTime(tglpaymentnamaLbh));
                        sqlLbh.Parameters.AddWithValue("@g", staffpaymentnamaLbh);
                        sqlLbh.ExecuteNonQuery();
                        koneksi.closeConnection();

                    }

                    SqlCommand querybayar = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment,staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                    querybayar.Parameters.AddWithValue("@a", idBooking);
                    querybayar.Parameters.AddWithValue("@b", idReservasi);
                    querybayar.Parameters.AddWithValue("@c", cbPembayaranReser.Text);
                    querybayar.Parameters.AddWithValue("@d", txtCCbayarreser.Text);
                    querybayar.Parameters.AddWithValue("@e", txtJumUang.Text);
                    querybayar.Parameters.AddWithValue("@f", DateTime.Now);
                    querybayar.Parameters.AddWithValue("@g", Login.idS.ToString());
                    querybayar.ExecuteNonQuery();
                    koneksi.closeConnection();

                    querybayar = new SqlCommand("update Reservasi set downpayment= downpayment+@a where reservasi_id =@b", koneksi.KoneksiDB());
                    querybayar.Parameters.AddWithValue("@a", Int32.Parse(txtJumUang.Text));
                    querybayar.Parameters.AddWithValue("@b", idReservasi);
                    querybayar.ExecuteNonQuery();
                    koneksi.closeConnection();

                    querybayar = new SqlCommand("update Booking set balance_due=balance_due-@a where booking_id=@b", koneksi.KoneksiDB());
                    querybayar.Parameters.AddWithValue("@a", Int32.Parse(txtJumUang.Text));
                    querybayar.Parameters.AddWithValue("@b", idBooking);
                    querybayar.ExecuteNonQuery();
                    koneksi.closeConnection();

                    int row = 0;
                    foreach (DataGridViewRow rw in this.dataGridView3.Rows)
                    {
                        for (int i = 0; i < rw.Cells.Count; i++)
                        {
                            string[] ketKalender = Convert.ToString(rw.Cells[i].Value).Split('-');
                            if (Convert.ToString(rw.Cells[i].Value) != string.Empty && i > 0 && ketKalender[0] == "EA")
                            {
                                DateTime tanggalPesan1;
                                int NoKamarInfo = Int32.Parse(rw.Cells[0].Value.ToString());
                                tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[rw.Cells[i].ColumnIndex].Name.ToString());

                                SqlCommand sqlq = new SqlCommand("select max(Reservasi.reservasi_id) from Reservasi where convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking')", koneksi.KoneksiDB());
                                sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
                                sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
                                string reservasiKamar = "0";
                                reservasiKamar = sqlq.ExecuteScalar().ToString();
                                koneksi.closeConnection();

                                sqlq = new SqlCommand("select sum(jumlahpayment) from pembayaran where reservasi_id=@r_id", koneksi.KoneksiDB());
                                sqlq.Parameters.AddWithValue("r_id", reservasiKamar);
                                int jmlhPayment = 0;
                                try
                                {
                                    jmlhPayment = Int32.Parse(sqlq.ExecuteScalar().ToString());
                                }catch
                                { }
                                koneksi.closeConnection();

                                if(jmlhPayment > 1){
                                    dataGridView3.Rows[row].Cells[i].Style.BackColor = Color.Pink;
                                }
                                else
                                {
                                    dataGridView3.Rows[row].Cells[i].Style.BackColor = Color.Red;
                                }
                            }
                            /*if (Convert.ToString(rw.Cells[i].Value) != string.Empty && i > 0 && ketKalender[0] == "OC")
                            {
                                dataGridView3.Rows[row].Cells[i].Style.BackColor = Color.Green;
                            }
                            if (Convert.ToString(rw.Cells[i].Value) != string.Empty && i > 0 && ketKalender[0] == "VD")
                            {
                                dataGridView3.Rows[row].Cells[i].Style.BackColor = Color.Yellow;
                            }*/
                        } row++;
                    }

                    panelPembayaran.Visible = false;
                //}
                //else
                //{
                //    MessageBox.Show("Pembayaran tidak bisa berlebih! Silakan alokasikan ke kamar lain");
                //}

                //Check booking hangus
                SqlCommand sql = new SqlCommand("Select b.booking_id from reservasi r inner join booking b on r.booking_id=b.booking_id where r.status='booking' and (datediff(minute,b.tgl_booking,SYSDATETIME())>180) group by b.booking_id having SUM(r.downpayment)<=0", koneksi.KoneksiDB());
                int bookinghangus = 0;
                reader = sql.ExecuteReader();
                while (reader.Read())
                {
                    bookinghangus++;
                }

                if (bookinghangus > 0)
                {
                    booking_notif = true;
                }
                else
                {
                    booking_notif = false;
                }
                koneksi.closeConnection();
                refresh_panelBookingHangus();
                txtuseDp.Text = "0";
                txtJumUang.Text = "0";

            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }

        }

        private void checkOutBookingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlCommand queryData = new SqlCommand("select booking_id from Reservasi where status='checkin' and kamar_no = @a", koneksi.KoneksiDB());
            queryData.Parameters.AddWithValue("@a", dataKamarCh);
            int idbookingData = Int32.Parse(queryData.ExecuteScalar().ToString());
            koneksi.closeConnection();

            string hargalebihReser = "";
            hargalebihReser = cekHargaLebih(idbookingData);

            queryData = new SqlCommand("update Reservasi set checkout=@tggal where booking_id =@a and status='checkin'", koneksi.KoneksiDB());
            queryData.Parameters.AddWithValue("@a", idbookingData);
            queryData.Parameters.AddWithValue("@tggal", DateTime.Now);
            queryData.ExecuteNonQuery();
            koneksi.closeConnection();

            SqlCommand sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", idbookingData);
            int kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", kodediskon);
            float potongan = float.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();


            queryData = new SqlCommand("select reservasi_id, tag_kamar, tag_restoran, downpayment from Reservasi where booking_id=@a and status<>'checkout' ", koneksi.KoneksiDB());
            queryData.Parameters.AddWithValue("@a", idbookingData);
            SqlDataReader readKumpulData = queryData.ExecuteReader();
            int[] reservasichid = new int[90];
            int[] tagKamarchid = new int[90];
            int[] tagrestoranchid = new int[90];
            int[] downpaymentchid = new int[90];
            int indexChid = 0;
            while (readKumpulData.Read())
            {
                reservasichid[indexChid] = Int32.Parse(readKumpulData["reservasi_id"].ToString());
                tagKamarchid[indexChid] = (int)(Int32.Parse(readKumpulData["tag_kamar"].ToString()) * potongan) / 100;
                tagrestoranchid[indexChid] = Int32.Parse(readKumpulData["tag_restoran"].ToString());
                downpaymentchid[indexChid] = Int32.Parse(readKumpulData["downpayment"].ToString());
                indexChid += 1;
            }
            koneksi.closeConnection();

            for (int i = 0; i < indexChid; i++)
            {
                if (Convert.ToInt32(hargalebihReser) > 0)
                {
                    SqlCommand sqlLbh = new SqlCommand("select payment_id,payment,nopayment,tggalpayment,staff_id from pembayaran where reservasi_id = @a and jumlahpayment = (select max(jumlahpayment) from pembayaran where reservasi_id = @a)", koneksi.KoneksiDB());
                    sqlLbh.Parameters.AddWithValue("@a", resIDLebih[0]);
                    SqlDataReader readDaLbh = sqlLbh.ExecuteReader();
                    string paymentLbh = ""; string paymentnamaLbh = ""; string nopaymentnamaLbh = "";
                    string tglpaymentnamaLbh = ""; string staffpaymentnamaLbh = "";
                    while (readDaLbh.Read())
                    {
                        paymentLbh = readDaLbh["payment_id"].ToString();
                        paymentnamaLbh = readDaLbh["payment"].ToString();
                        nopaymentnamaLbh = readDaLbh["nopayment"].ToString();
                        tglpaymentnamaLbh = readDaLbh["tggalpayment"].ToString();
                        staffpaymentnamaLbh = readDaLbh["staff_id"].ToString();

                    }
                    koneksi.closeConnection();

                    if ((tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]) > 0)
                    {
                        if (Convert.ToInt32(hargalebihReser) - (tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]) > 0)
                        {
                            sqlLbh = new SqlCommand("update pembayaran set jumlahpayment=jumlahpayment-@a where payment_id=@b", koneksi.KoneksiDB());
                            sqlLbh.Parameters.AddWithValue("@a", (tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]));
                            sqlLbh.Parameters.AddWithValue("@b", paymentLbh);
                            sqlLbh.ExecuteNonQuery();
                            koneksi.closeConnection();

                            sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment-@a where reservasi_id=@b", koneksi.KoneksiDB());
                            sqlLbh.Parameters.AddWithValue("@a", (tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]));
                            sqlLbh.Parameters.AddWithValue("@b", resIDLebih[0]);
                            sqlLbh.ExecuteNonQuery();
                            koneksi.closeConnection();

                            sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment+@a where reservasi_id=@b", koneksi.KoneksiDB());
                            sqlLbh.Parameters.AddWithValue("@a", (tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]));
                            sqlLbh.Parameters.AddWithValue("@b", reservasichid[i]);
                            sqlLbh.ExecuteNonQuery();
                            koneksi.closeConnection();

                            sqlLbh = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment,staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                            sqlLbh.Parameters.AddWithValue("@a", idbookingData);
                            sqlLbh.Parameters.AddWithValue("@b", reservasichid[i]);
                            sqlLbh.Parameters.AddWithValue("@c", paymentnamaLbh);
                            sqlLbh.Parameters.AddWithValue("@d", nopaymentnamaLbh);
                            sqlLbh.Parameters.AddWithValue("@e", (tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]));
                            sqlLbh.Parameters.AddWithValue("@f", tglpaymentnamaLbh);
                            sqlLbh.Parameters.AddWithValue("@g", staffpaymentnamaLbh);
                            sqlLbh.ExecuteNonQuery();
                            koneksi.closeConnection();
                            hargalebihReser = (Convert.ToInt32(hargalebihReser) - (tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i])).ToString();
                        }
                        else
                        {
                            sqlLbh = new SqlCommand("update pembayaran set jumlahpayment=jumlahpayment-@a where payment_id=@b", koneksi.KoneksiDB());
                            sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(hargalebihReser));
                            sqlLbh.Parameters.AddWithValue("@b", paymentLbh);
                            sqlLbh.ExecuteNonQuery();
                            koneksi.closeConnection();

                            sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment-@a where reservasi_id=@b", koneksi.KoneksiDB());
                            sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(hargalebihReser));
                            sqlLbh.Parameters.AddWithValue("@b", resIDLebih[0]);
                            sqlLbh.ExecuteNonQuery();
                            koneksi.closeConnection();

                            sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment+@a where reservasi_id=@b", koneksi.KoneksiDB());
                            sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(hargalebihReser));
                            sqlLbh.Parameters.AddWithValue("@b", reservasichid[i]);
                            sqlLbh.ExecuteNonQuery();
                            koneksi.closeConnection();

                            sqlLbh = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment,staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                            sqlLbh.Parameters.AddWithValue("@a", idbookingData);
                            sqlLbh.Parameters.AddWithValue("@b", reservasichid[i]);
                            sqlLbh.Parameters.AddWithValue("@c", paymentnamaLbh);
                            sqlLbh.Parameters.AddWithValue("@d", nopaymentnamaLbh);
                            sqlLbh.Parameters.AddWithValue("@e", Convert.ToInt32(hargalebihReser));
                            sqlLbh.Parameters.AddWithValue("@f", tglpaymentnamaLbh);
                            sqlLbh.Parameters.AddWithValue("@g", staffpaymentnamaLbh);
                            sqlLbh.ExecuteNonQuery();
                            koneksi.closeConnection();

                            SqlCommand querybayar = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment, staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                            querybayar.Parameters.AddWithValue("@a", idbookingData);
                            querybayar.Parameters.AddWithValue("@b", reservasichid[i]);
                            querybayar.Parameters.AddWithValue("@c", "Kontan");
                            querybayar.Parameters.AddWithValue("@d", "");
                            querybayar.Parameters.AddWithValue("@e", (tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]) - Convert.ToInt32(hargalebihReser));
                            querybayar.Parameters.AddWithValue("@f", DateTime.Now);
                            querybayar.Parameters.AddWithValue("@g", Login.idS.ToString());
                            querybayar.ExecuteNonQuery();
                            koneksi.closeConnection();

                            querybayar = new SqlCommand("update Reservasi set downpayment = downpayment + @a where reservasi_id = @b", koneksi.KoneksiDB());
                            querybayar.Parameters.AddWithValue("@a", (tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]) - Convert.ToInt32(hargalebihReser));
                            querybayar.Parameters.AddWithValue("@b", reservasichid[i]);
                            querybayar.ExecuteNonQuery();
                            koneksi.closeConnection();
                            hargalebihReser = (Convert.ToInt32(hargalebihReser) - Convert.ToInt32(hargalebihReser)).ToString();

                        }

                    }
                }
                else
                {
                    SqlCommand querybayar = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment, staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                    querybayar.Parameters.AddWithValue("@a", idbookingData);
                    querybayar.Parameters.AddWithValue("@b", reservasichid[i]);
                    querybayar.Parameters.AddWithValue("@c", "Kontan");
                    querybayar.Parameters.AddWithValue("@d", "");
                    querybayar.Parameters.AddWithValue("@e", tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]);
                    querybayar.Parameters.AddWithValue("@f", DateTime.Now);
                    querybayar.Parameters.AddWithValue("@g", Login.idS.ToString());
                    querybayar.ExecuteNonQuery();
                    koneksi.closeConnection();

                    querybayar = new SqlCommand("update Reservasi set downpayment = downpayment + @a where reservasi_id = @b", koneksi.KoneksiDB());
                    querybayar.Parameters.AddWithValue("@a", tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]);
                    querybayar.Parameters.AddWithValue("@b", reservasichid[i]);
                    querybayar.ExecuteNonQuery();
                    koneksi.closeConnection();
                }
            }


            /*
            List<Microsoft.Reporting.WinForms.ReportParameter> parameters = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("booking_id", idbookingData.ToString());
            parameters.Add(param);
            //Microsoft.Reporting.WinForms.ReportParameter param2 = new Microsoft.Reporting.WinForms.ReportParameter("room", dataKamarCh.ToString());
            //list.Add(param2);
            reportInvoice.ServerReport.ReportPath = "/Invoice/Invoice_Booking";
            reportInvoice.ServerReport.SetParameters(parameters);
            //reportInvoice.ServerReport.Refresh();
            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
            List<Microsoft.Reporting.WinForms.ReportParameter> parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            reportInvoice.ServerReport.SetParameters(parameter_reset);
            */

            queryData = new SqlCommand("update Booking set balance_due=0 where booking_id =@a", koneksi.KoneksiDB());
            queryData.Parameters.AddWithValue("@a", idbookingData);
            queryData.ExecuteNonQuery();
            koneksi.closeConnection();

            queryData = new SqlCommand("select kamar_no from Reservasi where booking_id=@a and status='checkin'", koneksi.KoneksiDB());
            queryData.Parameters.AddWithValue("@a", idbookingData);
            SqlDataReader readerData = queryData.ExecuteReader();
            ArrayList list = new ArrayList();
            while (readerData.Read())
            {
                list.Add(Int32.Parse(readerData["kamar_no"].ToString()));

            }
            koneksi.closeConnection();

            queryData = new SqlCommand("update Reservasi set status='checkout' where booking_id =@a and status='checkin'", koneksi.KoneksiDB());
            queryData.Parameters.AddWithValue("@a", idbookingData);
            queryData.ExecuteNonQuery();
            koneksi.closeConnection();

            foreach (int i in list)
            {
                SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@id", i);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

            }
            btnCheckInStatus_Click(sender, e);   
        
        }

        private void panelUser_Paint(object sender, PaintEventArgs e)
        {

        }

        private void batalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            /*SqlCommand batalQuery = new SqlCommand("select booking_id,tag_kamar, downpayment from Reservasi where kamar_no=@a and status='booking'", koneksi.KoneksiDB());
            batalQuery.Parameters.AddWithValue("@a", dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());
            SqlDataReader bacaQuery = batalQuery.ExecuteReader();
            int kodeBookingId = 0;
            int tagKamarBatal = 0;
            int downPaymentBatal = 0;

            while (bacaQuery.Read())
            {
                kodeBookingId = Int32.Parse(bacaQuery["booking_id"].ToString());
                tagKamarBatal = Int32.Parse(bacaQuery["tag_kamar"].ToString());
                downPaymentBatal = Int32.Parse(bacaQuery["downpayment"].ToString());
            }

            koneksi.closeConnection();

            batalQuery = new SqlCommand("update Reservasi set status='cancel', checkout=@b where kamar_no=@a and status='booking'", koneksi.KoneksiDB());
            batalQuery.Parameters.AddWithValue("@a", dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());
            batalQuery.Parameters.AddWithValue("@b", DateTime.Now);
            batalQuery.ExecuteNonQuery();
            koneksi.closeConnection();


            batalQuery = new SqlCommand("update Booking set balance_due=balance_due-@b where booking_id=@a ", koneksi.KoneksiDB());
            batalQuery.Parameters.AddWithValue("@a", kodeBookingId);
            batalQuery.Parameters.AddWithValue("@b", tagKamarBatal - downPaymentBatal);
            batalQuery.ExecuteNonQuery();
            koneksi.closeConnection();

            loadKalender(TglBulan, Tgltahun);
             */
        }

        private void btn_harga_khusus_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            refreshActivatedButton();
            btn_harga_khusus.FlatAppearance.BorderColor = Color.DodgerBlue;
            btn_harga_khusus.FlatAppearance.BorderSize = 2;
                        
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            SqlCommand sql = new SqlCommand("Select * from kamar_tipe", koneksi.KoneksiDB());

            combobox_kamar.Items.Clear();

            reader = sql.ExecuteReader();
            while (reader.Read())
            {
                ComboboxItem item = new ComboboxItem();
                item.Value = reader.GetValue(0).ToString();
                item.Text = reader.GetValue(1).ToString();
                combobox_kamar.Items.Add(item);
            }

            koneksi.closeConnection();

            panelHargaKhusus.BringToFront();
            hideBookingElement();
            refreshGridViewHargaKhusus();
        
        }

        private void btn_hargaKhususTambah_Click(object sender, EventArgs e)
        {
            try{
                ComboboxItem item = (ComboboxItem)combobox_kamar.SelectedItem;

                SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) jumlah
                    FROM Reservasi r inner join Kamar k on r.kamar_no=k.kamar_no
	                    --inner join Kamar_Tipe kt on k.kamar_tipe_id=kt.kamar_tipe_id
                    where (CONVERT(date,r.checkout)>=@tggl_berlaku 
		                    and CONVERT(date,r.checkin)<=@tggl_berakhir)
	                    and (r.status='booking' or r.status='checkin')
	                    and k.kamar_tipe_id=@kmr_tipe_id", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@tggl_berlaku", dateTime_tglBerlaku.Value.Date);
                cmd.Parameters.AddWithValue("@tggl_berakhir", dateTime_tglBerakhir.Value.Date);
                cmd.Parameters.AddWithValue("@kmr_tipe_id", item.Value);
                int totalR = Int32.Parse(cmd.ExecuteScalar().ToString());
                koneksi.closeConnection();

                if (totalR > 0)
                {
                    MessageBox.Show("Tidak dapat melakukan perubahan harga karena ada booking pada tipe kamar tersebut dan periode tersebut!");
                }
                else
                {

                    SqlCommand sql = new SqlCommand("insert into Harga_Khusus(kamar_tipe_id, tgl_berlaku, tgl_berakhir, harga, harga_weekend) values (@a,@b,@c,@d,@e)", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", item.Value);
                    sql.Parameters.Add("@b", SqlDbType.DateTime).Value = dateTime_tglBerlaku.Value.Date;
                    sql.Parameters.Add("@c", SqlDbType.DateTime).Value = dateTime_tglBerakhir.Value.Date;
                    sql.Parameters.AddWithValue("@d", input_hargaNormal.Text);
                    sql.Parameters.AddWithValue("@e", input_hargaWeekend.Text);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    refreshGridViewHargaKhusus();
                    setHarga();
                }
            
            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }

        }

        private void btn_hargaKhusuSimpan_Click(object sender, EventArgs e)
        {
            try{
                ComboboxItem item = (ComboboxItem)combobox_kamar.SelectedItem;

                SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) jumlah
                    FROM Reservasi r inner join Kamar k on r.kamar_no=k.kamar_no
	                    --inner join Kamar_Tipe kt on k.kamar_tipe_id=kt.kamar_tipe_id
                    where (CONVERT(date,r.checkout)>=@tggl_berlaku 
		                    and CONVERT(date,r.checkin)<=@tggl_berakhir)
	                    and (r.status='booking' or r.status='checkin')
	                    and k.kamar_tipe_id=@kmr_tipe_id", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@tggl_berlaku", dateTime_tglBerlaku.Value.Date);
                cmd.Parameters.AddWithValue("@tggl_berakhir", dateTime_tglBerakhir.Value.Date);
                cmd.Parameters.AddWithValue("@kmr_tipe_id", item.Value);
                int totalR = Int32.Parse(cmd.ExecuteScalar().ToString());
                koneksi.closeConnection();

                if (totalR > 0)
                {
                    MessageBox.Show("Tidak dapat melakukan perubahan harga karena ada booking pada tipe kamar tersebut dan periode tersebut! \n atau silakan batalkan booking tersebut.");
                }
                else
                {
                    SqlCommand sql = new SqlCommand("update Harga_Khusus set kamar_tipe_id=@a, tgl_berlaku=@b, tgl_berakhir=@c, harga=@d, harga_weekend=@e where harga_khusus_id=@f", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", item.Value);
                    sql.Parameters.Add("@b", SqlDbType.DateTime).Value = dateTime_tglBerlaku.Value.Date;
                    sql.Parameters.Add("@c", SqlDbType.DateTime).Value = dateTime_tglBerakhir.Value.Date;
                    sql.Parameters.AddWithValue("@d", input_hargaNormal.Text);
                    sql.Parameters.AddWithValue("@e", input_hargaWeekend.Text);
                    sql.Parameters.AddWithValue("@f", lbl_hargaKhususId.Text);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    refreshGridViewHargaKhusus();
                    setHarga();
                }
            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }
        
        }

        private void btn_hargaKhususHapus_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCommand sql = new SqlCommand("delete from Harga_Khusus where harga_khusus_id=@a", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", lbl_hargaKhususId.Text);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                refreshGridViewHargaKhusus();
                setHarga();
            
            }
            catch
            {
                MessageBox.Show("Silakan pilih data yang ingin dihapus!");
            }
        
        }

        private void dataGrid_hargaKhusus_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lbl_hargaKhususId.Text = dataGrid_hargaKhusus[0, e.RowIndex].Value.ToString();
            dateTime_tglBerlaku.Value = Convert.ToDateTime(dataGrid_hargaKhusus[2, e.RowIndex].Value);
            dateTime_tglBerakhir.Value = Convert.ToDateTime(dataGrid_hargaKhusus[3, e.RowIndex].Value);
            input_hargaNormal.Text = dataGrid_hargaKhusus[4, e.RowIndex].Value.ToString();
            input_hargaWeekend.Text = dataGrid_hargaKhusus[5, e.RowIndex].Value.ToString();
            combobox_kamar.Text = dataGrid_hargaKhusus[1, e.RowIndex].Value.ToString();
        }

        private void refreshGridViewHargaKhusus()
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter("select h.harga_khusus_id, k.kamar_tipe, h.tgl_berlaku, h.tgl_berakhir, h.harga, h.harga_weekend from harga_khusus h inner join kamar_tipe k on k.kamar_tipe_id=h.kamar_tipe_id", koneksi.KoneksiDB()); //c.con is the connection string
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            dataGrid_hargaKhusus.ReadOnly = true;
            dataGrid_hargaKhusus.DataSource = ds.Tables[0];
            koneksi.closeConnection();
            if (combobox_kamar.SelectedIndex >= 0) {
                ComboboxItem item = (ComboboxItem)combobox_kamar.SelectedItem;
                BindingSource bs = new BindingSource();
                bs.DataSource = dataGrid_hargaKhusus.DataSource;
                bs.Filter = "kamar_tipe like '%" + item.Text + "%'";
                dataGrid_hargaKhusus.DataSource = bs;
            }

            dataGrid_hargaKhusus.Columns[0].DisplayIndex = 5;
            
        }

        private void setHargaPeriodik()
        {
            SqlDataAdapter da = new SqlDataAdapter("select hp.harga_periodik_id, kt.kamar_tipe , hp.tgl_berlaku, hp.harga, hp.harga_weekend from Harga_Periodik hp inner join Kamar_Tipe kt on hp.kamar_tipe_id=kt.kamar_tipe_id where year(hp.tgl_berlaku)>2008", koneksi.KoneksiDB());
            dHargaPeriodik = new DataTable();
            da.Fill(dHargaPeriodik);
            dataGridView7.DataSource = dHargaPeriodik;

            dataGridView7.Columns[0].DisplayIndex = 4;

            koneksi.closeConnection();

        }

        DataTable dHargaPeriodik;
        int kodeEdit = 0;

        private void btnPeriodik_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            refreshActivatedButton();
            btnPeriodik.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnPeriodik.FlatAppearance.BorderSize = 2;
                        
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            hideBookingElement();
            cbJnsKamarPeriodik.Items.Clear();
            SqlCommand sql = new SqlCommand("select kamar_tipe_id,kamar_tipe from Kamar_Tipe", koneksi.KoneksiDB());
            SqlDataReader readKamar = sql.ExecuteReader();
            ComboboxItem item = new ComboboxItem();
            while (readKamar.Read())
            {
                item = new ComboboxItem();
                item.Text = readKamar["kamar_tipe"].ToString();
                item.Value = readKamar["kamar_tipe_id"].ToString();
                cbJnsKamarPeriodik.Items.Add(item);

            }

            setHargaPeriodik();
            //cbJnsKamarPeriodik.Text = cbJnsKamarPeriodik.Items[0].ToString();
            panelHargaPeriodik.BringToFront();

        }
        
        int item_id = 0;
        private void refreshGridViewDataItem()
        {
            SqlDataAdapter da = new SqlDataAdapter("select i.item_id, i.item, it.item_tipe, i.harga from item i inner join item_tipe it on i.item_tipe_id=it.item_tipe_id and it.item_tipe='"+ cb_tipeItem.Text +"' order by i.item", koneksi.KoneksiDB());
            
            DataTable dataItem = new DataTable();
            da.Fill(dataItem);
            GridView_dataItem.ReadOnly = true;
            GridView_dataItem.DataSource = dataItem;
            koneksi.closeConnection();
        }
        
        private void btn_pengaturan_item_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            refreshActivatedButton();
            btn_pengaturan_item.FlatAppearance.BorderColor = Color.DodgerBlue;
            btn_pengaturan_item.FlatAppearance.BorderSize = 2;
                        
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement();
            

            SqlCommand sql = new SqlCommand("Select * from item_tipe", koneksi.KoneksiDB());
            cb_tipeItem.Items.Clear();
            reader = sql.ExecuteReader();
            while (reader.Read())
            {
                ComboboxItem item = new ComboboxItem();
                item.Value = reader.GetValue(0).ToString();
                item.Text = reader.GetValue(1).ToString();
                cb_tipeItem.Items.Add(item);
            }
            koneksi.closeConnection();

            cb_tipeItem.SelectedIndex = 0;
            refreshGridViewDataItem();

            panel_pengaturanItem.BringToFront();
        }

        private void btn_tambahItem_Click(object sender, EventArgs e)
        {
            try
            {
                ComboboxItem item = (ComboboxItem)cb_tipeItem.SelectedItem;
                SqlCommand sql = new SqlCommand("insert into item(item, item_tipe_id, harga) values (@a,@b,@c)", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", input_namaItem.Text);
                sql.Parameters.AddWithValue("@b", item.Value);
                sql.Parameters.AddWithValue("@c", input_hargaItem.Text);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                refreshGridViewDataItem();
                item_id = 0;

                MessageBox.Show(input_namaItem.Text + " telah ditambahkan!");
            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }
        }
        
        private void FormUtama_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.Hide();
            //this.Parent = null;
            //Application.Exit();
        }
        
        private void cbKriteriaCari_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbKriteriaCari.Text.Equals("Laundry") || cbKriteriaCari.Text.Equals("Lainnya"))
            {
                txtHargaLaundry.Visible = true;
            }
            else
            {
                txtHargaLaundry.Visible = false;
            } 
            
            panel2.Visible = true;
            panel2.BringToFront();
            SqlCommand sqlC = new SqlCommand("select item_tipe_id from Item_Tipe where item_tipe = @a", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@a", cbKriteriaCari.Text);
            int KOdetipe = Int32.Parse(sqlC.ExecuteScalar().ToString());

            koneksi.closeConnection();

            SqlDataAdapter da = new SqlDataAdapter("select Item.item_id, Item.item, Item_Tipe.item_tipe, Item.harga from Item, Item_Tipe where Item.item_tipe_id = Item_Tipe.item_tipe_id and Item_Tipe.item_tipe_id=@a", koneksi.KoneksiDB());
            da.SelectCommand.Parameters.AddWithValue("@a", KOdetipe);
            DataTable ds = new DataTable();
            da.Fill(ds);
            dataGridView5.DataSource = ds;
            koneksi.closeConnection();
        
        }
        
        private void btn_simpanItem_Click(object sender, EventArgs e)
        {
            try{
                if (item_id != 0)
                {
                    ComboboxItem item = (ComboboxItem)cb_tipeItem.SelectedItem;
                    SqlCommand sql = new SqlCommand("update item set item=@a, item_tipe_id=@b, harga=@c where item_id=@d", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", input_namaItem.Text);
                    sql.Parameters.AddWithValue("@b", item.Value);
                    sql.Parameters.AddWithValue("@c", input_hargaItem.Text);
                    sql.Parameters.AddWithValue("@d", item_id);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    MessageBox.Show(input_namaItem.Text + " telah diubah!");
                }
                else
                {
                    MessageBox.Show("Silakan pilih item yang ingin diubah!");
                }
                refreshGridViewDataItem();
            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }
        }

        private void btn_hapusItem_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCommand sql = new SqlCommand("delete from item where item_id=@a", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", item_id);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                refreshGridViewDataItem();
                item_id = 0;

                MessageBox.Show(input_namaItem.Text + " telah dihapus!");
            }
            catch
            {
                MessageBox.Show("Silakan pilih data yang ingin dihapus!");
            }
        }

        private void btn_bookingHangus_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            resetBtnKonfigurasi();
            refreshActivatedButton();
            btn_bookingHangus.FlatAppearance.BorderColor = Color.DodgerBlue;
            btn_bookingHangus.FlatAppearance.BorderSize = 2;
            
            
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            
            HideBtnStatusKamar();
            hideBookingElement();
            refresh_panelBookingHangus();

            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            panelBookingHangus.BringToFront();
        }

        string booking_hangus_id = "";
        private void bookingKamarHangus(object sender, EventArgs e)
        {
            panelPembayaran.Visible = false;

            Button btn = sender as Button;
            dataKamarCh = Int32.Parse(btn.Text);
            booking_hangus_id = btn.Tag.ToString();

            contextMenuBookingHangus.Show(Cursor.Position);

            /*SqlCommand sql = new SqlCommand("Select count(*) from reservasi r inner join booking b on r.booking_id=b.booking_id where r.status='booking' and r.downpayment=0 and (datediff(minute,b.tgl_booking,SYSDATETIME())>180)", koneksi.KoneksiDB());
            int bookinghangus = Int32.Parse(sql.ExecuteScalar().ToString());
            if (bookinghangus > 0)
            {
                booking_notif = true;
            }
            else
            {
                booking_notif = false;
            }
            koneksi.closeConnection();*/

        }

        private void panelPesanItem_Click(object sender, EventArgs e)
        {
            panelPesanItem.BringToFront();
            panel2.Visible = false;
        }

        private void bayarToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            txtuseDp.Text = "0";
            txtJumUang.Text = "0";
            lblduidLebih.Visible = false;
            lblUseDP.Visible = false;
            txtuseDp.Visible = false;
            
            panelPembayaran.Visible = true;
            panelPembayaran.BringToFront();
            cbPembayaranReser.Text = cbPembayaranReser.Items[0].ToString();
            lblTanggalBayar.Text = DateTime.Now.Date.ToString("dd-MMM-yyyy");


            SqlCommand querybayar = new SqlCommand("select reservasi_id, booking_id from Reservasi where kamar_no = @a and status='booking' and booking_id=@b_id", koneksi.KoneksiDB());
            querybayar.Parameters.AddWithValue("@a", dataKamarCh);
            querybayar.Parameters.AddWithValue("@b_id", booking_hangus_id);
            SqlDataReader readData = querybayar.ExecuteReader();
            while (readData.Read())
            {
                idReservasi = Convert.ToInt32(readData["reservasi_id"].ToString());
                idBooking = Convert.ToInt32(readData["booking_id"].ToString());
            }
            koneksi.closeConnection();

            SqlCommand sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", idBooking);
            int kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", kodediskon);
            float potongan = float.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            querybayar = new SqlCommand("select downpayment,tag_kamar,tag_restoran from Reservasi where reservasi_id = @id", koneksi.KoneksiDB());
            querybayar.Parameters.AddWithValue("@id", idReservasi);
            SqlDataReader readD = querybayar.ExecuteReader();
            int tagihankamar = 0;
            int diskon = 0;
            while (readD.Read())
            {
                tagihankamar = (int)(Int32.Parse(readD["tag_kamar"].ToString()) *potongan )/100;
                tagihankamar += Int32.Parse(readD["tag_restoran"].ToString());
                diskon = Int32.Parse(readD["downpayment"].ToString());
            }
            koneksi.closeConnection();

            int totalbiayakamar = tagihankamar - diskon;

            koneksi.closeConnection();
            lblBiayaTag.Text = "Rp." + totalbiayakamar.ToString() + ",00" ;
            //lblBiayaTag.Text = "Tagihan : Rp." + totalbiayakamar.ToString() + ",00" + "  Tag Kamar" + tagihankamar.ToString() + "  Potongan" + potongan.ToString();
                
            lblBiayaTag.Tag = totalbiayakamar.ToString();
        }
        
        private void BatalStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Anda yakin untuk membatalkan semua booking kamar ini", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                SqlCommand queryData;
                /*= new SqlCommand("select max(booking_id) from Reservasi where status='booking' and kamar_no = @a", koneksi.KoneksiDB());
                queryData.Parameters.AddWithValue("@a", dataKamarCh);
                int idbookingData = Int32.Parse(queryData.ExecuteScalar().ToString());
                koneksi.closeConnection();
                */
                int idbookingData = Int32.Parse(booking_hangus_id);
                queryData = new SqlCommand("update Reservasi set status='cancel', checkout=@tggal where booking_id =@a", koneksi.KoneksiDB());
                queryData.Parameters.AddWithValue("@a", idbookingData);
                queryData.Parameters.AddWithValue("@tggal", DateTime.Now);
                queryData.ExecuteNonQuery();
                koneksi.closeConnection();

                queryData = new SqlCommand("update Booking set balance_due=0 where booking_id =@a", koneksi.KoneksiDB());
                queryData.Parameters.AddWithValue("@a", idbookingData);
                queryData.ExecuteNonQuery();
                koneksi.closeConnection();

                refresh_panelBookingHangus();
            }
        }
        
        private void refresh_panelBookingHangus()
        {
            panelBookingHangus.Controls.Clear();
            //SqlCommand sql = new SqlCommand("Select count(*) from reservasi r inner join booking b on r.booking_id=b.booking_id where r.status='booking' and r.downpayment=0 and (datediff(minute,b.tgl_booking,SYSDATETIME())>180)", koneksi.KoneksiDB());
            SqlCommand cmd = new SqlCommand("Select distinct b.booking_id from reservasi r inner join booking b on r.booking_id=b.booking_id where r.status='booking' and (datediff(minute,b.tgl_booking,SYSDATETIME())>180) group by b.booking_id having SUM(r.downpayment)<=0", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            Button[] Kamar;
            Kamar = new Button[1000];
            x = 0;
            while (reader.Read())
            {
                SqlCommand sql = new SqlCommand("Select distinct k.kamar_no, kt.kamar_tipe,kt.warna from reservasi r inner join booking b on r.booking_id=b.booking_id inner join kamar k on k.kamar_no=r.kamar_no inner join kamar_tipe kt on k.kamar_tipe_id=kt.kamar_tipe_id where b.booking_id=@booking_id and r.status='booking'", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@booking_id", reader.GetValue(0));
                SqlDataReader rd = sql.ExecuteReader();
                
                while (rd.Read())
                {
                    Kamar[x] = new Button();
                    Kamar[x].Text = rd.GetInt32(0).ToString();
                    Kamar[x].Name = rd.GetInt32(0).ToString();
                    Kamar[x].Visible = true;
                    //Kamar[x].Height = 35;
                    Kamar[x].Tag = reader.GetValue(0).ToString();
                    //Kamar[x].BackColor = Color.FromName(rd.GetString(2));
                    try
                    {
                        Kamar[x].BackColor = Color.FromArgb(Int32.Parse(rd.GetString(2)));
                    }
                    catch
                    {
                        Kamar[x].BackColor = Color.FromName(rd.GetString(2));
                    }
                    Kamar[x].Click += new EventHandler(bookingKamarHangus);
                    Kamar[x].Height = 45;
                    Kamar[x].Width = 95;
                    Kamar[x].FlatStyle = FlatStyle.Flat;
                    Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
                    Kamar[x].ImageAlign = btnBooking.ImageAlign;
                
                    Kamar[x].MouseEnter += new EventHandler(tooltipshowbookinghangus);
                    Kamar[x].MouseLeave += new EventHandler(tooltipclosebookinghangus);
                    panelBookingHangus.Controls.Add(Kamar[x]);
                    x += 1;
                }

            }
            koneksi.closeConnection();


        }

        private void PanelPesan_Click(object sender, EventArgs e)
        {
            panelPembayaran.Visible = false;
            panelCatatanBook.SendToBack();
        }

        private void panelBookingHangus_Click(object sender, EventArgs e)
        {
            panelPembayaran.Visible = false;
        }

        private void btn_submitPembayaranRestoran_Click(object sender, EventArgs e)
        {
            SqlCommand sql = new SqlCommand("select max(noPemesanan) from HRestaurant where noMeja =@a", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", noMejaDiclick);
            int nopesan = Int32.Parse(sql.ExecuteScalar().ToString());
            koneksi.closeConnection();
            int idReservasi = 0;
            int idBooking = 0;
            if (noKamarRestoran != 0)
            {

                sql = new SqlCommand("select reservasi_id from HRestaurant where NoPemesanan =@a", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", nopesan);
                idReservasi = Int32.Parse(sql.ExecuteScalar().ToString());
                koneksi.closeConnection();

                sql = new SqlCommand("select booking_id from reservasi where reservasi_id =@a", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", idReservasi);
                idBooking = Int32.Parse(sql.ExecuteScalar().ToString());
                koneksi.closeConnection();

            }

            sql = new SqlCommand("select Biaya from HRestaurant where NoPemesanan=@b ", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@b", nopesan);
            int biayaBaru = Int32.Parse(sql.ExecuteScalar().ToString());
            koneksi.closeConnection();

            SqlCommand querybayar = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment, staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
            //if (noKamarRestoran != 0)
            //{
            //    querybayar.Parameters.AddWithValue("@a", idBooking);
            //    querybayar.Parameters.AddWithValue("@b", idReservasi);
            //}
            //else
            //{
                querybayar.Parameters.AddWithValue("@a", DBNull.Value);
                querybayar.Parameters.AddWithValue("@b", DBNull.Value);
            //}
            querybayar.Parameters.AddWithValue("@c", cb_jenisPembaynaranRestor.Text);
            //querybayar.Parameters.AddWithValue("@c", "Kontan");
            querybayar.Parameters.AddWithValue("@d", input_CCPembayaranRestoran.Text);
            querybayar.Parameters.AddWithValue("@e", biayaBaru);
            querybayar.Parameters.AddWithValue("@f", DateTime.Now);
            querybayar.Parameters.AddWithValue("@g", Login.idS.ToString());
            querybayar.ExecuteNonQuery();
            koneksi.closeConnection();

            /*List<Microsoft.Reporting.WinForms.ReportParameter> list = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            //Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("NoMeja", noMejaPembayaran.Text);
            //list.Add(param);
            Microsoft.Reporting.WinForms.ReportParameter param2 = new Microsoft.Reporting.WinForms.ReportParameter("NoPemesanan", nopesan.ToString());
            list.Add(param2);
            reportInvoice.ServerReport.ReportPath = "/Invoice/InvoiceRestoran";
            reportInvoice.ServerReport.SetParameters(list);
            //reportInvoice.ServerReport.Refresh();
            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
            */
            //panelPembayaranRestoran.Visible = false;
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.dataTable1TableAdapter.Fill(this.inforRestoran.DataTable1, nopesan);
            //panelReportRestoran.BringToFront();
            reportInvoiceRestoran.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            reportInvoiceRestoran.LocalReport.SetParameters(parameter);
    
            reportInvoiceRestoran.LocalReport.Refresh();
            reportInvoiceRestoran.BringToFront();
            reportInvoiceRestoran.Refresh();
            reportInvoiceRestoran.RefreshReport();
        }

        private void printInvoiceKamarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Button btn = sender as Button;
            //contextMenuStrip2.Show(Cursor.Position);
            //dataKamarCh = Int32.Parse(btn.Text);
            SqlCommand sqlq = new SqlCommand("select booking_id from Reservasi where kamar_no = @dataKamar and status='checkin'", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@dataKamar", dataKamarCh);                    
            reader = sqlq.ExecuteReader();
            reader.Read();
            //MessageBox.Show(dataKamarCh.ToString());
            //List<Microsoft.Reporting.WinForms.ReportParameter> list = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            //Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("booking_id", reader.GetInt64(0).ToString());
            //list.Add(param);
            //Microsoft.Reporting.WinForms.ReportParameter param2 = new Microsoft.Reporting.WinForms.ReportParameter("room", dataKamarCh.ToString());
            //list.Add(param2);            
            //reportInvoice.ServerReport.ReportPath = "/Invoice/Invoice_Booking";
            //reportInvoice.ServerReport.SetParameters(list);
            //reportInvoice.BringToFront();
            //List<Microsoft.Reporting.WinForms.ReportParameter> parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            //reportInvoice.ServerReport.SetParameters(parameter_reset);
            
            /*List<Microsoft.Reporting.WinForms.ReportParameter> parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            Microsoft.Reporting.WinForms.ReportParameter p = new Microsoft.Reporting.WinForms.ReportParameter("booking_id");
            p.Values.Add(null);
            parameter_reset.Add(p);
            List<Microsoft.Reporting.WinForms.ReportParameter> list = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("booking_id", reader.GetInt64(0).ToString());
            list.Add(param);
            Microsoft.Reporting.WinForms.ReportParameter param2 = new Microsoft.Reporting.WinForms.ReportParameter("room", dataKamarCh.ToString());
            list.Add(param2);
            reportInvoice.ServerReport.ReportPath = "/Invoice/Invoice_Booking";
            reportInvoice.ServerReport.SetParameters(list);
            //reportInvoice.ServerReport.Refresh();
            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
            parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            reportInvoice.ServerReport.SetParameters(parameter_reset);*/
            String booking_id = reader.GetInt64(0).ToString();
            this.infoBooking.EnforceConstraints = false;
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.newSelectLastPaymentTableAdapter.Fill(this.lastPayment.NewSelectLastPayment, Int32.Parse(booking_id), Int32.Parse(dataKamarCh.ToString()));
                  
            this.TamuTableAdapter.Fill(this.infoBooking.Tamu, Int32.Parse(booking_id));
            this.NewSelectCommandTableAdapter.Fill(this.infoReservasi.NewSelectCommand, Int32.Parse(booking_id), Int32.Parse(dataKamarCh.ToString()));
            this.newSelectCommandTableAdapter1.Fill(this.pemesanan.NewSelectCommand, Int32.Parse(booking_id), Int32.Parse(dataKamarCh.ToString()));
            this.NewSelectPembayaranTableAdapter.Fill(this.infoPembayaran.NewSelectPembayaran, Int32.Parse(booking_id), Int32.Parse(dataKamarCh.ToString()));
            
            
            reportInvoice.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
            reportInvoice.LocalReport.SetParameters(parameter);

            string imagePath2 = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(booking_id) + ".png";
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(booking_id) + ".png"))
            {
                imagePath2 = "NULL";
            }
            ReportParameter parameter2 = new ReportParameter("ttd", imagePath2);
            reportInvoice.LocalReport.SetParameters(parameter2);
    
            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
            
        }

        private void printInvoiceBookingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlCommand sqlq = new SqlCommand("select booking_id from Reservasi where kamar_no = @dataKamar and status='checkin'", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@dataKamar", dataKamarCh);
            reader = sqlq.ExecuteReader();
            reader.Read();
            /*List<Microsoft.Reporting.WinForms.ReportParameter> list = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("booking_id", reader.GetInt64(0).ToString());
            list.Add(param);
            reportInvoice.ServerReport.ReportPath = "/Invoice/Invoice_Booking";
            reportInvoice.ServerReport.SetParameters(list);
            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
            List<Microsoft.Reporting.WinForms.ReportParameter> parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            reportInvoice.ServerReport.SetParameters(parameter_reset);*/
            String booking_id = reader.GetInt64(0).ToString();
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.newSelectLastPaymentTableAdapter.Fill(this.lastPayment.NewSelectLastPayment, Int32.Parse(booking_id), null);

            this.infoBooking.EnforceConstraints = false;
            this.TamuTableAdapter.Fill(this.infoBooking.Tamu, Int32.Parse(booking_id));
            this.NewSelectCommandTableAdapter.Fill(this.infoReservasi.NewSelectCommand, Int32.Parse(booking_id), null);
            this.newSelectCommandTableAdapter1.Fill(this.pemesanan.NewSelectCommand, Int32.Parse(booking_id), null);
            this.NewSelectPembayaranTableAdapter.Fill(this.infoPembayaran.NewSelectPembayaran, Int32.Parse(booking_id), null);
            reportInvoice.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
            reportInvoice.LocalReport.SetParameters(parameter);
            string imagePath2 = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(booking_id) + ".png";
            
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(booking_id) + ".png"))
            {
                imagePath2 = "NULL";
            }

            ReportParameter parameter2 = new ReportParameter("ttd", imagePath2);
            reportInvoice.LocalReport.SetParameters(parameter2);

            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
            
        }

        private void btnLaporanKeuangan_Click(object sender, EventArgs e)
        {
            //reportLaporanPendapatan.Invalidate();


            refreshActivatedButton();
            btnLaporanKeuangan.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnLaporanKeuangan.FlatAppearance.BorderSize = 2;

            flowLayoutPanel1.Visible = false;

            HideBtnStatusKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            //panelLaporanKeuangan.BringToFront();
            /*List<Microsoft.Reporting.WinForms.ReportParameter> list = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("tahun", "2014");
            list.Add(param);
            Microsoft.Reporting.WinForms.ReportParameter param2 = new Microsoft.Reporting.WinForms.ReportParameter("bulan", "8");
            list.Add(param2);
            reportInvoice.ServerReport.ReportPath = "/Invoice/Laporan_Pendapatan_Harian";
            reportInvoice.ServerReport.SetParameters(list);
            //reportInvoice.ServerReport.Refresh();
            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
            List<Microsoft.Reporting.WinForms.ReportParameter> parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            reportInvoice.ServerReport.SetParameters(parameter_reset);*/
            switch (DateTime.Now.Month)
            {
                case 1: cb_bulanLaporan.Text = "Januari"; break;
                case 2: cb_bulanLaporan.Text = "Februari"; break;
                case 3: cb_bulanLaporan.Text = "Maret"; break;
                case 4: cb_bulanLaporan.Text = "April"; break;
                case 5: cb_bulanLaporan.Text = "Mei"; break;
                case 6: cb_bulanLaporan.Text = "Juni"; break;
                case 7: cb_bulanLaporan.Text = "Juli"; break;
                case 8: cb_bulanLaporan.Text = "Agustus"; break;
                case 9: cb_bulanLaporan.Text = "September"; break;
                case 10: cb_bulanLaporan.Text = "Oktober"; break;
                case 11: cb_bulanLaporan.Text = "November"; break;
                default: cb_bulanLaporan.Text = "Desember"; break;
            }
            cb_tahunLaporan.Text = DateTime.Now.Year.ToString();
            flowLayoutPanel4.Visible = true;
            cekPilihLaporan = true;
            flowLayoutGrandTotalInput.Visible = false;
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.dataTable1TableAdapter1.Fill(this.infoPendapatan.DataTable1, DateTime.Now.Year, DateTime.Now.Month);

            reportLaporanPendapatan.Reset();
            reportLaporanPendapatan.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.Laporan_Pendapatan_Harian.rdlc";
            reportLaporanPendapatan.LocalReport.DataSources.Add(new ReportDataSource("infoPendapatan", (object)infoPendapatan.DataTable1));
            reportLaporanPendapatan.LocalReport.DataSources.Add(new ReportDataSource("infoHotel", (object)infoHotel.IDHotel));

            reportLaporanPendapatan.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            reportLaporanPendapatan.LocalReport.SetParameters(parameter);

            reportLaporanPendapatan.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
            reportLaporanPendapatan.RefreshReport();
            reportLaporanPendapatan.BringToFront();
        }
        

        void SubreportProcessingEventHandler(object sender, SubreportProcessingEventArgs e){
            this.DataTableInfoSubTableAdapter.Fill(this.infoSubPendapatan.DataTableInfoSub, "2014-09-04");
            e.DataSources.Add(new ReportDataSource("DataSetSub",(object)infoSubPendapatan.DataTableInfoSub));
        }

        private void pindahKamarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Anda yakin untuk memindahkan reservasi kamar ini", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                hideBookingElement();
                panelKamarDibooking.Controls.Clear();
                hidepanelPengaturanKamar();
                PanelPesan.BringToFront();
                PanelPesan.Controls.Clear();
                SqlCommand cmd = new SqlCommand((@"select count(*) from Kamar"), koneksi.KoneksiDB());

                int jumKamar = (int)cmd.ExecuteScalar();
                koneksi.closeConnection();
                ///button1.Text = jumKamar.ToString();
                Button[] Kamar;

                //command.Parameters.AddWithValue("@Username", username);
                //command.Parameters.AddWithValue("@Password", password);
                cmd = new SqlCommand("select checkin, checkout from Reservasi where kamar_no = @a and status='checkin'", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@a", dataKamarCh);
                SqlDataReader sqlRM = cmd.ExecuteReader();
                DateTime Dcheckin;
                DateTime Dcheckout = DateTime.Now;
                while (sqlRM.Read())
                {
                    Dcheckin = Convert.ToDateTime(sqlRM["checkin"].ToString());
                    Dcheckout = Convert.ToDateTime(sqlRM["checkout"].ToString());

                }

                koneksi.closeConnection();

                cmd = new SqlCommand(
                (@"
            select kamar_no
            from Kamar
            where status is null
            except
            select Kamar.kamar_no 
            from Kamar, Reservasi 
            where Kamar.kamar_no = Reservasi.kamar_no 
            and Reservasi.status in ('checkin','booking') 
                    and (
                        (Reservasi.checkin >= @checkindate
	                    and
	                    Reservasi.checkout <=@checkoutdate
	                    )
	                    or
	                    (
	                    Reservasi.checkin <= @checkindate
	                    and
	                    Reservasi.checkout >=@checkoutdate
	                    )
	                    or 
	                    (
	                    Reservasi.checkin >= @checkindate
	                    and
	                    Reservasi.checkin < @checkoutdate
	                    )
	                    or 
	                    (
	                    Reservasi.checkout > @checkindate
	                    and
	                    Reservasi.checkout <=@checkoutdate
	                    )
                    )
                       
                "), koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@checkindate", DateTime.Now);
                cmd.Parameters.AddWithValue("@checkoutdate", Dcheckout);

                /*
             
                 cmd = new SqlCommand(
                (@"select
                k.kamar_no,
                k.kamar_tipe_id,
                case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga
                from            
                Kamar k
                inner join kamar_tipe kt on k.kamar_tipe_id = kt.kamar_tipe_id 
                inner join harga h on h.tanggal_id = '2008-7-1'
                and kt.kamar_tipe_id = h.kamar_tipe_id"), koneksi.KoneksiDB());
                //cmd.Parameters.AddWithValue("@checkindate",checkinDate.Value.ToString("yyyy-M-d"));
                //cmd.Parameters.AddWithValue("@checkoutdate",checkoutDate.Value.ToString("yyyy-M-d"));
                 */


                reader = cmd.ExecuteReader();
                Kamar = new Button[jumKamar];
                x = 0;
                while (reader.Read())
                {
                    Kamar[x] = new Button();
                    Kamar[x].Text = reader.GetInt32(0).ToString();
                    Kamar[x].Name = reader.GetInt32(0).ToString();
                    Kamar[x].Visible = true;
                    //Kamar[x].Height = 35;
                    //Kamar[x].Tag = 0;
                    //Kamar[x].BackColor = Color.FromName(reader.GetString(1));
                    Kamar[x].Height = 45;
                    Kamar[x].Width = 95;
                    Kamar[x].FlatStyle = FlatStyle.Flat;
                    Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
                    Kamar[x].ImageAlign = btnBooking.ImageAlign;

                    Kamar[x].Click += new EventHandler(PindahKamar);
                    //Kamar[x].MouseEnter += new EventHandler(button1_MouseEnter_2);
                    //Kamar[x].MouseLeave += new EventHandler(button1_MouseLeave_1);

                    PanelPesan.Controls.Add(Kamar[x]);
                    x += 1;
                    //Kamar[x].MouseEnter += button1_MouseEnter_2;// Kamar_Tips;//new EventHandler(Kamar_Tips);

                }
                //conn.Close();
                koneksi.closeConnection();
            }
        }
        private void PindahKamar(object sender, EventArgs e)
        {
            //MessageBox.Show(dataKamarCh.ToString());
            Button idBtn = sender as Button;
            string KamarBaru = idBtn.Text;



            SqlCommand sqlC = new SqlCommand("select kamar_tipe_id from Kamar where kamar_no = @no", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@no", KamarBaru);
            string nilai = sqlC.ExecuteScalar().ToString();
            koneksi.closeConnection();

            sqlC = new SqlCommand("select kamar_tipe_id from Kamar where kamar_no = @no", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@no", dataKamarCh);
            string nilai2 = sqlC.ExecuteScalar().ToString();
            koneksi.closeConnection();


            sqlC = new SqlCommand("select checkin,checkout, tag_kamar,tamu_id,downpayment from Reservasi where kamar_no = @a and status='checkin'", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@a", dataKamarCh);
            SqlDataReader readCBaru = sqlC.ExecuteReader();
            int HargaKamarL = 0;
            int idTamu = 0;
            DateTime tngalkeluar = DateTime.Now.Date;
            DateTime tngalmasuk = DateTime.Now.Date;
            int downPaymentKamar = 0;
            while (readCBaru.Read())
            {
                HargaKamarL = Int32.Parse(readCBaru["tag_kamar"].ToString());
                tngalkeluar = Convert.ToDateTime(readCBaru["checkout"].ToString());
                tngalmasuk = Convert.ToDateTime(readCBaru["checkin"].ToString());
                idTamu = Int32.Parse(readCBaru["tamu_id"].ToString());
                downPaymentKamar = Int32.Parse(readCBaru["downpayment"].ToString());
            }
            koneksi.closeConnection();



            SqlCommand sql1 = new SqlCommand("select case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end hargaK from Harga where kamar_tipe_id = @tipe and tanggal_id >= @chin and tanggal_id< @chou", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@tipe", nilai);
            sql1.Parameters.AddWithValue("@chin", DateTime.Now.Date);
            sql1.Parameters.AddWithValue("@chou", tngalkeluar);
            SqlDataReader readCcc = sql1.ExecuteReader();

            int biayaKamar = 0;
            while (readCcc.Read())
            {
                biayaKamar += Int32.Parse(readCcc["hargaK"].ToString());
            }
            koneksi.KoneksiDB();

            //MessageBox.Show(tngalmasuk.ToString());
            sql1 = new SqlCommand("select case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end hargaK from Harga where kamar_tipe_id = @tipe and tanggal_id >= convert(date,@chin) and tanggal_id< @chou", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@tipe", nilai2);
            sql1.Parameters.AddWithValue("@chin", tngalmasuk);
            sql1.Parameters.AddWithValue("@chou", DateTime.Now.Date);
            SqlDataReader readCcc2 = sql1.ExecuteReader();

            int biayaKamar2 = 0;
            while (readCcc2.Read())
            {
                biayaKamar2 += Int32.Parse(readCcc2["hargaK"].ToString());
            }
            koneksi.KoneksiDB();


            string selisihJum = (biayaKamar2 + biayaKamar).ToString();
            sqlC = new SqlCommand("select reservasi_id,booking_id from Reservasi where kamar_no = @b and status='checkin'", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@b", dataKamarCh);
            SqlDataReader readC = sqlC.ExecuteReader();
            int idReser = 0;
            int idBook = 0;
            while (readC.Read())
            {
                idReser = Int32.Parse(readC["reservasi_id"].ToString());
                idBook = Int32.Parse(readC["booking_id"].ToString());
            }
            koneksi.closeConnection();

            sql1 = new SqlCommand("select booking_id, tag_restoran from Reservasi where status='checkin' and kamar_no = @kamarno", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@kamarno", dataKamarCh);
            SqlDataReader readCc = sql1.ExecuteReader();
            int kodeid = 0;
            int kodediskon = 0;
            int tagihanResto = 0;
            while (readCc.Read())
            {
                kodeid = Int32.Parse(readCc["booking_id"].ToString());
                tagihanResto = Int32.Parse(readCc["tag_restoran"].ToString());
            }
            koneksi.closeConnection();

            sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", kodeid);
            kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", kodediskon);
            float potongan = float.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();
            int[] hargaKR = new int[99];
            int[] hargaDR = new int[99];
            int countDr = 0;
            sql1 = new SqlCommand("select tag_kamar, downpayment from Reservasi where booking_id = @a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", kodeid);
            SqlDataReader readJ = sql1.ExecuteReader();
            while (readJ.Read())
            {
                hargaKR[countDr] = Int32.Parse(readJ["tag_kamar"].ToString());
                hargaDR[countDr] = Int32.Parse(readJ["downpayment"].ToString());
                
                countDr += 1;
            }
            koneksi.closeConnection();
            int cekManual = 0;
            for (int i = 0; i < countDr; i++)
            {
                if ((int)((hargaKR[i] * potongan) / 100) == hargaDR[i])
                {
                    cekManual += 1;
                }
            }

            int jumSeharusnya = 0;
            if (cekManual == countDr)
            {
                sqlC = new SqlCommand("update Reservasi set checkout=@a ,realcheckout=@a, status='checkout', tag_kamar=@c,downpayment=@d where kamar_no = @b and status='checkin'", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@a", DateTime.Now);
                sqlC.Parameters.AddWithValue("@b", dataKamarCh);
                sqlC.Parameters.AddWithValue("@c", biayaKamar2);
                sqlC.Parameters.AddWithValue("@d", (biayaKamar2 * potongan) / 100);

                sqlC.ExecuteNonQuery();
                koneksi.closeConnection();
            }
            else
            {
                sqlC = new SqlCommand("update Reservasi set checkout=@a ,realcheckout=@a, status='checkout',tag_restoran=0, tag_kamar=@c, downpayment=@d where kamar_no = @b and status='checkin'", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@a", DateTime.Now);
                sqlC.Parameters.AddWithValue("@b", dataKamarCh);
                sqlC.Parameters.AddWithValue("@c", biayaKamar2);
                jumSeharusnya = downPaymentKamar - (int)((biayaKamar2 * potongan) / 100);
                //120000-319200
                if (jumSeharusnya >= 0)
                {
                    sqlC.Parameters.AddWithValue("@d", downPaymentKamar - jumSeharusnya);
                }
                else
                {
                    sqlC.Parameters.AddWithValue("@d", downPaymentKamar);
                }
                sqlC.ExecuteNonQuery();
                koneksi.closeConnection();

            }

            sqlC = new SqlCommand("select sum(downpayment) from Reservasi where booking_id =@b", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@b", idBook);
            int totalDOwn = Int32.Parse(sqlC.ExecuteScalar().ToString());
            koneksi.closeConnection();
            sqlC = new SqlCommand("select sum(tag_restoran) from Reservasi where booking_id =@b", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@b", idBook);
            int totalRestoran = Int32.Parse(sqlC.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sqlC = new SqlCommand("select sum(tag_kamar) from Reservasi where booking_id =@b", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@b", idBook);
            int totaltagKamar = Int32.Parse(sqlC.ExecuteScalar().ToString()) + biayaKamar;
            koneksi.closeConnection();

            //tentukan kalau booking hasil gabungan
            sqlC = new SqlCommand("update Booking set tag_kamar=@as, grand_total=@as+@res, balance_due=@aa+@resto, statusbayar=1 where booking_id =@b", koneksi.KoneksiDB());
            //tentukan kalau booking hasil gabungan

            sqlC.Parameters.AddWithValue("@a", totaltagKamar);
            sqlC.Parameters.AddWithValue("@res", totalRestoran);
            sqlC.Parameters.AddWithValue("@aa", ((totaltagKamar * potongan) / 100) - totalDOwn + totalRestoran);
            sqlC.Parameters.AddWithValue("@b", idBook);
            sqlC.Parameters.AddWithValue("@as", totaltagKamar);
            if (cekManual == countDr)
            {
                sqlC.Parameters.AddWithValue("@resto", 0);
            }
            else
            {
                sqlC.Parameters.AddWithValue("@resto", tagihanResto);
            }
            sqlC.ExecuteNonQuery();
            koneksi.closeConnection();

            sqlC = new SqlCommand("update Kamar set status='1' where kamar_no =@b", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@b", dataKamarCh);
            sqlC.ExecuteNonQuery();
            koneksi.closeConnection();

            SqlCommand sql = new SqlCommand("insert into Reservasi(booking_id, checkin, checkout, tamu_id, kamar_no, tag_kamar,tag_restoran,tag_transport,harga_id,status,downpayment,realcheckout) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,'checkin',@j,@k)", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", idBook);
            sql.Parameters.AddWithValue("@b", DateTime.Now);
            sql.Parameters.AddWithValue("@c", tngalkeluar);
            sql.Parameters.AddWithValue("@d", idTamu);
            sql.Parameters.AddWithValue("@e", KamarBaru);
            sql.Parameters.AddWithValue("@f", biayaKamar);
            sql.Parameters.AddWithValue("@g", tagihanResto);
            sql.Parameters.AddWithValue("@h", 0);
            sql.Parameters.AddWithValue("@i", 1);

            if (cekManual == countDr)
            {
                sql.Parameters.AddWithValue("@j", (((HargaKamarL * potongan) / 100) - ((biayaKamar2 * potongan) / 100)));
            }//480000-()
            else
            {
                //  sql.Parameters.AddWithValue("@j", downPaymentKamar - ((biayaKamar2 * potongan) / 100));
                sql.Parameters.AddWithValue("@j", jumSeharusnya);
            }
            sql.Parameters.AddWithValue("@k", tngalkeluar);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();
            //saaa
            sqlC = new SqlCommand("select max(reservasi_id) from reservasi", koneksi.KoneksiDB());
            string idmaxreser = sqlC.ExecuteScalar().ToString();
            sqlC.ExecuteNonQuery();
            koneksi.closeConnection();


            sqlC = new SqlCommand("update pembayaran set reservasi_id=@a where reservasi_id =@b", koneksi.KoneksiDB());
            sqlC.Parameters.AddWithValue("@a", idmaxreser);
            sqlC.Parameters.AddWithValue("@b", idReser);
            sqlC.ExecuteNonQuery();
            koneksi.closeConnection();

            btnCheckInStatus_Click(sender, e);
            //ubah reservasi
            //ubah hrestaurant
            //ubah kamar
            //ubah kamar

        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panelPesanItem_Click_1(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void btnKonfirmasiCheckin_Click(object sender, EventArgs e)
        {
            bool cektanggal = false;
            int ctrCek = 0;
            foreach (DataRow dr in dKamarPesan.Rows)
            {
               // MessageBox.Show(dr["Checkin"].ToString());
                ctrCek = 1;
                    
                if (Convert.ToDateTime(dr["Checkin"].ToString()) != DateTime.Now.Date)
                {
                    cektanggal = true;
                }
            }
            foreach (DataRow dr in dKamarPesan.Rows)
            {

                SqlCommand sqlCheckin = new SqlCommand("select reservasi_id from Reservasi where kamar_no = @no and status='checkin'", koneksi.KoneksiDB());
                sqlCheckin.Parameters.AddWithValue("@no", dr["NO Kamar"]);
                SqlDataReader readCheckin = sqlCheckin.ExecuteReader();
                int CekData = 0;
                while (readCheckin.Read())
                {
                    CekData += 1;
                }
                koneksi.closeConnection();
                if (CekData > 0)
                {
                    cektanggal = true;
                }
            }


            if (cektanggal == false && ctrCek==1)
            {
                if (comboboxPembayaranBooking.Text.Equals("") || inputPembayaran.Text.Equals("") || inputNamaTamu.Text.Equals("") || inputTelepon.Text.Equals(""))
                {
                    MessageBox.Show("Pastikan Data Terisi");
                }
                else
                {
                    SqlCommand sql;
                    if (dataCustomer < 1)
                    {
                        sql = new SqlCommand("insert into Tamu(tamu,alamat,kota,telepon,email,perusahaan,tanggallahir,sebutan,gelar,noidentitas,jenisidentitas,warganegara,snapshot) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,@j,@k,@l,@m)", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", inputNamaTamu.Text);
                        sql.Parameters.AddWithValue("@b", inputAlamat.Text);
                        sql.Parameters.AddWithValue("@c", inputKota.Text);
                        sql.Parameters.AddWithValue("@d", inputTelepon.Text);
                        sql.Parameters.AddWithValue("@e", inputEmail.Text);
                        sql.Parameters.AddWithValue("@f", input_perusahaan.Text);
                        sql.Parameters.AddWithValue("@g", inputUlangTahun.Value);
                        sql.Parameters.AddWithValue("@h", inputSebutan.Text);
                        sql.Parameters.AddWithValue("@i", inputGelar.Text);
                        sql.Parameters.AddWithValue("@j", txtNoIdentitas.Text);
                        sql.Parameters.AddWithValue("@k", cbJnsIdentitas.Text);
                        sql.Parameters.AddWithValue("@l", txtWargaNegara.Text);
                        sql.Parameters.AddWithValue("@m", filejpeg);
                                                
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sql = new SqlCommand("select max(tamu_id) from Tamu", koneksi.KoneksiDB());
                        dataCustomer = Int32.Parse(sql.ExecuteScalar().ToString());
                        koneksi.closeConnection();
                        //btnKonfirmasiBooking.Text = "Booking Telah Dilakukan";
                        //btnKonfirmasiBooking.Enabled = false;
                    }

                    sql = new SqlCommand("update Tamu set tamu=@a,alamat=@b,kota=@c,telepon=@d,email=@e,perusahaan=@f,tanggallahir=@g,sebutan=@h,gelar=@i,noidentitas=@j,jenisidentitas=@k,warganegara=@l,snapshot=@m where tamu_id=@no", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", inputNamaTamu.Text);
                    sql.Parameters.AddWithValue("@b", inputAlamat.Text);
                    sql.Parameters.AddWithValue("@c", inputKota.Text);
                    sql.Parameters.AddWithValue("@d", inputTelepon.Text);
                    sql.Parameters.AddWithValue("@e", inputEmail.Text);
                    sql.Parameters.AddWithValue("@f", input_perusahaan.Text);
                    sql.Parameters.AddWithValue("@g", inputUlangTahun.Value);
                    sql.Parameters.AddWithValue("@h", inputSebutan.Text);
                    sql.Parameters.AddWithValue("@i", inputGelar.Text);
                    sql.Parameters.AddWithValue("@j", txtNoIdentitas.Text);
                    sql.Parameters.AddWithValue("@k", cbJnsIdentitas.Text);
                    sql.Parameters.AddWithValue("@l", txtWargaNegara.Text);
                    sql.Parameters.AddWithValue("@no", dataCustomer);
                    sql.Parameters.AddWithValue("@m", filejpeg);
                                                


                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();



                    sql = new SqlCommand("insert into Booking(tamu_id, tgl_booking, checkin, checkout, uang_muka, tag_kamar,tag_restoran,tag_transport,status,grand_total,payment,balance_due,note,booking_diskon_id,staff_id) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,@j,@k,@l,@m,@n,@o)", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", dataCustomer);
                    sql.Parameters.AddWithValue("@b", DateTime.Now);
                    sql.Parameters.AddWithValue("@c", DateTime.Now);
                    sql.Parameters.AddWithValue("@d", DateTime.Now);
                    sql.Parameters.AddWithValue("@e", 0);


                    float diskon = 100;
                    if (cb_diskon.Checked)
                    {
                        sql.Parameters.AddWithValue("@n", 1);

                        SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=1", koneksi.KoneksiDB());
                        diskon = float.Parse(s.ExecuteScalar().ToString());
                        //koneksi.closeConnection();   
                    }
                    else
                    {
                        if (Int32.Parse(diskonAngka.Text) > 0)
                        {
                            SqlCommand s = new SqlCommand("INSERT INTO Booking_diskon(booking_diskon, harga) VALUES('Custom Diskon', @hrg)", koneksi.KoneksiDB());
                            int diskonA = Int32.Parse(diskonAngka.Text);
                            float totalDiskon = (float)(diskonA * 100) / totalBiaya;
                            s.Parameters.AddWithValue("@hrg", ((float)(100 - totalDiskon) / 100));
                            s.ExecuteNonQuery();

                            s = new SqlCommand("select max(booking_diskon_id) from booking_diskon", koneksi.KoneksiDB());
                            int booking_diskon_id = Int32.Parse(s.ExecuteScalar().ToString());
                            sql.Parameters.AddWithValue("@n", booking_diskon_id);

                            diskon = (float)(100 - totalDiskon);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@n", 2);
                            SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=2", koneksi.KoneksiDB());
                            diskon = float.Parse(s.ExecuteScalar().ToString());
                        }
                    }

                    sql.Parameters.AddWithValue("@f", totalBiaya);
                    sql.Parameters.AddWithValue("@g", 0);
                    sql.Parameters.AddWithValue("@h", 0);
                    sql.Parameters.AddWithValue("@i", "NO");
                    sql.Parameters.AddWithValue("@j", totalBiaya);
                    sql.Parameters.AddWithValue("@k", 1);
                    totalBiaya = Convert.ToInt32((totalBiaya * diskon) / 100);
                    sql.Parameters.AddWithValue("@l", totalBiaya - Int32.Parse(inputPembayaran.Text));
                    sql.Parameters.AddWithValue("@m", txtCatatanBooking.Text);
                    //sql.Parameters.AddWithValue("@n", 1);
                    sql.Parameters.AddWithValue("@o", 1);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    sql = new SqlCommand("select max(booking_id) from Booking", koneksi.KoneksiDB());
                    int nilaimax = Int32.Parse(sql.ExecuteScalar().ToString());

                    koneksi.closeConnection();

                    //simpan reservasi
                    List<DataRow> rd = new List<DataRow>();
                    foreach (DataRow dr in dKamarPesan.Rows)
                    {
                        sql = new SqlCommand("insert into Reservasi(booking_id, checkin, checkout, tamu_id, kamar_no, tag_kamar,tag_restoran,tag_transport,harga_id,status,downpayment,realcheckout) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,'checkin',@j,@k)", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", nilaimax);
                        sql.Parameters.AddWithValue("@b", DateTime.Now);
                        sql.Parameters.AddWithValue("@c", dr["Checkout"]);
                        sql.Parameters.AddWithValue("@d", dataCustomer);
                        sql.Parameters.AddWithValue("@e", dr["NO Kamar"]);
                        sql.Parameters.AddWithValue("@f", dr["Harga"]);
                        sql.Parameters.AddWithValue("@g", 0);
                        sql.Parameters.AddWithValue("@h", 0);
                        sql.Parameters.AddWithValue("@i", 1);
                        if (comboBox4.Text.Equals("Down payment " + dr["NO Kamar"].ToString()))
                        {
                            
                            /*if (comboBox4.Text.Equals("OTA - Tanpa Batas Waktu"))
                            {
                                sql.Parameters.AddWithValue("@j", 1);
                            }
                            else
                            {*/
                        //        sql.Parameters.AddWithValue("@j", Int32.Parse(inputPembayaran.Text));
                                sql.Parameters.AddWithValue("@j", 0);
                            //}
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@j", 0);
                        }
                        sql.Parameters.AddWithValue("@k", dr["checkout"]);
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();
                    }

                    if (comboBox4.Text == "Lunas" || comboBox4.Text == "OTA - Tanpa Batas Waktu")
                    {
                        rd = new List<DataRow>();
                        int ctr = 0;
                        foreach (DataRow dr in dKamarPesan.Rows)
                        {
                            ctr++;
                            sql = new SqlCommand("select reservasi_id from Reservasi where kamar_no = @a and status = 'checkin' ", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@a", dr["NO Kamar"].ToString());
                            int reservasiIDPayment = Int32.Parse(sql.ExecuteScalar().ToString());
                            koneksi.closeConnection();
                            if (ctr == 1)
                            {
                                sql = new SqlCommand("insert into pembayaran(booking_id, reservasi_id,payment,nopayment,jumlahpayment,tggalpayment,staff_id) values (@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                                sql.Parameters.AddWithValue("@a", nilaimax);
                                sql.Parameters.AddWithValue("@b", reservasiIDPayment);
                                sql.Parameters.AddWithValue("@c", comboboxPembayaranBooking.Text);
                                sql.Parameters.AddWithValue("@d", inputCC1.Text);
                                sql.Parameters.AddWithValue("@e", 0);
                                sql.Parameters.AddWithValue("@f", DateTime.Now);
                                sql.Parameters.AddWithValue("@g", Login.idS.ToString());
                                sql.ExecuteNonQuery();
                                koneksi.closeConnection();
                            }
                            sql = new SqlCommand("insert into pembayaran(booking_id, reservasi_id,payment,nopayment,jumlahpayment,tggalpayment,staff_id) values (@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@a", nilaimax);
                            sql.Parameters.AddWithValue("@b", reservasiIDPayment);
                            sql.Parameters.AddWithValue("@c", comboboxPembayaranBooking.Text);
                            sql.Parameters.AddWithValue("@d", inputCC1.Text);
                            sql.Parameters.AddWithValue("@e", Convert.ToInt32((float.Parse(dr["Harga"].ToString()) * diskon) / 100));
                            sql.Parameters.AddWithValue("@f", DateTime.Now);
                            sql.Parameters.AddWithValue("@g", Login.idS.ToString());
                            sql.ExecuteNonQuery();
                            koneksi.closeConnection();

                            sql = new SqlCommand("update Reservasi set downpayment= downpayment+@a where reservasi_id =@b", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@a", Convert.ToInt32((float.Parse(dr["Harga"].ToString()) * diskon) / 100));
                            sql.Parameters.AddWithValue("@b", reservasiIDPayment);
                            sql.ExecuteNonQuery();
                            koneksi.closeConnection();

                        }
                    }
                    else
                    {
                        sql = new SqlCommand("select reservasi_id from Reservasi where kamar_no = @a and status = 'checkin' ", koneksi.KoneksiDB());
                        if (comboBox4.Text.Equals("OTA - Tanpa Batas Waktu"))
                        {
                            sql.Parameters.AddWithValue("@a", Int32.Parse(comboBox4.Items[0].ToString().Replace("Down payment ", "")));
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@a", Int32.Parse(comboBox4.Text.Replace("Down payment ", "")));
                        }
                        int reservasiIDPayment = Int32.Parse(sql.ExecuteScalar().ToString());
                        koneksi.closeConnection();
                        sql = new SqlCommand("insert into pembayaran(booking_id, reservasi_id,payment,nopayment,jumlahpayment,tggalpayment,staff_id) values (@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", nilaimax);
                        sql.Parameters.AddWithValue("@b", reservasiIDPayment);
                        sql.Parameters.AddWithValue("@c", comboboxPembayaranBooking.Text);
                        sql.Parameters.AddWithValue("@d", inputCC1.Text);
                        sql.Parameters.AddWithValue("@e", Int32.Parse(inputPembayaran.Text));
                        sql.Parameters.AddWithValue("@f", DateTime.Now);
                        sql.Parameters.AddWithValue("@g", Login.idS.ToString());

                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sql = new SqlCommand("update Reservasi set downpayment = @a where reservasi_id=@b",koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", Int32.Parse(inputPembayaran.Text));
                        sql.Parameters.AddWithValue("@b", reservasiIDPayment);
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();
                    }

                    //btnKonfirmasiBooking.Text = "Booking Telah Disimpan";
                    //btnKonfirmasiBooking.Enabled = false;
                    //panelKalender.BringToFront();
                    btnCheckInStatus_Click(sender, e);

                    foreach (DataRow dr in dKamarPesan.Rows)
                    {
                        sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0 and ik.Tipe='Rec'", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@room", dr["NO Kamar"].ToString());
                        reader = sql.ExecuteReader();
                        while (reader.Read())
                        {
                            cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                            cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                            cmd.Parameters.AddWithValue("@b", "HK");
                            cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                            cmd.Parameters.AddWithValue("@d", "R");
                            cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                            cmd.Parameters.AddWithValue("@f", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }
                        koneksi.closeConnection();
                    }

                    dKamarPesan.Clear();
                    comboBox4.Items.Clear();
                    dataCustomer = 0;
                }
            }
            else
            { MessageBox.Show("Terdapat NoKamar yang Belum dichekout \n Atau Tanggal checkin Harus hari ini"); }
        }
        private void setHarga()
        {
            SqlCommand sql = new SqlCommand("exec [dbo].[sp_generate_harga]",koneksi.KoneksiDB());
            sql.ExecuteNonQuery();
            koneksi.closeConnection();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            
            try{

                ComboboxItem selectedCar = (ComboboxItem)cbJnsKamarPeriodik.SelectedItem;

                SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) jumlah
                    FROM Reservasi r inner join Kamar k on r.kamar_no=k.kamar_no
	                    --inner join Kamar_Tipe kt on k.kamar_tipe_id=kt.kamar_tipe_id
                    where convert(date,r.checkout)>=@tggl and (r.status='booking' or r.status='checkin')
	                    and k.kamar_tipe_id=@kmr_tipe_id", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@tggl", dtPeriodik.Value.Date);
                cmd.Parameters.AddWithValue("@kmr_tipe_id", selectedCar.Value);
                int totalR = Int32.Parse(cmd.ExecuteScalar().ToString());
                koneksi.closeConnection();
                
                if(totalR > 0){
                    MessageBox.Show("Tidak dapat melakukan perubahan harga karena ada booking pada tipe kamar tersebut dan periode tersebut! \n atau silakan batalkan booking tersebut.");
                }else{
                    
                    SqlCommand sql = new SqlCommand("insert into Harga_Periodik (kamar_tipe_id,tgl_berlaku,harga,harga_weekend) values(@a,@b,@c,@d)", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", selectedCar.Value);
                    sql.Parameters.AddWithValue("@b", dtPeriodik.Value.Date);
                    sql.Parameters.AddWithValue("@c", txtHargaKPeriodik.Text);
                    sql.Parameters.AddWithValue("@d", txtHargaWkPeriodik.Text);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();


                    SqlDataAdapter da = new SqlDataAdapter("select hp.harga_periodik_id, kt.kamar_tipe , hp.tgl_berlaku, hp.harga, hp.harga_weekend from Harga_Periodik hp inner join Kamar_Tipe kt on hp.kamar_tipe_id=kt.kamar_tipe_id where year(hp.tgl_berlaku)>2008 and hp.kamar_tipe_id=@a", koneksi.KoneksiDB());
                    da.SelectCommand.Parameters.AddWithValue("@a", selectedCar.Value);
                    dHargaPeriodik = new DataTable();
                    da.Fill(dHargaPeriodik);
                    dataGridView7.DataSource = dHargaPeriodik;

                    koneksi.closeConnection();
                    setHarga();
                }
                
            
            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try{
                SqlCommand sql = new SqlCommand("delete from Harga_Periodik where harga_periodik_id=@a ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", kodeEdit);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();
                //setHargaPeriodik();

                ComboboxItem selectedCar = (ComboboxItem)cbJnsKamarPeriodik.SelectedItem;

                SqlDataAdapter da = new SqlDataAdapter("select hp.harga_periodik_id, kt.kamar_tipe , hp.tgl_berlaku, hp.harga, hp.harga_weekend from Harga_Periodik hp inner join Kamar_Tipe kt on hp.kamar_tipe_id=kt.kamar_tipe_id where year(hp.tgl_berlaku)>2008 and hp.kamar_tipe_id=@a", koneksi.KoneksiDB());
                da.SelectCommand.Parameters.AddWithValue("@a", selectedCar.Value);
                dHargaPeriodik = new DataTable();
                da.Fill(dHargaPeriodik);
                dataGridView7.DataSource = dHargaPeriodik;

                dataGridView7.Columns[0].DisplayIndex = 4;

                koneksi.closeConnection();
                setHarga();
            
            }
            catch
            {
                MessageBox.Show("Silakan pilih data yang ingin dihapus!");
            }
        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try{
                ComboboxItem selectedCar = (ComboboxItem)cbJnsKamarPeriodik.SelectedItem;

                SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) jumlah
                    FROM Reservasi r inner join Kamar k on r.kamar_no=k.kamar_no
	                    --inner join Kamar_Tipe kt on k.kamar_tipe_id=kt.kamar_tipe_id
                    where convert(date,r.checkout)>=@tggl and (r.status='booking' or r.status='checkin')
	                    and k.kamar_tipe_id=@kmr_tipe_id", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@tggl", dtPeriodik.Value.Date);
                cmd.Parameters.AddWithValue("@kmr_tipe_id", selectedCar.Value);
                int totalR = Int32.Parse(cmd.ExecuteScalar().ToString());
                koneksi.closeConnection();

                if (totalR > 0)
                {
                    MessageBox.Show("Tidak dapat melakukan perubahan harga karena ada booking pada tipe kamar tersebut dan periode tersebut! \n atau silakan batalkan booking tersebut.");
                }
                else
                {
                    SqlCommand sql = new SqlCommand("update Harga_Periodik set kamar_tipe_id =@a, tgl_berlaku=@b, harga=@c, harga_weekend=@d where harga_periodik_id = @e", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", selectedCar.Value);
                    sql.Parameters.AddWithValue("@b", dtPeriodik.Value.Date);
                    sql.Parameters.AddWithValue("@c", txtHargaKPeriodik.Text);
                    sql.Parameters.AddWithValue("@d", txtHargaWkPeriodik.Text);
                    sql.Parameters.AddWithValue("@e", kodeEdit);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    //setHargaPeriodik();
                    SqlDataAdapter da = new SqlDataAdapter("select hp.harga_periodik_id, kt.kamar_tipe , hp.tgl_berlaku, hp.harga, hp.harga_weekend from Harga_Periodik hp inner join Kamar_Tipe kt on hp.kamar_tipe_id=kt.kamar_tipe_id where year(hp.tgl_berlaku)>2008 and hp.kamar_tipe_id=@a", koneksi.KoneksiDB());
                    da.SelectCommand.Parameters.AddWithValue("@a", selectedCar.Value);
                    dHargaPeriodik = new DataTable();
                    da.Fill(dHargaPeriodik);
                    dataGridView7.DataSource = dHargaPeriodik;

                    dataGridView7.Columns[0].DisplayIndex = 4;
                    koneksi.closeConnection();

                    setHarga();
                }
            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }
          
        }

        private void dtFindPeriodik_ValueChanged(object sender, EventArgs e)
        {
            SqlDataAdapter das = new SqlDataAdapter("select * from Harga_Periodik where convert(date,tgl_berlaku)>=@a", koneksi.KoneksiDB());
            das.SelectCommand.Parameters.AddWithValue("@a", dtFindPeriodik.Value.Date);
            dHargaPeriodik = new DataTable();
            das.Fill(dHargaPeriodik);
            dataGridView7.DataSource = dHargaPeriodik;
            koneksi.closeConnection();
        }

        private void dataGridView7_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                /*SqlCommand sql = new SqlCommand("select harga_periodik_id from Harga_Periodik where tgl_berlaku=@b and harga=@c and harga_weekend=@d ", koneksi.KoneksiDB());
                //sql.Parameters.AddWithValue("@a", dataGridView7.Rows[e.RowIndex].Cells[1].Value.ToString());
                sql.Parameters.AddWithValue("@b", Convert.ToDateTime(dataGridView7.Rows[e.RowIndex].Cells[2].Value.ToString()));
                sql.Parameters.AddWithValue("@c", dataGridView7.Rows[e.RowIndex].Cells[3].Value.ToString());
                sql.Parameters.AddWithValue("@d", dataGridView7.Rows[e.RowIndex].Cells[4].Value.ToString());
                kodeEdit = Int32.Parse(sql.ExecuteScalar().ToString());*/

                kodeEdit = Int32.Parse(dataGridView7.Rows[e.RowIndex].Cells[0].Value.ToString());

                koneksi.closeConnection();
                //  MessageBox.Show(dataGridView7.Rows[e.RowIndex].Cells[0].Value.ToString());
                
                dtPeriodik.Value = Convert.ToDateTime(dataGridView7.Rows[e.RowIndex].Cells[2].Value.ToString());
                txtHargaKPeriodik.Text = dataGridView7.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtHargaWkPeriodik.Text = dataGridView7.Rows[e.RowIndex].Cells[4].Value.ToString();

                cbJnsKamarPeriodik.Text = dataGridView7.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
            catch { }
        }

        private void btnTambahUserKalender_Click(object sender, EventArgs e)
        {
            DataTamuKalenderBaru.Enabled = true;
            DataTamuKalender.Visible = false;
            DataTamuKalenderBaru.Visible = true;
        }

        private void btnTambahKalender_Click(object sender, EventArgs e)
        {
            if (txtnamaKalender.Text.Equals("") || txtTeleponKalender.Text.Equals("") || cbSebutanKalender.Text.Equals(""))
            {
                MessageBox.Show("Pastikan Data Terisi");
            }
            else
            {
                SqlCommand sql = new SqlCommand("insert into Tamu(tamu,alamat,kota,telepon,email,perusahaan,tanggallahir,sebutan,gelar,noidentitas,jenisidentitas) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,@j,@k)", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", txtnamaKalender.Text);
                sql.Parameters.AddWithValue("@b", txtAlamatKalender.Text);
                sql.Parameters.AddWithValue("@c", txtKotaKalender.Text);
                sql.Parameters.AddWithValue("@d", txtTeleponKalender.Text);
                sql.Parameters.AddWithValue("@e", txtEmailKalender.Text);
                sql.Parameters.AddWithValue("@f", txtPerusahaanKalender.Text);
                sql.Parameters.AddWithValue("@g", dtpTamuKalender.Value);
                sql.Parameters.AddWithValue("@h", cbSebutanKalender.Text);
                sql.Parameters.AddWithValue("@i", cbGelarKalender.Text);
                sql.Parameters.AddWithValue("@j", txtDataTamunoidentitas.Text);
                sql.Parameters.AddWithValue("@k", cbjnsidentitastamubaru.Text);

                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                txtnamaKalender.Text = "";
                txtAlamatKalender.Text = "";
                txtKotaKalender.Text = "";
                txtTeleponKalender.Text = "";
                txtEmailKalender.Text = "";
                txtPerusahaanKalender.Text = "";
                txtDataTamunoidentitas.Text = "";
                cbjnsidentitastamubaru.Text = "";

                cbSebutanKalender.Text = "";
                cbGelarKalender.Text = "";
                DataTamuKalenderBaru.Enabled = false;
                SqlDataAdapter da = new SqlDataAdapter("select tamu_id, tamu, alamat, kota, telepon from Tamu", koneksi.KoneksiDB());
                DataTable dset = new DataTable();
                da.Fill(dset);
                dataGridView6.DataSource = dset;
                koneksi.closeConnection();

                //langsung masukkan ke checkin

                DataTamuKalender.Visible = false;
                noIDdatatamu = Int32.Parse(dataGridView6.Rows[dataGridView6.CurrentRow.Index].Cells[0].Value.ToString());
                //DateTime tanggalPesan1 = Convert.ToDateTime(TglBulan + "/" + dataGridView3.Columns[columnSelect].Name.ToString() + "/" + Tgltahun);
                DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

                int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1
                SqlCommand sqlq = new SqlCommand("select max(Reservasi.reservasi_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking')", koneksi.KoneksiDB());
                sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
                sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);

                string reservasiKamar = sqlq.ExecuteScalar().ToString();


                //MessageBox.Show("sdfsdfdsf");
                SqlCommand qwe = new SqlCommand("select checkin from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                qwe.Parameters.AddWithValue("r_id", reservasiKamar);
                DateTime qwe_checkin = Convert.ToDateTime(qwe.ExecuteScalar().ToString());

                if ((qwe_checkin.Date < DateTime.Now.Date) || (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 7))
                {
                    sql = new SqlCommand("update Reservasi set status= 'checkin', tamu_id= (select max(tamu_id) from Tamu) where reservasi_id =@id", koneksi.KoneksiDB());
                }
                else
                {
                    sql = new SqlCommand("update Reservasi set status= 'checkin', checkin=SYSDATETIME(), tamu_id= (select max(tamu_id) from Tamu) where reservasi_id =@id", koneksi.KoneksiDB());
                }
                
                //sql.Parameters.AddWithValue("@a", noIDdatatamu);


                sql.Parameters.AddWithValue("@id", reservasiKamar);
                noIDdatatamu = 0;
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0 and ik.Tipe='Rec'", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@room", NoKamarInfo);
                reader = sql.ExecuteReader();
                while (reader.Read())
                {
                    cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                    cmd.Parameters.AddWithValue("@b", "HK");
                    cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                    cmd.Parameters.AddWithValue("@d", "R");
                    cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                    cmd.Parameters.AddWithValue("@f", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
                koneksi.closeConnection();

                panel4.Visible = false;
                opsistatusbookingkamar = 0;
                loadKalender(TglBulan, Tgltahun);



                //langsung masukkan ke checkin
                DataTamuKalenderBaru.Visible = false;
                DataTamuKalender.Visible = true;
            }
            
        }

        private void GridView_dataItem_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            item_id = (int)GridView_dataItem[0, e.RowIndex].Value;
            input_namaItem.Text = GridView_dataItem[1, e.RowIndex].Value.ToString();

            cb_tipeItem.Text = GridView_dataItem[2, e.RowIndex].Value.ToString(); ;

            input_hargaItem.Text = GridView_dataItem[3, e.RowIndex].Value.ToString();
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnDelR_Click(object sender, EventArgs e)
        {
            string checkNama = "";
            //string strJabatan = cmbJbtR.Text;
            string strJabatan = txtJbtR.Text;
            //connecting();
            //conn.Open();
            string strQ1 = "select jabatan from Jabatan where jabatan = @nm";
            cmd1 = new SqlCommand(strQ1, koneksi.KoneksiDB());
            cmd1.Parameters.AddWithValue("@nm", strJabatan);
            reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                checkNama = reader.GetString(0);
            }
            if (checkNama == "")
            {
                MessageBox.Show("Masukkan Jabatan !");
            }
            else
            {
                string strQueryDel = "delete from jabatan where jabatan = @jab";
                SqlCommand cmd = new SqlCommand(strQueryDel, koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@jab", strJabatan);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();
                MessageBox.Show("Data Rights Telah Dihapus");
                string strQ = "select * from jabatan";
                createTblNoParam(strQ);
            }
        }

        private void dgR_Click(object sender, EventArgs e)
        {
            // boxId = dgR.Rows[dgR.CurrentRow.Index].Cells[0].Value.GetType(Int32());
            boxJbt = dgR.Rows[dgR.CurrentRow.Index].Cells[1].Value.ToString();
            boxKmrTersedia = dgR.Rows[dgR.CurrentRow.Index].Cells[2].Value.ToString();
            boxKalender = dgR.Rows[dgR.CurrentRow.Index].Cells[3].Value.ToString();
            boxStatus = dgR.Rows[dgR.CurrentRow.Index].Cells[4].Value.ToString();
            boxSelesai = dgR.Rows[dgR.CurrentRow.Index].Cells[5].Value.ToString();
            boxDafTamu = dgR.Rows[dgR.CurrentRow.Index].Cells[6].Value.ToString();
            boxStaff = dgR.Rows[dgR.CurrentRow.Index].Cells[7].Value.ToString();
            boxHak = dgR.Rows[dgR.CurrentRow.Index].Cells[8].Value.ToString();
            boxResto = dgR.Rows[dgR.CurrentRow.Index].Cells[9].Value.ToString();
            boxAturKamar = dgR.Rows[dgR.CurrentRow.Index].Cells[10].Value.ToString();
            boxInv = dgR.Rows[dgR.CurrentRow.Index].Cells[11].Value.ToString();
            boxAturHotel = dgR.Rows[dgR.CurrentRow.Index].Cells[12].Value.ToString();
            boxAturKhusus = dgR.Rows[dgR.CurrentRow.Index].Cells[13].Value.ToString();
            boxAturPeri = dgR.Rows[dgR.CurrentRow.Index].Cells[14].Value.ToString();
            boxLap = dgR.Rows[dgR.CurrentRow.Index].Cells[15].Value.ToString();
            boxBkAngus = dgR.Rows[dgR.CurrentRow.Index].Cells[16].Value.ToString();
            boxAturItem = dgR.Rows[dgR.CurrentRow.Index].Cells[17].Value.ToString();
            boxUtang = dgR.Rows[dgR.CurrentRow.Index].Cells[18].Value.ToString();
            boxRekap = dgR.Rows[dgR.CurrentRow.Index].Cells[19].Value.ToString();
            boxLaporanRestoran = dgR.Rows[dgR.CurrentRow.Index].Cells[20].Value.ToString();
            boxBatal = dgR.Rows[dgR.CurrentRow.Index].Cells[21].Value.ToString();
            //cmbJbtR.Refresh();
            //cmbJbtR.Text = "";
            //cmbJbtR.SelectedText = boxJbt;
            txtJbtR.Text = boxJbt;
            if (boxKmrTersedia == "On") cKamarTersedia.Checked = true; else cKamarTersedia.Checked = false;
            if (boxKalender == "On") cKalenderBooking.Checked = true; else cKalenderBooking.Checked = false;
            if (boxStatus == "On") cStatusKamar.Checked = true; else cStatusKamar.Checked = false;
            if (boxSelesai == "On") cSelesaiBersih.Checked = true; else cSelesaiBersih.Checked = false;
            if (boxDafTamu == "On") cDaftarTamu.Checked = true; else cDaftarTamu.Checked = false;
            if (boxStaff == "On") cStaff.Checked = true; else cStaff.Checked = false;
            if (boxHak == "On") cHakAkses.Checked = true; else cHakAkses.Checked = false;
            if (boxResto == "On") cRestoran.Checked = true; else cRestoran.Checked = false;
            if (boxAturKamar == "On") cAturKamar.Checked = true; else cAturKamar.Checked = false;
            if (boxInv == "On") cInvoiceHistoris.Checked = true; else cInvoiceHistoris.Checked = false;
            if (boxAturHotel == "On") cAturInfoHotel.Checked = true; else cAturInfoHotel.Checked = false;
            if (boxAturKhusus == "On") cAturHargaKhusus.Checked = true; else cAturHargaKhusus.Checked = false;
            if (boxAturPeri == "On") cAturHargaPeriodik.Checked = true; else cAturHargaPeriodik.Checked = false;
            if (boxLap == "On") cLapKeuangan.Checked = true; else cLapKeuangan.Checked = false;
            if (boxBkAngus == "On") cBookingHangus.Checked = true; else cBookingHangus.Checked = false;
            if (boxAturItem == "On") cAturItem.Checked = true; else cAturItem.Checked = false;
            if (boxUtang == "On") cUtang.Checked = true; else cUtang.Checked = false;
            if (boxRekap == "On") cRekap.Checked = true; else cRekap.Checked = false;
            if (boxLaporanRestoran == "On") cLaporanRestoran.Checked = true; else cLaporanRestoran.Checked = false;
            if (boxBatal == "On") cBatalR.Checked = true; else cBatalR.Checked = false;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboboxPembayaranBooking.Enabled = true;
            if (comboBox4.SelectedItem == "Lunas")
            {
                inputPembayaran.Enabled = false;
                float diskon = 100;
                if (cb_diskon.Checked)
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=1", koneksi.KoneksiDB());
                    diskon = float.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();   
                }
                else
                {
                    if(diskonAngka.Enabled){
                        int diskonA = Int32.Parse(diskonAngka.Text);
                        float totalDiskon = (float)(diskonA * 100) / totalBiaya;
                        diskon = (100 - totalDiskon);
                    }
                    else
                    {
                        SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=2", koneksi.KoneksiDB());
                        diskon = float.Parse(s.ExecuteScalar().ToString());
                        koneksi.closeConnection();   
                    }
                }
                inputPembayaran.Text = ((int)((totalBiaya * diskon) / 100)).ToString();
            }
            else if (comboBox4.Text.Equals("OTA - Tanpa Batas Waktu"))
            {
                inputPembayaran.Enabled = false;
                float diskon = 100;
                if (cb_diskon.Checked)
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=1", koneksi.KoneksiDB());
                    diskon = float.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }
                else
                {
                    if (diskonAngka.Enabled)
                    {
                        int diskonA = Int32.Parse(diskonAngka.Text);
                        float totalDiskon = (float)(diskonA * 100) / totalBiaya;
                        diskon = (100 - totalDiskon);
                    }
                    else
                    {
                        SqlCommand s = new SqlCommand("SELECT harga * 100 FROM Booking_diskon where booking_diskon_id=2", koneksi.KoneksiDB());
                        diskon = float.Parse(s.ExecuteScalar().ToString());
                        koneksi.closeConnection();
                    }
                }
                inputPembayaran.Text = ((int)((totalBiaya * diskon) / 100)).ToString();
                comboboxPembayaranBooking.Text = "OTA (Online Travel Agent)";
                comboboxPembayaranBooking.Enabled = false;
            }
            else
            {
                inputPembayaran.Enabled = true;
                inputPembayaran.Text = "0";

            }
            
        }

        private void cb_diskon_CheckedChanged(object sender, EventArgs e)
        {
            if(comboBox4.SelectedItem == "Lunas"){
                comboBox4.SelectedIndex = -1;
                comboBox4.SelectedItem = "Lunas";
            }
            else if (comboBox4.SelectedItem == "OTA - Tanpa Batas Waktu")
            {
                comboBox4.SelectedIndex = -1;
                comboBox4.SelectedItem = "OTA - Tanpa Batas Waktu";
            }

            if(cb_diskon.Checked){
                diskonAngka.Enabled = false;
                lblRPDiskon.Enabled = false;
                DiskonPersen.Enabled = false;
            }
            else
            {
                diskonAngka.Enabled = true;
                lblRPDiskon.Enabled = true;
                DiskonPersen.Enabled = true;
            }

        }

        private void refreshGridViewDataUpdateBooking(String booking_id)
        {
            SqlDataAdapter da = new SqlDataAdapter("select reservasi_id, kamar_no, checkin, checkout, downpayment, tamu_id from reservasi where booking_id='" + booking_id + "' and status='booking'", koneksi.KoneksiDB());
            DataTable dataBooking = new DataTable();
            da.Fill(dataBooking);
            dataGridView_updateBooking.ReadOnly = true;
            dataGridView_updateBooking.DataSource = dataBooking;
            koneksi.closeConnection();
        }

        private void ubahBookingToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //DateTime tanggalPesan1 = Convert.ToDateTime(TglBulan + "/" + dataGridView3.Columns[columnSelect].Name.ToString() + "/" + Tgltahun);
            DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

            int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1
            SqlCommand sqlq = new SqlCommand("select max(Reservasi.booking_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking')", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
            sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
            string bookingKamar = sqlq.ExecuteScalar().ToString();
            koneksi.closeConnection();

            sqlq = new SqlCommand("select b.tgl_booking, t.tamu_id, t.tamu, b.booking_diskon_id from booking b inner join tamu t on t.tamu_id=b.tamu_id where b.booking_id=@bokid", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@bokid", bookingKamar);
            reader = sqlq.ExecuteReader();
            while (reader.Read())
            {
                update_namaTamu.Text = reader.GetString(2);
                update_tanggalBooking.Text = Convert.ToDateTime(reader.GetValue(0)).ToString("dd/MM/yyyy HH:mm:ss");
                if (reader.GetValue(3).ToString() == "1")
                {
                    update_bookingDiskon.Checked = true;
                }
                else
                {
                    update_bookingDiskon.Checked = false;
                }
            }
            koneksi.closeConnection();

            //MessageBox.Show(bookingKamar);
            update_bookingId.Text = bookingKamar;
            refreshGridViewDataUpdateBooking(bookingKamar);
            panelUpdateBooking.BringToFront();
            panelUpdateReservasi.Visible = false;
            panelCariKamarUpdateReservasi.Visible = false;
            panelTambahReservasi.Visible = false;

        }

        private void dataGridView_updateBooking_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //update_tglCheckIn.MinDate = DateTime.Now.Date;
            update_tglCheckOut.MinDate = update_tglCheckIn.Value.AddDays(1);

            if (update_reservasiId.Text != "-")
            {
                SqlCommand sqll = new SqlCommand("update reservasi set status='booking' where reservasi_id=@r_id", koneksi.KoneksiDB());
                sqll.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                sqll.ExecuteNonQuery();
                koneksi.closeConnection();
            }

            SqlCommand sql = new SqlCommand("select checkin, checkout, kamar_no, downpayment from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@r_id", dataGridView_updateBooking[0, e.RowIndex].Value.ToString());
            reader = sql.ExecuteReader();
            while (reader.Read())
            {
                update_tglCheckIn.Value = Convert.ToDateTime(reader.GetValue(0));
                update_tglCheckOut.Value = Convert.ToDateTime(reader.GetValue(1));
                update_nokamar.Text = reader.GetValue(2).ToString();
                update_downpayment.Text = reader.GetValue(3).ToString();
            }
            update_reservasiId.Text = dataGridView_updateBooking[0, e.RowIndex].Value.ToString();
            koneksi.closeConnection();

            sql = new SqlCommand("update reservasi set status='checkout' where reservasi_id=@r_id", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            panelUpdateReservasi.Visible = true;
            panelTambahReservasi.Visible = false;
            panelUpdateReservasi.BringToFront();

            flowLayoutPanel2.Enabled = false;
            keluarToolStripMenuItem.Enabled = false;
            btn_tambahReservasi.Enabled = false;
        }

        private void btn_batalUpdateReservasi_Click(object sender, EventArgs e)
        {
            SqlCommand sql = new SqlCommand("update reservasi set status='booking' where reservasi_id=@r_id", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
            sql.ExecuteNonQuery();
            panelUpdateReservasi.Visible = false;
            update_reservasiId.Text = "-";
            flowLayoutPanel2.Enabled = true;
            btn_tambahReservasi.Enabled = true;
            keluarToolStripMenuItem.Enabled = true;
            panelCariKamarUpdateReservasi.Visible = false;
        }

        private void btn_cariUpdateReservasi_Click(object sender, EventArgs e)
        {

            panelCariKamarUpdateReservasi.Visible = true;
            panelCariKamarUpdateReservasi.BringToFront();
            panelCariKamarUpdateReservasi.Controls.Clear();

            cmd = new SqlCommand((@"select count(*) from Kamar"), koneksi.KoneksiDB());

            int jumKamar = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();

            Button[] Kamar;

            cmd = new SqlCommand(
            (@"
            select k.kamar_no,kt.kamar_tipe,kt.warna,
            case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga 
            from 
            (
	            select distinct kamar_no
	            from 
	            Reservasi r
                where 
	                (
                        (r.checkin >= @checkindate
	                    and
	                    r.checkout <=@checkoutdate
	                    )
	                    or
	                    (
	                    r.checkin <= @checkindate
	                    and
	                    r.checkout >=@checkoutdate
	                    )
	                    or 
	                    (
	                    r.checkin >= @checkindate
	                    and
	                    r.checkin < @checkoutdate
	                    )
	                    or 
	                    (
	                    r.checkout > @checkindate
	                    and
	                    r.checkout <=@checkoutdate
	                    )
                    )
                    and r.status in ('booking','checkin') 
            )a
            full join
            Kamar k
            on a.kamar_no = k.kamar_no 
            inner join 
            kamar_tipe kt on k.kamar_tipe_id = kt.kamar_tipe_id inner join harga h on h.tanggal_id = '2014-1-1'
            and kt.kamar_tipe_id = h.kamar_tipe_id
			where a.kamar_no is null and k.status is null 
            "), koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@checkindate", update_tglCheckIn.Value.ToString("yyyy-M-d"));
            cmd.Parameters.AddWithValue("@checkoutdate", update_tglCheckOut.Value.ToString("yyyy-M-d"));

            reader = cmd.ExecuteReader();
            Kamar = new Button[jumKamar];
            x = 0;
            while (reader.Read())
            {
                Kamar[x] = new Button();
                Kamar[x].Text = reader.GetInt32(0).ToString();
                Kamar[x].Name = reader.GetInt32(0).ToString();
                Kamar[x].Visible = true;
                //Kamar[x].Height = 35;
                Kamar[x].Height = 45;
                Kamar[x].Width = 95;
                Kamar[x].FlatStyle = FlatStyle.Flat;
                Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
                Kamar[x].ImageAlign = btnBooking.ImageAlign;
                
                Kamar[x].Tag = reader.GetDouble(3).ToString();
                //Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                try
                {
                    Kamar[x].BackColor = Color.FromArgb(Int32.Parse(reader.GetString(2)));
                }
                catch
                {
                    Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                }
                Kamar[x].Click += new EventHandler(ubah_kamarUpdateReservasi);
                panelCariKamarUpdateReservasi.Controls.Add(Kamar[x]);
                x += 1;
            }
            koneksi.closeConnection();
        }

        private void ubah_kamarUpdateReservasi(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            update_nokamar.Text = btn.Text;
            tambah_nokamar.Text = btn.Text;

            panelCariKamarUpdateReservasi.Visible = false;
        }

        private void panelUpdateReservasi_Click(object sender, EventArgs e)
        {
            panelCariKamarUpdateReservasi.Visible = false;
        }

        private void panelUpdateBooking_Click(object sender, EventArgs e)
        {
            SqlCommand sql = new SqlCommand("update reservasi set status='booking' where reservasi_id=@r_id", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
            sql.ExecuteNonQuery();
            update_reservasiId.Text = "-";
            flowLayoutPanel2.Enabled = true;
            keluarToolStripMenuItem.Enabled = true;
            panelCariKamarUpdateReservasi.Visible = false;
            panelUpdateReservasi.Visible = false;
            panelTambahReservasi.Visible = false;
            btn_tambahReservasi.Enabled = true;
        }

        private void btn_simpanUpdateReservasi_Click(object sender, EventArgs e)
        {
            panelCariKamarUpdateReservasi.Visible = false;
            if (update_nokamar.Text == "-")
            {
                MessageBox.Show("Pilih no kamar!");
            }
            else
            {
                SqlCommand sql = new SqlCommand("update reservasi set status='booking' where reservasi_id=@r_id", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                SqlCommand sqlC = new SqlCommand("select kamar_tipe_id from Kamar where kamar_no = @no", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@no", update_nokamar.Text);
                string nilai = sqlC.ExecuteScalar().ToString();
                koneksi.closeConnection();

                sqlC = new SqlCommand("select tag_kamar from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                int biayaKamarLama = Int32.Parse(sqlC.ExecuteScalar().ToString());
                koneksi.KoneksiDB();


                sqlC = new SqlCommand("select case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end hargaK from Harga where kamar_tipe_id = @tipe and tanggal_id >= @chin and tanggal_id< @chou", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@tipe", Int32.Parse(nilai));
                sqlC.Parameters.AddWithValue("@chin", update_tglCheckIn.Value.Date);
                sqlC.Parameters.AddWithValue("@chou", update_tglCheckOut.Value.Date);
                SqlDataReader readC = sqlC.ExecuteReader();
                int biayaKamarBaru = 0;
                while (readC.Read())
                {
                    biayaKamarBaru += Int32.Parse(readC["hargaK"].ToString());
                }
                koneksi.KoneksiDB();

                //MessageBox.Show("Biaya Lama" + biayaKamarLama);
                //MessageBox.Show("Biaya Baru" + biayaKamarBaru);

                SqlCommand cmd = new SqlCommand("update reservasi set checkin=@checkin, checkout=@checkout, realcheckout=@checkout, kamar_no=@kmrno, tag_kamar=@tag_kamar where reservasi_id=@r_id", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@kmrno", update_nokamar.Text);
                cmd.Parameters.AddWithValue("@tag_kamar", biayaKamarBaru);
                cmd.Parameters.Add("@checkin", SqlDbType.DateTime).Value = update_tglCheckIn.Value.Date;
                cmd.Parameters.Add("@checkout", SqlDbType.DateTime).Value = update_tglCheckOut.Value.Date;
                cmd.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                float diskon = 100;
                //if (update_bookingDiskon.Checked)
                //{
                    SqlCommand s = new SqlCommand("SELECT bd.harga*100 FROM Booking_diskon bd inner join booking b on b.booking_diskon_id=bd.booking_diskon_id where booking_id=@b_id", koneksi.KoneksiDB());
                    s.Parameters.AddWithValue("@b_id", update_bookingId.Text);
                    diskon = float.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                /*}
                else
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=2", koneksi.KoneksiDB());
                    diskon = Int32.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }*/

                cmd = new SqlCommand("update booking set tag_kamar-=@tag_kamar, grand_total-=@grand_total, balance_due-=@balance_due where booking_id=@bokid", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("tag_kamar", biayaKamarLama - biayaKamarBaru);
                cmd.Parameters.AddWithValue("grand_total", biayaKamarLama - biayaKamarBaru);
                biayaKamarLama = (int)(biayaKamarLama * diskon) / 100;
                biayaKamarBaru = (int)(biayaKamarBaru * diskon) / 100;
                cmd.Parameters.AddWithValue("balance_due", biayaKamarLama - biayaKamarBaru);
                cmd.Parameters.AddWithValue("@bokid", update_bookingId.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                panelUpdateReservasi.Visible = false;
                refreshGridViewDataUpdateBooking(update_bookingId.Text);
                update_reservasiId.Text = "-";
                flowLayoutPanel2.Enabled = true;
                btn_tambahReservasi.Enabled = true;
                keluarToolStripMenuItem.Enabled = true;
            }
        }

        private void update_tglCheckOut_ValueChanged(object sender, EventArgs e)
        {
            update_nokamar.Text = "-";
        }

        private void update_tglCheckIn_ValueChanged(object sender, EventArgs e)
        {
            update_nokamar.Text = "-";
            update_tglCheckOut.MinDate = update_tglCheckIn.Value.AddDays(1);
        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void txtHargaLaundry_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbKriteriaCari_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
           // PanelInfoKalender.Visible = false;
        }

        private void dataGridView3_MouseEnter(object sender, EventArgs e)
        {
            cek_kamarTersedia = false;
        }

        private void dataGridView3_MouseLeave(object sender, EventArgs e)
        {
            cek_kamarTersedia = true;
        }

        private void cb_inputNoKamar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_inputNoKamar.Text == "-")
            {
                comboboxPembayaran.SelectedIndex = 0;
                comboboxPembayaran.Enabled = false;
            }else{
                comboboxPembayaran.Enabled = true;
            }
        }

        private void dataGrid_hargaKhusus_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                lbl_hargaKhususId.Text = dataGrid_hargaKhusus[0, e.RowIndex].Value.ToString();

                dateTime_tglBerlaku.Value = Convert.ToDateTime(dataGrid_hargaKhusus[2, e.RowIndex].Value);
                dateTime_tglBerakhir.Value = Convert.ToDateTime(dataGrid_hargaKhusus[3, e.RowIndex].Value);
                input_hargaNormal.Text = dataGrid_hargaKhusus[4, e.RowIndex].Value.ToString();
                input_hargaWeekend.Text = dataGrid_hargaKhusus[5, e.RowIndex].Value.ToString();

                combobox_kamar.Text = dataGrid_hargaKhusus[1, e.RowIndex].Value.ToString();
            }
            catch { }
        }

        private void datagridTamu_RowHeaderMouseClick(object sender, DataGridViewCellEventArgs e)
        {
            inputNamaTamu.Text = datagridTamu[1, e.RowIndex].Value.ToString();
            inputEmail.Text = datagridTamu[5, e.RowIndex].Value.ToString();
            inputAlamat.Text = datagridTamu[2, e.RowIndex].Value.ToString();
            inputKota.Text = datagridTamu[3, e.RowIndex].Value.ToString();
            input_perusahaan.Text = datagridTamu[6, e.RowIndex].Value.ToString();
            inputTelepon.Text = datagridTamu[4, e.RowIndex].Value.ToString();

            inputSebutan.Text = datagridTamu[7, e.RowIndex].Value.ToString();
            inputGelar.Text = datagridTamu[8, e.RowIndex].Value.ToString();
            txtNoIdentitas.Text = datagridTamu[9, e.RowIndex].Value.ToString();
            txtWargaNegara.Text  = datagridTamu[11, e.RowIndex].Value.ToString();
            cbJnsIdentitas.Text = datagridTamu[10, e.RowIndex].Value.ToString();
            if (input_perusahaan.Text == "")
            {
                cb_diskon.Checked = false;
            }
            else
            {
                cb_diskon.Checked = true;
            }
            //MessageBox.Show(datagridTamu[14, e.RowIndex].Value.ToString());
            //irwan tambahkan
            dataCustomer = Int32.Parse(datagridTamu[14, e.RowIndex].Value.ToString());
            //end irwan
            //}
            groupBukuTamu.Visible = false;
            groupBukuTamu.SendToBack();

            btnKonfirmasiBooking.Enabled = true;

        }

        private void dataGridView_updateBooking_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //update_tglCheckIn.MinDate = DateTime.Now.Date;
            update_tglCheckOut.MinDate = update_tglCheckIn.Value.AddDays(1);

            if (update_reservasiId.Text != "-")
            {
                SqlCommand sqll = new SqlCommand("update reservasi set status='booking' where reservasi_id=@r_id", koneksi.KoneksiDB());
                sqll.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                sqll.ExecuteNonQuery();
                koneksi.closeConnection();
            }

            SqlCommand sql = new SqlCommand("select checkin, checkout, kamar_no, downpayment from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@r_id", dataGridView_updateBooking[0, e.RowIndex].Value.ToString());
            reader = sql.ExecuteReader();
            while (reader.Read())
            {
                update_tglCheckIn.Value = Convert.ToDateTime(reader.GetValue(0));
                update_tglCheckOut.Value = Convert.ToDateTime(reader.GetValue(1));
                update_nokamar.Text = reader.GetValue(2).ToString();
                update_downpayment.Text = reader.GetValue(3).ToString();
            }
            update_reservasiId.Text = dataGridView_updateBooking[0, e.RowIndex].Value.ToString();
            koneksi.closeConnection();

            sql = new SqlCommand("update reservasi set status='checkout' where reservasi_id=@r_id", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            panelUpdateReservasi.Visible = true;
            panelTambahReservasi.Visible = false;
            panelUpdateReservasi.BringToFront();

            flowLayoutPanel2.Enabled = false;
            btn_tambahReservasi.Enabled = false;
            keluarToolStripMenuItem.Enabled = false;
        
        }

        private void dataGridView5_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            xPenang.Text = dataGridView5[0, e.RowIndex].Value.ToString();
            label15.Text = dataGridView5[1, e.RowIndex].Value.ToString();
            label3.Text = dataGridView5[3, e.RowIndex].Value.ToString();
            txtHargaLaundry.Text = dataGridView5[3, e.RowIndex].Value.ToString();
            txtJumlahPesanItem.Text = "1";
            panel2.Visible = false;

        }
        
        private void GridViewDaftarTamu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                SqlCommand cmd = new SqlCommand((@"select * from Tamu where tamu_id=@tamu_id"), koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@tamu_id", GridViewDaftarTamu[14, e.RowIndex].Value.ToString());
                tamu_id = GridViewDaftarTamu[14, e.RowIndex].Value.ToString();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    inputNamaDT.Text = reader.GetValue(1).ToString();
                    inputAlamatDT.Text = reader.GetValue(2).ToString();
                    inputKotaDT.Text = reader.GetValue(3).ToString();
                    inputTlpnDT.Text = reader.GetValue(4).ToString();
                    inputEmailDT.Text = reader.GetValue(5).ToString();
                    inputPerusahaanDT.Text = reader.GetValue(6).ToString();
                    if (reader.GetValue(7) != DBNull.Value)
                    {
                        //Console.WriteLine(reader.GetValue(7));
                        inputTglLhrDT.Value = Convert.ToDateTime(reader.GetValue(7));
                    }
                    else
                    {
                        inputTglLhrDT.Value = Convert.ToDateTime("1900-1-1 16:58:00"); ;
                    }
                    inputSebutanDT.Text = reader.GetValue(8).ToString();
                    inputGelarDT.Text = reader.GetValue(9).ToString();
                    txtNoIdentitasPanelTamu.Text = reader["noidentitas"].ToString();
                    cbJenisIdentitasPanelTamu.Text = reader["jenisidentitas"].ToString();
                    txtwntambah.Text = reader["warganegara"].ToString();
                }
                koneksi.closeConnection();
            }
            catch { }

        }

        private void GridViewItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                inputIdItem.Text = GridViewItem[0, e.RowIndex].Value.ToString();
                namaItem.Text = GridViewItem[1, e.RowIndex].Value.ToString();
                HargaItem.Text = GridViewItem[3, e.RowIndex].Value.ToString();
                //panelCariItem.Visible = false;
                //btn_Tambah_Item_Click(sender,);
                btn_addItem_Click(sender, e);
            }
            catch { }
        }

        private void GridView_dataItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                item_id = (int)GridView_dataItem[0, e.RowIndex].Value;
                input_namaItem.Text = GridView_dataItem[1, e.RowIndex].Value.ToString();

                cb_tipeItem.Text = GridView_dataItem[2, e.RowIndex].Value.ToString(); ;

                input_hargaItem.Text = GridView_dataItem[3, e.RowIndex].Value.ToString();
            }
            catch
            {
            }
        }

        private void btnPengaturanHarga_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();
            refreshActivatedButton();
            btnPeriodik_Click(sender, e);
            //refreshActivatedButton();
            //btnPengaturanHarga.FlatAppearance.BorderColor = Color.DodgerBlue;
            //btnPengaturanHarga.FlatAppearance.BorderSize = 2;
            //btnKalender_Click(sender, e);                        
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            HideBtnStatusKamar();
            btnPeriodik.Visible = true;
            btn_harga_khusus.Visible = true;
            dKamarPesan.Clear(); //tambahBaru

            //panelKamarDibooking.Visible = true;
            //comboboxPembayaranBooking.Text = comboboxPembayaranBooking.Items[0].ToString();
            comboBox4.Items.Clear();

            //refresh_kamar();
            panelPengaturanKamar.SendToBack();
            panelKamarDibooking.Controls.Clear();
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel4.Visible = false;
            groupBukuTamu.SendToBack();
            //panelDataTamu.Enabled = true;
            groupBukuTamu.Refresh();
            panelDataTamu.Refresh();
            groupBukuTamu.Visible = false;
            //groupBox2.Refresh();
            //inputNamaTamu.Text = "";
            //groupBukuTamu.Invalidate();
            //groupBukuTamu.Update();
            //groupBukuTamu.Refresh();
            //Application.DoEvents();
            KosongkanInput();
            hidepanelPengaturanKamar();
        }

        private void btnCheckInStatus_Click(object sender, EventArgs e)
        {
            resetBtnKonfigurasi();
            resetBtnLaporan();
            flowLayoutPanel4.Visible = false;

            refreshActivatedButton();
            btnCheckInStatus.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnCheckInStatus.FlatAppearance.BorderSize = 2;
            
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            hidepanelPengaturanKamar();
            PanelPesan.BringToFront();
            PanelPesan.Controls.Clear();
            
            SqlCommand cmd = new SqlCommand((@"select count(*) from Kamar, Reservasi where Kamar.kamar_no = Reservasi.kamar_no and Reservasi.status = 'checkin'"), koneksi.KoneksiDB());

            int jumKamar = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();
            ///button1.Text = jumKamar.ToString();

            //command.Parameters.AddWithValue("@Username", username);
            //command.Parameters.AddWithValue("@Password", password);

            cmd = new SqlCommand(
            (@"
            select Kamar.kamar_no, Reservasi.checkout 
            from Kamar, Reservasi 
            where Kamar.kamar_no = Reservasi.kamar_no 
            and Reservasi.status = 'checkin'
            order by kamar.kamar_no desc
            "), koneksi.KoneksiDB());

            /*
             
             cmd = new SqlCommand(
            (@"select
            k.kamar_no,
            k.kamar_tipe_id,
            case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga
            from            
            Kamar k
            inner join kamar_tipe kt on k.kamar_tipe_id = kt.kamar_tipe_id 
            inner join harga h on h.tanggal_id = '2008-7-1'
            and kt.kamar_tipe_id = h.kamar_tipe_id"), koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@checkindate",checkinDate.Value.ToString("yyyy-M-d"));
            //cmd.Parameters.AddWithValue("@checkoutdate",checkoutDate.Value.ToString("yyyy-M-d"));
             */

            String baruString = "";

            reader = cmd.ExecuteReader();
            Kamar = new Button[jumKamar];
            x = 0; JumKamarHigh = 0;
            while (reader.Read())
            {
                Kamar[x] = new Button();
                Kamar[x].Text = reader.GetInt32(0).ToString();
                Kamar[x].Name = reader.GetInt32(0).ToString();
                Kamar[x].Visible = true;
                //Kamar[x].Height = 35;
                //Kamar[x].Tag = 0;
                //Kamar[x].BackColor = Color.FromName(reader.GetString(1));
                Kamar[x].Click += new EventHandler(MunculKan);
                Kamar[x].MouseEnter += new EventHandler(tooltipshow);
                Kamar[x].MouseLeave += new EventHandler(tooltipclose);
                //Kamar[x].MouseEnter += new EventHandler(button1_MouseEnter_2);
                //Kamar[x].MouseLeave += new EventHandler(button1_MouseLeave_1);
                Kamar[x].Height = 45;
                Kamar[x].Width = 95;
                Kamar[x].FlatStyle = FlatStyle.Flat;
                Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
                Kamar[x].ImageAlign = btnBooking.ImageAlign;

                DateTime dateOut = Convert.ToDateTime(reader.GetValue(1));
                if (dateOut.Date <= DateTime.Now.Date && DateTime.Now.Hour >=8 )
                {
                    Kamar[x].BackColor = Color.Red;
                    Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.ED;
                }

                if (baruString.Equals(""))
                {
                    baruString = Kamar[x].Name.ToString().Substring(0, 1);
                }
                if (!Kamar[x].Name.ToString().Substring(0, 1).Equals(baruString))
                {
                    baruString = Kamar[x].Name.ToString().Substring(0, 1);
                    LinkLabel label1 = new LinkLabel();
                    label1.AutoSize = false;
                    label1.Height = 20;
                    label1.Width = PanelPesan.Width;
                    label1.BorderStyle = BorderStyle.Fixed3D;

                    PanelPesan.Controls.Add(label1);
                }
                PanelPesan.Controls.Add(Kamar[x]);
                x += 1; JumKamarHigh += 1;
                //Kamar[x].MouseEnter += button1_MouseEnter_2;// Kamar_Tips;//new EventHandler(Kamar_Tips);

            }
            //conn.Close();
            koneksi.closeConnection();
        
        }

        private void MunculKanStatusBooking(object sender, EventArgs e)
        {

            //            if (dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Red)
            //          {
            panelPembayaran.Visible = false;
            DateTime tglcheckin;
            Button btn = sender as Button;
            contextMenuStatusBooking.Show(Cursor.Position);
            dataKamarCh = Int32.Parse(btn.Text);
            noroom = Int32.Parse(btn.Text);
            //tglcheck = Convert.ToDateTime(btn.Name);
            //tes1
            // DateTime tanggalPesan1 = Convert.ToDateTime(TglBulan + "/" + dataGridView3.Columns[e.ColumnIndex].Name.ToString() + "/" + Tgltahun);

            SqlCommand sqlTgl = new SqlCommand("select checkin from Reservasi where kamar_no =@a and status='booking'", koneksi.KoneksiDB());
            sqlTgl.Parameters.AddWithValue("@a", Int32.Parse(btn.Text));
            SqlDataReader readTgl = sqlTgl.ExecuteReader();
            //int inDexC = 0;
            while (readTgl.Read())
            {
                tglcheckin = Convert.ToDateTime(readTgl["checkin"].ToString());

                SqlCommand sqlC = new SqlCommand("select checkin,tamu_id from Reservasi where kamar_no =@a and status='booking' and Reservasi.checkin <=@id and Reservasi.checkout > @id", koneksi.KoneksiDB());
                // SqlCommand sqlC = new SqlCommand("select checkin from Reservasi where kamar_no =@a and status='booking'", koneksi.KoneksiDB());
                //sqlC.Parameters.AddWithValue("@a", Int32.Parse(dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString()));
                sqlC.Parameters.AddWithValue("@a", Int32.Parse(btn.Text));
                sqlC.Parameters.AddWithValue("@id", tglcheckin);
                SqlDataReader readC = sqlC.ExecuteReader();
                int inDexC = 0;
                while (readC.Read())
                {
                    tglcheck = Convert.ToDateTime(readC["checkin"].ToString());
                    opsistatusbookingkamar = 1;
                    noidtamu = Convert.ToInt32(readC["tamu_id"].ToString());
                    if (readC["checkin"].ToString().Equals(DateTime.Now.Date.ToString()))
                    {

                        inDexC = 1;
                    }
                }
                if (inDexC < 1)
                {
                    // checkInToolStripMenuItem.Visible = false;
                    CheckIntoolStripMenuItem2.Visible = false;
                }
                else
                {
                    //checkInToolStripMenuItem.Visible = true;
                    CheckIntoolStripMenuItem2.Visible = true;
                }
            }
            //    rowSelect = e.RowIndex;
            //   columnSelect = e.ColumnIndex;
            //        }





            //panelPembayaran.Visible = false;

            //Button btn = sender as Button;
            //contextMenuStrip2.Show(Cursor.Position);
            //dataKamarCh = Int32.Parse(btn.Text);
            //SqlCommand sql = new SqlCommand("select reservasi_id,booking_id from Reservasi where kamar_no =@noKamar and status='booking' ", koneksi.KoneksiDB());

            ////SqlCommand sql = new SqlCommand("select reservasi_id,booking_id from Reservasi where kamar_no =@noKamar and convert(date,@tnggal)>=convert(date,checkin) and convert(date,@tnggal) <= convert(date,checkout) and status='checkin' ", koneksi.KoneksiDB());
            //sql.Parameters.AddWithValue("@noKamar", Int32.Parse(btn.Text));
            ////sql.Parameters.AddWithValue("@tnggal", DateTime.Now);
            //string idReservasi = "-";
            //string booking_id = "-";
            //SqlDataReader readZ = sql.ExecuteReader();

            //while (readZ.Read())
            //{
            //    try
            //    {
            //        idReservasi = readZ["reservasi_id"].ToString();
            //        booking_id = readZ["booking_id"].ToString();
            //    }
            //    catch
            //    {
            //        idReservasi = "-";
            //    }
            //}

            //koneksi.closeConnection();
            //sql = new SqlCommand("select statusbayar from Booking where booking_id=@a", koneksi.KoneksiDB());
            //sql.Parameters.AddWithValue("@a", booking_id);
            //string statusA = sql.ExecuteScalar().ToString();
            //koneksi.closeConnection();
            //if (statusA.Equals(""))
            //{
            //    checkOutToolStripMenuItem.Visible = true;
            //    printInvoiceKamarToolStripMenuItem.Visible = true;
            //}
            //else
            //{
            //    checkOutToolStripMenuItem.Visible = false;
            //    printInvoiceKamarToolStripMenuItem.Visible = false;

            //}
            //label22.Text = idReservasi;
            //dtPesan = new DataTable();

            //dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //dtPesan.Columns.Add("ID_ITEM".ToString());
            //dtPesan.Columns.Add("RESERVASI_ID".ToString());
            //dtPesan.Columns.Add("TANGGAL", typeof(DateTime));
            //dtPesan.Columns.Add("HARGA".ToString());
            //dataGridView4.DataSource = dtPesan;

        }

        public void tooltipshowstatusbooking(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            SqlCommand sqlq = new SqlCommand("select Tamu.tamu, Reservasi.checkin, Reservasi.checkout,Tamu.alamat,Tamu.kota,Tamu.telepon,Tamu.email from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and Reservasi.kamar_no=@nok and ( Reservasi.status='booking')", koneksi.KoneksiDB());
            // sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
            sqlq.Parameters.AddWithValue("@nok", btn.Text);
            reader = sqlq.ExecuteReader();

            while (reader.Read())
            {

                toolTip1.Show("----------------------------------------------------------------------------\r\n" +
        "                        Dibooking oleh " + reader.GetString(0) + "\r\n" +
        "----------------------------------------------------------------------------\r\n" +
          "Alamat " + reader["alamat"].ToString() + " Kota " + reader["kota"].ToString() + " No Telepon " + reader["telepon"].ToString() + " Email " + reader["email"].ToString() + "\r\n" +
                    //"Alamat " + reader.GetString(3) + " Kota " + reader.GetString(4) + " Email " + reader.GetString(6) + "\r\n" +
          "-----------------------------------------------------------------------------\r\n" +
        "Kamar " + btn.Text.ToString() + " Checkin " + reader.GetDateTime(1).ToString("dd/MMM/yyyy") + " Checkout " + reader.GetDateTime(2).ToString("dd/MMM/yyyy") + "\r\n" +
        "----------------------------------------------------------------------------\r\n"
        ,
                btn);

            }
            koneksi.closeConnection();
            SqlCommand sql = new SqlCommand("select reservasi_id,booking_id from Reservasi where kamar_no =@noKamar and status='booking' ", koneksi.KoneksiDB());
            //SqlCommand sql = new SqlCommand("select reservasi_id,booking_id from Reservasi where kamar_no =@noKamar and convert(date,@tnggal)>=convert(date,checkin) and convert(date,@tnggal) <= convert(date,checkout) and status='checkin' ", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@noKamar", Int32.Parse(btn.Text));
            //sql.Parameters.AddWithValue("@tnggal", DateTime.Now);
            string booking_id = "-";
            SqlDataReader readZ = sql.ExecuteReader();
            while (readZ.Read())
            {
                booking_id = readZ["booking_id"].ToString();
            }

            koneksi.closeConnection();


            sql = new SqlCommand("select kamar_no from Reservasi where booking_id=@a", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", booking_id);
            SqlDataReader readKamarno = sql.ExecuteReader();
            while (readKamarno.Read())
            {
                for (int i = 0; i < JumKamarHigh; i++)
                {
                    if (Kamar[i].Text.Equals(readKamarno["kamar_no"].ToString()))
                    {

                        Kamar[i].FlatStyle = FlatStyle.Flat;
                        Kamar[i].FlatAppearance.BorderColor = Color.Yellow;
                        Kamar[i].FlatAppearance.BorderSize = 2;
                    }
                }
            }
            koneksi.closeConnection();

            Thread.Sleep(500);
        }



        public void tooltipclosestatusbooking(object sender, EventArgs e)
        {

            Button btn = sender as Button;
            toolTip1.Hide(btn);
            for (int i = 0; i < JumKamarHigh; i++)
            {
                Kamar[i].FlatStyle = FlatStyle.Flat;
                Kamar[i].FlatAppearance.BorderColor = Color.Black;
                Kamar[i].FlatAppearance.BorderSize = 1;
            }
        }

        private void btnBookingStatus_Click(object sender, EventArgs e)
        {
            btnBookingStatus.Visible = true;
            btnCheckInStatus.Visible = true;
            btnKamarMaintenance.Visible = true;
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            hidepanelPengaturanKamar();
            panelStatusBooking.BringToFront();
            panelStatusBooking.Controls.Clear();
            SqlCommand cmd = new SqlCommand((@"select count(*) from Kamar, Reservasi where Kamar.kamar_no = Reservasi.kamar_no and Reservasi.status = 'booking'"), koneksi.KoneksiDB());

            int jumKamar = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();
            ///button1.Text = jumKamar.ToString();

            //command.Parameters.AddWithValue("@Username", username);
            //command.Parameters.AddWithValue("@Password", password);

            cmd = new SqlCommand(
            (@"
            select Kamar.kamar_no, Reservasi.checkout, Reservasi.checkin 
            from Kamar, Reservasi 
            where Kamar.kamar_no = Reservasi.kamar_no 
            and Reservasi.status = 'booking'
            order by kamar.kamar_no
            "), koneksi.KoneksiDB());

            /*
             
             cmd = new SqlCommand(
            (@"select
            k.kamar_no,
            k.kamar_tipe_id,
            case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga
            from            
            Kamar k
            inner join kamar_tipe kt on k.kamar_tipe_id = kt.kamar_tipe_id 
            inner join harga h on h.tanggal_id = '2008-7-1'
            and kt.kamar_tipe_id = h.kamar_tipe_id"), koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@checkindate",checkinDate.Value.ToString("yyyy-M-d"));
            //cmd.Parameters.AddWithValue("@checkoutdate",checkoutDate.Value.ToString("yyyy-M-d"));
             */


            reader = cmd.ExecuteReader();
            Kamar = new Button[jumKamar];
            x = 0; JumKamarHigh = 0;
            while (reader.Read())
            {
                Kamar[x] = new Button();
                Kamar[x].Text = reader.GetInt32(0).ToString();
                Kamar[x].Name = reader.GetInt32(0).ToString();
                //Kamar[x].Name = reader.GetInt32(2).ToString();
                //Kamar[x].Name = Convert.ToDateTime(reader.GetValue(2));

                Kamar[x].Visible = true;
                Kamar[x].Height = 45;
                Kamar[x].Width = 95;
                Kamar[x].FlatStyle = FlatStyle.Flat;
                Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
                Kamar[x].ImageAlign = btnBooking.ImageAlign;
                //Kamar[x].Tag = 0;
                //Kamar[x].BackColor = Color.FromName(reader.GetString(1));
                Kamar[x].Click += new EventHandler(MunculKanStatusBooking);
                Kamar[x].MouseEnter += new EventHandler(tooltipshowstatusbooking);
                Kamar[x].MouseLeave += new EventHandler(tooltipclosestatusbooking);
                //Kamar[x].MouseEnter += new EventHandler(button1_MouseEnter_2);
                //Kamar[x].MouseLeave += new EventHandler(button1_MouseLeave_1);
                DateTime dateOut = Convert.ToDateTime(reader.GetValue(1));
                tglcheck = Convert.ToDateTime(reader.GetValue(2));
                if (dateOut.Date <= DateTime.Now.Date)
                {
                    Kamar[x].BackColor = Color.Red;
                }

                panelStatusBooking.Controls.Add(Kamar[x]);
                x += 1; JumKamarHigh += 1;
                //Kamar[x].MouseEnter += button1_MouseEnter_2;// Kamar_Tips;//new EventHandler(Kamar_Tips);

            }
            //conn.Close();
            koneksi.closeConnection();
        
        }

        private void UbahtoolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SqlCommand sqlq = new SqlCommand("select Reservasi.booking_id from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and Reservasi.checkin <=@id and Reservasi.checkout > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking')", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@id", tglcheck);
            sqlq.Parameters.AddWithValue("@nok", noroom);
            string bookingKamar = sqlq.ExecuteScalar().ToString();
            koneksi.closeConnection();

            sqlq = new SqlCommand("select b.tgl_booking, t.tamu_id, t.tamu, b.booking_diskon_id from booking b inner join tamu t on t.tamu_id=b.tamu_id where b.booking_id=@bokid", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@bokid", bookingKamar);
            reader = sqlq.ExecuteReader();
            while (reader.Read())
            {
                update_namaTamu.Text = reader.GetString(2);
                update_tanggalBooking.Text = Convert.ToDateTime(reader.GetValue(0)).ToString("dd/MM/yyyy HH:mm:ss");
                if (reader.GetValue(3).ToString() == "1")
                {
                    update_bookingDiskon.Checked = true;
                }
                else
                {
                    update_bookingDiskon.Checked = false;
                }
            }
            koneksi.closeConnection();

            //MessageBox.Show(bookingKamar);
            update_bookingId.Text = bookingKamar;
            refreshGridViewDataUpdateBooking(bookingKamar);
            panelUpdateBooking.BringToFront();
            panelUpdateReservasi.Visible = false;
            panelTambahReservasi.Visible = false;
            panelCariKamarUpdateReservasi.Visible = false;
        
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            /*SqlCommand batalQuery = new SqlCommand("select booking_id,tag_kamar, downpayment from Reservasi where kamar_no=@a and status='booking'", koneksi.KoneksiDB());
            batalQuery.Parameters.AddWithValue("@a", noroom);
            SqlDataReader bacaQuery = batalQuery.ExecuteReader();
            int kodeBookingId = 0;
            int tagKamarBatal = 0;
            int downPaymentBatal = 0;

            while (bacaQuery.Read())
            {
                kodeBookingId = Int32.Parse(bacaQuery["booking_id"].ToString());
                tagKamarBatal = Int32.Parse(bacaQuery["tag_kamar"].ToString());
                downPaymentBatal = Int32.Parse(bacaQuery["downpayment"].ToString());
            }

            koneksi.closeConnection();

            batalQuery = new SqlCommand("update Reservasi set status='cancel', checkout=@b where kamar_no=@a and status='booking'", koneksi.KoneksiDB());
            // batalQuery.Parameters.AddWithValue("@a", dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());
            batalQuery.Parameters.AddWithValue("@a", noroom);
            batalQuery.Parameters.AddWithValue("@b", DateTime.Now);
            batalQuery.ExecuteNonQuery();
            koneksi.closeConnection();


            batalQuery = new SqlCommand("update Booking set balance_due=balance_due-@b where booking_id=@a ", koneksi.KoneksiDB());
            batalQuery.Parameters.AddWithValue("@a", kodeBookingId);
            batalQuery.Parameters.AddWithValue("@b", tagKamarBatal - downPaymentBatal);
            batalQuery.ExecuteNonQuery();
            koneksi.closeConnection();

            loadKalender(TglBulan, Tgltahun);
            refresh_kamar_status_booking();
            */
        }

        public void refresh_kamar_status_booking()
        {
            btnBookingStatus.Visible = true;
            btnCheckInStatus.Visible = true;
            btnKamarMaintenance.Visible = true;
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            hidepanelPengaturanKamar();
            panelStatusBooking.BringToFront();
            panelStatusBooking.Controls.Clear();
            SqlCommand cmd = new SqlCommand((@"select count(*) from Kamar, Reservasi where Kamar.kamar_no = Reservasi.kamar_no and Reservasi.status = 'booking'"), koneksi.KoneksiDB());

            int jumKamar = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();
            ///button1.Text = jumKamar.ToString();

            //command.Parameters.AddWithValue("@Username", username);
            //command.Parameters.AddWithValue("@Password", password);

            cmd = new SqlCommand(
            (@"
            select Kamar.kamar_no, Reservasi.checkout, Reservasi.checkin 
            from Kamar, Reservasi 
            where Kamar.kamar_no = Reservasi.kamar_no 
            and Reservasi.status = 'booking'
            order by kamar.kamar_no
            "), koneksi.KoneksiDB());

            /*
             
             cmd = new SqlCommand(
            (@"select
            k.kamar_no,
            k.kamar_tipe_id,
            case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga
            from            
            Kamar k
            inner join kamar_tipe kt on k.kamar_tipe_id = kt.kamar_tipe_id 
            inner join harga h on h.tanggal_id = '2008-7-1'
            and kt.kamar_tipe_id = h.kamar_tipe_id"), koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@checkindate",checkinDate.Value.ToString("yyyy-M-d"));
            //cmd.Parameters.AddWithValue("@checkoutdate",checkoutDate.Value.ToString("yyyy-M-d"));
             */

            
            reader = cmd.ExecuteReader();
            Kamar = new Button[jumKamar];
            x = 0; JumKamarHigh = 0;
            while (reader.Read())
            {
                Kamar[x] = new Button();
                Kamar[x].Text = reader.GetInt32(0).ToString();
                Kamar[x].Name = reader.GetInt32(0).ToString();
                Kamar[x].Visible = true;
                Kamar[x].Height = 45;
                //Kamar[x].Tag = 0;
                //Kamar[x].BackColor = Color.FromName(reader.GetString(1));
                Kamar[x].Click += new EventHandler(MunculKanStatusBooking);
                Kamar[x].MouseEnter += new EventHandler(tooltipshowstatusbooking);
                Kamar[x].MouseLeave += new EventHandler(tooltipclosestatusbooking);
                //Kamar[x].MouseEnter += new EventHandler(button1_MouseEnter_2);
                //Kamar[x].MouseLeave += new EventHandler(button1_MouseLeave_1);
                DateTime dateOut = Convert.ToDateTime(reader.GetValue(1));
                //tglcheck = Convert.ToDateTime(reader.GetValue(2));
                if (dateOut.Date <= DateTime.Now.Date)
                {
                    Kamar[x].BackColor = Color.Red;
                }

                panelStatusBooking.Controls.Add(Kamar[x]);
                x += 1; JumKamarHigh += 1;
                //Kamar[x].MouseEnter += button1_MouseEnter_2;// Kamar_Tips;//new EventHandler(Kamar_Tips);

            }
            //conn.Close();
            koneksi.closeConnection();
        }

        private void CheckIntoolStripMenuItem2_Click(object sender, EventArgs e)
        {

            SqlCommand sqlCheckin = new SqlCommand("select reservasi_id from Reservasi where kamar_no = @no and status='checkin'", koneksi.KoneksiDB());
            sqlCheckin.Parameters.AddWithValue("@no", noroom);
            SqlDataReader readCheckin = sqlCheckin.ExecuteReader();
            int CekData = 0;
            while (readCheckin.Read())
            {
                CekData += 1;
            }
            koneksi.closeConnection();

            if (CekData == 0)
            {
                panel4.Visible = true;
                panel4.BringToFront();
                DataTamuKalenderBaru.Enabled = false;

                DataTamuKalender.Visible = true;
                SqlDataAdapter da = new SqlDataAdapter("select tamu_id, tamu, alamat, kota, telepon from Tamu", koneksi.KoneksiDB());
                DataTable dset = new DataTable();
                da.Fill(dset);
                dataGridView6.DataSource = dset;
                koneksi.closeConnection();
            }
            else
            {
                MessageBox.Show("Maaf No Kamar Tersebut Belum TerCheckout");
            }


        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void label78_Click(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelKamar_Enter(object sender, EventArgs e)
        {
            //kamar_Ubah_Hapus.ActiveForm.Close();
        }

        private void panelKamar_MouseEnter(object sender, EventArgs e)
        {
            //kamar_Ubah_Hapus.ActiveForm.Close();
        }

        private void checkInBookingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Anda yakin untuk mengcheckinkan semua booking kamar ini", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1

                //DateTime tanggalPesan1 = Convert.ToDateTime(TglBulan + "/" + dataGridView3.Columns[columnSelect].Name.ToString() + "/" + Tgltahun);
                DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

                SqlCommand sqlq = new SqlCommand("select max(Reservasi.booking_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking')", koneksi.KoneksiDB());
                sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
                sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
                string reservasiKamar = sqlq.ExecuteScalar().ToString();
                koneksi.closeConnection();

                sqlq = new SqlCommand("select kamar_no, checkin, checkout from reservasi where booking_id=@id and status='booking'", koneksi.KoneksiDB());
                sqlq.Parameters.AddWithValue("@id", reservasiKamar);
                SqlDataReader reader = sqlq.ExecuteReader();
                int CekData = 0;
                int cekTgl = 0;
                int cekCheckIn = 0;
                while (reader.Read())
                {
                    SqlCommand sqlCheckin = new SqlCommand("select reservasi_id from Reservasi where kamar_no = @no and status='checkin'", koneksi.KoneksiDB());
                    sqlCheckin.Parameters.AddWithValue("@no", reader.GetValue(0).ToString());
                    SqlDataReader readCheckin = sqlCheckin.ExecuteReader();
                    while (readCheckin.Read())
                    {
                        CekData += 1;
                    }

                    DateTime tgl_checkin = Convert.ToDateTime(reader.GetValue(1));
                    DateTime tgl_checkout = Convert.ToDateTime(reader.GetValue(2));
                    if (tgl_checkin.Date > DateTime.Now.Date || tgl_checkout.Date < DateTime.Now.Date)
                    {
                        cekTgl += 1;
                    }

                    if (tgl_checkin.Date < DateTime.Now.Date)
                    {
                        cekCheckIn += 1;
                    }
                }
                koneksi.closeConnection();
                if (CekData == 0 && cekTgl == 0)
                {
                    SqlCommand sql;

                    SqlCommand sql1 = new SqlCommand(@"select kamar_no
                        from Reservasi
                        where booking_id =@id and status='booking'", koneksi.KoneksiDB());
                    sql1.Parameters.AddWithValue("@id", reservasiKamar);
                    SqlDataReader rd = sql1.ExecuteReader();
                    while (rd.Read()){
                        sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0 and ik.Tipe='Rec'", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@room", rd.GetValue(0).ToString());
                        reader = sql.ExecuteReader();
                        while (reader.Read())
                        {
                            cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                            cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                            cmd.Parameters.AddWithValue("@b", "HK");
                            cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                            cmd.Parameters.AddWithValue("@d", "R");
                            cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                            cmd.Parameters.AddWithValue("@f", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    koneksi.closeConnection();

                    if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 7)
                    {
                        sql = new SqlCommand("update Reservasi set status= 'checkin' where booking_id =@id and status='booking'", koneksi.KoneksiDB());
                    }
                    else
                    {
                        if (cekCheckIn > 0)
                        {
                            sql = new SqlCommand("update Reservasi set status= 'checkin' where booking_id =@id and status='booking'", koneksi.KoneksiDB());
                        }
                        else
                        {
                            sql = new SqlCommand("update Reservasi set status= 'checkin', checkin=SYSDATETIME() where booking_id =@id and status='booking'", koneksi.KoneksiDB());
                        }
                    }
                    sql.Parameters.AddWithValue("@id", reservasiKamar);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();
                    loadKalender(TglBulan, Tgltahun);
                }
                else
                {
                    MessageBox.Show("Terdapat NoKamar yang Belum dichekout \n Atau Tanggal checkin Harus hari ini");
                }
            }
        }

        private void pembatalantoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //checkout_kamar(sender, e, 0);
            DialogResult result = MessageBox.Show("Anda yakin untuk membatalkan reservasi kamar ini", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                SqlCommand sql = new SqlCommand("delete from reservasi where status='checkin' and kamar_no=@id", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@id", dataKamarCh);
                sql.ExecuteNonQuery();

                koneksi.closeConnection();

                btnCheckInStatus_Click(sender, e);
            }
        }

        private void gabungreservasitoolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataKamarGabung = dataKamarCh;
            targetgabungreservasitoolStripMenuItem.Enabled = true;
            ///MessageBox.Show(dataKamarGabung.ToString());
        }

        private void targetgabungreservasitoolStripMenuItem_Click(object sender, EventArgs e)
        {
            targetgabungreservasitoolStripMenuItem.Enabled = false;
            string sql = "execute sp_gabungbooking @reservasi_awal,@booking_target";
            SqlCommand cmd = new SqlCommand(sql, koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@reservasi_awal", dataKamarGabung);
            cmd.Parameters.AddWithValue("@booking_target", dataKamarCh);
            cmd.ExecuteNonQuery();
            koneksi.closeConnection();
            //MessageBox.Show(dataKamarCh.ToString());    
            /*SqlCommand sql = new SqlCommand("Select booking_id from reservasi where kamar_no =@nkamar and status='checkin'", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@nkamar", dataKamarGabung);
            sql.ExecuteNonQuery();
            
            koneksi.closeConnection();*/


        }

        private void reportLaporanPendapatan_Drillthrough(object sender, DrillthroughEventArgs e)
        {
            LocalReport report = (LocalReport)e.Report;
            String MakeId = "";
            IList<ReportParameter> list = report.OriginalParametersToDrillthrough;
            foreach (ReportParameter param in list)
            {
                //Since i know the parameter is only one and its not a multivalue 
                //I can directly fetch the first value from the Values array.
                MakeId = Convert.ToDateTime(param.Values[0]).ToString("yyyy-MM-dd");
            }
            Console.WriteLine(MakeId);

            this.infoSubPendapatan.EnforceConstraints = false;

            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.DataTableInfoSubTableAdapter.Fill(this.infoSubPendapatan.DataTableInfoSub, MakeId);
            report.DataSources.Add(new ReportDataSource("DataSetSub", (object)infoSubPendapatan.DataTableInfoSub));
            report.DataSources.Add(new ReportDataSource("infoHotel", (object)infoHotel.IDHotel));

            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            report.SetParameters(parameter);

        }


        private void inputUlangTahun_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cb_bulanLaporan_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboboxItem selectedCar = (ComboboxItem)cb_bulanLaporan.SelectedItem;
            if (cekPilihLaporan)
            {
                this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
                this.dataTable1TableAdapter1.Fill(this.infoPendapatan.DataTable1, Int32.Parse(cb_tahunLaporan.Text), Convert.ToInt32(selectedCar.Value));
                
                reportLaporanPendapatan.Reset();
                reportLaporanPendapatan.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.Laporan_Pendapatan_Harian.rdlc";
                reportLaporanPendapatan.LocalReport.DataSources.Add(new ReportDataSource("infoPendapatan", (object)infoPendapatan.DataTable1));
                reportLaporanPendapatan.LocalReport.DataSources.Add(new ReportDataSource("infoHotel", (object)infoHotel.IDHotel));
                reportLaporanPendapatan.LocalReport.EnableExternalImages = true;
                string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
                ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
                reportLaporanPendapatan.LocalReport.SetParameters(parameter);
          
                //reportLaporanPendapatan.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
                reportLaporanPendapatan.RefreshReport();
                //reportLaporanPendapatan.BringToFront();
            }
        }

        private void cb_tahunLaporan_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboboxItem selectedCar = (ComboboxItem)cb_bulanLaporan.SelectedItem;
            if (cekPilihLaporan)
            {
                this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
                this.dataTable1TableAdapter1.Fill(this.infoPendapatan.DataTable1, Int32.Parse(cb_tahunLaporan.Text), Convert.ToInt32(selectedCar.Value));
                reportLaporanPendapatan.Reset();
                reportLaporanPendapatan.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.Laporan_Pendapatan_Harian.rdlc";
                reportLaporanPendapatan.LocalReport.DataSources.Add(new ReportDataSource("infoPendapatan", (object)infoPendapatan.DataTable1));
                reportLaporanPendapatan.LocalReport.DataSources.Add(new ReportDataSource("infoHotel", (object)infoHotel.IDHotel));
                reportLaporanPendapatan.LocalReport.EnableExternalImages = true;
                string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
                ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
                reportLaporanPendapatan.LocalReport.SetParameters(parameter);
          
                //reportLaporanPendapatan.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
                reportLaporanPendapatan.RefreshReport();
                //reportLaporanPendapatan.BringToFront();
            }
        }

        private void btn_tambahReservasi_Click(object sender, EventArgs e)
        {
            panelTambahReservasi.Visible = true;
            panelTambahReservasi.BringToFront();
            tambah_tgglCheckIn.MinDate = DateTime.Today.AddDays(-1);
            tambah_tgglCheckOut.MinDate = DateTime.Today;
            tambah_tgglCheckOut.Value = DateTime.Today.AddDays(1);
        }

        private void btn_cariTambahReservasi_Click(object sender, EventArgs e)
        {
            //btn_cariUpdateReservasi_Click(sender, e);
            panelCariKamarUpdateReservasi.Visible = true;
            panelCariKamarUpdateReservasi.BringToFront();
            panelCariKamarUpdateReservasi.Controls.Clear();

            cmd = new SqlCommand((@"select count(*) from Kamar"), koneksi.KoneksiDB());

            int jumKamar = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();

            Button[] Kamar;

            cmd = new SqlCommand(
            (@"
            select k.kamar_no,kt.kamar_tipe,kt.warna,
            case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga 
            from 
            (
	            select distinct kamar_no
	            from 
	            Reservasi r
                where 
	                (
                        (r.checkin >= @checkindate
	                    and
	                    r.checkout <=@checkoutdate
	                    )
	                    or
	                    (
	                    r.checkin <= @checkindate
	                    and
	                    r.checkout >=@checkoutdate
	                    )
	                    or 
	                    (
	                    r.checkin >= @checkindate
	                    and
	                    r.checkin < @checkoutdate
	                    )
	                    or 
	                    (
	                    r.checkout > @checkindate
	                    and
	                    r.checkout <=@checkoutdate
	                    )
                    )
                    and r.status in ('booking','checkin') 
            )a
            full join
            Kamar k
            on a.kamar_no = k.kamar_no 
            inner join 
            kamar_tipe kt on k.kamar_tipe_id = kt.kamar_tipe_id inner join harga h on h.tanggal_id = '2014-1-1'
            and kt.kamar_tipe_id = h.kamar_tipe_id
			where a.kamar_no is null and k.status is null 
            "), koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@checkindate", tambah_tgglCheckIn.Value.ToString("yyyy-M-d"));
            cmd.Parameters.AddWithValue("@checkoutdate", tambah_tgglCheckOut.Value.ToString("yyyy-M-d"));

            reader = cmd.ExecuteReader();
            Kamar = new Button[jumKamar];
            x = 0;
            while (reader.Read())
            {
                Kamar[x] = new Button();
                Kamar[x].Text = reader.GetInt32(0).ToString();
                Kamar[x].Name = reader.GetInt32(0).ToString();
                Kamar[x].Visible = true;
                //Kamar[x].Height = 35;
                Kamar[x].Height = 45;
                Kamar[x].Width = 95;
                Kamar[x].FlatStyle = FlatStyle.Flat;
                Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
                Kamar[x].ImageAlign = btnBooking.ImageAlign;

                Kamar[x].Tag = reader.GetDouble(3).ToString();
                //Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                try
                {
                    Kamar[x].BackColor = Color.FromArgb(Int32.Parse(reader.GetString(2)));
                }
                catch
                {
                    Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                }
                Kamar[x].Click += new EventHandler(ubah_kamarUpdateReservasi);
                panelCariKamarUpdateReservasi.Controls.Add(Kamar[x]);
                x += 1;
            }
            koneksi.closeConnection();
        }

        private void btn_tambah_Click(object sender, EventArgs e)
        {
            if(tambah_nokamar.Text == "-"){
                MessageBox.Show("Pilih kamar terlebih dahulu!");
            }
            else
            {
                SqlCommand sqlC = new SqlCommand("select kamar_tipe_id from Kamar where kamar_no = @no", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@no", tambah_nokamar.Text);
                string nilai = sqlC.ExecuteScalar().ToString();
                koneksi.closeConnection();

                /*sqlC = new SqlCommand("select tag_kamar from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                int biayaKamarLama = Int32.Parse(sqlC.ExecuteScalar().ToString());
                koneksi.KoneksiDB();*/


                sqlC = new SqlCommand("select case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end hargaK from Harga where kamar_tipe_id = @tipe and tanggal_id >= @chin and tanggal_id< @chou", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@tipe", Int32.Parse(nilai));
                sqlC.Parameters.AddWithValue("@chin", tambah_tgglCheckIn.Value.Date);
                sqlC.Parameters.AddWithValue("@chou", tambah_tgglCheckOut.Value.Date);
                SqlDataReader readC = sqlC.ExecuteReader();
                int biayaKamarBaru = 0;
                while (readC.Read())
                {
                    biayaKamarBaru += Int32.Parse(readC["hargaK"].ToString());
                }
                koneksi.KoneksiDB();

                sqlC = new SqlCommand("select b.tamu_id from booking b where b.booking_id=@bokid", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@bokid", update_bookingId.Text);
                String tamu_idd = sqlC.ExecuteScalar().ToString();
                //MessageBox.Show("Biaya Lama" + biayaKamarLama);
                //MessageBox.Show("Biaya Baru" + biayaKamarBaru);

                SqlCommand cmd = new SqlCommand("INSERT INTO reservasi (booking_id, checkin, checkout, tamu_id, kamar_no, tag_kamar,tag_restoran,tag_transport,harga_id,status,downpayment,realcheckout) VALUES (@bok_id,@checkin, @checkout,@tamu_id,@kmrno,@tag_kamar,0,0, 1, 'booking',0,@checkout)", koneksi.KoneksiDB());
                //) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,'booking',@j,@k)
                cmd.Parameters.AddWithValue("@bok_id", update_bookingId.Text);
                cmd.Parameters.AddWithValue("@tamu_id", tamu_idd);
                cmd.Parameters.AddWithValue("@kmrno", tambah_nokamar.Text);
                cmd.Parameters.AddWithValue("@tag_kamar", biayaKamarBaru);
                cmd.Parameters.Add("@checkin", SqlDbType.DateTime).Value = tambah_tgglCheckIn.Value.Date;
                cmd.Parameters.Add("@checkout", SqlDbType.DateTime).Value = tambah_tgglCheckOut.Value.Date;
                //cmd.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                float diskon = 100;
                SqlCommand s = new SqlCommand("SELECT bd.harga*100 FROM Booking_diskon bd inner join booking b on b.booking_diskon_id=bd.booking_diskon_id where booking_id=@b_id", koneksi.KoneksiDB());
                s.Parameters.AddWithValue("@b_id", update_bookingId.Text);
                diskon = float.Parse(s.ExecuteScalar().ToString());
                koneksi.closeConnection();
                /*int diskon = 100;
                if (update_bookingDiskon.Checked)
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=1", koneksi.KoneksiDB());
                    diskon = Int32.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }
                else
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=2", koneksi.KoneksiDB());
                    diskon = Int32.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }*/

                cmd = new SqlCommand("update booking set tag_kamar+=@tag_kamar, grand_total+=@grand_total, balance_due+=@balance_due where booking_id=@bokid", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("tag_kamar", biayaKamarBaru);
                cmd.Parameters.AddWithValue("grand_total", biayaKamarBaru);
                //biayaKamarLama = (biayaKamarLama * diskon) / 100;
                biayaKamarBaru = (int)(biayaKamarBaru * diskon) / 100;
                cmd.Parameters.AddWithValue("balance_due", biayaKamarBaru);
                cmd.Parameters.AddWithValue("@bokid", update_bookingId.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                refreshGridViewDataUpdateBooking(update_bookingId.Text);

                panelTambahReservasi.Visible = false;
            }
        }

        private void panelTambahReservasi_Click(object sender, EventArgs e)
        {
            panelCariKamarUpdateReservasi.Visible = false;
        }

        private void tambah_tgglCheckIn_ValueChanged(object sender, EventArgs e)
        {
            tambah_tgglCheckOut.MinDate = tambah_tgglCheckIn.Value.AddDays(1);
            tambah_tgglCheckOut.Value = tambah_tgglCheckIn.Value.AddDays(1);
        }

        private void btn_hapusReservasi_Click(object sender, EventArgs e)
        {
            /*if(Int32.Parse(update_downpayment.Text)>0)
            {
                MessageBox.Show("Reservasi tidak bisa dihapus karena telah ada downpayment!");
            }
            else
            {*/
                panelCariKamarUpdateReservasi.Visible = false;

                SqlCommand sqlC = new SqlCommand("select tag_kamar from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                int biayaKamarLama = Int32.Parse(sqlC.ExecuteScalar().ToString());
                koneksi.KoneksiDB();

                SqlCommand cmd = new SqlCommand("update reservasi set status='cancel' where reservasi_id=@r_id", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                /*int diskon = 100;
                if (update_bookingDiskon.Checked)
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=1", koneksi.KoneksiDB());
                    diskon = Int32.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }
                else
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=2", koneksi.KoneksiDB());
                    diskon = Int32.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }*/
                float diskon = 100;
                SqlCommand s = new SqlCommand("SELECT bd.harga*100 FROM Booking_diskon bd inner join booking b on b.booking_diskon_id=bd.booking_diskon_id where booking_id=@b_id", koneksi.KoneksiDB());
                s.Parameters.AddWithValue("@b_id", update_bookingId.Text);
                diskon = float.Parse(s.ExecuteScalar().ToString());
                koneksi.closeConnection();

                cmd = new SqlCommand("update booking set tag_kamar-=@tag_kamar, grand_total-=@grand_total, balance_due-=@balance_due where booking_id=@bokid", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("tag_kamar", biayaKamarLama);
                cmd.Parameters.AddWithValue("grand_total", biayaKamarLama);
                biayaKamarLama = (int)(biayaKamarLama * diskon) / 100;

                cmd.Parameters.AddWithValue("balance_due", biayaKamarLama - Int32.Parse(update_downpayment.Text));
                cmd.Parameters.AddWithValue("@bokid", update_bookingId.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                panelUpdateReservasi.Visible = false;
                refreshGridViewDataUpdateBooking(update_bookingId.Text);
                update_reservasiId.Text = "-";
                flowLayoutPanel2.Enabled = true;
                keluarToolStripMenuItem.Enabled = true;
                btn_tambahReservasi.Enabled = true;
            //}
        }

        private void tambahReservasiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelCariKamarTambahReservasi.Visible = false;
            panelTambahReservasiCheckIn.BringToFront();

            tambahReservasi_checkin.MinDate = DateTime.Today.AddDays(-1);
            tambahreservasi_checkout.MinDate = DateTime.Today.AddDays(1);
            tambahreservasi_checkout.Value = tambahReservasi_checkin.Value.AddDays(1);

            //DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

            //int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1
            SqlCommand sqlq = new SqlCommand("select max(Reservasi.booking_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and Reservasi.kamar_no=@nok and Reservasi.status='checkin'", koneksi.KoneksiDB());
            //sqlq.Parameters.AddWithValue("@id", DateTime.Today.ToString("yyyy-MM-dd"));
            sqlq.Parameters.AddWithValue("@nok", dataKamarCh);
            string bookingKamar = sqlq.ExecuteScalar().ToString();
            koneksi.closeConnection();

            sqlq = new SqlCommand("select b.tgl_booking, t.tamu_id, t.tamu, b.booking_diskon_id from booking b inner join tamu t on t.tamu_id=b.tamu_id where b.booking_id=@bokid", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@bokid", bookingKamar);
            reader = sqlq.ExecuteReader();
            while (reader.Read())
            {
                tamuCheckIn.Text = reader.GetString(2);
                tgglBookingCheckIn.Text = Convert.ToDateTime(reader.GetValue(0)).ToString("dd/MM/yyyy HH:mm:ss");
                if (reader.GetValue(3).ToString() == "1")
                {
                    bookingCorporateCheckIn.Checked = true;
                }
                else
                {
                    bookingCorporateCheckIn.Checked = false;
                }
            }
            koneksi.closeConnection();

            //MessageBox.Show(bookingKamar);
            bookingIdCheckIn.Text = bookingKamar;

            refreshDataGridView_datareservasi(bookingKamar);
        }

        void refreshDataGridView_datareservasi(String booking_id)
        {
            SqlDataAdapter da = new SqlDataAdapter("select reservasi_id, kamar_no, checkin, checkout, downpayment,status, tamu_id from reservasi where booking_id='" + booking_id + "'", koneksi.KoneksiDB());
            DataTable dataBooking = new DataTable();
            da.Fill(dataBooking);
            datagridview_datareservasi.ReadOnly = true;
            datagridview_datareservasi.DataSource = dataBooking;
            koneksi.closeConnection();
        }

        private void tambahReservasi_checkin_ValueChanged(object sender, EventArgs e)
        {
            tambahreservasi_checkout.MinDate = tambahReservasi_checkin.Value.AddDays(1);
            tambahreservasi_checkout.Value = tambahReservasi_checkin.Value.AddDays(1);
            nokamar_tambahreservasi.Text = "-";
        }

        private void btn_carikamartambahreservasicheckin_Click(object sender, EventArgs e)
        {
            panelCariKamarTambahReservasi.Visible = true;
            panelCariKamarTambahReservasi.BringToFront();
            panelCariKamarTambahReservasi.Controls.Clear();

            cmd = new SqlCommand((@"select count(*) from Kamar"), koneksi.KoneksiDB());

            int jumKamar = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();

            Button[] Kamar;

            cmd = new SqlCommand(
            (@"
            select k.kamar_no,kt.kamar_tipe,kt.warna,
            case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga 
            from 
            (
	            select distinct kamar_no
	            from 
	            Reservasi r
                where 
	                (
                        (r.checkin >= @checkindate
	                    and
	                    r.checkout <=@checkoutdate
	                    )
	                    or
	                    (
	                    r.checkin <= @checkindate
	                    and
	                    r.checkout >=@checkoutdate
	                    )
	                    or 
	                    (
	                    r.checkin >= @checkindate
	                    and
	                    r.checkin < @checkoutdate
	                    )
	                    or 
	                    (
	                    r.checkout > @checkindate
	                    and
	                    r.checkout <=@checkoutdate
	                    )
                    )
                    and r.status in ('booking','checkin') 
            )a
            full join
            Kamar k
            on a.kamar_no = k.kamar_no 
            inner join 
            kamar_tipe kt on k.kamar_tipe_id = kt.kamar_tipe_id inner join harga h on h.tanggal_id = '2014-1-1'
            and kt.kamar_tipe_id = h.kamar_tipe_id
			where a.kamar_no is null and k.status is null 
            "), koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@checkindate", tambahReservasi_checkin.Value.ToString("yyyy-M-d"));
            cmd.Parameters.AddWithValue("@checkoutdate", tambahreservasi_checkout.Value.ToString("yyyy-M-d"));

            reader = cmd.ExecuteReader();
            Kamar = new Button[jumKamar];
            x = 0;
            while (reader.Read())
            {
                Kamar[x] = new Button();
                Kamar[x].Text = reader.GetInt32(0).ToString();
                Kamar[x].Name = reader.GetInt32(0).ToString();
                Kamar[x].Visible = true;
                //Kamar[x].Height = 35;
                Kamar[x].Height = 45;
                Kamar[x].Width = 95;
                Kamar[x].FlatStyle = FlatStyle.Flat;
                Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
                Kamar[x].ImageAlign = btnBooking.ImageAlign;

                Kamar[x].Tag = reader.GetDouble(3).ToString();
                //Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                try
                {
                    Kamar[x].BackColor = Color.FromArgb(Int32.Parse(reader.GetString(2)));
                }
                catch
                {
                    Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                }
                Kamar[x].Click += new EventHandler(tambah_kamarTambahReservasi);
                panelCariKamarTambahReservasi.Controls.Add(Kamar[x]);
                x += 1;
            }
            koneksi.closeConnection();
        }

        void tambah_kamarTambahReservasi(object sender, EventArgs e)
        {
            Button btn =  sender as Button;
            nokamar_tambahreservasi.Text = btn.Text;
            panelCariKamarTambahReservasi.Visible = false;
        }

        private void btn_tambahReservasiCheckIn_Click(object sender, EventArgs e)
        {
            if(nokamar_tambahreservasi.Text == "-"){
                MessageBox.Show("Pilih Kamar!!");
            }
            else
            {
                SqlCommand sqlC = new SqlCommand("select kamar_tipe_id from Kamar where kamar_no = @no", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@no", nokamar_tambahreservasi.Text);
                string nilai = sqlC.ExecuteScalar().ToString();
                koneksi.closeConnection();

                sqlC = new SqlCommand("select case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end hargaK from Harga where kamar_tipe_id = @tipe and tanggal_id >= @chin and tanggal_id< @chou", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@tipe", Int32.Parse(nilai));
                sqlC.Parameters.AddWithValue("@chin", tambahReservasi_checkin.Value.Date);
                sqlC.Parameters.AddWithValue("@chou", tambahreservasi_checkout.Value.Date);
                SqlDataReader readC = sqlC.ExecuteReader();
                int biayaKamarBaru = 0;
                while (readC.Read())
                {
                    biayaKamarBaru += Int32.Parse(readC["hargaK"].ToString());
                }
                koneksi.KoneksiDB();

                sqlC = new SqlCommand("select b.tamu_id from booking b where b.booking_id=@bokid", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@bokid", bookingIdCheckIn.Text);
                String tamu_idd = sqlC.ExecuteScalar().ToString();
                //MessageBox.Show("Biaya Lama" + biayaKamarLama);
                //MessageBox.Show("Biaya Baru" + biayaKamarBaru);

                SqlCommand cmd = new SqlCommand("INSERT INTO reservasi (booking_id, checkin, checkout, tamu_id, kamar_no, tag_kamar,tag_restoran,tag_transport,harga_id,status,downpayment,realcheckout) VALUES (@bok_id,@checkin, @checkout,@tamu_id,@kmrno,@tag_kamar,0,0, 1, 'booking',0,@checkout)", koneksi.KoneksiDB());
                //) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,'booking',@j,@k)
                cmd.Parameters.AddWithValue("@bok_id", bookingIdCheckIn.Text);
                cmd.Parameters.AddWithValue("@tamu_id", tamu_idd);
                cmd.Parameters.AddWithValue("@kmrno", nokamar_tambahreservasi.Text);
                cmd.Parameters.AddWithValue("@tag_kamar", biayaKamarBaru);
                cmd.Parameters.Add("@checkin", SqlDbType.DateTime).Value = tambahReservasi_checkin.Value.Date;
                cmd.Parameters.Add("@checkout", SqlDbType.DateTime).Value = tambahreservasi_checkout.Value.Date;
                //cmd.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                float diskon = 100;
                SqlCommand s = new SqlCommand("SELECT bd.harga*100 FROM Booking_diskon bd inner join booking b on b.booking_diskon_id=bd.booking_diskon_id where booking_id=@b_id", koneksi.KoneksiDB());
                s.Parameters.AddWithValue("@b_id", bookingIdCheckIn.Text);
                diskon = float.Parse(s.ExecuteScalar().ToString());
                koneksi.closeConnection();
                /*
                int diskon = 100;
                if (bookingCorporateCheckIn.Checked)
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=1", koneksi.KoneksiDB());
                    diskon = Int32.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }
                else
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=2", koneksi.KoneksiDB());
                    diskon = Int32.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }
                */
                cmd = new SqlCommand("update booking set tag_kamar+=@tag_kamar, grand_total+=@grand_total, balance_due+=@balance_due where booking_id=@bokid", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("tag_kamar", biayaKamarBaru);
                cmd.Parameters.AddWithValue("grand_total", biayaKamarBaru);
                //biayaKamarLama = (biayaKamarLama * diskon) / 100;
                biayaKamarBaru = (int)(biayaKamarBaru * diskon) / 100;
                cmd.Parameters.AddWithValue("balance_due", biayaKamarBaru);
                cmd.Parameters.AddWithValue("@bokid", bookingIdCheckIn.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                refreshDataGridView_datareservasi(bookingIdCheckIn.Text);

                MessageBox.Show("Reservasi telah ditambahkan!");
                //panelTambahReservasi.Visible = false;
            }
        }

        private void panelTambahReservasiCheckIn_Click(object sender, EventArgs e)
        {
            panelCariKamarTambahReservasi.Visible = false;
        }

        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{
        //    textBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
        //    textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
        //    AutoCompleteStringCollection namec = new AutoCompleteStringCollection();

        //    SqlCommand sql = new SqlCommand("select top 10 tamu from Tamu where tamu like '" + textBox1.Text + "%' ", koneksi.KoneksiDB());
        //    SqlDataReader sqlread = sql.ExecuteReader();
        //    while (sqlread.Read())
        //    {
        //        namec.Add(sqlread["tamu"].ToString());
        //    }
        //    textBox1.AutoCompleteCustomSource = namec;

        //    koneksi.closeConnection();
        
        //}
        string bookingdataID = "";
        int indexChid = 0;
        
        int totalUtang = 0;
        private void btnUtng_Click(object sender, EventArgs e)
        {
            refreshActivatedButton();
            btnUtng.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnUtng.FlatAppearance.BorderSize = 2;

            totalUtang = 0;


            SqlDataAdapter dataadapter = new SqlDataAdapter(@"select b.booking_id, b.tgl_booking,b.CheckIn,b.CheckOut, b.tamu, 
			'Rp. '+ STR(b.utang)
			 as utang
            from
            (
            select bok.booking_id, bok.tgl_booking, t.tamu,
	            (((select bd.harga*100 from Booking_Diskon bd where bd.booking_diskon_id=bok.booking_diskon_id)
	            *
	            (select sum(tag_kamar) from Reservasi where booking_id=bok.booking_id and status='checkout')
	            /100
	            )+
	            (select sum(tag_restoran) from Reservasi where booking_id=bok.booking_id and status='checkout')
	            )-
	            ISNULL((select sum(jumlahpayment) from pembayaran where booking_id=bok.booking_id),0)
	            as utang,
	            (select min(checkin) from Reservasi where booking_id=bok.booking_id and status='checkout') as CheckIn
	            ,(select max(checkout) from Reservasi where booking_id=bok.booking_id and status='checkout') as CheckOut
            from
            (
            select booking_id from Reservasi where status='checkout'
            group by booking_id
            except
            select booking_id from Reservasi where status='checkin'
            group by booking_id
            except
            select booking_id from Reservasi where status='booking'
            group by booking_id
            )a inner join Booking bok on a.booking_id= bok.booking_id
            inner join Tamu t on bok.tamu_id=t.tamu_id
            )b
            where b.utang > 100", koneksi.KoneksiDB());
            DataSet dse = new DataSet();
            dataGridUtang.Columns.Clear();
            //connection.Open();
            dataadapter.Fill(dse, "utang");
            dataGridUtang.DataSource = dse;
            dataGridUtang.DataMember = "utang";

            panelUtang.BringToFront();

            koneksi.closeConnection();
            var buttonCol = new DataGridViewButtonColumn();
            buttonCol.Name = "Lunasi";
            buttonCol.HeaderText = "";
            buttonCol.Text = "Lunasi";
            buttonCol.DefaultCellStyle.SelectionBackColor = Color.Gray;
            buttonCol.DefaultCellStyle.Font = btn_historis.Font;
            buttonCol.FlatStyle = btn_historis.FlatStyle;
            dataGridUtang.Columns.Add(buttonCol);

            foreach (DataGridViewRow row in dataGridUtang.Rows)
            {
                row.Cells["Lunasi"].Value = "Lunasi";
            }
            var buttonColInvoice = new DataGridViewButtonColumn();
            buttonColInvoice.Name = "Print Invoice";
            buttonColInvoice.HeaderText = "";
            buttonColInvoice.Text = "Print Invoice";
            buttonColInvoice.DefaultCellStyle.SelectionBackColor = Color.Gray;
            buttonColInvoice.DefaultCellStyle.Font = btn_historis.Font;
            buttonColInvoice.FlatStyle = btn_historis.FlatStyle;
            dataGridUtang.Columns.Add(buttonColInvoice);

            foreach (DataGridViewRow row in dataGridUtang.Rows)
            {
                row.Cells["Print Invoice"].Value = "Print Invoice";
            }

            indexChid = 0;
            panelUtang.Visible = true;
            dataGridUtang.Columns[0].Visible = false;
        }

        
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //private void dataGridUtang_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    string idbookingData = dataGridUtang[0, e.RowIndex].Value.ToString();
        //    bookingdataID = idbookingData;
        //    SqlCommand sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
        //    sql1.Parameters.AddWithValue("@a", idbookingData);
        //    int kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
        //    koneksi.closeConnection();

        //    sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
        //    sql1.Parameters.AddWithValue("@a", kodediskon);
        //    int potongan = Int32.Parse(sql1.ExecuteScalar().ToString());
        //    koneksi.closeConnection();


        //    SqlCommand queryData = new SqlCommand("select reservasi_id, tag_kamar, tag_restoran, downpayment from Reservasi where booking_id=@a and status='checkout' ", koneksi.KoneksiDB());
        //    queryData.Parameters.AddWithValue("@a", idbookingData);
        //    SqlDataReader readKumpulData = queryData.ExecuteReader();
        //    reservasichid = new int[90];
        //    tagKamarchid = new int[90];
        //    tagrestoranchid = new int[90];
        //    downpaymentchid = new int[90];
        //    indexChid = 0;
        //    while (readKumpulData.Read())
        //    {
        //        reservasichid[indexChid] = Int32.Parse(readKumpulData["reservasi_id"].ToString());
        //        tagKamarchid[indexChid] = (Int32.Parse(readKumpulData["tag_kamar"].ToString()) * potongan) / 100;
        //        tagrestoranchid[indexChid] = Int32.Parse(readKumpulData["tag_restoran"].ToString());
        //        downpaymentchid[indexChid] = Int32.Parse(readKumpulData["downpayment"].ToString());
        //        indexChid += 1;
        //    }
        //    koneksi.closeConnection();
        //    /*
        //    List<Microsoft.Reporting.WinForms.ReportParameter> parameters = new List<Microsoft.Reporting.WinForms.ReportParameter>();
        //    Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("booking_id", idbookingData.ToString());
        //    parameters.Add(param);
        //    //Microsoft.Reporting.WinForms.ReportParameter param2 = new Microsoft.Reporting.WinForms.ReportParameter("room", dataKamarCh.ToString());
        //    //list.Add(param2);
        //    reportInvoice.ServerReport.ReportPath = "/Invoice/Invoice_Booking";
        //    reportInvoice.ServerReport.SetParameters(parameters);
        //    //reportInvoice.ServerReport.Refresh();
        //    reportInvoice.RefreshReport();
        //    reportInvoice.BringToFront();
        //    List<Microsoft.Reporting.WinForms.ReportParameter> parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
        //    reportInvoice.ServerReport.SetParameters(parameter_reset);
        //    */

        //}

        private void checkOutKamarPendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@id", dataKamarCh);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            sql = new SqlCommand("update Reservasi set checkout=@tggal where kamar_no = @id and status='checkin' ", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@tggal", DateTime.Now);
            sql.Parameters.AddWithValue("@id", dataKamarCh);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            sql = new SqlCommand("update Reservasi set status= 'checkout' where kamar_no = @id and (datediff(minute,checkout,@tggal)<2 and datediff(minute,checkout,@tggal)>=0) and status='checkin' ", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@tggal", DateTime.Now);
            sql.Parameters.AddWithValue("@id", dataKamarCh);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            btnCheckInStatus_Click(sender, e);
        
        }

        private void checkOutBookingPendingToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SqlCommand queryData = new SqlCommand("select booking_id from Reservasi where status='checkin' and kamar_no = @a", koneksi.KoneksiDB());
            queryData.Parameters.AddWithValue("@a", dataKamarCh);
            int idbookingData = Int32.Parse(queryData.ExecuteScalar().ToString());
            koneksi.closeConnection();

            queryData = new SqlCommand("update Reservasi set checkout=@tggal where booking_id =@a and status='checkin'", koneksi.KoneksiDB());
            queryData.Parameters.AddWithValue("@a", idbookingData);
            queryData.Parameters.AddWithValue("@tggal", DateTime.Now);
            queryData.ExecuteNonQuery();
            koneksi.closeConnection();

            queryData = new SqlCommand("select kamar_no from Reservasi where booking_id=@a  and status='checkin'", koneksi.KoneksiDB());
            queryData.Parameters.AddWithValue("@a", idbookingData);
            SqlDataReader readerData = queryData.ExecuteReader();
            ArrayList list = new ArrayList();
            while (readerData.Read())
            {
                list.Add(Int32.Parse(readerData["kamar_no"].ToString()));

            }
            koneksi.closeConnection();

            queryData = new SqlCommand("update Reservasi set status='checkout' where booking_id =@a and status='checkin'", koneksi.KoneksiDB());
            queryData.Parameters.AddWithValue("@a", idbookingData);
            queryData.ExecuteNonQuery();
            koneksi.closeConnection();

            foreach (int i in list)
            {
                SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@id", i);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

            }
            btnCheckInStatus_Click(sender, e);

        }

        int[] reservasichid;
        int[] tagKamarchid;
        int[] tagrestoranchid;
        int[] downpaymentchid;
        int indezChid = 0;
        private void dataGridUtang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                

                if (dataGridUtang.Columns[e.ColumnIndex].Name == "Lunasi")
                {
                    indezChid = 0;
                    string bookingidX = dataGridUtang[0, e.RowIndex].Value.ToString();
                    SqlCommand sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
                    sql1.Parameters.AddWithValue("@a", bookingidX);
                    int kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
                    sql1.Parameters.AddWithValue("@a", kodediskon);
                    float potongan = float.Parse(sql1.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                    reservasichid = new int[90];
                    tagKamarchid = new int[90];
                    tagrestoranchid = new int[90];
                    downpaymentchid = new int[90];

                    SqlCommand queryData = new SqlCommand("select reservasi_id, tag_kamar, tag_restoran, downpayment from Reservasi where booking_id=@a and status='checkout' ", koneksi.KoneksiDB());
                    queryData.Parameters.AddWithValue("@a", bookingidX);
                    SqlDataReader readKumpulData = queryData.ExecuteReader();
                    int totalUtangZ = 0;
                    while (readKumpulData.Read())
                    {

                        reservasichid[indezChid] = Int32.Parse(readKumpulData["reservasi_id"].ToString());
                        tagKamarchid[indezChid] = (int)(Int32.Parse(readKumpulData["tag_kamar"].ToString()) * potongan) / 100;
                        tagrestoranchid[indezChid] = Int32.Parse(readKumpulData["tag_restoran"].ToString());
                        //downpaymentchid[indezChid] = Int32.Parse(readKumpulData["downpayment"].ToString());
                        totalUtangZ += tagKamarchid[indezChid] + tagrestoranchid[indezChid];
                        indezChid += 1;

                    }

                    koneksi.closeConnection();
                    queryData = new SqlCommand("select sum(jumlahpayment) from pembayaran where booking_id = @a", koneksi.KoneksiDB());
                    queryData.Parameters.AddWithValue("@a", bookingidX);
                    string jumBayarUtang = queryData.ExecuteScalar().ToString();
                    koneksi.closeConnection();
                    totalUtangZ = totalUtangZ - Convert.ToInt32(jumBayarUtang);
                    //MessageBox.Show("Total Utang = Rp." + totalUtangZ.ToString());
                    DialogResult result = MessageBox.Show("Anda yakin untuk melunasi booking ini? \n Jumlah Utang : " + totalUtangZ.ToString() + "\n Metode Pembayaran :" + cbjnispembayaran.Text.ToUpper(), "Confirmation", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        bookingidX = dataGridUtang[0, e.RowIndex].Value.ToString();
                        sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
                        sql1.Parameters.AddWithValue("@a", bookingidX);
                        kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
                        koneksi.closeConnection();

                        string hargalebihReser = "";
                        hargalebihReser = cekHargaLebih(Convert.ToInt32(bookingidX));
            
                        sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
                        sql1.Parameters.AddWithValue("@a", kodediskon);
                        potongan = float.Parse(sql1.ExecuteScalar().ToString());
                        koneksi.closeConnection();


                        queryData = new SqlCommand("select reservasi_id, tag_kamar, tag_restoran, downpayment from Reservasi where booking_id=@a and status='checkout' ", koneksi.KoneksiDB());
                        queryData.Parameters.AddWithValue("@a", bookingidX);
                        readKumpulData = queryData.ExecuteReader();
                        while (readKumpulData.Read())
                        {
                            int idreservasiUtng = Int32.Parse(readKumpulData["reservasi_id"].ToString());
                            int tagihanKmrUtng = (int)(Int32.Parse(readKumpulData["tag_kamar"].ToString()) * potongan) / 100;
                            int tagihanrestoUtng = Int32.Parse(readKumpulData["tag_restoran"].ToString());
                            SqlCommand querybayar = new SqlCommand("select sum(jumlahpayment) from pembayaran where booking_id = @a and reservasi_id = @b", koneksi.KoneksiDB());
                            querybayar.Parameters.AddWithValue("@a", bookingidX);
                            querybayar.Parameters.AddWithValue("@b", idreservasiUtng);
                            string jumBayarReser = querybayar.ExecuteScalar().ToString();
                            if (jumBayarReser.Equals(""))
                            {
                                jumBayarReser = "0";
                            }
                            if (Convert.ToInt32(hargalebihReser) > 0)
                            {
                                SqlCommand sqlLbh = new SqlCommand("select payment_id,payment,nopayment,tggalpayment,staff_id from pembayaran where reservasi_id = @a and jumlahpayment = (select max(jumlahpayment) from pembayaran where reservasi_id = @a)", koneksi.KoneksiDB());
                                sqlLbh.Parameters.AddWithValue("@a", resIDLebih[0]);
                                SqlDataReader readDaLbh = sqlLbh.ExecuteReader();
                                string paymentLbh = ""; string paymentnamaLbh = ""; string nopaymentnamaLbh = "";
                                string tglpaymentnamaLbh = ""; string staffpaymentnamaLbh = "";
                                while (readDaLbh.Read())
                                {
                                    paymentLbh = readDaLbh["payment_id"].ToString();
                                    paymentnamaLbh = readDaLbh["payment"].ToString();
                                    nopaymentnamaLbh = readDaLbh["nopayment"].ToString();
                                    tglpaymentnamaLbh = readDaLbh["tggalpayment"].ToString();
                                    staffpaymentnamaLbh = readDaLbh["staff_id"].ToString();

                                }
                                koneksi.closeConnection();
                                if ((tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser)) > 0)
                                {
                                    if (Convert.ToInt32(hargalebihReser) - (tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser)) > 0)
                                    {
                                        sqlLbh = new SqlCommand("update pembayaran set jumlahpayment=jumlahpayment-@a where payment_id=@b", koneksi.KoneksiDB());
                                        sqlLbh.Parameters.AddWithValue("@a", (tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser)));
                                        sqlLbh.Parameters.AddWithValue("@b", paymentLbh);
                                        sqlLbh.ExecuteNonQuery();
                                        koneksi.closeConnection();

                                        sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment-@a where reservasi_id=@b", koneksi.KoneksiDB());
                                        sqlLbh.Parameters.AddWithValue("@a", (tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser)));
                                        sqlLbh.Parameters.AddWithValue("@b", resIDLebih[0]);
                                        sqlLbh.ExecuteNonQuery();
                                        koneksi.closeConnection();

                                        sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment+@a where reservasi_id=@b", koneksi.KoneksiDB());
                                        sqlLbh.Parameters.AddWithValue("@a", (tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser)));
                                        sqlLbh.Parameters.AddWithValue("@b", idreservasiUtng);
                                        sqlLbh.ExecuteNonQuery();
                                        koneksi.closeConnection();

                                        sqlLbh = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment,staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                                        sqlLbh.Parameters.AddWithValue("@a", bookingidX);
                                        sqlLbh.Parameters.AddWithValue("@b", idreservasiUtng);
                                        sqlLbh.Parameters.AddWithValue("@c", paymentnamaLbh);
                                        sqlLbh.Parameters.AddWithValue("@d", nopaymentnamaLbh);
                                        sqlLbh.Parameters.AddWithValue("@e", (tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser)));
                                        sqlLbh.Parameters.AddWithValue("@f", Convert.ToDateTime(tglpaymentnamaLbh));
                                        sqlLbh.Parameters.AddWithValue("@g", staffpaymentnamaLbh);
                                        sqlLbh.ExecuteNonQuery();
                                        koneksi.closeConnection();
                                        hargalebihReser = (Convert.ToInt32(hargalebihReser) - (tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser))).ToString();
                                    }
                                    else
                                    {
                                        sqlLbh = new SqlCommand("update pembayaran set jumlahpayment=jumlahpayment-@a where payment_id=@b", koneksi.KoneksiDB());
                                        sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(hargalebihReser));
                                        sqlLbh.Parameters.AddWithValue("@b", paymentLbh);
                                        sqlLbh.ExecuteNonQuery();
                                        koneksi.closeConnection();

                                        sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment-@a where reservasi_id=@b", koneksi.KoneksiDB());
                                        sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(hargalebihReser));
                                        sqlLbh.Parameters.AddWithValue("@b", resIDLebih[0]);
                                        sqlLbh.ExecuteNonQuery();
                                        koneksi.closeConnection();

                                        sqlLbh = new SqlCommand("update Reservasi set downpayment=downpayment+@a where reservasi_id=@b", koneksi.KoneksiDB());
                                        sqlLbh.Parameters.AddWithValue("@a", Convert.ToInt32(hargalebihReser));
                                        sqlLbh.Parameters.AddWithValue("@b", idreservasiUtng);
                                        sqlLbh.ExecuteNonQuery();
                                        koneksi.closeConnection();

                                        sqlLbh = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment,staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                                        sqlLbh.Parameters.AddWithValue("@a", bookingidX);
                                        sqlLbh.Parameters.AddWithValue("@b", idreservasiUtng);
                                        sqlLbh.Parameters.AddWithValue("@c", paymentnamaLbh);
                                        sqlLbh.Parameters.AddWithValue("@d", nopaymentnamaLbh);
                                        sqlLbh.Parameters.AddWithValue("@e", Convert.ToInt32(hargalebihReser));
                                        sqlLbh.Parameters.AddWithValue("@f", Convert.ToDateTime(tglpaymentnamaLbh));
                                        sqlLbh.Parameters.AddWithValue("@g", staffpaymentnamaLbh);
                                        sqlLbh.ExecuteNonQuery();
                                        koneksi.closeConnection();

                                        querybayar = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment, staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                                        querybayar.Parameters.AddWithValue("@a", bookingidX);
                                        querybayar.Parameters.AddWithValue("@b", idreservasiUtng);
                                        querybayar.Parameters.AddWithValue("@c", "Kontan");
                                        querybayar.Parameters.AddWithValue("@d", "");
                                        querybayar.Parameters.AddWithValue("@e", (tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser)) - Convert.ToInt32(hargalebihReser));
                                        querybayar.Parameters.AddWithValue("@f", DateTime.Now);
                                        querybayar.Parameters.AddWithValue("@g", Login.idS.ToString());
                                        querybayar.ExecuteNonQuery();
                                        koneksi.closeConnection();

                                        querybayar = new SqlCommand("update Reservasi set downpayment = downpayment + @a where reservasi_id = @b", koneksi.KoneksiDB());
                                        querybayar.Parameters.AddWithValue("@a", (tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser)) - Convert.ToInt32(hargalebihReser));
                                        querybayar.Parameters.AddWithValue("@b", idreservasiUtng);
                                        querybayar.ExecuteNonQuery();
                                        koneksi.closeConnection();
                                        hargalebihReser = (Convert.ToInt32(hargalebihReser) - Convert.ToInt32(hargalebihReser)).ToString();

                                    }
                                }
                            }
                            else
                            {
                                querybayar = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment, staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
                                querybayar.Parameters.AddWithValue("@a", bookingidX);
                                querybayar.Parameters.AddWithValue("@b", idreservasiUtng);
                                querybayar.Parameters.AddWithValue("@c", cbjnispembayaran.Text);
                                querybayar.Parameters.AddWithValue("@d", "");
                                querybayar.Parameters.AddWithValue("@e", tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser));
                                querybayar.Parameters.AddWithValue("@f", DateTime.Now);
                                querybayar.Parameters.AddWithValue("@g", Login.idS.ToString());
                                querybayar.ExecuteNonQuery();

                                querybayar = new SqlCommand("update Reservasi set downpayment = downpayment + @a where reservasi_id = @b", koneksi.KoneksiDB());
                                querybayar.Parameters.AddWithValue("@a", tagihanrestoUtng + tagihanKmrUtng - Convert.ToInt32(jumBayarReser));
                                querybayar.Parameters.AddWithValue("@b", idreservasiUtng);
                                querybayar.ExecuteNonQuery();
                            }
                        }
                        koneksi.closeConnection();

                        queryData = new SqlCommand("update Booking set balance_due=0 where booking_id =@a", koneksi.KoneksiDB());
                        queryData.Parameters.AddWithValue("@a", bookingidX);
                        queryData.ExecuteNonQuery();
                        koneksi.closeConnection();
                        btnUtng_Click(sender, e);

                        this.infoBooking.EnforceConstraints = false;

                        this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
                        this.TamuTableAdapter.Fill(this.infoBooking.Tamu, Int32.Parse(bookingidX));
                        this.NewSelectCommandTableAdapter.Fill(this.infoReservasi.NewSelectCommand, Int32.Parse(bookingidX), null);
                        this.newSelectCommandTableAdapter1.Fill(this.pemesanan.NewSelectCommand, Int32.Parse(bookingidX), null);
                        this.NewSelectPembayaranTableAdapter.Fill(this.infoPembayaran.NewSelectPembayaran, Int32.Parse(bookingidX), null);
                        reportInvoice.LocalReport.EnableExternalImages = true;
                        string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
                        ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                        reportInvoice.LocalReport.SetParameters(parameter);
                        string imagePath2 = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(bookingidX) + ".png";
                        if (!File.Exists(Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(bookingidX) + ".png"))
                        {
                            imagePath2 = "NULL";
                        }
                        ReportParameter parameter2 = new ReportParameter("ttd", imagePath2);
                        reportInvoice.LocalReport.SetParameters(parameter2);
    
                        reportInvoice.RefreshReport();
                        reportInvoice.BringToFront();
                    }
                    else{
                    }
                }
                else if(dataGridUtang.Columns[e.ColumnIndex].Name == "Print Invoice")
                {
                    
                    indezChid = 0;
                    string bookingidX = dataGridUtang[0, e.RowIndex].Value.ToString();
                    SqlCommand sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
                    sql1.Parameters.AddWithValue("@a", bookingidX);
                    int kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
                    sql1.Parameters.AddWithValue("@a", kodediskon);
                    float potongan = float.Parse(sql1.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                    reservasichid = new int[90];
                    tagKamarchid = new int[90];
                    tagrestoranchid = new int[90];
                    downpaymentchid = new int[90];

                    SqlCommand queryData = new SqlCommand("select reservasi_id, tag_kamar, tag_restoran, downpayment from Reservasi where booking_id=@a and status='checkout' ", koneksi.KoneksiDB());
                    queryData.Parameters.AddWithValue("@a", bookingidX);
                    SqlDataReader readKumpulData = queryData.ExecuteReader();
                    int totalUtangZ = 0;
                    while (readKumpulData.Read())
                    {
                       
                        reservasichid[indezChid] = Int32.Parse(readKumpulData["reservasi_id"].ToString());
                        tagKamarchid[indezChid] = (int)(Int32.Parse(readKumpulData["tag_kamar"].ToString()) * potongan) / 100;
                        tagrestoranchid[indezChid] = Int32.Parse(readKumpulData["tag_restoran"].ToString());
                        //downpaymentchid[indezChid] = Int32.Parse(readKumpulData["downpayment"].ToString());
                        totalUtangZ += tagKamarchid[indezChid] + tagrestoranchid[indezChid];
                        indezChid += 1;

                    }

                    koneksi.closeConnection();
                    queryData = new SqlCommand("select sum(jumlahpayment) from pembayaran where booking_id = @a",koneksi.KoneksiDB());
                    queryData.Parameters.AddWithValue("@a", bookingidX);
                    string jumBayarUtang = queryData.ExecuteScalar().ToString();
                    koneksi.closeConnection();
                    totalUtangZ = totalUtangZ - Convert.ToInt32(jumBayarUtang);
                    //MessageBox.Show("Total Utang = Rp." + totalUtangZ.ToString());
                    
                    this.infoBooking.EnforceConstraints = false;

                    this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
                    this.TamuTableAdapter.Fill(this.infoBooking.Tamu, Int32.Parse(bookingidX));
                    this.NewSelectCommandTableAdapter.Fill(this.infoReservasi.NewSelectCommand, Int32.Parse(bookingidX), null);
                    this.newSelectCommandTableAdapter1.Fill(this.pemesanan.NewSelectCommand, Int32.Parse(bookingidX), null);
                    this.NewSelectPembayaranTableAdapter.Fill(this.infoPembayaran.NewSelectPembayaran, Int32.Parse(bookingidX), null);
                    reportInvoice.LocalReport.EnableExternalImages = true;
                    string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
                    ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                    reportInvoice.LocalReport.SetParameters(parameter);
                    string imagePath2 = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(bookingidX) + ".png";
                    if (!File.Exists(Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(bookingidX) + ".png"))
                    {
                        imagePath2 = "NULL";
                    }
                    ReportParameter parameter2 = new ReportParameter("ttd", imagePath2);
                    reportInvoice.LocalReport.SetParameters(parameter2);

                    reportInvoice.RefreshReport();
                    reportInvoice.BringToFront();
                    
                }
            }
            catch
            {
        
            }
        }

        private void kuragiHariToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string lmaHari = Interaction.InputBox("Kurangi berapa Hari =");
            try
            {
                //MessageBox.Show( DateTime.Now.Date.AddDays(Int32.Parse(lmaHari)).ToString());
                SqlCommand sql1 = new SqlCommand("select checkout from Reservasi where kamar_no =@noKamar and status = 'checkin'", koneksi.KoneksiDB());
                sql1.Parameters.AddWithValue("@noKamar", dataKamarCh);
                //tambahBaru
                DateTime haricheck = Convert.ToDateTime(sql1.ExecuteScalar().ToString());
                //DATENAME(dw,tanggal_id) in ('Saturday','Sunday')
                koneksi.closeConnection();
                //      MessageBox.Show(haricheck.AddDays(Convert.ToDouble(lmaHari)*-1).ToString());

                if (haricheck.AddDays(Convert.ToDouble(lmaHari) * -1) <= DateTime.Today)
                {
                    MessageBox.Show("Hari tidak dapat dikurangi lebih dari hari ini!");
                }
                else
                {

                    SqlCommand sql = new SqlCommand("select case when DATENAME(dw,tanggal_id) in ('Saturday','Sunday') then harga_weekend else harga end harga from Harga inner join Kamar on Kamar.kamar_tipe_id = Harga.kamar_tipe_id and Kamar.kamar_no=@NOKAMAR and Harga.tanggal_id >= convert(date,@chin) and Harga.tanggal_id< @chou", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@NOKAMAR", dataKamarCh);
                    sql.Parameters.AddWithValue("@chin", haricheck.AddDays(Convert.ToDouble(lmaHari) * -1));
                    sql.Parameters.AddWithValue("@chou", haricheck);

                    SqlDataReader sqlDataHarga = sql.ExecuteReader();
                    int jumHargaLama = 0;
                    while (sqlDataHarga.Read())
                    {
                        jumHargaLama += Int32.Parse(sqlDataHarga["harga"].ToString());
                    }
                    koneksi.closeConnection();

                    sql = new SqlCommand("update Reservasi set checkout = @tnggalcheck,realcheckout=@tnggalcheck, tag_kamar = tag_kamar - @nilai where status='checkin' and kamar_no=@kamarno", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@kamarno", dataKamarCh);
                    sql.Parameters.AddWithValue("@nilai", jumHargaLama);
                    sql.Parameters.AddWithValue("@tnggalcheck", haricheck.AddDays(Convert.ToDouble(lmaHari) * -1));
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    sql = new SqlCommand("select booking_id from Reservasi where status='checkin' and kamar_no = @kamarno", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@kamarno", dataKamarCh);
                    int kodeid = Int32.Parse(sql.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    sql = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", kodeid);
                    int kodediskon = Int32.Parse(sql.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    sql = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", kodediskon);
                    float potongan = float.Parse(sql.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    sql = new SqlCommand("update Booking set grand_total = grand_total - @nilai,tag_kamar=tag_kamar-@nilai, balance_due=balance_due-@nilai2 where  booking_id=@id", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@nilai", jumHargaLama);
                    sql.Parameters.AddWithValue("@nilai2", (int)(jumHargaLama * potongan) / 100);

                    sql.Parameters.AddWithValue("@id", kodeid);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();
                }
            }//tambahBaru
            catch
            {
                if (lmaHari != "")
                {
                    MessageBox.Show("Inputan harus dalam bentuk integer!");
                }
            }


            btnCheckInStatus_Click(sender, e);
        }

        private void input_tamu_historis_TextChanged(object sender, EventArgs e)
        {
            //if (input_tamu_historis.Text.Length >= 3)
            //{
                /*BindingSource bs = new BindingSource();
                bs.DataSource = gridView_historis.DataSource;
                bs.Filter = "tamu like '%" + input_tamu_historis.Text + "%'";
            */
            reload_dataHistoris(input_tamu_historis.Text , filter_checkin.Value, filter_checkout.Value);
                //gridView_historis.DataSource = bs;
        }

        private void btn_gabungBookingHistoris_Click(object sender, EventArgs e)
        {

            /*
            string bookin_id_first = "";
            foreach (DataGridViewRow row in gridView_historis.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ColumnIndex == 8) //Checkbox historis
                    {
                        if(Convert.ToBoolean(cell.Value) == true){
                            Console.WriteLine(row.Cells[0].Value);
                        }
                    }
                }
            }*/
            int cek_booking_diskon_id = 0;
            int first_booking_diskon_id = 0;
            foreach (int number in uniqueBooking_id)
            {
                if(first_booking_diskon_id == 0){
                    cmd = new SqlCommand("select booking_diskon_id from booking where booking_id=@a", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@a", number);
                    first_booking_diskon_id = Int32.Parse(cmd.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }
                else
                {
                    int next_booking_diskon_id;
                    cmd = new SqlCommand("select booking_diskon_id from booking where booking_id=@a", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@a", number);
                    next_booking_diskon_id = Int32.Parse(cmd.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                    if(first_booking_diskon_id != next_booking_diskon_id){
                        cek_booking_diskon_id++;
                    }
                    first_booking_diskon_id = next_booking_diskon_id;
                }
            }


            //if (cek_booking_diskon_id == 0)
            //{
            //    int ctr_book = 0;
            //    int first_booking_id = 0;
            //    foreach (int number in uniqueBooking_id)
            //    {
            //        ctr_book++;
            //        if (ctr_book <= 1)
            //        {
            //            first_booking_id = number;
            //        }
            //        else
            //        {
            //            gabungBooking(first_booking_id, number);
            //        }
            //    }
            //    reload_dataHistoris(input_tamu_historis.Text, filter_checkin.Value, filter_checkout.Value);
            //}
            //else
            //{
            //    MessageBox.Show("Diskon booking harus disamakan terlebih dahulu!");
            //}

            if (cek_booking_diskon_id != 0)
            {
                //MessageBox.Show("Diskon untuk booking yang digabungkan telah dibatalkan, mohon masukkan jumlah diskon kembali.");
                DialogResult result = MessageBox.Show("Diskon booking akan dibatalkan sebelum booking digabungkan. Jumlah diskon dapat dimasukkan kembali di panel Kamar Checkin atau Jadwal Booking. \nLanjutkan penggabungan booking?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    //int ctr_book = 0;
                    //int first_booking_id = 0;
                    //foreach (int number in uniqueBooking_id)
                    //{
                    //    ctr_book++;
                    //    if (ctr_book <= 1)
                    //    {
                    //        first_booking_id = number;
                    //    }
                    //    else
                    //    {
                    //        gabungBooking(first_booking_id, number);
                    //    }
                    //}
                    //reload_dataHistoris(input_tamu_historis.Text, filter_checkin.Value, filter_checkout.Value);
                    kondisional_gabung_booking();
                }
            }
            else
            {
                kondisional_gabung_booking();
            }

           
            
            

        }

        void kondisional_gabung_booking()
        {
            int ctr_book = 0;
            int first_booking_id = 0;
            foreach (int number in uniqueBooking_id)
            {
                ctr_book++;
                if (ctr_book <= 1)
                {
                    first_booking_id = number;
                }
                else
                {
                    gabungBooking(first_booking_id, number);
                }
            }
            reload_dataHistoris(input_tamu_historis.Text, filter_checkin.Value, filter_checkout.Value);
        }

        void gabungBooking(int booking_id_awal, int booking_id_akhir)
        {
            SqlCommand sql = new SqlCommand("select bk.harga*100 from booking bok inner join Booking_Diskon bk on bk.booking_diskon_id = bok.booking_diskon_id and bok.booking_id = @booking_awal", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@booking_awal", booking_id_awal);
            float diskon = float.Parse(sql.ExecuteScalar().ToString());
            //MessageBox.Show(diskon.ToString());
            koneksi.closeConnection();

            sql = new SqlCommand(@"select sum(res.downpayment) 
                from Reservasi res
                where res.booking_id =@booking_akhir
                group by res.booking_id", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@booking_akhir", booking_id_akhir);
            int downpayment = Int32.Parse(sql.ExecuteScalar().ToString());
            //MessageBox.Show(downpayment.ToString());
            koneksi.closeConnection();

            sql = new SqlCommand(@"select sum(res.tag_restoran) 
                from Reservasi res
                where res.booking_id =@booking_akhir
                group by res.booking_id", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@booking_akhir", booking_id_akhir);
            int charge = Int32.Parse(sql.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sql = new SqlCommand(@"update boktarget set boktarget.tag_kamar = bokawal.tag_kamar + boktarget.tag_kamar
                ,boktarget.grand_total = boktarget.grand_total + ((((bokawal.grand_total - @charge)*@diskon)/100)+@charge)
                ,boktarget.balance_due = boktarget.balance_due + (((((bokawal.grand_total - @charge)*@diskon)/100)+@charge)-@downawal)
                ,boktarget.statusbayar = 1
                ,boktarget.booking_diskon_id = 0
                from Booking bokawal
                inner join 
                Booking  boktarget 
                on 
                bokawal.booking_id = @booking_akhir
                and
                boktarget.booking_id = @booking_awal", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@booking_awal", booking_id_awal);
            sql.Parameters.AddWithValue("@booking_akhir", booking_id_akhir);
            sql.Parameters.AddWithValue("@diskon", diskon);
            sql.Parameters.AddWithValue("@charge", charge);
            sql.Parameters.AddWithValue("@downawal", downpayment);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            sql = new SqlCommand(@"update res set booking_id = @booking_awal
                from Reservasi res where booking_id = @booking_akhir", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@booking_awal", booking_id_awal);
            sql.Parameters.AddWithValue("@booking_akhir", booking_id_akhir);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            sql = new SqlCommand(@"update pem set booking_id = @booking_awal
                from pembayaran pem where booking_id = @booking_akhir", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@booking_awal", booking_id_awal);
            sql.Parameters.AddWithValue("@booking_akhir", booking_id_akhir);
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

        }

        private void filter_checkin_ValueChanged(object sender, EventArgs e)
        {
            reload_dataHistoris(input_tamu_historis.Text, filter_checkin.Value, filter_checkout.Value);
        }

        private void filter_checkout_ValueChanged(object sender, EventArgs e)
        {
            reload_dataHistoris(input_tamu_historis.Text, filter_checkin.Value, filter_checkout.Value);
        }
        
        private void btn_hapusReservasiNPembayaran_Click(object sender, EventArgs e)
        {
            if (Int32.Parse(update_downpayment.Text) > 0)
            {
                DialogResult result = MessageBox.Show("Pembayaran pada reservasi ini akan dihapus! \n Apakah anda yakin?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    panelCariKamarUpdateReservasi.Visible = false;

                    SqlCommand sqlC = new SqlCommand("select tag_kamar from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                    sqlC.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                    int biayaKamarLama = Int32.Parse(sqlC.ExecuteScalar().ToString());
                    koneksi.KoneksiDB();

                    /*int diskon = 100;
                    if (update_bookingDiskon.Checked)
                    {
                        SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=1", koneksi.KoneksiDB());
                        diskon = Int32.Parse(s.ExecuteScalar().ToString());
                        koneksi.closeConnection();
                    }
                    else
                    {
                        SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=2", koneksi.KoneksiDB());
                        diskon = Int32.Parse(s.ExecuteScalar().ToString());
                        koneksi.closeConnection();
                    }*/


                    float diskon = 100;
                    SqlCommand s = new SqlCommand("SELECT bd.harga*100 FROM Booking_diskon bd inner join booking b on b.booking_diskon_id=bd.booking_diskon_id where booking_id=@b_id", koneksi.KoneksiDB());
                    s.Parameters.AddWithValue("@b_id", update_bookingId.Text);
                    diskon = float.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    SqlCommand cmd = new SqlCommand("delete from pembayaran where reservasi_id=@r_id", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                    cmd.ExecuteNonQuery();
                    koneksi.closeConnection();

                    cmd = new SqlCommand("delete from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                    cmd.ExecuteNonQuery();
                    koneksi.closeConnection();

                    cmd = new SqlCommand("update booking set tag_kamar-=@tag_kamar, grand_total-=@grand_total, balance_due-=@balance_due where booking_id=@bokid", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("tag_kamar", biayaKamarLama);
                    cmd.Parameters.AddWithValue("grand_total", biayaKamarLama);
                    biayaKamarLama = (int)(biayaKamarLama * diskon) / 100;
                    cmd.Parameters.AddWithValue("balance_due", biayaKamarLama - Int32.Parse(update_downpayment.Text));
                    cmd.Parameters.AddWithValue("@bokid", update_bookingId.Text);
                    cmd.ExecuteNonQuery();
                    koneksi.closeConnection();

                    panelUpdateReservasi.Visible = false;
                    refreshGridViewDataUpdateBooking(update_bookingId.Text);
                    update_reservasiId.Text = "-";
                    flowLayoutPanel2.Enabled = true;
                    keluarToolStripMenuItem.Enabled = true;
                    btn_tambahReservasi.Enabled = true;
                }
            }
            else
            {
                panelCariKamarUpdateReservasi.Visible = false;

                SqlCommand sqlC = new SqlCommand("select tag_kamar from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                sqlC.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                int biayaKamarLama = Int32.Parse(sqlC.ExecuteScalar().ToString());
                koneksi.KoneksiDB();

                /*int diskon = 100;
                if (update_bookingDiskon.Checked)
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=1", koneksi.KoneksiDB());
                    diskon = Int32.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }
                else
                {
                    SqlCommand s = new SqlCommand("SELECT harga*100 FROM Booking_diskon where booking_diskon_id=2", koneksi.KoneksiDB());
                    diskon = Int32.Parse(s.ExecuteScalar().ToString());
                    koneksi.closeConnection();
                }*/


                float diskon = 100;
                SqlCommand s = new SqlCommand("SELECT bd.harga*100 FROM Booking_diskon bd inner join booking b on b.booking_diskon_id=bd.booking_diskon_id where booking_id=@b_id", koneksi.KoneksiDB());
                s.Parameters.AddWithValue("@b_id", update_bookingId.Text);
                diskon = float.Parse(s.ExecuteScalar().ToString());
                koneksi.closeConnection();

                SqlCommand cmd = new SqlCommand("delete from pembayaran where reservasi_id=@r_id", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                cmd = new SqlCommand("delete from reservasi where reservasi_id=@r_id", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@r_id", update_reservasiId.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                cmd = new SqlCommand("update booking set tag_kamar-=@tag_kamar, grand_total-=@grand_total, balance_due-=@balance_due where booking_id=@bokid", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("tag_kamar", biayaKamarLama);
                cmd.Parameters.AddWithValue("grand_total", biayaKamarLama);
                biayaKamarLama = (int)(biayaKamarLama * diskon) / 100;
                cmd.Parameters.AddWithValue("balance_due", biayaKamarLama - Int32.Parse(update_downpayment.Text));
                cmd.Parameters.AddWithValue("@bokid", update_bookingId.Text);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                panelUpdateReservasi.Visible = false;
                refreshGridViewDataUpdateBooking(update_bookingId.Text);
                update_reservasiId.Text = "-";
                flowLayoutPanel2.Enabled = true;
                keluarToolStripMenuItem.Enabled = true;
                btn_tambahReservasi.Enabled = true;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void bayarToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            txtJumUang.Text = "0";
            lblduidLebih.Visible = true;
            lblUseDP.Visible = true;
            txtuseDp.Visible = true;
            
            
            dataGridView3.Enabled = false;

            int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1

            DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

            SqlCommand sqlq = new SqlCommand("select max(Reservasi.reservasi_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking' or Reservasi.status='checkout')", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
            sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
            idReservasi = Convert.ToInt32(sqlq.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sqlq = new SqlCommand("select booking_id from reservasi where reservasi_id = @r_id", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("r_id", idReservasi);
            idBooking = Convert.ToInt32(sqlq.ExecuteScalar().ToString());
            koneksi.closeConnection();

            panelPembayaran.Visible = true;
            panelPembayaran.BringToFront();
            cbPembayaranReser.Text = cbPembayaranReser.Items[0].ToString();
            lblTanggalBayar.Text = DateTime.Now.Date.ToString("dd-MMM-yyyy");


            //addX
            txtuseDp.Text = "0";

            string hargaLebihA = "";
            hargaLebihA = cekHargaLebih(idBooking);
            lblduidLebih.Text = "DownPayment Booking : (Rp." + hargaLebihA + ",00)";
            if (hargaLebihA.Equals("0"))
            {
                txtuseDp.Enabled = false;
            }
            else
            {
                txtuseDp.Enabled = true;
            }
            //endX

            SqlCommand sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", idBooking);
            int kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sql1 = new SqlCommand("select isnull(statusbayar,0) booking_gabungan from Booking where booking_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", idBooking);
            int booking_gabungan = Int32.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", kodediskon);
            float potongan = float.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            SqlCommand querybayar;

            sql1 = new SqlCommand("SELECT sum(biayadiskon) harga FROM dbo.infoReservasi(@a,null)", koneksi.KoneksiDB());
            sql1.Parameters.AddWithValue("@a", idBooking);
            int harga_booking_gabungan = Int32.Parse(sql1.ExecuteScalar().ToString());
            koneksi.closeConnection();

            /**/
            querybayar = new SqlCommand("select downpayment,tag_kamar,tag_restoran from Reservasi where reservasi_id = @id", koneksi.KoneksiDB());
            querybayar.Parameters.AddWithValue("@id", idReservasi);
            SqlDataReader readD = querybayar.ExecuteReader();
            int tagihankamar = 0;
            int diskon = 0;
            while (readD.Read())
            {

                tagihankamar = (int)(Int32.Parse(readD["tag_kamar"].ToString()) * potongan )/100;
                tagihankamar += Int32.Parse(readD["tag_restoran"].ToString());
                diskon = Int32.Parse(readD["downpayment"].ToString());
            }
            koneksi.closeConnection();
            
            int totalbiayakamar = tagihankamar - diskon;

            koneksi.closeConnection();
            
            if (totalbiayakamar < 0)
            {
                lblBiayaTag.Text = "Tagihan : Rp.0,00";
                lblBiayaTag.Tag = "0";
            }
            else
            {

                if (booking_gabungan == 1)
                {
                    //    MessageBox.Show("Booking Gabungan seharga " + harga_booking_gabungan.ToString());
                    lblBiayaTag.Text = "Tagihan : Rp." + harga_booking_gabungan.ToString() + ",00";
                    //lblBiayaTag.Text = "Tagihan : Rp." + totalbiayakamar.ToString() + ",00" + "  Tag Kamar" + tagihankamar.ToString() + "  Potongan" + potongan.ToString();

                    lblBiayaTag.Tag = harga_booking_gabungan.ToString();


                }
                else
                {
                    lblBiayaTag.Text = "Tagihan : Rp." + totalbiayakamar.ToString() + ",00";
                    //lblBiayaTag.Text = "Tagihan : Rp." + totalbiayakamar.ToString() + ",00" + "  Tag Kamar" + tagihankamar.ToString() + "  Potongan" + potongan.ToString();

                    lblBiayaTag.Tag = totalbiayakamar.ToString();
                }
                
            
            }
            
        }

        private void pembayaran_exit_MouseHover(object sender, EventArgs e)
        {
            pembayaran_exit.BackColor = System.Drawing.Color.Red;
        }

        private void pembayaran_exit_Click(object sender, EventArgs e)
        {
            panelPembayaran.SendToBack();
            dataGridView3.Enabled = true;
        }

        private void pembayaran_exit_MouseLeave(object sender, EventArgs e)
        {
            pembayaran_exit.BackColor = System.Drawing.Color.FromArgb(255,100,100);
        }

        private void availableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlCommand queryStatus = new SqlCommand("update Kamar set status= NULL where kamar_no=@a",koneksi.KoneksiDB());
            queryStatus.Parameters.AddWithValue("@a", kamarStatus);
            queryStatus.ExecuteNonQuery();
            koneksi.closeConnection();
            setTampilanKamar();
            if (houseKeepingToolStripMenuItem.Enabled == false)
            {

                panelCleaningBy.BringToFront();
                lblNoKamarClean.Text = kamarStatus.ToString();
                cbNamaPegawaiCLeaning.Items.Clear();
                txtNoteCleaning.Text = "";

                SqlCommand list = new SqlCommand("select nama from Staff where Id_jabatan = 7", koneksi.KoneksiDB());
                SqlDataReader readList = list.ExecuteReader();
                while (readList.Read())
                {
                    cbNamaPegawaiCLeaning.Items.Add(readList["nama"].ToString());
                }
                koneksi.closeConnection();
            }
        
        }

        private void houseKeepingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlCommand queryStatus = new SqlCommand("update Kamar set status= '1' where kamar_no=@a", koneksi.KoneksiDB());
            queryStatus.Parameters.AddWithValue("@a", kamarStatus);
            queryStatus.ExecuteNonQuery();
            koneksi.closeConnection();
            setTampilanKamar();
        }

        private void maintenanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlCommand queryStatus = new SqlCommand("update Kamar set status= '2' where kamar_no=@a", koneksi.KoneksiDB());
            queryStatus.Parameters.AddWithValue("@a", kamarStatus);
            queryStatus.ExecuteNonQuery();
            koneksi.closeConnection();
            setTampilanKamar();
        }

        private void btnRekapHariIni_Click(object sender, EventArgs e)
        {
            refreshActivatedButton();
            btnRekapHariIni.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnRekapHariIni.FlatAppearance.BorderSize = 2;
            this.infoSubPendapatan.EnforceConstraints = false;

            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.DataTableInfoSubTableAdapter.Fill(this.infoSubPendapatan.DataTableInfoSub, DateTime.Today.Date.ToString("yyyy-MM-dd"));
            
            reportRekapHariIni.Reset();
            reportRekapHariIni.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.SubLaporan_Pendapatan_Harian.rdlc";
            reportRekapHariIni.LocalReport.DataSources.Add(new ReportDataSource("DataSetSub", (object)infoSubPendapatan.DataTableInfoSub));
            reportRekapHariIni.LocalReport.DataSources.Add(new ReportDataSource("infoHotel", (object)infoHotel.IDHotel));

            reportRekapHariIni.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            reportRekapHariIni.LocalReport.SetParameters(parameter);
          
            reportRekapHariIni.BringToFront();
            reportRekapHariIni.RefreshReport();
        }

        private void panelCatatanCLosse_Click(object sender, EventArgs e)
        {
            panelCatatanBook.SendToBack();
            txtCatatanBook.Text = "";
        }

        int bokidCatatan = 0;
        private void catatanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelCatatanBook.BringToFront();
            int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1

            DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());


            SqlCommand sqlq = new SqlCommand("select max(Reservasi.booking_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking')", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
            sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
            bokidCatatan = Convert.ToInt32(sqlq.ExecuteScalar().ToString());
            koneksi.closeConnection();
            sqlq = new SqlCommand("select note from Booking where booking_id=@a", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@a", bokidCatatan);
            txtCatatanBook.Text = sqlq.ExecuteScalar().ToString();
            koneksi.closeConnection();


        }

        private void btnsubmitcatatanbook_Click(object sender, EventArgs e)
        {
            if (bokidCatatan > 0)
            {
                SqlCommand sqlq = new SqlCommand("update Booking set note = @a where booking_id=@b", koneksi.KoneksiDB());
                sqlq.Parameters.AddWithValue("@a", txtCatatanBook.Text);
                sqlq.Parameters.AddWithValue("@b", bokidCatatan);
                sqlq.ExecuteNonQuery();
                koneksi.closeConnection();
                panelCatatanBook.SendToBack();
                bokidCatatan = 0;
            }
        }

        private void tambahNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelCatatanBook.BringToFront();
            SqlCommand sqlq = new SqlCommand("select booking_id from Reservasi where kamar_no = @dataKamar and status='checkin'", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@dataKamar", dataKamarCh);
            bokidCatatan = Convert.ToInt32(sqlq.ExecuteScalar().ToString());
            koneksi.closeConnection();

            sqlq = new SqlCommand("select note from Booking where booking_id=@a", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@a", bokidCatatan);
            txtCatatanBook.Text = sqlq.ExecuteScalar().ToString();
            koneksi.closeConnection();

        }

        private void panelCatatanCLosse_MouseHover(object sender, EventArgs e)
        {
            panelCatatanCLosse.BackColor = System.Drawing.Color.Red;
        }

        private void panelCatatanCLosse_MouseLeave(object sender, EventArgs e)
        {
            panelCatatanCLosse.BackColor = System.Drawing.Color.FromArgb(255,100,100);
        }

        private void diskonAngka_TextChanged(object sender, EventArgs e)
        {

            try
            {
                comboBox4.SelectedIndex = 0;

                int diskonA = Int32.Parse(diskonAngka.Text);
                float totalDiskon = (float)(diskonA * 100) / totalBiaya;
                DiskonPersen.Text = totalDiskon.ToString();
            }catch
            {
                diskonAngka.Text = "0";
            }
        }

        private void printInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlCommand sql = new SqlCommand("select max(noPemesanan) from HRestaurant where noMeja =@a", koneksi.KoneksiDB());
            sql.Parameters.AddWithValue("@a", noMejaDiclick);
            int nopesan = Int32.Parse(sql.ExecuteScalar().ToString());
            koneksi.closeConnection();
            
            //panelPembayaranRestoran.Visible = false;
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.dataTable1TableAdapter.Fill(this.inforRestoran.DataTable1, nopesan);
            //panelReportRestoran.BringToFront();
            reportInvoiceRestoran.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            reportInvoiceRestoran.LocalReport.SetParameters(parameter);
       
            reportInvoiceRestoran.LocalReport.Refresh();
            reportInvoiceRestoran.BringToFront();
            reportInvoiceRestoran.Refresh();
            reportInvoiceRestoran.RefreshReport();
        }

        private void cb_tipeItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshGridViewDataItem();
        }

        DateTime[] jamLogout = new DateTime[4];
        void isiArrayJamLogout()
        {
            cmd = new SqlCommand("select jam_logout1, jam_logout2, jam_logout3, jam_logout4 from idhotel",koneksi.KoneksiDB());
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            jamLogout[0] = DateTime.ParseExact(reader.GetValue(0).ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
            jamLogout[1] = DateTime.ParseExact(reader.GetValue(1).ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
            jamLogout[2] = DateTime.ParseExact(reader.GetValue(2).ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
            jamLogout[3] = DateTime.ParseExact(reader.GetValue(3).ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);

            koneksi.closeConnection();
        }

        private void panelPengaturanHotel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelCheckOut_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkOutBookingToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SqlCommand sqlttd = new SqlCommand("select tandatangan from IDHotel", koneksi.KoneksiDB());
            string b = sqlttd.ExecuteScalar().ToString();
            koneksi.closeConnection();

            if (b.Equals("Ya"))
            {
                CekTtd = 2;
                paneltandatangan.BringToFront();
                bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                }
                pictureBox1.Image = bmp;
                SqlCommand queryData = new SqlCommand("select booking_id from Reservasi where status='checkin' and kamar_no = @a", koneksi.KoneksiDB());
                queryData.Parameters.AddWithValue("@a", dataKamarCh);
                koBookTtd = queryData.ExecuteScalar().ToString();
                koneksi.closeConnection();

                SubmitTandaTangan = 2;

            }
            else
            {
                DialogResult result = MessageBox.Show("Anda yakin untuk mengcheckoutkan semua booking kamar ini", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SqlCommand queryData = new SqlCommand("select booking_id from Reservasi where status='checkin' and kamar_no = @a", koneksi.KoneksiDB());
                    queryData.Parameters.AddWithValue("@a", dataKamarCh);
                    int idbookingData = Int32.Parse(queryData.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    queryData = new SqlCommand(@"select isnull(
                        (((select isnull(bd.harga*100,0) from Booking_Diskon bd where bd.booking_diskon_id=bok.booking_diskon_id)
                        *
                        (select  isnull(sum(tag_kamar),0) from Reservasi where booking_id=bok.booking_id)
                        /100
                        )+
                        (select  isnull(sum(tag_restoran),0) from Reservasi where booking_id=bok.booking_id)
                        )-
                        ISNULL((select  isnull(sum(jumlahpayment),0) from pembayaran where booking_id=bok.booking_id),0)
                    ,0)    as utang
                    from Booking bok
                    where bok.booking_id=@b_id", koneksi.KoneksiDB());
                    queryData.Parameters.AddWithValue("@b_id", idbookingData);
                    int utang = Convert.ToInt32(float.Parse(queryData.ExecuteScalar().ToString()));
                    koneksi.closeConnection();

                    if (utang > 100)
                    {
                        lbl_checkout.Text = "Booking ini masih memiliki utang Rp. " + utang;
                        lbl_checkout.Tag = "booking";

                        panelCheckOut.BringToFront();
                    }
                    else
                    {
                        queryData = new SqlCommand("update Reservasi set checkout=@tggal where booking_id =@a and status='checkin'", koneksi.KoneksiDB());
                        queryData.Parameters.AddWithValue("@a", idbookingData);
                        queryData.Parameters.AddWithValue("@tggal", DateTime.Now);
                        queryData.ExecuteNonQuery();
                        koneksi.closeConnection();

                        queryData = new SqlCommand("select kamar_no from Reservasi where booking_id=@a  and status='checkin'", koneksi.KoneksiDB());
                        queryData.Parameters.AddWithValue("@a", idbookingData);
                        SqlDataReader readerData = queryData.ExecuteReader();
                        ArrayList list = new ArrayList();
                        while (readerData.Read())
                        {
                            list.Add(Int32.Parse(readerData["kamar_no"].ToString()));

                        }
                        koneksi.closeConnection();

                        queryData = new SqlCommand("update Reservasi set status='checkout' where booking_id =@a and status='checkin'", koneksi.KoneksiDB());
                        queryData.Parameters.AddWithValue("@a", idbookingData);
                        queryData.ExecuteNonQuery();
                        koneksi.closeConnection();

                        foreach (int i in list)
                        {
                            SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@id", i);
                            sql.ExecuteNonQuery();
                            koneksi.closeConnection();

                            sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@room", i);
                            reader = sql.ExecuteReader();
                            while (reader.Read())
                            {
                                if (reader.GetValue(2).ToString().Equals("Expen"))
                                {
                                    cmd = new SqlCommand("INSERT INTO StockExpen(ItemName, Total, Time) VALUES(@a, @b, @c)", koneksi.KoneksiDB());
                                    cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                    cmd.Parameters.AddWithValue("@b", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                    cmd.Parameters.AddWithValue("@c", DateTime.Now);
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                                    cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                    cmd.Parameters.AddWithValue("@b", 'R');
                                    cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                    cmd.Parameters.AddWithValue("@d", 'L');
                                    cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                                    cmd.Parameters.AddWithValue("@f", DateTime.Now);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            koneksi.closeConnection();

                        }
                        btnCheckInStatus_Click(sender, e);
                    }
                }
                       
            }
            
        }
        String nilaiIDKamar;
        int SubmitTandaTangan = 0;
        private void checkOutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SqlCommand sqlttd = new SqlCommand("select tandatangan from IDHotel", koneksi.KoneksiDB());
            string b = sqlttd.ExecuteScalar().ToString();
            koneksi.closeConnection();

            if (b.Equals("Ya"))
            {

                CekTtd = 1;
                paneltandatangan.BringToFront();
                bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                }
                pictureBox1.Image = bmp;
                SqlCommand querybayar = new SqlCommand("select reservasi_id from Reservasi where kamar_no = @a and status='checkin'", koneksi.KoneksiDB());
                querybayar.Parameters.AddWithValue("@a", dataKamarCh);
                nilaiIDKamar = querybayar.ExecuteScalar().ToString();
                koneksi.closeConnection();
                SubmitTandaTangan = 1;
            }
            else
            {
                DialogResult result = MessageBox.Show("Anda yakin untuk mengcheckoutkan reservasi kamar ini", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SqlCommand queryData = new SqlCommand("select booking_id from Reservasi where status='checkin' and kamar_no = @a", koneksi.KoneksiDB());
                    queryData.Parameters.AddWithValue("@a", dataKamarCh);
                    int idbookingData = Int32.Parse(queryData.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    queryData = new SqlCommand("select reservasi_id from Reservasi where status='checkin' and kamar_no = @a", koneksi.KoneksiDB());
                    queryData.Parameters.AddWithValue("@a", dataKamarCh);
                    int idreservasiData = Int32.Parse(queryData.ExecuteScalar().ToString());
                    koneksi.closeConnection();

                    queryData = new SqlCommand(@"select
                        (((select bd.harga*100 from Booking_Diskon bd where bd.booking_diskon_id=bok.booking_diskon_id)
                        *
                        (select sum(tag_kamar) from Reservasi where reservasi_id=@r_id)
                        /100
                        )+
                        (select sum(tag_restoran) from Reservasi where reservasi_id=@r_id)
                        )-
                        ISNULL((select sum(jumlahpayment) from pembayaran where reservasi_id=@r_id),0)
                        as utang
                    from Booking bok
                    where bok.booking_id=@b_id", koneksi.KoneksiDB());
                    queryData.Parameters.AddWithValue("@b_id", idbookingData);
                    queryData.Parameters.AddWithValue("@r_id", idreservasiData);
                    int utang = Convert.ToInt32(float.Parse(queryData.ExecuteScalar().ToString()));
                    koneksi.closeConnection();

                    if (utang > 100)
                    {
                        queryData = new SqlCommand(@"select
                        (((select bd.harga*100 from Booking_Diskon bd where bd.booking_diskon_id=bok.booking_diskon_id)
                        *
                        (select sum(tag_kamar) from Reservasi where booking_id=bok.booking_id)
                        /100
                        )+
                        (select sum(tag_restoran) from Reservasi where booking_id=bok.booking_id)
                        )-
                        ISNULL((select sum(jumlahpayment) from pembayaran where booking_id=bok.booking_id),0)
                        as utang
                    from Booking bok
                    where bok.booking_id=@b_id", koneksi.KoneksiDB());
                        queryData.Parameters.AddWithValue("@b_id", idbookingData);
                        int utangB = Convert.ToInt32(float.Parse(queryData.ExecuteScalar().ToString()));
                        koneksi.closeConnection();
                        if (utangB > 100)
                        {
                            lbl_checkout.Text = "Kamar ini masih memiliki utang Rp. " + utang;
                            lbl_checkout.Tag = "kamar";

                            panelCheckOut.BringToFront();
                        }
                        else
                        {
                            SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@id", dataKamarCh);
                            sql.ExecuteNonQuery();
                            koneksi.closeConnection();

                            sql = new SqlCommand("update Reservasi set checkout=@tggal where kamar_no = @id and status='checkin' ", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@tggal", DateTime.Now);
                            sql.Parameters.AddWithValue("@id", dataKamarCh);
                            sql.ExecuteNonQuery();
                            koneksi.closeConnection();

                            sql = new SqlCommand("update Reservasi set status= 'checkout' where kamar_no = @id and (datediff(minute,checkout,@tggal)<2 and datediff(minute,checkout,@tggal)>=0) and status='checkin' ", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@tggal", DateTime.Now);
                            sql.Parameters.AddWithValue("@id", dataKamarCh);
                            sql.ExecuteNonQuery();
                            koneksi.closeConnection();

                            btnCheckInStatus_Click(sender, e);

                            sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@room", dataKamarCh);
                            reader = sql.ExecuteReader();
                            while (reader.Read())
                            {
                                if (reader.GetValue(2).ToString().Equals("Expen"))
                                {
                                    cmd = new SqlCommand("INSERT INTO StockExpen(ItemName, Total, Time) VALUES(@a, @b, @c)", koneksi.KoneksiDB());
                                    cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                    cmd.Parameters.AddWithValue("@b", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                    cmd.Parameters.AddWithValue("@c", DateTime.Now);
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                                    cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                    cmd.Parameters.AddWithValue("@b", 'R');
                                    cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                    cmd.Parameters.AddWithValue("@d", 'L');
                                    cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                                    cmd.Parameters.AddWithValue("@f", DateTime.Now);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            koneksi.closeConnection();
                        }
                    }
                    else
                    {
                        SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@id", dataKamarCh);
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sql = new SqlCommand("update Reservasi set checkout=@tggal where kamar_no = @id and status='checkin' ", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@tggal", DateTime.Now);
                        sql.Parameters.AddWithValue("@id", dataKamarCh);
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();

                        sql = new SqlCommand("update Reservasi set status= 'checkout' where kamar_no = @id and (datediff(minute,checkout,@tggal)<2 and datediff(minute,checkout,@tggal)>=0) and status='checkin' ", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@tggal", DateTime.Now);
                        sql.Parameters.AddWithValue("@id", dataKamarCh);
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();

                        btnCheckInStatus_Click(sender, e);

                        sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@room", dataKamarCh);
                        reader = sql.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader.GetValue(2).ToString().Equals("Expen"))
                            {
                                cmd = new SqlCommand("INSERT INTO StockExpen(ItemName, Total, Time) VALUES(@a, @b, @c)", koneksi.KoneksiDB());
                                cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                cmd.Parameters.AddWithValue("@b", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                cmd.Parameters.AddWithValue("@c", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                                cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                cmd.Parameters.AddWithValue("@b", 'R');
                                cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                cmd.Parameters.AddWithValue("@d", 'L');
                                cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                                cmd.Parameters.AddWithValue("@f", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        koneksi.closeConnection();
                    }
                }
                     
            }
        }

        private void btn_tmbhPendingBill_Click(object sender, EventArgs e)
        {
            if (lbl_checkout.Tag.Equals("booking"))
            {
                SqlCommand queryData = new SqlCommand("select booking_id from Reservasi where status='checkin' and kamar_no = @a", koneksi.KoneksiDB());
                queryData.Parameters.AddWithValue("@a", dataKamarCh);
                int idbookingData = Int32.Parse(queryData.ExecuteScalar().ToString());
                koneksi.closeConnection();

                queryData = new SqlCommand("update Reservasi set checkout=@tggal where booking_id =@a and status='checkin'", koneksi.KoneksiDB());
                queryData.Parameters.AddWithValue("@a", idbookingData);
                queryData.Parameters.AddWithValue("@tggal", DateTime.Now);
                queryData.ExecuteNonQuery();
                koneksi.closeConnection();

                queryData = new SqlCommand("select kamar_no from Reservasi where booking_id=@a  and status='checkin'", koneksi.KoneksiDB());
                queryData.Parameters.AddWithValue("@a", idbookingData);
                SqlDataReader readerData = queryData.ExecuteReader();
                ArrayList list = new ArrayList();
                while (readerData.Read())
                {
                    list.Add(Int32.Parse(readerData["kamar_no"].ToString()));

                }
                koneksi.closeConnection();

                queryData = new SqlCommand("update Reservasi set status='checkout' where booking_id =@a and status='checkin'", koneksi.KoneksiDB());
                queryData.Parameters.AddWithValue("@a", idbookingData);
                queryData.ExecuteNonQuery();
                koneksi.closeConnection();

                foreach (int i in list)
                {
                    SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@id", i);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@room", i);
                    reader = sql.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.GetValue(2).ToString().Equals("Expen"))
                        {
                            cmd = new SqlCommand("INSERT INTO StockExpen(ItemName, Total, Time) VALUES(@a, @b, @c)", koneksi.KoneksiDB());
                            cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                            cmd.Parameters.AddWithValue("@b", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                            cmd.Parameters.AddWithValue("@c", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                            cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                            cmd.Parameters.AddWithValue("@b", 'R');
                            cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                            cmd.Parameters.AddWithValue("@d", 'L');
                            cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                            cmd.Parameters.AddWithValue("@f", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    koneksi.closeConnection();

                }
                btnCheckInStatus_Click(sender, e);

            }
            else if(lbl_checkout.Tag.Equals("kamar"))
            {
                SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@id", dataKamarCh);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand("update Reservasi set checkout=@tggal where kamar_no = @id and status='checkin' ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@tggal", DateTime.Now);
                sql.Parameters.AddWithValue("@id", dataKamarCh);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand("update Reservasi set status= 'checkout' where kamar_no = @id and (datediff(minute,checkout,@tggal)<2 and datediff(minute,checkout,@tggal)>=0) and status='checkin' ", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@tggal", DateTime.Now);
                sql.Parameters.AddWithValue("@id", dataKamarCh);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                btnCheckInStatus_Click(sender, e);

                sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@room", dataKamarCh);
                reader = sql.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetValue(2).ToString().Equals("Expen"))
                    {
                        cmd = new SqlCommand("INSERT INTO StockExpen(ItemName, Total, Time) VALUES(@a, @b, @c)", koneksi.KoneksiDB());
                        cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                        cmd.Parameters.AddWithValue("@b", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                        cmd.Parameters.AddWithValue("@c", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                        cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                        cmd.Parameters.AddWithValue("@b", 'R');
                        cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                        cmd.Parameters.AddWithValue("@d", 'L');
                        cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                        cmd.Parameters.AddWithValue("@f", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                koneksi.closeConnection();
            }
        }

        private void btn_kembali_Click(object sender, EventArgs e)
        {
            panelCheckOut.SendToBack();
        }

        private void checkout_close_Click(object sender, EventArgs e)
        {
            panelCheckOut.SendToBack();
        }

        private void inputNamaTamu_TextChanged(object sender, EventArgs e)
        {
            /*groupBukuTamu.Visible = true;
            groupBukuTamu.Height = 500;
            //groupBukuTamu.Dock = DockStyle.Top;
            groupBukuTamu.BringToFront();
            refreshGridDataTamu(datagridTamu);

            inputCariNamaTamu.Text = inputNamaTamu.Text;

            if(total_data_tamu<=0){
                groupBukuTamu.Visible = false;
                groupBukuTamu.SendToBack();
            }*/
        }

        private void inputNamaTamu_KeyUp(object sender, KeyEventArgs e)
        {
            groupBukuTamu.Visible = true;
            groupBukuTamu.Height = 500;
            //groupBukuTamu.Dock = DockStyle.Top;
            groupBukuTamu.BringToFront();
            refreshGridDataTamu(datagridTamu);

            inputCariNamaTamu.Text = inputNamaTamu.Text;

            if (total_data_tamu <= 0)
            {
                groupBukuTamu.Visible = false;
                groupBukuTamu.SendToBack();
            }
        }

        private void btnLoadLogoHotel_Click(object sender, EventArgs e)
        {
            openFD.Title = "Insert an image ";
            openFD.InitialDirectory = "c:";
            openFD.FileName = "";
            openFD.Filter = "JPEG Image|*.jpg|GIF Image|*.gif|PNG Image|*.png";
            openFD.Multiselect = false;
            if (openFD.ShowDialog() != DialogResult.OK)
                return;

            logoHotelShow.ImageLocation = openFD.FileName;
        }

        private void update_bookingDiskon_CheckedChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(update_bookingDiskon.Checked.ToString());
            if (update_bookingDiskon.Checked)
            {
                SqlCommand s = new SqlCommand("update booking set booking_diskon_id=1 where booking_id=@b_id", koneksi.KoneksiDB());
                s.Parameters.AddWithValue("@b_id", update_bookingId.Text);
                s.ExecuteNonQuery();
                koneksi.closeConnection();
            }
            else
            {
                SqlCommand s = new SqlCommand("update booking set booking_diskon_id=2 where booking_id=@b_id", koneksi.KoneksiDB());
                s.Parameters.AddWithValue("@b_id", update_bookingId.Text);
                s.ExecuteNonQuery();
                koneksi.closeConnection();
            }
        }

        private void bookingCorporateCheckIn_CheckedChanged(object sender, EventArgs e)
        {
            if (bookingCorporateCheckIn.Checked)
            {
                SqlCommand s = new SqlCommand("update booking set booking_diskon_id=1 where booking_id=@b_id", koneksi.KoneksiDB());
                s.Parameters.AddWithValue("@b_id", bookingIdCheckIn.Text);
                s.ExecuteNonQuery();
                koneksi.closeConnection();
            }
            else
            {
                SqlCommand s = new SqlCommand("update booking set booking_diskon_id=2 where booking_id=@b_id", koneksi.KoneksiDB());
                s.Parameters.AddWithValue("@b_id", bookingIdCheckIn.Text);
                s.ExecuteNonQuery();
                koneksi.closeConnection();
            }
        }

        private void filter_item_TextChanged(object sender, EventArgs e)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = GridView_dataItem.DataSource;
            bs.Filter = "item like '%" + filter_item.Text + "%'";
            GridView_dataItem.DataSource = bs;
        }

        private void panelBayarRestoran_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboboxPembayaran_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboboxPembayaran.Text.Equals("Simpan"))
            {

                lblPembayaranResto.Enabled = false;
                cb_jenisPembaynaranRestor.Enabled = false;
            }
            else
            {

                lblPembayaranResto.Enabled = true;
                cb_jenisPembaynaranRestor.Enabled = true;
            }
        }

        private void btn_pendapatanRestoran_Click(object sender, EventArgs e)
        {

            resetBtnKonfigurasi();
            refreshActivatedButton();

            btn_pendapatanRestoran.FlatAppearance.BorderColor = Color.CornflowerBlue;
            btn_pendapatanRestoran.FlatAppearance.BorderSize = 2;
                        
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.infoPendapatanRestoranTableAdapter.Fill(this.infoPendapatanRestoran._infoPendapatanRestoran);

            reportPendapatanRestoran.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            reportPendapatanRestoran.LocalReport.SetParameters(parameter);

            reportPendapatanRestoran.RefreshReport();
            reportPendapatanRestoran.BringToFront();

        }

        private void refreshdataTamu()
        {
            inputNamaDT.Text = ""; inputTlpnDT.Text = ""; inputEmailDT.Text = "";
            inputAlamatDT.Text = ""; inputKotaDT.Text = ""; inputPerusahaanDT.Text = "";
            inputSebutanDT.Text = ""; txtNoIdentitasPanelTamu.Text = "";
            inputGelarDT.Text = ""; cbJenisIdentitasPanelTamu.Text = "";
            inputSearchDT.Text = "";
            inputTglLhrDT.Value = Convert.ToDateTime("1900-1-1 16:58:00");
            txtwntambah.Text = "";
        }
        private void btnInsertDataTamu_Click(object sender, EventArgs e)
        {
            try
            {
                if (!inputNamaDT.Text.Equals("") && !inputTlpnDT.Text.Equals("") && !inputSebutanDT.Text.Equals(""))
                {
                    SqlCommand cmd = new SqlCommand("insert into Tamu(tamu,alamat,kota,telepon,email,perusahaan,tanggallahir,sebutan,gelar,noidentitas,jenisidentitas,warganegara) values (@a,@b,@c,@d,@e,@f,@g,@h,@i,@j,@k,@l)", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@a", inputNamaDT.Text);
                    cmd.Parameters.AddWithValue("@b", inputAlamatDT.Text);
                    cmd.Parameters.AddWithValue("@c", inputKotaDT.Text);
                    cmd.Parameters.AddWithValue("@d", inputTlpnDT.Text);
                    cmd.Parameters.AddWithValue("@e", inputEmailDT.Text);
                    cmd.Parameters.AddWithValue("@f", inputPerusahaanDT.Text);
                    cmd.Parameters.Add("@g", SqlDbType.DateTime).Value = inputTglLhrDT.Value.Date;
                    cmd.Parameters.AddWithValue("@h", inputSebutanDT.Text);
                    cmd.Parameters.AddWithValue("@i", inputGelarDT.Text);
                    cmd.Parameters.AddWithValue("@j", txtNoIdentitasPanelTamu.Text);
                    cmd.Parameters.AddWithValue("@k", cbJenisIdentitasPanelTamu.Text);
                    cmd.Parameters.AddWithValue("@l", txtwntambah.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Insert Berhasil");
                    refreshdataTamu();

                    koneksi.closeConnection();
                }
                else
                {
                    MessageBox.Show("Mohon masukkan data tamu baru.");
                }
            }
            catch
            {
                MessageBox.Show("Insert Gagal");
            }

            refreshGridDataTamu(GridViewDaftarTamu);
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void cbJnsKamarPeriodik_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboboxItem selectedCar = (ComboboxItem)cbJnsKamarPeriodik.SelectedItem;

            SqlDataAdapter da = new SqlDataAdapter("select hp.harga_periodik_id, kt.kamar_tipe , hp.tgl_berlaku, hp.harga, hp.harga_weekend from Harga_Periodik hp inner join Kamar_Tipe kt on hp.kamar_tipe_id=kt.kamar_tipe_id where year(hp.tgl_berlaku)>2008 and hp.kamar_tipe_id=@a", koneksi.KoneksiDB());
            da.SelectCommand.Parameters.AddWithValue("@a", selectedCar.Value);
            dHargaPeriodik = new DataTable();
            da.Fill(dHargaPeriodik);
            dataGridView7.DataSource = dHargaPeriodik;

            dataGridView7.Columns[0].DisplayIndex = 4;

            koneksi.closeConnection();
        }

        private void combobox_kamar_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboboxItem item = (ComboboxItem)combobox_kamar.SelectedItem;
            BindingSource bs = new BindingSource();
            bs.DataSource = dataGrid_hargaKhusus.DataSource;
            bs.Filter = "kamar_tipe like '%" + item.Text + "%'";
            dataGrid_hargaKhusus.DataSource = bs;
        }

        private void panelPesanRestoran_Click(object sender, EventArgs e)
        {
            //panelCariItem.Visible = false;
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tambahHariToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void panelKamarDibooking_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void DiskonPersen_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void DiskonPersen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                comboBox4.SelectedIndex = 0;

                diskonAngka.Text = Convert.ToString((int)(float.Parse(DiskonPersen.Text) * totalBiaya/100));

                //int diskonA = Int32.Parse(diskonAngka.Text);
                //float totalDiskon = float.Parse(diskonAngka.Text.ToString());
                //DiskonPersen.Text = totalDiskon.ToString();
            }
            catch
            {
                diskonAngka.Text = "0";
            }
        }
        private int CekTtd = 0;
        
        private void tToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CekTtd = 0;
           
            paneltandatangan.BringToFront();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
            }
            pictureBox1.Image = bmp;
            int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1

            //DateTime tanggalPesan1 = Convert.ToDateTime(TglBulan + "/" + dataGridView3.Columns[columnSelect].Name.ToString() + "/" + Tgltahun);
            DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

            SqlCommand sqlq = new SqlCommand("select max(Reservasi.booking_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok and (Reservasi.status='booking' or Reservasi.status='checkin')", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
            sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
            koBookTtd = sqlq.ExecuteScalar().ToString();
            koneksi.closeConnection();
        }
        private Point? _Previous = null;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _Previous = e.Location;
            pictureBox1_MouseMove(sender, e);
        }

        Bitmap bmp;
        Boolean cektandatngan = false;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Previous != null)
            {
                if (pictureBox1.Image == null)
                {
                    bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);
                    }
                    pictureBox1.Image = bmp;
                }
                using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                {
                    g.DrawLine(Pens.Black, _Previous.Value, e.Location);
                }
                pictureBox1.Invalidate();
                _Previous = e.Location;
                cektandatngan = true;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _Previous = null;
        }

        private void submittandatangan_Click(object sender, EventArgs e)
        {
            if (cektandatngan == true)
            {
                cektandatngan = false;
                try
                {
                    paneltandatangan.SendToBack();
                    if (CekTtd == 0)
                    {
                        bmp.Save(Application.StartupPath + "\\gambar\\" + koBookTtd + ".png", ImageFormat.Png);
                    }
                    else if (CekTtd == 1)
                    {
                        bmp.Save(Application.StartupPath + "\\gambar\\" + nilaiIDKamar + ".png", ImageFormat.Png);

                    }
                    else if (CekTtd == 2)
                    {
                        bmp.Save(Application.StartupPath + "\\gambar\\" + koBookTtd + "(2).png", ImageFormat.Png);

                    }
                    if (SubmitTandaTangan == 1)
                    {
                        DialogResult result = MessageBox.Show("Anda yakin untuk mengcheckoutkan reservasi kamar ini", "Confirmation", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            SqlCommand queryData = new SqlCommand("select booking_id from Reservasi where status='checkin' and kamar_no = @a", koneksi.KoneksiDB());
                            queryData.Parameters.AddWithValue("@a", dataKamarCh);
                            int idbookingData = Int32.Parse(queryData.ExecuteScalar().ToString());
                            koneksi.closeConnection();

                            queryData = new SqlCommand("select reservasi_id from Reservasi where status='checkin' and kamar_no = @a", koneksi.KoneksiDB());
                            queryData.Parameters.AddWithValue("@a", dataKamarCh);
                            int idreservasiData = Int32.Parse(queryData.ExecuteScalar().ToString());
                            koneksi.closeConnection();

                            queryData = new SqlCommand(@"select
                        (((select bd.harga*100 from Booking_Diskon bd where bd.booking_diskon_id=bok.booking_diskon_id)
                        *
                        (select sum(tag_kamar) from Reservasi where reservasi_id=@r_id)
                        /100
                        )+
                        (select sum(tag_restoran) from Reservasi where reservasi_id=@r_id)
                        )-
                        ISNULL((select sum(jumlahpayment) from pembayaran where reservasi_id=@r_id),0)
                        as utang
                    from Booking bok
                    where bok.booking_id=@b_id", koneksi.KoneksiDB());
                            queryData.Parameters.AddWithValue("@b_id", idbookingData);
                            queryData.Parameters.AddWithValue("@r_id", idreservasiData);
                            int utang = Convert.ToInt32(float.Parse(queryData.ExecuteScalar().ToString()));
                            koneksi.closeConnection();

                            if (utang > 100)
                            {
                                queryData = new SqlCommand(@"select
                        (((select bd.harga*100 from Booking_Diskon bd where bd.booking_diskon_id=bok.booking_diskon_id)
                        *
                        (select sum(tag_kamar) from Reservasi where booking_id=bok.booking_id)
                        /100
                        )+
                        (select sum(tag_restoran) from Reservasi where booking_id=bok.booking_id)
                        )-
                        ISNULL((select sum(jumlahpayment) from pembayaran where booking_id=bok.booking_id),0)
                        as utang
                    from Booking bok
                    where bok.booking_id=@b_id", koneksi.KoneksiDB());
                                queryData.Parameters.AddWithValue("@b_id", idbookingData);
                                int utangB = Convert.ToInt32(float.Parse(queryData.ExecuteScalar().ToString()));
                                koneksi.closeConnection();
                                if (utangB > 100)
                                {
                                    lbl_checkout.Text = "Kamar ini masih memiliki utang Rp. " + utang;
                                    lbl_checkout.Tag = "kamar";

                                    panelCheckOut.BringToFront();
                                }
                                else
                                {
                                    SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
                                    sql.Parameters.AddWithValue("@id", dataKamarCh);
                                    sql.ExecuteNonQuery();
                                    koneksi.closeConnection();

                                    sql = new SqlCommand("update Reservasi set checkout=@tggal where kamar_no = @id and status='checkin' ", koneksi.KoneksiDB());
                                    sql.Parameters.AddWithValue("@tggal", DateTime.Now);
                                    sql.Parameters.AddWithValue("@id", dataKamarCh);
                                    sql.ExecuteNonQuery();
                                    koneksi.closeConnection();

                                    sql = new SqlCommand("update Reservasi set status= 'checkout' where kamar_no = @id and (datediff(minute,checkout,@tggal)<2 and datediff(minute,checkout,@tggal)>=0) and status='checkin' ", koneksi.KoneksiDB());
                                    sql.Parameters.AddWithValue("@tggal", DateTime.Now);
                                    sql.Parameters.AddWithValue("@id", dataKamarCh);
                                    sql.ExecuteNonQuery();
                                    koneksi.closeConnection();

                                    btnCheckInStatus_Click(sender, e);

                                    sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0", koneksi.KoneksiDB());
                                    sql.Parameters.AddWithValue("@room", dataKamarCh);
                                    reader = sql.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        if (reader.GetValue(2).ToString().Equals("Expen"))
                                        {
                                            cmd = new SqlCommand("INSERT INTO StockExpen(ItemName, Total, Time) VALUES(@a, @b, @c)", koneksi.KoneksiDB());
                                            cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                            cmd.Parameters.AddWithValue("@b", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                            cmd.Parameters.AddWithValue("@c", DateTime.Now);
                                            cmd.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                                            cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                            cmd.Parameters.AddWithValue("@b", 'R');
                                            cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                            cmd.Parameters.AddWithValue("@d", 'L');
                                            cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                                            cmd.Parameters.AddWithValue("@f", DateTime.Now);
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                    koneksi.closeConnection();
                                }
                            }
                            else
                            {
                                SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
                                sql.Parameters.AddWithValue("@id", dataKamarCh);
                                sql.ExecuteNonQuery();
                                koneksi.closeConnection();

                                sql = new SqlCommand("update Reservasi set checkout=@tggal where kamar_no = @id and status='checkin' ", koneksi.KoneksiDB());
                                sql.Parameters.AddWithValue("@tggal", DateTime.Now);
                                sql.Parameters.AddWithValue("@id", dataKamarCh);
                                sql.ExecuteNonQuery();
                                koneksi.closeConnection();

                                sql = new SqlCommand("update Reservasi set status= 'checkout' where kamar_no = @id and (datediff(minute,checkout,@tggal)<2 and datediff(minute,checkout,@tggal)>=0) and status='checkin' ", koneksi.KoneksiDB());
                                sql.Parameters.AddWithValue("@tggal", DateTime.Now);
                                sql.Parameters.AddWithValue("@id", dataKamarCh);
                                sql.ExecuteNonQuery();
                                koneksi.closeConnection();

                                btnCheckInStatus_Click(sender, e);

                                sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0", koneksi.KoneksiDB());
                                sql.Parameters.AddWithValue("@room", dataKamarCh);
                                reader = sql.ExecuteReader();
                                while (reader.Read())
                                {
                                    if (reader.GetValue(2).ToString().Equals("Expen"))
                                    {
                                        cmd = new SqlCommand("INSERT INTO StockExpen(ItemName, Total, Time) VALUES(@a, @b, @c)", koneksi.KoneksiDB());
                                        cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                        cmd.Parameters.AddWithValue("@b", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                        cmd.Parameters.AddWithValue("@c", DateTime.Now);
                                        cmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                                        cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                        cmd.Parameters.AddWithValue("@b", 'R');
                                        cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                        cmd.Parameters.AddWithValue("@d", 'L');
                                        cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                                        cmd.Parameters.AddWithValue("@f", DateTime.Now);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                koneksi.closeConnection();
                            }
                        }
                        SubmitTandaTangan = 0;
                    }
                    else if (SubmitTandaTangan == 2)
                    {
                        DialogResult result = MessageBox.Show("Anda yakin untuk mengcheckoutkan semua booking kamar ini", "Confirmation", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            SqlCommand queryData = new SqlCommand("select booking_id from Reservasi where status='checkin' and kamar_no = @a", koneksi.KoneksiDB());
                            queryData.Parameters.AddWithValue("@a", dataKamarCh);
                            int idbookingData = Int32.Parse(queryData.ExecuteScalar().ToString());
                            koneksi.closeConnection();

                            queryData = new SqlCommand(@"select
                        (((select bd.harga*100 from Booking_Diskon bd where bd.booking_diskon_id=bok.booking_diskon_id)
                        *
                        (select sum(tag_kamar) from Reservasi where booking_id=bok.booking_id)
                        /100
                        )+
                        (select sum(tag_restoran) from Reservasi where booking_id=bok.booking_id)
                        )-
                        ISNULL((select sum(jumlahpayment) from pembayaran where booking_id=bok.booking_id),0)
                        as utang
                    from Booking bok
                    where bok.booking_id=@b_id", koneksi.KoneksiDB());
                            queryData.Parameters.AddWithValue("@b_id", idbookingData);
                            int utang = Convert.ToInt32(float.Parse(queryData.ExecuteScalar().ToString()));
                            koneksi.closeConnection();

                            if (utang > 100)
                            {
                                lbl_checkout.Text = "Booking ini masih memiliki utang Rp. " + utang;
                                lbl_checkout.Tag = "booking";

                                panelCheckOut.BringToFront();
                            }
                            else
                            {
                                queryData = new SqlCommand("update Reservasi set checkout=@tggal where booking_id =@a and status='checkin'", koneksi.KoneksiDB());
                                queryData.Parameters.AddWithValue("@a", idbookingData);
                                queryData.Parameters.AddWithValue("@tggal", DateTime.Now);
                                queryData.ExecuteNonQuery();
                                koneksi.closeConnection();

                                queryData = new SqlCommand("select kamar_no from Reservasi where booking_id=@a  and status='checkin'", koneksi.KoneksiDB());
                                queryData.Parameters.AddWithValue("@a", idbookingData);
                                SqlDataReader readerData = queryData.ExecuteReader();
                                ArrayList list = new ArrayList();
                                while (readerData.Read())
                                {
                                    list.Add(Int32.Parse(readerData["kamar_no"].ToString()));

                                }
                                koneksi.closeConnection();

                                queryData = new SqlCommand("update Reservasi set status='checkout' where booking_id =@a and status='checkin'", koneksi.KoneksiDB());
                                queryData.Parameters.AddWithValue("@a", idbookingData);
                                queryData.ExecuteNonQuery();
                                koneksi.closeConnection();

                                foreach (int i in list)
                                {
                                    SqlCommand sql = new SqlCommand("update Kamar set status='1' where kamar_no=@id ", koneksi.KoneksiDB());
                                    sql.Parameters.AddWithValue("@id", i);
                                    sql.ExecuteNonQuery();
                                    koneksi.closeConnection();

                                    sql = new SqlCommand(@"select ik.ItemName, ik.Jumlah, ik.Tipe
                                from Kamar k inner join InventoryKamar ik on k.kamar_tipe_id=ik.kamar_tipe_id
	                                and k.kamar_kapasitas_id=ik.kamar_kapasitas_id
                                where k.kamar_no=@room and ik.Jumlah>0", koneksi.KoneksiDB());
                                    sql.Parameters.AddWithValue("@room", i);
                                    reader = sql.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        if (reader.GetValue(2).ToString().Equals("Expen"))
                                        {
                                            cmd = new SqlCommand("INSERT INTO StockExpen(ItemName, Total, Time) VALUES(@a, @b, @c)", koneksi.KoneksiDB());
                                            cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                            cmd.Parameters.AddWithValue("@b", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                            cmd.Parameters.AddWithValue("@c", DateTime.Now);
                                            cmd.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                                            cmd.Parameters.AddWithValue("@a", reader.GetValue(0).ToString());
                                            cmd.Parameters.AddWithValue("@b", 'R');
                                            cmd.Parameters.AddWithValue("@c", Int32.Parse(reader.GetValue(1).ToString()) * -1);
                                            cmd.Parameters.AddWithValue("@d", 'L');
                                            cmd.Parameters.AddWithValue("@e", Int32.Parse(reader.GetValue(1).ToString()));
                                            cmd.Parameters.AddWithValue("@f", DateTime.Now);
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                    koneksi.closeConnection();

                                }
                                btnCheckInStatus_Click(sender, e);
                            }
                        }
                        SubmitTandaTangan = 0;
                    }
                    pictureBox1.Image = null;
                    bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);
                    }
                    pictureBox1.Image = bmp;

                }
                catch
                {

                }
            }
            else
            {
                MessageBox.Show("Pastikan Customer Tanda Tangan");
            }
        }
        
        private string koBookTtd;

        private void btnLaporanBaru_Click(object sender, EventArgs e)
        {

            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            string tggalLaporan = DateTime.Now.Date.ToString("yyyy-MM-dd");
            this.dataTable1TableAdapter2.Fill(this.InfoGrandTotal.DataTable1, tggalLaporan);

            reportLaporanBooking.Reset();
            reportLaporanBooking.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.Report_Tagihan_Harian.rdlc";
            reportLaporanBooking.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", (object)InfoGrandTotal.DataTable1));
            reportLaporanBooking.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", (object)infoHotel.IDHotel));

            reportLaporanBooking.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            reportLaporanBooking.LocalReport.SetParameters(parameter);
            
            //reportLaporanBooking.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
            reportLaporanBooking.RefreshReport();
            reportLaporanBooking.BringToFront();
        }

        private void btn_Konfigurasi_Click(object sender, EventArgs e)
        {
            resetBtnLaporan();

            refreshActivatedButton();
            btn_Konfigurasi.FlatAppearance.BorderColor = Color.DodgerBlue;
            btn_Konfigurasi.FlatAppearance.BorderSize = 2;
            
            btnPengaturanKamar.Size = new Size(183, 30);
            btnPengaturanKamar.Margin = new Padding(30,3,3,3);

            btnPengaturanHotel.Size = new Size(183, 30);
            btnPengaturanHotel.Margin = new Padding(30, 3, 3, 3);

            btnPengaturanHarga.Size = new Size(183, 30);
            btnPengaturanHarga.Margin = new Padding(30, 3, 3, 3);

            btn_harga_khusus.Size = new Size(183, 30);
            btn_harga_khusus.Margin = new Padding(30, 3, 3, 3);

            btnPeriodik.Size = new Size(183, 30);
            btnPeriodik.Margin = new Padding(30, 3, 3, 3);

            btn_pengaturan_item.Size = new Size(183, 30);
            btn_pengaturan_item.Margin = new Padding(30, 3, 3, 3);

            btnRights.Size = new Size(183, 30);
            btnRights.Margin = new Padding(30, 3, 3, 3);

            btnUser.Size = new Size(183, 30);
            btnUser.Margin = new Padding(30, 3, 3, 3);

            btn_stockExpen.Size = new Size(183, 30);
            btn_stockExpen.Margin = new Padding(30, 3, 3, 3);

            btn_inventorykamar.Size = new Size(183, 30);
            btn_inventorykamar.Margin = new Padding(30, 3, 3, 3);

            panelCheckinDate.Visible = false;
            panelCheckoutDate.Visible = false;
        
        }

        private void btnLaporan_Click(object sender, EventArgs e)
        {
            resetBtnKonfigurasi();
            refreshActivatedButton();
            btnLaporan.FlatAppearance.BorderColor = Color.DodgerBlue;
            btnLaporan.FlatAppearance.BorderSize = 2;

            btnLaporanKeuangan.Size = new Size(183, 30);
            btnLaporanKeuangan.Margin = new Padding(30, 3, 3, 3);

            btnLaporanGrandTotal.Size = new Size(183, 30);
            btnLaporanGrandTotal.Margin = new Padding(30, 3, 3, 3);

            btnUtng.Size = new Size(183, 30);
            btnUtng.Margin = new Padding(30, 3, 3, 3);

            btnRekapHariIni.Size = new Size(183, 30);
            btnRekapHariIni.Margin = new Padding(30, 3, 3, 3);

            btn_pendapatanRestoran.Size = new Size(183, 30);
            btn_pendapatanRestoran.Margin = new Padding(30, 3, 3, 3);

            btnLaporanWNA.Size = new Size(183, 30);
            btnLaporanWNA.Margin = new Padding(30, 3, 3, 3);

            btnLaporanTandaTangan.Size = new Size(183, 30);
            btnLaporanTandaTangan.Margin = new Padding(30, 3, 3, 3);

            btnLaporanTopCorp.Size = new Size(183, 30);
            btnLaporanTopCorp.Margin = new Padding(30, 3, 3, 3);

            panelCheckinDate.Visible = false;
            panelCheckoutDate.Visible = false;
        
        }

        private void btnLaporanGrandTotal_Click(object sender, EventArgs e)
        {

            //btnLaporanGrandTotal.FlatAppearance.BorderColor = Color.CornflowerBlue;
            //btnLaporanGrandTotal.FlatAppearance.BorderSize = 2;

            ///////////////////////
            //this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            //string tggalLaporan = DateTime.Now.Date.ToString("yyyy-MM-dd");
            //this.dataTable1TableAdapter2.Fill(this.InfoGrandTotal.DataTable1, tggalLaporan);

            //reportLaporanBooking.Reset();
            //reportLaporanBooking.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.Report_Tagihan_Harian.rdlc";
            //reportLaporanBooking.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", (object)InfoGrandTotal.DataTable1));
            //reportLaporanBooking.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", (object)infoHotel.IDHotel));

            //reportLaporanBooking.LocalReport.EnableExternalImages = true;
            //string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            //ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            //reportLaporanBooking.LocalReport.SetParameters(parameter);

            ////reportLaporanBooking.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
            //reportLaporanBooking.RefreshReport();
            //reportLaporanBooking.BringToFront();     
                        
            ///////////////////


            resetBtnKonfigurasi();
            refreshActivatedButton();
            btnLaporanGrandTotal.FlatAppearance.BorderColor = Color.CornflowerBlue;
            btnLaporanGrandTotal.FlatAppearance.BorderSize = 2;

            flowLayoutPanel1.Visible = false;

            HideBtnStatusKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            //panelLaporanKeuangan.BringToFront();
            /*List<Microsoft.Reporting.WinForms.ReportParameter> list = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("tahun", "2014");
            list.Add(param);
            Microsoft.Reporting.WinForms.ReportParameter param2 = new Microsoft.Reporting.WinForms.ReportParameter("bulan", "8");
            list.Add(param2);
            reportInvoice.ServerReport.ReportPath = "/Invoice/Laporan_Pendapatan_Harian";
            reportInvoice.ServerReport.SetParameters(list);
            //reportInvoice.ServerReport.Refresh();
            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
            List<Microsoft.Reporting.WinForms.ReportParameter> parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            reportInvoice.ServerReport.SetParameters(parameter_reset);*/
            switch (DateTime.Now.Month)
            {
                case 1: comboBoxGrandTotalBulan.Text = "Januari"; break;
                case 2: comboBoxGrandTotalBulan.Text = "Februari"; break;
                case 3: comboBoxGrandTotalBulan.Text = "Maret"; break;
                case 4: comboBoxGrandTotalBulan.Text = "April"; break;
                case 5: comboBoxGrandTotalBulan.Text = "Mei"; break;
                case 6: comboBoxGrandTotalBulan.Text = "Juni"; break;
                case 7: comboBoxGrandTotalBulan.Text = "Juli"; break;
                case 8: comboBoxGrandTotalBulan.Text = "Agustus"; break;
                case 9: comboBoxGrandTotalBulan.Text = "September"; break;
                case 10: comboBoxGrandTotalBulan.Text = "Oktober"; break;
                case 11: comboBoxGrandTotalBulan.Text = "November"; break;
                default: comboBoxGrandTotalBulan.Text = "Desember"; break;
            }
            ////khusus rilis tabhotel 12122014
            //////comboBoxGrandTotalBulan.Visible = false;
            //////comboBoxGrandTotalTahun.Visible = false;
            ////khusus rilis tabhotel 12122014
            cekPilihLaporanGrandTotal = true;
            comboBoxGrandTotalTahun.Text = DateTime.Now.Year.ToString();
            flowLayoutGrandTotalInput.Visible = true;
            cekPilihLaporan = true;
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.dataTable1TableAdapter1.Fill(this.infoPendapatan.DataTable1, DateTime.Now.Year, DateTime.Now.Month);
            this.BookingTableAdapter.Fill(this.infoTagihanLunasDataSet.Booking, DateTime.Now.Year, DateTime.Now.Month);
            this.infoKontrolKeuanganTableAdapter.Fill(this.infoKontrolKeuanganDataSet2.infoKontrolKeuangan, DateTime.Now.Year, DateTime.Now.Month);

            reportTagihanLunas.Reset();

            reportTagihanLunas.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.Laporan_Kontrol_Keuangan.rdlc";
            reportTagihanLunas.LocalReport.DataSources.Add(new ReportDataSource("infoPendapatan", (object)infoPendapatan.DataTable1));
            reportTagihanLunas.LocalReport.DataSources.Add(new ReportDataSource("infoHotel", (object)infoHotel.IDHotel));
            reportTagihanLunas.LocalReport.DataSources.Add(new ReportDataSource("infoTagihanLunas", (object)infoTagihanLunasDataSet.Booking));
            reportTagihanLunas.LocalReport.DataSources.Add(new ReportDataSource("infoKontrolKeuangan", (object)infoKontrolKeuanganDataSet2.infoKontrolKeuangan));
            

            reportTagihanLunas.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            reportTagihanLunas.LocalReport.SetParameters(parameter);

            reportTagihanLunas.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
            reportTagihanLunas.RefreshReport();
            reportTagihanLunas.BringToFront();
            
        }

        private void btnLaporanBaru(object sender, EventArgs e)
        {

        }

        private void btn_tandatangan_Click(object sender, EventArgs e)
        {
            CekTtd = 0;
            paneltandatangan.BringToFront();

            SqlCommand sqlq = new SqlCommand("select IDENT_CURRENT('Booking')+1", koneksi.KoneksiDB());
            koBookTtd = sqlq.ExecuteScalar().ToString();
            koneksi.closeConnection();
        }

        private void btn_stockExpen_Click(object sender, EventArgs e)
        {
            cb_tambahStockE.Items.Clear();
            cb_tambahStockE.Text = "";
            jumlahTambahStockE.Text = "0";

            DataSet stockEx = new DataSet();
            da = new SqlDataAdapter("SELECT ItemName as Name, SUM(total) as Stock FROM StockExpen GROUP BY ItemName", koneksi.KoneksiDB());
            da.Fill(stockEx, "stockEx");
            dataGrid_StockExpen.DataSource = stockEx;
            dataGrid_StockExpen.DataMember = "stockEx";
            koneksi.closeConnection();

            cmd = new SqlCommand("select distinct ItemName from StockExpen", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cb_tambahStockE.Items.Add(reader.GetValue(0).ToString());
            }
            koneksi.closeConnection();


            cb_tambahStockR.Items.Clear();
            cb_tambahStockR.Text = "";
            jumlahRec.Text = "0";
            DataSet stockRec = new DataSet();
            da = new SqlDataAdapter(@"select ItemName as Name, ISNULL([WH],0) WH, ISNULL([HK],0) HK, 
	                ISNULL([R],0) R, ISNULL([L],0) L, ISNULL([P],0) P
                    from
                    (
		                select ItemName, tempatAsal as tempat, sum(JumlahAsal) as total
		                from stockrec
		                where tempatAsal is not null
		                group by ItemName, tempatAsal
		                union
		                select ItemName, tempatTujuan, sum(JumlahTujuan) as total
		                from stockrec
		                where tempatTujuan is not null
		                group by ItemName, tempatTujuan
                    ) b
                    pivot
                    (
	                    sum(b.total)
	                    for b.tempat in ([WH],[HK],[R],[L],[P])
                    ) c ", koneksi.KoneksiDB());
            da.Fill(stockRec, "stockRec");
            dataGrid_StockRec.DataSource = stockRec;
            dataGrid_StockRec.DataMember = "stockRec";
            koneksi.closeConnection();

            cmd = new SqlCommand("select distinct ItemName from StockRec", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cb_tambahStockR.Items.Add(reader.GetValue(0).ToString());
            }
            koneksi.closeConnection();

            DariRec.Items.Clear();
            /*cmd = new SqlCommand(@"select distinct ISNULL(TempatAsal, '-') as tempat
                    from StockRec
                    union
                    select distinct ISNULL(TempatTujuan, '-')
                    from StockRec", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DariRec.Items.Add(reader.GetValue(0).ToString());
            }
            koneksi.closeConnection();*/
            DariRec.Items.Add("-");
            DariRec.Items.Add("WH");
            DariRec.Items.Add("HK");
            DariRec.Items.Add("R");
            DariRec.Items.Add("L");
            DariRec.Items.Add("P");
            //DariRec.SelectedIndex = 0;

            TujuanRec.Items.Clear();
            /*cmd = new SqlCommand(@"select distinct ISNULL(TempatAsal,'-') as tempat
                    from StockRec
                    union
                    select distinct ISNULL(TempatTujuan, '-')
                    from StockRec", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                TujuanRec.Items.Add(reader.GetValue(0).ToString());
            }
            koneksi.closeConnection();*/
            TujuanRec.Items.Add("-");
            TujuanRec.Items.Add("WH");
            TujuanRec.Items.Add("HK");
            TujuanRec.Items.Add("R");
            TujuanRec.Items.Add("L");
            TujuanRec.Items.Add("P");
            //TujuanRec.SelectedIndex = 0;

            panelStockExpen.BringToFront();
        }

        private void btn_tambahkanStockE_Click(object sender, EventArgs e)
        {
            try
            {
                cmd = new SqlCommand("INSERT INTO StockExpen(ItemName, Total, Time) VALUES(@a, @b, @c)", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@a", cb_tambahStockE.Text);
                cmd.Parameters.AddWithValue("@b", jumlahTambahStockE.Text);
                cmd.Parameters.AddWithValue("@c", DateTime.Now);

                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                DataSet stockEx = new DataSet();
                da = new SqlDataAdapter("SELECT ItemName as Name, SUM(total) as Stock FROM StockExpen GROUP BY ItemName", koneksi.KoneksiDB());
                da.Fill(stockEx, "stockEx");

                dataGrid_StockExpen.DataSource = stockEx;
                dataGrid_StockExpen.DataMember = "stockEx";
                koneksi.closeConnection();

                cb_tambahStockE.Items.Clear();
                cmd = new SqlCommand("select distinct ItemName from StockExpen", koneksi.KoneksiDB());
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cb_tambahStockE.Items.Add(reader.GetValue(0).ToString());
                }
                koneksi.closeConnection();
            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }
        }

        private void panelStockExpen_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn_tambahkanStockRec_Click(object sender, EventArgs e)
        {
            if (cb_tambahStockR.Text.Length != 0)
            {
            try
            {
                cmd = new SqlCommand("INSERT INTO StockRec(ItemName, TempatAsal, JumlahAsal, TempatTujuan, JumlahTujuan, Time) VALUES(@a, @b, @c, @d, @e, @f)", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@a", cb_tambahStockR.Text);
                if(DariRec.Text.Equals("-")){
                    cmd.Parameters.AddWithValue("@b", DBNull.Value);
                    cmd.Parameters.AddWithValue("@c", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@b", DariRec.Text);
                    cmd.Parameters.AddWithValue("@c", Int32.Parse(jumlahRec.Text) * -1);
                }

                if (TujuanRec.Text.Equals("-"))
                {
                    cmd.Parameters.AddWithValue("@d", DBNull.Value);
                    cmd.Parameters.AddWithValue("@e", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@d", TujuanRec.Text);
                    cmd.Parameters.AddWithValue("@e", Int32.Parse(jumlahRec.Text));
                }
                cmd.Parameters.AddWithValue("@f", DateTime.Now);

                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                cb_tambahStockR.Items.Clear();
                cb_tambahStockR.Text = "";
                jumlahRec.Text = "0";
                DataSet stockRec = new DataSet();
                da = new SqlDataAdapter(@"select ItemName as Name, ISNULL([WH],0) WH, ISNULL([HK],0) HK, 
	                    ISNULL([R],0) R, ISNULL([L],0) L, ISNULL([P],0) P
                        from
                        (
		                    select ItemName, tempatAsal as tempat, sum(JumlahAsal) as total
		                    from stockrec
		                    where tempatAsal is not null
		                    group by ItemName, tempatAsal
		                    union
		                    select ItemName, tempatTujuan, sum(JumlahTujuan) as total
		                    from stockrec
		                    where tempatTujuan is not null
		                    group by ItemName, tempatTujuan
                        ) b
                        pivot
                        (
	                        sum(b.total)
	                        for b.tempat in ([WH],[HK],[R],[L],[P])
                        ) c ", koneksi.KoneksiDB());
                da.Fill(stockRec, "stockRec");
                dataGrid_StockRec.DataSource = stockRec;
                dataGrid_StockRec.DataMember = "stockRec";
                koneksi.closeConnection();

                cmd = new SqlCommand("select distinct ItemName from StockRec", koneksi.KoneksiDB());
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cb_tambahStockR.Items.Add(reader.GetValue(0).ToString());
                }
                koneksi.closeConnection();

                /*
                DariRec.Items.Clear();
                cmd = new SqlCommand(@"select distinct ISNULL(TempatAsal, '-') as tempat
                        from StockRec
                        union
                        select distinct ISNULL(TempatTujuan, '-')
                        from StockRec", koneksi.KoneksiDB());
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DariRec.Items.Add(reader.GetValue(0).ToString());
                }
                koneksi.closeConnection();
                
                TujuanRec.Items.Clear();
                cmd = new SqlCommand(@"select distinct ISNULL(TempatAsal,'-') as tempat
                        from StockRec
                        union
                        select distinct ISNULL(TempatTujuan, '-')
                        from StockRec", koneksi.KoneksiDB());
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TujuanRec.Items.Add(reader.GetValue(0).ToString());
                }
                koneksi.closeConnection();*/

            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }
            }
            else
            MessageBox.Show("Mohon tentukan nama inventori.");
        }

        private void btn_inventorykamar_Click(object sender, EventArgs e)
        {
            ComboboxItem item;
            tb_jumlahBarang.Text = "0";

            cb_tipeKamar.Items.Clear();
            cmd = new SqlCommand(@"select kamar_tipe_id, kamar_tipe from kamar_tipe", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new ComboboxItem();
                item.Value = reader.GetValue(0).ToString();
                item.Text = reader.GetValue(1).ToString();
                cb_tipeKamar.Items.Add(item);
            }
            koneksi.closeConnection();

            cb_kapasitasKamar.Items.Clear();
            cmd = new SqlCommand(@"select kamar_kapasitas_id, kamar_kapasitas from kamar_kapasitas", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new ComboboxItem();
                item.Value = reader.GetValue(0).ToString();
                item.Text = reader.GetValue(1).ToString();
                cb_kapasitasKamar.Items.Add(item);
            }
            koneksi.closeConnection();

            cb_namaBarang.Items.Clear();
            cmd = new SqlCommand(@"select distinct ItemName, 'Expen' as 'Tipe'
                from StockExpen
                union
                select distinct ItemName, 'Rec'
                from StockRec", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new ComboboxItem();
                item.Value = reader.GetValue(1).ToString();
                item.Text = reader.GetValue(0).ToString();
                cb_namaBarang.Items.Add(item);
            }
            koneksi.closeConnection();

            DataSet invenKamar = new DataSet();
            da = new SqlDataAdapter(@"select kt.kamar_tipe as 'Kamar Tipe', kk.kamar_kapasitas as 'Kamar Kapasitas', ik.ItemName as 'Barang', ik.Jumlah, ik.Tipe
                from inventorykamar ik inner join Kamar_Tipe kt on ik.kamar_tipe_id=kt.kamar_tipe_id
	                inner join Kamar_Kapasitas kk on ik.kamar_kapasitas_id=kk.kamar_kapasitas_id", koneksi.KoneksiDB());
            da.Fill(invenKamar, "invenKamar");
            dataGrid_inventoryKamar.DataSource = invenKamar;
            dataGrid_inventoryKamar.DataMember = "invenKamar";
            koneksi.closeConnection();

            panelInvetoryKamar.BringToFront();
        }

        private void btn_simpanInventoryKamar_Click(object sender, EventArgs e)
        {
            try
            {
                cmd = new SqlCommand("insert into InventoryKamar(kamar_tipe_id,kamar_kapasitas_id,ItemName,Jumlah,Tipe) values(@a,@b,@c,@d,@e)", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@a", ((ComboboxItem)cb_tipeKamar.SelectedItem).Value);
                cmd.Parameters.AddWithValue("@b", ((ComboboxItem)cb_kapasitasKamar.SelectedItem).Value);
                cmd.Parameters.AddWithValue("@c", ((ComboboxItem)cb_namaBarang.SelectedItem).Text);
                cmd.Parameters.AddWithValue("@d", tb_jumlahBarang.Text);
                cmd.Parameters.AddWithValue("@e", ((ComboboxItem)cb_namaBarang.SelectedItem).Value);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Inventory kamar telah ditambahkan!");
                refresh_dataGrid_inventoryKamar(Int32.Parse(((ComboboxItem)cb_tipeKamar.SelectedItem).Value.ToString()), Int32.Parse(((ComboboxItem)cb_kapasitasKamar.SelectedItem).Value.ToString()));
            }catch
            {
                try
                {
                    cmd = new SqlCommand("update InventoryKamar set Jumlah=@d where kamar_tipe_id=@a and kamar_kapasitas_id=@b and ItemName=@c", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@a", ((ComboboxItem)cb_tipeKamar.SelectedItem).Value);
                    cmd.Parameters.AddWithValue("@b", ((ComboboxItem)cb_kapasitasKamar.SelectedItem).Value);
                    cmd.Parameters.AddWithValue("@c", ((ComboboxItem)cb_namaBarang.SelectedItem).Text);
                    cmd.Parameters.AddWithValue("@d", tb_jumlahBarang.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Jumlah inventory telah diubah!");
                    refresh_dataGrid_inventoryKamar(Int32.Parse(((ComboboxItem)cb_tipeKamar.SelectedItem).Value.ToString()), Int32.Parse(((ComboboxItem)cb_kapasitasKamar.SelectedItem).Value.ToString()));
                }catch
                {
                    MessageBox.Show("Data tidak valid!");
                }
            }
            koneksi.closeConnection();

        }

        private void dataGrid_inventoryKamar_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGrid_inventoryKamar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                if (e.RowIndex >= 0)
                {
                    cb_namaBarang.Text = dataGrid_inventoryKamar[2, e.RowIndex].Value.ToString();
                    tb_jumlahBarang.Text = dataGrid_inventoryKamar[3, e.RowIndex].Value.ToString();

                    cb_tipeKamar.Text = dataGrid_inventoryKamar[0, e.RowIndex].Value.ToString();
                    cb_kapasitasKamar.Text = dataGrid_inventoryKamar[1, e.RowIndex].Value.ToString();
                }
            }
            catch { }
        }

        void refresh_dataGrid_inventoryKamar(int tipe, int kapasitas)
        {
            DataSet invenKamar = new DataSet();
            da = new SqlDataAdapter(@"select kt.kamar_tipe as 'Kamar Tipe', kk.kamar_kapasitas as 'Kamar Kapasitas', ik.ItemName as 'Barang', ik.Jumlah, ik.Tipe
                from inventorykamar ik inner join Kamar_Tipe kt on ik.kamar_tipe_id=kt.kamar_tipe_id
	                inner join Kamar_Kapasitas kk on ik.kamar_kapasitas_id=kk.kamar_kapasitas_id
                    where ik.kamar_tipe_id="+ tipe +" and ik.kamar_kapasitas_id="+ kapasitas +"", koneksi.KoneksiDB());
            da.Fill(invenKamar, "invenKamar");
            dataGrid_inventoryKamar.DataSource = invenKamar;
            dataGrid_inventoryKamar.DataMember = "invenKamar";
            koneksi.closeConnection();
        }

        private void cb_tipeKamar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cb_tipeKamar.SelectedIndex>=0 && cb_kapasitasKamar.SelectedIndex>=0){
                refresh_dataGrid_inventoryKamar(Int32.Parse(((ComboboxItem)cb_tipeKamar.SelectedItem).Value.ToString()), Int32.Parse(((ComboboxItem)cb_kapasitasKamar.SelectedItem).Value.ToString()));
            }
        }

        private void cb_kapasitasKamar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_tipeKamar.SelectedIndex >= 0 && cb_kapasitasKamar.SelectedIndex >= 0)
            {
                refresh_dataGrid_inventoryKamar(Int32.Parse(((ComboboxItem)cb_tipeKamar.SelectedItem).Value.ToString()), Int32.Parse(((ComboboxItem)cb_kapasitasKamar.SelectedItem).Value.ToString()));
            }
        }

        private void btnCleanSubmit_Click(object sender, EventArgs e)
        {
            SqlCommand list = new SqlCommand("select staff_id from Staff where nama = @a", koneksi.KoneksiDB());
            list.Parameters.AddWithValue("@a", cbNamaPegawaiCLeaning.Text);
            string idStaff = list.ExecuteScalar().ToString();
            koneksi.closeConnection();

            list = new SqlCommand("insert into historyClean(tanggalClean,idPegawai,noteClean,kamarNo) values (@a,@b,@c,@d)", koneksi.KoneksiDB());
            list.Parameters.AddWithValue("@a", DateTime.Now);
            list.Parameters.AddWithValue("@b", idStaff);
            list.Parameters.AddWithValue("@c", txtNoteCleaning.Text);
            list.Parameters.AddWithValue("@d", lblNoKamarClean.Text);
            list.ExecuteNonQuery();
            koneksi.closeConnection();

            panelCleaningBy.SendToBack();
        }

        private void label137_Click(object sender, EventArgs e)
        {

        }

        private void btnLaporanWNA_Click(object sender, EventArgs e)
        {
            resetBtnKonfigurasi();
            refreshActivatedButton();
            btnLaporanWNA.FlatAppearance.BorderColor = Color.CornflowerBlue;
            btnLaporanWNA.FlatAppearance.BorderSize = 2;

            flowLayoutPanel1.Visible = false;

            HideBtnStatusKamar();
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();
            //panelLaporanKeuangan.BringToFront();
            /*List<Microsoft.Reporting.WinForms.ReportParameter> list = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            Microsoft.Reporting.WinForms.ReportParameter param = new Microsoft.Reporting.WinForms.ReportParameter("tahun", "2014");
            list.Add(param);
            Microsoft.Reporting.WinForms.ReportParameter param2 = new Microsoft.Reporting.WinForms.ReportParameter("bulan", "8");
            list.Add(param2);
            reportInvoice.ServerReport.ReportPath = "/Invoice/Laporan_Pendapatan_Harian";
            reportInvoice.ServerReport.SetParameters(list);
            //reportInvoice.ServerReport.Refresh();
            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
            List<Microsoft.Reporting.WinForms.ReportParameter> parameter_reset = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            reportInvoice.ServerReport.SetParameters(parameter_reset);*/
            
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.tamuTableAdapter1.Fill(this.laporanWNA.Tamu);

            reportWNA.Reset();
            reportWNA.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.laporanWNA.rdlc";
            reportWNA.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", (object)laporanWNA.Tamu));
            reportWNA.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", (object)infoHotel.IDHotel));

            reportWNA.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            reportWNA.LocalReport.SetParameters(parameter);

            reportWNA.RefreshReport();
            reportWNA.BringToFront();
         
        }

        private void refreshKontrolkeuangan()
        {
            ComboboxItem tahunDipilih = (ComboboxItem)comboBoxGrandTotalTahun.SelectedItem;
            ComboboxItem bulanDipilih = (ComboboxItem)comboBoxGrandTotalBulan.SelectedItem;

            if (cekPilihLaporanGrandTotal)
            {
                this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
                this.dataTable1TableAdapter1.Fill(this.infoPendapatan.DataTable1, Convert.ToInt16(tahunDipilih.Value), Convert.ToInt16(bulanDipilih.Value));
                this.BookingTableAdapter.Fill(this.infoTagihanLunasDataSet.Booking, Convert.ToInt16(tahunDipilih.Value), Convert.ToInt16(bulanDipilih.Value));
                this.infoKontrolKeuanganTableAdapter.Fill(this.infoKontrolKeuanganDataSet2.infoKontrolKeuangan, Convert.ToInt16(tahunDipilih.Value), Convert.ToInt16(bulanDipilih.Value));

                reportTagihanLunas.Reset();

                reportTagihanLunas.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.Laporan_Kontrol_Keuangan.rdlc";
                reportTagihanLunas.LocalReport.DataSources.Add(new ReportDataSource("infoPendapatan", (object)infoPendapatan.DataTable1));
                reportTagihanLunas.LocalReport.DataSources.Add(new ReportDataSource("infoHotel", (object)infoHotel.IDHotel));
                reportTagihanLunas.LocalReport.DataSources.Add(new ReportDataSource("infoTagihanLunas", (object)infoTagihanLunasDataSet.Booking));
                reportTagihanLunas.LocalReport.DataSources.Add(new ReportDataSource("infoKontrolKeuangan", (object)infoKontrolKeuanganDataSet2.infoKontrolKeuangan));


                reportTagihanLunas.LocalReport.EnableExternalImages = true;
                string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
                ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
                reportTagihanLunas.LocalReport.SetParameters(parameter);

                reportTagihanLunas.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
                reportTagihanLunas.RefreshReport();
                reportTagihanLunas.BringToFront();

            }
        }

        private void comboBoxGrandTotalTahun_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshKontrolkeuangan();
        }

        private void btnLaporanTandaTangan_Click(object sender, EventArgs e)
        {
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.DataTableTandaTanganTableAdapter.Fill(this.InfoTandaTangan.DataTableTandaTangan);

            reportTandaTangan.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar";
            ReportParameter parameter = new ReportParameter("Url", imagePath);
            reportTandaTangan.LocalReport.SetParameters(parameter);

            imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter2 = new ReportParameter("ImageTest", imagePath);
            reportTandaTangan.LocalReport.SetParameters(parameter2);

            reportTandaTangan.LocalReport.SetParameters(parameter);

            reportTandaTangan.RefreshReport();
            reportTandaTangan.BringToFront();
        }

        private void cb_bahasa_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btn_Scrollitemnaik_Click(object sender, EventArgs e)
        {
            GridViewItem.FirstDisplayedScrollingRowIndex = GridViewItem.FirstDisplayedScrollingRowIndex - 1;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            GridViewItem.FirstDisplayedScrollingRowIndex = GridViewItem.FirstDisplayedScrollingRowIndex + 1;
        }

        private void btn_Tambah_Item_Click(object sender, EventArgs e)
        {
            inputJumlahItem.Text = Convert.ToString(Convert.ToInt16(inputJumlahItem.Text) + 1);
        }

        private void btn_Kurang_Item_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt16(inputJumlahItem.Text) > 1)
            {
                inputJumlahItem.Text = Convert.ToString(Convert.ToInt16(inputJumlahItem.Text) - 1);

            }
        }

        private void btn_tambahDiskon_Click(object sender, EventArgs e)
        {
            panelCariKamarUpdateReservasi.Visible = false;
            ////Interaction.InputBox("Masukkan jumlah diskon (dalam bentuk nominal) !") = 0;
            //string Str = Interaction.InputBox("Masukkan jumlah diskon (dalam bentuk nominal) !", "", "0");//textBox1.Text.Trim();

            //int jumlahDiskon;
            ////int jumlahDiskon = Int32.Parse(Interaction.InputBox("Masukkan jumlah diskon (dalam bentuk nominal) !"));

            //bool isNum = int.TryParse(Str, out jumlahDiskon);

            //if (isNum && jumlahDiskon > 0 && Str.Trim().Length > 3)
            //{
                try
                {
                    int jumlahDiskon = Int32.Parse(Interaction.InputBox("Masukkan jumlah diskon (dalam bentuk nominal) !"));
                    cmd = new SqlCommand("INSERT INTO Pemesanan(item_id,reservasi_id,tgl_pemesanan,harga) VALUES (@a,@b,@c,@d)", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@a", "12");
                    cmd.Parameters.AddWithValue("@b", update_reservasiId.Text);
                    cmd.Parameters.AddWithValue("@c", DateTime.Now);
                    cmd.Parameters.AddWithValue("@d", jumlahDiskon * -1);
                    cmd.ExecuteNonQuery();
                    koneksi.closeConnection();

                    cmd = new SqlCommand("update Reservasi set tag_restoran=tag_restoran+@biaya where reservasi_id=@idr", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@biaya", jumlahDiskon * -1);
                    cmd.Parameters.AddWithValue("@idr", update_reservasiId.Text);
                    cmd.ExecuteNonQuery();
                    koneksi.closeConnection();

                }
                catch
                {
                    MessageBox.Show("Inputan tidak valid!");
                }
            //}

            panelUpdateReservasi.Visible = false;
            //update_reservasiId.Text = "-";
            //flowLayoutPanel2.Enabled = true;
            panelUpdateBooking_Click(sender, e);
            keluarToolStripMenuItem.Enabled = true;
            btn_tambahReservasi.Enabled = true;
        }
        
        private void tambahkanDiskonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                cmd = new SqlCommand("select reservasi_id from reservasi where kamar_no=@a and status='checkin'", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@a", dataKamarCh);
                int reservasi_id = Int32.Parse(cmd.ExecuteScalar().ToString());

                koneksi.closeConnection();

                int jumlahDiskon = Int32.Parse(Interaction.InputBox("Masukkan jumlah diskon (dalam bentuk nominal) !"));
                cmd = new SqlCommand("INSERT INTO Pemesanan(item_id,reservasi_id,tgl_pemesanan,harga) VALUES (@a,@b,@c,@d)", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@a", "12");
                cmd.Parameters.AddWithValue("@b", reservasi_id);
                cmd.Parameters.AddWithValue("@c", DateTime.Now);
                cmd.Parameters.AddWithValue("@d", jumlahDiskon * -1);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                cmd = new SqlCommand("update Reservasi set tag_restoran=tag_restoran+@biaya where reservasi_id=@idr", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@biaya", jumlahDiskon * -1);
                cmd.Parameters.AddWithValue("@idr", reservasi_id);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }
        }

        private void printInvoiceRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

            int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1
            SqlCommand sqlq = new SqlCommand("select max(Reservasi.booking_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
            sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
            string bookingKamar = sqlq.ExecuteScalar().ToString();
            koneksi.closeConnection();

            String booking_id = bookingKamar;
            this.infoBooking.EnforceConstraints = false;
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.newSelectLastPaymentTableAdapter.Fill(this.lastPayment.NewSelectLastPayment, Int32.Parse(booking_id), NoKamarInfo);

            this.TamuTableAdapter.Fill(this.infoBooking.Tamu, Int32.Parse(booking_id));
            this.NewSelectCommandTableAdapter.Fill(this.infoReservasi.NewSelectCommand, Int32.Parse(booking_id), NoKamarInfo);
            this.newSelectCommandTableAdapter1.Fill(this.pemesanan.NewSelectCommand, Int32.Parse(booking_id), NoKamarInfo);
            this.NewSelectPembayaranTableAdapter.Fill(this.infoPembayaran.NewSelectPembayaran, Int32.Parse(booking_id), NoKamarInfo);
            reportInvoice.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
            reportInvoice.LocalReport.SetParameters(parameter);
            string imagePath2 = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(booking_id) + ".png";
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(booking_id) + ".png"))
            {
                imagePath2 = "NULL";
            }
            ReportParameter parameter2 = new ReportParameter("ttd", imagePath2);
            reportInvoice.LocalReport.SetParameters(parameter2);

            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
        }

        private void printInvoiceGroupBookingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

            int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1
            SqlCommand sqlq = new SqlCommand("select max(Reservasi.booking_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok", koneksi.KoneksiDB());
            sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
            sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
            string bookingKamar = sqlq.ExecuteScalar().ToString();
            koneksi.closeConnection();

            String booking_id = bookingKamar;
            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.newSelectLastPaymentTableAdapter.Fill(this.lastPayment.NewSelectLastPayment, Int32.Parse(booking_id), null);

            this.infoBooking.EnforceConstraints = false;
            this.TamuTableAdapter.Fill(this.infoBooking.Tamu, Int32.Parse(booking_id));
            this.NewSelectCommandTableAdapter.Fill(this.infoReservasi.NewSelectCommand, Int32.Parse(booking_id), null);
            this.newSelectCommandTableAdapter1.Fill(this.pemesanan.NewSelectCommand, Int32.Parse(booking_id), null);
            this.NewSelectPembayaranTableAdapter.Fill(this.infoPembayaran.NewSelectPembayaran, Int32.Parse(booking_id), null);
            reportInvoice.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
            reportInvoice.LocalReport.SetParameters(parameter);
            string imagePath2 = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(booking_id) + ".png";

            if (!File.Exists(Directory.GetCurrentDirectory() + "\\gambar\\" + Convert.ToString(booking_id) + ".png"))
            {
                imagePath2 = "NULL";
            }

            ReportParameter parameter2 = new ReportParameter("ttd", imagePath2);
            reportInvoice.LocalReport.SetParameters(parameter2);

            reportInvoice.RefreshReport();
            reportInvoice.BringToFront();
        }

        private void tambahkanTagLainnyaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime tanggalPesan1 = Convert.ToDateTime(Tgltahun.ToString() + "-" + TglBulan.ToString() + "-" + dataGridView3.Columns[columnSelect].Name.ToString());

                int NoKamarInfo = Int32.Parse(dataGridView3.Rows[rowSelect].Cells[0].Value.ToString());//tes1
                SqlCommand sqlq = new SqlCommand("select max(Reservasi.reservasi_id) from Reservasi, Tamu where Tamu.tamu_id = Reservasi.tamu_id and convert(date,Reservasi.checkin) <=@id and convert(date,Reservasi.checkout) > @id and Reservasi.kamar_no=@nok", koneksi.KoneksiDB());
                sqlq.Parameters.AddWithValue("@id", tanggalPesan1);
                sqlq.Parameters.AddWithValue("@nok", NoKamarInfo);
                int reservasi_id = Int32.Parse(sqlq.ExecuteScalar().ToString());
                koneksi.closeConnection();

                int jumlahDiskon = Int32.Parse(Interaction.InputBox("Masukkan jumlah tagihan lainnya !"));
                cmd = new SqlCommand("INSERT INTO Pemesanan(item_id,reservasi_id,tgl_pemesanan,harga) VALUES (@a,@b,@c,@d)", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@a", "12");
                cmd.Parameters.AddWithValue("@b", reservasi_id);
                cmd.Parameters.AddWithValue("@c", DateTime.Now);
                cmd.Parameters.AddWithValue("@d", jumlahDiskon);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                cmd = new SqlCommand("update Reservasi set tag_restoran=tag_restoran+@biaya where reservasi_id=@idr", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@biaya", jumlahDiskon);
                cmd.Parameters.AddWithValue("@idr", reservasi_id);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

            }
            catch
            {
                MessageBox.Show("Inputan tidak valid!");
            }
        }

        private void btnLaporanTopCorp_Click(object sender, EventArgs e)
        {
            resetBtnKonfigurasi();
            refreshActivatedButton();
            btnLaporanTopCorp.FlatAppearance.BorderColor = Color.CornflowerBlue;
            btnLaporanTopCorp.FlatAppearance.BorderSize = 2;

            flowLayoutPanel1.Visible = false;

            HideBtnStatusKamar();
            btnPeriodik.Visible = false;
            btn_harga_khusus.Visible = false;
            hideBookingElement();
            panelKamarDibooking.Controls.Clear();

            this.IDHotelTableAdapter.Fill(this.infoHotel.IDHotel);
            this.TopCorpTableAdapter.Fill(this.DSTopCorp.TopCorp);

            reportTopCorp.Reset();
            reportTopCorp.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.LaporanTopCorp.rdlc";
            reportTopCorp.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", (object)infoHotel.IDHotel));
            reportTopCorp.LocalReport.DataSources.Add(new ReportDataSource("DSTopCorp", (object)DSTopCorp.TopCorp));
            
            reportTopCorp.LocalReport.EnableExternalImages = true;
            string imagePath = "file://" + Directory.GetCurrentDirectory() + "\\gambar\\LogoC.png";
            ReportParameter parameter = new ReportParameter("ImageTest", imagePath);
            reportTopCorp.LocalReport.SetParameters(parameter);

            reportTopCorp.RefreshReport();
            reportTopCorp.BringToFront();
            
        }

        private void comboBoxGrandTotalBulan_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshKontrolkeuangan();
        }

        private void btnUlangtahun_Click(object sender, EventArgs e)
        {
            //if (inputSearchPerusahaanDT.Text.Length >= 3)
            //{

            //try
            //{
            //    MailMessage message = new MailMessage();
            //    SmtpClient smtp = new SmtpClient();

            //    message.From = new MailAddress("kunto.wb@gmail.com");
            //    message.To.Add(new MailAddress("kuntowb@indosuryaasia.com"));
            //    message.Subject = "Test";
            //    message.Body = "Content";

            //    smtp.Port = 587;
            //    smtp.Host = "smtp.gmail.com";
            //    smtp.EnableSsl = true;
            //    smtp.UseDefaultCredentials = true;
            //    smtp.Credentials = new NetworkCredential("kunto.wb@gmail.com", "!Predator99");
            //    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    smtp.Send(message);
            //}

            //catch (Exception ex)
            //{
            //    MessageBox.Show("err: " + ex.Message);
            //}


            /////this one works/////
            //try
            //{
            //    MailMessage mail = new MailMessage();
            //    SmtpClient SmtpServer = new SmtpClient("mail.indosuryaasia.com");
            //    mail.From = new MailAddress("kuntow@indosuryaasia.com");
            //    mail.To.Add("kunto.wb@gmail.com");
            //    mail.Subject = "Your Subject";
            //    mail.Body = "Your Textbox Here!";
            //    SmtpServer.Port = 26;
            //    SmtpServer.Credentials = new NetworkCredential("kuntow@indosuryaasia.com", "!IndoSurya99");
            //    SmtpServer.Send(mail);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Seems some problem!");
            //}
            /////this one works/////
            

            //SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            //client.EnableSsl = true;
            //MailAddress from = new MailAddress("kunto.wb@gmail.com", "[ Your full name here]");
            //MailAddress to = new MailAddress("kuntow@indosuryaasia.com", "Your recepient name");
            //MailMessage message = new MailMessage(from, to);
            //message.Body = "This is a test e-mail message sent using gmail as a relay server ";
            //message.Subject = "Gmail test email with SSL and Credentials";
            //NetworkCredential myCreds = new NetworkCredential("kunto.wb@gmail.com", "!Predator99", "");
            //client.Credentials = myCreds;
            //try
            //{
            //    client.Send(message);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Exception is:" + ex.ToString());
            //}
            //Console.WriteLine("Goodbye.");

            inputSearchDT.Text = "";
            inputSearchPerusahaanDT.Text = "";
            BindingSource bs = new BindingSource();
            bs.DataSource = GridViewDaftarTamu.DataSource;
            //bs.Filter = "tanggallahir = '" + String.Format("{0:M/d/yyyy}", DateTime.Now.Date) +"' order by kota";
            //bs.Filter = "perusahaan like '%" + inputSearchPerusahaanDT.Text + "%'";
            bs.Filter = "bulan_lahir = " + DateTime.Now.Month.ToString();
            bs.Filter += " and hari_lahir >= " + DateTime.Now.Day.ToString();
            bs.Sort = "hari_lahir";
            GridViewDaftarTamu.DataSource = bs;
            //}

            //MessageBox.Show("Hari ini terdapat " + jumUlangTahun.ToString() + " tamu berulang tahun.");

            //DialogResult dialogResult = MessageBox.Show("Hari ini terdapat " + jumUlangTahun.ToString() + " tamu berulang tahun.", "Peringatan Ulang Tahun Tamu", MessageBoxButtons.OK);
            //if (dialogResult == DialogResult.OK)
            //{
            //    btnUlangtahun_Click(sender, e);                
            //}
            //else if (dialogResult == DialogResult.No)
            //{
            //    MessageBox.Show("Besok terdapat " + jumUlangTahun.ToString() + " tamu berulang tahun.");

            //}

        }

        private void label39_Click(object sender, EventArgs e)
        {

        }

        private void btnEmailVoucher_Click(object sender, EventArgs e)
        {
            ///this one works/////
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.indosuryaasia.com");
                mail.From = new MailAddress("kuntow@indosuryaasia.com");
                mail.To.Add("kunto.wb@gmail.com");
                mail.Subject = "Judul email";
                mail.Body = "Test email message";
                SmtpServer.Port = 26;
                SmtpServer.Credentials = new NetworkCredential("kuntow@indosuryaasia.com", "!IndoSurya99");
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Seems some problem!");
            }
            ///this one works/////

        }

        // Enable/disable connection related controls
        private void EnableConnectionControls(bool enable)
        {
            devicesCombo.Enabled = enable;
            videoResolutionsCombo.Enabled = enable;
            snapshotResolutionsCombo.Enabled = enable;
            connectButton.Enabled = enable;
            disconnectButton.Enabled = !enable;
            triggerButton.Enabled = (!enable) && (snapshotCapabilities.Length != 0);
        }

        private void picIdentitas_Click(object sender, EventArgs e)
        {
//
            devicesCombo.SelectedIndex = 0;// devicesCombo.Items.Count - 1;
            
            //int webcamx=0;

            //foreach (FilterInfo device in videoDevices)
            //{
            //    devicesCombo.Items.Add(device.Name);
            //}

            int webcamx = 0;

            while (devicesCombo.SelectedItem.ToString() != "Webcam C170")
            {
                webcamx++;
                devicesCombo.SelectedIndex = webcamx;// devicesCombo.Items.Count - 1;
                //MessageBox.Show(webcamx.ToString());
            }
            //MessageBox.Show(devicesCombo.SelectedItem.ToString());
            videoResolutionsCombo.SelectedIndex = 0;// videoResolutionsCombo.Items.Count - 1;
            //snapshotResolutionsCombo.SelectedIndex = 2;
            connectButton_Click(sender, e);

            //if (videoDevice != null)
            //{
            //    if ((videoCapabilities != null) && (videoCapabilities.Length != 0))
            //    {
            //        videoDevice.VideoResolution = videoCapabilities[videoResolutionsCombo.SelectedIndex];
            //    }

            //    if ((snapshotCapabilities != null) && (snapshotCapabilities.Length != 0))
            //    {
            //        videoDevice.ProvideSnapshots = true;
            //        videoDevice.SnapshotResolution = snapshotCapabilities[snapshotResolutionsCombo.SelectedIndex];
            //        videoDevice.SnapshotFrame += new NewFrameEventHandler(videoDevice_SnapshotFrame);
            //    }

            //    EnableConnectionControls(false);
            //    videoSourcePlayer.Visible = true;
            //    videoSourcePlayer.BringToFront();
            //    videoSourcePlayer.VideoSource = videoDevice;
            //    videoSourcePlayer.Start();
            //}


            //if (videoDevice != null)
            //{
            //    if ((videoCapabilities != null) && (videoCapabilities.Length != 0))
            //    {
            //        videoDevice.VideoResolution = videoCapabilities[videoResolutionsCombo.SelectedIndex];
            //    }

            //    if ((snapshotCapabilities != null) && (snapshotCapabilities.Length != 0))
            //    {
            //        videoDevice.ProvideSnapshots = true;
            //        videoDevice.SnapshotResolution = snapshotCapabilities[snapshotResolutionsCombo.SelectedIndex];
            //        videoDevice.SnapshotFrame += new NewFrameEventHandler(videoDevice_SnapshotFrame);
            //    }

            //    EnableConnectionControls(false);

            //    videoSourcePlayer.VideoSource = videoDevice;
            //    videoSourcePlayer.Start();
            //}

        }


        // Collect supported video and snapshot sizes
        private void EnumeratedSupportedFrameSizes(VideoCaptureDevice videoDevice)
        {
            this.Cursor = Cursors.WaitCursor;

            videoResolutionsCombo.Items.Clear();
            snapshotResolutionsCombo.Items.Clear();

            try
            {
                videoCapabilities = videoDevice.VideoCapabilities;
                snapshotCapabilities = videoDevice.SnapshotCapabilities;

                foreach (VideoCapabilities capabilty in videoCapabilities)
                {
                    videoResolutionsCombo.Items.Add(string.Format("{0} x {1}",
                        capabilty.FrameSize.Width, capabilty.FrameSize.Height));
                }

                foreach (VideoCapabilities capabilty in snapshotCapabilities)
                {
                    snapshotResolutionsCombo.Items.Add(string.Format("{0} x {1}",
                        capabilty.FrameSize.Width, capabilty.FrameSize.Height));
                }

                if (videoCapabilities.Length == 0)
                {
                    videoResolutionsCombo.Items.Add("Not supported");
                }
                if (snapshotCapabilities.Length == 0)
                {
                    snapshotResolutionsCombo.Items.Add("Not supported");
                }

                videoResolutionsCombo.SelectedIndex = 0;
                snapshotResolutionsCombo.SelectedIndex = 0;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        // New snapshot frame is available
        private void videoDevice_SnapshotFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Console.WriteLine(eventArgs.Frame.Size);
            //pictureBox1.Image.Save()
            //pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            //ShowSnapshot((Bitmap)eventArgs.Frame.Clone());
            //Disconnect();
            //ShowSnapshot((Bitmap)eventArgs.Frame.Clone());
            //ShowSnapshot((Bitmap)eventArgs.Frame.Clone());
            //ShowSnapshot((Bitmap)eventArgs.Frame.Clone());
            //MessageBox.Show(DateTime.Now.ToUniversalTime().ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_"));
            //pictureBox1.Image.Save(@"D:\Development\Webcam Test\" + DateTime.Now.ToUniversalTime().ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + ".jpeg", ImageFormat.Jpeg);
            //ShowSnapshot((Bitmap)eventArgs.Frame.Clone());
            //pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            //MessageBox.Show(DateTime.Now.ToUniversalTime().ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_"));
            //pictureBox1.Image.Save(@"D:\Development\Webcam Test\" + DateTime.Now.ToUniversalTime().ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + ".jpeg");
            ////ShowSnapshot((Bitmap)eventArgs.Frame.Clone());
            //pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            //for (int i = 0; i < 2; i++)
            //{
            //    pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            //}

        }
        //}

        private void ShowSnapshot(Bitmap snapshot)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Bitmap>(ShowSnapshot), snapshot);
            }
            else
            {
                picIdentitas.Image = snapshot;
                picIdentitas.Image.Save(Directory.GetCurrentDirectory() + "\\gambar\\" + DateTime.Now.ToUniversalTime().ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") +"_" + inputNamaTamu.Text + ".jpeg"
                );
                //picIdentitas.BringToFront();
                
                //timer1.Interval = 3000;
                //pictureBox1.SendToBack();           
                //if (snapshotForm == null)
                //{
                //    snapshotForm = new SnapshotForm();
                //    snapshotForm.FormClosed += new FormClosedEventHandler(snapshotForm_FormClosed);
                //    snapshotForm.Show();
                //}

                //snapshotForm.SetImage(snapshot);
            }
        }

        private void devicesCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (videoDevices.Count != 0)
            {
                videoDevice = new VideoCaptureDevice(videoDevices[devicesCombo.SelectedIndex].MonikerString);
                EnumeratedSupportedFrameSizes(videoDevice);
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (videoDevice != null)
            {
                if ((videoCapabilities != null) && (videoCapabilities.Length != 0))
                {
                    videoDevice.VideoResolution = videoCapabilities[videoResolutionsCombo.SelectedIndex];
                }

                if ((snapshotCapabilities != null) && (snapshotCapabilities.Length != 0))
                {
                    videoDevice.ProvideSnapshots = true;
                    videoDevice.SnapshotResolution = snapshotCapabilities[snapshotResolutionsCombo.SelectedIndex];
                    videoDevice.SnapshotFrame += new NewFrameEventHandler(videoDevice_SnapshotFrame);
                }

                EnableConnectionControls(false);
                videoSourcePlayer.Visible = true;
                videoSourcePlayer.BringToFront();
                
                videoSourcePlayer.VideoSource = videoDevice;
                videoSourcePlayer.Start();
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            Disconnect();
        }


        // Disconnect from video device
        private void Disconnect()
        {
            if (videoSourcePlayer.VideoSource != null)
            {
                // stop video device
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
                videoSourcePlayer.VideoSource = null;

                if (videoDevice.ProvideSnapshots)
                {
                    videoDevice.SnapshotFrame -= new NewFrameEventHandler(videoDevice_SnapshotFrame);
                }

                EnableConnectionControls(true);
            }
        }


        void DrawToBitmap(Control ctl, Bitmap bmp)
        {
            Cursor = Cursors.WaitCursor;         // yes it takes a while
            Panel p = new Panel();               // the containing panel
            Point oldLocation = ctl.Location;    // 
            p.Location = Point.Empty;            //
            this.Controls.Add(p);                //

            int maxWidth = 2000;                 // you may want to try other sizes
            int maxHeight = 2000;                //

            Bitmap bmp2 = new Bitmap(maxWidth, maxHeight);  // the buffer

            p.Height = maxHeight;               // set up the..
            p.Width = maxWidth;                 // ..container

            ctl.Location = new Point(0, 0);     // starting point
            ctl.Parent = p;                     // inside the container
            p.Show();                           // 
            p.BringToFront();                   //

            // we'll draw onto the large bitmap with G
            using (Graphics G = Graphics.FromImage(bmp))
                for (int y = 0; y < ctl.Height; y += maxHeight)
                {
                    ctl.Top = -y;                   // move up
                    for (int x = 0; x < ctl.Width; x += maxWidth)
                    {
                        ctl.Left = -x;             // move left
                        p.DrawToBitmap(bmp2, new Rectangle(0, 0, maxWidth, maxHeight));
                        G.DrawImage(bmp2, x, y);   // patch together
                    }
                }

            ctl.Location = p.Location;         // restore..
            ctl.Parent = this;                 // form layout <<<==== ***
            p.Dispose();                       // clean up

            Cursor = Cursors.Default;          // done
        }
        string filejpeg="";
        private void videoSourcePlayer_Click(object sender, EventArgs e)
        {

            var bm = new Bitmap(videoSourcePlayer.Width, videoSourcePlayer.Height);
            DrawToBitmap(videoSourcePlayer, bm);// .DrawToBitmap(bm, bm.Size);
            filejpeg = Directory.GetCurrentDirectory() + "\\gambar\\"+ DateTime.Now.ToUniversalTime().ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") +"_"+ inputNamaTamu.Text + ".jpeg";
            bm.Save(filejpeg, ImageFormat.Jpeg);
            bm.Dispose();                      // get rid of the big one!
            GC.Collect();
            picIdentitas.Image = Image.FromFile(filejpeg);

//////            if ((videoDevice != null) && (videoDevice.ProvideSnapshots))
//////            {
//////                videoDevice.SimulateTrigger();
//////                videoSourcePlayer.Visible = false;
//////            }

//////            if (triggerButton.Text == "Aktifkan")
//////            {
//////                triggerButton.Text = "Simpan Gambar";
//////                picIdentitas.SendToBack();
//////            }
////////            disconnectButton_Click(sender, e);
            Disconnect();
        }

        private void inputTelepon_TextChanged(object sender, EventArgs e)
        {

        }

        private void panelDataTamu_MouseLeave(object sender, EventArgs e)
        {
            Disconnect();

        }

        private void FormUtama_Resize(object sender, EventArgs e)
        {
            //if (WindowState == FormWindowState.Minimized)
            //{
            //    this.Hide();
            //}
        }

        private void notifyIcon1_Click_1(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void groupBox14_Enter(object sender, EventArgs e)
        {

        }

        private void GridViewAddItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //  if (GridViewAddItem.Columns[e.ColumnIndex].Name == "InvoiceBooking")
            //{
                //if (gridView_historis.SelectedCells.Count > 0)
                //{
            int selectedrowindex = GridViewAddItem.SelectedCells[0].RowIndex;

            DataGridViewRow selectedRow = GridViewAddItem.Rows[selectedrowindex];
            //dt.Columns.Add("IdItem".ToString());
            string a = Convert.ToString(selectedRow.Cells["IdItem"].Value);
            //MessageBox.Show(a);
            btn_removeItem_Click(sender, e);
        }

        //private void btnCariutg_Click(object sender, EventArgs e)
        //{
        //    SqlDataAdapter dataadapter = new SqlDataAdapter("select Booking.booking_id, Booking.tgl_booking, Tamu.tamu , 0 as 'Utang' from Booking, Tamu where Tamu.tamu_id=Booking.tamu_id and tamu=@a and Booking.balance_due>0", koneksi.KoneksiDB());
        //    dataadapter.SelectCommand.Parameters.AddWithValue("@a",txtCariutang.Text);
        //    DataSet dse = new DataSet();
        //    //connection.Open();
        //    dataadapter.Fill(dse, "utang");
        //    dataGridUtang.DataSource = dse;
        //    dataGridUtang.DataMember = "utang";

        //    for (int i = 0; i < dataGridUtang.Rows.Count; i++)
        //    {
        //        string idbookingData = dataGridUtang[0, i].Value.ToString();
        //        bookingdataID = idbookingData;
        //        SqlCommand sql1 = new SqlCommand("select booking_diskon_id from Booking where booking_id=@a", koneksi.KoneksiDB());
        //        sql1.Parameters.AddWithValue("@a", idbookingData);
        //        int kodediskon = Int32.Parse(sql1.ExecuteScalar().ToString());
        //        koneksi.closeConnection();

        //        sql1 = new SqlCommand("select harga*100 from Booking_Diskon where booking_diskon_id=@a", koneksi.KoneksiDB());
        //        sql1.Parameters.AddWithValue("@a", kodediskon);
        //        int potongan = Int32.Parse(sql1.ExecuteScalar().ToString());
        //        koneksi.closeConnection();


        //        SqlCommand queryData = new SqlCommand("select reservasi_id, tag_kamar, tag_restoran, downpayment from Reservasi where booking_id=@a and status='checkout' ", koneksi.KoneksiDB());
        //        queryData.Parameters.AddWithValue("@a", idbookingData);
        //        SqlDataReader readKumpulData = queryData.ExecuteReader();
        //        reservasichid = new int[90];
        //        tagKamarchid = new int[90];
        //        tagrestoranchid = new int[90];
        //        downpaymentchid = new int[90];
        //        indexChid = 0;
        //        while (readKumpulData.Read())
        //        {
        //            reservasichid[indexChid] = Int32.Parse(readKumpulData["reservasi_id"].ToString());
        //            tagKamarchid[indexChid] = (Int32.Parse(readKumpulData["tag_kamar"].ToString()) * potongan) / 100;
        //            tagrestoranchid[indexChid] = Int32.Parse(readKumpulData["tag_restoran"].ToString());
        //            downpaymentchid[indexChid] = Int32.Parse(readKumpulData["downpayment"].ToString());
        //            indexChid += 1;
        //        }
        //        koneksi.closeConnection();
        //        int biayaUtang = 0;
        //        for (int j = 0; j < indexChid; j++)
        //        {
        //            biayaUtang = biayaUtang + ((tagrestoranchid[j] + tagKamarchid[j] - downpaymentchid[j]));
        //        }
        //        dataGridUtang[3, i].Value = biayaUtang.ToString();
        //    }
        //    indexChid = 0;
        //    //connection.Close();
        //    koneksi.closeConnection();
        
        //}

        //private void btnLunasutg_Click(object sender, EventArgs e)
        //{
        //    for (int i = 0; i < indexChid; i++)
        //    {
        //        SqlCommand querybayar = new SqlCommand("insert into pembayaran(booking_id,reservasi_id,payment, nopayment, jumlahpayment, tggalpayment, staff_id) values(@a,@b,@c,@d,@e,@f,@g)", koneksi.KoneksiDB());
        //        querybayar.Parameters.AddWithValue("@a", bookingdataID);
        //        querybayar.Parameters.AddWithValue("@b", reservasichid[i]);
        //        querybayar.Parameters.AddWithValue("@c", "Kontan");
        //        querybayar.Parameters.AddWithValue("@d", "");
        //        querybayar.Parameters.AddWithValue("@e", tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]);
        //        querybayar.Parameters.AddWithValue("@f", DateTime.Now);
        //        querybayar.Parameters.AddWithValue("@g", Login.idS.ToString());
        //        querybayar.ExecuteNonQuery();
        //        koneksi.closeConnection();

        //        querybayar = new SqlCommand("update Reservasi set downpayment = downpayment + @a where reservasi_id = @b", koneksi.KoneksiDB());
        //        querybayar.Parameters.AddWithValue("@a", tagrestoranchid[i] + tagKamarchid[i] - downpaymentchid[i]);
        //        querybayar.Parameters.AddWithValue("@b", reservasichid[i]);
        //        querybayar.ExecuteNonQuery();
        //        koneksi.closeConnection();
                
        //    }
        //    if (indexChid > 0)
        //    {
        //        SqlCommand queryData = new SqlCommand("update Booking set balance_due=0 where booking_id =@a", koneksi.KoneksiDB());
        //        queryData.Parameters.AddWithValue("@a", bookingdataID);
        //        queryData.ExecuteNonQuery();
        //        koneksi.closeConnection();
        //    }
        //    indexChid = 0;
            
        //    btnUtng_Click(sender, e);    
        
        //}

        //private void txtCariutang_TextChanged(object sender, EventArgs e)
        //{
        //    txtCariutang.AutoCompleteMode = AutoCompleteMode.Suggest;
        //    txtCariutang.AutoCompleteSource = AutoCompleteSource.CustomSource;
        //    AutoCompleteStringCollection namec = new AutoCompleteStringCollection();

        //    SqlCommand sql = new SqlCommand("select top 5 tamu from Tamu where tamu like '"+ txtCariutang.Text+ "%'",koneksi.KoneksiDB());
        //    SqlDataReader sqlread = sql.ExecuteReader();
        //    while (sqlread.Read())
        //    {
        //        namec.Add(sqlread["tamu"].ToString());
        //    }
        //    txtCariutang.AutoCompleteCustomSource = namec;
        //    koneksi.closeConnection();

        //}

        
        //private void btnUser_Click(object sender, EventArgs e)
        //{
        //    panelUser.BringToFront();
        //    panelCheckinDate.Visible = false;
        //    panelCheckoutDate.Visible = false;
            
        //}

        //end irwan

    }

}




//irwan tambahkan
class ComboboxItem
{
    public string Text { get; set; }
    public object Value { get; set; }

    public override string ToString()
    {
        return Text;
    }
}
//end irwan
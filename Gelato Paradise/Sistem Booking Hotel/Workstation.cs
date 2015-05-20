using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Microsoft.Reporting.WinForms;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Data.SqlClient;

using System.Runtime.InteropServices;

namespace Sistem_Booking_Hotel
{
    public partial class Workstation : Form
    {

        const int LB_GETHORIZONTALEXTENT = 0x0193;
        const int LB_SETHORIZONTALEXTENT = 0x0194;

        const long WS_HSCROLL = 0x00100000L;

        const int SWP_FRAMECHANGED = 0x0020;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOZORDER = 0x0004;

        const int GWL_STYLE = (-16);


        SqlCommand cmd; SqlCommand cmd1;        
        configconn koneksi = new configconn();
        SqlDataReader reader;

        DataTable invoice_item = new DataTable();
        
        int x = 0;
        double hargainvoice = 0;
        int antrian = 1;
        int rasadipilih = 0;
        string namabtnRasaDipilih;
        int jmlRasaCupUntukDipilih;
        int invoice_item_id_Dipilih;
        
        public Workstation()
        {
            InitializeComponent();
            flowKasirjumlah.HorizontalScroll.Enabled = true;
            AddStyle(flowKasirjumlah.Handle, (uint)WS_HSCROLL);
            SendMessage(flowKasirjumlah.Handle, LB_SETHORIZONTALEXTENT, 1000000, 1000000);
        }

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hwnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern void SetWindowLong(IntPtr hwnd, int index, uint value);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
              int Y, int cx, int cy, uint uFlags);


        private void AddStyle(IntPtr handle, uint addStyle)
        {
            // Get current window style
            uint windowStyle = GetWindowLong(handle, GWL_STYLE);

            // Modify style
            SetWindowLong(handle, GWL_STYLE, windowStyle | addStyle);

            // Let the window know of the changes
            SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOZORDER | SWP_NOSIZE | SWP_FRAMECHANGED);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnKasir_Click(object sender, EventArgs e)
        {
            getItem();
            panelKasir.BringToFront();
            resetBtn();
            flowKasirjumlah.Controls.Clear();
            lblHargaInvoice.Text = "0";
            hargainvoice = 0;
            reset_invoice_item();
            comboMetodePembayaran.SelectedIndex = 0;
            inputPembayaran.Text = "0";
            labelNominalDikembalikan.Text = "0";
            btnOrder.Text = "Bayar";
            flowKasirInput.Enabled = true;
            inputPembayaran.Enabled = true;
            inputVoucher.Enabled = true;
            inputVoucher.Text = "";
        }

        private void reset_invoice_item()
        {
            invoice_item.Reset();
            //invoice_item.Columns.Add("invoice_id".ToString());
            invoice_item.Columns.Add("item", typeof(string));
            invoice_item.Columns.Add("harga", typeof(int));        
        }

        private void getItem()
        {
            flowKasirInput.Controls.Clear();
            cmd = new SqlCommand((@"select count(*) from Item where item_tipe_id = 1"), koneksi.KoneksiDB());
            int jumKamar = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();
            Button[] Item;
            cmd = new SqlCommand(
            (@"select * from Item where item_tipe_id = 1 order by item_id asc"), koneksi.KoneksiDB());
            String baruString = "";
            reader = cmd.ExecuteReader();
            Item = new Button[jumKamar];
            x = 0;
            while (reader.Read())
            {
                Item[x] = new Button();
                Item[x].Text = //reader.GetInt32(0).ToString() + "\n\r" + 
                    reader.GetString(1);
                Item[x].Name = "btn"+reader.GetInt32(0).ToString();
                Item[x].Visible = true;
                Item[x].Height = 80;
                Item[x].Width = 130;
                Item[x].FlatStyle = FlatStyle.Flat;
                Item[x].Tag = reader.GetDouble(3).ToString();
                Item[x].Margin = new Padding(8, 8, 8, 8);
                Item[x].TextAlign = button1.TextAlign;
                Item[x].Font = button1.Font;
                //Item[x].Tag = 
                Item[x].Click += new EventHandler(addItem);
                Item[x].ImageAlign = btnKasir.ImageAlign;

                flowKasirInput.Controls.Add(Item[x]);
                x += 1;
            }
            //conn.Close();
            koneksi.closeConnection();
        }


        private void resetBtn()
        {
            closeKonfigBtn();
            closeLapBtn();
            
        }

        private void btnScooper_Click(object sender, EventArgs e)
        {
            panelAntrianScooper.BringToFront();
            resetBtn();
        }

        private void Workstation_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'suplailain2DataSet.SuplaiLain' table. You can move, or remove it, as needed.
            this.suplaiLainTableAdapter.FillSuplaiLain(this.suplailain2DataSet.SuplaiLain);
            // TODO: This line of code loads data into the 'suplailain2_DataSet.SuplaiLain' table. You can move, or remove it, as needed.
            this.suplaiTableAdapter2.FillSuplaiMaster(this.suplaiMasterDataSet.Suplai);
            // TODO: This line of code loads data into the 'lapsuplaiDataSet.Suplai' table. You can move, or remove it, as needed.
            this.suplaiTableAdapter1.FillLapSuplai(this.lapsuplaiDataSet.Suplai);
            //MessageBox.Show(Login.idS.ToString());
            // TODO: This line of code loads data into the 'sisasuplaiDataSet.SisaSuplai' table. You can move, or remove it, as needed.
            this.sisaSuplaiTableAdapter.Fill(this.sisasuplaiDataSet.SisaSuplai);
            // TODO: This line of code loads data into the 'suplaiDataSet.Suplai' table. You can move, or remove it, as needed.
            this.suplaiTableAdapter.Fill(this.suplaiDataSet.Suplai);
            // TODO: This line of code loads data into the 'suplaiDataSet.Suplai' table. You can move, or remove it, as needed.
            //this.suplaiTableAdapter.FillSuplai(this.suplaiDataSet.Suplai);
            // TODO: This line of code loads data into the 'workstationInvoiceDataSet.Invoice' table. You can move, or remove it, as needed.
            //this.InvoiceTableAdapter.FillGelatoInvoice(this.workstationInvoiceDataSet.Invoice);
            
            SqlCommand sql;
            sql = new SqlCommand(@"update i set status_layan = 1 from Invoice i where 
					convert(varchar(10),waktu_order,120) < convert(varchar(10),getdate(),120)", koneksi.KoneksiDB());
            //sql.Parameters.AddWithValue("@a", Convert.ToInt32(jmlRasaCupDipilih * 1.0 / jmlTotalRasaCupDipilih * 1.0 * jumNetto));
            //sql.Parameters.AddWithValue("@b", Convert.ToInt32(stringRasa));
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            //try
            //{

            //    Process firstProc = new Process();
            //    firstProc.StartInfo.FileName = @"C:\Program Files\Gelato ReportSyncLive\Gelato Paradise Report.exe";
            //    firstProc.EnableRaisingEvents = true;

            //    firstProc.Start();

            //    ////firstProc.WaitForExit();

            //    ////You may want to perform different actions depending on the exit code.
            //    //Console.WriteLine("First process exited: " + firstProc.ExitCode);

            //    //Process secondProc = new Process();
            //    //secondProc.StartInfo.FileName = "mspaint.exe";
            //    //secondProc.Start();

            //}
            //catch (Exception ex)
            //{
            //    //Console.WriteLine("Sinkronisasi Laporan Live Gagal : " + ex.Message);
            //    //return;
            //}

            lblHargaInvoice.Text = "0";
            resetBtn();
            getItem();
            reset_invoice_item();
            comboMetodePembayaran.SelectedIndex = 0;
            check_staff(sender,e);

            //this.reportViewer1.RefreshReport();
            //this.reportViewer1.RefreshReport();
            
            ////kirimCSV();
            ////Upload("ftp://gelatoparadise.co.id", "upload@gelatoparadise.co.id", "!GPsync", @"C:\invoice.csv");
            //update_data();
            //System.Diagnostics.Process.Start("http://gelatoparadise.co.id/upload/load_stock_rows.php");
            ////UploadFtpFile("upload", "C:\\myOutput.csv");
    //        UploadFile(string FtpUrl, string fileName, string userName, string password,string
    //UploadDirectory="");
            //buatCSV();

            if (Login.idS == 14 || Login.idS == 1 || Login.idS == 5)
                report_sync();

        }

        private void update_data()
        {
            buatCSV();

            WebClient client = new WebClient();
            int ada_internet = 0;
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            try
            {
                Stream data = client.OpenRead("http://www.google.com");
                StreamReader reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                Console.WriteLine(s);
                data.Close();
                reader.Close();
                ada_internet = 1;
            }
            catch (Exception e)
            {
                ada_internet = 0;                
                MessageBox.Show("Koneksi dengan server laporan live terputus, mohon periksa sambungan internet.");
            }

            if (ada_internet == 1)
            {
                Upload("ftp://gelatoparadise.co.id", "upload@gelatoparadise.co.id", "!GPsync", @"d:\stok.csv");
                Upload("ftp://gelatoparadise.co.id", "upload@gelatoparadise.co.id", "!GPsync", @"d:\profit.csv");

                try
                {
                    Stream data = client.OpenRead("http://gelatoparadise.co.id/upload/load_stock_rows.php");
                    StreamReader reader = new StreamReader(data);
                    string s = reader.ReadToEnd();
                    Console.WriteLine(s);
                    data.Close();
                    reader.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Koneksi dengan server laporan live terputus, mohon periksa sambungan internet.");
                }
            }    
            
        
        }

        private void check_staff(object sender, EventArgs e)
        {
            switch (Login.idS)
            {
                case 14: akses_admin(); btnKasir_Click(sender, e); break;//admin
                case 1: akses_admin(); btnKasir_Click(sender, e); break;//admin
                case 2: akses_scooper(); btnScooper_Click(sender, e); break;//scooper
                case 5: akses_kasir(); btnKasir_Click(sender, e); break;//kasir
                case 6: akses_teamleader(); btnKonfSisaSuplai_Click(sender, e); break;//leader
                case 15: akses_suplai(); btnKonfSuplaiMaster_Click(sender, e); break;//manager
                default: break;//scooper
            }
        }

        private void akses_admin()
        {
            btnKasir.Visible = true;
            btnScooper.Visible = true;
            btnKonfigurasi.Visible = true;
            btnLaporan.Visible = true;
            //btnKonfSuplai.Visible = true;
            //btnKonfSisaSuplai.Visible = true;            
        
        }

        private void akses_scooper()
        {
            btnKasir.Visible = false;
            btnScooper.Visible = true;
            btnKonfigurasi.Visible = false;
            btnLaporan.Visible = false;
            btnKonfSuplai.Visible = false;
            btnKonfSisaSuplai.Visible = false;

        }

        private void akses_kasir()
        {
            btnKasir.Visible = true;
            btnScooper.Visible = false;
            btnKonfigurasi.Visible = false;
            btnLaporan.Visible = false;
            btnKonfSuplai.Visible = false;
            btnKonfSisaSuplai.Visible = false;

        }

        private void akses_teamleader()
        {
            btnKasir.Visible = true;
            btnScooper.Visible = true;
            btnKonfigurasi.Visible = true;
            btnLaporan.Visible = false;
            btnKonfSuplai.Visible = false;
            btnKonfSisaSuplai.Visible = true;
        }

        private void akses_suplai()
        {
            //btnKasir.Visible = false;
            //btnScooper.Visible = false;
            //btnKonfigurasi.Visible = true;
            //btnLaporan.Visible = false;
            //btnKonfSuplai.Visible = true;
            //btnKonfSisaSuplai.Visible = false;
            btnKasir.Visible = true;
            btnScooper.Visible = true;
            btnKonfigurasi.Visible = true;
            btnLaporan.Visible = true;
            //btnKonfSuplai.Visible = true;
            //btnKonfSisaSuplai.Visible = true;            
            //dataSuplaiMaster[4, null].Visible = false;//[4, e.RowIndex]
            dataSuplaiMaster.Columns["gudang"].Visible = true;

        }

        private void button10_Click(object sender, EventArgs e)
        {
            panelScooper.BringToFront();
        }

        private void btnAntrianInvoice_Click(object sender, EventArgs e)
        {
            panelScooper.BringToFront();
        }


        string invoice_id_dipilih = "";
        private void loadCup(object sender,EventArgs e)
        {
            Button btn = sender as Button;
            invoice_id_dipilih = btn.Tag.ToString();
            splitContainerPilihRasa.Visible = false;
            flowCup.Controls.Clear();

            flowPilihRasa.Controls.Clear();

            flowKasirInput.Controls.Clear();

            cmd = new SqlCommand((@"select count(*) from 
                                    Invoice i
                                    inner join
                                    Invoice_item ii
                                    on
                                    i.invoice_id = ii.invoice_id
				                    and 
				                    DATEDIFF(hh,i.waktu_order,getdate()) < 5
                                    where 
                                    i.antrian = @a and ii.status_layan is null"), koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@a", btn.Text);
            
            int jumCup = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();
            Button[] Cup;
            cmd = new SqlCommand(
            (@"select 
                ii.invoice_item_id
                ,ii.invoice_id
                ,ii.item_id
                ,ii.harga
                ,it.item
                ,it.pilihan
                ,count(k.konsumsi_id) konsumsi
                ,it.item_tipe_id
                from 
                Invoice i
                inner join
                Invoice_item ii
                on
                i.invoice_id = ii.invoice_id
                inner join 
                Item it
                on 
                ii.item_id = it.item_id
                left join 
                Konsumsi k
                on k.invoice_item_id = ii.invoice_item_id
                where                
                i.antrian = @a
                and 
                ii.status_layan is null
                and
                it.pilihan > 0
				and 
				DATEDIFF(hh,i.waktu_order,getdate()) < 5
                group by 
                ii.invoice_item_id
                ,ii.invoice_id
                ,ii.item_id
                ,ii.harga
                ,it.item
                ,it.pilihan
                ,it.item_tipe_id"), koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@a", btn.Text);
            reader = cmd.ExecuteReader();
            Cup = new Button[jumCup];
            int cupx = 0;



            while (reader.Read())
            {
                if (reader.GetInt32(7) == 3)
                {
                    cmd = new SqlCommand(@"update ii  
                                                set ii.status_layan = 1,waktu_delivery = getdate()
                                                from invoice_item ii
                                                where ii.invoice_item_id = @a", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@a", invoice_item_id_Dipilih);
                    cmd.ExecuteNonQuery();
                    koneksi.closeConnection();
                }
                else
                {
                    Cup[cupx] = new Button();
                    Cup[cupx].Text = reader.GetString(4) + " (" + reader.GetInt32(6).ToString() + "/" + reader.GetInt32(5).ToString() + ")";// +"\n\r" + 
                    //cupx.ToString();//reader.GetString(1);
                    Cup[cupx].Name = "btnCup" + x.ToString();//reader.GetInt32(0).ToString();
                    Cup[cupx].Visible = true;
                    Cup[cupx].Height = 110;
                    Cup[cupx].Width = 140;
                    Cup[cupx].FlatStyle = FlatStyle.Flat;
                    //Cup[x].Tag = reader.GetDouble(3).ToString();
                    Cup[cupx].Margin = new Padding(15, 15, 15, 15);
                    Cup[cupx].TextAlign = button1.TextAlign;
                    Cup[cupx].Font = button1.Font;
                    Cup[cupx].Tag = reader.GetInt32(0).ToString();
                    //Item[x].Tag = 
                    Cup[cupx].Click += new EventHandler(pilihCup);
                    Cup[cupx].ImageAlign = btnKasir.ImageAlign;



                    flowCup.Controls.Add(Cup[cupx]);
                    cupx += 1;
                }
            }
            ////conn.Close();


            int cupBelumDilayani = 0;

            foreach (Control control in flowCup.Controls)
            {
                if (control is Button && control.Visible == true)
                    cupBelumDilayani = cupBelumDilayani + 1;
            }

            if (cupBelumDilayani == 0)
            {
                cmd = new SqlCommand(@"update i 
                                                set i.status_layan = 1,waktu_delivery=getdate()
                                                from invoice i
                                                where i.invoice_id = @a", koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@a", invoice_id_dipilih);
                cmd.ExecuteNonQuery();
                koneksi.closeConnection();

                btnScooper_Click(sender, e);
            }
            else 
                refreshPilihRasa();

            koneksi.closeConnection();
            panelScooper.BringToFront();
            refreshPilihRasa();
        }




        public void tandaiCup()
        {
            foreach (Control control in flowCup.Controls)
            {
                if (control is Button)
                    if ((control as Button).Tag == namabtnRasaDipilih)
                    {
                        string stringCup = control.Text;
                        int jmlRasaCupDipilih = Convert.ToInt32(control.Text.Substring(control.Text.IndexOf("(") + 1, control.Text.IndexOf("/") - control.Text.IndexOf("(") - 1));//, btn.Text.IndexOf("/")).ToString();
                        int jmlRasaCupTotal = Convert.ToInt32(control.Text.Substring(control.Text.IndexOf("/") + 1, control.Text.IndexOf(")") - control.Text.IndexOf("/") - 1));//, btn.Text.IndexOf("/")).ToString();
                        //btnScooper.Text = jmlRasaCupTotal.ToString();
                        //control.Text = control.Text.Replace("(", "((");
                        control.Text = control.Text.Replace("(" + jmlRasaCupDipilih.ToString() + "/", "(" + (jmlRasaCupDipilih + 1).ToString() + "/");
                        if (jmlRasaCupDipilih == jmlRasaCupTotal-1) deaktivasiPilihanRasa();
                    }
                    else
                    { 
                        control.Enabled = false;
                        control.BackColor = button16.BackColor;
                    }
            }
        }

        public void deaktivasiPilihanRasa()
        {
            foreach (Control control in flowPilihRasa.Controls)
            {
                if (control is Button)
                    //if ((control as Button).Tag == namabtnRasaDipilih)
                    //{
                    //    string stringCup = control.Text;
                    //    int jmlRasaCupDipilih = Convert.ToInt16(control.Text.Substring(control.Text.IndexOf("(") + 1, control.Text.IndexOf("/") - control.Text.IndexOf("(") - 1));//, btn.Text.IndexOf("/")).ToString();
                    //    int jmlRasaCupTotal = Convert.ToInt16(control.Text.Substring(control.Text.IndexOf("/") + 1, control.Text.IndexOf(")") - control.Text.IndexOf("/") - 1));//, btn.Text.IndexOf("/")).ToString();
                    //    btnScooper.Text = jmlRasaCupTotal.ToString();
                    //    //control.Text = control.Text.Replace("(", "((");
                    //    control.Text = control.Text.Replace("(" + jmlRasaCupDipilih.ToString() + "/", "(" + (jmlRasaCupDipilih + 1).ToString() + "/");
                    //}
                    //else
                    {
                        control.Enabled = false;
                        control.BackColor = button16.BackColor;
                    }
            }
        }


        private void pilihCup(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            //btnScooper.Text = btn.Name; // "sdfsdf";//"sdfsdf";//btn.Name; //btn.Tag.ToString();
            namabtnRasaDipilih = btn.Tag.ToString();
            invoice_item_id_Dipilih = Convert.ToInt32(btn.Tag.ToString());
            int jmlRasaCupDipilih = Convert.ToInt32(btn.Text.Substring(btn.Text.IndexOf("(") + 1, btn.Text.IndexOf("/") - btn.Text.IndexOf("(") - 1));//, btn.Text.IndexOf("/")).ToString();
            //btnScooper.Text = jmlRasaCupTotal.ToString();
            //control.Text = control.Text.Replace("(", "((");
            btn.Text = btn.Text.Replace("(" + jmlRasaCupDipilih.ToString() + "/", "(0/");

            jmlRasaCupUntukDipilih = Convert.ToInt16(btn.Text.Substring(btn.Text.IndexOf("/") + 1, btn.Text.IndexOf(")") - btn.Text.IndexOf("/") - 1));//, btn.Text.IndexOf("/")).ToString();
            //btnScooper.Text = jmlRasaCupUntukDipilih.ToString();                        
            //btnScooper.Text = namabtnRasaDipilih;
            rasadipilih = 0;
            splitContainerPilihRasa.Visible = true;
           // LookControl();
            refreshPilihRasa();
            btnCupSelesai.Visible = false;
            //btnScooper.Text =
            //    //btn.Text.Substring(
            //btn.Text.IndexOf("(").ToString() + " " + btn.Text.IndexOf("/").ToString();//, btn.Text.IndexOf("/")).ToString();

            //btnScooper.Text = invoice_item_id_Dipilih.ToString();//btn.Text.Substring(btn.Text.IndexOf("(") + 1, btn.Text.IndexOf("/") - btn.Text.IndexOf("(")-1);//, btn.Text.IndexOf("/")).ToString();
            
            //btn.Text.Substring(7, 1);//, btn.Text.IndexOf("/")).ToString();
            



        }

        private void pilihRasa(object sender, EventArgs e)
        {
            
            btnCupSelesai.Visible = true;
            //btnScooper.Text = namabtnRasaDipilih;
            //Button btnRasaDipilih = flowCup.Controls.Find(namabtnRasaDipilih, true).FirstOrDefault() as Button;
            //((TextBox)flowCup.Controls.Find("controlName", true)[0]).Text = namabtnRasaDipilih;
            //btnRasaDipilih.Text = "x";
            //LookControl();
            rasadipilih = rasadipilih + 1;
            Button btn = sender as Button;
            //btnScooper.Text = btn.Tag.ToString();
            //namabtnRasaDipilih = btn.Name;
            //btnScooper.Text = btn.Text;


            //, btn.Text.IndexOf("/")).ToString();
                        

            if (btn.Text.IndexOf("(") > 0)
            {

                int jmlSatuRasaDipilih = Convert.ToInt16(btn.Text.Substring(btn.Text.IndexOf("(") + 1, btn.Text.IndexOf(")") - btn.Text.IndexOf("(") - 1));

                btn.Text = btn.Text.Substring(0, btn.Text.IndexOf("(")) + "(" + Convert.ToString(jmlSatuRasaDipilih + 1) + ")";//Convert.ToString(Convert.ToInt32(btn.Tag.ToString()) + 1);

                
                //btnScooper.Text = jmlSatuRasaDipilih.ToString();
            

            }
            else
            {
                btn.Text = btn.Text + " (1)";//.Substring(0, btn.Text.IndexOf("("));
            }

                //btn.Tag = Convert.ToString(Convert.ToInt32(btn.Tag.ToString())+1);

                tandaiCup();
        }

        private void refreshPilihRasa()
        {
            flowPilihRasa.Controls.Clear();

            cmd = new SqlCommand((@"SELECT count(*) FROM Suplai where kuantitas > 0"), koneksi.KoneksiDB());
            
            int jumRasa = (int)cmd.ExecuteScalar(); 
            koneksi.closeConnection();
            Button[] Rasa;
            cmd = new SqlCommand(
            (@"select nama,suplai_id 
            from suplai 
            where kuantitas > 0
            order by nama"), koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            Rasa = new Button[jumRasa];
            int rasax = 0;
            while (reader.Read())
            {
                Rasa[rasax] = new Button();
                Rasa[rasax].Text = reader.GetString(0);
                Rasa[rasax].Name = "btnRasa" + x.ToString();//reader.GetInt32(0).ToString();
                Rasa[rasax].Visible = true;
                Rasa[rasax].Height = 70;
                Rasa[rasax].Width = 140;
                Rasa[rasax].FlatStyle = FlatStyle.Flat;
                Rasa[rasax].Margin = new Padding(8, 8, 8, 8);
                Rasa[rasax].TextAlign = button1.TextAlign;
                Rasa[rasax].Font = button1.Font;
                Rasa[rasax].Tag = reader.GetInt32(1);
                Rasa[rasax].Click += new EventHandler(pilihRasa);
                Rasa[rasax].ImageAlign = btnKasir.ImageAlign;

                flowPilihRasa.Controls.Add(Rasa[rasax]);
                rasax += 1;
            }
            ////conn.Close();
            koneksi.closeConnection();
            flowPilihRasa.BringToFront();
        
        }


        private void btnKonfigurasi_Click(object sender, EventArgs e)
        {
            panelKonfigurasi.BringToFront();
            resetBtn();
            openKonfigBtn();
            
        }



        private void openKonfigBtn()
        {
            //btnKonfJabatan.Width = 161;
            //btnKonfJabatan.Height = 40;
            //btnKonfPersonil.Width = 161;
            //btnKonfPersonil.Height = 40;
            btnKonfSuplai.Width = 161;
            btnKonfSuplai.Height = 40;
            btnKonfSisaSuplai.Width = 161;
            btnKonfSisaSuplai.Height = 40;
            btnTutupShift.Width = 161;
            btnTutupShift.Height = 40;
            btnKonfSesuaikanSuplai.Width = 161;
            btnKonfSesuaikanSuplai.Height = 40;

            btnKonfSuplaiMaster.Width = 161;
            btnKonfSuplaiMaster.Height = 40;

            btnKonfSuplaiLain.Width = 161;
            btnKonfSuplaiLain.Height = 40;
        
        }

        private void openLapBtn()
        {
            //btnKonfJabatan.Width = 161;
            //btnKonfJabatan.Height = 40;
            //btnKonfPersonil.Width = 161;
            //btnKonfPersonil.Height = 40;
            btnLapPendapatan.Width = 161;
            btnLapPendapatan.Height = 40;
            btnLapSuplai.Width = 161;
            btnLapSuplai.Height = 40;
        
        }

        private void closeLapBtn()
        {
            //btnKonfJabatan.Width = 161;
            //btnKonfJabatan.Height = 40;
            //btnKonfPersonil.Width = 161;
            //btnKonfPersonil.Height = 40;
            btnLapPendapatan.Width = 0;
            btnLapPendapatan.Height = 0;
            btnLapSuplai.Width = 0;
            btnLapSuplai.Height = 0;
        
        }

        private void closeKonfigBtn()
        {
            btnKonfJabatan.Width = 0;
            btnKonfJabatan.Height = 0;
            btnKonfPersonil.Width = 0;
            btnKonfPersonil.Height = 0;
            btnKonfSuplai.Width = 0;
            btnKonfSuplai.Height = 0;
            btnKonfSisaSuplai.Width = 0;
            btnKonfSisaSuplai.Height = 0;
            btnTutupShift.Width = 0;
            btnTutupShift.Height = 0;
            btnKonfSesuaikanSuplai.Width = 0;
            btnKonfSesuaikanSuplai.Height = 0;
            btnKonfSuplaiMaster.Width = 0;
            btnKonfSuplaiMaster.Height = 0;
            btnKonfSuplaiLain.Height = 0;
            btnKonfSuplaiLain.Width = 0;
        }


        private void btnKonfPersonil_Click(object sender, EventArgs e)
        {
            panelKonfPersonil.BringToFront();
            
            openKonfigBtn();

        }

        private void btnKonfJabatan_Click(object sender, EventArgs e)
        {
            panelKonfJabatan.BringToFront();
            resetBtn();
            openKonfigBtn();
        }

        private void btnLaporan_Click(object sender, EventArgs e)
        {
            resetBtn();
            openLapBtn();
            
        }

        private void batalkanCup(object sender,EventArgs e)
        {
            Button btn = sender as Button;
            
            foreach (Control control in flowKasirjumlah.Controls)
            {
                if (control is Button && control.Name == btn.Name)
                {
                    control.Visible = false;
                }
                
                if (control is Label && control.Name == btn.Name.Replace("order","orderHarga"))
                {
                    control.Visible = false;
                    lblHargaInvoice.Text = (Convert.ToInt32(lblHargaInvoice.Text) - Convert.ToInt32(control.Text)*1.1).ToString();
                }


            }

            Label itemHarga = new Label();
            itemHarga.Visible = false;

        }

        private void addItem(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Button itemDipesan = new Button();
            
            itemDipesan.Name = "order"+btn.Name;
            itemDipesan.Text = btn.Text;
            itemDipesan.Margin = new Padding(3, 3, 3, 3);
            itemDipesan.BackColor = btn.BackColor;
            itemDipesan.Height = btn.Height-40;
            itemDipesan.Width = btn.Width-40;
            itemDipesan.Image = btn.Image;
            itemDipesan.ImageAlign = btn.ImageAlign;
            itemDipesan.FlatStyle = btn.FlatStyle;
            //itemDipesan.Click += new EventHandler(batalkanCup);
            
            Label itemHarga = new Label();
            itemHarga.Name = "orderHarga" + btn.Name;
            itemHarga.Text = btn.Tag.ToString();
            itemHarga.Margin = new Padding(3, 3, 3, 3);
            itemHarga.BackColor = btn.BackColor;


            itemHarga.ImageAlign = btn.ImageAlign;
            itemHarga.FlatStyle = btn.FlatStyle;
            hargainvoice = hargainvoice +
            Convert.ToInt32(btn.Tag.ToString())*1.0;
            lblHargaInvoice.Text = hargainvoice.ToString();

            flowKasirjumlah.Controls.Add(itemDipesan);
            flowKasirjumlah.Controls.Add(itemHarga);
            
            DataRow row_invoice_item = invoice_item.NewRow();
            row_invoice_item["item"] = btn.Text;
            row_invoice_item["harga"] = btn.Tag.ToString();
            invoice_item.Rows.Add(row_invoice_item);


            if (
                    (
                        (DateTime.Now.Date.ToShortDateString() == "01/05/2015" || DateTime.Now.Date.ToShortDateString() == "02/05/2015")
                        && (DateTime.Now > Convert.ToDateTime("21:00"))
                    )
                    ||
                    (
                        (DateTime.Now.Date.ToShortDateString() == "03/05/2015")
                        && (DateTime.Now < Convert.ToDateTime("01:00"))
                    )

                )
            {
            MessageBox.Show("Diskon 10% ditambahkan.");
            
            double harga_diskon = Convert.ToDouble(btn.Tag.ToString()) * 0.1 * -1;
            cmd = new SqlCommand((@"select item from 
                                    item i
                                    where netto =1 
                                    and harga = @a"), koneksi.KoneksiDB());
            cmd.Parameters.AddWithValue("@a", harga_diskon);
            reader = cmd.ExecuteReader();
            reader.Read();

            //MessageBox.Show("Validasi voucher dilakukan. 2");
            string item_diskon = reader.GetString(0);

            row_invoice_item = invoice_item.NewRow();
            row_invoice_item["item"] = item_diskon;// "Diskon 10%";
            row_invoice_item["harga"] = harga_diskon;// Convert.ToInt32(btn.Tag.ToString()) * 0.1 * -1;
            invoice_item.Rows.Add(row_invoice_item);

            lblHargaInvoice.Text = (Convert.ToInt32(lblHargaInvoice.Text) + harga_diskon).ToString();
            //cmd = new SqlCommand(@"update voucher set valid_status = 1,invoice_id = 999999,waktu_pemakaian = getdate() where code = @a", koneksi.KoneksiDB());
            //cmd.Parameters.AddWithValue("@a", inputVoucher.Text);
            //cmd.ExecuteNonQuery();
            //koneksi.closeConnection();

            //invoice_voucher = inputVoucher.Text;

            //inputPembayaran.Focus();
            }
            //MessageBox.Show(Convert.ToDateTime (DateTime.Now.ToShortTimeString()).ToString());

//            MessageBox.Show(DateTime.Now.Date.ToShortDateString());
                //Convert.ToDateTime("17:56").ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            //if (int.TryParse(text2, out num2))
            //{
            //    // It was assigned.
            //}
            
            int num2;
            SqlCommand sql;

            if (btnOrder.Text == "Bayar" )
            {
                
                    if (Convert.ToInt32(lblHargaInvoice.Text) > 0)
                    {

                        if (int.TryParse(inputPembayaran.Text, out num2))
                        {
                  
                        sql = new SqlCommand(@"
                    select 
                    count (antrian) as total_antrian
                    from 
                    invoice i
                    where 
                    datepart(hh,waktu_order) > 6
                    and 
                    convert(varchar(10),waktu_order,120) = convert(varchar(10),getdate(),120)
                    group by
                    convert(varchar(10),waktu_order,120)", koneksi.KoneksiDB());
                        try
                        {
                            antrian = Int32.Parse(sql.ExecuteScalar().ToString());
                            antrian = antrian + 1;

                        }
                        catch { }

                        koneksi.closeConnection();

                        sql = new SqlCommand("insert into Invoice(harga,antrian,status_layan,metode_pembayaran,waktu_order) values (@a,@b,0,@d,getdate())", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", hargainvoice);
                        sql.Parameters.AddWithValue("@b", antrian);
                        sql.Parameters.AddWithValue("@d", comboMetodePembayaran.Text);
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();


                        sql = new SqlCommand(@"
                    select 
                    max(invoice_id) as invoice_id
                    ,convert(varchar(10),waktu_order,120) as tanggal 
                    from 
                    invoice i
                    where 
                    datepart(hh,waktu_order) > 6
                    and 
                    convert(varchar(10),waktu_order,120) = convert(varchar(10),getdate(),120)
                    group by
                    convert(varchar(10),waktu_order,120)", koneksi.KoneksiDB());
                        int invoicemax = Int32.Parse(sql.ExecuteScalar().ToString());
                        koneksi.closeConnection();

                        if (invoice_voucher != null)
                        {
                            sql = new SqlCommand(@"update voucher set invoice_id = @a where code = @b", koneksi.KoneksiDB());
                            sql.Parameters.AddWithValue("@a", invoicemax);
                            sql.Parameters.AddWithValue("@b", invoice_voucher);
                            sql.ExecuteNonQuery();
                            koneksi.closeConnection();
                        }
                        invoice_voucher = null;


                        if (Convert.ToInt32(inputPembayaran.Text) >= Convert.ToInt32(lblHargaInvoice.Text))
                        {
                            ////simpan reservasi
                            List<DataRow> rd = new List<DataRow>();
                            foreach (DataRow dr in invoice_item.Rows)
                            {
                                //MessageBox.Show(invoicemax.ToString());
                                sql = new SqlCommand("insert into invoice_item(invoice_id, item_id, harga, waktu_order) values (@a,(select item_id from item where item = @b),@c,getdate())", koneksi.KoneksiDB());
                                sql.Parameters.AddWithValue("@a", invoicemax);
                                sql.Parameters.AddWithValue("@b", dr["item"]);
                                sql.Parameters.AddWithValue("@c", dr["harga"]);
                                sql.ExecuteNonQuery();
                                koneksi.closeConnection();

                                sql = new SqlCommand(@"update suplailain set kuantitas = kuantitas - 1  where item_id = (select item_id from item where item = @a)", koneksi.KoneksiDB());
                                sql.Parameters.AddWithValue("@a", dr["item"]);
                                sql.ExecuteNonQuery();
                                koneksi.closeConnection();

                            }


                            labelNominalDikembalikan.Text = (Convert.ToInt32(inputPembayaran.Text) - Convert.ToInt32(lblHargaInvoice.Text)).ToString();
                            btnOrder.Text = "Pesanan Baru";
                            flowKasirInput.Enabled = false;
                            inputPembayaran.Enabled = false;
                            //PrintInvisibleControl(flowKasirjumlah, @"C:\button.jpg");

                            reportInvoice.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.InvoiceGelato.rdlc";
                            InvoiceTableAdapter.FillGelatoInvoice(this.workstationInvoiceDataSet.Invoice, invoicemax);
                            ReportParameter parameter1 = new ReportParameter("invoice_id", antrian.ToString());
                            ReportParameter parameter2 = new ReportParameter("dibayarkan", inputPembayaran.Text);
                            reportInvoice.LocalReport.SetParameters(parameter1);
                            reportInvoice.LocalReport.SetParameters(parameter2);
                            reportInvoice.RefreshReport();

                        }
                        else MessageBox.Show("Pembayaran terlalu kecil.");


                        }
                        else MessageBox.Show("Angka pembayaran tidak valid.");
            

                    }
                else MessageBox.Show("Belum ada pesanan terdaftar.");
            
            
            }
            else
                btnKasir_Click(sender, e);
            
                //MessageBox.Show(invoicemax.ToString());
                sql = new SqlCommand(@"delete i 
                from 
                Invoice i
                left join 
                Invoice_item ii
                on 
                i.invoice_id = ii.invoice_id
                where 
                ii.invoice_id is null", koneksi.KoneksiDB());
                sql.ExecuteNonQuery();
                koneksi.closeConnection();
            /*
delete i 
from 
Invoice i
left join 
Invoice_item ii
on 
i.invoice_id = ii.invoice_id
where 
ii.invoice_id is null
             */
            //var bm = new Bitmap(flowKasirjumlah.Width, flowKasirjumlah.Height);
            //DrawToBitmap(flowKasirjumlah, bm);// .DrawToBitmap(bm, bm.Size);
            //string filejpeg = Directory.GetCurrentDirectory() + "\\gambar\\" + DateTime.Now.ToUniversalTime().ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + ".jpeg";
            //bm.Save(filejpeg, ImageFormat.Jpeg);
            //bm.Dispose();                      // get rid of the big one!
            //GC.Collect();
            
            
        }


        private void PrintInvisibleControl(Control myControl, string filename)
        {

            Graphics g = myControl.CreateGraphics();
            //new bitmap object to save the image        
            Bitmap bmp = new Bitmap(myControl.Width, myControl.Height);
            //Drawing control to the bitmap        
            myControl.DrawToBitmap(bmp, new Rectangle(0, 0, myControl.Width, myControl.Height));
            bmp.Save(filename, ImageFormat.Jpeg);
            bmp.Dispose();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            //toolStripStatusTime.Text =

            toolStripStatusTime.Text = "Hari " +
            switchHari((int)DateTime.Now.DayOfWeek)
                + " Tanggal " +
            DateTime.Now.ToString("d-M-yyyy") + " Jam " + DateTime.Now.ToString("HH:mm:ss");

            if (DateTime.Now.Second % 2 == 0)
            {
                refresh_antrian();
            
            }

            if (DateTime.Now.Minute % 30 == 0 && DateTime.Now.Second % 59 == 0 && Login.idS != 2)
            {
                koreksi_invoice();
                ////buatCSV();
                ////kirimCSV();
                ////Upload("ftp://gelatoparadise.co.id", "upload@gelatoparadise.co.id", "!GPsync", @"C:\invoice.csv");
                ////Upload("ftp://gelatoparadise.co.id", "upload@gelatoparadise.co.id", "!GPsync", @"C:\profit.csv");
                //buatCSV();
                ////kirimCSV();
                ////Upload("ftp://gelatoparadise.co.id", "upload@gelatoparadise.co.id", "!GPsync", @"C:\invoice.csv");
                //Upload("ftp://gelatoparadise.co.id", "upload@gelatoparadise.co.id", "!GPsync", @"d:\stok.csv");
                ////System.Diagnostics.Process.Start("http://gelatoparadise.co.id/upload/load_stock_rows.php");
                
                /////untuk pelaporan live
                //update_data();
                /////untuk pelaporan live
                //if (btnKasir.Visible == true)
                //{
                //    //MessageBox.Show("Mempersiapkan Printer Invoice");
                //    //reportInvoice.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.InvoiceGelato.rdlc";
                //    //InvoiceTableAdapter.FillGelatoInvoice(this.workstationInvoiceDataSet.Invoice, 0);
                //    //ReportParameter parameter = new ReportParameter("invoice_id", "0");
                //    //reportInvoice.LocalReport.SetParameters(parameter);
                //    //reportInvoice.RefreshReport();

                //    Process firstProc = new Process();
                //    firstProc.StartInfo.FileName = @"C:\Program Files\Gelato ReportSyncLive\Gelato Paradise Report.exe";
                //    firstProc.EnableRaisingEvents = true;

                //    firstProc.Start();

                //}
            
            }
            
        }

        private void report_sync()
        {
            if (btnKasir.Visible == true)
            {
                //MessageBox.Show("Mempersiapkan Printer Invoice");
                //reportInvoice.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.InvoiceGelato.rdlc";
                //InvoiceTableAdapter.FillGelatoInvoice(this.workstationInvoiceDataSet.Invoice, 0);
                //ReportParameter parameter = new ReportParameter("invoice_id", "0");
                //reportInvoice.LocalReport.SetParameters(parameter);
                //reportInvoice.RefreshReport();

                Process firstProc = new Process();
                firstProc.StartInfo.FileName = @"C:\Program Files\Gelato ReportSyncLive\Gelato Paradise Report.exe";
                firstProc.EnableRaisingEvents = true;

                firstProc.Start();

            }
        }

        private void koreksi_invoice()
        {
            SqlCommand sql;
            //MessageBox.Show(invoicemax.ToString());
            sql = new SqlCommand(@"delete i 
                from 
                Invoice i
                left join 
                Invoice_item ii
                on 
                i.invoice_id = ii.invoice_id
                where 
                ii.invoice_id is null", koneksi.KoneksiDB());
            sql.ExecuteNonQuery();
            koneksi.closeConnection();
        }
        
        private void buatCSV()
        {
            //profit_14h_csv_vw

            cmd = new SqlCommand(
(@"select * from profit_14h_csv_vw"), koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            //reader.Read();

            var file = @"d:\profit.csv";

            using (var stream = File.CreateText(file))
            {
                while (reader.Read())
                {
                    //for (int i = 0; i < reader.Count(); i++)
                    //{
                    //string first = reader[1].ToString();
                    // string second = image.ToString();
                    string csvRow = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                        //first
                        reader[0].ToString(),
                        reader[1].ToString(),
                        reader[2].ToString(),
                        reader[3].ToString(),
                        reader[4].ToString(),
                        reader[5].ToString(),
                        reader[6].ToString(),
                        reader[7].ToString(),
                        reader[8].ToString()
                        );

                    stream.WriteLine(csvRow);
                }
                stream.Close();
                stream.Dispose();
            }


            cmd = new SqlCommand(
            (@"select * from lap_stok_export_vw"), koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            
            file = @"d:\stok.csv";

            using (var stream = File.CreateText(file))
            {
                while (reader.Read())
                {
                    //for (int i = 0; i < reader.Count(); i++)
                    //{
                    //string first = reader[1].ToString();
                    // string second = image.ToString();
                    string csvRow = string.Format("{0};{1};{2};{3};{4};{5}", 
                        //first
                        reader[0].ToString(),
                        reader[1].ToString(),
                        reader[2].ToString(), 
                        reader[3].ToString(),
                        reader[4].ToString(), 
                        reader[5].ToString()
                                                
                        );

                    stream.WriteLine(csvRow);
                }
                stream.Close();
                stream.Dispose();
            }

        }


    ///Base FtpUrl of FTP Server
    ///Local Filename to Upload
    ///Username of FTP Server
    ///Password of FTP Server
    ///[Optional]Specify sub Folder if any
    /// Status String from Server
    public static string UploadFile(string FtpUrl, string fileName, string userName, string password,string
    UploadDirectory="")
    {
        string PureFileName = new FileInfo(fileName).Name;
        String uploadUrl = String.Format("{0}{1}/{2}", FtpUrl,UploadDirectory,PureFileName);
        FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
        req.Proxy = null;
        req.Method = WebRequestMethods.Ftp.UploadFile;
        req.Credentials = new NetworkCredential(userName,password);
        req.UseBinary = true;
        req.UsePassive = true;
        byte[] data = File.ReadAllBytes(fileName);
        req.ContentLength = data.Length;
        Stream stream = req.GetRequestStream();
        stream.Write(data, 0, data.Length);
        stream.Close();
        FtpWebResponse res = (FtpWebResponse)req.GetResponse();
        return res.StatusDescription;
    }


    public void UploadFtpFile(string folderName, string fileName)
    {

        FtpWebRequest request;
        try
        {
            //string folderName;
            //string fileName;
            string absoluteFileName = Path.GetFileName(fileName);

            request = WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}/{2}", "indosuryaasia.com", folderName, absoluteFileName))) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Credentials = new NetworkCredential("upload", "upload");
            //request.ConnectionGroupName = "group";

            using (FileStream fs = File.OpenRead(fileName))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();
                requestStream.Flush();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error");
        }
    }


    private static void Upload(string ftpServer, string userName, string password, string filename)
    {
        using (System.Net.WebClient client = new System.Net.WebClient())
        {
            try
            {
                client.Credentials = new System.Net.NetworkCredential(userName, password);
                client.UploadFile(ftpServer + "/" + new FileInfo(filename).Name, "STOR", filename);

            }catch(Exception e){
                MessageBox.Show("Koneksi dengan server laporan terputus.");
            }
        }
    }
        
        private void kirimCSV()
        { 
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential("upload", "upload");
                //client.Credentials = new NetworkCredential("indosury","!IndoSur100");
            //    client.UploadFile("ftp://ftpserver.com/target.zip, "STOR", localFilePath);
                client.UploadFile("ftp://indosuryaasia.com/stok.csv", "STOR", @"d:\stok.csv");
            }    
        }

        private void refresh_antrian()
        {
            int jumAntrian = 0;

            flowAntrianInvoice.Controls.Clear();
            //try
            //{
                cmd = new SqlCommand((@"select count(*) from Invoice where status_layan = 0"), koneksi.KoneksiDB());
                
                jumAntrian = jumAntrian + (int)cmd.ExecuteScalar();
                //MessageBox.Show(jumAntrian.ToString());
                koneksi.closeConnection();
            //}
            //catch { }

            if (jumAntrian >= 1)
            {
                Button[] Antrian;

                cmd = new SqlCommand(
                (@"select * from Invoice where status_layan = 0 "), koneksi.KoneksiDB());

                String baruString = "";
                reader = cmd.ExecuteReader();
                Antrian = new Button[jumAntrian];
                x = 0;
                while (reader.Read())
                {
                    Antrian[x] = new Button();
                    Antrian[x].Text = reader.GetInt32(2).ToString();// +"\n\r" + 
                    Antrian[x].Name = "btnAntrian" + x.ToString();//reader.GetInt32(0).ToString();
                    Antrian[x].Visible = true;
                    Antrian[x].Height = 113;
                    Antrian[x].Width = 127;
                    Antrian[x].FlatStyle = FlatStyle.Flat;
                    Antrian[x].Tag = reader.GetInt32(0).ToString();
                    Antrian[x].Margin = new Padding(20, 20, 20, 20);
                    Antrian[x].TextAlign = btnAntrianInvoice.TextAlign;
                    Antrian[x].Font = btnAntrianInvoice.Font;
                    Antrian[x].Click += new EventHandler(loadCup);
                    
                    flowAntrianInvoice.Controls.Add(Antrian[x]);
                    x += 1;

                }
                //conn.Close();
                koneksi.closeConnection();

            }
        }

        private void btnNominal1_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1)
                if (inputPembayaran.Text == "0")
                {
                    inputPembayaran.Text = "1";
                }
                else
                {
                    inputPembayaran.Text = inputPembayaran.Text + "1";
                }
            else inputVoucher.Text = inputVoucher.Text + "1";

        }

        private void btnNominal2_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) 
            if (inputPembayaran.Text == "0")
            {
                inputPembayaran.Text = "2";
            }
            else
            {
                inputPembayaran.Text = inputPembayaran.Text + "2";
            }
            else inputVoucher.Text = inputVoucher.Text + "2";
        }

        private void btnNominal3_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) 
            if (inputPembayaran.Text == "0")
            {
                inputPembayaran.Text = "3";
            }
            else
            {
                inputPembayaran.Text = inputPembayaran.Text + "3";
            }
            else inputVoucher.Text = inputVoucher.Text + "3";
        }

        private void btnNominal4_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) 
            if (inputPembayaran.Text == "0")
            {
                inputPembayaran.Text = "4";
            }
            else
            {
                inputPembayaran.Text = inputPembayaran.Text + "4";
            }
            else inputVoucher.Text = inputVoucher.Text + "4";
        }

        private void btnNominal5_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) 
            if (inputPembayaran.Text == "0")
            {
                inputPembayaran.Text = "5";
            }
            else
            {
                inputPembayaran.Text = inputPembayaran.Text + "5";
            }
            else inputVoucher.Text = inputVoucher.Text + "5";
        }

        private void btnNominal6_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) 
            if (inputPembayaran.Text == "0")
            {
                inputPembayaran.Text = "6";
            }
            else
            {
                inputPembayaran.Text = inputPembayaran.Text + "6";
            }
            else inputVoucher.Text = inputVoucher.Text + "6";
        }

        private void btnNominal7_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) 
            if (inputPembayaran.Text == "0")
            {
                inputPembayaran.Text = "7";
            }
            else
            {
                inputPembayaran.Text = inputPembayaran.Text + "7";
            }
            else inputVoucher.Text = inputVoucher.Text + "7";
        }

        private void btnNominal8_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) 
            if (inputPembayaran.Text == "0")
            {
                inputPembayaran.Text = "8";
            }
            else
            {
                inputPembayaran.Text = inputPembayaran.Text + "8";
            }else inputVoucher.Text = inputVoucher.Text + "8";
        }

        private void btnNominal9_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) 
            if (inputPembayaran.Text == "0")
            {
                inputPembayaran.Text = "9";
            }
            else
            {
                inputPembayaran.Text = inputPembayaran.Text + "9";
            }
            else inputVoucher.Text = inputVoucher.Text + "9";
        }

        private void btnNominal0_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) 
            if (inputPembayaran.Text == "0")
            {
                inputPembayaran.Text = "0";
            }
            else
            {
                inputPembayaran.Text = inputPembayaran.Text + "0";
            }else inputVoucher.Text = inputVoucher.Text + "0";
        }

        private void btnNominal000_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) 
            if (inputPembayaran.Text == "0")
            {
                inputPembayaran.Text = "0";
            }
            else
            {
                inputPembayaran.Text = inputPembayaran.Text + "000";
            }
            else inputVoucher.Text = inputVoucher.Text + "000";
        }

        private void btnNominalKosong_Click(object sender, EventArgs e)
        {
            if (fokusPembayaran == 1) inputPembayaran.Text = "0"; else inputVoucher.Text = "";
        }

        private void btnCupSelesai_Click(object sender, EventArgs e)
        {
            SqlCommand sql;
            
            string namaRasa = "";
            
            cmd = new SqlCommand((@"select sum(i.netto) netto
                                    from Invoice_item ii inner join 
                                    item i
                                    on 
                                    ii.item_id = i.item_id
                                    where 
                                    ii.invoice_item_id = @a"), koneksi.KoneksiDB());
            
            cmd.Parameters.AddWithValue("@a", invoice_item_id_Dipilih);
            
            int jumNetto = (int)cmd.ExecuteScalar();
            koneksi.closeConnection();
            int jmlTotalRasaCupDipilih = 0;

            foreach (Control control in flowCup.Controls)
            {
                if (control is Button && control.Enabled == true)
                {
                    control.Visible = false;
                }
                else
                {
                    control.Enabled = true;
                    control.BackColor = btnKasir.BackColor;
                }
            }

            foreach (Control control in flowPilihRasa.Controls)
            {
                if (control is Button && (control.Text.IndexOf("(") > 0))
                //if ((control as Button).Tag == namabtnRasaDipilih)
                {
                    ////string stringRasa = control.Tag.ToString();
                    int jmlRasaCupDipilih =
                        Convert.ToInt32(
                        control.Text.Substring(
                        control.Text.IndexOf("(") + 1,
                        control.Text.IndexOf(")") - control.Text.IndexOf("(") - 1)
                        );//, btn.Text.IndexOf("/")).ToString();

                    /////MessageBox.Show(jmlRasaCupDipilih.ToString());
                    ////namaRasa = namaRasa + " " + stringRasa + "(" + jmlRasaCupDipilih.ToString() + ")";
                    jmlTotalRasaCupDipilih = jmlTotalRasaCupDipilih + jmlRasaCupDipilih;
                    ////control.Visible = false;

                }
                //else control.Enabled = true;
            }

            foreach (Control control in flowPilihRasa.Controls)
            {

                if (control is Button && (control.Text.IndexOf("(") > 0))
                //if ((control as Button).Tag == namabtnRasaDipilih)
                {
                    string stringRasa = control.Tag.ToString();
                    int jmlRasaCupDipilih = Convert.ToInt32(control.Text.Substring(control.Text.IndexOf("(") + 1, control.Text.IndexOf(")") - control.Text.IndexOf("(") - 1));//, btn.Text.IndexOf("/")).ToString();
                    //namaRasa = namaRasa + " " + stringRasa + "(" + jmlRasaCupDipilih.ToString() + ")";
                    //int jmlRasaCupTotal = Convert.ToInt16(control.Text.Substring(control.Text.IndexOf("/") + 1, control.Text.IndexOf(")") - control.Text.IndexOf("/") - 1));//, btn.Text.IndexOf("/")).ToString();
                    ////btnScooper.Text = jmlRasaCupTotal.ToString();
                    ////control.Text = control.Text.Replace("(", "((");
                    //control.Text = control.Text.Replace("(" + jmlRasaCupDipilih.ToString() + "/", "(" + (jmlRasaCupDipilih + 1).ToString() + "/");
                    //if (jmlRasaCupDipilih == jmlRasaCupTotal - 1) deaktivasiPilihanRasa();


                    sql = new SqlCommand(@"insert into Konsumsi(suplai_id,invoice_item_id,kuantitas) values (@a,@b,@c)", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", Convert.ToInt32(stringRasa));
                    sql.Parameters.AddWithValue("@b", invoice_item_id_Dipilih);
                    sql.Parameters.AddWithValue("@c", Convert.ToInt32(jmlRasaCupDipilih * 1.0 / jmlTotalRasaCupDipilih * 1.0 * jumNetto));
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    sql = new SqlCommand(@"update s set kuantitas = kuantitas - @a from suplai s where suplai_id = @b", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", Convert.ToInt32(jmlRasaCupDipilih * 1.0 / jmlTotalRasaCupDipilih * 1.0 * jumNetto));
                    sql.Parameters.AddWithValue("@b", Convert.ToInt32(stringRasa));
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    sql = new SqlCommand(@"update ii  
                                                set ii.status_layan = 1,waktu_delivery = getdate()
                                                from invoice_item ii
                                                where ii.invoice_item_id = @a", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", invoice_item_id_Dipilih);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();
                }

                //btnScooper.Text = namaRasa;
                //else
                //{
                //    control.Enabled = false;
                //    control.BackColor = button16.BackColor;
                //}
            }

            int cupBelumDilayani = 0;

            foreach (Control control in flowCup.Controls)
            {
                if (control is Button && control.Visible == true)
                    cupBelumDilayani = cupBelumDilayani + 1;
            }

            if (cupBelumDilayani == 0)
            {
                sql = new SqlCommand(@"update i 
                                                set i.status_layan = 1,waktu_delivery=getdate()
                                                from invoice i
                                                where i.invoice_id = @a", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", invoice_id_dipilih);
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                btnScooper_Click(sender, e);
            }
            else refreshPilihRasa();

            btnCupSelesai.Visible = false;
        }

        private void reportInvoice_RenderingComplete(object sender, Microsoft.Reporting.WinForms.RenderingCompleteEventArgs e)
        {
            reportInvoice.PrintDialog();
        }
        int fokusPembayaran = 1;
        private void inputPembayaran_MouseClick(object sender, MouseEventArgs e)
        {
            fokusPembayaran = 1;
        }

        private void inputVoucher_MouseClick(object sender, MouseEventArgs e)
        {
            fokusPembayaran = 2;
        }

        private void btnKonfSuplai_Click(object sender, EventArgs e)
        {
            panelKonfSuplai.BringToFront();
        }

        private void dataSuplai_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.Validate();
                this.suplaiBindingSource.EndEdit();
                this.suplaiTableAdapter.Update(this.suplaiDataSet.Suplai);
                //MessageBox.Show("Update berhasil");
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("Update gagal");
            }
        }

        private void dataSuplai_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2)
                {
                    e.Value = 0;
                }
            }
            catch (Exception ex)
            {

            }
        }



        private void btnKonfSisaSuplai_Click(object sender, EventArgs e)
        {
            panelKonfSisaSuplai.BringToFront();
            isikanSemuaRasaSisaSuplai();
            this.sisaSuplaiTableAdapter.Fill(this.sisasuplaiDataSet.SisaSuplai);
        }

        private void isikanSemuaRasaSisaSuplai()
        {
            SqlCommand sql;

            sql = new SqlCommand("delete from SisaSuplai where kuantitas = 0", koneksi.KoneksiDB());
            sql.ExecuteNonQuery();
            koneksi.closeConnection();

            sql = new SqlCommand("insert into SisaSuplai(nama,kuantitas,waktu) select nama,0 as kuantitas,getdate() as waktu from Suplai", koneksi.KoneksiDB());
            sql.ExecuteNonQuery();
            koneksi.closeConnection();
        }

        private void dataSisaSuplai_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.Validate();
                this.sisaSuplaiBindingSource.EndEdit();
                this.sisaSuplaiTableAdapter.Update(this.sisasuplaiDataSet.SisaSuplai);
                //MessageBox.Show("Update berhasil");
                //isikanSemuaRasaSisaSuplai();
                
            
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("Update gagal");
            }

        }

        string invoice_voucher = null;

        private void inputVoucher_TextChanged(object sender, EventArgs e)
        {
            if (inputVoucher.Text.Length == 7) 
            {


                //MessageBox.Show("Validasi voucher dilakukan. 1");
                cmd = new SqlCommand((@"select * from 
                                    voucher v
                                    where 
                                    v.code = @a and valid_status = 1"), koneksi.KoneksiDB());
                cmd.Parameters.AddWithValue("@a", inputVoucher.Text);
                reader = cmd.ExecuteReader();
                reader.Read();
                //MessageBox.Show("Validasi voucher dilakukan. 2");
                int voucher_nominal = reader.GetInt32(4);
                //int voucher_valid = (int)cmd.ExecuteScalar();
                koneksi.closeConnection();

                //MessageBox.Show("Validasi voucher dilakukan.");
                if (voucher_nominal >= 1)
                {
                    MessageBox.Show("Validasi voucher berhasil.");

                    DataRow row_invoice_item = invoice_item.NewRow();
                    row_invoice_item["item"] = "Voucher Small";
                    row_invoice_item["harga"] = voucher_nominal * -1;
                    invoice_item.Rows.Add(row_invoice_item);          

                    lblHargaInvoice.Text = (Convert.ToInt32(lblHargaInvoice.Text) - voucher_nominal).ToString();
                    cmd= new SqlCommand(@"update voucher set valid_status = 1,invoice_id = 999999,waktu_pemakaian = getdate() where code = @a", koneksi.KoneksiDB());
                    cmd.Parameters.AddWithValue("@a", inputVoucher.Text);                
                    cmd.ExecuteNonQuery();
                    koneksi.closeConnection();

                    invoice_voucher = inputVoucher.Text;
                    
                    inputPembayaran.Focus();
                }
            }
        }

        private void inputPembayaran_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void lblHargaInvoice_TextChanged(object sender, EventArgs e)
        {
            lblHargaItem.Text =   (Convert.ToInt32(lblHargaInvoice.Text) / 11 * 10).ToString();
        }

        private void btnLapPendapatan_Click(object sender, EventArgs e)
        {
            //panelLapPendapatan.BringToFront();
            reportTutupShift();
        }

        private void reportTutupShift()
        {
            reportLapPendapatan.LocalReport.ReportEmbeddedResource = "Sistem_Booking_Hotel.LaporanTutupShift.rdlc";
            ShiftReportTableAdapter.FillShiftReport(this.shiftreportDataSet.ShiftReport);
            reportLapPendapatan.RefreshReport();
            panelLapPendapatan.BringToFront();
            
        }

        private void btnTutupShift_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Tutup shift sekarang?", "Konfirmasi", MessageBoxButtons.YesNo);
            
            if (result == DialogResult.Yes)
            {

                int shift = 0;
                DateTime waktuakhirshift = DateTime.Now.Date;
                SqlCommand sql;
                try
                {
                    sql = new SqlCommand(
                    (@"select top 1 convert(int,shift) shift 
            ,waktu 
            from ShiftReport order by waktu desc
            "), koneksi.KoneksiDB());
                    reader = sql.ExecuteReader();
                    reader.Read();

                    shift = reader.GetInt32(0);
                    waktuakhirshift = reader.GetDateTime(1);
                }
                catch
                {

                }


                if (shift > 0)
                {
                    if (shift == 1)
                    {
                        //MessageBox.Show("1");
                        sql = new SqlCommand(@"insert into ShiftReport(waktu,shift,invoice,pendapatan) 
                    select 
                    getdate()                 
                    ,2
                    ,count(*) invoice
                    ,convert(int, isnull(sum(i.harga),0)) pendapatan
                    from Invoice i
                    where 
                    waktu_order between @a and getdate()
                    ", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", waktuakhirshift);
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();

                    }
                    else
                    {
                        //MessageBox.Show("2");
                        sql = new SqlCommand(@"insert into ShiftReport(waktu,shift,invoice,pendapatan) 
                    select 
                    getdate()                 
                    ,1
                    ,count(*) invoice
                    ,convert(int, isnull(sum(i.harga),0)) pendapatan
                    from Invoice i
                    where 
                    waktu_order between @a and getdate() 
                    ", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", waktuakhirshift.ToString());
                        sql.ExecuteNonQuery();
                        koneksi.closeConnection();
                    }
                }
                else
                {
                    MessageBox.Show("0");
                    sql = new SqlCommand(@"insert into ShiftReport(waktu,shift,invoice,pendapatan) 
                    select 
                    getdate()                 
                    ,1
                    ,count(*) invoice
                    ,convert(int, isnull(sum(i.harga),0)) pendapatan
                    from Invoice i
                    where 
                    waktu_order between dateadd(hh,9,convert(varchar(10),getdate(),120)) and getdate()", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", waktuakhirshift);
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                }
                reportTutupShift();

            }
        }
        /*
//        int shift = 1;
        private void btnTutupShift_Click(object sender, EventArgs e)
        {
            SqlCommand sql;
            sql = new SqlCommand(
            (@"select convert(int,shift) shift from ShiftState"), koneksi.KoneksiDB());
            reader = sql.ExecuteReader();
            reader.Read();

            int shift = reader.GetInt32(0);
            //DateTime waktuakhirshift = reader.GetDateTime(1);
 
            if(shift==1)
            {

                sql = new SqlCommand(
                    (@"select 
                    count(*) invoice
                    ,convert(int, isnull(sum(i.harga),0)) pendapatan
                    from Invoice i
                    where 
                    convert(varchar(10),getdate(),120) = convert(varchar(10),waktu_order,120)
                    and datepart(hh,waktu_order) between 9 and getdate()"), koneksi.KoneksiDB());
                reader = sql.ExecuteReader();
                reader.Read();
                int invoiceshift = reader.GetInt32(0);
                int pendapatanshift = reader.GetInt32(1);
                koneksi.closeConnection();


                sql = new SqlCommand(@"insert into ShiftReport(waktu,shift,invoice,pendapatan) values (getdate(),@a,@b,@c)", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", shift.ToString());
                sql.Parameters.AddWithValue("@b", invoiceshift.ToString());
                sql.Parameters.AddWithValue("@c", pendapatanshift.ToString());
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                sql = new SqlCommand(@"update shiftstate set shift = 2,waktu =getdate()", koneksi.KoneksiDB());
                sql.ExecuteNonQuery();
                koneksi.closeConnection();
                
            }
            else 
            {
                //btnTutupShift.Text = btnTutupShift.Text.Replace("2", "1");
                sql = new SqlCommand(
            (@"select convert(int,shift) shift,waktu from ShiftState"), koneksi.KoneksiDB());
                reader = sql.ExecuteReader();
                reader.Read();
                //MessageBox.Show("asdsad");
                DateTime waktushiftsebelumnya = reader.GetDateTime(1);
                //MessageBox.Show(waktushiftsebelumnya.ToString());

                sql = new SqlCommand(
    (@"select 
                    count(*) invoice
                    ,convert(int, isnull(sum(i.harga),0)) pendapatan
                    from Invoice i
                    where 
                    convert(varchar(10),getdate(),120) = convert(varchar(10),waktu_order,120)
                    and datepart(hh,waktu_order) between 9 and getdate()"), koneksi.KoneksiDB());
                reader = sql.ExecuteReader();
                reader.Read();
                int invoiceshift = reader.GetInt32(0);
                int pendapatanshift = reader.GetInt32(1);
                koneksi.closeConnection();



                sql = new SqlCommand(@"insert into ShiftReport(waktu,shift,invoice,pendapatan) values (getdate(),@a,@b,@c)", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", shift.ToString());
                sql.Parameters.AddWithValue("@b", invoiceshift.ToString());
                sql.Parameters.AddWithValue("@c", pendapatanshift.ToString());
                sql.ExecuteNonQuery();
                koneksi.closeConnection();


                sql = new SqlCommand(@"update shiftstate set shift = 1,waktu =getdate()", koneksi.KoneksiDB());
                //sql.Parameters.AddWithValue("@a", shift.ToString());
                //sql.Parameters.AddWithValue("@b", invoiceshift.ToString());
                //sql.Parameters.AddWithValue("@c", pendapatanshift.ToString());
                sql.ExecuteNonQuery();
                koneksi.closeConnection();

                

            }
            reportTutupShift();
            
        }
        */
        private void reportLapPendapatan_RenderingComplete(object sender, RenderingCompleteEventArgs e)
        {
            reportLapPendapatan.PrintDialog();
        }

        private void btnLapSuplai_Click(object sender, EventArgs e)
        {
            dataLapSuplai.Refresh();
            dataLapSuplai.BringToFront();
            
        }

        private void btnKonfSesuaikanSuplai_Click(object sender, EventArgs e)
        {

            SqlCommand sql;
            sql = new SqlCommand(@"update s
                                    set 
                                    s.kuantitas = ssv.kuantitas
                                    from 
                                    sisasuplai_vw ssv
                                    inner join 
                                    Suplai s
                                    on 
                                    ssv.suplai_id = s.suplai_id
                                    where 
                                    ssv.kuantitas is not null
                                ", koneksi.KoneksiDB());
            //sql.Parameters.AddWithValue("@a", Convert.ToInt32(jmlRasaCupDipilih * 1.0 / jmlTotalRasaCupDipilih * 1.0 * jumNetto));
            //sql.Parameters.AddWithValue("@b", Convert.ToInt32(stringRasa));
            sql.ExecuteNonQuery();
            koneksi.closeConnection();
            MessageBox.Show("Suplai Telah Disesuaikan Dengan Laporan Suplai Manual.");
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void inputPembayaran_Enter(object sender, EventArgs e)
        {
            fokusPembayaran = 1;
        }

        private void btnKonfSuplaiMaster_Click(object sender, EventArgs e)
        {
            this.suplaiTableAdapter2.FillSuplaiMaster(this.suplaiMasterDataSet.Suplai);
            
            dataSuplaiMaster.BringToFront();
        }

        private void dataSuplaiMaster_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SqlCommand sql;
            if (e.ColumnIndex == 5)
            {
                //MessageBox.Show((e.RowIndex + 1) + "  Row  " + (e.ColumnIndex + 1) + "  Column button clicked ");
                //MessageBox.Show(dataSuplaiMaster.Rows[e.RowIndex + 1].Cells[e.ColumnIndex + 1]);
                //dataSuplaiMaster.Item("kulkas", 5);
                //MessageBox.Show(dataSuplaiMaster[0, e.RowIndex].Value.ToString());
                
                //SqlCommand sql;
                //sql = new SqlCommand(@"update s set kulkas = @b from suplai s where suplai_id = @a", koneksi.KoneksiDB());
                //sql.Parameters.AddWithValue("@a", Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()));
                //sql.Parameters.AddWithValue("@b", Convert.ToInt32(dataSuplaiMaster[0, e.RowIndex].Value.ToString()));
                //sql.ExecuteNonQuery();
                //koneksi.closeConnection();

                try
                {

                    sql = new SqlCommand(@"insert into jurnal_suplai(suplai_id,kuantitas,waktu ) values (@a,@b,getdate())", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", dataSuplaiMaster[0, e.RowIndex].Value.ToString());
                    sql.Parameters.AddWithValue("@b", dataSuplaiMaster[5, e.RowIndex].Value.ToString());
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    dataSuplaiMaster[4, e.RowIndex].Value = Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()) + Convert.ToInt32(dataSuplaiMaster[5, e.RowIndex].Value.ToString());
                    dataSuplaiMaster[7, e.RowIndex].Value = Convert.ToInt32(dataSuplaiMaster[7, e.RowIndex].Value.ToString()) - Convert.ToInt32(dataSuplaiMaster[5, e.RowIndex].Value.ToString());
                    
                    dataSuplaiMaster[5, e.RowIndex].Value = 0;

                    this.Validate();
                    this.suplaiBindingSource2.EndEdit();
                    this.suplaiTableAdapter2.Update(this.suplaiMasterDataSet.Suplai);
                    

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Update gagal, hubungi IT.");
                }

            }

            if (e.ColumnIndex == 6)
            {
                //MessageBox.Show((e.RowIndex + 1) + "  Row  " + (e.ColumnIndex + 1) + "  Column button clicked ");
                //MessageBox.Show(dataSuplaiMaster.Rows[e.RowIndex + 1].Cells[e.ColumnIndex + 1]);
                //dataSuplaiMaster.Item("kulkas", 5);
                //MessageBox.Show(dataSuplaiMaster[0, e.RowIndex].Value.ToString());

                //SqlCommand sql;
                //sql = new SqlCommand(@"update s set kulkas = @b from suplai s where suplai_id = @a", koneksi.KoneksiDB());
                //sql.Parameters.AddWithValue("@a", Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()));
                //sql.Parameters.AddWithValue("@b", Convert.ToInt32(dataSuplaiMaster[0, e.RowIndex].Value.ToString()));
                //sql.ExecuteNonQuery();
                //koneksi.closeConnection();

                try
                {

                    sql = new SqlCommand(@"insert into jurnal_suplai(suplai_id,kuantitas,waktu ) values (@a,@b,getdate())", koneksi.KoneksiDB());
                    sql.Parameters.AddWithValue("@a", dataSuplaiMaster[0, e.RowIndex].Value.ToString());
                    sql.Parameters.AddWithValue("@b", (Convert.ToInt32(dataSuplaiMaster[6, e.RowIndex].Value) - Convert.ToInt32(dataSuplaiMaster[2, e.RowIndex].Value)).ToString() );
                    sql.ExecuteNonQuery();
                    koneksi.closeConnection();

                    dataSuplaiMaster[2, e.RowIndex].Value = Convert.ToInt32(dataSuplaiMaster[6, e.RowIndex].Value.ToString());
                    //dataSuplaiMaster[5, e.RowIndex].Value = 0;

                    this.Validate();
                    this.suplaiBindingSource2.EndEdit();
                    this.suplaiTableAdapter2.Update(this.suplaiMasterDataSet.Suplai);
                    MessageBox.Show("Update berat timbangan berhasil");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Update gagal, hubungi IT.");
                }

            }

            if (e.ColumnIndex == 2)
            {
                //if (Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()) >= Convert.ToInt32(dataSuplaiMaster[2, e.RowIndex].Value.ToString()))
                if (Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()) + Convert.ToInt32(display) >= Convert.ToInt32(dataSuplaiMaster[2, e.RowIndex].Value.ToString()))
                {
                    //MessageBox.Show(dataSuplaiMaster[0, e.RowIndex].Value.ToString());

                    //dataSuplaiMaster[4, e.RowIndex].Value = Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()) + Convert.ToInt32(dataSuplaiMaster[5, e.RowIndex].Value.ToString());
                    // dataSuplaiMaster[2, e.RowIndex].Value = 0;
                    ////MessageBox.Show((e.RowIndex + 1) + "  Row  " + (e.ColumnIndex + 1) + "  Column button clicked ");
                    //dataSuplaiMaster[4, e.RowIndex].Value = Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()) + Convert.ToInt32(dataSuplaiMaster[5, e.RowIndex].Value.ToString());
                    //dataSuplaiMaster[5, e.RowIndex].Value = 0;


                    try
                    {
                        dataSuplaiMaster[4, e.RowIndex].Value = Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()) + Convert.ToInt32(display) - Convert.ToInt32(dataSuplaiMaster[2, e.RowIndex].Value.ToString());

                        //dataSuplaiMaster[4, e.RowIndex].Value = Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()) - Convert.ToInt32(dataSuplaiMaster[2, e.RowIndex].Value.ToString());
                        this.Validate();
                        this.suplaiBindingSource2.EndEdit();
                        this.suplaiTableAdapter2.Update(this.suplaiMasterDataSet.Suplai);
                        //MessageBox.Show("Update berhasil");
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Input Gagal, hubungi IT.");
                    }
                }
                else
                {
                    try
                    {
                        dataSuplaiMaster[2, e.RowIndex].Value = display;
                    }

                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Stok kulkas tidak mencukupi.");
                    }
                }
            }
            else 
            {
                //dataSuplaiMaster[4, e.RowIndex].Value = 0;
                //dataSuplaiMaster[1, e.RowIndex].Value  = "Testing 4";
                //MessageBox.Show((e.RowIndex + 1) + "  Row  " + (e.ColumnIndex + 1) + "  Column button clicked ");
                this.Validate();
                this.suplaiBindingSource2.EndEdit();
                this.suplaiTableAdapter2.Update(this.suplaiMasterDataSet.Suplai);
                        
            }

        }

        string display;// = dataSuplaiMaster[2, e.RowIndex].Value.ToString();
            
        private void dataSuplaiMaster_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            display = dataSuplaiMaster[2, e.RowIndex].Value.ToString();
            
            if (e.ColumnIndex == 3)
            {

                //dataSuplaiMaster[4, e.RowIndex].Value = Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()) + Convert.ToInt32(dataSuplaiMaster[5, e.RowIndex].Value.ToString());
                ////MessageBox.Show((e.RowIndex + 1) + "  Row  " + (e.ColumnIndex + 1) + "  Column button clicked ");
                //dataSuplaiMaster[4, e.RowIndex].Value = Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()) + Convert.ToInt32(dataSuplaiMaster[5, e.RowIndex].Value.ToString());
                //dataSuplaiMaster[5, e.RowIndex].Value = 0;
                
                //MessageBox.Show(dataSuplaiMaster[0, e.RowIndex].Value.ToString());

                try
                {

                    dataSuplaiMaster[4, e.RowIndex].Value = Convert.ToInt32(dataSuplaiMaster[4, e.RowIndex].Value.ToString()) + Convert.ToInt32(dataSuplaiMaster[2, e.RowIndex].Value.ToString());
                    dataSuplaiMaster[2, e.RowIndex].Value = 0;
                
                    this.Validate();
                    this.suplaiBindingSource2.EndEdit();
                    this.suplaiTableAdapter2.Update(this.suplaiMasterDataSet.Suplai);
                    //MessageBox.Show("Update berhasil");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Update gagal, hubungi IT.");
                }
            }


        }



        private void dataSuplaiMaster_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 3)
                {
                    e.Value = "Kembali Ke Kulkas";
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void dataSuplaiMaster_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
//            if (e.ColumnIndex == 2)
//            {
////                SqlCommand sql;
////                sql = new SqlCommand(@"update i set status_layan = 1 from Invoice i where 
////                					convert(varchar(10),waktu_order,120) < convert(varchar(10),getdate(),120)", koneksi.KoneksiDB());
////                //sql.Parameters.AddWithValue("@a", Convert.ToInt32(jmlRasaCupDipilih * 1.0 / jmlTotalRasaCupDipilih * 1.0 * jumNetto));
////                //sql.Parameters.AddWithValue("@b", Convert.ToInt32(stringRasa));
////                sql.ExecuteNonQuery();
////                koneksi.closeConnection();
//                //MessageBox.Show("changed");
//                MessageBox.Show(dataSuplaiMaster[1, e.RowIndex].Value.ToString());
//            }

        }

        private void dataSuplaiMaster_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            MessageBox.Show("Periksa kembali input data.");
        }

        private void dataSuplaiMaster_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            display = dataSuplaiMaster[2, e.RowIndex].Value.ToString();
            
        }

        private void btnKonfSuplaiLain_Click(object sender, EventArgs e)
        {
            this.suplaiLainTableAdapter.FillSuplaiLain(this.suplailain2DataSet.SuplaiLain);
            dataSuplaiLain.BringToFront();
        }

        private void dataSuplaiLain_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.Validate();
                this.suplaiLainBindingSource2.EndEdit();
                this.suplaiLainTableAdapter.Update(this.suplailain2DataSet.SuplaiLain);

                SqlCommand sql;

                sql = new SqlCommand(@"insert into jurnal_suplai_lain(suplai_lain_id,kuantitas,waktu ) values (@a,@b,getdate())", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", dataSuplaiLain[0, e.RowIndex].Value.ToString());
                sql.Parameters.AddWithValue("@b", dataSuplaiLain[2, e.RowIndex].Value.ToString());
                sql.ExecuteNonQuery();
                koneksi.closeConnection();
                //MessageBox.Show("Update berhasil");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Update gagal");
            }

        }

        private void btnScooper_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Workstation_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        
        }

        private void Workstation_FormClosing(object sender, FormClosingEventArgs e)
        {
            var reportsyncProcesses = Process.GetProcesses().Where(pr => pr.ProcessName == "ReportSync");

            foreach (var process in reportsyncProcesses)
            {
                process.Kill();
            }//Gelato Paradise Report.exe

            reportsyncProcesses = Process.GetProcesses().Where(pr => pr.ProcessName == "Gelato Paradise Report");

            foreach (var process in reportsyncProcesses)
            {
                process.Kill();
            }//Gelato Paradise Report.exe

            //foreach (var process in Process.GetProcessesByName("notepad.exe"))
            //{
            //    process.Kill();
            //}

            //foreach (var process in Process.GetProcessesByName("ReportSync.exe"))
            //{
            //    process.Kill();
            //}
            //foreach (var process in Process.GetProcessesByName("Gelato Paradise Report.exe"))
            //{
            //    process.Kill();
            //}
        }

        

//        int shift = 1;
//        private void btnTutupShift_Click(object sender, EventArgs e)
//        {   

//            SqlCommand sql;
//            sql = new SqlCommand(
//                (@"select 
//                    count(*) shiftakhir
//                    from shiftreport
//                    where 
//                    shift  = 2
//                    and 
//                    (waktu > 
//                    dateadd(hh,6, dateadd(dd,-1,convert(varchar(10),getdate(),120)))
//                    )
//                    or
//                    (
//                    waktu between
//                    dateadd(hh,0, dateadd(dd,0,convert(varchar(10),getdate(),120)))
//                    and
//                    dateadd(hh,6, dateadd(dd,0,convert(varchar(10),getdate(),120)))
//                    )"), koneksi.KoneksiDB());
//            int shiftakhir = (int)sql.ExecuteScalar();
//            //while (reader.Read())
//            //{

//            //    shift = 1;//reader.GetInt32(0);
//            //}

//            //string myScalarQuery = "select count(*) from TableName";

//            //SqlCommand myCommand = new SqlCommand(myScalarQuery, myConnection);
//            //myCommand.Connection.Open();
//            //int count = (int)myCommand.ExecuteScalar();
//            //myConnection.Close();
//            koneksi.closeConnection();

//            if (shiftakhir != null )//shift.ToString().Length > 0) 
//            { shift = 1; }
//            else
//            { shift = 2; }

//                //int invoiceshift = reader.GetInt32(0);
//                //MessageBox.Show(shift.ToString());
//                if (shift < 2)
//                {
//                    shift = Convert.ToInt32(btnTutupShift.Text.Substring(btnTutupShift.Text.IndexOf("(") + 1, btnTutupShift.Text.IndexOf(")") - btnTutupShift.Text.IndexOf("(") - 1));//, btn.Text.IndexOf("/")).ToString();
//                    //MessageBox.Show(shift.ToString());
//                    //shift = shift + 1;
//                    btnTutupShift.Text = btnTutupShift.Text.Replace(shift.ToString(), (shift + 1).ToString());

//                    sql = new SqlCommand(
//                    (@"select 
//                    count(*) invoice
//                    ,convert(int, isnull(sum(i.harga),0)) pendapatan
//                    from Invoice i
//                    where 
//                    convert(varchar(10),getdate(),120) = convert(varchar(10),waktu_order,120)"), koneksi.KoneksiDB());
//                    reader = sql.ExecuteReader();
//                    reader.Read();

//                    int invoiceshift = reader.GetInt32(0);
//                    //MessageBox.Show(invoiceshift);

//                    int pendapatanshift = reader.GetInt32(1);
//                    koneksi.closeConnection();


//                    try
//                    {
//                        sql = new SqlCommand(@"insert into ShiftReport(waktu,shift,invoice,pendapatan) values (getdate(),@a,@b,@c)", koneksi.KoneksiDB());
//                        sql.Parameters.AddWithValue("@a", shift.ToString());
//                        sql.Parameters.AddWithValue("@b", invoiceshift.ToString());
//                        sql.Parameters.AddWithValue("@c", pendapatanshift.ToString());
//                        sql.ExecuteNonQuery();
//                        koneksi.closeConnection();
//                    }
//                    catch { MessageBox.Show("Error"); }
//                }
//                else MessageBox.Show("Hanya terdapat maksimum dua kali perubahan shift dalam satu hari.");

//                if (btnTutupShift.Text.IndexOf("2") == 0) { btnLapPendapatan.Enabled = false; }
//        }

    }
}

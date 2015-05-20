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
using System.IO;
using System.Threading;


namespace Sistem_Booking_Hotel
{
    public partial class Login : Form
    {
        public static int idS = 0;
        public int cekNilai;

        public Login()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;

        }

        
           
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    //empty implementation<
        //}

        private void Login_Load(object sender, EventArgs e)
        {
           // Login.back .FromArgb(100, 88, 44, 55);
            inputIdentitas.Focus();
            inputIdentitas.Select(inputIdentitas.Text.Length, 0);
            hakcipta.Text = "Gelato Paradise Workstation © "+ DateTime.Now.Year.ToString() +" PT. Indo Surya Asia";
            //auto_login(sender, e);
        
        }


        private void auto_login(object sender, EventArgs e)
        {
            inputIdentitas.Text = "admin";
            inputSandi.Text = "admin456";
            btnLogin_Click(sender, e);
            
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            validate();

        }

        private void inputSandi_Enter(object sender, EventArgs e)
        {
            //validate();
        }

        //private void validate()
        //{
        //    if ((inputIdentitas.Text == inputSandi.Text) && ((inputIdentitas.Text == "admin") || (inputIdentitas.Text == "resepsionis")))
        //    {
        //        FormUtama FormUtama = new FormUtama();

        //        FormUtama.Show();
        //    }
        //    else
        //        MessageBox.Show("Login Gagal");
        //}
        private int fastexp(int huruf, int key, int N)
        {
            int fasexp = 0;
            int a1 = huruf;
            int z1 = key;
            int x = 1;
            while (z1 != 0)
            {
                if (z1 % 2 == 0)
                {
                    z1 = z1 / 2;
                    a1 = (a1 * a1) % N;
                }
                else
                {
                    z1 = z1 - 1;
                    x = (x * a1) % N;
                }
                fasexp = x;
            }
            return fasexp;
        }
        string[,] huruf = new string[6, 6];
        int baris = 0;
        int kolom = 0;
        string[] plaint;
        int[,] kolomPlaint = new int[999, 999];

        private int cekData(char x)
        {
            string keyEN = "INDOSYA";

            Boolean ada = false;
            for (int i = 0; i < keyEN.Length; i++)
            {
                if (keyEN.ToLower().Substring(i, 1).Equals(x.ToString())) ada = true;
            }
            if (ada == false)
            {
                return 1;
            }
            else return 0;
        }


        private string encrypt(string x)
        {
            plaint = new string[999];
            plaint = x.Split();

            huruf = new string[6, 6];
            kolomPlaint = new int[999, 999];
            baris = 0;
            kolom = 0;
            string keyEN = "INDOSYA";
            int panjang = keyEN.Length;
            for (int i = 0; i < keyEN.Length; i++)
            {
                huruf[baris, kolom] = keyEN.ToLower().Substring(i, 1);
                kolom += 1;
                if (kolom == 6)
                {
                    baris += 1;
                    kolom = 0;
                }
            }
            for (int i = 97; i < 123; i++)
            {
                char b = Convert.ToChar(i);
                if (cekData(b) == 1)
                {
                    huruf[baris, kolom] = b.ToString();
                    kolom += 1;
                    if (kolom == 6)
                    {
                        baris += 1;
                        kolom = 0;
                    }
                }
            }
            for (int i = 48; i < 58; i++)
            {
                char b = Convert.ToChar(i);
                if (cekData(b) == 1)
                {
                    huruf[baris, kolom] = b.ToString();
                    kolom += 1;
                    if (kolom == 6)
                    {
                        baris += 1;
                        kolom = 0;
                    }
                }
            }
            for (int i = 0; i < plaint.Length; i++)
            {
                for (int j = 0; j < plaint[i].Length; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        for (int l = 0; l < 6; l++)
                        {
                            if (huruf[k, l].Equals(plaint[i].ToLower().Substring(j, 1)))
                            {
                                kolomPlaint[i, j] = k;
                                kolomPlaint[i, j + plaint[i].Length] = l;
                            }
                        }
                    }
                }
            }
            string gabung = "";
            for (int i = 0; i < plaint.Length; i++)
            {
                for (int j = 0; j < plaint[i].Length * 2; j += 2)
                {
                    gabung = gabung + huruf[kolomPlaint[i, j], kolomPlaint[i, j + 1]];
                }
            }
            return gabung;
        }

        private bool validasiSerial(string x, string namaH, string alamH)
        {

            bool statusserial = false;
            try
            {
                double privateData = 23;
                decimal modulo = 187;
                string namaPer = x.Substring(0, 16);
                string alama = x.Substring(16, 12);

                alamH = alamH.Substring(0, 3) + alamH.Substring(alamH.Length - 3, 3);
                char[] perName = namaPer.ToUpper().ToCharArray();
                // char[] perlama = rangetngl.ToUpper().ToCharArray();
                char[] perala = alama.ToUpper().ToCharArray();
                int j = 0;

                //string ceklama = "";
                //for (int i = 0; i < perlama.Length; i += 2)
                //{
                //    double myNewInt = 0;
                //    //MessageBox.Show(perName[i].ToString() + perName[i + 1].ToString());
                //    if (perlama[i].ToString().Equals("0"))
                //    {
                //        myNewInt = Convert.ToDouble(Convert.ToInt32(perlama[i + 1].ToString(), 16));
                //    }
                //    else
                //    {
                //        myNewInt = Convert.ToDouble(Convert.ToInt32(perlama[i].ToString() + perlama[i + 1].ToString(), 16));
                //    }

                //    int c = fastexp(Convert.ToInt32(myNewInt), Convert.ToInt32(privateData), Convert.ToInt32(modulo));
                //    ceklama = ceklama + Convert.ToChar(Convert.ToInt32(c)).ToString();

                //}
                string cekalama = "";
                for (int i = 0; i < perala.Length; i += 2)
                {
                    double myNewInt = 0;
                    //MessageBox.Show(perName[i].ToString() + perName[i + 1].ToString());
                    if (perala[i].ToString().Equals("0"))
                    {
                        myNewInt = Convert.ToDouble(Convert.ToInt32(perala[i + 1].ToString(), 16));
                    }
                    else
                    {
                        myNewInt = Convert.ToDouble(Convert.ToInt32(perala[i].ToString() + perala[i + 1].ToString(), 16));
                    }

                    int c = fastexp(Convert.ToInt32(myNewInt), Convert.ToInt32(privateData), Convert.ToInt32(modulo));
                    cekalama = cekalama + Convert.ToChar(Convert.ToInt32(c)).ToString();

                }

                string namaPerusahaan = "";
                for (int i = 0; i < perName.Length; i += 2)
                {
                    double myNewInt = 0;
                    //MessageBox.Show(perName[i].ToString() + perName[i + 1].ToString());
                    if (perName[i].ToString().Equals("0"))
                    {
                        myNewInt = Convert.ToDouble(Convert.ToInt32(perName[i + 1].ToString(), 16));
                    }
                    else
                    {
                        myNewInt = Convert.ToDouble(Convert.ToInt32(perName[i].ToString() + perName[i + 1].ToString(), 16));
                    }

                    int c = fastexp(Convert.ToInt32(myNewInt), Convert.ToInt32(privateData), Convert.ToInt32(modulo));
                    namaPerusahaan = namaPerusahaan + Convert.ToChar(Convert.ToInt32(c)).ToString();


                }
                //string tanggalaktivasi = "";
                //for (int i = 0; i < tanggal.Length; i += 2)
                //{
                //    double myNewInt = 0;
                //    //MessageBox.Show(perName[i].ToString() + perName[i + 1].ToString());
                //    if (tanggal[i].ToString().Equals("0"))
                //    {
                //        myNewInt = Convert.ToDouble(Convert.ToInt32(tanggal[i + 1].ToString(), 16));
                //    }
                //    else
                //    {
                //        myNewInt = Convert.ToDouble(Convert.ToInt32(tanggal[i].ToString() + tanggal[i + 1].ToString(), 16));
                //    }

                //    int c = fastexp(Convert.ToInt32(myNewInt), Convert.ToInt32(privateData), Convert.ToInt32(modulo));
                //    tanggalaktivasi = tanggalaktivasi + Convert.ToChar(Convert.ToInt32(c)).ToString();

                //}
                //int jumHariaktiv = (Convert.ToInt32(tanggalaktivasi.Substring(0, 2)) * 360) + Convert.ToInt32(tanggalaktivasi.Substring(2, 2)) + (Convert.ToInt32(tanggalaktivasi.Substring(4, 2)) * 30);
                //string hariini = DateTime.Now.Date.ToString("yyddMM");
                //int jumHariDeaktiv = (Convert.ToInt32(hariini.Substring(0, 2)) * 360) + Convert.ToInt32(hariini.Substring(2, 2)) + (Convert.ToInt32(hariini.Substring(4, 2)) * 30);

                //if (jumHariDeaktiv - jumHariaktiv < Convert.ToInt32(ceklama) && jumHariDeaktiv - jumHariaktiv >= 0)
                //{
                //    statusserial = true;
                //}
                int pjng = namaH.Length;
                if (pjng < 8)
                {
                    for (int kl = 0; kl < 8 - pjng; kl++)
                    {
                        namaH = namaH + " ";
                    }
                }

                namaH = Reverse(namaH).Substring(0, 8);
                namaH = encrypt(namaH);

                if (statusserial == false)
                {
                    if (namaPerusahaan.ToUpper().Equals(namaH.ToUpper()))
                    {
                        statusserial = true;
                    }
                    else
                    {
                        statusserial = false;
                    }
                }

                if (statusserial == true)
                {
                    if (alamH.ToUpper().Equals(cekalama.ToUpper()))
                    {
                        statusserial = true;
                    }
                    else
                    {
                        statusserial = false;
                    }
                }
            }
            catch
            {
                statusserial = false;
            }
            return statusserial;
         
        }

        public string Reverse(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = String.Empty;
            for (int i = cArray.Length - 1; i > -1; i--)
            {
                reverse += cArray[i];
            }

            return reverse;
        }

        private void validate()
        {
            configconn config = new configconn();
            //configconn.conn.Open();

            SqlCommand cmd = new SqlCommand("select Id_jabatan,staff_id from Staff where username=@nama and password=@pass", config.KoneksiDB());
            cmd.Parameters.AddWithValue("@nama", inputIdentitas.Text);
            cmd.Parameters.AddWithValue("@pass", inputSandi.Text);
            SqlDataReader reader;
            reader = cmd.ExecuteReader();
            cekNilai = 0;
            while (reader.Read())
            {
                cekNilai = (reader.GetInt32(0));
                idS = reader.GetInt32(1);
            }
            config.closeConnection();
            /*
            if ((inputIdentitas.Text == inputSandi.Text) && ((inputIdentitas.Text == "admin") || (inputIdentitas.Text == "resepsionis")))
            {
                FormUtama FormUtama = new FormUtama();

                FormUtama.Show();
            }
             */

            cmd = new SqlCommand("select serial,Nama_Hotel,Alamat from IDHotel", config.KoneksiDB());
            SqlDataReader readS = cmd.ExecuteReader();
            string nilaiKode = "";
            string namaHotel = "";
            string alamathotel = "";

            while (readS.Read())
            {
                nilaiKode = readS["serial"].ToString();
                namaHotel = readS["Nama_Hotel"].ToString();
                alamathotel = readS["Alamat"].ToString();
            }

            config.closeConnection();
            bool cekIndex = false;
            if (nilaiKode.Equals(""))
            {

                if (!File.Exists(@"C:\ProgramData\asscriptdb.dat"))
                {
                    string[] lines = new string[1];
                    lines[0] = DateTime.Now.Date.ToString("dd-MM-yyyy");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\ProgramData\asscriptdb.dat"))
                    {
                        foreach (string line in lines)
                        {
                            file.WriteLine(line);
                        }
                    }
                    cekIndex = true;
                }
                else
                {
                    string[] line = new string[1];
                    int ctr = 0;
                    using (StreamReader reader2 = new StreamReader(@"C:\ProgramData\asscriptdb.dat"))
                    {
                        line[ctr] = reader2.ReadLine();
                    }
                    string[] pecah = line[ctr].Split('-');
                    string[] hariini = DateTime.Now.Date.ToString("dd-MM-yyyy").Split('-');

                    int selisihM = (Int32.Parse(pecah[1]) + (Int32.Parse(pecah[0]) * 30) + (Int32.Parse(pecah[2]) * 365));
                    int selisihM2 = (Int32.Parse(hariini[1]) + (Int32.Parse(hariini[0]) * 30) + (Int32.Parse(hariini[2]) * 365));
                    int slisih = selisihM2 - selisihM;
                    if (slisih <= 30 && slisih >= 0)
                    {
                        cekIndex = true;
                    }
                    else
                    {
                        cekIndex = false;
                    }
                }
                //emergency live test, remove ASAP
                // cekIndex = true;
                //emergency live test, remove ASAP
            }
            else
            {

                cekIndex = validasiSerial(nilaiKode, namaHotel, alamathotel);
                //emergency live test, remove ASAP
                // cekIndex = true;
                //emergency live test, remove ASAP
            }

            if (cekIndex == false)
            {
                MessageBox.Show("Product Key Expired");
            }
            else
            {
                if (cekNilai > 0 && cekIndex == true)
                //if (cekNilai > 0)
                {
                    //string admin = "staff";

                    //FormUtama FormUtama = new FormUtama();
                    //FormUtama.getAdmin = "staff";
                    //FormUtama.Name = "hotel";
                    //FormUtama.ShowDialog();

                    Workstation Workstation = new Workstation();
                    //FormUtama.getAdmin = "staff";
                    Workstation.Name = "workstation";
                    Workstation.ShowDialog();

                    inputIdentitas.Text = "";
                    inputSandi.Text = "";
                    inputIdentitas.Focus();

                    // FormCollection fc = Application.OpenForms;
                    //// ProgressIndicator pi = new ProgressIndicator();
                    // pi.Show();
                    // pi.BringToFront();
                    // bool cekD = false;
                    // foreach (Form frm in fc)
                    // {
                    //     if (frm.Name.Equals("hotel"))
                    //     {
                    //         //cek
                    //         cekD = true;
                    //     }
                    // }
                    // if (cekD)
                    // {
                    //  //   pi.Close();
                    // }

                    //this.Hide();

                }

                    //else if (cekNilai == 3)
                //{

                    //    //AdminForm administratorForm = new AdminForm();
                //    //administratorForm.Show();
                //    //string admin = "admin";
                //    FormUtama FormUtama = new FormUtama();
                //    FormUtama.getAdmin = "admin";
                //    Button btnPengaturanKamar = ((Button)FormUtama.Controls.Find("btnPengaturanKamar", true)[0]);
                //    //if (removeaddBooking != null) panelKamarDibooking.Controls.Remove(removeaddBooking);
                //    //FormUtama.Controls.btn
                //    FormUtama.Show();

                    //    this.Hide();
                //    //Form2 newForm = new Form2();
                //    //newForm.TheValue = value;
                //    //newForm.ShowDialog();
                //    /*
                //    cobaForm coba = new cobaForm();
                //    coba.Show();
                //    this.Hide();
                //     */
                //}
                else
                MessageBox.Show("Login Gagal");
                

            }
        }

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) validate();
        }

        private void keluarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnConn_Click(object sender, EventArgs e)
        {
            DatabaseConnectionForm databaseConnection = new DatabaseConnectionForm();
            databaseConnection.Show();
            //MessageBox.Show(ConfigurationManager.ConnectionStrings["Sistem_Booking_Hotel.Properties.Settings.tabHotelConnectionString"].ToString() + ".This is your connection");
        }

        private void btnInputSerialNumber_Click(object sender, EventArgs e)
        {
            FormSerial fs = new FormSerial();
            fs.Show();
            fs.BringToFront();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

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

namespace Sistem_Booking_Hotel
{
    public partial class DataKamar : Form
    {
        SqlCommand cmd;
        SqlDataReader reader;

        Label[] JKamar;
        int jumKamar;

        private readonly FormUtama formU;

        public DataKamar(FormUtama f)
        {
            formU = f;
            InitializeComponent();
        }

        int x = 0;
        configconn koneksi = new configconn();
        
        public void refresh_kamar()
        {
            //configconn.conn.Open();
            SqlCommand cmd = new SqlCommand((@"select count(*) from Kamar"), koneksi.KoneksiDB());
            int jumKamar = (int)cmd.ExecuteScalar();

            ///button1.Text = jumKamar.ToString();
            Button[] Kamar;
            cmd = new SqlCommand(@"SELECT k.kamar_no,kt.kamar_tipe,kt.warna
                                    FROM Kamar k inner join 
                                    Kamar_Tipe kt on 
                                    k.kamar_tipe_id = kt.kamar_tipe_id", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            Kamar = new Button[jumKamar+1];
            x = 0;
            //FormUtama formutama = new FormUtama();
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
                try
                {
                    Kamar[x].BackColor = Color.FromArgb(Int32.Parse(reader.GetString(2)));
                }
                catch { 
                    Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                }
                //Kamar[x].BackColor = Color.FromName(reader.GetString(2));
                Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
                Kamar[x].ImageAlign = ContentAlignment.MiddleLeft;//formutama.btnboo btnBooking.ImageAlign;
                Kamar[x].Click += new EventHandler(load_Ubah_Hapus);
                //Kamar[x].MouseEnter += new EventHandler(button1_MouseEnter_2);
                //Kamar[x].MouseLeave += new EventHandler(button1_MouseLeave_1);

                flowLayoutPanel1.Controls.Add(Kamar[x]);
                x += 1;
                //MessageBox.Show("ok");
                //Kamar[x].MouseEnter += button1_MouseEnter_2;// Kamar_Tips;//new EventHandler(Kamar_Tips);

            }
            Kamar[x] = new Button();
            //Kamar[x].Text = reader.GetInt32(0).ToString() + "\n\r" + reader.GetString(1);
            //Kamar[x].Name = reader.GetInt32(0).ToString();
            Kamar[x].Text = "+";
            Kamar[x].Visible = true;
            //Kamar[x].Height = 35;
            //Kamar[x].Tag = reader.GetDouble(2).ToString();
            Kamar[x].BackColor = Color.Aqua;
            Kamar[x].Height = 45;
            Kamar[x].Width = 95;
            Kamar[x].FlatStyle = FlatStyle.Flat;
            //Kamar[x].BackColor = Color.FromName(reader.GetString(1));
            //Kamar[x].Image = Sistem_Booking_Hotel.Properties.Resources.room;
            //Kamar[x].ImageAlign = ContentAlignment.MiddleLeft;//formutama.btnboo btnBooking.ImageAlign;
                
            Kamar[x].Click += new EventHandler(tambah_Kamar);
            flowLayoutPanel1.Controls.Add(Kamar[x]);
            //conn.Close();
            koneksi.KoneksiDB().Close();
        }
        //kamar_Ubah_Hapus loadUbahHapus = new kamar_Ubah_Hapus();
        protected void load_Ubah_Hapus(object sender, EventArgs e)
        {
            kamar_Ubah_Hapus loadUbahHapus = new kamar_Ubah_Hapus(formU);
            Button btn = sender as Button;
            loadUbahHapus.passNoKamar(Convert.ToInt32(btn.Text));
            loadUbahHapus.ShowDialog();
            loadUbahHapus.Location = new Point(btn.Location.X + (btn.Width) + loadUbahHapus.Width, btn.Location.Y);
        }

        protected void tambah_Kamar(object sender, EventArgs e)
        {
            TambahKamar tambahKamar = new TambahKamar(formU);
            tambahKamar.ShowDialog();
        }

        protected void button1_MouseEnter_2(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            //toolTip1.Hide(btn);
        }

        protected void button1_MouseLeave_1(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            //toolTip1.Hide(btn);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            TambahKamar tambahKamar = new TambahKamar(formU);
            tambahKamar.ShowDialog();
        }

        private void DataKamar_Load(object sender, EventArgs e)
        {
            refresh_kamar();
            /*
            configconn.conn.Open();
            cmd = new SqlCommand("select count(*) from Kamar", configconn.conn);
            jumKamar = (int)cmd.ExecuteScalar();

            cmd = new SqlCommand("select kamar_no from Kamar", configconn.conn);
            reader = cmd.ExecuteReader();
            int panjangLbl = (panel1.Width / 5) - 20;
            int lebarLbl = panjangLbl - ((panjangLbl * 2) / 3);
            JKamar = new Label[jumKamar];
            int ctr = 0; int posX = 0; int posY = 0;
            while (reader.Read())
            {
                JKamar[ctr] = new Label();
                JKamar[ctr].Text = reader.GetInt32(0).ToString();
                JKamar[ctr].Name = reader.GetInt32(0).ToString();

                JKamar[ctr].Size = new Size(panjangLbl, lebarLbl);
                JKamar[ctr].Font = new Font("Arial", 16);
                JKamar[ctr].ImageAlign = ContentAlignment.MiddleCenter;

                JKamar[ctr].Location = new Point((panjangLbl * posX) + (20 * posX), (posY * lebarLbl) + (20 * posY));
                JKamar[ctr].BackColor = Color.Red;
                ctr += 1; posX += 1;
                if (posX > 4)
                {
                    posX = 0; posY += 1;
                }
            }
            //button tambah
            JKamar[ctr] = new Label();
            JKamar[ctr].Size = new Size(panjangLbl, lebarLbl);
            JKamar[ctr].Font = new Font("Arial", 16);
            JKamar[ctr].ImageAlign = ContentAlignment.MiddleCenter;
            JKamar[ctr].Text = "Tambah";
            JKamar[ctr].Location = new Point((panjangLbl * posX) + (20 * posX), (posY * lebarLbl) + (20 * posY));
            JKamar[ctr].BackColor = Color.Red;

            configconn.conn.Close();

            for (int i = 0; i < jumKamar; i++)
            {
                configconn.conn.Close();
                configconn.conn.Open();
                cmd = new SqlCommand("select kt.kamar_tipe from Kamar_Tipe kt, Kamar k where kt.kamar_tipe_id = k.kamar_tipe_id and k.kamar_no=@nokamar", configconn.conn);
                cmd.Parameters.AddWithValue("@nokamar", JKamar[i].Text);
                SqlDataReader reader2 = cmd.ExecuteReader();
                while (reader2.Read())
                {
                    if (reader2.GetString(0).Equals("Hijau"))
                    {
                        JKamar[i].BackColor = Color.Green;
                    }
                    else if (reader2.GetString(0).Equals("Biru"))
                    {
                        JKamar[i].BackColor = Color.Blue;
                    }
                    else if (reader2.GetString(0).Equals("Kuning"))
                    {
                        JKamar[i].BackColor = Color.Yellow;
                    }
                    else if (reader2.GetString(0).Equals("Merah"))
                    {
                        JKamar[i].BackColor = Color.Red;
                    }
                    else if (reader2.GetString(0).Equals("Gold"))
                    {
                        JKamar[i].BackColor = Color.Gold;
                    }
                    else
                    {
                        JKamar[i].BackColor = Color.Silver;
                    }
                    JKamar[i].MouseEnter += new EventHandler(JKenter);
                    JKamar[i].MouseLeave += new EventHandler(JKLeave);
                    //JKamar[i].Click += new EventHandler(JKClick);
                    panel2.Controls.Add(JKamar[i]);
                }
                configconn.conn.Close();
            }*/
            //cekKamar();
        }

        //Form4 form3 = new Form4();
        //protected void JKenter(object sender, EventArgs e)
        //{
        //    Label lbl = sender as Label;
        //    form3.setLabel(lbl.Text);
        //    form3.Show();
        //    form3.Location = new Point(lbl.Location.X + (lbl.Width) + form3.Width, lbl.Location.Y);
        //}

        //protected void JKLeave(object sender, EventArgs e)
        //{
        //    Label lbl = sender as Label;
        //    form3.Hide();
        //}

        private int nomorKamar;
        private Color[] warnaKamar;
        int count=0;
        private bool[] cekHapusKamar;
        protected void JKClick(object sender, EventArgs e)
        {
            Label lbl = sender as Label;

            if (lbl.BackColor == Color.Gray)
            {
                for (int a = 0; a < count; a++)
                {
                    lbl.BackColor = warnaKamar[a];
                }
            }

            else 
            {
                warnaKamar[count] = lbl.BackColor;
                lbl.BackColor = Color.Gray;
                nomorKamar = Convert.ToInt32(lbl.Text);
                count++;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            HapusKamar hapusKamar = new HapusKamar();
            hapusKamar.ShowDialog();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            UbahKamar ubahKamar = new UbahKamar(formU);
            ubahKamar.ShowDialog();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            TambahKamar tambahKamar = new TambahKamar(formU);
            tambahKamar.ShowDialog();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            UbahKamar ubahKamar = new UbahKamar(formU);
            ubahKamar.ShowDialog();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            HapusKamar hapusKamar = new HapusKamar();
            hapusKamar.ShowDialog();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            //refresh_kamar();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel1_Click(object sender, EventArgs e)
        {
            //((FormUtama)this.Owner).hilangkan();
            formU.hilangkan();
        }

        private void DataKamar_MouseEnter(object sender, EventArgs e)
        {
            //kamar_Ubah_Hapus.ActiveForm.Close();
        }

        private void DataKamar_MouseClick(object sender, MouseEventArgs e)
        {
            //kamar_Ubah_Hapus.ActiveForm.Close();
        }
        
    }
}

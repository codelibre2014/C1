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

namespace Sistem_Booking_Hotel
{
    public partial class TambahKamar : Form
    {
        private readonly FormUtama formU;
        public TambahKamar(FormUtama f)
        {
            InitializeComponent();
            formU = f;
        }
        SqlCommand cmd;
        SqlDataReader reader;
        configconn koneksi = new configconn();
        //SqlDataReader reader1;

        private void button1_Click(object sender, EventArgs e)
        {
            configconn.conn.Open();
            int kamarKapasitasID;
            int kamarTipeID;
            cmd = new SqlCommand("select kk.kamar_kapasitas_id from Kamar_Kapasitas kk where kk.kamar_kapasitas ='" + comboBox2.Text + "'", configconn.conn);
            reader = cmd.ExecuteReader();
            reader.Read();
            kamarKapasitasID = reader.GetInt32(0);
            configconn.conn.Close();

            configconn.conn.Open();
            cmd = new SqlCommand("select kt.kamar_tipe_id from Kamar_Tipe kt where kt.kamar_tipe ='" + comboBox1.Text + "'", configconn.conn);
            reader = cmd.ExecuteReader();
            reader.Read();
            kamarTipeID = reader.GetInt32(0);
            configconn.conn.Close();

            /*
            cmd = new SqlCommand("insert into Kamar(kamar_no, kamar_tipe_id, kamar_kapasitas_id) values(@param1, @param2, @param3)", Form3.conn);
            
            Form3.conn.Open();
            cmd.ExecuteNonQuery();
            Form3.conn.Close();*/

            using (SqlCommand dataCommand = configconn.conn.CreateCommand())
            {
                configconn.conn.Open();
                dataCommand.CommandText = "INSERT INTO Kamar (kamar_no, kamar_tipe_id, kamar_kapasitas_id, smoking) values(@val1, @param2, @param3,@param4)";

                dataCommand.Parameters.AddWithValue("@val1", Convert.ToInt32(textBox1.Text));
                dataCommand.Parameters.AddWithValue("@param2", kamarTipeID);
                dataCommand.Parameters.AddWithValue("@param3", kamarKapasitasID);
                if (chSmoke.Checked)
                {
                    dataCommand.Parameters.AddWithValue("@param4", 1);
                }
                else
                {
                    dataCommand.Parameters.AddWithValue("@param4", 0);
                }

                try
                {
                    dataCommand.ExecuteNonQuery();
                    configconn.conn.Close();
                    //MessageBox.Show("kamar " + textBox1.Text + " telah ditambahkan!");
                    this.Close();                    
                }
                catch
                {
                    MessageBox.Show("kamar " + textBox1.Text + " tidak valid!");
                }
                
            }

            formU.refreshPengaturanKamar();
        }

        private void TambahKamar_Load(object sender, EventArgs e)
        {
            cmd = new SqlCommand("select kamar_tipe from Kamar_Tipe", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader.GetString(0));
            }
            reader.Close();
            cmd = new SqlCommand("select kamar_kapasitas from Kamar_Kapasitas", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                comboBox2.Items.Add(reader.GetString(0));
            }
            //configconn.conn.Close();
            koneksi.KoneksiDB().Close();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            string tipeKamar = Interaction.InputBox("Masukkan TipeKamar  =");
            string warnaKamar;// = Interaction.InputBox("Masukkan WarnaKamar  =");

            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                warnaKamar = colorDialog1.Color.ToArgb().ToString();
                //tipeKamar = Interaction.InputBox("Masukkan TipeKamar  =");
                formU.Focus();
                if (tipeKamar.Equals("")) { }
                else
                {
                    try
                    {
                        SqlCommand sql = new SqlCommand("insert into Kamar_Tipe(kamar_tipe,warna) values (@a,@b)", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", tipeKamar);
                        sql.Parameters.AddWithValue("@b", warnaKamar);
                        sql.ExecuteNonQuery();
                        comboBox1.Items.Add(tipeKamar);
                        koneksi.KoneksiDB().Close();

                        sql = new SqlCommand("select max(kamar_tipe_id) from Kamar_Tipe", koneksi.KoneksiDB());
                        int kodeHarga = Int32.Parse(sql.ExecuteScalar().ToString());
                        koneksi.KoneksiDB().Close();

                        sql = new SqlCommand("insert into Harga_Periodik(kamar_tipe_id, tgl_berlaku,harga,harga_weekend) values(@a,'7/1/2008',200000,200000)", koneksi.KoneksiDB());
                        sql.Parameters.AddWithValue("@a", kodeHarga);
                        sql.ExecuteNonQuery();
                        koneksi.KoneksiDB().Close();

                        sql = new SqlCommand("exec [dbo].[sp_generate_harga]", koneksi.KoneksiDB());
                        sql.ExecuteNonQuery();
                        koneksi.KoneksiDB().Close();

                    }
                    catch
                    {

                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string tipeKamar = Interaction.InputBox("Jenis Tipe Kapasitas =");
            //tipeKamar = Interaction.InputBox("Jenis Tipe Kapasitas =");
            string jumKamar = Interaction.InputBox("Kapasitas Kamar =");
            if(tipeKamar.Equals("") ||  jumKamar.Equals("")){}
            else{
                SqlCommand sql = new SqlCommand("insert into Kamar_Kapasitas(kamar_kapasitas, jumlah_tamu) values (@a,@b)", koneksi.KoneksiDB());
                sql.Parameters.AddWithValue("@a", tipeKamar);
                sql.Parameters.AddWithValue("@b", jumKamar);
                sql.ExecuteNonQuery();
                comboBox2.Items.Add(tipeKamar);
                koneksi.KoneksiDB().Close();
            }
            formU.Focus();
        }

        private void TambahKamar_Deactivate(object sender, EventArgs e)
        {
            //TambahKamar.ActiveForm.Close();
        }
    }
}

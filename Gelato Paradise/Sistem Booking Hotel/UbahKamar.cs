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
    public partial class UbahKamar : Form
    {
        private readonly FormUtama formU;
        public UbahKamar(FormUtama f)
        {
            formU = f;
            InitializeComponent();
        }

        SqlCommand cmd;
        SqlDataReader reader;
        configconn koneksi = new configconn();
        
        int idKamar;
        public void idNoKamar(int noKamar)
        {
            //configconn.conn.Open();
            idKamar = noKamar;
            cmd = new SqlCommand("select kamar_no, kamar_tipe, kamar_kapasitas, jumlah_tamu,smoking from Kamar km, Kamar_Kapasitas kk, Kamar_Tipe kt where km.kamar_no = " + idKamar + " and km.kamar_tipe_id = kt.kamar_tipe_id and km.kamar_kapasitas_id = kk.kamar_kapasitas_id", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            reader.Read();
            textBox1.Text = idKamar.ToString();
            comboBox1.Text = reader.GetString(1);
            textBox2.Text = idKamar.ToString();
            comboBox2.Text = reader.GetString(2);
            if (reader["smoking"].ToString().Equals("1"))
            {
                cheditsmoke.Checked = true;
            }
            else
            {
                cheditsmoke.Checked = false;
            }
            //configconn.conn.Close();
            koneksi.KoneksiDB().Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //configconn.conn.Open();
            int kamarKapasitasID;
            int kamarTipeID;
            cmd = new SqlCommand("select kk.kamar_kapasitas_id from Kamar_Kapasitas kk where kk.kamar_kapasitas ='" + comboBox2.Text + "'", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            reader.Read();
            kamarKapasitasID = reader.GetInt32(0);
            //configconn.conn.Close();
            koneksi.KoneksiDB().Close();
            //configconn.conn.Open();
            cmd = new SqlCommand("select kt.kamar_tipe_id from Kamar_Tipe kt where kt.kamar_tipe ='" + comboBox1.Text + "'", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            reader.Read();
            kamarTipeID = reader.GetInt32(0);
            koneksi.KoneksiDB().Close();

            using (SqlCommand dataCommand = koneksi.KoneksiDB().CreateCommand())
            {
                //configconn.conn.Open();
                string nilaismoke = "1";
                if (cheditsmoke.Checked)
                {
                    nilaismoke = "1";
                }
                else
                {
                    nilaismoke = "0";
                }
                dataCommand.CommandText = "update Kamar set kamar_no = " + Convert.ToInt32(textBox2.Text) +
                                                         ", kamar_tipe_id = " + kamarTipeID +
                                                         ", kamar_kapasitas_id = " + kamarKapasitasID +
                                                         ", smoking ="+nilaismoke+
                                                      " where kamar_no = " + Convert.ToInt32(textBox1.Text) + ";";

                //dataCommand.Parameters.AddWithValue("@val1", Convert.ToInt32(textBox1.Text));
                //dataCommand.Parameters.AddWithValue("@param2", kamarTipeID);
                //dataCommand.Parameters.AddWithValue("@param3", kamarKapasitasID);


                dataCommand.ExecuteNonQuery();
                //configconn.conn.Close();
                koneksi.KoneksiDB().Close();
                MessageBox.Show("kamar " + textBox1.Text + " telah diubah");
                this.Close();
            }
            formU.refreshPengaturanKamar();
            this.Close();
        }

        private void UbahKamar_Load(object sender, EventArgs e)
        {
            //configconn.conn.Open();

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

        private void UbahKamar_Deactivate(object sender, EventArgs e)
        {
            //UbahKamar.ActiveForm.Close();
        }

        private void UbahKamar_Activated(object sender, EventArgs e)
        {
            //UbahKamar.ActiveForm UbahKamar_Deactivate
            
        }

        private void UbahKamar_MouseEnter(object sender, EventArgs e)
        {
            //UbahKamar.ActiveForm.Deactivate += new EventHandler(UbahKamar_Deactivate);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

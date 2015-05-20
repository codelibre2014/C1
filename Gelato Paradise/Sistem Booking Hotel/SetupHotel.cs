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
    public partial class SetupHotel : Form
    {
        public SetupHotel()
        {
            InitializeComponent();
        }

        String nama_hotel = "";
        configconn koneksi = new configconn();
        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlCommand dataCommand = koneksi.KoneksiDB().CreateCommand())
            {
                //koneksi.KoneksiDB().Open();

                dataCommand.CommandText = "update IDHotel set Nama_Hotel = '" + textBox1.Text +
                                                         "', Alamat = '" + richTextBox1.Text +
                                                         "', Telepon = '" + textBox2.Text +
                                                         "', Kota = '" + textBox3.Text +
                                                      "' where Nama_Hotel = '" + nama_hotel + "';";

                //dataCommand.Parameters.AddWithValue("@val1", Convert.ToInt32(textBox1.Text));
                //dataCommand.Parameters.AddWithValue("@param2", kamarTipeID);
                //dataCommand.Parameters.AddWithValue("@param3", kamarKapasitasID);


                dataCommand.ExecuteNonQuery();
                koneksi.KoneksiDB().Close();
                MessageBox.Show("Data ID Hotel telah diubah");
                this.Close();
            }
        }

        SqlCommand cmd;
        SqlDataReader reader;
        private void SetupHotel_Load(object sender, EventArgs e)
        {
            //koneksi.KoneksiDB().Open();

            cmd = new SqlCommand("select * from IDHotel", koneksi.KoneksiDB());
            reader = cmd.ExecuteReader();
            reader.Read();
            textBox1.Text = reader.GetString(0);
            nama_hotel = reader.GetString(0);
            richTextBox1.Text = reader.GetString(1);
            textBox2.Text = reader.GetString(2);
            textBox3.Text = reader.GetString(3);
            koneksi.KoneksiDB().Close();
        }
    }
}

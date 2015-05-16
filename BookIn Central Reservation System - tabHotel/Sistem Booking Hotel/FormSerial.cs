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
    public partial class FormSerial : Form
    {
        public FormSerial()
        {
            InitializeComponent();
        }

        private void btnConn_Click(object sender, EventArgs e)
        {
            configconn config = new configconn();
            //config.KoneksiDB();
            SqlCommand cmd = new SqlCommand((@"update IDHotel set Nama_Hotel = '" + textBox1.Text +
                                                         "', Alamat = '" + textBox2.Text +
                                                         "', Telepon = '" + textBox3.Text +
                                                         "', Kota = '" + textBox4.Text +
                                                         "', Jam_checkout = '" + textBox5.Text +
                                                      "', serial = '" + textBox6.Text + textBox7.Text + textBox8.Text + "';"), config.KoneksiDB());
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data ID Hotel telah diubah");
            }
            catch
            {
                MessageBox.Show("Input data tidak valid!");
            }

            config.KoneksiDB().Close();
            this.Close();
        
        }

        private void keluarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

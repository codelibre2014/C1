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
    public partial class HapusKamar : Form
    {
        public HapusKamar()
        {
            InitializeComponent();
        }

        SqlCommand cmd;
        private void button1_Click(object sender, EventArgs e)
        {
            configconn.conn.Open();
            cmd = new SqlCommand("delete from Kamar where kamar_no = @paramkamar", configconn.conn);
            cmd.Parameters.AddWithValue("@paramkamar", Convert.ToInt32(textBox1.Text));
            cmd.ExecuteNonQuery();
            configconn.conn.Close();
            MessageBox.Show("kamar " + textBox1.Text + " telah terhapus");
        }

        private void HapusKamar_Load(object sender, EventArgs e)
        {

        }
    }
}

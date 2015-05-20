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
    public partial class kamar_Ubah_Hapus : Form
    {
        private readonly FormUtama formU;

        public kamar_Ubah_Hapus(FormUtama f)
        {
            formU = f;
            InitializeComponent();
        }
        int nomorKamarX;
        public void passNoKamar(int noKamar)
        {
            nomorKamarX = noKamar;
        }

        private void kamar_Ubah_Hapus_Load(object sender, EventArgs e)
        {

        }

        private void btn_Kamar_Ubah_Click(object sender, EventArgs e)
        {
            UbahKamar ubahKamar = new UbahKamar(formU);
            ubahKamar.idNoKamar(nomorKamarX);
            //ubahKamar.StartPosition = FormStartPosition.Manual;
            //ubahKamar.Location.X = MousePosition.X -10;

            ubahKamar.ShowDialog();
            ubahKamar.Focus();
            this.Close();
        }

        SqlCommand cmd;
        private void btn_Kamar_Hapus_Click(object sender, EventArgs e)
        {
            //HapusKamar hapusKamar = new HapusKamar();
            //hapusKamar.Show();
            this.Close();
            configconn.conn.Open();
            cmd = new SqlCommand("update Kamar set status = 2 where kamar_no = @paramkamar", configconn.conn);
            cmd.Parameters.AddWithValue("@paramkamar", nomorKamarX);
            cmd.ExecuteNonQuery();
            configconn.conn.Close();
            MessageBox.Show("kamar " + nomorKamarX.ToString() + " sedang maintenance");

            formU.refreshPengaturanKamar();
        }

        private void kamar_Ubah_Hapus_Leave(object sender, EventArgs e)
        {
            //kamar_Ubah_Hapus.ActiveForm.Close();
        }

        private void kamar_Ubah_Hapus_MouseLeave(object sender, EventArgs e)
        {
            //kamar_Ubah_Hapus.ActiveForm.Close();
        }

        private void kamar_Ubah_Hapus_Deactivate(object sender, EventArgs e)
        {
            //kamar_Ubah_Hapus.ActiveForm.Close();
        }

        private void hapuskamar_Click(object sender, EventArgs e)
        {
            this.Close();
            configconn.conn.Open();
            cmd = new SqlCommand("delete from kamar where kamar_no = @paramkamar", configconn.conn);
            cmd.Parameters.AddWithValue("@paramkamar", nomorKamarX);
            cmd.ExecuteNonQuery();
            configconn.conn.Close();
            MessageBox.Show("kamar " + nomorKamarX.ToString() + " telah dihapus!");

            formU.refreshPengaturanKamar();
        }
    }
}

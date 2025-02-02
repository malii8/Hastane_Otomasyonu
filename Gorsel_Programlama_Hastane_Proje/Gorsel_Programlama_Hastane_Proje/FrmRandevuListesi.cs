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

namespace Gorsel_Programlama_Hastane_Proje
{
    public partial class FrmRandevuListesi : Form
    {
        public FrmRandevuListesi()
        {
            InitializeComponent();
        }

        sqlbaglantisi bgl = new sqlbaglantisi();

        private void FrmRandevuListesi_Load(object sender, EventArgs e)
        {
            RandevuListele();
            BranslariGetir();
        }

        private void RandevuListele()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Randevular", bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            bgl.baglanti().Close();
        }

        private void BranslariGetir()
        {
            SqlCommand komut = new SqlCommand("Select BransAd From Tbl_Branslar", bgl.baglanti());
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                CmbBrans.Items.Add(dr[0].ToString());
            }
            bgl.baglanti().Close();
        }

        private void CmbBrans_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            CmbDoktor.Items.Clear();
            SqlCommand komut = new SqlCommand("Select DoktorAd + ' ' + DoktorSoyad From Tbl_Doktorlar Where DoktorBrans=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", CmbBrans.Text);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                CmbDoktor.Items.Add(dr[0].ToString());
            }
            bgl.baglanti().Close();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            Txtid.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            MskTarih.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            MskSaat.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            CmbBrans.Text = dataGridView1.Rows[secilen].Cells[3].Value.ToString();
            CmbDoktor.Text = dataGridView1.Rows[secilen].Cells[4].Value.ToString();
            ChkDurum.Checked = Convert.ToBoolean(dataGridView1.Rows[secilen].Cells[5].Value);
            MskTC.Text = dataGridView1.Rows[secilen].Cells[6].Value.ToString();
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            SqlCommand komut = new SqlCommand("Update Tbl_Randevular Set RandevuTarih=@p1, RandevuSaat=@p2, RandevuBrans=@p3, RandevuDoktor=@p4, RandevuDurum=@p5, HastaTC=@p6 Where RandevuID=@p7", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", MskTarih.Text);
            komut.Parameters.AddWithValue("@p2", MskSaat.Text);
            komut.Parameters.AddWithValue("@p3", CmbBrans.Text);
            komut.Parameters.AddWithValue("@p4", CmbDoktor.Text);
            komut.Parameters.AddWithValue("@p5", ChkDurum.Checked);
            komut.Parameters.AddWithValue("@p6", MskTC.Text);
            komut.Parameters.AddWithValue("@p7", Txtid.Text);
            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Randevu başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearInputs();
            RandevuListele();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txtid.Text))
            {
                MessageBox.Show("Silmek için bir kayıt seçiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SqlCommand komut = new SqlCommand("Delete From Tbl_Randevular Where RandevuID=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", Txtid.Text);
            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Randevu başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearInputs();
            RandevuListele();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(MskTarih.Text) || string.IsNullOrWhiteSpace(MskSaat.Text) ||
                string.IsNullOrWhiteSpace(CmbBrans.Text) || string.IsNullOrWhiteSpace(CmbDoktor.Text) ||
                string.IsNullOrWhiteSpace(MskTC.Text))
            {
                MessageBox.Show("Tüm alanları doldurunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (MskTC.Text.Length != 11 || !long.TryParse(MskTC.Text, out _))
            {
                MessageBox.Show("TC Kimlik numarası 11 haneli olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool IsValidDate(string date)
        {
            DateTime tempDate;
            return DateTime.TryParseExact(date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out tempDate);
        }

        private bool IsValidTime(string time)
        {
            DateTime tempTime;
            return DateTime.TryParseExact(time, "HH:mm", null, System.Globalization.DateTimeStyles.None, out tempTime);
        }

        private void ClearInputs()
        {
            Txtid.Clear();
            MskTarih.Clear();
            MskSaat.Clear();
            CmbBrans.SelectedIndex = -1;
            CmbDoktor.Items.Clear();
            ChkDurum.Checked = false;
            MskTC.Clear();
        }

        private void CmbDoktor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}

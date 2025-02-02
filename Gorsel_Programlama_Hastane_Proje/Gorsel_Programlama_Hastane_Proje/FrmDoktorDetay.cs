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
    public partial class FrmDoktorDetay : Form
    {
        public FrmDoktorDetay()
        {
            InitializeComponent();
        }

        sqlbaglantisi bgl = new sqlbaglantisi();
        public string TC;

        private void FrmDoktorDetay_Load(object sender, EventArgs e)
        {
            LblTC.Text = TC;

            // Doktor Ad Soyad
            SqlCommand komut = new SqlCommand("select DoktorAd, DoktorSoyad from Tbl_Doktorlar where DoktorTC=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", LblTC.Text);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                LblAdSoyad.Text = dr[0] + " " + dr[1];
            }
            bgl.baglanti().Close();

            // Randevuları Listeleme
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Randevular Where RandevuDoktor='" + LblAdSoyad.Text + "'", bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            FrmDoktorBilgiDuzenle fr = new FrmDoktorBilgiDuzenle();
            fr.TCNO = LblTC.Text;
            fr.Show();
        }

        private void BtnDuyurular_Click(object sender, EventArgs e)
        {
            FrmDuyurular fr = new FrmDuyurular();
            fr.Show();
        }

        private void BtnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            RchSikayet.Text = dataGridView1.Rows[secilen].Cells[7].Value.ToString();

            // Randevu Tanı ve Reçete bilgilerini doldurma
            TxtTani.Text = dataGridView1.Rows[secilen].Cells[8]?.Value?.ToString();
            TxtRecete.Text = dataGridView1.Rows[secilen].Cells[9]?.Value?.ToString();
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTani.Text) || string.IsNullOrWhiteSpace(TxtRecete.Text))
            {
                MessageBox.Show("Tanı ve reçete bilgileri boş bırakılamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Seçilen randevu ID
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            string randevuID = dataGridView1.Rows[secilen].Cells[0].Value.ToString();

            SqlCommand komut = new SqlCommand("Update Tbl_Randevular Set HastaTanı=@p1, HastaRecete=@p2 Where RandevuID=@p3", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", TxtTani.Text);
            komut.Parameters.AddWithValue("@p2", TxtRecete.Text);
            komut.Parameters.AddWithValue("@p3", randevuID);
            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Tanı ve reçete bilgileri kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FrmDoktorDetay_Load(sender, e); // Listeyi güncelleme
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            // Seçilen randevu ID
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            string randevuID = dataGridView1.Rows[secilen].Cells[0].Value.ToString();

            SqlCommand komut = new SqlCommand("Update Tbl_Randevular Set HastaTani=NULL, HastaRecete=NULL Where RandevuID=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", randevuID);
            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Tanı ve reçete bilgileri silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FrmDoktorDetay_Load(sender, e); // Listeyi güncelleme
        }

        private void btnRandevuGuncelle_Click(object sender, EventArgs e)
        {
            // Gerekirse buraya yeni işlevler eklenebilir
        }
    }
}

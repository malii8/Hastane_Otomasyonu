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
    public partial class FrmSekreterDetay : Form
    {
        public FrmSekreterDetay()
        {
            InitializeComponent();
        }
        public string TCnumara;
        sqlbaglantisi bgl = new sqlbaglantisi();

        private void FrmSekreterDetay_Load(object sender, EventArgs e)
        {
            LblTC.Text = TCnumara;

            //Ad Soyad Getirme
            SqlCommand komut1 = new SqlCommand("Select SekreterAd,SekreterSoyad From Tbl_Sekreterler where SekreterTC=@p1", bgl.baglanti());
            komut1.Parameters.AddWithValue("@p1", LblTC.Text);
            SqlDataReader dr1 = komut1.ExecuteReader();
            while (dr1.Read())
            {
                LblAdSoyad.Text = dr1[0] + " " + dr1[1];
            }
            bgl.baglanti().Close();

            //Branşları Datagride Aktarma 
            DataTable dt1 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select BransAd from Tbl_Branslar", bgl.baglanti());
            da.Fill(dt1);
            dataGridView1.DataSource = dt1;

            //Doktorları Listeye Aktarma 
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter("select (DoktorAd + ' ' + DoktorSoyad) as 'Doktorlar',DoktorBrans From Tbl_Doktorlar", bgl.baglanti());
            da2.Fill(dt2);
            dataGridView2.DataSource = dt2;

            // Branşlar comboboxa aktarma
            SqlCommand komut2 = new SqlCommand("Select BransAd From Tbl_Branslar", bgl.baglanti());
            SqlDataReader dr2 = komut2.ExecuteReader();
            while (dr2.Read())
            {
                CmbBrans.Items.Add(dr2[0]);
            }
            bgl.baglanti().Close();
        }

        private void Btnkaydet_Click(object sender, EventArgs e)
        {
            // Boş alan kontrolü
            if (string.IsNullOrWhiteSpace(MskTarih.Text) || string.IsNullOrWhiteSpace(MskSaat.Text) ||
                string.IsNullOrWhiteSpace(CmbBrans.Text) || string.IsNullOrWhiteSpace(CmbDoktor.Text))
            {
                MessageBox.Show("Tüm alanların doldurulması zorunludur.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tarih ve saat format kontrolü
            DateTime tarih;
            if (!DateTime.TryParse(MskTarih.Text, out tarih))
            {
                MessageBox.Show("Geçerli bir tarih giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tarih ve saat ileri olmalı
            DateTime saat;
            if (!DateTime.TryParse(MskSaat.Text, out saat))
            {
                MessageBox.Show("Geçerli bir saat giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime randevuTarihi = tarih.Date + saat.TimeOfDay;
            if (randevuTarihi <= DateTime.Now)
            {
                MessageBox.Show("Randevu tarihi ve saati bugünden ileri bir zaman olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SqlCommand KomutKaydet = new SqlCommand("insert into Tbl_Randevular (RandevuTarih,RandevuSaat,RandevuBrans,RandevuDoktor, RandevuDurum,HastaTC) values (@r1,@r2,@r3,@r4,@r5,@r6)", bgl.baglanti());
            KomutKaydet.Parameters.AddWithValue("@r1", MskTarih.Text);
            KomutKaydet.Parameters.AddWithValue("@r2", MskSaat.Text);
            KomutKaydet.Parameters.AddWithValue("@r3", CmbBrans.Text);
            KomutKaydet.Parameters.AddWithValue("@r4", CmbDoktor.Text);
            KomutKaydet.Parameters.AddWithValue("@r5", ChkDurum.Checked);
            KomutKaydet.Parameters.AddWithValue("@r6", MskTC.Text);


            KomutKaydet.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Randevu Oluşturuldu", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Formu temizle
            MskTarih.Clear();
            MskSaat.Clear();
            CmbBrans.SelectedIndex = -1;
            CmbDoktor.Items.Clear();
        }

        private void CmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            CmbDoktor.Items.Clear();

            SqlCommand komut = new SqlCommand("Select DoktorAd, DoktorSoyad From Tbl_Doktorlar Where DoktorBrans =@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", CmbBrans.Text);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                CmbDoktor.Items.Add(dr[0] + " " + dr[1]);
            }

            bgl.baglanti().Close();
        }

        private void BtnDuyuruOlustur_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RchDuyuru.Text))
            {
                MessageBox.Show("Duyuru metni boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SqlCommand komut = new SqlCommand("insert into Tbl_Duyurular (Duyuru)  values (@d1)", bgl.baglanti());
            komut.Parameters.AddWithValue("@d1", RchDuyuru.Text);
            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Duyuru Oluşturuldu", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RchDuyuru.Clear();
        }

        private void BtnDoktorPanel_Click(object sender, EventArgs e)
        {
            FrmDoktorPaneli drp = new FrmDoktorPaneli();
            drp.Show();
        }

        private void BtnBransPanel_Click(object sender, EventArgs e)
        {
            FrmBransPaneli frb = new FrmBransPaneli();
            frb.Show();
        }

        private void BtnListe_Click(object sender, EventArgs e)
        {
            FrmRandevuListesi frl = new FrmRandevuListesi();
            frl.Show();
        }

        private void btnDuyuru_Click(object sender, EventArgs e)
        {
            FrmDuyurular frd = new FrmDuyurular();
            frd.Show();
        }

        private void ChkDurum_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

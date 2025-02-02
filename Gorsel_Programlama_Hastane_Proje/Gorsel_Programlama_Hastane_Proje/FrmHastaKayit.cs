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
using System.Text.RegularExpressions;

namespace Gorsel_Programlama_Hastane_Proje
{
    public partial class FrmHastaKayit : Form
    {
        public FrmHastaKayit()
        {
            InitializeComponent();
        }

        sqlbaglantisi bgl = new sqlbaglantisi();

        private void BtnKayitYap_Click(object sender, EventArgs e)
        {
            // Ad ve Soyad doğrulama
            if (!IsValidName(TxtAd.Text) || !IsValidName(TxtSoyad.Text))
            {
                MessageBox.Show("Ad ve Soyad sadece harflerden oluşmalıdır boş bırakılamaz ve noktalama işaretleri içermemelidir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Boş alan kontrolü
            if (string.IsNullOrWhiteSpace(MskTC.Text) || string.IsNullOrWhiteSpace(MskTelefon.Text) || string.IsNullOrWhiteSpace(TxtSifre.Text) || string.IsNullOrWhiteSpace(CmbCinsiyet.Text))
            {
                MessageBox.Show("TC, Telefon, Şifre ve Cinsiyet alanları boş bırakılamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // TC kimlik numarasını kontrol et
            SqlCommand kontrolKomut = new SqlCommand("SELECT COUNT(*) FROM Tbl_Hastalar WHERE HastaTC = @p1", bgl.baglanti());
            kontrolKomut.Parameters.AddWithValue("@p1", MskTC.Text);
            int kayitSayisi = Convert.ToInt32(kontrolKomut.ExecuteScalar());
            bgl.baglanti().Close();

            if (kayitSayisi > 0)
            {
                MessageBox.Show("Bu TC kimlik numarasıyla zaten bir kayıt mevcut.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Yeni kayıt ekleme
            SqlCommand komut = new SqlCommand("INSERT INTO Tbl_Hastalar (HastaAd, HastaSoyad, HastaTC, HastaTelefon, HastaSifre, HastaCinsiyet) VALUES (@p1, @p2, @p3, @p4, @p5, @p6)", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", TxtAd.Text);
            komut.Parameters.AddWithValue("@p2", TxtSoyad.Text);
            komut.Parameters.AddWithValue("@p3", MskTC.Text);
            komut.Parameters.AddWithValue("@p4", MskTelefon.Text);
            komut.Parameters.AddWithValue("@p5", TxtSifre.Text);
            komut.Parameters.AddWithValue("@p6", CmbCinsiyet.Text);
            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Kayıt başarılı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool IsValidName(string name)
        {
            // Sadece harflerden oluşmalı, noktalama işaretleri olmamalı
            return Regex.IsMatch(name, "^[a-zA-ZğüşöçıİĞÜŞÖÇ]+$", RegexOptions.CultureInvariant);
        }
    }
}

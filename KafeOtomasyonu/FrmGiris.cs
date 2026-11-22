using KafeOtomasyonu.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeOtomasyonu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 2. Ekrandaki Numpad butonlarına (0-9) ortak bağlanacak Click olayı
        private void NumpadTus_Click(object sender, EventArgs e)
        {
            Button basilanTus = (Button)sender;

            // Eğer şifre kutusu henüz 4 hane olmamışsa basılan rakamı ekle
            if (txtPin.Text.Length < 4)
            {
                txtPin.Text += basilanTus.Text;
            }
        }

        private void txtPin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Eğer basılan tuş rakam değilse ve Backspace (silme) tuşu değilse engelle
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (txtPin.Text.Length > 0)
            {
                txtPin.Text = txtPin.Text.Substring(0, txtPin.Text.Length - 1);
            }
        }
        private readonly VeriTabani _vt = new VeriTabani();

        private void Form1_Load(object sender, EventArgs e)
        {
            PersonelleriGetir();
        }
        private void PersonelleriGetir()
        {
            string sorgu = "SELECT PersonelID, AdSoyad FROM Personel WHERE AktifMi = 1 ORDER BY AdSoyad";
            DataTable dt = _vt.VeriGetir(sorgu);

            cmbPersonel.DataSource = dt;
            cmbPersonel.DisplayMember = "AdSoyad";
            cmbPersonel.ValueMember = "PersonelID";
            cmbPersonel.SelectedIndex = -1; // Açılışta kimse seçili gelmesin
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            if (cmbPersonel.SelectedIndex == -1 || cmbPersonel.SelectedValue == null)
            {
                MessageBox.Show("Lütfen önce personel seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPin.Text.Length != 4)
            {
                MessageBox.Show("Lütfen 4 haneli şifrenizi eksiksiz giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ComboBox SelectedValue bazen DataRowView dönebilir, güvenli dönüşüm yapalım
            if (!int.TryParse(cmbPersonel.SelectedValue.ToString(), out int seciliPersonelID))
            {
                MessageBox.Show("Personel bilgisi okunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sorgu = @"SELECT PersonelID, AdSoyad, KullaniciAdi, RolID 
                     FROM Personel 
                     WHERE PersonelID = @pid AND PinKodu = @pin AND AktifMi = 1";

            SqlParameter[] parametreler =
            {
        new SqlParameter("@pid", seciliPersonelID),
        new SqlParameter("@pin", txtPin.Text.Trim())
    };

            DataTable dt = _vt.VeriGetir(sorgu, parametreler);

            if (dt.Rows.Count > 0)
            {
                DataRow satir = dt.Rows[0];

                UserSession.PersonelID = Convert.ToInt32(satir["PersonelID"]);
                UserSession.AdSoyad = satir["AdSoyad"].ToString();
                UserSession.KullaniciAdi = satir["KullaniciAdi"].ToString();
                UserSession.RolID = Convert.ToInt32(satir["RolID"]);

                txtPin.Clear();

               
                FrmAnaMenu anaMenu = new FrmAnaMenu();
                this.Hide();
                anaMenu.ShowDialog();
                this.Show();
                PersonelleriGetir();

                MessageBox.Show($"Hoş geldiniz, {UserSession.AdSoyad}!", "Giriş Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Hatalı PIN kodu! Lütfen tekrar deneyiniz.", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPin.Clear();
            }
        }
    }
}

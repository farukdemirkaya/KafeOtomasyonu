using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KafeOtomasyonu.Core;

namespace KafeOtomasyonu
{
    public partial class FrmAnaMenu : Form
    {
        private readonly VeriTabani _vt = new VeriTabani();

        public FrmAnaMenu()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void FrmAnaMenu_Load(object sender, EventArgs e)
        {
            KullaniciBilgisiVeYetkiAyarla();
            DashboardGuncelle();
        }

        private void KullaniciBilgisiVeYetkiAyarla()
        {
            string rolMetni = UserSession.AdminMi ? "Yönetici (Admin)" : "Kasa Personeli";
            lblAktifKullanici.Text = $"Aktif Kullanıcı: {UserSession.AdSoyad} | Yetki: {rolMetni}";
            lblTarihSaat.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            // Yönetici (Admin) olmayan personeller için kısıtlamalar
            if (!UserSession.AdminMi)
            {
                btnPersonelYonetim.Enabled = false;
                btnPdfRaporlar.Enabled = false;
                // İstersen tamamen gizlemek için Visible = false da yapabilirsin:
                // btnPersonelYonetim.Visible = false;
                // btnPdfRaporlar.Visible = false;
            }
        }

        public void DashboardGuncelle()
        {
            try
            {
                // 1. GÜNÜN CİROSU (Sadece 'Tamamlandı' durumundaki bugünkü satışlar)
                string ciroSorgu = @"SELECT ISNULL(SUM(ToplamTutar), 0) 
                                     FROM Satislar 
                                     WHERE CAST(SatisTarihi AS DATE) = CAST(GETDATE() AS DATE) 
                                     AND SatisDurumu = 'Tamamlandı'";
                DataTable dtCiro = _vt.VeriGetir(ciroSorgu);
                decimal gunlukCiro = Convert.ToDecimal(dtCiro.Rows[0][0]);
                lblGunlukCiro.Text = gunlukCiro.ToString("C2");

                // 2. KRİTİK STOK DURUMU (StokMiktari <= MinStokSiniri olan aktif ürünler)
                string stokSorgu = @"SELECT UrunAdi 
                                     FROM Urunler 
                                     WHERE SatistaMi = 1 AND MinStokSiniri > 0 AND StokMiktari <= MinStokSiniri";
                DataTable dtStok = _vt.VeriGetir(stokSorgu);

                if (dtStok.Rows.Count > 0)
                {
                    lblKritikStok.Text = $"{dtStok.Rows.Count} Ürün Kritik Seviyede! ({dtStok.Rows[0]["UrunAdi"]} vb.)";
                    pnlKritikStok.BackColor = Color.Firebrick;
                    lblKritikStok.ForeColor = Color.White;
                }
                else
                {
                    lblKritikStok.Text = "Tüm Stoklar Yeterli";
                    pnlKritikStok.BackColor = Color.ForestGreen;
                    lblKritikStok.ForeColor = Color.White;
                }

                // 3. GÜNLÜK KASA KAPANIŞ (Z-RAPORU) DURUMU
                string kasaSorgu = @"SELECT COUNT(*) 
                                     FROM KasaKapanislari 
                                     WHERE KapanisGunu = CAST(GETDATE() AS DATE)";
                DataTable dtKasa = _vt.VeriGetir(kasaSorgu);
                int kapanisSayisi = Convert.ToInt32(dtKasa.Rows[0][0]);

                if (kapanisSayisi > 0)
                {
                    lblKasaDurumu.Text = "Bugünün Kasa Kapanışı Yapıldı";
                    lblKasaDurumu.BackColor = Color.SeaGreen;
                    lblKasaDurumu.ForeColor = Color.White;
                }
                else
                {
                    lblKasaDurumu.Text = "Gün Sonu Kapanışı Bekleniyor";
                    lblKasaDurumu.BackColor = Color.DarkOrange;
                    lblKasaDurumu.ForeColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Özet veriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Oturumu Kapat / Hızlı Kilit (FrmGiris'e geri döner)
        private void btnCikisYap_Click(object sender, EventArgs e)
        {
            UserSession.Temizle();
            this.Close(); // FrmGiris içinde ShowDialog() ile açıldığı için otomatik giriş ekranına döner
        }

        private void FrmAnaMenu_Activated(object sender, EventArgs e)
        {
            DashboardGuncelle(); // Form aktif olduğunda özet verileri güncelle
        }
    }
}

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
        private void OzetLabelSinirlariniAyarla()
        {
            LabelSinirla(lblGunlukCiro, pnlGunlukCiro);
            LabelSinirla(lblKritikStok, pnlKritikStok);
            LabelSinirla(lblKasaDurumu, pnlKasaDurumu);
        }

        private void LabelSinirla(Label lbl, Control panel)
        {
            if (lbl == null || panel == null) return;

            lbl.AutoSize = false;
            lbl.Location = new Point(10, 10);
            lbl.Size = new Size(Math.Max(50, panel.Width - 20), Math.Max(30, panel.Height - 20));
            lbl.MaximumSize = new Size(Math.Max(50, panel.Width - 20), Math.Max(30, panel.Height - 20));
        }



        private void KullaniciBilgisiVeYetkiAyarla()
        {
            string rolMetni = UserSession.AdminMi ? "Yönetici (Admin)" : "Kasa Personeli";
            lblAktifKullanici.Text = $"Aktif Kullanıcı:\n{UserSession.AdSoyad} ({rolMetni})";
            lblTarihSaat.Text = DateTime.Now.ToString("dd.MM.yyyy\nHH:mm");

            if (!UserSession.AdminMi)
            {
                btnPersonelYonetim.Enabled = false;
                btnPdfRaporlar.Enabled = false;
            }
        }




        public void DashboardGuncelle()
        {
            try
            {
                OzetLabelSinirlariniAyarla();

                // 1. GÜNÜN CİROSU
                string ciroSorgu = @"SELECT ISNULL(SUM(ToplamTutar), 0)
                                     FROM Satislar
                                     WHERE CAST(SatisTarihi AS DATE) = CAST(GETDATE() AS DATE)
                                     AND SatisDurumu = 'Tamamlandı'";
                DataTable dtCiro = _vt.VeriGetir(ciroSorgu);
                decimal gunlukCiro = (dtCiro != null && dtCiro.Rows.Count > 0 && dtCiro.Rows[0][0] != DBNull.Value)
                    ? Convert.ToDecimal(dtCiro.Rows[0][0])
                    : 0m;

                lblGunlukCiro.Text = $"Günlük Ciro:\n{gunlukCiro:C2}";

                // 2. KRİTİK STOK DURUMU
                string stokSorgu = @"SELECT UrunAdi
                                     FROM Urunler
                                     WHERE SatistaMi = 1 AND MinStokSiniri > 0 AND StokMiktari <= MinStokSiniri";
                DataTable dtStok = _vt.VeriGetir(stokSorgu);

                if (dtStok != null && dtStok.Rows.Count > 0)
                {
                    string ilkUrun = dtStok.Rows[0]["UrunAdi"]?.ToString() ?? "";
                    lblKritikStok.Text = $"{dtStok.Rows.Count} Ürün Kritik Stokta!\n({ilkUrun})";
                    if (pnlKritikStok != null) pnlKritikStok.BackColor = Color.Firebrick;
                    lblKritikStok.ForeColor = Color.White;
                }
                else
                {
                    lblKritikStok.Text = "Stok Durumu:\nTüm Stoklar Yeterli";
                    if (pnlKritikStok != null) pnlKritikStok.BackColor = Color.ForestGreen;
                    lblKritikStok.ForeColor = Color.White;
                }

                // 3. GÜNLÜK KASA KAPANIŞ (Z-RAPORU) DURUMU
                string kasaSorgu = @"SELECT COUNT(*)
                                     FROM KasaKapanislari
                                     WHERE KapanisGunu = CAST(GETDATE() AS DATE)";
                DataTable dtKasa = _vt.VeriGetir(kasaSorgu);
                int kapanisSayisi = (dtKasa != null && dtKasa.Rows.Count > 0 && dtKasa.Rows[0][0] != DBNull.Value)
                    ? Convert.ToInt32(dtKasa.Rows[0][0])
                    : 0;

                if (kapanisSayisi > 0)
                {
                    lblKasaDurumu.Text = "Kasa Durumu:\nBugünün Kapanışı Yapıldı";
                    lblKasaDurumu.BackColor = Color.SeaGreen;
                    lblKasaDurumu.ForeColor = Color.White;
                }
                else
                {
                    TimeSpan simdi = DateTime.Now.TimeOfDay;
                    bool geceYarisinaYakin = simdi >= new TimeSpan(21, 0, 0);

                    if (geceYarisinaYakin)
                    {
                        lblKasaDurumu.Text = "Gece 12'ye Yaklaşıldı!\nKasa Kapanışı Bekleniyor";
                        lblKasaDurumu.BackColor = Color.Crimson;
                    }
                    else
                    {
                        lblKasaDurumu.Text = "Kasa Durumu:\nGün Sonu Kapanışı Bekleniyor";
                        lblKasaDurumu.BackColor = Color.DarkOrange;
                    }

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

        private void btnKasaSatis_Click(object sender, EventArgs e)
        {
            FrmSatis satisFormu = new FrmSatis();
            this.Hide();
            satisFormu.ShowDialog();
            this.Show();
            DashboardGuncelle();

            // Satış ekranından dönüldüğünde ana menüdeki özet kartları tazelemek için:
            // DashboardGuncelle();

        }
       


        private void btnCikisYap_Click_1(object sender, EventArgs e)
        {
            UserSession.Temizle();
            this.Close();
        }
    }
}

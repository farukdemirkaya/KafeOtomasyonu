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
    public partial class FrmSatis : Form
    {
        public FrmSatis()
        {
            InitializeComponent();
        }



        private Button _seciliKategoriButonu;
        private DataTable _tumUrunler;
        private readonly VeriTabani _vt = new VeriTabani();

        private void FrmSatis_Load(object sender, EventArgs e)
        {
            SepetGridHazirla();
            UrunleriYukle();
            KategoriButonlariniHazirla();
            AdetButonlariniHazirla();
            SepetiGuncelle();
            TarihVeFisSayaciniGuncelle();

            dvgSepet.AutoGenerateColumns = false;

            // Formu ekran çalışma alanına göre büyük aç ve ortala
            var alan = Screen.PrimaryScreen.WorkingArea;
            this.StartPosition = FormStartPosition.Manual;
            this.Width = Math.Min(1366, alan.Width - 410);
            this.Height = Math.Min(820, alan.Height - 140);
            this.Left = alan.Left + (alan.Width - this.Width) / 2;
            this.Top = alan.Top + (alan.Height - this.Height) / 2;


        }

        // 4. Transaction gibi özel işlemler için açık olmayan SqlConnection nesnesi üretir
       

        private void SatisiKaydet(int odemeTipId, string odemeAdi, decimal toplamTutar, decimal paraUstu)
        {

            try
            {
                // Bağlantıyı kendi VeriTabani sınıfına göre al (örnek: _vt.BaglantiGetir())
                using (var baglanti = _vt.BaglantiOlustur())
                {
                    baglanti.Open();
                    using (var islem = baglanti.BeginTransaction())
                    {
                        try
                        {
                            string satisSorgu = @"
                        INSERT INTO Satislar
                            (SatisTarihi, PersonelID, OdemeTipID, ToplamTutar, SatisDurumu)
                        VALUES
                            (@SatisTarihi, @PersonelID, @OdemeTipID, @ToplamTutar, @SatisDurumu);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            int satisId;
                            using (var cmdSatis = new System.Data.SqlClient.SqlCommand(satisSorgu, baglanti, islem))
                            {
                                cmdSatis.Parameters.AddWithValue("@SatisTarihi", DateTime.Now);
                                // Aktif personel ID'n varsa onu ver; yoksa geçerli bir PersonelID yaz veya DBNull.Value kullan
                                cmdSatis.Parameters.AddWithValue("@PersonelID", 1);
                                cmdSatis.Parameters.AddWithValue("@OdemeTipID", odemeTipId);
                                cmdSatis.Parameters.AddWithValue("@ToplamTutar", toplamTutar);
                                cmdSatis.Parameters.AddWithValue("@SatisDurumu", 1);

                                satisId = Convert.ToInt32(cmdSatis.ExecuteScalar());
                            }

                            string detaySorgu = @"
                                              INSERT INTO SatisDetaylari
                                                (SatisID, UrunID, Miktar, BirimFiyat)
                                                    VALUES
                                                 (@SatisID, @UrunID, @Miktar, @BirimFiyat);";

                            {


                                string stokDusSorgu = @"
                        UPDATE Urunler
                        SET StokMiktari = StokMiktari - @Miktar
                        WHERE UrunID = @UrunID AND StokTakipTipi = 1;";

                                foreach (var kalem in _sepet)
                                {
                                    using (var cmdDetay = new System.Data.SqlClient.SqlCommand(detaySorgu, baglanti, islem))
                                    {
                                        cmdDetay.Parameters.AddWithValue("@SatisID", satisId);
                                        cmdDetay.Parameters.AddWithValue("@UrunID", kalem.UrunID);
                                        cmdDetay.Parameters.AddWithValue("@Miktar", kalem.Adet);
                                        cmdDetay.Parameters.AddWithValue("@BirimFiyat", kalem.BirimFiyat);
                                        cmdDetay.ExecuteNonQuery();
                                    }

                                    if (kalem.StokTakipTipi == 1)
                                    {
                                        using (var cmdStok = new System.Data.SqlClient.SqlCommand(stokDusSorgu, baglanti, islem))
                                        {
                                            cmdStok.Parameters.AddWithValue("@Miktar", kalem.Adet);
                                            cmdStok.Parameters.AddWithValue("@UrunID", kalem.UrunID);
                                            cmdStok.ExecuteNonQuery();
                                        }
                                    }
                                }

                                islem.Commit();
                                TarihVeFisSayaciniGuncelle();

                                string mesaj = odemeAdi == "Nakit"
                                    ? $"Satış kaydedildi! (Satış No: #{satisId})\nToplam: ₺{toplamTutar:N2}\nPara Üstü: ₺{paraUstu:N2}"
                                    : $"Kredi kartı satışı kaydedildi! (Satış No: #{satisId})\nToplam: ₺{toplamTutar:N2}";

                                MessageBox.Show(mesaj, "Satış Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Ekranı sıfırla ve güncel stokları yükle
                                _sepet.Clear();
                                _alinanTutar = 0m;
                                _seciliAdet = 1;

                                var birButonu = TumButonlariGetir(this)
                                    .FirstOrDefault(b => b.Tag is int val && val == 1);
                                if (birButonu != null)
                                    AdetButonVurgula(birButonu);

                                UrunleriYukle();
                                if (_seciliKategoriButonu?.Tag != null && int.TryParse(_seciliKategoriButonu.Tag.ToString(), out int aktifKatId))
                                {
                                    UrunButonlariniOlustur(aktifKatId);
                                }

                                SepetiGuncelle();
                            }
                        }
                        catch
                        {
                            islem.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Satış kaydedilirken hata oluştu:\n{ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SepetGridHazirla()
        {
            dvgSepet.Columns.Clear();
            dvgSepet.AutoGenerateColumns = false;
            dvgSepet.AllowUserToAddRows = false;
            dvgSepet.AllowUserToDeleteRows = false;
            dvgSepet.ReadOnly = true;
            dvgSepet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dvgSepet.MultiSelect = false;
            dvgSepet.RowHeadersVisible = false;
            dvgSepet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dvgSepet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUrunAdi",
                HeaderText = "Ürün",
                DataPropertyName = "UrunAdi",
                FillWeight = 45
            });

            dvgSepet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAdet",
                HeaderText = "Adet",
                DataPropertyName = "Adet",
                FillWeight = 15
            });

            dvgSepet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colBirimFiyat",
                HeaderText = "Fiyat",
                DataPropertyName = "BirimFiyat",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dvgSepet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTutar",
                HeaderText = "Tutar",
                DataPropertyName = "Tutar",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });
        }
        private void UrunleriYukle()
        {
            string sorgu = @"SELECT UrunID, UrunAdi, KategoriID, BirimFiyat, StokMiktari, StokTakipTipi, SatistaMi
                     FROM Urunler
                     WHERE SatistaMi = 1
                     ORDER BY UrunAdi";
            _tumUrunler = _vt.VeriGetir(sorgu);
        }

        private void KategoriButonlariniHazirla()
        {
            foreach (Control ctrl in FlwKategoriler.Controls)
            {
                if (ctrl is Button btn)
                {
                    if (btn.Tag == null || string.IsNullOrWhiteSpace(btn.Tag.ToString()))
                    {
                        btn.Tag = btn.Text.Trim();
                    }

                    btn.Click -= KategoriButon_Click;
                    btn.Click += KategoriButon_Click;
                }
            }
        }

        private void KategoriButon_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn)) return;

            if (_seciliKategoriButonu != null && _seciliKategoriButonu != btn)
            {
                _seciliKategoriButonu.BackColor = SystemColors.Control;
                _seciliKategoriButonu.ForeColor = SystemColors.ControlText;
            }

            btn.BackColor = Color.SeaGreen;
            btn.ForeColor = Color.White;
            _seciliKategoriButonu = btn;

            if (btn.Tag != null && int.TryParse(btn.Tag.ToString(), out int kategoriId))
            {
                UrunButonlariniOlustur(kategoriId);
            }
        }


        private void UrunButonlariniOlustur(int kategoriId)
        {
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.WrapContents = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.Controls.Clear();

            if (_tumUrunler == null) return;

            // scroll bar payı (yaklaşık 20px) düşülerek 3 sütuna göre buton genişliği hesaplanır
            int kullanilabilirGenislik = Math.Max(240, flowLayoutPanel1.ClientSize.Width - 24);
            int sutunSayisi = 3;
            int yatayBosluk = 6;
            int butonGenislik = (kullanilabilirGenislik - (sutunSayisi * yatayBosluk * 2)) / sutunSayisi;
            butonGenislik = Math.Max(85, butonGenislik);

            var filtrelenmis = _tumUrunler.AsEnumerable()
                .Where(r => r["KategoriID"] != DBNull.Value && Convert.ToInt32(r["KategoriID"]) == kategoriId);

            foreach (DataRow row in filtrelenmis)
            {
                string urunAdi = row["UrunAdi"].ToString();
                decimal fiyat = Convert.ToDecimal(row["BirimFiyat"]);
                decimal stok = Convert.ToDecimal(row["StokMiktari"]);
                int stokTakipTipi = Convert.ToInt32(row["StokTakipTipi"]);

                bool stokYok = (stokTakipTipi == 1 && stok <= 0);

                Button btnUrun = new Button
                {
                    Width = butonGenislik,
                    Height = 85,
                    Margin = new Padding(yatayBosluk),
                    Tag = row,
                    Enabled = !stokYok,
                    BackColor = stokYok ? Color.LightGray : Color.White,
                    ForeColor = stokYok ? Color.DarkGray : Color.Black,
                    Text = stokYok ? $"{urunAdi}\n(Stok Yok)" : $"{urunAdi}\n₺{fiyat:N2}"
                };

                btnUrun.Click += UrunButon_Click;
                flowLayoutPanel1.Controls.Add(btnUrun);
            }
        }

        private decimal _alinanTutar = 0m;
        private int _seciliAdet = 1;
        private Button _seciliAdetButonu;
        private readonly List<SepetKalemi> _sepet = new List<SepetKalemi>();

        private class SepetKalemi
        {
            public int UrunID { get; set; }
            public string UrunAdi { get; set; }
            public decimal BirimFiyat { get; set; }
            public int Adet { get; set; }
            public int StokTakipTipi { get; set; }
            public decimal MevcutStok { get; set; }
            public decimal Tutar => BirimFiyat * Adet;
        }

        private void AdetButonlariniHazirla()
        {
            foreach (Button btn in TumButonlariGetir(this))
            {
                if (int.TryParse(btn.Text.Trim(), out int adet) && adet >= 1 && adet <= 14)
                {
                    btn.Tag = adet;
                    btn.Click -= AdetButon_Click;
                    btn.Click += AdetButon_Click;

                    if (adet == 1)
                    {
                        AdetButonVurgula(btn);
                    }
                }
            }
        }

        private IEnumerable<Button> TumButonlariGetir(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Button b)
                    yield return b;

                foreach (Button child in TumButonlariGetir(ctrl))
                    yield return child;
            }
        }

        private void AdetButon_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn) || btn.Tag == null) return;

            if (int.TryParse(btn.Tag.ToString(), out int adet))
            {
                _seciliAdet = adet;
                AdetButonVurgula(btn);
            }
        }

        private void AdetButonVurgula(Button aktifButon)
        {
            if (_seciliAdetButonu != null && _seciliAdetButonu != aktifButon)
            {
                _seciliAdetButonu.BackColor = SystemColors.Control;
                _seciliAdetButonu.ForeColor = SystemColors.ControlText;
            }

            aktifButon.BackColor = Color.DodgerBlue;
            aktifButon.ForeColor = Color.White;
            _seciliAdetButonu = aktifButon;
        }

        private void UrunButon_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is DataRow row)) return;

            int urunId = Convert.ToInt32(row["UrunID"]);
            string urunAdi = row["UrunAdi"].ToString();
            decimal birimFiyat = Convert.ToDecimal(row["BirimFiyat"]);
            decimal stokMiktari = Convert.ToDecimal(row["StokMiktari"]);
            int stokTakipTipi = Convert.ToInt32(row["StokTakipTipi"]);

            int eklenecekAdet = _seciliAdet;
            var mevcutKalem = _sepet.FirstOrDefault(x => x.UrunID == urunId);
            int sepettekiAdet = mevcutKalem != null ? mevcutKalem.Adet : 0;

            if (stokTakipTipi == 1 && (sepettekiAdet + eklenecekAdet) > stokMiktari)
            {
                MessageBox.Show($"{urunAdi} için yeterli stok yok! Mevcut stok: {stokMiktari:0.##}",
                    "Stok Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (mevcutKalem != null)
            {
                mevcutKalem.Adet += eklenecekAdet;
            }
            else
            {
                _sepet.Add(new SepetKalemi
                {
                    UrunID = urunId,
                    UrunAdi = urunAdi,
                    BirimFiyat = birimFiyat,
                    Adet = eklenecekAdet,
                    StokTakipTipi = stokTakipTipi,
                    MevcutStok = stokMiktari
                });
            }

            // Seçili adeti tekrar 1'e sıfırla
            _seciliAdet = 1;
            var birButonu = TumButonlariGetir(this)
                .FirstOrDefault(b => b.Tag is int val && val == 1);
            if (birButonu != null)
                AdetButonVurgula(birButonu);

            SepetiGuncelle();
        }

        private void SepetiGuncelle()
        {
            int? seciliUrunId = null;
            if (dvgSepet.CurrentRow?.DataBoundItem is SepetKalemi aktifKalem)
            {
                seciliUrunId = aktifKalem.UrunID;
            }

            dvgSepet.AutoGenerateColumns = false;
            dvgSepet.DataSource = null;
            dvgSepet.DataSource = _sepet.ToList();

            if (seciliUrunId.HasValue)
            {
                foreach (DataGridViewRow row in dvgSepet.Rows)
                {
                    if (row.DataBoundItem is SepetKalemi kalem && kalem.UrunID == seciliUrunId.Value)
                    {
                        row.Selected = true;
                        dvgSepet.CurrentCell = row.Cells[0];
                        break;
                    }
                }
            }

            decimal genelToplam = _sepet.Sum(x => x.Tutar);

            lblGenelToplam.Text = $"Toplam : ₺{genelToplam:N2}";
            ParaUstuGuncelle();
        }




        private void adt7_Click(object sender, EventArgs e)
        {

        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dvgSepet.CurrentRow?.DataBoundItem is SepetKalemi seciliKalem)
            {
                var kalem = _sepet.FirstOrDefault(x => x.UrunID == seciliKalem.UrunID);
                if (kalem != null)
                {
                    if (kalem.Adet > 1)
                        kalem.Adet -= 1;
                    else
                        _sepet.Remove(kalem);

                    SepetiGuncelle();
                }
            }
            else
            {
                MessageBox.Show("Lütfen sepetten silinecek bir ürün seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ParaUstuGuncelle()
        {
            decimal genelToplam = _sepet.Sum(x => x.Tutar);
            decimal fark = _alinanTutar - genelToplam;

            // İstersen sağ üstteki label'a alınan tutarı da yazabilirsin:
            // lblAlinan.Text = $"Alınan: ₺{_alinanTutar:N2}";

            if (genelToplam <= 0 && _alinanTutar <= 0)
            {
                lblParaUstu.Text = "Para Üstü : ₺0,00";
            }
            else if (fark < 0)
            {
                // Henüz sepet toplamına ulaşmadıysa eksi ile kalan/eksik tutarı gösterir
                lblParaUstu.Text = $"Para Üstü : -₺{Math.Abs(fark):N2}";
            }
            else
            {
                // Sepet toplamına ulaştı veya geçtiyse para üstünü gösterir
                lblParaUstu.Text = $"Para Üstü : ₺{fark:N2}";
            }
        }

        private void ParaButon_Click(object sender, EventArgs e)
        {
            if (_sepet.Count == 0 || _sepet.Sum(x => x.Tutar) <= 0)
            {
                MessageBox.Show("Önce sepete ürün eklemelisiniz.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!(sender is Button btn)) return;

            decimal eklenecek = 0m;

            if (btn.Tag != null && decimal.TryParse(btn.Tag.ToString(), out decimal tagTutar))
            {
                eklenecek = tagTutar;
            }
            else
            {
                string temizMetin = btn.Text
                    .Replace("₺", "")
                    .Replace("TL", "")
                    .Trim();

                if (!decimal.TryParse(temizMetin, out eklenecek))
                    return;
            }

            _alinanTutar += eklenecek;
            ParaUstuGuncelle();
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            if (_sepet.Count == 0 && _alinanTutar == 0m) return;

            _sepet.Clear();
            _alinanTutar = 0m;
            _seciliAdet = 1;

            var birButonu = TumButonlariGetir(this)
                .FirstOrDefault(b => b.Tag is int val && val == 1);
            if (birButonu != null)
                AdetButonVurgula(birButonu);

            SepetiGuncelle();
            ParaUstuGuncelle();
        }

        private void btnNakitOdeme_Click(object sender, EventArgs e)
        {
            decimal genelToplam = _sepet.Sum(x => x.Tutar);
            if (genelToplam <= 0 || _sepet.Count == 0)
            {
                MessageBox.Show("Sepette satışa hazır ürün bulunmuyor.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Banknot butonlarına basıldıysa alınan tutarı kullan; basılmadıysa toplam tutarı baz al
            decimal alinan = _alinanTutar > 0 ? _alinanTutar : genelToplam;
            decimal paraUstu = alinan > genelToplam ? alinan - genelToplam : 0m;

            SatisiKaydet(odemeTipId: 1, odemeAdi: "Nakit", toplamTutar: genelToplam, paraUstu: paraUstu);
        }

        private void btnKartOdeme_Click(object sender, EventArgs e)
        {
            decimal genelToplam = _sepet.Sum(x => x.Tutar);
            if (genelToplam <= 0 || _sepet.Count == 0)
            {
                MessageBox.Show("Sepette satışa hazır ürün bulunmuyor.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // OdemeTipID = 2 (Kredi Kartı) varsayımıyla — kendi OdemeTipleri tablosundaki ID ile eşleştir
            SatisiKaydet(odemeTipId: 2, odemeAdi: "Kredi Kartı", toplamTutar: genelToplam, paraUstu: 0m);
        }

        private void btnGeriDon_Click(object sender, EventArgs e)
        {
            // Eğer ana menü formu zaten arka planda açıksa onu öne getirip satış ekranını kapat
            var anaMenu = Application.OpenForms.OfType<FrmAnaMenu>().FirstOrDefault();
            if (anaMenu != null)
            {
                anaMenu.Show();
                anaMenu.BringToFront();
                this.Close();
            }
            else
            {
                // Açık değilse yeni örnek açıp satış ekranını kapat
                FrmAnaMenu yeniAnaMenu = new FrmAnaMenu();
                yeniAnaMenu.Show();
                this.Close();
            }
        }

        private void TarihVeFisSayaciniGuncelle()
        {
            // 1. Tarih etiketi (kendi Label adını yaz, örneğin lblTarih)
            lblTarih.Text = DateTime.Now.ToString("dd.MM.yyyy");

            // 2. O gün kesilen fiş sayısı (kendi Label adını yaz, örneğin lblFisSayisi)
            try
            {
                string sorgu = @"
            SELECT COUNT(*)
            FROM Satislar
            WHERE CAST(SatisTarihi AS DATE) = CAST(GETDATE() AS DATE);";

                DataTable dt = _vt.VeriGetir(sorgu);
                int gunlukFisSayisi = (dt != null && dt.Rows.Count > 0)
                    ? Convert.ToInt32(dt.Rows[0][0])
                    : 0;

                lblFisSayisi.Text = $"Fiş : {gunlukFisSayisi}";
            }
            catch
            {
                lblFisSayisi.Text = "Fiş : 0";
            }
        }
        
      
    }
 }


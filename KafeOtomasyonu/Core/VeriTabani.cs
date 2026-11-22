using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KafeOtomasyonu.Core
{
    public class VeriTabani
    {
        // Merkezi bağlantı cümlesi (Server ve DB adını kendi ortamınıza göre güncelleyebilirsiniz)
        private readonly string baglantiCumlesi = @"Server=MSI\SQLEXPRESS; Database=KafeOtomasyonDB; Integrated Security=True;";

        // 1. Veri Listeleme (Parametresiz)
        public DataTable VeriGetir(string sorgu)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection baglanti = new SqlConnection(baglantiCumlesi))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(sorgu, baglanti))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri listeleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        // 2. Veri Listeleme (Parametreli - Güvenli Sorgular İçin)
        public DataTable VeriGetir(string sorgu, SqlParameter[] parametreler)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection baglanti = new SqlConnection(baglantiCumlesi))
                {
                    using (SqlCommand komut = new SqlCommand(sorgu, baglanti))
                    {
                        if (parametreler != null)
                            komut.Parameters.AddRange(parametreler);

                        using (SqlDataAdapter da = new SqlDataAdapter(komut))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Parametreli veri getirme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        // 3. Ekleme, Güncelleme, Silme (INSERT, UPDATE, DELETE)
        public bool KomutCalistir(string sorgu, params SqlParameter[] parametreler)
        {
            try
            {
                using (SqlConnection baglanti = new SqlConnection(baglantiCumlesi))
                {
                    using (SqlCommand komut = new SqlCommand(sorgu, baglanti))
                    {
                        if (parametreler != null)
                        {
                            komut.Parameters.AddRange(parametreler);
                        }
                        baglanti.Open();
                        int etkilenenSatir = komut.ExecuteNonQuery();
                        return etkilenenSatir > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı işlem hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
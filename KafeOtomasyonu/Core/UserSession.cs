using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafeOtomasyonu.Core
{

    public static class UserSession
    {
        public static int PersonelID { get; set; }
        public static string AdSoyad { get; set; }
        public static string KullaniciAdi { get; set; }
        public static int RolID { get; set; } // 1: Admin, 2: Personel
        public static bool AdminMi => RolID == 1;

        // Oturumu sıfırlamak için (Çıkış yapıldığında)
        public static void Temizle()
        {
            PersonelID = 0;
            AdSoyad = string.Empty;
            KullaniciAdi = string.Empty;
            RolID = 0;
        }
    }
}

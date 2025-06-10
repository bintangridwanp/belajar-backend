using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jurnal4
{
    class KodeProduk
    {
        private String[] ProdukElektronik =
        {
            "Laptop", "Smartphone", "Tablet", "Headset", "keyboard", "Mouse", "Printer", "Monitor", "Smartwatch", "Kamera"
        };

        private String[] Kode_produk =
        {
            "E100", "E101", "E102", "E103", "E104", "E105", "E106", "E107", "E108", "E109"
        };


        public String? getKodeProduk(String ProdukElektronik)
        {
            for (int i = 0; i < this.ProdukElektronik.Length; i++)
            {
                if(ProdukElektronik == this.ProdukElektronik[i])
                {
                    return Kode_produk[i];
                }
            }
            return null;
        }
    }
}

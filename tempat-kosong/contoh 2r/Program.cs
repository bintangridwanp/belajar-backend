// See https://aka.ms/new-console-template for more information
using jurnal4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        KodeProduk produk = new KodeProduk();

        Console.WriteLine(produk.getKodeProduk("Headset"));
        Console.WriteLine(produk.getKodeProduk("Smartwatch"));

        FanLaptop mode = new FanLaptop();

        mode.modeUp();
        mode.modeUp();
        mode.modeUp();
        mode.modeUp();
        mode.offTurboShorcut();

        mode.onTurboShorcut();
        mode.modeDown();
        mode.modeDown();
        mode.modeDown();
        mode.modeDown();
        mode.onTurboShorcut();
    }
}

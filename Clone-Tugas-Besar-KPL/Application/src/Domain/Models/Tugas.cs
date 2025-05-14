namespace Tubes_KPL.src.Domain.Models
{
    public enum KategoriTugas
    {
        Akademik,
        NonAkademik
    }

    public enum StatusTugas
    {
        BelumMulai = 0,
        SedangDikerjakan = 1,
        Selesai = 2,
        Terlewat = 3
    }

    public class Tugas
    {
        public int Id { get; set; }
        public string Judul { get; set; }
        public DateTime Deadline { get; set; }
        public StatusTugas Status { get; set; }
        public KategoriTugas Kategori { get; set; }
    }
}
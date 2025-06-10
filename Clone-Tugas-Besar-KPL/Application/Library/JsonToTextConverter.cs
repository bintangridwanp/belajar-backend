namespace TaskUtilities.Libraries
{
    public class JsonToTextConverter
    {
        
        public static string ConvertTasksToText(List<Tugas> tasks)
        {
            var textContent = "Daftar Tugas:\n";
            textContent += "=================================================================================================\n";
            textContent += "| ID |            Judul            |   Kategori   |      Status      |       Deadline        |\n";
            textContent += "=================================================================================================\n";

            foreach (var t in tasks)
            {
                string deadline = FormatDate(t.Deadline);
                string status = t.Status.ToString();

                if (IsDeadlineApproaching(t.Deadline) && t.Status != StatusTugas.Selesai && t.Status != StatusTugas.Terlewat)
                {
                    deadline += " ⚠️";
                }

                textContent += $"| {t.Id,-3}| {t.Judul,-28} | {t.Kategori,-12} | {status,-16} | {deadline,-21} |\n";
            }
            textContent += "=================================================================================================\n";
            return textContent;
        }

        public static int DaysUntilDeadline(DateTime deadline)
        {
            if (deadline < DateTime.Today)
                throw new ArgumentException("Deadline tidak boleh di masa lalu", nameof(deadline));

            var today = DateTime.Today;
            var days = (int)Math.Ceiling((deadline.Date - today).TotalDays);

            if (days < 0)
                throw new InvalidOperationException("Hasil perhitungan tidak valid.");

            return days;
        }

        public static bool IsDeadlineApproaching(DateTime deadline)
        {
            var daysRemaining = DaysUntilDeadline(deadline);
            return daysRemaining >= 0 && daysRemaining <= 3;
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd MMMM yyyy");
        }

        public class Tugas
        {
            public int Id { get; set; }
            public string Judul { get; set; }
            public DateTime Deadline { get; set; }
            public StatusTugas Status { get; set; }
            public KategoriTugas Kategori { get; set; }
        }

        public enum StatusTugas
        {
            BelumMulai = 0,
            SedangDikerjakan = 1,
            Selesai = 2,
            Terlewat = 3
        }

        public enum KategoriTugas
        {
            Akademik,
            NonAkademik
        }
        
    }
}
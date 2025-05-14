using System.Globalization;

namespace Tubes_KPL.src.Application.Helpers
{
    public static class DateHelper
    {
        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd MMMM yyyy");
        }

        public static string FormatDateTime(DateTime date)
        {
            return date.ToString("dd MMMM yyyy HH:mm");
        }

        // Design by Contract: Precondition by bintang
        public static int DaysUntilDeadline(DateTime deadline)
        {
            //Preconditions
            if (deadline < DateTime.Today)
            {
                Console.WriteLine($"[ERROR] Deadline tidak valid. Input: {deadline}");
                throw new ArgumentException("Deadline tidak boleh di masa lalu", nameof(deadline));
            }

            var today = DateTime.Today;
            var days = (int)Math.Ceiling((deadline.Date - today).TotalDays);

            //Postconditions
            if (days < 0)
            {
                Console.WriteLine($"[ERROR] Hasil perhitungan tidak valid. Days: {days}");
                throw new InvalidOperationException("Hasil perhitungan tidak valid.");
            }
            return days;
        }

        public static bool IsDeadlineApproaching(DateTime deadline)
        {
            var daysRemaining = DaysUntilDeadline(deadline);
            return daysRemaining >= 0 && daysRemaining <= 3;
        }
        
        public static bool IsDeadlinePassed(DateTime deadline)
        {
            return deadline < DateTime.Now;
        }
        
        public static bool TryParseDate(string input, out DateTime result)
        {
            return DateTime.TryParseExact(input, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}
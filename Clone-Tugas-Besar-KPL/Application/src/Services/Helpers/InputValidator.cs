using Spectre.Console;
using Tubes_KPL.src.Domain.Models;

namespace Tubes_KPL.src.Application.Helpers
{
    public static class InputValidator
    {
        // Defensive programming/design by contract code by zuhri
        public static string NonEmptyInput(string prompt)
        {
            string? input;
            Console.Write(prompt);
            input = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(input))
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[red]Input tidak boleh kosong![/]");
                Console.Write(prompt);
                input = Console.ReadLine();

            }
            ;

            return input.Trim();
        }

        // input ui by zuhri
        public static int InputValidStatus()
        {
            var selectedStatus = AnsiConsole.Prompt(
                new SelectionPrompt<StatusTugas>()
                    .Title("Pilih [green]Status Tugas[/]:")
                    .PageSize(4)
                    .AddChoices(Enum.GetValues<StatusTugas>())
            );

            return (int)selectedStatus;
        }

        // Defensive programming/design by contract code by zuhri
        public static bool IsValidJudul(string judul)
        {
            return !string.IsNullOrWhiteSpace(judul) && judul.Length <= 100;
        }

        // bintang : poin 4 Validasi Input (Pre/Postcondition) 
        public static bool IsValidDeadline(DateTime deadline)
        {
            if (deadline < DateTime.Now)
            {
                Console.WriteLine($"[ERROR] Deadline tidak valid. Input: {deadline}");
                return false;
            }

            return true;
        }

        public static bool IsValidStatusTransition(StatusTugas currentStatus, StatusTugas newStatus)
        {
            switch (currentStatus)
            {
                case StatusTugas.BelumMulai:
                    return newStatus == StatusTugas.SedangDikerjakan ||
                           newStatus == StatusTugas.Selesai ||
                           newStatus == StatusTugas.Terlewat;

                case StatusTugas.SedangDikerjakan:
                    return newStatus == StatusTugas.Selesai ||
                           newStatus == StatusTugas.Terlewat;

                case StatusTugas.Selesai:
                    return newStatus == StatusTugas.Terlewat;

                case StatusTugas.Terlewat:
                    return false;

                default:
                    return false;
            }
        }

        public static bool TryParseId(string idInput, out int id)
        {
            return int.TryParse(idInput, out id) && id > 0;
        }
    }
}
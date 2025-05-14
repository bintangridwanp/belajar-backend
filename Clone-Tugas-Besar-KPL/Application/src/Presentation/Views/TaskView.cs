using Spectre.Console;
using Tubes_KPL.src.Application.Helpers;
using Tubes_KPL.src.Infrastructure.Configuration;
using Tubes_KPL.src.Presentation.Presenters;

namespace Tubes_KPL.src.Presentation.Views
{
    public class TaskView
    {
        private readonly TaskPresenter _presenter;
        private readonly IConfigProvider _configProvider;

        public TaskView(TaskPresenter presenter, IConfigProvider configProvider)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _configProvider = configProvider ?? throw new ArgumentNullException(nameof(configProvider));
        }

        public async Task ShowMainMenu()
        {
            var isRunning = true;

            while (isRunning)
            {
                AnsiConsole.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold yellow]=== APLIKASI MANAJEMEN TUGAS MAHASISWA ===[/]")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Gunakan panah atas/bawah untuk memilih)[/]")
                        .AddChoices([
                            "Lihat Daftar Tugas",
                            "Lihat Tugas Berdasarkan Rentang Waktu",
                            "Lihat Detail Tugas",
                            "Tambah Tugas Baru",
                            "Perbarui Tugas",
                            "Ubah Status Tugas",
                            "Hapus Tugas",
                            "Cetak Daftar Tugas ke File JSON dan TXT",
                            "Keluar"
                         ]));

                switch (choice)
                {
                    case "Lihat Daftar Tugas":
                        await ShowAllTasks();
                        break;
                    case "Lihat Tugas Berdasarkan Rentang Waktu":
                        await ShowTasksByDateRange();
                        break;
                    case "Lihat Detail Tugas":
                        await ShowTaskDetails();
                        break;
                    case "Tambah Tugas Baru":
                        await AddNewTask();
                        break;
                    case "Perbarui Tugas":
                        await UpdateTask();
                        break;
                    case "Ubah Status Tugas":
                        await UpdateTaskStatus();
                        break;
                    case "Hapus Tugas":
                        await DeleteTask();
                        break;
                    case "Cetak Daftar Tugas ke File JSON dan TXT":
                        await PrintTasksToFiles();
                        break;
                    case "Keluar":
                        if (AnsiConsole.Confirm("Apakah Anda yakin ingin keluar?"))
                        {
                            isRunning = false;
                            AnsiConsole.MarkupLine("[green]Terima kasih! Program akan ditutup.[/]");
                            await Task.Delay(1000);
                        }
                        break;
                }
            }
        }
        private async Task ShowAllTasks()
        {
            Console.Clear();

            string result = await _presenter.GetAllTasks();
            Console.WriteLine(result);

            Console.WriteLine("\nTekan Enter untuk kembali ke menu utama...");
            Console.ReadLine();
        }

        private async Task ShowTasksByDateRange()
        {
            Console.Clear();
            Console.WriteLine("=== LIHAT TUGAS BERDASARKAN RENTANG WAKTU ===\n");

            Console.Write("Masukkan Tanggal Mulai (DD/MM/YYYY): ");
            string startDateStr = Console.ReadLine();

            Console.Write("Masukkan Tanggal Akhir (DD/MM/YYYY): ");
            string endDateStr = Console.ReadLine();

            string result = await _presenter.GetTasksByDateRange(startDateStr, endDateStr);
            if (!string.IsNullOrWhiteSpace(result))
                Console.WriteLine("\n" + result);

            Console.WriteLine("\nTekan Enter untuk kembali ke menu utama...");
            Console.ReadLine();
        }


        private async Task ShowTaskDetails()
        {
            Console.Clear();
            Console.WriteLine("=== DETAIL TUGAS ===\n");

            Console.Write("Masukkan ID Tugas: ");
            string idStr = Console.ReadLine();

            string result = await _presenter.GetTaskDetails(idStr);

            // Add reminder logic
            //var reminderSettings = _configProvider.GetConfig<Dictionary<string, object>>("ReminderSettings");
            //if (reminderSettings != null && ((JsonElement)reminderSettings["EnableReminders"]).GetBoolean())
            //{
            //    Console.WriteLine("[Pengingat Aktif]");
            //}

            Console.WriteLine("\n" + result);

            Console.WriteLine("\nTekan Enter untuk kembali ke menu utama...");
            Console.ReadLine();
        }

        private async Task AddNewTask()
        {
            Console.Clear();
            Console.WriteLine("=== TAMBAH TUGAS BARU ===\n");

            Console.Write("Judul Tugas: ");
            string judul = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(judul))
            {
                var defaultTaskConfig = _configProvider.GetConfig<Dictionary<string, object>>("DefaultTask");
                judul = defaultTaskConfig["Judul"].ToString();
                Console.WriteLine($"Judul default digunakan: {judul}");
            }

            Console.Write("Deadline (DD/MM/YYYY): ");
            string deadlineStr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(deadlineStr) || !DateHelper.TryParseDate(deadlineStr, out DateTime deadline))
            {
                Console.WriteLine("Deadline tidak dimasukkan atau format tidak valid. Menggunakan tanggal hari ini sebagai default.");
                deadline = DateTime.Now;
            }

            Console.WriteLine("Kategori Tugas:");
            Console.WriteLine("0. Akademik");
            Console.WriteLine("1. Non-Akademik");
            Console.Write("Pilih Kategori (0/1): ");

            if (!int.TryParse(Console.ReadLine(), out int kategoriIndex) || kategoriIndex < 0 || kategoriIndex > 1)
            {
                Console.WriteLine("Kategori tidak valid! Menggunakan default: Akademik");
                kategoriIndex = 0;
            }

            string result = await _presenter.CreateTask(judul, deadline.ToString("dd/MM/yyyy"), kategoriIndex);
            Console.WriteLine("\n" + result);

            Console.WriteLine("\nTekan Enter untuk kembali ke menu utama...");
            Console.ReadLine();
        }

        private async Task UpdateTask()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]PERBARUI TUGAS[/]");

            Console.Write("Masukkan ID Tugas: ");
            string idStr = Console.ReadLine();

            Console.Write("Judul Tugas Baru: ");
            string judul = Console.ReadLine();

            Console.Write("Deadline Baru (DD/MM/YYYY): ");
            string deadlineStr = Console.ReadLine();

            Console.WriteLine("Kategori Tugas:");
            Console.WriteLine("0. Akademik");
            Console.WriteLine("1. Non-Akademik");
            Console.Write("Pilih Kategori (0/1): ");

            if (!int.TryParse(Console.ReadLine(), out int kategoriIndex) || kategoriIndex < 0 || kategoriIndex > 1)
            {
                Console.WriteLine("Kategori tidak valid! Menggunakan default: Akademik");
                kategoriIndex = 0;
            }

            string result = await _presenter.UpdateTask(idStr, judul, deadlineStr, kategoriIndex);
            Console.WriteLine("\n" + result);

            Console.WriteLine("\nTekan Enter untuk kembali ke menu utama...");
            Console.ReadLine();
        }

        private async Task UpdateTaskStatus()
        {
            Console.Clear();
            Console.WriteLine("=== UBAH STATUS TUGAS ===\n");

            string idStr = InputValidator.NonEmptyInput("Masukkan ID Tugas: ");

            var result = await _presenter.UpdateTaskStatus(idStr);

            AnsiConsole.MarkupLine($"\n{result}");
            Console.WriteLine("\nTekan Enter untuk kembali ke menu utama...");
            Console.ReadLine();
        }

        private async Task DeleteTask()
        {
            Console.Clear();
            Console.WriteLine("=== HAPUS TUGAS ===\n");

            Console.Write("Masukkan ID Tugas: ");
            string idStr = Console.ReadLine();

            Console.Write("Anda yakin ingin menghapus tugas ini? (y/n): ");
            string confirmation = Console.ReadLine()?.ToLower();

            if (confirmation != "y")
            {
                Console.WriteLine("Operasi dibatalkan.");
                Console.WriteLine("\nTekan Enter untuk kembali ke menu utama...");
                Console.ReadLine();
                return;
            }

            string result = await _presenter.DeleteTask(idStr);
            Console.WriteLine("\n" + result);

            Console.WriteLine("\nTekan Enter untuk kembali ke menu utama...");
            Console.ReadLine();
        }
        private async Task PrintTasksToFiles()
        {
            Console.Clear();
            Console.WriteLine("=== CETAK DAFTAR TUGAS KE FILE ===\n");

            Console.Write("Masukkan path untuk file JSON: ");
            string jsonFilePath = Console.ReadLine();

            Console.Write("Masukkan path untuk file TXT: ");
            string textFilePath = Console.ReadLine();

            try
            {
                string result = await _presenter.PrintTasksToFilesFromApi(jsonFilePath, textFilePath);
                Console.WriteLine("\n" + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Terjadi kesalahan: {ex.Message}");
            }

            Console.WriteLine("\nTekan Enter untuk kembali ke menu utama...");
            Console.ReadLine();
        }
    }
}
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Tubes_KPL.src.Infrastructure.Configuration;
using Tubes_KPL.src.Presentation.Presenters;
using Tubes_KPL.src.Presentation.Views;
using Tubes_KPL.src.Application.Services;
using Tubes_KPL.src.Infrastructure.Repositories;

namespace Tubes_KPL
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                string configFilePath = "../../../src/Infrastructure/Configuration/config.json";
                if (!System.IO.File.Exists(configFilePath))
                {
                    Console.WriteLine($"[ERROR] File konfigurasi tidak ditemukan: {configFilePath}");
                    return;
                }

                var configProvider = new JsonConfigProvider(configFilePath);

                var repository = new TugasRepository();

                var taskService = new TaskService(repository);

                var taskPresenter = new TaskPresenter(configProvider);

                var taskView = new TaskView(taskPresenter, configProvider);

                Console.WriteLine("Memulai aplikasi Manajemen Tugas Mahasiswa...");
                await taskView.ShowMainMenu();

                Console.WriteLine("Aplikasi telah ditutup. Terima kasih!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Terjadi kesalahan: {ex.Message}");
                Console.WriteLine("Aplikasi mengalami masalah. Silakan coba lagi nanti.");
                Console.WriteLine("Tekan Enter untuk keluar...");
                Console.ReadLine();
            }
        }
    }
}
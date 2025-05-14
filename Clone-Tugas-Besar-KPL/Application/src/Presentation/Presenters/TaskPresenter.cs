using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;
using TaskUtilities.Libraries;
using Tubes_KPL.src.Application.Helpers;
using Tubes_KPL.src.Application.Services;
using Tubes_KPL.src.Domain.Models;
using Tubes_KPL.src.Infrastructure.Configuration;
using Tubes_KPL.src.Services.Libraries;
using TaskUtilities.Libraries;
namespace Tubes_KPL.src.Presentation.Presenters
{
    public class TaskPresenter
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:4000/api/tugas";

        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IConfigProvider _configProvider;

        public TaskPresenter(IConfigProvider configProvider)
        {
            _httpClient = new HttpClient();
            _configProvider = configProvider ?? throw new ArgumentNullException(nameof(configProvider));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }


        public async Task<string> CreateTask(string judul, string deadlineStr, int kategoriIndex)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(judul))
                {
                    var defaultTaskConfig = _configProvider.GetConfig<Dictionary<string, string>>("DefaultTask");
                    judul = defaultTaskConfig.TryGetValue("Judul", out string? value) && !string.IsNullOrWhiteSpace(value)
                        ? value : "Tugas Default";
                }

                if (!DateHelper.TryParseDate(deadlineStr, out DateTime deadline))
                {
                    var defaultTaskConfig = _configProvider.GetConfig<Dictionary<string, string>>("DefaultTask");
                    int defaultDays = int.Parse(defaultTaskConfig["DeadlineDaysFromNow"]);
                    deadline = DateTime.Now.AddDays(defaultDays);
                }

                KategoriTugas kategori = kategoriIndex == 0 ? KategoriTugas.Akademik : KategoriTugas.NonAkademik;

                var newTugas = new Tugas
                {
                    Judul = judul,
                    Deadline = deadline,
                    Kategori = kategori,
                    Status = StatusTugas.BelumMulai
                };

                var response = await _httpClient.PostAsJsonAsync(BaseUrl, newTugas, _jsonOptions);
                if (response.IsSuccessStatusCode)
                {
                    var createdTask = await response.Content.ReadFromJsonAsync<Tugas>(_jsonOptions);
                    return $"Tugas berhasil dibuat dengan ID: {createdTask.Id}";
                }
                return $"Error: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<Result<string>> UpdateTaskStatus(string idStr)
        {
            int statusIndex = InputValidator.InputValidStatus();

            try
            {
                // Design by Contract: Precondition by zuhri
                if (!InputValidator.TryParseId(idStr, out int id))
                    return Result<string>.Failure("ID tugas tidak valid! Pastikan berupa angka positif.");

                if (!Enum.IsDefined(typeof(StatusTugas), statusIndex))
                    return Result<string>.Failure("Indeks status tidak valid! Gunakan 0-3.");

                StatusTugas newStatus = (StatusTugas)statusIndex;

                var getResponse = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (!getResponse.IsSuccessStatusCode)
                    return Result<string>.Failure("Tugas tidak ditemukan!");

                var task = await getResponse.Content.ReadFromJsonAsync<Tugas>(_jsonOptions);
                if (task is null)
                    return Result<string>.Failure("Gagal membaca data tugas dari server.");

                if (!StatusStateMachine.CanTransition(task.Status, newStatus))
                {
                    return Result<string>.Failure($"Tidak bisa mengubah status dari {task.Status} ke {newStatus}!");
                }

                task.Status = newStatus;
                var updateResponse = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", task, _jsonOptions);

                if (!updateResponse.IsSuccessStatusCode)
                    return Result<string>.Failure($"Gagal memperbarui status. Kode: {updateResponse.StatusCode}");

                var updatedTask = await updateResponse.Content.ReadFromJsonAsync<Tugas>(_jsonOptions);

                return Result<string>.Success($"Status tugas '{updatedTask?.Judul}' berhasil diubah menjadi {updatedTask?.Status}");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Terjadi kesalahan: {ex.Message}");
            }
        }


        public async Task<string> UpdateTask(string idStr, string judul, string deadlineStr, int kategoriIndex)
        {
            try
            {
                if (!InputValidator.IsValidJudul(judul))
                    return "Judul tugas tidak valid! Pastikan tidak kosong dan maksimal 100 karakter.";

                if (!InputValidator.TryParseId(idStr, out int id))
                    return "ID tugas tidak valid! Pastikan berupa angka positif.";

                if (!DateHelper.TryParseDate(deadlineStr, out DateTime deadline))
                    return "Format tanggal tidak valid! Gunakan format DD/MM/YYYY.";

                if (!InputValidator.IsValidDeadline(deadline))
                    return "Deadline tidak dapat diatur di masa lalu.";

                KategoriTugas kategori = kategoriIndex == 0 ? KategoriTugas.Akademik : KategoriTugas.NonAkademik;

                var getResponse = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (!getResponse.IsSuccessStatusCode)
                    return "Tugas tidak ditemukan!";

                var task = await getResponse.Content.ReadFromJsonAsync<Tugas>(_jsonOptions);
                task.Judul = judul;
                task.Deadline = deadline;
                task.Kategori = kategori;

                var updateResponse = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", task, _jsonOptions);
                if (updateResponse.IsSuccessStatusCode)
                {
                    var updatedTask = await updateResponse.Content.ReadFromJsonAsync<Tugas>(_jsonOptions);
                    return $"Tugas '{updatedTask.Judul}' berhasil diperbarui";
                }
                return $"Error: {updateResponse.StatusCode}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        
        // Automata & Enum : by bintang 
        
        // Define the enum at the class level
        private enum DeleteTaskState { Start, Validating, Deleting, Completed, Error }

        public async Task<string> DeleteTask(string idStr)
        {
            DeleteTaskState currentState = DeleteTaskState.Start;

            try
            {
                // Transition to Validating state
                currentState = DeleteTaskState.Validating;

                if (!InputValidator.TryParseId(idStr, out int id))
                {
                    currentState = DeleteTaskState.Error;
                    return "ID tugas tidak valid! Pastikan berupa angka positif.";
                }

                var getResponse = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (!getResponse.IsSuccessStatusCode)
                {
                    currentState = DeleteTaskState.Error;
                    return "Tugas tidak ditemukan!";
                }

                // Transition to Deleting state
                currentState = DeleteTaskState.Deleting;

                var task = await getResponse.Content.ReadFromJsonAsync<Tugas>(_jsonOptions);
                string judulTugas = task.Judul;

                var deleteResponse = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                if (deleteResponse.IsSuccessStatusCode)
                {
                    // Transition to Completed state
                    currentState = DeleteTaskState.Completed;
                    return $"Tugas '{judulTugas}' berhasil dihapus";
                }

                currentState = DeleteTaskState.Error;
                return $"Error: {deleteResponse.StatusCode}";
            }
            catch (Exception ex)
            {
                currentState = DeleteTaskState.Error;
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> GetTaskDetails(string idStr)
        {
            try
            {
                if (!InputValidator.TryParseId(idStr, out int id))
                    return "ID tugas tidak valid! Pastikan berupa angka positif.";

                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                    return "Tugas tidak ditemukan!";

                var tugas = await response.Content.ReadFromJsonAsync<Tugas>(_jsonOptions);

                var reminderSettings = _configProvider.GetConfig<Dictionary<string, object>>("ReminderSettings");
                if (reminderSettings == null)
                    return "Pengaturan pengingat deadline tidak ditemukan!";

                int daysBeforeDeadline = ((JsonElement)reminderSettings["DaysBeforeDeadline"]).GetInt32();

                string statusWarning = "";
                if (DateHelper.DaysUntilDeadline(tugas.Deadline) <= daysBeforeDeadline && tugas.Status != StatusTugas.Selesai)
                {
                    statusWarning = $"\nPeringatan: Deadline {DateHelper.DaysUntilDeadline(tugas.Deadline)} hari lagi!";
                }

                return $"Detail Tugas #{tugas.Id}:\n" +
                       $"Judul: {tugas.Judul}\n" +
                       $"Kategori: {tugas.Kategori}\n" +
                       $"Status: {tugas.Status}\n" +
                       $"Deadline: {DateHelper.FormatDate(tugas.Deadline)}" +
                       statusWarning;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> GetTasksByDateRange(string startDateStr, string endDateStr)
        {
            try
            {
                if (!DateHelper.TryParseDate(startDateStr, out DateTime startDate))
                    return "Tanggal mulai tidak valid! Gunakan format DD/MM/YYYY.";

                if (!DateHelper.TryParseDate(endDateStr, out DateTime endDate))
                    return "Tanggal akhir tidak valid! Gunakan format DD/MM/YYYY.";

                var response = await _httpClient.GetAsync($"{BaseUrl}/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
                if (!response.IsSuccessStatusCode)
                    return $"Error: {response.StatusCode}";

                var tasks = await response.Content.ReadFromJsonAsync<List<Tugas>>(_jsonOptions);
                if (tasks == null || !tasks.Any())
                    return "Tidak ada tugas yang ditemukan dalam rentang waktu tersebut.";

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Spectre.Console.Color.Blue)
                    .Title("[bold yellow]Daftar Tugas Berdasarkan Rentang Waktu[/]")
                    .AddColumn(new TableColumn("[bold]ID[/]").Centered())
                    .AddColumn(new TableColumn("[bold]Judul[/]").LeftAligned())
                    .AddColumn(new TableColumn("[bold]Kategori[/]").Centered())
                    .AddColumn(new TableColumn("[bold]Status[/]").Centered())
                    .AddColumn(new TableColumn("[bold]Deadline[/]").Centered());

                foreach (var t in tasks)
                {
                    table.AddRow(
                        t.Id.ToString(),
                        t.Judul.EscapeMarkup(),
                        t.Kategori.ToString(),
                        t.Status.ToString(),
                        DateHelper.FormatDate(t.Deadline)
                    );
                }

                AnsiConsole.Write(table);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> GetAllTasks()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                if (!response.IsSuccessStatusCode)
                    return $"[red]Error: {response.StatusCode}[/]";

                var tasks = await response.Content.ReadFromJsonAsync<List<Tugas>>(_jsonOptions);
                if (!tasks.Any())
                    return "[yellow]Tidak ada tugas yang tersedia.[/]";

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Spectre.Console.Color.Blue)
                    .Title("[bold yellow]Daftar Tugas[/]")
                    .AddColumn(new TableColumn("[bold]ID[/]").Centered())
                    .AddColumn(new TableColumn("[bold]Judul[/]").LeftAligned())
                    .AddColumn(new TableColumn("[bold]Kategori[/]").Centered())
                    .AddColumn(new TableColumn("[bold]Status[/]").Centered())
                    .AddColumn(new TableColumn("[bold]Deadline[/]").Centered());

                foreach (var t in tasks)
                {
                    var deadlineText = DateHelper.FormatDate(t.Deadline);
                    var statusText = t.Status.ToString();

                    if (DateHelper.IsDeadlineApproaching(t.Deadline))
                    {
                        if (t.Status != StatusTugas.Selesai && t.Status != StatusTugas.Terlewat)
                        {
                            deadlineText = $"[bold yellow on red]{deadlineText} ⚠️[/]";
                        }
                    }

                    switch (t.Status)
                    {
                        case StatusTugas.BelumMulai:
                            statusText = $"[grey]{statusText}[/]";
                            break;
                        case StatusTugas.SedangDikerjakan:
                            statusText = $"[blue]{statusText}[/]";
                            break;
                        case StatusTugas.Selesai:
                            statusText = $"[green]{statusText}[/]";
                            break;
                        case StatusTugas.Terlewat:
                            statusText = $"[red]{statusText}[/]";
                            break;
                    }

                    table.AddRow(
                        t.Id.ToString(),
                        t.Judul.EscapeMarkup(),
                        t.Kategori.ToString(),
                        statusText,
                        deadlineText
                    );
                }

                AnsiConsole.Write(table);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"[red]Error: {ex.Message.EscapeMarkup()}[/]";
            }
        }

        public async Task<string> PrintTasksToFilesFromApi(string jsonFilePath, string textFilePath)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Mengakses API di: {BaseUrl}");

                var response = await _httpClient.GetAsync(BaseUrl);
                if (!response.IsSuccessStatusCode)
                    return $"[ERROR] Gagal mengambil data dari API. Status code: {response.StatusCode}";

                List<Tugas>? tasks = await response.Content.ReadFromJsonAsync<List<Tugas>>(_jsonOptions);
                if (tasks == null || !tasks.Any())
                    return "[INFO] Tidak ada tugas yang tersedia untuk dicetak.";

                var jsonContent = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonFilePath, jsonContent);

                var textContent = JsonToTextConverter.ConvertTasksToText(tasks);
                File.WriteAllText(textFilePath, textContent);

                return $"[INFO] Daftar tugas berhasil dicetak ke file JSON: {jsonFilePath} dan file TXT: {textFilePath}.";
            }
            catch (Exception ex)
            {
                return $"[ERROR] Gagal mencetak daftar tugas: {ex.Message}";
            }
        }
    }
}
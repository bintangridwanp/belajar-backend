using Tubes_KPL.src.Domain.Interfaces;
using Tubes_KPL.src.Domain.Models;

namespace Tubes_KPL.src.Application.Services
{
    public class TaskService
    {
        private readonly ITugasRepository _repository;

        public TaskService(ITugasRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        //Bintang : poin 1 Defensive Programming + Logging
        public Tugas BuatTugas(string judul, DateTime deadline, KategoriTugas kategori)
        {
            if (string.IsNullOrWhiteSpace(judul))
            {
                Console.WriteLine($"[ERROR] Judul tugas tidak boleh kosong. Input: {judul}");
                throw new ArgumentException("Judul tugas tidak boleh kosong", nameof(judul));
            }

            if (deadline < DateTime.Now)
            {
                Console.WriteLine($"[ERROR] Deadline tidak boleh di masa lalu. Input: {deadline}");
                throw new ArgumentException("Deadline tidak boleh di masa lalu", nameof(deadline));
            }

            Console.WriteLine($"[INFO] Membuat tugas baru dengan judul '{judul}' dan deadline '{deadline}'.");

            var tugas = new Tugas
            {
                Judul = judul,
                Deadline = deadline,
                Status = StatusTugas.BelumMulai,
                Kategori = kategori
            };

            _repository.Tambah(tugas);
            return tugas;
        }

        public Tugas UbahStatusTugas(int id, StatusTugas status)
        {
            var tugas = _repository.AmbilById(id);
            if (tugas == null)
                throw new KeyNotFoundException($"Tugas dengan ID {id} tidak ditemukan");

            if (!IsValidStatusTransition(tugas.Status, status))
                throw new InvalidOperationException($"Transisi status dari {tugas.Status} ke {status} tidak valid");

            if (tugas.Deadline < DateTime.Now && status != StatusTugas.Terlewat)
            {
                tugas.Status = StatusTugas.Terlewat;
            }
            else
            {
                tugas.Status = status;
            }

            _repository.Perbarui(tugas);
            return tugas;
        }

        // bintang : poin 3 automata
        private bool IsValidStatusTransition(StatusTugas currentStatus, StatusTugas newStatus)
        {
            bool isValid = false;
            switch (currentStatus)
            {
                case StatusTugas.BelumMulai:
                    isValid = newStatus == StatusTugas.SedangDikerjakan ||
                              newStatus == StatusTugas.Selesai ||
                              newStatus == StatusTugas.Terlewat;
                    break;

                case StatusTugas.SedangDikerjakan:
                    isValid = newStatus == StatusTugas.Selesai ||
                              newStatus == StatusTugas.Terlewat;
                    break;

                case StatusTugas.Selesai:
                    isValid = newStatus == StatusTugas.Terlewat;
                    break;

                case StatusTugas.Terlewat:
                    isValid = false;
                    break;
            }
            Console.WriteLine($"[LOG] Transisi dari {currentStatus} ke {newStatus} " +
                              (isValid ? "valid." : "tidak valid."));

            return isValid;
        }

        public Tugas PerbaruiTugas(int id, string judul, DateTime deadline, KategoriTugas kategori)
        {
            var tugas = _repository.AmbilById(id);
            if (tugas == null)
                throw new KeyNotFoundException($"Tugas dengan ID {id} tidak ditemukan");

            if (string.IsNullOrWhiteSpace(judul))
                throw new ArgumentException("Judul tugas tidak boleh kosong", nameof(judul));

            tugas.Judul = judul;
            tugas.Deadline = deadline;
            tugas.Kategori = kategori;

            if (deadline < DateTime.Now && tugas.Status != StatusTugas.Selesai)
            {
                tugas.Status = StatusTugas.Terlewat;
            }

            _repository.Perbarui(tugas);
            return tugas;
        }

        public void HapusTugas(int id)
        {
            var tugas = _repository.AmbilById(id);
            if (tugas == null)
                throw new KeyNotFoundException($"Tugas dengan ID {id} tidak ditemukan");

            _repository.Hapus(id);
        }

        public IEnumerable<Tugas> AmbilSemuaTugas()
        {
            return _repository.AmbilSemua();
        }

        public Tugas AmbilTugasById(int id)
        {
            var tugas = _repository.AmbilById(id);
            if (tugas == null)
                throw new KeyNotFoundException($"Tugas dengan ID {id} tidak ditemukan");

            return tugas;
        }

        public void PerbaruiStatusTerlewat()
        {
            var tugasList = _repository.AmbilSemua().ToList();
            var now = DateTime.Now;

            foreach (var tugas in tugasList)
            {
                if (tugas.Deadline < now && tugas.Status != StatusTugas.Terlewat && tugas.Status != StatusTugas.Selesai)
                {
                    tugas.Status = StatusTugas.Terlewat;
                    _repository.Perbarui(tugas);
                }
            }
        }
    }
}
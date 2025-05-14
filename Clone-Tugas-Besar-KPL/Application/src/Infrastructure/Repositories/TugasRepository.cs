using System.Text.Json;
using Tubes_KPL.src.Domain.Interfaces;
using Tubes_KPL.src.Domain.Models;

namespace Tubes_KPL.src.Infrastructure.Repositories
{
    public class TugasRepository : ITugasRepository
    {
        private readonly string _filePath = "../../../API/Storage/Tugas.json";
        private List<Tugas> _tugasList;

        public TugasRepository()
        {
            if (File.Exists(_filePath))
            {
                var jsonContent = File.ReadAllText(_filePath);
                _tugasList = JsonSerializer.Deserialize<List<Tugas>>(jsonContent) ?? new List<Tugas>();
            }
            else
            {
                _tugasList = new List<Tugas>();
            }
        }

        public IEnumerable<Tugas> AmbilSemua()
        {
            return _tugasList;
        }

        public Tugas AmbilById(int id)
        {
            return _tugasList.FirstOrDefault(t => t.Id == id);
        }

        public void Tambah(Tugas tugas)
        {
            tugas.Id = _tugasList.Count > 0 ? _tugasList.Max(t => t.Id) + 1 : 1;
            _tugasList.Add(tugas);
            SimpanKeFile();
        }

        public void Perbarui(Tugas tugas)
        {
            var existingTugas = AmbilById(tugas.Id);
            if (existingTugas != null)
            {
                existingTugas.Judul = tugas.Judul;
                existingTugas.Deadline = tugas.Deadline;
                existingTugas.Status = tugas.Status;
                existingTugas.Kategori = tugas.Kategori;
                SimpanKeFile();
            }
        }

        public void Hapus(int id)
        {
            var tugas = AmbilById(id);
            if (tugas != null)
            {
                _tugasList.Remove(tugas);
                SimpanKeFile();
            }
        }

        private void SimpanKeFile()
        {
            var jsonContent = JsonSerializer.Serialize(_tugasList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, jsonContent);
        }
    }
}
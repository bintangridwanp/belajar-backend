using Tubes_KPL.src.Domain.Models;
using System.Collections.Generic;

namespace Tubes_KPL.src.Domain.Interfaces
{
    public interface ITugasRepository
    {
        void Tambah(Tugas tugas);

        void Perbarui(Tugas tugas);

        void Hapus(int id);

        Tugas AmbilById(int id);

        IEnumerable<Tugas> AmbilSemua();
    }
}
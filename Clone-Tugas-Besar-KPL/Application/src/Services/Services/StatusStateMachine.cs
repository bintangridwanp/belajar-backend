using Tubes_KPL.src.Domain.Models;

namespace Tubes_KPL.src.Application.Services
{
    public static class StatusStateMachine
    {
        // Automata code by zuhri
        private static readonly Dictionary<StatusTugas, StatusTugas[]> transitions = new()
        {
            { StatusTugas.BelumMulai, new[] { StatusTugas.SedangDikerjakan } },
            { StatusTugas.SedangDikerjakan, new[] { StatusTugas.Selesai, StatusTugas.Terlewat } },
            { StatusTugas.Selesai, new[] { StatusTugas.SedangDikerjakan } },
            { StatusTugas.Terlewat, new[] { StatusTugas.SedangDikerjakan } }
        };

        public static bool CanTransition(StatusTugas from, StatusTugas to)
            => transitions.ContainsKey(from) && transitions[from].Contains(to);
    }

}
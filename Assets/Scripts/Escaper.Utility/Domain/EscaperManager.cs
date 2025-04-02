using System.Collections.Generic;
using Escaper.Utility.Domain.Status;

namespace Escaper.Utility.Domain
{
    public class EscaperManager
    {
        private static readonly EscaperManager _instance = new EscaperManager();

        public static EscaperManager Instance { get => _instance; }

        private static List<StatusHolderBase> statusHolderBases = new List<StatusHolderBase>();

        public void Resolve(StatusHolderBase statusHolder)
        {

        }
    }
}
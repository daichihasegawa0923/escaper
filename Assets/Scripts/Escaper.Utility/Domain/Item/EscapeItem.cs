using System.Collections.Generic;

namespace Escaper.Utility.Domain.Item
{
    public class EscapeItem : IdentifiedDomain
    {
        public EscapeItem(string id, string name) : base(id)
        {
            Name = name;
        }

        public string Name { get; }

        public List<EscapeItemImage> Images { get; } = new List<EscapeItemImage>();

    }
}
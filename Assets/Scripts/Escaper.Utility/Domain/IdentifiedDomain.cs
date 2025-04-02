#nullable enable

using System;

namespace Escaper.Utility.Domain
{
    public abstract class IdentifiedDomain
    {
        public string Id { get; }

        public IdentifiedDomain(string? id)
        {
            Id = id ?? Guid.NewGuid().ToString();
        }
    }
}
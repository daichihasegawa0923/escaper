#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Escaper.Utility.Domain.Status
{
    public abstract class StatusHolderBase : IdentifiedDomain
    {

        protected StatusHolderBase(string? id) : base(id) { }

        public abstract List<StatusValue> Statuses { get; }

        private List<string> StatusIds { get => Statuses.Select(x => x.Id).ToList(); }

        private StatusValue? _current;

        public StatusValue Current { get => _current != null ? _current : Statuses.FirstOrDefault(); private set => _current = value; }

        public void Set(StatusValue status)
        {
            if (!Statuses.Contains(status))
            {
                throw new Exception();
            }
            _current = status;
        }

        public void Set(string id)
        {
            if (!StatusIds.Contains(id))
            {
                throw new Exception();
            }
            var target = Statuses.Where(status => Equals(status.Id, id)).FirstOrDefault();
            if (target == null)
            {
                throw new Exception();
            }
            Current = target;
        }

        public bool ToNext()
        {
            if (Current.Next == null)
            {
                return false;
            }
            Current = Current.Next;
            return true;
        }

        protected abstract void Enable(StatusValue status);

        protected abstract void Disable(StatusValue status);
    }
}
#nullable enable

using System;
using UnityEngine;

namespace Escaper.Utility.Domain.Status
{
    [Serializable]
    public class StatusValue
    {
        public StatusValue()
        {
            _id = Guid.NewGuid().ToString();
        }

        [SerializeField]
        private string _id;
        public string Id { get => _id; }

        [SerializeField]
        private string _name = "";
        public string Name { get => _name; }

        [SerializeField]
        public StatusValue? _next;
        public StatusValue? Next { get => _next; }
    }
}
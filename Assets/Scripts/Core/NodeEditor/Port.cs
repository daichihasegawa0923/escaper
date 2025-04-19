using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public class Port
    {
        public string Name { get; set; }
        public Node Node { get; private set; }
        public bool IsInput { get; private set; }
        public object Value { get; set; }

        public Port(string name, Node node, bool isInput)
        {
            Name = name;
            Node = node;
            IsInput = isInput;
            Value = null;
        }
    }
}
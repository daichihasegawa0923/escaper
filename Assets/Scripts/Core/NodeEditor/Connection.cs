using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public class Connection
    {
        public Port SourcePort { get; private set; }
        public Port TargetPort { get; private set; }
        public Node SourceNode => SourcePort?.Node;
        public Node TargetNode => TargetPort?.Node;

        public Connection(Port source, Port target)
        {
            SourcePort = source;
            TargetPort = target;
        }

        public bool IsValid()
        {
            if (SourcePort == null || TargetPort == null) return false;
            if (SourceNode == TargetNode) return false;
            if (SourcePort.IsInput == TargetPort.IsInput) return false;
            return true;
        }
    }
}
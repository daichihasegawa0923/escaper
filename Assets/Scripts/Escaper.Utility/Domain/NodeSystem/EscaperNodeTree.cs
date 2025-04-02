using System.Collections.Generic;
using System.Linq;
using DISystem;
using Escaper.Utility.Domain.DomainService;

namespace Escaper.Utility.Domain.NodeSystem
{
    public class EscaperNodeTree
    {
        private readonly string _name;
        public string Name { get => _name; }
        private readonly List<EscaperNode> _nodes = new List<EscaperNode>();

        public EscaperNodeTree(string name, List<EscaperNode> nodes)
        {
            _name = name;
            _nodes.AddRange(nodes);
        }

        public void AddNode(EscaperNode node)
        {
            _nodes.Add(node);
        }

        public void RestoreStatus(EscaperNode leafNode)
        {
            var list = EscaperNode.GetNodesRootToLeaf(leafNode);
            list.ForEach(node =>
            {
                node.StatusStates.ToList().ForEach(state =>
                {
                    state.Key.Set(state.Value);
                });
            });
        }
    }
}
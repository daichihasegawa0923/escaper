#nullable enable

using System.Collections.Generic;
using Escaper.Utility.Domain.Status;

namespace Escaper.Utility.Domain.NodeSystem
{
    public class EscaperNode : IdentifiedDomain
    {
        public EscaperNode(
            string? id,
            Dictionary<StatusHolderBase, StatusValue> statusStates,
            EscaperNode? parent) : base(id)
        {
            StatusStates = statusStates;
            Parent = parent;
        }

        public static EscaperNode GetRoot(EscaperNode node)
        {
            return node.Parent == null ? node : GetRoot(node.Parent);
        }

        public static List<EscaperNode> GetNodesRootToLeaf(EscaperNode leafNode)
        {
            return GetNodesRootToLeaf(leafNode, new List<EscaperNode>());
        }

        private static List<EscaperNode> GetNodesRootToLeaf(EscaperNode leafNode, List<EscaperNode> list)
        {
            list.Insert(0, leafNode);
            if (leafNode.Parent == null)
            {
                return list;
            }
            return GetNodesRootToLeaf(leafNode.Parent, list);
        }

        public EscaperNode? Parent { get; }

        public Dictionary<StatusHolderBase, StatusValue> StatusStates { get; }
    }
}
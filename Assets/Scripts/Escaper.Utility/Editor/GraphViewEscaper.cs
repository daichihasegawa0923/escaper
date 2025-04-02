using UnityEditor.Experimental.GraphView;

namespace Escaper.Utility.Editor
{
    public class GraphViewEscaper : GraphView
    {
        public GraphViewEscaper()
        {
            var node = new Node
            {
                title = "hello",
                style = {
                    top = 20,
                    left = 20
                }

            };
            node.RefreshExpandedState();
            node.RefreshPorts();
            AddElement(node);
        }
    }
}
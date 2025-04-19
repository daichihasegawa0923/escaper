using UnityEditor;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public class StartNode : Node
    {
        private static readonly Color NODE_COLOR = new Color(0.4f, 0.6f, 0.2f, 0.8f);

        public StartNode(string name) : base(name)
        {
            Position = new Vector2(50, 100);
        }

        public override void DrawNode()
        {
            // ノードの背景を描画
            Rect nodeRect = new Rect(Position.x, Position.y, NODE_WIDTH, GetNodeHeight());
            EditorGUI.DrawRect(nodeRect, NODE_COLOR);

            // ノードのヘッダーを描画
            Rect headerRect = new Rect(Position.x, Position.y, NODE_WIDTH, HEADER_HEIGHT);
            EditorGUI.DrawRect(headerRect, new Color(0.3f, 0.3f, 0.3f, 0.8f));
            GUI.Label(headerRect, Name, new GUIStyle { alignment = TextAnchor.MiddleCenter });
        }

        protected override void InitializePorts()
        {
            OutputPorts.Add(new Port("Output", this, false));
        }

        protected override float GetNodeHeight()
        {
            return HEADER_HEIGHT + FIELD_MARGIN * 2;
        }

        public override Color GetNodeColor()
        {
            return NODE_COLOR;
        }
    }
}
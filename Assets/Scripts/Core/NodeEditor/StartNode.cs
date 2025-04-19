using UnityEditor;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public class StartNode : Node
    {
        private static readonly Color NodeColor = new Color(0.2f, 0.8f, 0.2f);

        public StartNode(string name, Vector2 position) : base(name, position)
        {
        }

        public override void DrawNode()
        {
            UpdateNodeRect();

            // 背景色を設定
            Color originalColor = GUI.color;
            GUI.color = GetNodeColor();
            GUI.Box(NodeRect, "");
            GUI.color = originalColor;

            // ヘッダー
            Rect headerRect = new Rect(Position.x, Position.y, 200f, 20f);
            GUI.Box(headerRect, Name);

            // ポートの描画
            DrawPorts();
        }

        private void DrawPorts()
        {
            // 出力ポートの描画
            for (int i = 0; i < OutputPorts.Count; i++)
            {
                float y = Position.y + 30 + i * 25f;
                GUI.Box(new Rect(Position.x + 175, y, 20, 20), "→");
                GUI.Label(new Rect(Position.x + 70, y, 100, 20), OutputPorts[i].Name);
            }
        }

        public override Color GetNodeColor()
        {
            return NodeColor;
        }

        protected override void InitializePorts()
        {
            AddOutputPort("Next");
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public class StatusNode : Node
    {
        private static readonly Color NodeColor = new Color(0.7f, 0.7f, 0.7f);
        public string Status { get; set; }

        public StatusNode(string name, Vector2 position) : base(name, position)
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

            float currentY = Position.y + 25f;

            // GameObjectフィールド
            Rect objectFieldRect = new Rect(Position.x + 5f, currentY, 190f, 20f);
            TargetObject = (GameObject)EditorGUI.ObjectField(
                objectFieldRect,
                "Target",
                TargetObject,
                typeof(GameObject),
                true
            );
            currentY += 25f;

            // Statusフィールド
            Rect statusFieldRect = new Rect(Position.x + 5f, currentY, 190f, 20f);
            Status = EditorGUI.TextField(statusFieldRect, "Status", Status);
            currentY += 25f;

            // ポートの描画
            DrawPorts(currentY);
        }

        private void DrawPorts(float startY)
        {
            // 入力ポートの描画
            for (int i = 0; i < InputPorts.Count; i++)
            {
                float y = startY + i * 25f;
                GUI.Box(new Rect(Position.x + 5, y, 20, 20), "←");
                GUI.Label(new Rect(Position.x + 30, y, 100, 20), InputPorts[i].Name);
            }

            // 出力ポートの描画
            for (int i = 0; i < OutputPorts.Count; i++)
            {
                float y = startY + i * 25f;
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
            AddInputPort("Input");
            AddOutputPort("Output");
        }
    }
}
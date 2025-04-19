using UnityEditor;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public class StatusNode : Node
    {
        private static readonly Color NODE_COLOR = new Color(0.2f, 0.4f, 0.6f, 0.8f);
        public string Status { get; private set; }

        public StatusNode(string name) : base(name)
        {
            Position = new Vector2(100, 100);
            Status = "New Status";
        }

        protected override void InitializeRows()
        {
            base.InitializeRows();
            AddRow(new StatusRow(Status, OnStatusChanged));
        }

        private void OnStatusChanged(string newStatus)
        {
            Status = newStatus;
        }

        protected override void InitializePorts()
        {
            InputPorts.Add(new Port("Input", this, true));
            OutputPorts.Add(new Port("Output", this, false));
        }

        public override Color GetNodeColor()
        {
            return NODE_COLOR;
        }

        public override void DrawNode()
        {
            UpdateNodeRect();

            // ノードの背景を描画
            EditorGUI.DrawRect(NodeRect, GetNodeColor());

            // ノードのヘッダーを描画
            Rect headerRect = new Rect(Position.x, Position.y, NODE_WIDTH, HEADER_HEIGHT);
            EditorGUI.DrawRect(headerRect, new Color(0.3f, 0.3f, 0.3f, 0.8f));
            GUI.Label(headerRect, Name, new GUIStyle { alignment = TextAnchor.MiddleCenter });

            float currentY = Position.y + HEADER_HEIGHT + FIELD_MARGIN;

            // GameObjectフィールド
            Rect objectFieldRect = new Rect(Position.x + FIELD_MARGIN, currentY, NODE_WIDTH - FIELD_MARGIN * 2, FIELD_HEIGHT);
            EditorGUI.DrawRect(objectFieldRect, new Color(0.25f, 0.25f, 0.25f, 0.8f));
            GameObject = (GameObject)EditorGUI.ObjectField(objectFieldRect, GameObject, typeof(GameObject), true);
            currentY += FIELD_HEIGHT + FIELD_MARGIN;

            // Statusフィールド
            Rect statusFieldRect = new Rect(Position.x + FIELD_MARGIN, currentY, NODE_WIDTH - FIELD_MARGIN * 2, FIELD_HEIGHT);
            EditorGUI.DrawRect(statusFieldRect, new Color(0.25f, 0.25f, 0.25f, 0.8f));
            Status = EditorGUI.TextField(statusFieldRect, "Status", Status);
        }

        protected override float GetNodeHeight()
        {
            return HEADER_HEIGHT + FIELD_MARGIN * 2 + FIELD_HEIGHT * 2 + Mathf.Max(InputPorts.Count, OutputPorts.Count) * PORT_SPACING;
        }
    }
}
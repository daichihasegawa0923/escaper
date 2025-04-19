using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public class Node
    {
        public const float NODE_WIDTH = 200f;
        public const float HEADER_HEIGHT = 30f;
        public const float FIELD_HEIGHT = 25f;
        public const float FIELD_MARGIN = 5f;
        public const float PORT_SPACING = 30f;

        public string Id { get; private set; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public List<Port> InputPorts { get; private set; }
        public List<Port> OutputPorts { get; private set; }
        public GameObject GameObject { get; set; }
        public Rect NodeRect { get; private set; }

        protected List<NodeRow> _rows;

        public Node(string name)
        {
            Id = System.Guid.NewGuid().ToString();
            Name = name;
            Position = Vector2.zero;
            InputPorts = new List<Port>();
            OutputPorts = new List<Port>();
            _rows = new List<NodeRow>();
            InitializeRows();
            InitializePorts();
            UpdateNodeRect();
        }

        protected virtual void InitializeRows()
        {
            if (GameObject != null)
            {
                _rows.Add(new GameObjectRow(GameObject, OnGameObjectChanged));
            }
        }

        protected virtual void OnGameObjectChanged(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        protected virtual void InitializePorts()
        {
            // サブクラスで実装
        }

        public void UpdateNodeRect()
        {
            NodeRect = new Rect(Position.x, Position.y, NODE_WIDTH, GetNodeHeight());
        }

        protected virtual float GetNodeHeight()
        {
            float height = HEADER_HEIGHT;
            foreach (var row in _rows)
            {
                height += row.Height + row.Margin;
            }
            height += Mathf.Max(InputPorts.Count, OutputPorts.Count) * (NodeRow.DEFAULT_HEIGHT + NodeRow.DEFAULT_MARGIN);
            return height;
        }

        public virtual void DrawNode()
        {
            UpdateNodeRect();

            // ノードの背景を描画
            EditorGUI.DrawRect(NodeRect, GetNodeColor());

            // ノードのヘッダーを描画
            Rect headerRect = new Rect(Position.x, Position.y, NODE_WIDTH, HEADER_HEIGHT);
            EditorGUI.DrawRect(headerRect, new Color(0.3f, 0.3f, 0.3f, 0.8f));
            GUI.Label(headerRect, Name, new GUIStyle { alignment = TextAnchor.MiddleCenter });

            // 行を描画
            float currentY = Position.y + HEADER_HEIGHT;
            foreach (var row in _rows)
            {
                currentY += row.Margin;
                Rect rowRect = new Rect(Position.x + 5, currentY, NODE_WIDTH - 10, row.Height);
                row.Draw(rowRect);
                currentY += row.Height;
            }

            // ポートを描画
            currentY += NodeRow.DEFAULT_MARGIN;
            for (int i = 0; i < Mathf.Max(InputPorts.Count, OutputPorts.Count); i++)
            {
                currentY += NodeRow.DEFAULT_MARGIN;
                if (i < InputPorts.Count)
                {
                    Rect portRect = new Rect(Position.x + 5, currentY, NodeRow.DEFAULT_HEIGHT, NodeRow.DEFAULT_HEIGHT);
                    new PortRow(InputPorts[i], true).Draw(portRect);
                }
                if (i < OutputPorts.Count)
                {
                    Rect portRect = new Rect(Position.x + NODE_WIDTH - NodeRow.DEFAULT_HEIGHT - 5, currentY, NodeRow.DEFAULT_HEIGHT, NodeRow.DEFAULT_HEIGHT);
                    new PortRow(OutputPorts[i], false).Draw(portRect);
                }
                currentY += NodeRow.DEFAULT_HEIGHT;
            }
        }

        public virtual Color GetNodeColor()
        {
            return new Color(0.2f, 0.2f, 0.2f, 0.8f);
        }

        protected void AddRow(NodeRow row)
        {
            _rows.Add(row);
            UpdateNodeRect();
        }
    }
}
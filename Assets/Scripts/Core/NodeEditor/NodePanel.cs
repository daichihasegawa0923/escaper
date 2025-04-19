using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public class NodePanel
    {
        private const float NODE_WIDTH = 200f;
        private const float NODE_HEIGHT = 120f;
        private const float PORT_HEIGHT = 20f;
        private const float PORT_SPACING = 25f;
        private const float HEADER_HEIGHT = 20f;
        private const float FIELD_HEIGHT = 20f;
        private const float FIELD_MARGIN = 5f;

        private static readonly Color StartNodeColor = new Color(0.2f, 0.8f, 0.2f);
        private static readonly Color StatusNodeColor = new Color(0.7f, 0.7f, 0.7f);
        private static readonly Color PuzzleNodeColor = new Color(0.2f, 0.4f, 0.8f);

        private List<Node> _nodes;
        private List<Connection> _connections;
        private Node _selectedNode;
        private Port _startPort;
        private bool _isConnecting;

        public NodePanel(List<Node> nodes, List<Connection> connections)
        {
            _nodes = nodes ?? new List<Node>();
            _connections = connections ?? new List<Connection>();
        }

        public void Draw()
        {
            DrawConnections();
            DrawNodes();
            DrawConnectionPreview();
        }

        private void DrawNodes()
        {
            foreach (var node in _nodes)
            {
                DrawNode(node);
            }
        }

        private void DrawNode(Node node)
        {
            node.UpdateNodeRect();

            // ノードの背景色を設定
            Color originalColor = GUI.color;
            GUI.color = GetNodeColor(node.Type);
            GUI.Box(node.NodeRect, "");
            GUI.color = originalColor;

            // ヘッダー
            Rect headerRect = new Rect(node.Position.x, node.Position.y, NODE_WIDTH, HEADER_HEIGHT);
            GUI.Box(headerRect, node.Name);

            float currentY = node.Position.y + HEADER_HEIGHT + FIELD_MARGIN;

            // GameObjectフィールド（Startノード以外）
            if (node.Type != NodeType.Start)
            {
                Rect objectFieldRect = new Rect(
                    node.Position.x + FIELD_MARGIN,
                    currentY,
                    NODE_WIDTH - FIELD_MARGIN * 2,
                    FIELD_HEIGHT
                );
                EditorGUI.BeginChangeCheck();
                GameObject newTarget = (GameObject)EditorGUI.ObjectField(
                    objectFieldRect,
                    "Target",
                    node.TargetObject,
                    typeof(GameObject),
                    true
                );
                if (EditorGUI.EndChangeCheck())
                {
                    node.TargetObject = newTarget;
                    GUI.changed = true;
                }
                currentY += FIELD_HEIGHT + FIELD_MARGIN;
            }

            // ステータスフィールド（Statusノードのみ）
            if (node.Type == NodeType.Status)
            {
                Rect statusFieldRect = new Rect(
                    node.Position.x + FIELD_MARGIN,
                    currentY,
                    NODE_WIDTH - FIELD_MARGIN * 2,
                    FIELD_HEIGHT
                );
                EditorGUI.BeginChangeCheck();
                string newStatus = EditorGUI.TextField(statusFieldRect, "Status", node.Status);
                if (EditorGUI.EndChangeCheck())
                {
                    node.Status = newStatus;
                    GUI.changed = true;
                }
                currentY += FIELD_HEIGHT + FIELD_MARGIN;
            }

            float portStartY = currentY;

            // 入力ポートを描画
            for (int i = 0; i < node.InputPorts.Count; i++)
            {
                Rect portRect = new Rect(
                    node.Position.x + 5,
                    portStartY + i * PORT_SPACING,
                    20,
                    PORT_HEIGHT
                );
                GUI.Box(portRect, "←");

                // ポート名を表示
                Rect labelRect = new Rect(
                    portRect.x + portRect.width + 5,
                    portRect.y,
                    100,
                    PORT_HEIGHT
                );
                GUI.Label(labelRect, node.InputPorts[i].Name);
            }

            // 出力ポートを描画
            for (int i = 0; i < node.OutputPorts.Count; i++)
            {
                Rect portRect = new Rect(
                    node.Position.x + NODE_WIDTH - 25,
                    portStartY + i * PORT_SPACING,
                    20,
                    PORT_HEIGHT
                );
                GUI.Box(portRect, "→");

                // ポート名を表示
                Rect labelRect = new Rect(
                    portRect.x - 105,
                    portRect.y,
                    100,
                    PORT_HEIGHT
                );
                GUI.Label(labelRect, node.OutputPorts[i].Name);
            }
        }

        private Color GetNodeColor(NodeType type)
        {
            switch (type)
            {
                case NodeType.Start:
                    return StartNodeColor;
                case NodeType.Status:
                    return StatusNodeColor;
                case NodeType.Puzzle:
                    return PuzzleNodeColor;
                default:
                    return Color.white;
            }
        }

        private void DrawConnections()
        {
            foreach (var connection in _connections)
            {
                Vector2 startPos = GetPortPosition(connection.SourcePort);
                Vector2 endPos = GetPortPosition(connection.TargetPort);
                DrawBezier(startPos, endPos);
            }
        }

        private void DrawConnectionPreview()
        {
            if (_isConnecting && _startPort != null)
            {
                Vector2 startPos = GetPortPosition(_startPort);
                Vector2 endPos = Event.current.mousePosition;
                DrawBezier(startPos, endPos);
            }
        }

        private void DrawBezier(Vector2 start, Vector2 end)
        {
            Vector2 startTangent = start + Vector2.right * 50f;
            Vector2 endTangent = end - Vector2.right * 50f;
            Handles.DrawBezier(start, end, startTangent, endTangent, Color.white, null, 1f);
        }

        private Vector2 GetPortPosition(Port port)
        {
            float x = port.IsInput ? port.Node.Position.x + 5 : port.Node.Position.x + NODE_WIDTH - 25;
            float y = port.Node.Position.y + 30 + (port.IsInput ? port.Node.InputPorts : port.Node.OutputPorts).IndexOf(port) * PORT_SPACING;
            return new Vector2(x, y);
        }

        public void HandleEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    HandleMouseDown(e);
                    break;
                case EventType.MouseDrag:
                    HandleMouseDrag(e);
                    break;
                case EventType.MouseUp:
                    HandleMouseUp(e);
                    break;
            }
        }

        private void HandleMouseDown(Event e)
        {
            if (e.button == 0) // 左クリック
            {
                _selectedNode = _nodes.Find(n => new Rect(n.Position.x, n.Position.y, NODE_WIDTH, NODE_HEIGHT).Contains(e.mousePosition));
                if (_selectedNode != null)
                {
                    _startPort = FindPortAtPosition(_selectedNode, e.mousePosition);
                    if (_startPort != null)
                    {
                        _isConnecting = true;
                        e.Use();
                    }
                }
            }
        }

        private void HandleMouseDrag(Event e)
        {
            if (_selectedNode != null)
            {
                _selectedNode.Position = e.mousePosition;
                GUI.changed = true;
            }
        }

        private void HandleMouseUp(Event e)
        {
            if (_isConnecting && _startPort != null)
            {
                Port targetPort = FindPortAtPosition(null, e.mousePosition);
                if (targetPort != null && targetPort != _startPort && CanConnect(_startPort, targetPort))
                {
                    _connections.Add(new Connection(_startPort, targetPort));
                }
            }

            _isConnecting = false;
            _startPort = null;
            GUI.changed = true;
        }

        private Port FindPortAtPosition(Node node, Vector2 position)
        {
            if (node == null)
            {
                node = _nodes.Find(n => new Rect(n.Position.x, n.Position.y, NODE_WIDTH, NODE_HEIGHT).Contains(position));
            }

            if (node != null)
            {
                // 入力ポートをチェック
                for (int i = 0; i < node.InputPorts.Count; i++)
                {
                    Rect portRect = new Rect(
                        node.Position.x + 5,
                        node.Position.y + 30 + i * PORT_SPACING,
                        20,
                        PORT_HEIGHT
                    );
                    if (portRect.Contains(position))
                    {
                        return node.InputPorts[i];
                    }
                }

                // 出力ポートをチェック
                for (int i = 0; i < node.OutputPorts.Count; i++)
                {
                    Rect portRect = new Rect(
                        node.Position.x + NODE_WIDTH - 25,
                        node.Position.y + 30 + i * PORT_SPACING,
                        20,
                        PORT_HEIGHT
                    );
                    if (portRect.Contains(position))
                    {
                        return node.OutputPorts[i];
                    }
                }
            }

            return null;
        }

        private bool CanConnect(Port start, Port end)
        {
            if (start == null || end == null) return false;
            if (start.Node == end.Node) return false;
            if (start.IsInput == end.IsInput) return false;
            if (_connections == null) return true;
            return !_connections.Any(c => c.SourcePort == start && c.TargetPort == end);
        }
    }
}
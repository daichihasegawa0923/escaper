using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public abstract class BaseNodePanel<T> where T : Node
    {
        protected const float NODE_WIDTH = 200f;
        protected const float NODE_HEIGHT = 120f;
        protected const float PORT_HEIGHT = 20f;
        protected const float PORT_SPACING = 25f;
        protected const float HEADER_HEIGHT = 20f;
        protected const float FIELD_HEIGHT = 20f;
        protected const float FIELD_MARGIN = 5f;
        protected const float PORT_HIGHLIGHT_SIZE = 24f;
        protected const float CONNECTION_LINE_WIDTH = 2f;
        protected const float CONNECTION_CURVE = 50f;

        protected List<T> _nodes;
        protected List<Connection> _connections;
        protected T _selectedNode;
        protected Port _startPort;
        protected bool _isConnecting;
        protected Port _hoveredPort;
        protected Connection _selectedConnection;
        protected Vector2 _dragOffset;

        protected BaseNodePanel(List<T> nodes, List<Connection> connections)
        {
            _nodes = nodes ?? new List<T>();
            _connections = connections ?? new List<Connection>();
        }

        public void Draw()
        {
            DrawConnections();
            DrawNodes();
            DrawConnectionPreview();
            DrawPortHighlights();
        }

        protected virtual void DrawNodes()
        {
            foreach (var node in _nodes)
            {
                DrawNode(node);
            }
        }

        protected abstract void DrawNode(T node);

        protected virtual void DrawConnections()
        {
            foreach (var connection in _connections)
            {
                Vector2 startPos = GetPortPosition(connection.SourcePort);
                Vector2 endPos = GetPortPosition(connection.TargetPort);

                // 接続線を描画
                DrawBezier(startPos, endPos, Color.white);

                // 接続線の中央に小さな円を描画（削除用のハンドル）
                Vector2 midPoint = (startPos + endPos) * 0.5f;
                Rect handleRect = new Rect(midPoint.x - 8, midPoint.y - 8, 16, 16);
                EditorGUI.DrawRect(handleRect, new Color(0.8f, 0.2f, 0.2f, 0.5f));

                // 削除ハンドルに「×」マークを描画
                Handles.color = Color.white;
                Handles.DrawLine(new Vector3(midPoint.x - 5, midPoint.y - 5), new Vector3(midPoint.x + 5, midPoint.y + 5));
                Handles.DrawLine(new Vector3(midPoint.x - 5, midPoint.y + 5), new Vector3(midPoint.x + 5, midPoint.y - 5));
            }
        }

        protected virtual void DrawConnectionPreview()
        {
            if (_isConnecting && _startPort != null)
            {
                Vector2 startPos = GetPortPosition(_startPort);
                Vector2 endPos = Event.current.mousePosition;

                // 接続可能なポートがある場合は、そのポートに向かって接続プレビューを表示
                if (_hoveredPort != null && CanConnect(_startPort, _hoveredPort))
                {
                    endPos = GetPortPosition(_hoveredPort);
                    DrawBezier(startPos, endPos, Color.green);
                }
                else
                {
                    DrawBezier(startPos, endPos, Color.yellow);
                }
            }
        }

        protected virtual void DrawPortHighlights()
        {
            if (_isConnecting && _startPort != null)
            {
                // 接続可能なポートをハイライト表示
                foreach (var node in _nodes)
                {
                    var ports = _startPort.IsInput ? node.OutputPorts : node.InputPorts;
                    foreach (var port in ports)
                    {
                        if (port != _startPort && CanConnect(_startPort, port))
                        {
                            Vector2 portPos = GetPortPosition(port);
                            Rect highlightRect = new Rect(
                                portPos.x - PORT_HIGHLIGHT_SIZE / 2,
                                portPos.y - PORT_HIGHLIGHT_SIZE / 2,
                                PORT_HIGHLIGHT_SIZE,
                                PORT_HIGHLIGHT_SIZE
                            );

                            // ハイライトの背景
                            EditorGUI.DrawRect(highlightRect, new Color(0.2f, 0.8f, 0.2f, 0.3f));

                            // ハイライトの枠
                            Handles.color = Color.green;
                            Handles.DrawSolidDisc(portPos, Vector3.forward, PORT_HIGHLIGHT_SIZE / 2);
                        }
                    }
                }
            }
        }

        protected virtual void DrawBezier(Vector2 start, Vector2 end, Color color)
        {
            Vector2 startTangent = start + Vector2.right * CONNECTION_CURVE;
            Vector2 endTangent = end - Vector2.right * CONNECTION_CURVE;

            // 太い線で描画
            Handles.color = color;
            Handles.DrawBezier(start, end, startTangent, endTangent, color, null, CONNECTION_LINE_WIDTH);

            // 矢印を描画
            Vector2 direction = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            Vector2 arrowPos = end - direction * 10f;

            // 矢印の先端を描画
            Handles.color = color;
            Handles.DrawSolidDisc(arrowPos, Vector3.forward, 5f);

            // 矢印の両翼を描画
            Vector2 leftWing = arrowPos - direction * 10f + perpendicular * 5f;
            Vector2 rightWing = arrowPos - direction * 10f - perpendicular * 5f;

            Handles.DrawLine(arrowPos, leftWing);
            Handles.DrawLine(arrowPos, rightWing);
        }

        protected virtual Vector2 GetPortPosition(Port port)
        {
            float x = port.IsInput ? port.Node.Position.x + 5 : port.Node.Position.x + NODE_WIDTH - 25;
            float y = port.Node.Position.y + 30 + (port.IsInput ? port.Node.InputPorts : port.Node.OutputPorts).IndexOf(port) * PORT_SPACING;
            return new Vector2(x, y);
        }

        public virtual void HandleEvents(Event e)
        {
            _hoveredPort = null;

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
                case EventType.MouseMove:
                    HandleMouseMove(e);
                    break;
                case EventType.Repaint:
                    // マウスがポートの上にあるかチェック
                    CheckPortHover(e.mousePosition);
                    break;
                case EventType.ContextClick:
                    HandleContextClick(e);
                    break;
            }
        }

        protected virtual void HandleMouseDown(Event e)
        {
            if (e.button == 0)
            {
                // まず接続線の削除ハンドルをクリックしたかチェック
                Connection clickedConnection = FindConnectionAtPosition(e.mousePosition);
                if (clickedConnection != null)
                {
                    // 接続線を削除
                    _connections.Remove(clickedConnection);
                    GUI.changed = true;
                    e.Use();
                    Debug.Log("接続を削除しました");
                    return;
                }

                // 次にポートをクリックしたかチェック
                _startPort = FindPortAtPosition(null, e.mousePosition);
                if (_startPort != null)
                {
                    _isConnecting = true;
                    e.Use();
                    return; // ポートをクリックした場合はノードの選択をスキップ
                }

                // ポートをクリックしていない場合はノードの選択を処理
                _selectedNode = _nodes.Find(n => new Rect(n.Position.x, n.Position.y, NODE_WIDTH, NODE_HEIGHT).Contains(e.mousePosition));
                if (_selectedNode != null)
                {
                    _dragOffset = e.mousePosition - _selectedNode.Position;
                }
            }
        }

        protected virtual void HandleMouseDrag(Event e)
        {
            if (_selectedNode != null)
            {
                _selectedNode.Position = e.mousePosition - _dragOffset;
                GUI.changed = true;
            }
        }

        protected virtual void HandleMouseUp(Event e)
        {
            if (_isConnecting && _startPort != null)
            {
                Port targetPort = FindPortAtPosition(null, e.mousePosition);
                if (targetPort != null && targetPort != _startPort && CanConnect(_startPort, targetPort))
                {
                    _connections.Add(new Connection(_startPort, targetPort));
                    Debug.Log($"接続を作成: {_startPort.Node.Name}.{_startPort.Name} -> {targetPort.Node.Name}.{targetPort.Name}");
                }
            }

            _isConnecting = false;
            _startPort = null;
            GUI.changed = true;
        }

        protected virtual void HandleMouseMove(Event e)
        {
            // マウスがポートの上にあるかチェック
            CheckPortHover(e.mousePosition);
        }

        protected virtual void CheckPortHover(Vector2 mousePosition)
        {
            if (_isConnecting && _startPort != null)
            {
                _hoveredPort = FindPortAtPosition(null, mousePosition);
                if (_hoveredPort != null && !CanConnect(_startPort, _hoveredPort))
                {
                    _hoveredPort = null;
                }
            }
        }

        protected virtual Port FindPortAtPosition(Node node, Vector2 position)
        {
            if (node == null)
            {
                node = _nodes.Find(n => new Rect(n.Position.x, n.Position.y, NODE_WIDTH, NODE_HEIGHT).Contains(position));
            }

            if (node != null)
            {
                foreach (var port in node.InputPorts.Concat(node.OutputPorts))
                {
                    Rect portRect = new Rect(
                        port.IsInput ? node.Position.x + 5 : node.Position.x + NODE_WIDTH - 25,
                        node.Position.y + 30 + (port.IsInput ? node.InputPorts : node.OutputPorts).IndexOf(port) * PORT_SPACING,
                        20,
                        PORT_HEIGHT
                    );

                    // ポートの検出範囲を広げる
                    Rect extendedRect = new Rect(
                        portRect.x - 5,
                        portRect.y - 5,
                        portRect.width + 10,
                        portRect.height + 10
                    );

                    if (extendedRect.Contains(position))
                    {
                        return port;
                    }
                }
            }

            return null;
        }

        protected virtual bool CanConnect(Port start, Port end)
        {
            if (start == null || end == null) return false;
            if (start.Node == end.Node) return false;
            if (start.IsInput == end.IsInput) return false;
            if (_connections == null) return true;
            return !_connections.Any(c => c.SourcePort == start && c.TargetPort == end);
        }

        protected virtual void HandleContextClick(Event e)
        {
            // 接続線の削除ハンドルをクリックしたかチェック
            _selectedConnection = FindConnectionAtPosition(e.mousePosition);
            if (_selectedConnection != null)
            {
                // 接続線を削除
                _connections.Remove(_selectedConnection);
                _selectedConnection = null;
                GUI.changed = true;
                e.Use();
                Debug.Log("接続を削除しました");
            }
        }

        protected virtual Connection FindConnectionAtPosition(Vector2 position)
        {
            foreach (var connection in _connections)
            {
                Vector2 startPos = GetPortPosition(connection.SourcePort);
                Vector2 endPos = GetPortPosition(connection.TargetPort);
                Vector2 midPoint = (startPos + endPos) * 0.5f;

                // 接続線の中央の削除ハンドルをクリックしたかチェック（検出範囲を広げる）
                Rect handleRect = new Rect(midPoint.x - 8, midPoint.y - 8, 16, 16);
                if (handleRect.Contains(position))
                {
                    return connection;
                }
            }

            return null;
        }
    }

    public class NodePanel : BaseNodePanel<Node>
    {
        public NodePanel(List<Node> nodes, List<Connection> connections) : base(nodes, connections)
        {
        }

        protected override void DrawNode(Node node)
        {
            node.DrawNode();
        }
    }
}
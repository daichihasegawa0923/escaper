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

        protected List<T> _nodes;
        protected List<Connection> _connections;
        protected T _selectedNode;
        protected Port _startPort;
        protected bool _isConnecting;

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
        }

        protected virtual void DrawNodes()
        {
            foreach (var node in _nodes)
            {
                node.DrawNode();
            }
        }

        protected virtual void DrawConnections()
        {
            foreach (var connection in _connections)
            {
                Vector2 startPos = GetPortPosition(connection.SourcePort);
                Vector2 endPos = GetPortPosition(connection.TargetPort);
                DrawBezier(startPos, endPos);
            }
        }

        protected virtual void DrawConnectionPreview()
        {
            if (_isConnecting && _startPort != null)
            {
                Vector2 startPos = GetPortPosition(_startPort);
                Vector2 endPos = Event.current.mousePosition;
                DrawBezier(startPos, endPos);
            }
        }

        protected virtual void DrawBezier(Vector2 start, Vector2 end)
        {
            Vector2 startTangent = start + Vector2.right * 50f;
            Vector2 endTangent = end - Vector2.right * 50f;
            Handles.DrawBezier(start, end, startTangent, endTangent, Color.white, null, 1f);
        }

        protected virtual Vector2 GetPortPosition(Port port)
        {
            float x = port.IsInput ? port.Node.Position.x + 5 : port.Node.Position.x + NODE_WIDTH - 25;
            float y = port.Node.Position.y + 30 + (port.IsInput ? port.Node.InputPorts : port.Node.OutputPorts).IndexOf(port) * PORT_SPACING;
            return new Vector2(x, y);
        }

        public virtual void HandleEvents(Event e)
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

        protected virtual void HandleMouseDown(Event e)
        {
            if (e.button == 0)
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

        protected virtual void HandleMouseDrag(Event e)
        {
            if (_selectedNode != null)
            {
                _selectedNode.Position = e.mousePosition;
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
                }
            }

            _isConnecting = false;
            _startPort = null;
            GUI.changed = true;
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
                    if (portRect.Contains(position))
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
    }

    public class NodePanel : BaseNodePanel<Node>
    {
        public NodePanel(List<Node> nodes, List<Connection> connections) : base(nodes, connections)
        {
        }
    }
}
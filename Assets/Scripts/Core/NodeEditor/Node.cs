using System.Collections.Generic;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public enum NodeType
    {
        Start,
        Status,
        Puzzle
    }

    public class Node
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public List<Port> InputPorts { get; private set; }
        public List<Port> OutputPorts { get; private set; }
        public GameObject TargetObject { get; set; }
        public Rect NodeRect { get; private set; }
        public NodeType Type { get; private set; }
        public string Status { get; set; }

        public Node(string name, Vector2 position, NodeType type)
        {
            Id = System.Guid.NewGuid().ToString();
            Name = name;
            Position = position;
            Type = type;
            InputPorts = new List<Port>();
            OutputPorts = new List<Port>();
            UpdateNodeRect();
        }

        public void UpdateNodeRect()
        {
            NodeRect = new Rect(Position.x, Position.y, 200f, 120f);
        }

        public void AddInputPort(string name)
        {
            InputPorts.Add(new Port(name, this, true));
        }

        public void AddOutputPort(string name)
        {
            OutputPorts.Add(new Port(name, this, false));
        }
    }
}
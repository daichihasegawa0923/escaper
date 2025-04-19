using System.Collections.Generic;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public abstract class Node
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public List<Port> InputPorts { get; private set; }
        public List<Port> OutputPorts { get; private set; }
        public GameObject TargetObject { get; set; }
        public Rect NodeRect { get; private set; }

        protected Node(string name, Vector2 position)
        {
            Id = System.Guid.NewGuid().ToString();
            Name = name;
            Position = position;
            InputPorts = new List<Port>();
            OutputPorts = new List<Port>();
            UpdateNodeRect();
            InitializePorts();
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

        // ノードの描画処理を各派生クラスで実装
        public abstract void DrawNode();

        // ノードの種類に応じたポートを初期化
        protected abstract void InitializePorts();

        // ノードの種類に応じた背景色を取得
        public abstract Color GetNodeColor();
    }
}
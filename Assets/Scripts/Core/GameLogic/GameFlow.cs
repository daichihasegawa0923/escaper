using System;
using System.Collections.Generic;
using Escaper.Core.Events;
using Escaper.Core.NodeEditor;

namespace Escaper.Core.GameLogic
{
    /// <summary>
    /// ゲームの進行を管理するクラス！めっちゃスムーズな進行を実現するよ！
    /// </summary>
    public class GameFlow
    {
        private GameState _gameState;
        private Node _currentNode;
        private Dictionary<string, Node> _nodes;
        private List<Connection> _connections;

        public event Action<Node> OnNodeChanged;
        public event Action<string> OnGameStateChanged;

        public GameFlow(GameState gameState)
        {
            _gameState = gameState;
            _nodes = new Dictionary<string, Node>();
            _connections = new List<Connection>();
        }

        /// <summary>
        /// ノードを追加するよ！
        /// </summary>
        public void AddNode(Node node)
        {
            _nodes[node.Id] = node;
        }

        /// <summary>
        /// 接続を追加するよ！
        /// </summary>
        public void AddConnection(Connection connection)
        {
            if (connection.IsValid())
            {
                _connections.Add(connection);
            }
        }

        /// <summary>
        /// 現在のノードを設定するよ！
        /// </summary>
        public void SetCurrentNode(Node node)
        {
            if (_nodes.ContainsKey(node.Id))
            {
                _currentNode = node;
                OnNodeChanged?.Invoke(node);
                GameEventSystem.Publish("NodeChanged", node);
            }
        }

        /// <summary>
        /// 次のノードに進むよ！
        /// </summary>
        public void MoveToNextNode(string portName)
        {
            var nextConnections = _connections.FindAll(c =>
                c.SourceNode == _currentNode &&
                c.SourcePort.Name == portName);

            if (nextConnections.Count > 0)
            {
                var nextNode = nextConnections[0].TargetNode;
                SetCurrentNode(nextNode);
            }
        }

        /// <summary>
        /// ゲームの状態を変更するよ！
        /// </summary>
        public void ChangeGameState(string stateName)
        {
            OnGameStateChanged?.Invoke(stateName);
            GameEventSystem.Publish("GameStateChanged", stateName);
        }

        /// <summary>
        /// ゲームをリセットするよ！
        /// </summary>
        public void ResetGame()
        {
            _currentNode = null;
            _connections.Clear();
            GameEventSystem.Publish("GameReset", null);
        }
    }
}
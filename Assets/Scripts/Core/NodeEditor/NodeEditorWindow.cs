using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public class NodeEditorWindow : EditorWindow
    {
        private List<Node> _nodes;
        private List<Connection> _connections;
        private NodePanel _nodePanel;
        private Vector2 _scrollPosition;
        private Vector2 _lastMousePosition;
        private Node _selectedNode;

        [MenuItem("Window/Escaper/Node Editor")]
        public static void ShowWindow()
        {
            GetWindow<NodeEditorWindow>("Escaper");
        }

        private void OnEnable()
        {
            _nodes = new List<Node>();
            _connections = new List<Connection>();
            _nodePanel = new NodePanel(_nodes, _connections);

            // スタートノードを追加
            Node startNode = new StartNode("Start");
            startNode.Position = new Vector2(100, 100);
            _nodes.Add(startNode);
        }

        private void OnGUI()
        {
            // マウス位置を保存（スクロールビューの前に）
            _lastMousePosition = Event.current.mousePosition;

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // 右クリックメニュー
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                ShowContextMenu();
                Event.current.Use();
            }

            // ノードパネルの描画
            _nodePanel.Draw();

            // イベント処理
            if (Event.current.type == EventType.MouseDown ||
                Event.current.type == EventType.MouseDrag ||
                Event.current.type == EventType.MouseUp ||
                Event.current.type == EventType.MouseMove ||
                Event.current.type == EventType.ContextClick)
            {
                _nodePanel.HandleEvents(Event.current);
                Repaint();
            }

            EditorGUILayout.EndScrollView();
        }

        private void ShowContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            Vector2 mousePosition = _lastMousePosition + _scrollPosition;

            // ノードの選択をチェック
            _selectedNode = _nodes.Find(n => new Rect(n.Position.x, n.Position.y, 200f, 120f).Contains(mousePosition));

            if (_selectedNode != null)
            {
                // 選択されたノードのコンテキストメニュー
                menu.AddItem(new GUIContent("Delete Node"), false, () => DeleteNode(_selectedNode));
                menu.AddSeparator("");
            }

            // 通常のコンテキストメニュー
            menu.AddItem(new GUIContent("Add Status Node"), false, () => AddStatusNode(mousePosition));
            menu.AddItem(new GUIContent("Add Puzzle Node"), false, () => AddPuzzleNode(mousePosition));
            menu.ShowAsContext();
        }

        private void DeleteNode(Node node)
        {
            if (node != null)
            {
                // ノードに関連する接続を削除
                _connections.RemoveAll(c => c.SourcePort.Node == node || c.TargetPort.Node == node);

                // ノードを削除
                _nodes.Remove(node);
                GUI.changed = true;
                Debug.Log($"ノードを削除しました: {node.Name}");
            }
        }

        private void AddStatusNode(Vector2 position)
        {
            if (_nodes != null)
            {
                Node node = new StatusNode("Status Node");
                node.Position = position;
                _nodes.Add(node);
                GUI.changed = true;
            }
        }

        private void AddPuzzleNode(Vector2 position)
        {
            if (_nodes != null)
            {
                Node node = new PuzzleNode("Puzzle Node");
                node.Position = position;
                _nodes.Add(node);
                GUI.changed = true;
            }
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace Escaper.Core.NodeEditor
{
    public abstract class NodeRow
    {
        public const float DEFAULT_HEIGHT = 30f;
        public const float DEFAULT_MARGIN = 5f;

        public virtual float Height => DEFAULT_HEIGHT;
        public virtual float Margin => DEFAULT_MARGIN;

        public abstract void Draw(Rect rect);
    }

    public class GameObjectRow : NodeRow
    {
        private GameObject _gameObject;
        private System.Action<GameObject> _onGameObjectChanged;

        public GameObjectRow(GameObject gameObject, System.Action<GameObject> onGameObjectChanged)
        {
            _gameObject = gameObject;
            _onGameObjectChanged = onGameObjectChanged;
        }

        public override void Draw(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.25f, 0.25f, 0.25f, 0.8f));
            var newGameObject = (GameObject)EditorGUI.ObjectField(rect, _gameObject, typeof(GameObject), true);
            if (newGameObject != _gameObject)
            {
                _gameObject = newGameObject;
                _onGameObjectChanged?.Invoke(_gameObject);
            }
        }
    }

    public class StatusRow : NodeRow
    {
        private string _status;
        private System.Action<string> _onStatusChanged;

        public StatusRow(string status, System.Action<string> onStatusChanged)
        {
            _status = status;
            _onStatusChanged = onStatusChanged;
        }

        public override void Draw(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.25f, 0.25f, 0.25f, 0.8f));
            var newStatus = EditorGUI.TextField(rect, _status);
            if (newStatus != _status)
            {
                _status = newStatus;
                _onStatusChanged?.Invoke(_status);
            }
        }
    }

    public class PortRow : NodeRow
    {
        private Port _port;
        private bool _isInput;

        public PortRow(Port port, bool isInput)
        {
            _port = port;
            _isInput = isInput;
        }

        public override void Draw(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 0.8f));
            GUI.Box(rect, _isInput ? "←" : "→");
            var labelRect = new Rect(
                _isInput ? rect.x + 20 : rect.x - 100,
                rect.y,
                100,
                rect.height
            );
            GUI.Label(labelRect, _port.Name);
        }
    }
}
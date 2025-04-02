using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Escaper.Utility.Editor
{
    public class GraphWindowEscaper : EditorWindow
    {
        [MenuItem("Tools/Escaper")]
        public static void Open()
        {
            var window = GetWindow<GraphWindowEscaper>();
            window.titleContent = new GUIContent("Escaper");
        }

        private void OnEnable()
        {
            var graphView = new GraphViewEscaper();
            rootVisualElement.Add(graphView);
            graphView.StretchToParentSize();
        }
    }
}
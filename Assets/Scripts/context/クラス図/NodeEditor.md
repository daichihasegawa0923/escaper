 ```mermaid
classDiagram
    class Node {
        <<abstract>>
        +string Id
        +string Name
        +Vector2 Position
        +List~Port~ InputPorts
        +List~Port~ OutputPorts
        +GameObject TargetObject
        +Rect NodeRect
        +Node(string name, Vector2 position)
        +void UpdateNodeRect()
        +void AddInputPort(string name)
        +void AddOutputPort(string name)
        +abstract void DrawNode()
        +abstract Color GetNodeColor()
        #abstract void InitializePorts()
    }

    class StartNode {
        -static Color NodeColor
        +StartNode(string name, Vector2 position)
        +void DrawNode()
        +Color GetNodeColor()
        #void InitializePorts()
    }

    class StatusNode {
        -static Color NodeColor
        +string Status
        +StatusNode(string name, Vector2 position)
        +void DrawNode()
        +Color GetNodeColor()
        #void InitializePorts()
    }

    class PuzzleNode {
        -static Color NodeColor
        +PuzzleNode(string name, Vector2 position)
        +void DrawNode()
        +Color GetNodeColor()
        #void InitializePorts()
    }

    class Port {
        +string Name
        +Node Node
        +bool IsInput
        +Port(string name, Node node, bool isInput)
    }

    class Connection {
        +Port SourcePort
        +Port TargetPort
        +Connection(Port source, Port target)
    }

    class NodePanel {
        -List~Node~ _nodes
        -List~Connection~ _connections
        -Node _selectedNode
        -Port _startPort
        -bool _isConnecting
        +NodePanel(List~Node~ nodes, List~Connection~ connections)
        +void Draw()
        +void HandleEvents(Event e)
    }

    class NodeEditorWindow {
        -List~Node~ _nodes
        -List~Connection~ _connections
        -NodePanel _nodePanel
        -Vector2 _scrollPosition
        -Vector2 _lastMousePosition
        +static void ShowWindow()
        -void OnEnable()
        -void OnGUI()
        -void ShowContextMenu()
        -void AddStatusNode(Vector2 position)
        -void AddPuzzleNode(Vector2 position)
    }

    Node <|-- StartNode
    Node <|-- StatusNode
    Node <|-- PuzzleNode
    Node "1" *-- "many" Port
    Connection "1" *-- "2" Port
    NodePanel "1" *-- "many" Node
    NodePanel "1" *-- "many" Connection
    NodeEditorWindow "1" *-- "1" NodePanel
    NodeEditorWindow "1" *-- "many" Node
    NodeEditorWindow "1" *-- "many" Connection
```
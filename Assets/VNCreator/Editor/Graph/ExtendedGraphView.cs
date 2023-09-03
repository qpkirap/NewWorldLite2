using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace VNCreator.Editors.Graph
{
#if UNITY_EDITOR
    public class ExtendedGraphView : GraphView
    {
        private Vector2 mousePosition = new Vector2();
        private StoryObject Container;

        private readonly List<BaseNode> allNodes = new();

        public ExtendedGraphView(StoryObject container)
        {
            this.Container = container;
            
            SetupZoom(0.1f, 2);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            this.deleteSelection += (operationName, user) =>
            {
                foreach (var baseNode in allNodes.ToList())
                {
                    if (baseNode.IsSelectable())
                    {
                        baseNode.OnDelete();
                        
                        DeleteSelection();

                        allNodes.Remove(baseNode);
                    }
                }
            };
        }

        public void GenerateActionNode(Vector2 _mousePos, bool _startNode, bool _endNode, CommandData commandData = null)
        {
            var actionNode = new CommandNode(commandData, Container);
            
            actionNode.title = "Action";
            
            actionNode.SetPosition(new Rect((new Vector2(viewTransform.position.x, viewTransform.position.y) * -(1 / scale)) + (_mousePos * (1 / scale)), Vector2.one));
            
            if (!_startNode)
            {
                Port _inputPort = CreatePort(actionNode, Direction.Input, Port.Capacity.Multi);
                _inputPort.portName = "Input";
                actionNode.inputContainer.Add(_inputPort);
            }

            if (!_endNode)
            {
                Port _outputPort = CreatePort(actionNode, Direction.Output, Port.Capacity.Single);
                _outputPort.portName = "Next";
                actionNode.outputContainer.Add(_outputPort);
            }
        }

        public void GenerateDialogueNode(string _nodeName, Vector2 _mousePos, int choiceAmount, bool _startNode, bool _endNode)
        {
            AddElement(CreateDialogueNode(_nodeName, _mousePos, choiceAmount, _startNode, _endNode));
        }

        public DialogueNode CreateDialogueNode(string nodeName, Vector2 mousePos, int choiceAmount, bool _startNode, bool _endNode)
        {
            return CreateDialogueNode(nodeName, mousePos, choiceAmount, new string[choiceAmount].ToList(), _startNode, _endNode);
        }

        public DialogueNode CreateDialogueNode(
            string nodeName, 
            Vector2 mousePos, 
            int choiceAmount, 
            List<string> choices,
            bool startNode, 
            bool endNode, 
            DialogueNodeData data = null)
        {
            DialogueNode node = new DialogueNode(StoryObject.DialogueNodeDataKeys, Container, data);

            node.title = nodeName;
            node.SetPosition(new Rect((new Vector2(viewTransform.position.x, viewTransform.position.y) * -(1 / scale)) + (mousePos * (1/scale)), Vector2.one));
            node.EntityCache.SetValue("startNode", startNode);
            node.EntityCache.SetValue("endNode", endNode);
            node.EntityCache.SetValue("choices", choiceAmount);
            node.EntityCache.SetValue("choiceOptions", choices);

            if (!startNode)
            {
                Port _inputPort = CreatePort(node, Direction.Input, Port.Capacity.Multi);
                _inputPort.portName = "Input";
                node.inputContainer.Add(_inputPort);
            }

            if (!endNode)
            {
                if (choiceAmount > 1)
                {
                    for (int i = 0; i < choiceAmount; i++)
                    {
                        Port _outputPort = CreatePort(node, Direction.Output, Port.Capacity.Single);
                        _outputPort.portName = "Choice " + (i + 1);

                        string _value = data.ChoiceOptions.Count == 0 ? "Choice " + (i + 1) : node.EntityCache.ChoiceOptions[i];
                        int _idx = i;

                        TextField _field = new TextField { value = _value };
                        _field.RegisterValueChangedCallback(
                            e =>
                            {
                                var copyList = node.EntityCache.ChoiceOptions.ToList();
                                copyList[_idx] = _field.value;
                                
                                node.EntityCache.SetValue("choiceOptions", copyList);
                            }
                            );

                        node.outputContainer.Add(_field);
                        node.outputContainer.Add(_outputPort);
                    }
                }
                else
                {
                    Port _outputPort = CreatePort(node, Direction.Output, Port.Capacity.Single);
                    _outputPort.portName = "Next";
                    node.outputContainer.Add(_outputPort);
                }
            }
            
            node.mainContainer.Add(node.visuals);

            node.RefreshExpandedState();
            node.RefreshPorts();
            
            allNodes.Add(node);

            return node;
        }

        Port CreatePort(BaseNode _node, Direction _portDir, Port.Capacity _capacity)
        {
            return _node.InstantiatePort(Orientation.Horizontal, _portDir, _capacity, typeof(float));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach((port) =>
            {
                if (startPort != port && startPort.node != port.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        void MousePos(Vector2 _v2)
        {
            mousePosition = _v2;
        }
    }
#endif
}
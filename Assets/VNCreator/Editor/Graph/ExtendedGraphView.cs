using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using System.CodeDom;
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
                Debug.Log("asdf");
            };
        }

        public void GenerateActionNode(Vector2 _mousePos, bool _startNode, bool _endNode, CommandData commandData = null)
        {
            var actionNode = new ActionNode(commandData, Container);
            
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
            DialogueNode _node = new DialogueNode(data, Container);

            _node.RegisterCallback<ChangeEvent<DialogueNode>>(e =>
            {
                Debug.Log("sadf");
            });

            _node.title = nodeName;
            _node.SetPosition(new Rect((new Vector2(viewTransform.position.x, viewTransform.position.y) * -(1 / scale)) + (mousePos * (1/scale)), Vector2.one));
            _node.dialogue.SetValue("startNode", startNode);
            _node.dialogue.SetValue("endNode", endNode);
            _node.dialogue.SetValue("choices", choiceAmount);
            _node.dialogue.SetValue("choiceOptions", choices);

            if (!startNode)
            {
                Port _inputPort = CreatePort(_node, Direction.Input, Port.Capacity.Multi);
                _inputPort.portName = "Input";
                _node.inputContainer.Add(_inputPort);
            }

            if (!endNode)
            {
                if (choiceAmount > 1)
                {
                    for (int i = 0; i < choiceAmount; i++)
                    {
                        Port _outputPort = CreatePort(_node, Direction.Output, Port.Capacity.Single);
                        _outputPort.portName = "Choice " + (i + 1);

                        string _value = data.ChoiceOptions.Count == 0 ? "Choice " + (i + 1) : _node.dialogue.ChoiceOptions[i];
                        int _idx = i;

                        TextField _field = new TextField { value = _value };
                        _field.RegisterValueChangedCallback(
                            e =>
                            {
                                var copyList = _node.dialogue.ChoiceOptions.ToList();
                                copyList[_idx] = _field.value;
                                
                                _node.dialogue.SetValue("choiceOptions", copyList);
                            }
                            );

                        _node.outputContainer.Add(_field);
                        _node.outputContainer.Add(_outputPort);
                    }
                }
                else
                {
                    Port _outputPort = CreatePort(_node, Direction.Output, Port.Capacity.Single);
                    _outputPort.portName = "Next";
                    _node.outputContainer.Add(_outputPort);
                }
            }
            
            _node.mainContainer.Add(_node.visuals);

            _node.RefreshExpandedState();
            _node.RefreshPorts();

            return _node;
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
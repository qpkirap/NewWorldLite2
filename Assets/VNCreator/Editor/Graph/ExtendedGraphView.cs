using System;
using System.Collections;
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

        public ExtendedGraphView()
        {
            SetupZoom(0.1f, 2);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
        }

        public void GenerateActionNode(Vector2 _mousePos, bool _startNode, bool _endNode, CommandData data)
        {
            var actionNode = new ActionNode(data);
            
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

        public DialogueNode CreateDialogueNode(string _nodeName, Vector2 _mousePos, int choiceAmount, bool _startNode, bool _endNode)
        {
            return CreateDialogueNode(_nodeName, _mousePos, choiceAmount, new string[choiceAmount].ToList(), _startNode, _endNode, new DialogueNodeData());
        }

        public DialogueNode CreateDialogueNode(string _nodeName, Vector2 _mousePos, int choiceAmount, List<string> _choices, bool _startNode, bool _endNode, DialogueNodeData _data)
        {
            DialogueNode _node = new DialogueNode(_data);

            _node.title = _nodeName;
            _node.SetPosition(new Rect((new Vector2(viewTransform.position.x, viewTransform.position.y) * -(1 / scale)) + (_mousePos * (1/scale)), Vector2.one));
            _node.DialogueNodeData.SetValue("startNode", _startNode);
            _node.DialogueNodeData.SetValue("endNode", _endNode);
            _node.DialogueNodeData.SetValue("choices", choiceAmount);
            _node.DialogueNodeData.SetValue("choiceOptions", _choices);

            if (!_startNode)
            {
                Port _inputPort = CreatePort(_node, Direction.Input, Port.Capacity.Multi);
                _inputPort.portName = "Input";
                _node.inputContainer.Add(_inputPort);
            }

            if (!_endNode)
            {
                if (choiceAmount > 1)
                {
                    for (int i = 0; i < choiceAmount; i++)
                    {
                        Port _outputPort = CreatePort(_node, Direction.Output, Port.Capacity.Single);
                        _outputPort.portName = "Choice " + (i + 1);

                        string _value = _data.ChoiceOptions.Count == 0 ? "Choice " + (i + 1) : _node.DialogueNodeData.ChoiceOptions[i];
                        int _idx = i;

                        TextField _field = new TextField { value = _value };
                        _field.RegisterValueChangedCallback(
                            e =>
                            {
                                var copyList = _node.DialogueNodeData.ChoiceOptions.ToList();
                                copyList[_idx] = _field.value;
                                
                                _node.DialogueNodeData.SetValue("choiceOptions", copyList);
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
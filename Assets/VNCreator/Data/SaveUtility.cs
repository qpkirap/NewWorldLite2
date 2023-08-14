﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine.UIElements;
using VNCreator.Editors.Graph;

namespace VNCreator
{
    public class SaveUtility
    {
#if UNITY_EDITOR
        public void SaveGraph(StoryObject _story, ExtendedGraphView graph)
        {
            EditorUtility.SetDirty(_story);

            List<DialogueNodeData> nodes = new List<DialogueNodeData>();
            List<Link> links = new List<Link>();
            
            foreach (var graphNode in graph.nodes.Cast<BaseNode>())
            {
                switch (graphNode.NodeType)
                {
                    case NodeType.Action:
                    {
                        var action = (ActionNode)graphNode;

                        _story.commandDatas.Add(action.EntityCache);
                    }
                        break;
                    case NodeType.Dialogue:
                    {
                        var dialogue = (DialogueNode)graphNode;
                        
                        _story.nodes.SetValue(StoryObject.DialogueNodeDataKeys, dialogue);
                    }
                        break;
                }
            }

            var edges = graph.edges.ToList();
            for (int i = 0; i < edges.Count; i++)
            {
                var _output = (BaseNode)edges[i].output.node;
                var _input = (BaseNode)edges[i].input.node;

                links.Add(new Link 
                { 
                    guid = _output.Guid,
                    targetGuid = _input.Guid,
                    portId = i
                });
            }

            _story.SetLists(nodes, links);
            
            EditorUtility.SetDirty(_story);

            //_story.nodes = nodes;
            //_story.links = links;
        }

        public void LoadGraph(StoryObject _story, ExtendedGraphView _graph)
        {
            foreach (DialogueNodeData _data in _story.nodes)
            {
                DialogueNode _tempNode = _graph.CreateDialogueNode("", _data.NodePosition.position, _data.Choices, _data.ChoiceOptions.ToList(), _data.StartNode, _data.EndNode, _data);
                _graph.AddElement(_tempNode);
            }

            GenerateLinks(_story, _graph);
        }

        void GenerateLinks(StoryObject _story, ExtendedGraphView _graph)
        {
            var nodes = _graph.nodes.ToList().Cast<BaseNode>().ToList();

            for (int i = 0; i < nodes.Count; i++)
            {
                int outputIdx = 1;
                
                List<Link> links = _story.links.Where(x => x.guid == nodes[i].Guid).ToList();
                
                for (int j = 0; j < links.Count; j++)
                {
                    var targetGuid = links[j].targetGuid;
                    
                    var target = nodes.First(x => x.Guid == targetGuid);
                    
                    LinkNodes(nodes[i].outputContainer[links.Count > 1 ? outputIdx : 0].Q<Port>(), (Port)target.inputContainer[0], _graph);
                    
                    outputIdx += 2;
                }
            }
        }

        void LinkNodes(Port _output, Port _input, ExtendedGraphView _graph)
        {
            //Debug.Log(_output);

            Edge _temp = new Edge
            {
                output = _output,
                input = _input
            };

            _temp.input.Connect(_temp);
            _temp.output.Connect(_temp);
            _graph.Add(_temp);
        }
#endif
    }
}

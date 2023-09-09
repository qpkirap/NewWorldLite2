using System.Collections.Generic;
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
        public void SaveGraph(StoryObject story, ExtendedGraphView graph)
        {
            EditorUtility.SetDirty(story);

            List<DialogueNodeData> nodes = new List<DialogueNodeData>();
            List<Link> links = new List<Link>();
            
            foreach (var graphNode in graph.nodes.Cast<BaseNode>())
            {
                switch (graphNode.NodeType)
                {
                    case NodeType.Action:
                    {
                        var action = (CommandNode)graphNode;
                    }
                        break;
                    case NodeType.Dialogue:
                    {
                        var dialogue = (DialogueNode)graphNode;
                        
                        dialogue.EntityCache.SetValue("nodePosition", dialogue.GetPosition());
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

            story.SetLists(links);
            
            EditorUtility.SetDirty(story);

            //_story.nodes = nodes;
            //_story.links = links;
        }

        public void LoadGraph(StoryObject story, ExtendedGraphView graph)
        {
            foreach (var data in story.nodes)
            {
                var tempNode = graph.CreateDialogueNode("", data.NodePosition.position, data.Choices, data.ChoiceOptions.ToList(), data.StartNode, data.EndNode, data);
                
                graph.AddElement(tempNode);
            }
            
            foreach (var command in story.commandDatas)
            {
                var commandNode = graph.GenerateActionNode(command.NodePosition.position, command.IsStartNode, command.IsEndNode, command);
                
                graph.Add(commandNode);
            }

            GenerateLinks(story, graph);
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

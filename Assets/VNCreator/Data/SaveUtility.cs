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
        public void SaveGraph(StoryObject _story, ExtendedGraphView _graph)
        {
            EditorUtility.SetDirty(_story);

            List<DialogueNodeData> nodes = new List<DialogueNodeData>();
            List<Link> links = new List<Link>();

            /*foreach (DialogueNode _node in _graph.nodes.ToList().Cast<DialogueNode>().ToList())
            {
                    nodes.Add(
                    new DialogueNodeData(
                        guid: _node.DialogueNodeData.Guid,
                        characterName : _node.DialogueNodeData.CharacterName,
                        dialogueText : _node.DialogueNodeData.DialogueText,
                        backgroundSprList : _node.DialogueNodeData.BackgroundSprList?.ToList(),
                        startNode : _node.DialogueNodeData.StartNode,
                        endNode : _node.DialogueNodeData.EndNode,
                        choices : _node.DialogueNodeData.Choices,
                        choiceOptions : _node.DialogueNodeData.ChoiceOptions?.ToList(),
                        nodePosition : _node.GetPosition(),
                        soundEffect : _node.DialogueNodeData.SoundEffect,
                        backgroundMusic : _node.DialogueNodeData.BackgroundMusic,
                        characterSprList : _node.DialogueNodeData.CharacterSprList?.ToList()));
            }*/

            var _edges = _graph.edges.ToList();
            for (int i = 0; i < _edges.Count; i++)
            {
                var _output = (BaseNode)_edges[i].output.node;
                var _input = (BaseNode)_edges[i].input.node;

                links.Add(new Link 
                { 
                    guid = _output.Guid,
                    targetGuid = _input.Guid,
                    portId = i
                });
            }

            _story.SetLists(nodes, links);

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
            List<DialogueNode> _nodes = _graph.nodes.ToList().Cast<DialogueNode>().ToList();

            for (int i = 0; i < _nodes.Count; i++)
            {
                int _outputIdx = 1;
                List<Link> _links = _story.links.Where(x => x.guid == _nodes[i].DialogueNodeData.Id).ToList();
                for (int j = 0; j < _links.Count; j++)
                {
                    string targetGuid = _links[j].targetGuid;
                    DialogueNode _target = _nodes.First(x => x.DialogueNodeData.Id == targetGuid);
                    LinkNodes(_nodes[i].outputContainer[_links.Count > 1 ? _outputIdx : 0].Q<Port>(), (Port)_target.inputContainer[0], _graph);
                    _outputIdx += 2;
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

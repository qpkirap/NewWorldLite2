using System.Collections.Generic;
using UnityEngine;

namespace VNCreator
{
    [CreateAssetMenu(fileName = "New Story", menuName = "New Story")]
    public class StoryObject : ScriptableEntity
    {
        public List<Link> links;
        public List<DialogueNodeData> nodes;

        public void SetLists(List<DialogueNodeData> _nodes, List<Link> _links)
        {
            links = new List<Link>();
            for (int i = 0; i < _links.Count; i++)
            {
                links.Add(_links[i]);
            }

            nodes = new List<DialogueNodeData>();
            for (int i = 0; i < _nodes.Count; i++)
            {
                nodes.Add(_nodes[i]);
            }
        }

        public DialogueNodeData GetFirstNode()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].StartNode)
                {
                    return nodes[i];
                }
            }

            Debug.LogError("You need a start node");
            return null;
        }
        public DialogueNodeData GetCurrentNode(string _currentGuid)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Guid == _currentGuid)
                    return nodes[i];
            }

            return null;
        }

        List<Link> _tempLinks = new List<Link>();
        public DialogueNodeData GetNextNode(string _currentGuid, int _choiceId)
        {
            _tempLinks = new List<Link>();

            for (int i = 0; i < links.Count; i++)
            {
                if (links[i].guid == _currentGuid)
                    _tempLinks.Add(links[i]);
            }

            if(_choiceId < _tempLinks.Count)
                return GetCurrentNode(_tempLinks[_choiceId].targetGuid);
            else
                return null;
        }
    }
}

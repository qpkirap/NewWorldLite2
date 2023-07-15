using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNCreator
{
    public class DisplayBase : MonoBehaviour
    {
        public StoryObject story;

        protected NodeData currentNode;
        protected bool lastNode;

        protected List<string> loadList = new List<string>();

        void Awake()
        {
            if (PlayerPrefs.GetString(GameSaveManager.currentLoadName) == string.Empty)
            {
                currentNode = story.GetFirstNode();
                loadList.Add(currentNode.Guid);
            }
            else
            {
                loadList = GameSaveManager.Load();
                if(loadList == null || loadList.Count == 0)
                {
                    currentNode = story.GetFirstNode();
                    loadList = new List<string>();
                    loadList.Add(currentNode.Guid);
                }
                else
                {
                    currentNode = story.GetCurrentNode(loadList[loadList.Count - 1]);
                }
            }
        }

        protected virtual void NextNode(int _choiceId)
        {
            if (!lastNode) 
            {
                currentNode = story.GetNextNode(currentNode.Guid, _choiceId);
                lastNode = currentNode.EndNode;
                loadList.Add(currentNode.Guid);
            }
        }

        protected virtual void Previous()
        {
            loadList.RemoveAt(loadList.Count - 1);
            currentNode = story.GetCurrentNode(loadList[loadList.Count - 1]);
            lastNode = currentNode.EndNode;
        }

        protected void Save()
        {
            GameSaveManager.Save(loadList);
        }
    }
}

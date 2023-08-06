using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNCreator
{
    public class DisplayBase : MonoBehaviour
    {
        public StoryObject story;

        protected DialogueNodeData CurrentDialogueNode;
        protected bool lastNode;

        protected List<string> loadList = new List<string>();

        void Awake()
        {
            if (PlayerPrefs.GetString(GameSaveManager.currentLoadName) == string.Empty)
            {
                CurrentDialogueNode = story.GetFirstNode();
                loadList.Add(CurrentDialogueNode.Id);
            }
            else
            {
                loadList = GameSaveManager.Load();
                if(loadList == null || loadList.Count == 0)
                {
                    CurrentDialogueNode = story.GetFirstNode();
                    loadList = new List<string>();
                    loadList.Add(CurrentDialogueNode.Id);
                }
                else
                {
                    CurrentDialogueNode = story.GetCurrentNode(loadList[loadList.Count - 1]);
                }
            }
        }

        protected virtual void NextNode(int _choiceId)
        {
            if (!lastNode) 
            {
                CurrentDialogueNode = story.GetNextNode(CurrentDialogueNode.Id, _choiceId);
                lastNode = CurrentDialogueNode.EndNode;
                loadList.Add(CurrentDialogueNode.Id);
            }
        }

        protected virtual void Previous()
        {
            loadList.RemoveAt(loadList.Count - 1);
            CurrentDialogueNode = story.GetCurrentNode(loadList[loadList.Count - 1]);
            lastNode = CurrentDialogueNode.EndNode;
        }

        protected void Save()
        {
            GameSaveManager.Save(loadList);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VNCreator
{
    [Serializable]
    public class NodeData
    {
        private string guid;
        [SerializeField] private string characterName;
        [SerializeField] private string dialogueText;
        [SerializeField] private bool startNode;
        [SerializeField] private bool endNode;
        [SerializeField] private int choices = 1;
        [SerializeField] List<string> choiceOptions;
        [SerializeField] private Rect nodePosition;
        [SerializeField] private AudioClip soundEffect;
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] List<AssetReference> characterSprList;
        [SerializeField] List<AssetReference> backgroundSprList;

        public string Guid => string.IsNullOrEmpty(guid) ? guid = GUID.Generate().ToString() : guid;
        public string DialogueText => dialogueText;

        public bool StartNode => startNode;

        public bool EndNode => endNode;

        public int Choices => choices;

        public Rect NodePosition => nodePosition;

        public AudioClip SoundEffect => soundEffect;

        public AudioClip BackgroundMusic => backgroundMusic;
        
        public IReadOnlyList<string> ChoiceOptions => choiceOptions;
        public IReadOnlyList<AssetReference> CharacterSprList => characterSprList;
        public IReadOnlyList<AssetReference> BackgroundSprList => backgroundSprList;

        public string CharacterName => characterName;

        public NodeData()
        {
        }

        public NodeData(string guid, string characterName,
            string dialogueText, bool startNode, bool endNode, int choices, 
            List<string> choiceOptions,
            Rect nodePosition, AudioClip soundEffect, AudioClip backgroundMusic, 
            List<AssetReference> characterSprList,
            List<AssetReference> backgroundSprList)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(guid)) this.guid = GUID.Generate().ToString();      
#endif
            
            this.characterName = characterName;
            this.dialogueText = dialogueText;
            this.startNode = startNode;
            this.endNode = endNode;
            this.choices = choices;
            this.choiceOptions = choiceOptions;
            this.nodePosition = nodePosition;
            this.soundEffect = soundEffect;
            this.backgroundMusic = backgroundMusic;
            this.characterSprList = characterSprList;
            this.backgroundSprList = backgroundSprList;
        }
    }
}
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
        public string guid;
        public AssetReference characterSpr;
        [SerializeField] private string characterName;
        public string dialogueText;
        public Sprite backgroundSpr;
        public bool startNode;
        public bool endNode;
        public int choices = 1;
        public List<string> choiceOptions;
        public Rect nodePosition;
        public AudioClip soundEffect;
        public AudioClip backgroundMusic;
        public List<AssetReference> characterSprList;

        public string CharacterName => characterName;
        public AddressableSprite CharacterSpr => new(characterSpr);

        public NodeData()
        {
            guid = Guid.NewGuid().ToString();
        }

        public NodeData(string guid, AssetReference characterSpr, string characterName, string dialogueText, Sprite backgroundSpr, bool startNode, bool endNode, int choices, List<string> choiceOptions, Rect nodePosition, AudioClip soundEffect, AudioClip backgroundMusic, List<AssetReference> characterSprList)
        {
            if (string.IsNullOrEmpty(guid)) this.guid = GUID.Generate().ToString();
            
            this.characterSpr = characterSpr;
            this.characterName = characterName;
            this.dialogueText = dialogueText;
            this.backgroundSpr = backgroundSpr;
            this.startNode = startNode;
            this.endNode = endNode;
            this.choices = choices;
            this.choiceOptions = choiceOptions;
            this.nodePosition = nodePosition;
            this.soundEffect = soundEffect;
            this.backgroundMusic = backgroundMusic;
            this.characterSprList = characterSprList;
        }
    }
}
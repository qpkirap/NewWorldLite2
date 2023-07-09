#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.AddressableAssets;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEngine.AddressableAssets;
#endif

namespace VNCreator
{
#if UNITY_EDITOR
    public class BaseNode : Node
    {
        public NodeData nodeData;
        public NodeViewer visuals;

        public BaseNode(NodeData _data)
        {
            nodeData = _data != null ? _data : new NodeData();
            visuals = new NodeViewer(this);
        }
    }

    public class NodeViewer : VisualElement
    {
        BaseNode node;

        public NodeViewer(BaseNode _node)
        {
            node = _node;

            VisualTreeAsset tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/VNCreator/Editor/Graph/Node/BaseNodeTemplate.uxml");
            tree.CloneTree(this);

            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/VNCreator/Editor/Graph/Node/BaseNodeStyle.uss"));

            UpdateCharacterSprAsync();
            InitListViewAsync();

            TextField charNameField = this.Query<TextField>("Char_Name");
            charNameField.value = node.nodeData.characterName;
            charNameField.RegisterValueChangedCallback(
                e =>
                {
                    node.nodeData.characterName = charNameField.value;
                }
            );

            TextField dialogueField = this.Query<TextField>("Dialogue_Field");
            dialogueField.multiline = true;
            dialogueField.value = node.nodeData.dialogueText;
            dialogueField.RegisterValueChangedCallback(
                e =>
                {
                    node.nodeData.dialogueText = dialogueField.value;
                }
            );

            ObjectField sfxField = this.Query<ObjectField>("Sound_Field").First();
            sfxField.objectType = typeof(AudioClip);
            sfxField.value = node.nodeData.soundEffect;
            sfxField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                e =>
                {
                    node.nodeData.soundEffect = (AudioClip)e.newValue;
                }
            );

            ObjectField musicField = this.Query<ObjectField>("Music_Field").First();
            musicField.objectType = typeof(AudioClip);
            musicField.value = node.nodeData.soundEffect;
            musicField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                e =>
                {
                    node.nodeData.backgroundMusic = (AudioClip)e.newValue;
                }
            );

            VisualElement backSprDisplay = this.Query<VisualElement>("Back_Img");
            backSprDisplay.style.backgroundImage = node.nodeData.backgroundSpr ? node.nodeData.backgroundSpr.texture : null;

            ObjectField backSprField = this.Query<ObjectField>("Back_Selector").First();
            backSprField.objectType = typeof(Sprite);
            backSprField.value = node.nodeData.backgroundSpr;
            backSprField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                e =>
                {
                    node.nodeData.backgroundSpr = (Sprite)e.newValue;
                    backSprDisplay.style.backgroundImage = node.nodeData.backgroundSpr ? node.nodeData.backgroundSpr.texture : null;
                }
            );
        }

        private async UniTask InitListViewAsync()
        {
            ListView view = this.Query<ListView>("Char_Img_List");
            var tempList = new List<ObjectField>();

            if (node.nodeData.characterSprList is { Count: > 0 })
            {
                for (var i = 0; i < node.nodeData.characterSprList.Count; i++)
                {
                    var item = node.nodeData.characterSprList[i];
                    
                    ObjectField field = new ObjectField();
                    
                    tempList.Add(field);

                    BindItem(field, i);

                    if (item != null && item.RuntimeKeyIsValid())
                    {
                        var adrSprite = new AddressableSprite(item);

                        var sprite = await adrSprite.LoadAsync();

                        field.SetValueWithoutNotify(sprite);
                        field.SendEvent(new ChangeEvent<ChangeEvent<UnityEngine.Object>>());
                    }
                }
            }
            
            view.itemsSource = tempList;
            view.selectionType = SelectionType.Single;
            view.showAddRemoveFooter = true;

            SyncLists();

            view.makeItem = () => new ObjectField();
            view.bindItem = BindItem;
            view.unbindItem = UnbindItem;

            void UnbindItem(VisualElement e, int index)
            {
                SyncLists();
            }

            void BindItem(VisualElement e, int index)
            {
                Debug.Log($"Bind {index} count {tempList.Count}");
                
                var convert = e as ObjectField;
                
                convert.objectType = typeof(Sprite);

                tempList[index] = convert;
                
                convert.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                    e =>
                    {
                        tempList[index].value = e.newValue;
                        
                        SyncLists();

                        Debug.Log($"convert.RegisterCallback {e.newValue}");
                    }
                );
            }

            void SyncLists()
            {
                node.nodeData.characterSprList ??= new();

                var delta = tempList.Count - node.nodeData.characterSprList.Count;

                //sync count
                if (delta > 0)
                {
                    for (int i = 0; i < delta; i++)
                    {
                        node.nodeData.characterSprList.Add(null);
                    }
                }
                else
                {
                    node.nodeData.characterSprList
                        .RemoveRange(Mathf.Clamp(node.nodeData.characterSprList.Count - 1, 0, node.nodeData.characterSprList.Count - 1),
                            Mathf.Abs(delta));
                }

                //syncValue
                for (var i = 0; i < tempList.Count; i++)
                {
                    var tempItem = tempList[i];

                    if (tempItem == null)
                    {
                        node.nodeData.characterSprList[i] = null;
                    }
                    else
                    {
                        var asset = GetAssetReferenceFromGUID(tempItem.value);

                        if (asset != null) node.nodeData.characterSprList[i] = asset;
                        else node.nodeData.characterSprList[i] = null;
                    }
                }
            }
        }

        private async UniTask UpdateCharacterSprAsync()
        {
            VisualElement charSprDisplay = this.Query<VisualElement>("Char_Img");

            ObjectField charSprField = this.Query<ObjectField>("Icon_Selection").First();
            charSprField.objectType = typeof(Sprite);

            charSprField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                ChangeEvent
            );
            
            await UpdateFieldBgImage();
            
            async void ChangeEvent(ChangeEvent<Object> e)
            {
                node.nodeData.characterSpr.SetEditorAsset(e.newValue);

                await UpdateFieldBgImage();
            }

            async UniTask UpdateFieldBgImage()
            {
                if (node.nodeData.characterSpr != null 
                    && node.nodeData.CharacterSpr is { RuntimeKeyIsValid: true })
                {
                    var sprite = await node.nodeData.CharacterSpr.LoadAsync();

                    charSprDisplay.style.backgroundImage = sprite.texture;
                }
                else
                {
                    charSprDisplay.style.backgroundImage = null;
                }
            }
        }

        private AssetReference GetAssetReferenceFromGUID<T>(T value) where T : UnityEngine.Object 
        {
            var path = AssetDatabase.GetAssetPath(value);
            var guid = AssetDatabase.AssetPathToGUID(path);
            var asset = GetAssetReferenceFromGUID(guid);

            return asset;
        }
        
        private AssetReference GetAssetReferenceFromGUID(string guid)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                return null;

            var groups = settings.groups;
            
            foreach (var group in groups)
            {
                var entry = group.GetAssetEntry(guid);

                if (entry == null) continue;

                var asset = new AssetReference(entry.guid);

                return asset;
            }

            return null;
        }
    }
#endif
}

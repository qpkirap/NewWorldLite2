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
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
#endif

namespace VNCreator
{
#if UNITY_EDITOR
    public class DialogueNode : BaseNode<DialogueNodeData>
    {
        public DialogueNodeData DialogueNodeData;
        public NodeViewer visuals;

        public override string Guid => DialogueNodeData.Id;
        public override NodeType NodeType => NodeType.Dialogue;

        public DialogueNode(DialogueNodeData _data, StoryObject container) : base(container)
        {
            DialogueNodeData = _data != null ? _data : new DialogueNodeData();
            visuals = new NodeViewer(this);
        }
    }

    public class NodeViewer : VisualElement
    {
        DialogueNode node;

        public NodeViewer(DialogueNode _node)
        {
            node = _node;

            VisualTreeAsset tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(DialogueNodePaths.Tree);
            tree.CloneTree(this);

            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueNodePaths.StyleSheets));
            
            InitCharSprListViewAsync();
            InitBgSprListViewAsync();

            TextField charNameField = this.Query<TextField>("Char_Name");
            charNameField.value = node.DialogueNodeData.CharacterName;
            charNameField.RegisterValueChangedCallback(
                e =>
                {
                    node.DialogueNodeData.SetValue("characterName", charNameField.value);
                }
            );

            TextField dialogueField = this.Query<TextField>("Dialogue_Field");
            dialogueField.multiline = true;
            dialogueField.value = node.DialogueNodeData.DialogueText;
            dialogueField.RegisterValueChangedCallback(
                e =>
                {
                    node.DialogueNodeData.SetValue("dialogueText", dialogueField.value);
                }
            );

            ObjectField sfxField = this.Query<ObjectField>("Sound_Field").First();
            sfxField.objectType = typeof(AudioClip);
            sfxField.value = node.DialogueNodeData.SoundEffect;
            sfxField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                e =>
                {
                    node.DialogueNodeData.SetValue("soundEffect", (AudioClip)e.newValue);
                }
            );

            ObjectField musicField = this.Query<ObjectField>("Music_Field").First();
            musicField.objectType = typeof(AudioClip);
            musicField.value = node.DialogueNodeData.BackgroundMusic;
            musicField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                e =>
                {
                    node.DialogueNodeData.SetValue("backgroundMusic", (AudioClip)e.newValue);
                }
            );
        }
        
        private async UniTask InitBgSprListViewAsync()
        {
            ListView view = this.Query<ListView>("Bg_Img_List");
            VisualElement bgSprDisplay = this.Query<VisualElement>("Back_Img");

            var source = node.DialogueNodeData.GetValue<List<AssetReference>>("backgroundSprList");

            view.itemsSource = source;
            
            view.makeItem = () => new ObjectField();
            view.bindItem = BindItem;
            view.unbindItem = UnbindItem;
            view.selectionType = SelectionType.Single;
            view.showAddRemoveFooter = true;

            view.RefreshItems();
            UpdatePreviewAsync();

            void UnbindItem(VisualElement e, int index)
            {
            }

            async void BindItem(VisualElement e, int index)
            {
                var convert = e as ObjectField;
                
                convert.objectType = typeof(Sprite);

                convert.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                    e =>
                    {
                        source[index] = 
                            GetAssetReferenceFromGUID(e.newValue);

                        UpdatePreviewAsync();
                    }
                );
                
                var item = node.DialogueNodeData.BackgroundSprList[index];

                if (item != null && item.RuntimeKeyIsValid())
                {
                    var adrSprite = new AddressableSprite(item);

                    var sprite = await adrSprite.LoadAsync();

                    convert.value = sprite;
                }
                else convert.value = null;
            }

            async void UpdatePreviewAsync()
            {
                if (node.DialogueNodeData.BackgroundSprList != null
                    && node.DialogueNodeData.BackgroundSprList.Any())
                {
                    var items = node.DialogueNodeData.BackgroundSprList.Where(x => x != null && x.RuntimeKeyIsValid()).ToList();

                    if (items.Any())
                    {
                        var random = items[Random.Range(0, items.Count)];

                        var adrSprite = new AddressableSprite(random);

                        var sprite = await adrSprite.LoadAsync();

                        bgSprDisplay.style.backgroundImage = sprite.texture;
                        
                        return;
                    }
                }
                
                bgSprDisplay.style.backgroundImage = null;
            }
        }

        private async UniTask InitCharSprListViewAsync()
        {
            ListView view = this.Query<ListView>("Char_Img_List");
            VisualElement charSprDisplay = this.Query<VisualElement>("Char_Img");

            var source = node.DialogueNodeData.GetValue<List<AssetReference>>("characterSprList");

            view.itemsSource = source;
            
            view.makeItem = () => new ObjectField();
            view.bindItem = BindItem;
            view.unbindItem = UnbindItem;
            view.selectionType = SelectionType.Single;
            view.showAddRemoveFooter = true;

            view.RefreshItems();
            UpdatePreviewAsync();

            void UnbindItem(VisualElement e, int index)
            {
            }

            async void BindItem(VisualElement e, int index)
            {
                var convert = e as ObjectField;
                
                convert.objectType = typeof(Sprite);

                convert.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                    e =>
                    {
                        source[index] = 
                            GetAssetReferenceFromGUID(e.newValue);

                        UpdatePreviewAsync();
                    }
                );
                
                var item = node.DialogueNodeData.CharacterSprList[index];

                if (item != null && item.RuntimeKeyIsValid())
                {
                    var adrSprite = new AddressableSprite(item);

                    var sprite = await adrSprite.LoadAsync();

                    convert.value = sprite;
                }
                else convert.value = null;
            }

            async void UpdatePreviewAsync()
            {
                if (node.DialogueNodeData.CharacterSprList != null
                    && node.DialogueNodeData.CharacterSprList.Any())
                {
                    var items = node.DialogueNodeData.CharacterSprList.Where(x => x != null && x.RuntimeKeyIsValid()).ToList();

                    if (items.Any())
                    {
                        var random = items[Random.Range(0, items.Count)];

                        var adrSprite = new AddressableSprite(random);

                        var sprite = await adrSprite.LoadAsync();

                        charSprDisplay.style.backgroundImage = sprite.texture;
                        
                        return;
                    }
                }
                
                charSprDisplay.style.backgroundImage = null;
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

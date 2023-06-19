#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using UnityEditor;
#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.AddressableAssets;
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

            LoadAsync();

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

        private async UniTask LoadAsync()
        {
            VisualElement charSprDisplay = this.Query<VisualElement>("Char_Img");

            await UpdateBack();

            ObjectField charSprField = this.Query<ObjectField>("Icon_Selection").First();
            charSprField.objectType = typeof(Sprite);
            charSprField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                async e =>
                {
                    var editorAsset = GetAssetReferenceFromGUID(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(e.newValue)));
                    
                    node.nodeData.characterSpr = editorAsset;
                    
                    await UpdateBack();
                }
            );

            
            async UniTask UpdateBack()
            {
                if (node.nodeData.CharacterSpr.RuntimeKeyIsValid)
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

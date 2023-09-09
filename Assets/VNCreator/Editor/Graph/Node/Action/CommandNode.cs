#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.UIElements;

namespace VNCreator
{
    public class CommandNode : BaseNode<CommandData, CommandDataListComponentContainerEditor>
    {
        public ActionNodeViewer visuals;

        public override string Guid => EntityCache.Id;
        public override NodeType NodeType => NodeType.Action;
        
        public CommandNode(CommandData commandData, StoryObject container) 
            : base(StoryObject.CommandDataKey, container, commandData)
        {
            visuals = new(this);
        }
    }

    public class ActionNodeViewer : VisualElement
    {
        private CommandNode node;
        
        public ActionNodeViewer(CommandNode node)
        {
            this.node = node;

            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ActionNodePaths.Tree);
            tree.CloneTree(this);
            
            var styleSheetsAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>(ActionNodePaths.StyleSheets);
            styleSheets.Add(styleSheetsAsset);
            
            var dropdownAction = this.Query<DropdownField>("DropdownAction");
            var actionList = this.Query<VisualElement>("ActionList");
        }
    }
}

#endif
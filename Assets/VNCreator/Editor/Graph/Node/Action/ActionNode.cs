#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.UIElements;

namespace VNCreator
{
    public class ActionNode : BaseNode<CommandAction>
    {
        public CommandData CommandData;
        public ActionNodeViewer Viewer;

        public ActionNode(CommandData commandData, StoryObject container) : base(container)
        {
            CommandData = commandData;
            Viewer = new(this);
        }

        public override string Guid => CommandData.Id;
        public override NodeType NodeType => NodeType.Action;
    }

    public class ActionNodeViewer : VisualElement
    {
        private IEditorsFactory EditorsFactory => EditorCache.CommandComponentsEditorsFactory;
        private ActionNode node;
        
        public ActionNodeViewer(ActionNode node)
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
#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine.UIElements;

namespace VNCreator
{
    public class ActionNode : BaseNode
    {
        public CommandData CommandData;
        public ActionNodeViewer Viewer;

        public ActionNode(CommandData commandData)
        {
            CommandData = commandData ?? new CommandData();
            Viewer = new(this);
        }

        public override NodeType NodeType => NodeType.Action;
    }

    public class ActionNodeViewer : VisualElement
    {
        private IEditorsFactory EditorsFactory => EditorCache.CommandComponentsEditorsFactory;
        private ActionNode node;
        
        public ActionNodeViewer(ActionNode node)
        {
            EditorCache.Init();

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
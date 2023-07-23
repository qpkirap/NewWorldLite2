#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine.UIElements;

namespace VNCreator
{
    public class ActionNode : Node
    {
        public CommandData CommandData;
        public ActionNodeViewer Viewer;

        public ActionNode(CommandData commandData)
        {
            CommandData = commandData ?? new CommandData();
            Viewer = new(this);
        }
    }

    public class ActionNodeViewer : VisualElement
    {
        private ActionNode node;
        
        public ActionNodeViewer(ActionNode node)
        {
            this.node = node;

            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ActionNodePaths.Tree);
            tree.CloneTree(this);
            
            var styleSheetsAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>(ActionNodePaths.StyleSheets);
            styleSheets.Add(styleSheetsAsset);
        }
    }
}

#endif
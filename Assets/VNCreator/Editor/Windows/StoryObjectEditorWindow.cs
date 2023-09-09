using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using VNCreator.Editors.Graph;

namespace VNCreator.Editors
{
#if UNITY_EDITOR
    public class StoryObjectEditorWindow : BaseConfigEditor<StoryObject>
    {
        private StoryObject storyObj;
        
        ExtendedGraphView graphView;
        private static SaveUtility save = new SaveUtility();

        private Vector2 mousePosition = new Vector2();

        public static void Open(StoryObject _storyObj)
        {
            var window = CreateWindow<StoryObjectEditorWindow>();
            
            window.storyObj = _storyObj;
            
            window.CreateGraphView(_storyObj.nodes == null ? 0 : 1);
            
            EditorCache.Init();
        }

        public override void CloseEditor()
        {
            base.CloseEditor();
            
            save.SaveGraph(storyObj, graphView);
        }

        private void ShowContextMenu(KeyDownEvent keyEvent)
        {
            if (keyEvent.keyCode != KeyCode.Space) return; 
            
            mousePosition = Event.current.mousePosition;
            GenericMenu _menu = new GenericMenu();
            _menu.AddItem(new GUIContent("Add Actions"), false, () => graphView.GenerateActionNode(mousePosition, false, false, null));
            _menu.AddItem(new GUIContent("Add Node"), false, () => graphView.GenerateDialogueNode("", mousePosition, 1, false, false));
            _menu.AddItem(new GUIContent("Add Node (2 Choices)"), false, () => graphView.GenerateDialogueNode("", mousePosition, 2, false, false));
            _menu.AddItem(new GUIContent("Add Node (3 Choices)"), false, () => graphView.GenerateDialogueNode("", mousePosition, 3, false, false));
            _menu.AddItem(new GUIContent("Add Node (Start)"), false, () => graphView.GenerateDialogueNode("", mousePosition, 1, true, false));
            _menu.AddItem(new GUIContent("Add Node (End)"), false, () => graphView.GenerateDialogueNode("", mousePosition, 1, false, true));
            _menu.AddItem(new GUIContent("Save"), false, () => save.SaveGraph(storyObj, graphView));
            
            //actions
            //_menu.AddItem(new GUIContent("CommandAction"), false, () => graphView.GenerateActionNode(mousePosition, false, false, false, storyObj.nodes));
            
            _menu.ShowAsContext();
        }
        
        void CreateGraphView(int _nodeCount)
        {
            graphView = new ExtendedGraphView(storyObj);
            graphView.RegisterCallback<KeyDownEvent>(ShowContextMenu);
            graphView.StretchToParentSize();
            
            Window.rootVisualElement.Add(graphView);
          
            if (_nodeCount == 0) 
            {
                //graphView.GenerateNode(Vector2.zero, 1, true, false);
                return;
            }
            save.LoadGraph(storyObj, graphView);
        }

        public override string Title { get; } = "Story";
    }
#endif
}



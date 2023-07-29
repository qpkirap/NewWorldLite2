﻿using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using VNCreator.Commands;
using VNCreator.Editors.Graph;

namespace VNCreator.Editors
{
#if UNITY_EDITOR
    public class StoryObjectEditorWindow : BaseConfigEditor<StoryObject>
    {
        private static CommandDataEditor commandDataEditor;
        
        private StoryObject storyObj;
        
        ExtendedGraphView graphView;
        SaveUtility save = new SaveUtility();

        private Vector2 mousePosition = new Vector2();

        public static void Open(StoryObject _storyObj)
        {
            commandDataEditor ??= new();
            commandDataEditor.Init("CommandData", _storyObj);

            var window = CreateWindow<StoryObjectEditorWindow>();

            //commandDataEditor.SetSubEntityState(true);

            window.storyObj = _storyObj;
            window.CreateGraphView(_storyObj.nodes == null ? 0 : 1);

            CheckActionCommandData(_storyObj);
        }

        private void ShowContextMenu(KeyDownEvent keyEvent)
        {
            if (keyEvent.keyCode != KeyCode.Space) return; 
            
            mousePosition = Event.current.mousePosition;
            GenericMenu _menu = new GenericMenu();
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
            graphView = new ExtendedGraphView();
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

        private static void CheckActionCommandData(StoryObject storyObj)
        {
            if (storyObj == null || commandDataEditor == null) return;

            if (storyObj.CommandData == null)
            {
                commandDataEditor.CreateTo("CommandData", storyObj);
            }
        }

        public override string Title { get; } = "Story";
    }
#endif
}



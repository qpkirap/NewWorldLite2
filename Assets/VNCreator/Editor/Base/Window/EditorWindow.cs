#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace VNCreator
{
    public sealed class EditorWindow : UnityEditor.EditorWindow
    {
        internal event Action OnLateGUI;
        internal event Action<EditorWindow> OnClosed;

        private bool destroyed;
        private BaseEditorWindow rootEditor;

        [SerializeField] private string rootEditorType;

        public string Title => rootEditor.Title;
        public BaseEditorWindow RootEditor => rootEditor;

        internal string RootEditorType => rootEditorType;

        private void OnEnable()
        {
            // reinit editor
            if (!string.IsNullOrEmpty(rootEditorType))
            {
                var editorType = Type.GetType(rootEditorType);

                rootEditor = ReflectionUtils.CreateByType<BaseEditorWindow>(editorType);

                SetupWindow(rootEditor);
                rootEditor.SetupWindow(this);
            }

            EditorApplication.wantsToQuit += WantsToQuitEvent;
        }

        private void OnGUI()
        {
            CheckBackEvent();

            OnLateGUI?.Invoke();
            OnLateGUI = null;
        }

        internal void SetupWindow(BaseEditorWindow editor)
        {
            rootEditorType = editor.GetType().AssemblyQualifiedName;
            rootEditor = editor;
        }

        private void CheckBackEvent()
        {
            var e = Event.current;

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                CloseEditor();
            }
        }

        internal void CheckDirty()
        {
            rootEditor?.CheckDirty();
        }

        internal void CloseEditor()
        {
            if (!destroyed)
            {
                Close();
            }

            rootEditor?.CloseEditor();

            destroyed = true;

            EditorApplication.wantsToQuit -= WantsToQuitEvent;

            OnClosed?.Invoke(this);
        }

        private bool WantsToQuitEvent()
        {
            CloseEditor();

            return false;
        }

        private void OnDestroy()
        {
            destroyed = true;

            CloseEditor();
        }
    }
}

#endif
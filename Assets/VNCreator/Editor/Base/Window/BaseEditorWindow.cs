#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace VNCreator
{
    public abstract class BaseEditorWindow : ICustomEditor
    {
        public event Action OnLateGUI
        {
            add => Window.OnLateGUI += value;
            remove => Window.OnLateGUI -= value;
        }

        public event Action<EditorWindow> OnClosed
        {
            add => Window.OnClosed += value;
            remove => Window.OnClosed -= value;
        }

        public abstract string Title { get; }

        public EditorWindow Window { get; private set; }

        public Rect Position => Window.position;
        public bool Docked => Window.docked;
        public bool WantsMouseMove { get => Window.wantsMouseMove; set => Window.wantsMouseMove = value; }
        public GUIContent TitleContent { get => Window.titleContent; set => Window.titleContent = value; }
        public Vector2 MaxSize { get => Window.maxSize; set => Window.maxSize = value; }
        public Vector2 MinSize { get => Window.minSize; set => Window.minSize = value; }

        public void SetupWindow(EditorWindow window)
        {
            Window = window;

            SetupWindow();
        }

        public void SetupWindow()
        {
            OnEnable();
        }

        public virtual void OnEnable()
        {
        }

        public virtual void CreateMenuItems(List<Action> items)
        {
        }

        public virtual void CheckDirty()
        {
        }

        public virtual void DrawEditor()
        {
        }

        public virtual void CloseEditor()
        {
            CheckDirty();
        }

        public void Close()
        {
            Window?.Close();
        }

        public void Repaint()
        {
            Window?.Repaint();
        }

        public void Show()
        {
            Window?.Show();
        }
    }
}

#endif
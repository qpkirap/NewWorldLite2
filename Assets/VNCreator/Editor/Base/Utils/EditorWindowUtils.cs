#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditorWindow = UnityEditor.EditorWindow;

namespace VNCreator
{
    public partial class EditorUtils
    {
        private static readonly List<EditorWindow> windows = new();
        private static readonly Dictionary<string, EditorWindow> singleWindows = new();

        public static T CreateWindow<T>(
            string title = "",
            float x = 0,
            float y = 0,
            float width = 450,
            float height = 600,
            bool registerOnBuffer = false)
            where T : BaseEditorWindow, new()
        {
            var rect = new Rect(x, y, width, height);
            var window = CreateWindow<T>(rect, title, utility: false, focus: true, registerOnBuffer);

            return window;
        }

        public static T GetWindow<T>(
            string title = "",
            float x = 0,
            float y = 0,
            float width = 450,
            float height = 600,
            bool registerOnBuffer = true)
            where T : BaseEditorWindow, new()
        {
            var rect = new Rect(x, y, width, height);
            var window = GetWindow<T>(rect, title, utility: false, focus: true, registerOnBuffer);

            return window;
        }

        public static T CreateWindow<T>(
            Rect rect,
            string title,
            bool utility,
            bool focus,
            bool registerOnBuffer)
            where T : BaseEditorWindow, new()
        {
            var editor = new T();
            var window = UnityEditorWindow.CreateWindow<EditorWindow>(title);

            window.position = rect;

            SetupWindow(editor, window, utility, focus, registerOnBuffer);

            AddWindow(window);

            return editor;
        }

        public static T GetWindow<T>(
            Rect rect,
            string title,
            bool utility,
            bool focus,
            bool registerOnBuffer)
            where T : BaseEditorWindow, new()
        {
            var editor = new T();
            var window = GetSingleWindow<T>(title, rect);

            SetupWindow(editor, window, utility, focus, registerOnBuffer);

            AddWindow(window);

            return editor;
        }

        private static EditorWindow GetSingleWindow<T>(string title, Rect rect)
            where T : BaseEditorWindow, new()
        {
            var type = typeof(T).AssemblyQualifiedName;

            if (!singleWindows.TryGetValue(type, out var window))
            {
                window = UnityEditorWindow.CreateWindow<EditorWindow>(title);
                window.position = rect;

                singleWindows[type] = window;
            }

            return window;
        }

        private static void SetupWindow<T>(
            T editor,
            EditorWindow window,
            bool utility,
            bool focus,
            bool registerOnBuffer)
            where T : BaseEditorWindow, new()
        {
            window.SetupWindow(editor);
            editor.SetupWindow(window);

            window.minSize = GetMinSize(window.position);
            window.titleContent = new GUIContent(window.Title);

            if (utility)
            {
                window.ShowUtility();
            }

            if (focus)
            {
                window.Focus();
            }

            if (registerOnBuffer)
            {
                EditorWindowBuffer.Register(window);
            }
        }

        private static Vector2 GetMinSize(Rect rect)
        {
            return new()
            {
                x = Mathf.Min(350, rect.width),
                y = Mathf.Min(200, rect.height)
            };
        }

        private static void AddWindow(EditorWindow window)
        {
            if (window != null && !windows.Contains(window))
            {
                windows.Add(window);

                window.OnClosed += WindowClosed;
            }
        }

        private static void WindowClosed(EditorWindow window)
        {
            windows.Remove(window);
            singleWindows.Remove(window.RootEditorType);

            window.OnClosed -= WindowClosed;
        }

        private static IEnumerable<EditorWindow> GetOpenedWindows()
        {
            CheckOpenedWindows();

            return windows;
        }

        private static void CheckOpenedWindows()
        {
            if (windows.Any()) return;

            var customEditorType = typeof(EditorWindow);
            var methodName = nameof(EditorWindow.HasOpenInstances);

            var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                {
                    return customEditorType.IsAssignableFrom(x)
                        && !x.IsInterface
                        && !x.IsAbstract
                        && !x.ContainsGenericParameters;
                });

            foreach (var type in types)
            {
                if ((bool)type.GetMethod(methodName, flags).MakeGenericMethod(type).Invoke(null, null))
                {
                    var window = UnityEditorWindow.GetWindow(type) as EditorWindow;

                    window.CheckDirty();

                    AddWindow(window);
                }
            }
        }

        public static T[] FindEditorsByType<T>() where T : BaseEditorWindow
        {
            var editorType = typeof(T).AssemblyQualifiedName;

            return Resources
                .FindObjectsOfTypeAll<EditorWindow>()
                .Where(x => x.RootEditorType == editorType)
                .Select(x => x.RootEditor)
                .OfType<T>()
                .ToArray();
        }
    }
}

#endif
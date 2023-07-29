using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VNCreator
{
    public partial class EditorUtils
    {
        public static void DrawValueEditor<T, TFilter>(this object obj, string title, string fieldName, Action<T> onChanged = null)
            where T : Object
            where TFilter : Object
        {
            obj.DrawValueEditor<T, TFilter>(new GUIContent(title, title), fieldName, onChanged);
        }
        
        public static void DrawValueEditor<T, TFilter>(this object obj, GUIContent title, string fieldName, Action<T> onChanged = null)
            where T : Object
            where TFilter : Object
        {
            CreateChangeBlock(
                onDraw: () =>
                {
                    var value = obj.GetValue<T>(fieldName);

                    return string.IsNullOrEmpty(title.text)
                        ? DrawObjectEditor<T, TFilter>(value)
                        : DrawObjectEditor<T, TFilter>(title, value);
                },
                onChanged: (value) =>
                {
                    obj.SetValue(fieldName, value);

                    onChanged?.Invoke((T)value);
                });
        }
        
        public static object DrawObjectEditor<T, TFilter>(GUIContent title, T value)
            where T : Object
            where TFilter : Object
        {
            return EditorGUILayout.ObjectField(title, value, typeof(TFilter), allowSceneObjects: false);
        }

        public static object DrawObjectEditor<T, TFilter>(T value)
            where T : Object
            where TFilter : Object
        {
            return EditorGUILayout.ObjectField(value, typeof(TFilter), allowSceneObjects: false);
        }

        public static object DrawObjectEditor<T, TFilter>(Rect rect, T value)
            where T : Object
            where TFilter : Object
        {
            return EditorGUI.ObjectField(rect, value, typeof(TFilter), allowSceneObjects: false);
        }

        public static T DrawObjectEditor<T>(Rect rect, T value)
            where T : Object
        {
            return DrawObjectEditor<T, T>(rect, value) as T;
        }
        
        public static void CreateChangeBlock<T>(RBlock<T> onDraw, Action<T> onChanged)
        {
            EditorGUI.BeginChangeCheck();

            var value = onDraw();

            if (EditorGUI.EndChangeCheck())
            {
                onChanged?.Invoke(value);
            }
        }
        
        #region BOX
        public static void DrawBox(string title = "", bool listPaddingOn = false, GUIStyle style = null, Action onDrawContent = null)
        {
            var boxStyle = style ?? new GUIStyle(GUI.skin.box);

            if (listPaddingOn) boxStyle.padding = new RectOffset(15, 5, 0, 0);

            EditorGUILayout.BeginVertical(boxStyle);

            if (!string.IsNullOrEmpty(title))
            {
                GUILayout.Label(title, new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 });
            }

            onDrawContent?.Invoke();

            EditorGUILayout.EndVertical();
        }

        public static bool DrawOpenBox(string title, bool opened, bool listPaddingOn = false, Action onDrawContent = null, Action onRemove = null)
        {
            DrawBox(
                listPaddingOn: listPaddingOn,
                onDrawContent: () =>
                {
                    EditorGUILayout.BeginHorizontal();

                    // Title
                    EditorGUILayout.LabelField(title, new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 });

                    // Open
                    var buttonParams = GUIParams.New().WithWidth(32);

                    DrawButton(opened ? "▾" : "▸", buttonParams, () =>
                    {
                        opened = !opened;
                    });

                    // Remove
                    if (onRemove != null)
                    {
                        DrawButton("X", buttonParams.WithColor(GUIParams.red), action: onRemove);
                    }

                    EditorGUILayout.EndHorizontal();

                    if (opened) onDrawContent?.Invoke();
                });

            return opened;
        }

        public static void DrawDataBlock<T>(string title, ref T refData, List<T> datas, string[] options) where T : Object
        {
            EditorGUILayout.BeginHorizontal();

            // data
            var data = refData;
            var currentIndex = datas.IndexOf(data);
            var index = EditorGUILayout.Popup(title, currentIndex, options.ToArray());

            refData = datas.ElementAtOrDefault(index);

            data = refData;

            // open
            DrawButton("OPEN", GUIParams.New().WithWidth(50), () =>
            {
                Selection.activeObject = data;
            });

            EditorGUILayout.EndHorizontal();
        }
        #endregion BOX
        
        #region READ ONLY BLOCK
        public static void ReadOnlyBlock(GUIParams drawParams, Action action)
        {
            var guiEnabled = GUI.enabled;
            GUI.enabled = !drawParams.ReadOnly;

            action?.Invoke();

            GUI.enabled = guiEnabled;
        }
        #endregion READ ONLY BLOCK
        
        #region BUTTONS
        public static void DrawButton(string title, GUIParams drawParams = default, Action action = null)
        {
            DrawButton(new GUIContent(title), drawParams, action);
        }

        public static void DrawButton(GUIContent title, GUIParams drawParams = default, Action action = null)
        {
            ReadOnlyBlock(drawParams, () =>
            {
                var style = drawParams.Style ?? GUI.skin.button;

                SetBgColor(drawParams.Color);

                var buttonResult = drawParams.Texture != null
                    ? GUILayout.Button(
                        content: new GUIContent(drawParams.Texture, title.tooltip),
                        style: new GUIStyle(style) { padding = new(2, 2, 2, 2) },
                        options: drawParams.Options)
                    : GUILayout.Button(
                        content: title,
                        style: style,
                        options: drawParams.Options);

                if (buttonResult)
                {
                    action?.Invoke();
                }

                SetBgColor(drawParams.DefaultColor);
            });
        }

        public static void DrawButton(Rect rect, string title, GUIParams drawParams = default, Action action = null)
        {
            DrawButton(rect, new GUIContent(title), drawParams, action);
        }

        public static void DrawButton(Rect rect, GUIContent title, GUIParams drawParams = default, Action action = null)
        {
            ReadOnlyBlock(drawParams, () =>
            {
                var style = drawParams.Style ?? GUI.skin.button;

                SetBgColor(drawParams.Color);

                var buttonResult = drawParams.Texture != null
                    ? GUI.Button(
                        rect,
                        content: new GUIContent(drawParams.Texture, title.tooltip),
                        style: new GUIStyle(style) { padding = new(2, 2, 2, 2) })
                    : GUI.Button(
                        rect,
                        content: title,
                        style: style);

                if (buttonResult)
                {
                    action?.Invoke();
                }

                SetBgColor(drawParams.DefaultColor);
            });
        }
        #endregion BUTTONS
        
        #region COLOR
        public static void DrawColorIndicator(
            Rect rect,
            bool state,
            string enableColor = "00FF00",
            string disableColor = "FF0000",
            Action onAction = null)
        {
            DrawColorIndicator(rect, state, enableColor.ToColor(), disableColor.ToColor(), onAction);
        }

        public static void DrawColorIndicator(
            Rect rect,
            bool state,
            Color? enableColor,
            Color? disableColor,
            Action onAction = null)
        {
            var defaultColor = GUI.backgroundColor;

            if (!enableColor.HasValue) enableColor = Color.green;
            if (!disableColor.HasValue) disableColor = Color.red;

            GUI.backgroundColor = state
                ? enableColor.Value
                : disableColor.Value;

            if (GUI.Button(rect, ""))
            {
                onAction?.Invoke();
            }

            GUI.backgroundColor = defaultColor;
        }

        private static void SetBgColor(Color? color)
        {
            if (color.HasValue) GUI.backgroundColor = color.Value;
        }
        #endregion
    }
}
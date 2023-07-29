using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VNCreator
{
    public static partial class EditorUtils
    {
        public static GUIParams? saveButtonParams;
        private static readonly List<SerializedObject> dirtyItems = new();
        
        [MenuItem("File/Save All", validate = false, priority = 175)]
        public static void SaveAssets()
        {
            SaveAssets(clearAll: true);
        }
        
        public static void DrawSaveButton()
        {
            if (!saveButtonParams.HasValue)
            {
                saveButtonParams = GUIParams.New()
                    .WithWidth(21)
                    .WithHeight(21)
                    .WithTexture("save_icon");
            }

            DrawButton(
                title: new GUIContent("S", "Сохранение изменений в конфигах"),
                drawParams: saveButtonParams.Value,
                action: SaveAssets);
        }
        
        public static void SaveAssets(bool clearAll = false)
        {
            foreach (var window in GetOpenedWindows()) window.CheckDirty();
            foreach (var item in dirtyItems) SetDirty(item);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ClearDirty(clearAll);
        }
        
        public static void ClearDirty(bool clearAll = false)
        {
            var removes = new List<SerializedObject>();

            dirtyItems.ForEach(item =>
            {
                try
                {
                    if (item.targetObject != null)
                    {
                        EditorUtility.ClearDirty(item.targetObject);
                    }
                }
                catch
                {
                    removes.Add(item);
                }
            });

            foreach (var r in removes)
            {
                dirtyItems.Remove(r);
            }

            if (clearAll)
            {
                dirtyItems.Clear();
            }
        }
        
        public static void SetDirty<T>(T item)
            where T : Object
        {
            if (item != null)
            {
                SetDirty(new SerializedObject(item));
            }
        }
        
        public static void SetDirty(SerializedObject item)
        {
            try
            {
                if (item == null || item.targetObject == null) return;

                if (!dirtyItems.Contains(item))
                {
                    dirtyItems.Add(item);
                }

                item.ApplyModifiedProperties();

                EditorUtility.SetDirty(item.targetObject);
            }
            catch
            {
            }
        }
    }
}
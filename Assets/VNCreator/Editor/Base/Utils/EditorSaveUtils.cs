using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VNCreator
{
    public static partial class EditorUtils
    {
        private static readonly List<SerializedObject> dirtyItems = new();
        
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
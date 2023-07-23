using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VNCreator
{
    public static partial class EditorUtils
    {
        public static object InstantiateEntity(Type entityType)
        {
            if (entityType == null)
            {
                return null;
            }
            
            else if (entityType.IsSubclassOf(typeof(ScriptableEntity)))
            {
                return ScriptableEntity.Create(entityType);
            }
            else
            {
                return entityType.CreateByType();
            }
        }
        
        public static bool AssetExist(Object obj)
        {
            return !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj));
        }
        
        #region SUB ASSETS
        public static void AddSubAssetsToObject<TEntity>(string fieldName, Object targetObj)
            where TEntity : ScriptableObject
        {
            AddSubAssetsToObject(targetObj.GetValue<List<TEntity>>(fieldName), targetObj);
        }

        public static void AddSubAssetsToObject<TEntity>(IEnumerable<TEntity> assets, Object targetObj)
            where TEntity : ScriptableObject
        {
            foreach (var asset in assets)
            {
                AddSubAssetToObject(asset, targetObj);
            }
        }

        public static void AddSubAssetToObject<TEntity>(TEntity asset, Object targetObj)
            where TEntity : ScriptableObject
        {
            asset.hideFlags |= HideFlags.HideInHierarchy;

            AssetDatabase.AddObjectToAsset(asset, targetObj);
        }
        #endregion
    }
}
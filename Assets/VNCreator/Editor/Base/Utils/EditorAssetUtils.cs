using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        
        public static object LoadAsset(Type assetType, Action onError = null, bool quiet = true)
        {
            var typeName = assetType.Name;

            var assetGUID = AssetDatabase
                .FindAssets($"t:{typeName}")
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(assetGUID))
            {
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGUID), assetType);
            }
            else
            {
                if (!quiet)
                {
                    Debug.LogError($"Ассет {typeName} не найден");
                }

                onError?.Invoke();

                return null;
            }
        }
        
        public static T LoadAsset<T>(Action onError = null, bool quiet = true) where T : Object
        {
            return LoadAsset(typeof(T), onError, quiet) as T;
        }
        
        public static bool AssetExist(Object obj)
        {
            return !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj));
        }
        
        public static string CreateAssetPath<TData, TConfig>(TData data, string namePrefix = "", params string[] subFolders)
            where TData : IEntity
            where TConfig : BaseConfig
        {
            var config = LoadAsset<TConfig>();
            var folderPath = $"{config.GetConfigFolderPath()}/{(subFolders.Length > 0 ? string.Join("/", subFolders) : "")}";
            var fileName = $"{namePrefix}_{data.Id}";

            return CreateAssetPath(folderPath, fileName);
        }
        
        public static string CreateAssetPath(string folderPath, string name, string ext = "asset")
        {
            CheckFolder(folderPath);

            return $"{folderPath}/{name}.{ext}";
        }
        
        /// <summary> 
        /// Проверить и создать недостающие папки 
        /// </summary>
        /// <param name="folderPath">Путь к проверяемой папке (Assets/..)</param>
        public static void CheckFolder(string folderPath)
        {
            var parentPath = new DirectoryInfo(folderPath)
                .Parent.FullName
                .Replace("\\", "/")
                .Replace(Application.dataPath, "Assets");

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                CheckFolder(parentPath);

                Directory.CreateDirectory(Application.dataPath.Replace("Assets", "") + folderPath);
            }
        }
        
        public static string GetConfigFolderPath(this BaseConfig config)
        {
            var configFolder = config.GetValue<Object>("configFolder");
            var folderPath = GetAssetPath(configFolder);

            return folderPath;
        }
        
        public static string GetAssetPath(Object asset)
        {
            return AssetDatabase.GetAssetPath(asset);
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
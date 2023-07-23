using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNCreator
{
    public abstract class BaseEditorsFactory<TEntity, TEditorAttr> : IEditorsFactory
        where TEditorAttr : BaseEditorAttribute
    {
        protected Dictionary<string, EditorData> entityMenuCache;
        protected Dictionary<string, EditorData> entityEditorCache;
        
        protected Dictionary<string, TEditorAttr> editorAttributeCache;
        
        protected Collection<Type> entityCollection;
        protected Collection<Type> entityEditorCollection;
        
        protected List<(object entity, ICustomEditor editor)> editors = new();

        public Collection<Type> EntityCollection => entityCollection;
        public Collection<Type> EntityEditorCollection => entityEditorCollection;
        
        public BaseEditorsFactory()
        {
            CheckEditorCache();
        }
        
        protected virtual IEnumerable<EditorData> GetEntityDataList(Dictionary<string, EditorData> entityCache, params object[] filters)
        {
            return entityCache.Values
                .OrderBy(x => x.dataType);
            //.ThenBy(x => x.menuOrder);
        }
        
        public List<ICustomEditor> GetOrCreateEditors<T>(
            IEnumerable<T> entities,
            bool orderedByAttribute = true,
            string cacheName = "")
        {
            var result = entities.Select(entity =>
            {
                return GetOrCreateEditor(entity, cacheName);
            });

            if (orderedByAttribute)
            {
                result = result
                    .OrderBy(x => GetEditorOrder(x))
                    .ThenBy(x => GetEditorTitle(x));
            }

            return result.ToList();
        }
        
        public ICustomEditor GetOrCreateEditor(object entity, string cacheName = "")
        {
            var entityType = entity.GetType();

            // get or create editor
            if (!TryGetEditor(entity, out var editor) && !TryCreateEditor(entity, out editor))
            {
                Debug.LogError($"Не найден редактор для компонента: {entityType.Name}");

                return default;
            }

            // init editor
            entityEditorCache.TryGetValue(entityType.Name, out var editorData);

            switch (editor)
            {
                case BaseEditor entityEditor:
                    entityEditor.SetEditorData(editorData);
                    entityEditor.Init(entity);
                    break;
            }

            return editor;
        }
        
        private bool TryGetEditor(object entity, out ICustomEditor editor)
        {
            var data = editors.FirstOrDefault(x => ReferenceEquals(x.entity, entity));

            editor = data.editor;

            return editor != null;
        }
        
        private bool TryCreateEditor(object entity, out ICustomEditor editor)
        {
            var entityType = entity.GetType();

            if (TryCreateEditor(entityType, out editor))
            {
                editors.Add((entity, editor));

                return true;
            }
            else
            {
                editor = null;

                return false;
            }
        }
        
        private void CheckEditorCache()
        {
            if (editorAttributeCache == null || entityMenuCache == null || entityEditorCache == null)
            {
                CreateEditorCache();

                CreateTypeCollections();
            }
        }
        
        private void CreateEditorCache()
        {
            entityMenuCache = new();
            entityEditorCache = new();
            editorAttributeCache = new();

            var entityType = typeof(TEntity);

            var entityEditors = ReflectionUtils
                .FindChildTypesOf<ICustomEditor>()
                .Where(x =>
                {
                    return x.HasAttribute<TEditorAttr>()
                           && x.GetAttribute<TEditorAttr>().CheckType(entityType);
                });

            foreach (var entityEditor in entityEditors)
            {
                // editor cache
                var editorAttr = entityEditor.GetAttribute<TEditorAttr>();
                var entityTypeName = editorAttr.type.Name;

                editorAttributeCache[entityTypeName] = editorAttr;
                
                var entityData = CreateEntityData(editorAttr);

                // cache
                entityEditorCache[entityTypeName] = entityData;
            }
        }
        
        private void CreateTypeCollections()
        {
            var entityTypes = editorAttributeCache.Values.Select(x => x.type);
            var editorTypes = editorAttributeCache.Values.Select(x => x.editorType);

            entityCollection = new Collection<Type>(
                entityTypes,
                entityTypes.Select(x => x.Name));

            entityEditorCollection = new Collection<Type>(
                editorTypes,
                editorTypes.Select(x => x.Name));
        }
        
        private int GetEditorOrder(ICustomEditor entityEditor)
        {
            if (entityEditor == null) return 0;

            //var entityTypeName = GetEntityTypeName(entityEditor);

            return int.MaxValue;
        }
        
        private string GetEditorTitle(ICustomEditor entityEditor)
        {
            if (entityEditor == null) return "";

            //var entityTypeName = GetEntityTypeName(entityEditor);

            return null;
        }
        
        private string GetEntityTypeName(ICustomEditor entityEditor)
        {
            return entityEditor switch
            {
                BaseEditor baseEditor => baseEditor.Entity.GetType().Name,
                _ => throw new NotImplementedException()
            };
        }
        
        protected virtual EditorData CreateEntityData(TEditorAttr editorAttr)
        {
            return new EditorData(editorAttr.type);
        }
    }
}
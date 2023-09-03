using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNCreator
{
    public abstract class BaseContainerFactory<TEntity, TEditorAttr> : IContainerFactory<TEntity> 
        where TEditorAttr : BaseEditorAttribute
        where TEntity : Component
    {
        protected Dictionary<string, TEditorAttr> editorAttributeCache;
        
        protected List<(object entity, IComponentEntityEditor<TEntity> editor)> editors = new();
        
        public BaseContainerFactory()
        {
            CreateEditorCache();
        }
        
        public List<IComponentEntityEditor<TEntity>> GetOrCreateEditors(IEnumerable<TEntity> entities)
        {
            var result = entities.Select(GetOrCreateEditor);

            return result.ToList();
        }
        
        public IComponentEntityEditor<TEntity> GetOrCreateEditor(TEntity entity)
        {
            var entityType = entity.GetType();

            // get or create editor
            if (!TryGetEditor(entity, out var editor) && !TryCreateEditor(entityType, out editor))
            {
                Debug.LogError($"Не найден редактор для компонента: {entityType.Name}");

                return default;
            }

            return editor;
        }
        
        public IComponentEntityEditor<TEntity> CreateEditor(Type entityType)
        {
            // get or create editor
            if (!TryCreateEditor(entityType, out var editor))
            {
                Debug.LogError($"Не найден редактор для компонента: {entityType.Name}");

                return default;
            }

            return editor;
        }
        
        private void CreateEditorCache()
        {
            editorAttributeCache = new();

            var entityType = typeof(TEntity);

            var entityEditors = ReflectionUtils
                .FindChildTypesOf<IComponentEntityEditor<TEntity>>()
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
            }
        }
        
        private bool TryGetEditor(object entity, out IComponentEntityEditor<TEntity> editor)
        {
            var data = editors.FirstOrDefault(x => ReferenceEquals(x.entity, entity));

            editor = data.editor;

            return editor != null;
        }
        
        private bool TryCreateEditor(Type entityType, out IComponentEntityEditor<TEntity> editor)
        {
            editor = null;

            if (editorAttributeCache.TryGetValue(entityType.Name, out var editorAttr))
            {
                if (editorAttr.editorType.IsSubclassOf(typeof(ComponentEntityEditor<TEntity>)))
                {
                    editor = editorAttr.editorType.CreateByType<ComponentEntityEditor<TEntity>>();
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public interface IContainerFactory<TEntity>
        where TEntity : Component
    {
        IComponentEntityEditor<TEntity> CreateEditor (Type entityType);
    }
}
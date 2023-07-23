using System;
using UnityEditor;
using UnityEngine;

namespace VNCreator
{
    /// <summary>
    /// Базовый редактор <see cref="TEntity"/>
    /// </summary>
    public abstract class BaseEntityEditor<TEntity> : BaseEntityEditor
        where TEntity : class
    {
        public new TEntity Entity => base.Entity as TEntity;
    }
    
    public abstract class BaseEntityEditor : BaseEditor
    {
        public bool IsSubEntity { get; private set; }
        
        protected virtual string GetDataPath(ScriptableObject entity)
        {
            throw new NotImplementedException();
        }
        
        public void SetSubEntityState(bool isSubEntity)
        {
            IsSubEntity = isSubEntity;
        }
        
        #region [Instantiate, Clone, Remove]
        /// <summary>
        /// Создать новую сущность и записать ее в контейнер
        /// </summary>
        public object CreateTo(string fieldName, object container)
        {
            var entity = InstantiateEntity(container);

            container.SetValue(fieldName, entity);

            return entity;
        }

        public override object InstantiateEntity(object container)
        {
            if (!ContainerIsExist(container))
            {
                return null;
            }

            var entity = base.InstantiateEntity(container);

            SaveEntity(entity, container);

            return entity;
        }

        /// <summary>
        /// Проверить контейнер 
        /// </summary>
        /// <param name="container">Контейнер сущности</param>
        /// <returns>Результат проверки</returns>
        private bool ContainerIsExist(object container)
        {
            return !IsSubEntity || (container is ScriptableObject sc && EditorUtils.AssetExist(sc));
        }

        /// <summary>
        /// Клонировать сущность и записать в контейнер
        /// </summary>
        /// <param name="fieldName">Имя поля сущности</param>
        /// <param name="container">Контейнер сущности</param>
        /// <returns>Клон сущности</returns>
        public object CloneTo(string fieldName, object container)
        {
            var entity = CloneEntity(container);

            container.SetValue(fieldName, entity);

            return entity;
        }

        public override object CloneEntity(object container)
        {
            var newEntity = base.CloneEntity(container);

            if (IsSubEntity && newEntity is IEntity e)
            {
                e.RemoveCloneSuffix();
            }

            SaveEntity(newEntity, container);

            return newEntity;
        }

        /// <summary>
        /// Сохранить ассет сущности
        /// </summary>
        /// <param name="entity">Сущность</param>
        public void SaveEntity(object entity)
        {
            SaveEntity(entity, Container);
        }

        /// <summary>
        /// Сохранить ассет сущности
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <param name="container">Контейнер сущности</param>
        public void SaveEntity(object entity, object container)
        {
            if (entity is ScriptableObject sEntity)
            {
                if (IsSubEntity)
                {
                    EditorUtils.AddSubAssetToObject(sEntity, container as ScriptableObject);
                }
                else
                {
                    AssetDatabase.CreateAsset(sEntity, GetDataPath(sEntity));
                }
            }
        }

        public override void RemoveEntity()
        {
            if (Entity is ScriptableObject sEntity)
            {
                if (IsSubEntity)
                {
                    AssetDatabase.RemoveObjectFromAsset(sEntity);
                }
                else
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sEntity));
                }
            }

            base.RemoveEntity();
        }
        #endregion [Instantiate, Clone, Remove]
    }
}
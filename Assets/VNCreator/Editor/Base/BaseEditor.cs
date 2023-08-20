using System;
using UnityEditor;
using UnityEngine;

namespace VNCreator
{
    public abstract class BaseEditor : ICustomEditor
    {
        public event Action<object> OnCloned;
        public event Action<object> OnRemoved;
        
        [SerializeField] private object entity;
        [SerializeField] private object container;
        
        protected EditorData data;

        public object Entity => entity;
        public object Container => container;

        public virtual void Init(string fieldName, object container)
        {
            var entity = container?.GetValue<object>(fieldName);

            Init(entity, container);
        }

        public virtual void Init(object entity, object container = null)
        {
            this.entity = entity;
            this.container = container;
        }
        
        /// <summary>
        /// Задать данные редактора сущности
        /// </summary>
        /// <param name="data">Данные редактора сущности</param>
        public void SetEditorData(EditorData data)
        {
            this.data = data;
        }

        public virtual void DrawEditor()
        {
        }

        public virtual void CloseEditor()
        {
            CheckDirty();
        }

        public virtual void CheckDirty()
        {
            if (Entity is ScriptableObject sEntity)
            {
                EditorUtils.SetDirty(sEntity);
            }
        }
        
        protected virtual void CloneCallback()
        {
            var newEntity = CloneEntity();

            OnCloned?.Invoke(newEntity);
        }

        protected virtual bool RemoveCallback()
        {
            if (DrawRemoveDialog())
            {
                RemoveEntity();

                OnRemoved?.Invoke(Entity);

                return true;
            }
            else
            {
                return false;
            }
        }
        
        protected virtual bool DrawRemoveDialog()
        {
            return EditorUtility.DisplayDialog($"Удалить?", "Удалить", "Удалить", "Отмена");
        }

        /// <summary>
        /// Создать клон сущности
        /// </summary>
        public object CloneEntity()
        {
            return CloneEntity(Container);
        }

        /// <summary>
        /// Создать клон сущности
        /// </summary>
        public virtual object CloneEntity(object container)
        {
            if (Entity is ICloneable cEntity)
            {
                return cEntity.Clone();
            }
            else
            {
                Debug.LogWarning("Клонирование невозможно! " +
                                 "Entity не существует или требуется реализация ICloneable интерфейса");

                return default;
            }
        }
        
        public virtual object InstantiateEntity(object container)
        {
            var entityType = data?.dataType ?? this.GetAttribute<BaseEditorAttribute>().type;

            return EditorUtils.InstantiateEntity(entityType);
        }
        
        /// <summary>
        /// Удалить сущность
        /// </summary>
        public virtual void RemoveEntity()
        {
        }
    }
}
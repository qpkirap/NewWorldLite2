using System.Collections.Generic;
using UnityEngine;

namespace VNCreator
{
    public class EntitiesListContainer<T> where T : Component
    {
        protected static BaseEntityEditor entityEditor { get; private set; }
        protected static List<T> entities;
        protected static List<T> selectedItems = new();
        
        protected readonly string fieldName;
        protected readonly object container;

        public EntitiesListContainer(string fieldName, object container)
        {
            if (string.IsNullOrEmpty(fieldName) || container == null) return;

            this.fieldName = fieldName;
            this.container = container;

            var list = container.GetValue<List<T>>(this.fieldName);

            if (list != null) entities = list;
            
            entityEditor ??= EditorCache.GetEditor<T>();
            
            entityEditor.SetSubEntityState(true);
        }

        public virtual T CreateItem()
        {
            var item = (T)entityEditor.InstantiateEntity(container);
            
            entities.SetValue("values", item);

            return item;
        }

        public virtual void OnSelectItem(T component)
        {
            Debug.Log($"Selected: {component.Id}");
            
            selectedItems.Add(component);
        }

        public virtual void OnUnselected(T component)
        {
            selectedItems.Remove(component);
        }

        public virtual void OnDelete(T component)
        {
            for (var i = 0; i < selectedItems.Count; i++)
            {
                var item = selectedItems[i];
                
                if (item != null)
                {
                    entityEditor.Init(item, container);
                
                    entityEditor.RemoveEntity();

                    entities.Remove(item);
                }
            }
            
            entities.RemoveAll(x => x == null);
            
            selectedItems.Clear();
        }
    }
}
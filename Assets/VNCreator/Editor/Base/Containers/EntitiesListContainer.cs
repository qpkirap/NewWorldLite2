using System.Collections.Generic;
using System.Linq;

namespace VNCreator
{
    public class EntitiesListContainer<T> where T : Component
    {
        protected readonly string fieldName;
        protected readonly object container;

        protected readonly List<T> entities;
        protected BaseEntityEditor entityEditor { get; private set; }
        protected Component lastSelect { get; private set; }

        public EntitiesListContainer(string fieldName, object container)
        {
            if (string.IsNullOrEmpty(fieldName) || container == null) return;

            this.fieldName = fieldName;
            this.container = container;

            var list = container.GetValue<List<T>>(this.fieldName);

            if (list != null) entities = list;
            
            entityEditor = EditorCache.GetComponentEditor(typeof(T));
            
            entityEditor.SetSubEntityState(true);
        }

        public virtual T CreateItem()
        {
            var item = (T)entityEditor.InstantiateEntity(container);
            
            entities.SetValue("values", item);

            return item;
        }

        public virtual void OnSelectItem(Component component)
        {
            lastSelect = component;
        }

        public virtual void OnDelete(Component component)
        {
            if (component == null) return;
            
            var item = entities.FirstOrDefault(x => x == component);

            if (item != null)
            {
                entityEditor.Init(item, container);
                
                entityEditor.RemoveEntity();

                entities.Remove(item);

                entities.RemoveAll(x => x == null);
            }
        }
    }
}
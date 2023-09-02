namespace VNCreator
{
    public class EntityContainer<T> where T : Component
    {
        protected readonly string fieldName;
        protected readonly object container;
        
        protected BaseEntityEditor entityEditor { get; private set; }
        protected bool IsSelect { get; private set; }
        
        private T entity;
        
        public EntityContainer(string fieldName, object container)
        {
            if (string.IsNullOrEmpty(fieldName) || container == null) return;

            this.fieldName = fieldName;
            this.container = container;

            entity = (T)this.container.GetValue(this.fieldName);

            entityEditor = EditorCache.GetEditor<T>();
            
            entityEditor.SetSubEntityState(true);
        }

        public virtual T CreateItem()
        {
            var item = (T)entityEditor.InstantiateEntity(container);

            if (entity != null)
            {
                entityEditor.Init(entity, container);
                
                entityEditor.RemoveEntity();
            }

            entity = item;

            return item;
        }

        public virtual void OnSelectItem(Component component)
        {
            IsSelect = component == entity;
        }

        public virtual void OnDelete(Component component)
        {
            if (entity != null && entity == component)
            {
                entityEditor.Init(entity, container);
                
                entityEditor.RemoveEntity();
            }

            entity = null;
        }
    }
}
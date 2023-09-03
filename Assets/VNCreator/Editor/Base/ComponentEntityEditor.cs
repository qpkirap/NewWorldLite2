namespace VNCreator
{
    public class ComponentEntityEditor<T> : IComponentEntityEditor<T>
        where T : Component
    {
        protected EntityContainer<T> entityContainer;

        public virtual void InitContainer(string fieldName, object container)
        {
            entityContainer = new(fieldName, container);
        }

        public T CreateItem() => entityContainer.CreateItem();
        
        public void OnSelectItem(T component) => entityContainer.OnSelectItem(component);
        public void OnUnselected() => entityContainer.OnUnselected();
        public void OnDelete(T component) => entityContainer.OnDelete(component);
    }
}
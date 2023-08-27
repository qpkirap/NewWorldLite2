namespace VNCreator
{
    public class ComponentEntityEditor<T> : BaseEntityEditor, IComponentEntityEditor<T>
        where T : Component
    {
        private EntityContainer<T> entityContainer;

        public void InitContainer(string fieldName, object container)
        {
            entityContainer = new(fieldName, container);
        }

        public T CreateItem() => entityContainer.CreateItem();
        
        public void OnSelectItem(Component component) => entityContainer.OnSelectItem(component);
        
        public void OnDelete(Component component) => entityContainer.OnDelete(component);
    }
}
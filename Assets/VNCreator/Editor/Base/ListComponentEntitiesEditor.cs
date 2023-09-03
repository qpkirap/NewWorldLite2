namespace VNCreator
{
    public class ListComponentEntitiesEditor<T> : IComponentEntityEditor<T>
        where T : Component
    {
        private EntitiesListContainer<T> listContainer;
        
        public void InitContainer(string fieldName, object container)
        {
            listContainer = new(fieldName, container);
        }
        
        public T CreateItem() => listContainer.CreateItem();
        
        public void OnSelectItem(T component) => listContainer.OnSelectItem(component);
        public void OnUnselected(T component) => listContainer.OnUnselected(component);
        public void OnDelete(T component) => listContainer.OnDelete(component);
    }
}
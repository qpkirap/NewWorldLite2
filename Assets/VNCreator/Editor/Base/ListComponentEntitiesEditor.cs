namespace VNCreator
{
    public class ListComponentEntitiesEditor<T> : BaseEntityEditor, IComponentEntityEditor<T>
        where T : Component
    {
        private EntitiesListContainer<T> listContainer;
        

        public void InitContainer(string fieldName, object container)
        {
            listContainer = new(fieldName, container);
        }
        
        public T CreateItem() => listContainer.CreateItem();
        
        public void OnSelectItem(Component component) => listContainer.OnSelectItem(component);
        
        public void OnDelete(Component component) => listContainer.OnDelete(component);
    }
}
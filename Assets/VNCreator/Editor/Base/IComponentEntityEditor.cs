namespace VNCreator
{
    public interface IComponentEntityEditor<out T> 
        where T : Component
    {
        void InitContainer(string fieldName, object container);
        
        public T CreateItem();

        public void OnSelectItem(Component component);
        
        public void OnDelete(Component component);
    }
}
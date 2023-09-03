namespace VNCreator
{
    public interface IComponentEntityEditor<T> where T : Component
    {
        void InitContainer(string fieldName, object container);
        
        public T CreateItem();

        public void OnSelectItem(T component);
        public void OnUnselected();
        
        public void OnDelete(T component);
    }
}
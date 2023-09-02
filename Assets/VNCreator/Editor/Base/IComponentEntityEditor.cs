namespace VNCreator
{
    public interface IComponentEntityEditor<T> : ICustomEditor
        where T : Component
    {
        void InitContainer(string fieldName, object container);
        
        public T CreateItem();

        public void OnSelectItem(T component);
        
        public void OnDelete(T component);
    }
}
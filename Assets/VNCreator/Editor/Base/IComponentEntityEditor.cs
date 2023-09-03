namespace VNCreator
{
    public interface IComponentEntityEditor<T> where T : Component
    {
        void InitContainer(string fieldName, object container);
        
        public T CreateItem();
        void OnSelectItem(T component);
        void OnUnselected(T component);
        void OnDelete(T component);
    }
}
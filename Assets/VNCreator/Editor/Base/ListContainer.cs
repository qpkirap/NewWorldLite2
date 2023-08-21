using System.Collections.Generic;

namespace VNCreator
{
    public abstract class ListContainerEditor<T>
        where T : Component
    {
        protected readonly string fieldName;
        protected readonly object container;
        
        protected readonly List<T> entities;
        protected BaseEntityEditor entityEditor;

        protected ListContainerEditor(string fieldName, object container)
        {
            if (string.IsNullOrEmpty(fieldName) || container == null) return;

            this.fieldName = fieldName;
            this.container = container;

            var list = container.GetValue<List<T>>(fieldName);

            if (list != null) entities = list;
        }

        protected virtual void Init()
        {
            entityEditor = EditorCache.GetEditor(typeof(T));
            
            entityEditor.SetSubEntityState(true);
        }
    }
}
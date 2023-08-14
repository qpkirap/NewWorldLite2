#if UNITY_EDITOR

using Node = UnityEditor.Experimental.GraphView.Node;

namespace VNCreator
{
    public abstract class BaseNode<T> : BaseNode 
        where T : Component
    {
        protected BaseEntityEditor editorCache;
        
        public T EntityCache { get; }
        
        protected BaseNode(string fieldName, object container, T entityCache = null) : base(fieldName, container)
        {
            editorCache = EditorCache.GetEditor(typeof(T));
            
            editorCache.SetSubEntityState(true);
            
            editorCache.Init(fieldName, container);

            EntityCache = entityCache;
            
            EntityCache ??= (T)editorCache.InstantiateEntity(container);
        }

        public override BaseEntityEditor GetEditor()
        {
            if (editorCache != null) return editorCache;
            else editorCache ??= EditorCache.GetEditor(typeof(T));

            return editorCache;
        }
    }
    
    public abstract class BaseNode : Node
    {
        protected object Container;
        
        public BaseNode(string fieldName, object container)
        {
            this.Container = container;
            
            Init(fieldName, container);
        }

        private void Init(string fieldName, object container)
        {
            GetEditor()?.Init(fieldName, Container);
        }

        public abstract BaseEntityEditor GetEditor();
        public abstract string Guid { get; }
        public abstract NodeType NodeType { get; }
    }
}

#endif
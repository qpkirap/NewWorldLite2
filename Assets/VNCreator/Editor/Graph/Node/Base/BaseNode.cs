#if UNITY_EDITOR

using Node = UnityEditor.Experimental.GraphView.Node;

namespace VNCreator
{
    public abstract class BaseNode<TComponent, TEditor> : BaseNode 
        where TComponent : Component
        where TEditor : IComponentEntityEditor<TComponent>
    {
        private readonly TEditor editorCache;
        
        public TComponent EntityCache { get; }
        
        protected BaseNode(string fieldName, object container, TComponent entityCache = null) : base(fieldName, container)
        {
            this.EntityCache = entityCache;
            this.editorCache = (TEditor)EditorCache.GetComponentEditor(typeof(TEditor));
            
            this.editorCache.InitContainer(fieldName, container);

            EntityCache ??= editorCache.CreateItem();
        }

        public override void OnDelete()
        {
            editorCache.OnDelete(EntityCache);
        }

        public override void OnSelected()
        {
            editorCache.OnSelectItem(EntityCache);
            
            base.OnSelected();
        }
    }
    
    public abstract class BaseNode : Node
    {
        protected object Container;
        
        public BaseNode(string fieldName, object container)
        {
        }
        
        public abstract NodeType NodeType { get; }
        public abstract string Guid { get; }

        public abstract void OnDelete();
    }
}

#endif
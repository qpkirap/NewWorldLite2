#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
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

            EntityCache = entityCache;

            if (EntityCache == null)
            {
                EntityCache = (T)editorCache.InstantiateEntity(container);

                var target = container.GetValue(fieldName);
                
                target.SetValue("values", EntityCache);
            }
            
            editorCache.Init(EntityCache, Container);
        }

        public override void OnDelete()
        {
            GetEditor().RemoveEntity();
            
            
            
            base.OnDelete();
        }

        protected override BaseEntityEditor GetEditor()
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
            GetEditor()?.Init(fieldName, container);
        }

        protected abstract BaseEntityEditor GetEditor();
        public abstract NodeType NodeType { get; }
        public abstract string Guid { get; }

        public virtual void OnDelete()
        {
        }
    }
}

#endif
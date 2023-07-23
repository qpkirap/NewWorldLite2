using System;
using System.Linq;

namespace VNCreator
{
    public class ComponentEntityData : EditorData
    {
        public readonly Type[] containerFilter;

        public ComponentEntityData(EditorData entityData, Type[] containerFilter) : base(entityData)
        {
            this.containerFilter = containerFilter;
        }

        public bool CheckContainer(Type containerType)
        {
            return containerFilter == null
                   || containerFilter.Length == 0
                   || containerFilter.Any(x => x.IsAssignableFrom(containerType));
        }
    }
}
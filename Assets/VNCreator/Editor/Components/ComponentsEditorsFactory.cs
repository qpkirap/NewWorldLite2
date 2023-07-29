using System;
using System.Collections.Generic;
using System.Linq;

namespace VNCreator
{
    public class ComponentsEditorsFactory <TComponent> : BaseEditorsFactory<TComponent, StoryObjectEditorAttribute>
        where TComponent : Component
    {
        protected override IEnumerable<EditorData> GetEntityDataList(Dictionary<string, EditorData> entityCache, params object[] filters)
        {
            var containerTypes = filters.OfType<Type>();

            return base.GetEntityDataList(entityCache, filters)
                .Where(x =>
                {
                    return x is ComponentEntityData data
                           && containerTypes.Any(type => data.CheckContainer(type));
                });
        }

        protected override EditorData CreateEntityData(StoryObjectEditorAttribute editorAttr)
        {
            var data = base.CreateEntityData(editorAttr);

            return new ComponentEntityData(data, editorAttr.containerFilter);
        }
    }
}
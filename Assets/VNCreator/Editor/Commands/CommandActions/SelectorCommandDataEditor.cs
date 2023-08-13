using System;
using System.Collections.Generic;
using System.Linq;

namespace VNCreator
{
    public class SelectorCommandDataEditor  : BaseEntityEditor
    {
        public Collection<Type> commandActionCollection;

        private Dictionary<Type, BaseEntityEditor> editorCache;
        
        private CommandData Data => Entity as CommandData;
        
        private BaseEntityEditor currentEditor;
        
        public override void Init(object entity, object container = null)
        {
            UpdateCollection();

            base.Init(entity, container);
        }

        public BaseEntityEditor GetEditor(Type type)
        {
            editorCache ??= new();

            if (type == null) return null;

            if (editorCache.TryGetValue(type, out var editorC))
            {
                return editorC;
            }

            var editor = EditorCache.GetEditor(type);

            if (editor == null) return null;

            var convert = editor as CommandActionEditor;

            editorCache.TryAdd(type, convert);

            return convert;
        }
        
        private void UpdateCollection()
        {
            var items = ReflectionUtils.FindChildTypesOf<CommandAction>();

            var options = items.Select(x => x.ToString());

            commandActionCollection = new Collection<Type>(items, options, autoSort: false);
        }
    }
}
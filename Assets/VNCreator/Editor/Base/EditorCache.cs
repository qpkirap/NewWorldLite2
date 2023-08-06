using System;
using System.Collections.Generic;

namespace VNCreator
{
    //TODO нужно разделить Editor и Runtime
    public static class EditorCache
    {
        public static CommandComponentsEditorsFactory CommandComponentsEditorsFactory;

        private static readonly Dictionary<Type, BaseEntityEditor> EditorsCache = new();

        public static void Init()
        {
            CommandComponentsEditorsFactory ??= new();
        }
        
        public static BaseEntityEditor GetEditor(Type type)
        {
            if (type == null) return null;

            if (EditorsCache.TryGetValue(type, out var editorC))
            {
                return editorC;
            }
            
            var editor = CommandComponentsEditorsFactory.CreateEditor(type);

            if (editor == null) return null;

            var convert = editor as BaseEntityEditor;

            EditorsCache.TryAdd(type, convert);

            return convert;
        }
    }
}
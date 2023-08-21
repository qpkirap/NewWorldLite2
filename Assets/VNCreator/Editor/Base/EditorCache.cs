using System;
using System.Collections.Generic;
using System.Linq;

namespace VNCreator
{
    //TODO нужно разделить Editor и Runtime
    public static class EditorCache
    {
        public static void Init()
        {
        }
        
        public static BaseEntityEditor GetEditor(Type type)
        {
            if (type == null) return null;
            
            var factory = CreateFactory(type);

            if (factory == null) return null;

            var editor = (BaseEntityEditor)factory.CreateEditor(type);

            return editor;
        }
        
        private static IEditorsFactory CreateFactory(Type componentType)
        {
            var factoryType = ReflectionUtils
                .FindChildTypesOf(typeName: "ComponentsEditorsFactory")
                .FirstOrDefault(
                    x =>
                    {
                        return x.BaseType
                            .GetGenericArguments()
                            .Contains(componentType);
                    });

            return factoryType != null
                ? factoryType.CreateByType<IEditorsFactory>()
                : default;
        }
    }
}
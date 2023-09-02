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
        
        public static IComponentEntityEditor<T> GetComponentEditor<T>() where T : Component
        {
            var type = typeof(T);
            
            var factory = CreateFactory(type);

            var editor = (IComponentEntityEditor<T>)factory?.CreateEditor(type);

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
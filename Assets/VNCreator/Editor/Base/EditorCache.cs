using System;
using System.Linq;

namespace VNCreator
{
    //TODO нужно разделить Editor и Runtime
    public static class EditorCache
    {
        public static void Init()
        {
        }

        public static IComponentEntityEditor<T> GetComponentEntityEditor<T>()
            where T : Component
        {
            var type = typeof(T);

            var factory = CreateContainerFactory<T>(type);
            
            if (factory == null) return null;
            
            var editor = (IComponentEntityEditor<T>)factory.CreateEditor(type);
            
            return editor;
        }

        public static BaseEntityEditor GetEditor(Type type)
        {
            var factory = CreateFactory(type);

            if (factory == null) return null;

            var editor = (BaseEntityEditor)factory.CreateEditor(type);

            return editor;
        }
        
        public static BaseEntityEditor GetEditor<T>()
        {
            var type = typeof(T);

            return GetEditor(type);
        }

        private static IContainerFactory<TEntity> CreateContainerFactory<TEntity>(Type componentType)
            where TEntity : Component
        {
            var factoryType = ReflectionUtils
                .FindChildTypesOf(typeName: "ContainerFactory")
                .FirstOrDefault(
                    x =>
                    {
                        return x.BaseType
                            .GetGenericArguments()
                            .Contains(componentType);
                    });
            
            return factoryType != null
                ? factoryType.CreateByType<IContainerFactory<TEntity>>()
                : default;
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
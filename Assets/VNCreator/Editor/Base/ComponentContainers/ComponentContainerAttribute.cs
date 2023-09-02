using System;

namespace VNCreator
{
    /// <summary>
    /// Аттрибут редактора компонента
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ComponentContainerAttribute : BaseEditorAttribute
    {
        /// <summary> 
        /// Фильтр контейнеров, к которым принадлежит компонент 
        /// </summary>
        public readonly Type[] containerFilter;

        /// <summary>
        /// Аттрибут редактора компонента
        /// </summary>
        /// <param name="type">Тип компонента</param>
        /// <param name="editorType">Тип редактора компонента</param>
        /// <param name="containerFilter">Фильтр контенеров, к которым принадлежит компонент</param>
        public ComponentContainerAttribute(Type type, Type editorType, params Type[] containerFilter) : base(type, editorType)
        {
            this.containerFilter = containerFilter;
        }
    }
}
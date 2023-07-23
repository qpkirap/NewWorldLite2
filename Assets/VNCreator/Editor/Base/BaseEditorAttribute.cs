using System;

namespace VNCreator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BaseEditorAttribute : Attribute
    {
        public readonly Type type;
        public readonly Type editorType;

        /// <summary>
        /// Аттрибут редктора сущности
        /// </summary>
        /// <param name="type">Тип сущности</param>
        /// <param name="editorType">Тип редактора сущности</param>
        public BaseEditorAttribute(Type type, Type editorType)
        {
            this.type = type;
            this.editorType = editorType;
        }

        public bool CheckType(Type type)
        {
            return type.IsAssignableFrom(this.type);
        }
    }
}
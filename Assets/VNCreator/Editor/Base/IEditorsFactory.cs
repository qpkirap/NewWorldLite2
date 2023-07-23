using System;
using System.Collections.Generic;

namespace VNCreator
{
    public interface IEditorsFactory
    {
        /// <summary>
        /// Получить список редакторов для сущностей
        /// </summary>
        /// <param name="entities">Список сущностей</param>
        /// <param name="orderedByAttribute">Сортировать элементы используя данные атрибута?<br/><see cref="MenuAttribute.order"/></param>
        /// <returns>Список редакторов для сущностей</returns>
        List<ICustomEditor> GetOrCreateEditors<T>(IEnumerable<T> entities, bool orderedByAttribute = true);

        /// <summary>
        /// Получить редактор для сущности
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <returns>Редактор сущности</returns>
        ICustomEditor GetOrCreateEditor(object entity);

        /// <summary>
        /// Получить редактор для сущности
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <returns>Редактор сущности</returns>
        TEditor GetOrCreateEditor<TEditor>(object entity) where TEditor : ICustomEditor;

        /// <summary>
        /// Получить редактор по типу сущности
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <returns>Редактор сущности</returns>
        ICustomEditor CreateEditor(Type entityType);

        /// <summary>
        /// Получить список всех элементов
        /// </summary>
        /// <param name="filters">Данные для фильтрации списка меню</param>
        /// <returns>Коллекция всех элементов</returns>
        Collection<EditorData> GetDataCollection(params object[] filters);

        /// <summary>
        /// Получить тип редактора
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <returns>Тип редактора</returns>
        Type GetEditorType(Type entityType);

        /// <summary>
        /// Получить атрибут редактора
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <returns>Атрибут редактора</returns>
        BaseEditorAttribute GetEditorAttribute(Type entityType);

    }
}
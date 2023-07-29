using UnityEngine;

namespace VNCreator
{
    /// <summary>
    /// Делегат рендера элемента списка
    /// </summary>
    /// <typeparam name="TEntity">Тип отображаемой сущности</typeparam>
    /// <param name="index">Индекс элемента в списке</param>
    /// <param name="entity">Элемент списка</param>
    /// <param name="r">Данные о форме элемента</param>
    /// <param name="x">r.x</param>
    /// <param name="rX">r.x + r.width</param>
    /// <param name="y">r.y</param>
    /// <param name="w">r.width</param>
    /// <param name="h">r.height</param>
    // index, entity, r, x, rX, y, w, h
    public delegate void ElementCallbackDelegate<TEntity>(int index, TEntity entity, Rect r, float x, float rX, float y, float w, float h);

    /// <summary>
    /// Делегат с возвращаемым значением
    /// </summary>
    /// <typeparam name="T1">Тип возвращаемого значения</typeparam>
    public delegate T1 RBlock<out T1>();

    /// <summary>
    /// Делегат с возвращаемым значением
    /// </summary>
    /// <typeparam name="T1">Тип возвращаемого значения</typeparam>
    public delegate T1 RBlock<out T1, in T2>(T2 t1);
}
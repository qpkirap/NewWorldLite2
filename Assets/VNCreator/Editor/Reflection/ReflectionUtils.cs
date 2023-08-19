using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VNCreator
{
    public static class ReflectionUtils
    {
        public const BindingFlags baseFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        public const BindingFlags instanceFlags = baseFlags | BindingFlags.Instance;
        public const BindingFlags flattenFlags = baseFlags | BindingFlags.FlattenHierarchy;

        private static readonly Type serializedFieldType = typeof(SerializeField);

        private static List<Type> allTypesCache;
        public static List<Type> AllTypesCache => allTypesCache ??= GetAllTypes();

        #region GET MEMBERS

        public static List<(TAttribute attr, MemberData data)> GetMembersByAttribute<TAttribute>(
            this object obj,
            string basePath = "",
            int depth = 0,
            int maxDepth = 4)
            where TAttribute : Attribute
        {
            var type = obj.GetType();

            return type.GetMembersByAttribute<TAttribute>(basePath, depth, maxDepth);
        }

        public static List<(TAttribute attr, MemberData data)> GetMembersByAttribute<TAttribute>(
            this Type type,
            string basePath = "",
            int depth = 0,
            int maxDepth = 4)
            where TAttribute : Attribute
        {
            var methods = new List<MemberData>();

            type.GetMembersByAttribute<TAttribute>(methods, basePath, depth, maxDepth);

            return methods
                .Where(x => x.HasAttribute<TAttribute>())
                .Select(x => (x.GetAttribute<TAttribute>(), x))
                .ToList();
        }

        public static void GetMembersByAttribute<T>(
            this Type type,
            List<MemberData> paths,
            string basePath = "",
            int depth = 0,
            int maxDepth = 4)
            where T : Attribute
        {
            depth++;

            if (type == null || depth > maxDepth)
            {
                return;
            }

            var members = type
                .GetMembers(instanceFlags)
                .Where(x => x.IsDefined(typeof(T)) || x.IsDefined(serializedFieldType));

            foreach (var member in members)
            {
                if (member.IsDefined(typeof(T)))
                {
                    foreach (var info in CreateMembersInfo<T>(member, basePath))
                    {
                        paths.Add(info);
                    }
                }

                if (!member.TryGetMemberType(out Type memberType))
                {
                    continue;
                }

                GetMembersByAttribute<T>(memberType, paths, CreatePath(basePath, member.Name), depth, maxDepth);
            }
        }

        private static bool TryGetMemberType(this MemberInfo member, out Type type)
        {
            type = null;

            if (member.Name.Contains("get_") || member.Name.Contains("set_") || member.Name.Contains("op_"))
            {
                return false;
            }

            switch (member.MemberType)
            {
                case MemberTypes.Field: type = (member as FieldInfo).FieldType; break;
                case MemberTypes.Method: type = (member as MethodInfo).ReturnType; break;
                case MemberTypes.Property: type = (member as PropertyInfo).PropertyType; break;
            }

            return type != null;
        }

        private static IEnumerable<MemberData> CreateMembersInfo<T>(MemberInfo memberInfo, string basePath)
            where T : Attribute
        {
            var parameters = (memberInfo is MethodInfo method)
                ? method.GetParameters()
                    .Select(p => new MemberParameter(p.ParameterType, p.Name))
                    .ToArray()
                : null;

            var attrs = memberInfo.GetCustomAttributes<T>();
            var path = CreatePath(basePath, memberInfo.Name);

            memberInfo.TryGetMemberType(out Type returnType);

            return attrs.Select(attr =>
            {
                return new MemberData(path, parameters, returnType, attr);
            });
        }

        private static string CreatePath(string basePath, string path)
        {
            return $"{(string.IsNullOrEmpty(basePath) ? "" : $"{basePath}/")}{path}";
        }

        public static bool TryGetMember(this object obj, string fieldName, out MemberInfo member, out object targetObj)
        {
            (member, targetObj) = obj.GetMember(fieldName);

            return member != null;
        }

        public static (MemberInfo member, object targetObj) GetMember(this object obj, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return default;
            }

            var targetObject = obj;

            var fieldNamePaths = fieldName.Split('.');
            var pathLength = fieldNamePaths.Length;

            for (var i = 0; i < pathLength; i++)
            {
                var field = fieldNamePaths[i];

                if (i < pathLength - 1)
                {
                    if (field.Contains("Array")) continue;
                    if (field.Contains("data["))
                    {
                        var array = targetObject as System.Collections.IList;
                        var dataIndex = int.Parse(Regex.Match(field, @"\d{1,4}").Value);

                        targetObject = array[dataIndex];
                    }
                    else
                    {
                        targetObject = targetObject.GetValue(field);
                    }
                }
                else if (targetObject is System.Collections.IList list)
                {
                    var test = (Regex.Match(field, @"\d{1,4}"));
                    var tt = test.Value;
                    
                    var dataIndex = int.Parse(Regex.Match(field, @"\d{1,4}").Value);

                    return (new ListInfo(), new Tuple<System.Collections.IList, int>(list, dataIndex));
                }
                else
                {
                    var type = targetObject.GetType();

                    return (type.GetMember(field, instanceFlags).FirstOrDefault(), targetObject);
                }
            }

            return default;
        }

        public static ParameterInfo[] GetMethodParameters<TAttribute>(this object obj)
            where TAttribute : Attribute
        {
            var type = obj.GetType();
            var method = GetMethod<TAttribute>(type);

            return method?.GetParameters();
        }

        public static IEnumerable<MethodInfo> GetMethods<TAttribute>(this object obj)
            where TAttribute : Attribute
        {
            var type = obj.GetType();

            return GetMethods<TAttribute>(type);
        }

        #endregion

        #region INVOKE

        public static object InvokeMember<T>(
            this object targetObject,
            MemberData memberData,
            BindingFlags bindingFlags = instanceFlags)
            where T : Attribute
        {
            if (targetObject != null && memberData != null && !string.IsNullOrEmpty(memberData.Path))
            {
                var memberObj = targetObject;
                var memberPathSegments = memberData.Path.Split('/');

                for (var i = 0; i < memberPathSegments.Length; i++)
                {
                    var memberPathSegment = memberPathSegments[i];
                    var type = memberObj.GetType();

                    if (i < memberPathSegments.Length - 1)
                    {
                        var member = type.GetMember(memberPathSegment, bindingFlags).FirstOrDefault();

                        memberObj = GetValue(member, memberObj, memberData);
                    }
                    else
                    {
                        var members = type.GetMembers(bindingFlags);
                        var member = members.FirstOrDefault(x => x.Name == memberPathSegment && x.IsDefined(typeof(T)));

                        return GetValue(member, memberObj, memberData);
                    }
                }
            }

            return null;
        }

        public static object InvokeStaticGenericMethod(this Type type, string methodName, params object[] parameters)
        {
            var method = GetMethod(type, methodName, parameters, flattenFlags);

            return method.MakeGenericMethod(type).Invoke(null, parameters);
        }

        public static object InvokeStaticMethod(this Type type, string methodName, params object[] parameters)
        {
            var method = GetMethod(type, methodName, parameters, flattenFlags);

            return method.Invoke(null, parameters);
        }

        public static object InvokeMethod(this object obj, string methodName, params object[] parameters)
        {
            var type = obj.GetType();
            var method = GetMethod(type, methodName, parameters);

            return method.Invoke(obj, parameters);
        }

        public static object InvokeMethod<TAttribute>(this object obj, params object[] parameters)
            where TAttribute : Attribute
        {
            var type = obj.GetType();
            var method = GetMethod<TAttribute>(type, parameters);

            return method.Invoke(obj, parameters);
        }

        private static MethodInfo GetMethod(
            Type type,
            string methodName,
            object[] parameters,
            BindingFlags bindingFlags = instanceFlags)
        {
            var parametersCount = parameters.Length;
            var methods = type.GetMethods(bindingFlags);

            return methods.FirstOrDefault(x =>
            {
                return x.Name == methodName
                    && x.GetParameters().Length == parametersCount;
            });
        }

        private static MethodInfo GetMethod<TAttribute>(
            Type type,
            BindingFlags bindingFlags = instanceFlags)
            where TAttribute : Attribute
        {
            var methods = GetMethods<TAttribute>(type, bindingFlags);

            return methods.FirstOrDefault();
        }

        private static IEnumerable<MethodInfo> GetMethods<TAttribute>(
            Type type,
            BindingFlags bindingFlags = instanceFlags)
            where TAttribute : Attribute
        {
            var attrType = typeof(TAttribute);
            var methods = type.GetMethods(bindingFlags);

            return methods.Where(x =>
            {
                return x.IsDefined(attrType);
            });
        }

        private static MethodInfo GetMethod<TAttribute>(
            Type type,
            object[] parameters,
            BindingFlags bindingFlags = instanceFlags)
            where TAttribute : Attribute
        {
            var attrType = typeof(TAttribute);
            var parametersCount = parameters.Length;

            var methods = type.GetMethods(bindingFlags);

            return methods.FirstOrDefault(x =>
            {
                return x.IsDefined(attrType)
                    && x.GetParameters().Length == parametersCount;
            });
        }

        private static object InvokeMethod(MethodInfo method, object memberObj, MemberData memberData)
        {
            var parameters = GetParameters(method, memberData).ToArray();

            return method.Invoke(memberObj, parameters);
        }

        private static IEnumerable<object> GetParameters(MethodInfo method, MemberData memberData)
        {
            var parameters = method.GetParameters();

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var value = memberData.Parameters[i];

                yield return value.GetValue(parameter.ParameterType);
            }
        }

        #endregion

        #region GET | SET VALUE
        public static object GetValue(this object obj, string fieldName, object defaultValue = default)
        {
            var value = obj.GetValue(fieldName);

            if (value == null)
            {
                obj.SetValue(fieldName, defaultValue);

                return defaultValue;
            }
            else
            {
                return value;
            }
        }

        public static T GetValue<T>(this object obj, string fieldName, T defaultValue = default)
        {
            return (T)GetValue(obj, fieldName, (object)defaultValue);
        }

        public static object GetValue(this object obj, string fieldName)
        {
            var (memeberInfo, targetObject) = obj.GetMember(fieldName);

            return memeberInfo != null
                ? GetValue(memeberInfo, targetObject, memberData: null)
                : targetObject;
        }

        private static object GetValue(MemberInfo member, object memberObj, MemberData memberData)
        {
            return member switch
            {
                FieldInfo field => GetValue(field, memberObj),
                MethodInfo method => InvokeMethod(method, memberObj, memberData),
                PropertyInfo property => GetValue(property, memberObj),
                ListInfo => GetListValue(memberObj),
                _ => default,
            };
        }

        private static object GetValue(FieldInfo field, object memberObj)
        {
            return field.GetValue(memberObj);
        }

        private static object GetValue(PropertyInfo property, object memberObj)
        {
            return property.GetValue(memberObj);
        }

        public static void SetValue(this object obj, string fieldName, object value)
        {
            var (memeberInfo, targetObject) = obj.GetMember(fieldName);

            if (memeberInfo != null)
            {
                SetValue(memeberInfo, targetObject, value);
            }
        }

        private static void SetValue(MemberInfo member, object memberObj, object value)
        {
            switch (member)
            {
                case FieldInfo field: SetValue(field, memberObj, value); break;
                case PropertyInfo property: SetValue(property, memberObj, value); break;
                case ListInfo: SetValueToList(memberObj, value); break;
            }
        }

        private static object GetListValue(object memberObj)
        {
            return memberObj is Tuple<System.Collections.IList, int> data
                ? data.Item1[data.Item2]
                : default;
        }

        private static void SetValueToList(object memberObj, object value)
        {
            if (memberObj is Tuple<System.Collections.IList, int> data)
            {
                data.Item1[data.Item2] = value;
            }
        }

        private static void SetValue(FieldInfo field, object memberObj, object value)
        {
            field.SetValue(memberObj, value);
        }

        private static void SetValue(PropertyInfo property, object memberObj, object value)
        {
            property.SetValue(memberObj, value);
        }
        #endregion

        #region FIND TYPES
        private static List<Type> GetAllTypes()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .ToList();
        }

        /// <summary> Поиск всех типов во всех Assemblies являющихся наследником <typeparamref name="T"/> </summary>
        /// <returns> Список наследников типа <typeparamref name="T"/> </returns>
        public static IEnumerable<Type> FindChildTypesOf<T>()
        {
            var type = typeof(T);

            return AllTypesCache.Where(x =>
            {
                return !x.IsInterface
                    && !x.IsAbstract
                    && type.IsAssignableFrom(x);
            });
        }

        public static IEnumerable<Type> FindChildTypesOf(string typeName)
        {
            return AllTypesCache.Where(x =>
            {
                return !x.IsInterface
                    && !x.IsAbstract
                    && x.BaseType != null
                    && x.BaseType.Name.Contains(typeName);
            });
        }

        public static IEnumerable<Type> FindTypeByAttribute<T>() where T : Attribute
        {
            return AllTypesCache.Where(x => x.HasAttribute<T>());
        }
        #endregion

        #region CREATE TYPE
        public static T CreateByType<T>()
        {
            return typeof(T).CreateByType<T>();
        }

        public static T CreateByType<T>(params object[] args)
        {
            return typeof(T).CreateByType<T>(args);
        }

        public static object CreateByType(this Type type)
        {
            return type.Assembly.CreateInstance(type.FullName);
        }

        public static T CreateByType<T>(this Type type)
        {
            return (T)type.Assembly.CreateInstance(type.FullName);
        }

        public static T CreateByType<T>(this Type type, params object[] args)
        {
            return (T)type.Assembly.CreateInstance(
                type.FullName,
                ignoreCase: false,
                BindingFlags.Default,
                binder: null,
                args,
                culture: null,
                activationAttributes: null);
        }
        #endregion

        #region ATTRIBUTES
        public static bool HasAttribute<T>(this MemberInfo member)
            where T : Attribute
        {
            return member != null && member.IsDefined(typeof(T), inherit: true);
        }

        public static bool HasAttribute<T>(this object obj)
            where T : Attribute
        {
            return obj != null && obj.GetType().HasAttribute<T>();
        }

        public static bool HasAttribute<T>(this object obj, string fieldName)
            where T : Attribute
        {
            return obj != null
                && obj.TryGetMember(fieldName, out var member, out _)
                && member.HasAttribute<T>();
        }

        public static bool HasAttribute<T>(this Type type)
            where T : Attribute
        {
            return type.IsDefined(typeof(T), inherit: true);
        }

        public static T GetAttribute<T>(this MemberInfo member)
            where T : Attribute
        {
            return member?.GetCustomAttribute<T>(inherit: true);
        }

        public static T GetAttribute<T>(this object obj)
            where T : Attribute
        {
            return obj?.GetType().GetAttribute<T>();
        }

        public static T GetAttribute<T>(this object obj, string fieldName)
            where T : Attribute
        {
            return obj != null && obj.TryGetMember(fieldName, out var member, out _)
                ? member.GetAttribute<T>()
                : null;
        }

        public static T GetAttribute<T>(this Type type)
            where T : Attribute
        {
            return type.GetCustomAttribute<T>(inherit: true);
        }

        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member)
            where T : Attribute
        {
            return member?.GetCustomAttributes<T>(inherit: true);
        }

        public static IEnumerable<T> GetAttributes<T>(this object obj)
            where T : Attribute
        {
            return obj?.GetType().GetAttributes<T>();
        }

        public static IEnumerable<T> GetAttributes<T>(this Type type)
            where T : Attribute
        {
            return type.GetCustomAttributes<T>(inherit: true);
        }

        public static bool TryGetAttribute<T>(this object obj, out T attr)
            where T : Attribute
        {
            if (obj == null)
            {
                attr = null;
                return false;
            }

            return obj.GetType().TryGetAttribute(out attr);
        }

        public static bool TryGetAttribute<T>(this Type obj, out T attr)
            where T : Attribute
        {
            var checkAttr = obj.HasAttribute<T>();

            attr = checkAttr
                ? obj.GetAttribute<T>()
                : null;

            return checkAttr;
        }

        public static bool TryGetAttributes<T>(this object obj, out IEnumerable<T> attrs)
            where T : Attribute
        {
            if (obj == null)
            {
                attrs = null;
                return false;
            }

            return obj.GetType().TryGetAttributes(out attrs);
        }

        public static bool TryGetAttributes<T>(this Type obj, out IEnumerable<T> attrs)
            where T : Attribute
        {
            var checkAttr = obj.HasAttribute<T>();

            attrs = checkAttr
                ? obj.GetAttributes<T>()
                : null;

            return checkAttr;
        }
        #endregion

#if UNITY_EDITOR
        #region SERIALIZED PROPERTY OBJECT VALUE
        public static T GetSerializedValue<T>(this Object obj, string fieldName)
        {
            return new SerializedObject(obj).GetSerializedValue<T>(fieldName);
        }

        public static void SetSerializedValue(this Object obj, string fieldName, object value)
        {
            new SerializedObject(obj).SetSerializedValue(fieldName, value);
        }

        public static T GetSerializedValue<T>(this SerializedObject obj, string fieldName)
        {
            return (T)obj.FindProperty(fieldName).GetValue();
        }

        public static void SetSerializedValue(this SerializedObject obj, string fieldName, object value)
        {
            obj.FindProperty(fieldName).SetValue(value);
        }

        public static object GetValue(this SerializedProperty property)
        {
            if (property == null) return null;

            var obj = property.serializedObject.targetObject as object;
            var paths = property.propertyPath.Split('.');

            for (var i = 0; i < paths.Length; i++)
            {
                var path = paths[i];

                if (path.Contains("Array")) continue;
                if (path.Contains("data["))
                {
                    var array = obj as System.Collections.IList;
                    var dataIndex = int.Parse(Regex.Match(path, @"\d{1,4}").Value);

                    obj = array[dataIndex];
                }
                else
                {
                    var type = obj.GetType();
                    var field = type.GetField(path, instanceFlags | BindingFlags.Default);

                    obj = field.GetValue(obj);
                }
            }

            return obj;
        }

        public static void SetValue(this SerializedProperty property, object val)
        {
            var list = new List<KeyValuePair<FieldInfo, object>>();
            var obj = property.serializedObject.targetObject as object;

            foreach (var path in property.propertyPath.Split('.'))
            {
                var type = obj.GetType();
                var field = type.GetField(path, instanceFlags | BindingFlags.Default);

                list.Add(new KeyValuePair<FieldInfo, object>(field, obj));

                obj = field.GetValue(obj);
            }

            for (var i = list.Count - 1; i >= 0; --i)
            {
                list[i].Key.SetValue(list[i].Value, val);

                val = list[i].Value;
            }
        }
        #endregion
#endif 
    }
}

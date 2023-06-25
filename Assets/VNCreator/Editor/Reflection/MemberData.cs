using System;
using System.Linq;
using UnityEngine;

namespace VNCreator
{
    [Serializable]
    public class MemberData : ICloneable
    {
        [SerializeField] protected string path;
        [SerializeField] protected string returnTypeName;
        [Space] [SerializeField] protected MemberParameter[] parameters;

        private Type returnType;
        private readonly Attribute attr;

        public string Path => path;
        public MemberParameter[] Parameters => parameters;

        public Type ReturnType
        {
            get
            {
                if (returnType == null && !string.IsNullOrEmpty(returnTypeName))
                {
                    returnType = Type.GetType(returnTypeName, false);
                }

                return returnType;
            }
        }

        public MemberData(string path, MemberParameter[] parameters, Type returnType, Attribute attr)
        {
            this.path = path;
            this.parameters = parameters ?? new MemberParameter[0];
            this.returnType = returnType;
            this.attr = attr;

            returnTypeName = returnType.AssemblyQualifiedName;
        }

        public MemberData(MemberData data) : this(data.path, data.parameters, data.ReturnType, data.attr)
        {
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return attr is T;
        }

        public T GetAttribute<T>() where T : Attribute
        {
            return attr as T;
        }

        public object Clone()
        {
            var data = MemberwiseClone() as MemberData;

            data.parameters = parameters
                .Select(x => x.Clone() as MemberParameter)
                .ToArray();

            return data;
        }
    }
}
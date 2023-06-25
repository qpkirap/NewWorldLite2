using System;
using UnityEngine;

namespace VNCreator
{
    [Serializable]
    public class MemberParameter : ICloneable
    {
        [SerializeField] private string value;

        public string Name { get; }
        public Type Type { get; private set; }

        public MemberParameter(Type type, string parameterName)
        {
            Type = type;
            Name = parameterName;
        }

        public object GetValue(Type type = null)
        {
            Type ??= type;

            if (Type.Equals(typeof(string)))
            {
                return value;
            }

            if (Type.Equals(typeof(int)))
            {
                int.TryParse(value, out var intValue);

                return intValue;
            }

            if (Type.Equals(typeof(float)))
            {
                var floatValue = value.FloatParse();

                return floatValue;
            }

            if (Type.Equals(typeof(bool)))
            {
                bool.TryParse(value, out var boolValue);

                return boolValue;
            }

            if (Type.BaseType.Equals(typeof(Enum)))
            {
                Enum.TryParse(Type, value, out var enumValue);

                return enumValue ?? Enum.GetValues(Type).GetValue(0);
            }

            return null;
        }

        public object Clone()
        {
            return new MemberParameter(Type, Name);
        }
    }
}
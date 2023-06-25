using System;
using System.Reflection;

namespace VNCreator
{
    public class ListInfo : MemberInfo
    {
        public override MemberTypes MemberType => (MemberTypes)(-1);

        public override Type DeclaringType => throw new NotImplementedException();
        public override Type ReflectedType => throw new NotImplementedException();

        public override string Name => throw new NotImplementedException();

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
    }
}
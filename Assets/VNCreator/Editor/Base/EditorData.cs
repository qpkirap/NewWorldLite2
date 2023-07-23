using System;

namespace VNCreator
{
    public class EditorData
    {
        public readonly Type dataType;
        
        public EditorData(EditorData data) : this(
            data.dataType)
        {
        }

        public EditorData(Type dataType)
        {
            this.dataType = dataType;
        }
    }
}
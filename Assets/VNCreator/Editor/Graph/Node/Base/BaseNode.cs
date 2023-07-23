#if UNITY_EDITOR

using Node = UnityEditor.Experimental.GraphView.Node;

namespace VNCreator
{
    public abstract class BaseNode : Node
    {
        public abstract NodeType NodeType { get; }
    }
}

#endif
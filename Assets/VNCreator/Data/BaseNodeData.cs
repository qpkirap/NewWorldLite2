namespace VNCreator
{
    public abstract class BaseNodeData
    {
        public abstract NodeType NodeType { get; }
    }

    public enum NodeType
    {
        Dialogue,
        Action
    }
}
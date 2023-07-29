namespace VNCreator
{
    public abstract class BaseNodeData : ScriptableEntity
    {
        public abstract NodeType NodeType { get; }
    }

    public enum NodeType
    {
        Dialogue,
        Action
    }
}
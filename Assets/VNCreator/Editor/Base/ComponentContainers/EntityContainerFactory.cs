namespace VNCreator
{
    public class EntityContainerFactory<TComponent> : BaseContainerFactory<TComponent, ComponentContainerAttribute>
        where TComponent : Component
    {
    }
}
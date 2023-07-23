namespace VNCreator
{
    public class Component : ScriptableEntity
    {
        public override object Clone()
        {
            var newComponent = base.Clone() as Component;

            newComponent.RemoveCloneSuffix();

            return newComponent;
        }
    }
}
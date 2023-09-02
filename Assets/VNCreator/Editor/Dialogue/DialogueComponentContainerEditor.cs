namespace VNCreator
{
    [ComponentContainer(typeof(DialogueNodeData), typeof(DialogueComponentContainerEditor))]
    public class DialogueComponentContainerEditor : ComponentEntityEditor<DialogueNodeData>
    {
        public override void InitContainer(string fieldName, object container)
        {
            base.InitContainer(fieldName, container);
        }
    }
}
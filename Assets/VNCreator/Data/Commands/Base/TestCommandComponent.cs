namespace VNCreator
{
    public class TestCommandActionComponent : CommandAction<TestCommand>
    {
        public override TestCommand GetCommand()
        {
            return new(Id);
        }
    }
}
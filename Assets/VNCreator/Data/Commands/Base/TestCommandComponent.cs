namespace VNCreator
{
    public class TestCommandComponent : Command<TestCommand>
    {
        public override TestCommand GetCommand()
        {
            return new(Id);
        }
    }
}
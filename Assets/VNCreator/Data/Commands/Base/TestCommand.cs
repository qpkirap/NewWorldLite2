namespace VNCreator
{
    public class TestCommand : ICommand<TestCommand>
    {
        public TestCommand(string Id)
        {
            this.Id = Id;
        }
        
        public string Id { get; }
        public void Execute()
        {
            
        }

        public void Undo()
        {
            
        }
    }
}
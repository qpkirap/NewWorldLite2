namespace VNCreator
{
    public static class EditorCache
    {
        public static CommandComponentsEditorsFactory CommandComponentsEditorsFactory;

        public static void Init()
        {
            CommandComponentsEditorsFactory = new();
        }
    }
}
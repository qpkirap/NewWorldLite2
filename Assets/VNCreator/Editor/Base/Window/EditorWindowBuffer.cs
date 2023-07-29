namespace VNCreator
{
    public static class EditorWindowBuffer
    {
        private static EditorWindow lastEditor;

        public static EditorWindow LastEditor
        {
            get => lastEditor;
            set
            {
                if (lastEditor != null && lastEditor != value)
                {
                    lastEditor.CloseEditor();
                }

                lastEditor = value;
            }
        }

        public static void Register(EditorWindow window)
        {
            LastEditor = window;
        }

        public static void Reset()
        {
            LastEditor = null;
        }
    }
}
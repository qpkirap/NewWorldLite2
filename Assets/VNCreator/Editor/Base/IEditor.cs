namespace VNCreator
{
    public interface IEditor
    {
        void DrawEditor();

        void CloseEditor();
        void CheckDirty();
    }
}
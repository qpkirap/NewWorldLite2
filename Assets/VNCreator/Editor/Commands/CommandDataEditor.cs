using UnityEngine;

namespace VNCreator.Commands
{
    [StoryObjectEditor(typeof(CommandData), typeof(CommandDataEditor))]
    public class CommandDataEditor : BaseEntityEditor<CommandData>
    {
        protected override string GetDataPath(ScriptableObject entity)
        {
            var componentName = entity.GetType().Name.Replace("Data", "");

            return EditorUtils.CreateAssetPath<CommandData, StoryObject>(
                data: entity as CommandData,
                namePrefix: componentName,
                subFolders: $"{nameof(CommandData)}/{componentName}s");
        }
    }
}
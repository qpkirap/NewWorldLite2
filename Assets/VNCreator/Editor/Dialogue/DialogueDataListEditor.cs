using UnityEngine;

namespace VNCreator
{
    [StoryObjectEditor(typeof(DialogueNodeData), typeof(DialogueDataListEditor))]
    public class DialogueDataListEditor : ListComponentEntitiesEditor<DialogueNodeData>
    {
        protected override string GetDataPath(ScriptableObject entity)
        {
            var componentName = entity.GetType().Name.Replace("Data", "");

            return EditorUtils.CreateAssetPath<DialogueNodeData, StoryObject>(
                data: entity as DialogueNodeData,
                namePrefix: componentName,
                subFolders: $"{nameof(DialogueNodeData)}/{componentName}s");
        }
    }
}
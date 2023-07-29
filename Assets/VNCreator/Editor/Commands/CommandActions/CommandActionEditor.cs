using UnityEngine;

namespace VNCreator
{
    public class CommandActionEditor : BaseEntityEditor
    {
        public override object CloneEntity(object container)
        {
            var newEntity = base.CloneEntity(container) as CommandAction;

            if (IsSubEntity)
            {
                newEntity.RemoveCloneSuffix();
            }

            return newEntity;
        }

        protected override string GetDataPath(ScriptableObject entity)
        {
            var componentName = entity.GetType().Name.Replace("Data", "");

            return EditorUtils.CreateAssetPath<CommandAction, StoryObject>(
                data: entity as CommandAction,
                namePrefix: componentName,
                subFolders: $"{nameof(CommandAction)}/{componentName}s");
        }
    }
}
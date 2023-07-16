using System.Collections.Generic;
using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace VNCreator
{
    [CustomEditor(typeof(Command))]
    public class CommandEditor : ToolboxEditor
    {
        private List<Command> source;

        public void Init(List<Command> source)
        {
            this.source = source;
        }
        
        public Command Instantiate()
        {
            var type = target.GetType();

            var instance = ScriptableObject.CreateInstance(type);

            return instance as Command;
        }
    }
}
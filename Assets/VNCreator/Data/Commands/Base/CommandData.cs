using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VNCreator
{
    [Serializable]
    public class CommandData : BaseNodeData
    {
        private string guid;
        
        [SerializeField] private List<CommandAction> commandActions;
        
        public IReadOnlyList<CommandAction> CommandActions => commandActions;

#if UNITY_EDITOR
        public string GuidEditor => string.IsNullOrEmpty(guid) ? guid = GUID.Generate().ToString() : guid;
#endif

        public override NodeType NodeType => NodeType.Action;
    }
}
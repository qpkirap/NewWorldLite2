using System;
using System.Collections.Generic;
using UnityEngine;

namespace VNCreator
{
    [Serializable]
    public class CommandData : Component
    {
        [SerializeField] private List<CommandAction> commandActions;
        [SerializeField] private Rect nodePosition;
        [SerializeField] private bool isEndNode;
        [SerializeField] private bool isStartNode;
        
        public IReadOnlyList<CommandAction> CommandActions => commandActions;
        public Rect NodePosition => nodePosition;
        public bool IsEndNode => isEndNode;
        public bool IsStartNode => isStartNode;
    }
}
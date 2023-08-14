using System;
using System.Collections.Generic;
using UnityEngine;

namespace VNCreator
{
    [Serializable]
    public class CommandData : Component
    {
        [SerializeField] private List<CommandAction> commandActions;
        
        public IReadOnlyList<CommandAction> CommandActions => commandActions;
    }
}
using System;
using UnityEngine;

namespace VNCreator
{
    public abstract class Command<T> : Command
        where T : ICommand<T>
    {
        private T commandCache;

        public abstract T GetCommand();

        public override Type GetMyType()
        {
            return GetCommand()?.GetType();
        }

        public override void Execute()
        {
            GetCommand()?.Execute();
        }

        public override void Undo()
        {
            GetCommand()?.Undo();
        }
        
    }
    
    public abstract class Command : ScriptableObject
    {
        public abstract Type GetMyType();
        public abstract void Execute();
        public abstract void Undo();
    }
    
    public interface ICommand<out T>
    {
        string Id { get; }
        void Execute();
        void Undo();
    }
}
using System;
using App.Services;
using System.Collections.Generic;

namespace App.Events
{
    public static class EventBus<T>
    {
        static readonly HashSet<IEventBinding<T>> bindings = new();
        
        public static void Register(IEventBinding<T> binding) => bindings.Add(binding);
        public static void Deregister(IEventBinding<T> binding) => bindings.Remove(binding);

        public static void Raise(T @event)
        {
            foreach (var binding in bindings)
            {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }

        public static void Clear()
        {
            bindings.Clear();
        }
    }
}
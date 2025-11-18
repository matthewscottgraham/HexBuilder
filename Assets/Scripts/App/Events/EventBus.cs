using System.Collections.Generic;

namespace App.Events
{
    public static class EventBus<T>
    {
        private static readonly HashSet<IEventBinding<T>> Bindings = new();

        public static void Register(IEventBinding<T> binding)
        {
            Bindings.Add(binding);
        }

        public static void Deregister(IEventBinding<T> binding)
        {
            Bindings.Remove(binding);
        }

        public static void Raise(T @event)
        {
            foreach (var binding in Bindings)
            {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }

        public static void Clear()
        {
            Bindings.Clear();
        }
    }
}
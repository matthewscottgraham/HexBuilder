using System;
using App.Services;
using System.Collections.Generic;

namespace App.Events
{
    public class EventBus<T> : IDisposable
    {
        private readonly HashSet<IEventBinding<T>> _bindings = new();

        public void Dispose()
        {
            _bindings.Clear();
            ServiceLocator.Instance.Deregister(this);
        }
        
        public void Register(IEventBinding<T> binding) => _bindings.Add(binding);
        public void Deregister(IEventBinding<T> binding) => _bindings.Remove(binding);

        public void Raise(T @event)
        {
            foreach (var binding in _bindings)
            {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }
    }
}
using System;

namespace App.Events
{
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> _onEvent = _ => { };
        private Action _onEventNoArgs = () => { };

        public EventBinding(Action<T> onEvent)
        {
            _onEvent = onEvent;
        }

        public EventBinding(Action onEventNoArgs)
        {
            _onEventNoArgs = onEventNoArgs;
        }

        Action<T> IEventBinding<T>.OnEvent
        {
            get => _onEvent;
            set => _onEvent = value;
        }

        Action IEventBinding<T>.OnEventNoArgs
        {
            get => _onEventNoArgs;
            set => _onEventNoArgs = value;
        }

        public void Add(Action<T> onEvent)
        {
            _onEvent += onEvent;
        }

        public void Remove(Action<T> onEvent)
        {
            _onEvent -= onEvent;
        }

        public void Add(Action onEventNoArgs)
        {
            _onEventNoArgs += onEventNoArgs;
        }

        public void Remove(Action onEventNoArgs)
        {
            _onEventNoArgs = onEventNoArgs;
        }
    }
}
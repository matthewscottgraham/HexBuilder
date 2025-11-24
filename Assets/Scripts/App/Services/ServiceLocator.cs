using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Services
{
    public class ServiceLocator : IDisposable
    {
        private readonly Dictionary<Type, object> _services = new();

        public ServiceLocator()
        {
            if (Instance != null)
            {
                Dispose();
                return;
            }

            Instance = this;
        }

        public static ServiceLocator Instance { get; private set; }

        public void Dispose()
        {
            if (Instance == this) Instance = null;
            var serviceTypes = _services.Keys.ToArray();
            foreach (var service in serviceTypes)
                for (var i = 0; i < _services.Keys.Count; i++)
                    Deregister(service);
            _services.Clear();
        }

        public void Register<T>(T service)
        {
            if (!_services.TryAdd(service.GetType(), service))
                Debug.LogError($"Service type already registered: {service.GetType().Name}");
        }

        public void Deregister<T>(T service)
        {
            if (service == null) return;
            var type = service.GetType();
            Deregister(type);
        }

        public void Deregister(Type type)
        {
            if (!_services.ContainsKey(type)) return;
            _services.Remove(type);
        }

        public T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service)) return service as T;

            throw new Exception($"Service not registered: {typeof(T).Name}");
        }

        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = obj as T;
                return true;
            }

            service = null;
            return false;
        }
    }
}
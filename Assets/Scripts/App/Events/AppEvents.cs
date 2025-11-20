using UnityEngine;

namespace App.Events
{
    public struct SceneLoadedEvent : IEvent
    {
        public SceneLoadedEvent(string sceneName)
        {
            SceneName = sceneName;
        }

        public string SceneName;
    }

    public struct SceneUnloadedEvent : IEvent
    {
        public SceneUnloadedEvent(string sceneName)
        {
            SceneName = sceneName;
        }

        public string SceneName;
    }

    public struct AppExitEvent : IEvent
    {
        
    }

    public struct GameExitEvent : IEvent
    {
        
    }

    public struct FileSaveEvent : IEvent
    {
    }

    public struct FileLoadEvent : IEvent
    {
    }

    public struct InteractEvent : IEvent
    {
    }

    public struct MoveEvent : IEvent
    {
        public MoveEvent(Vector2 delta)
        {
            Delta = delta;
        }

        public Vector2 Delta;
    }
}
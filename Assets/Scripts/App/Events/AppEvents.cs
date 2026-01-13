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

    public struct AppExitEvent : IEvent { }

    public struct GameExitEvent : IEvent { }

    public struct GameReloadEvent : IEvent { }

    public struct GamePauseEvent : IEvent { }

    public struct GameResumeEvent : IEvent { }

    public struct FileSaveEvent : IEvent { }
    public struct FileLoadEvent : IEvent { }
    
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

    public struct ZoomEvent : IEvent
    {
        public ZoomEvent(float delta)
        {
            Delta = delta;
        }
        
        public readonly float Delta;
    }
    
    public struct RotateEvent : IEvent
    {
        public RotateEvent(float delta)
        {
            Delta = delta;
        }
        
        public readonly float Delta;
    }

    public struct DragEvent : IEvent
    {
        public DragEvent(Vector2 delta)
        {
            Delta = delta;
        }

        public Vector2 Delta;
    }

    public struct SetMusicVolume : IEvent
    {
        public SetMusicVolume(float volume)
        {
            Volume = volume;
        }

        public readonly float Volume;
    }
    public struct SetSfxVolume : IEvent
    {
        public SetSfxVolume(float volume)
        {
            Volume = volume;
        }

        public readonly float Volume;
    }
    
    public struct PlaySoundEvent : IEvent
    {
        public PlaySoundEvent(string soundID, bool randomPitch = false)
        {
            SoundID = soundID;
            RandomPitch = randomPitch;
        }

        public readonly string SoundID;
        public readonly bool RandomPitch;
    }
    
    public struct PlayMusicEvent : IEvent { }
    public struct StopMusicEvent : IEvent { }

    public struct PlayVFXBurstEvent : IEvent
    {
        public PlayVFXBurstEvent(string effectID, Vector3 position, Vector3 rotation)
        {
            EffectID = effectID;
            Position = position;
            Rotation = rotation;
        }
        
        public readonly string EffectID;
        public readonly Vector3 Position;
        public readonly Vector3 Rotation;
    }
}
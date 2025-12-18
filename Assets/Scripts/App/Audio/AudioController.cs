using System;
using System.Collections;
using System.Collections.Generic;
using App.Events;
using App.Services;
using App.Utils;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace App.Audio
{
    public class AudioController : MonoBehaviour, IDisposable
    {
        private const int MaxPoolSize = 25;
        
        private EventBinding<PlayMusicEvent> _playMusicEvent;
        private EventBinding<StopMusicEvent> _stopMusicEvent;
        private EventBinding<PlaySoundEvent> _playSoundEvent;
        private EventBinding<GamePauseEvent> _gamePauseEvent;
        private EventBinding<GameResumeEvent> _gameResumeEvent;

        private AudioSource _musicSource;
        private readonly Dictionary<string, AudioClip> _soundClips = new();
        private readonly List<AudioSource> _activeAudioSources = new();
        private ObjectPool<AudioSource> _soundSourcesPool;

        public void Initialize()
        {
            _soundSourcesPool = new ObjectPool<AudioSource>(CreateSoundSource, OnGetSoundSource, OnReleaseSoundSource);
            _musicSource = gameObject.AddChild<AudioSource>("Music");
            _musicSource.loop = true;
            _musicSource.clip = Resources.Load<AudioClip>("Audio/Music/musicA");
            _musicSource.volume = 0.3f;

            _playMusicEvent = new EventBinding<PlayMusicEvent>(HandlePlayMusic);
            EventBus<PlayMusicEvent>.Register(_playMusicEvent);
            
            _stopMusicEvent = new EventBinding<StopMusicEvent>(HandleStopMusic);
            EventBus<StopMusicEvent>.Register(_stopMusicEvent);
            
            _playSoundEvent = new EventBinding<PlaySoundEvent>(HandlePlaySound);
            EventBus<PlaySoundEvent>.Register(_playSoundEvent);
            
            _gamePauseEvent = new EventBinding<GamePauseEvent>(HandleGamePause);
            EventBus<GamePauseEvent>.Register(_gamePauseEvent);
            
            _gameResumeEvent = new EventBinding<GameResumeEvent>(HandleGameResume);
            EventBus<GameResumeEvent>.Register(_gameResumeEvent);
            
            HandlePlayMusic();
        }

        public void Dispose()
        {
            _soundClips.Clear();
            _soundSourcesPool.Clear();
            EventBus<PlayMusicEvent>.Deregister(_playMusicEvent);
            EventBus<StopMusicEvent>.Deregister(_stopMusicEvent);
            EventBus<PlaySoundEvent>.Deregister(_playSoundEvent);
            EventBus<GamePauseEvent>.Deregister(_gamePauseEvent);
            EventBus<GameResumeEvent>.Deregister(_gameResumeEvent);
            _playMusicEvent = null;
            _stopMusicEvent = null;
            _playSoundEvent = null;
            _gamePauseEvent = null;
            _gameResumeEvent = null;
            ServiceLocator.Instance.Deregister(this);
        }

        public void RegisterSound(string soundID, AudioClip clip)
        {
            if (_soundClips.ContainsKey(soundID)) return;
            _soundClips.Add(soundID, clip);
        }

        private void HandlePlayMusic()
        {
            _musicSource.Play();
        }

        private void HandleStopMusic()
        {
            _musicSource.Stop();
        }

        private void HandlePlaySound(PlaySoundEvent playSoundEvent)
        {
            if (_soundSourcesPool.CountInactive == 0 && _soundSourcesPool.CountAll >= MaxPoolSize) return;
            var source = _soundSourcesPool.Get();
            source.clip = _soundClips[playSoundEvent.SoundID];
            source.pitch = playSoundEvent.RandomPitch ? Random.Range(0.9f, 1.1f) : 1;
            source.Play();
            StartCoroutine(ReleaseSoundSourceWhenFinished(source));
        }

        private IEnumerator ReleaseSoundSourceWhenFinished(AudioSource source)
        {
            yield return new WaitWhile(() => source.isPlaying);
            _soundSourcesPool.Release(source);
        }

        private void HandleGamePause()
        {
            _musicSource.Pause();
            foreach (var soundSource in _activeAudioSources)
            {
                    soundSource.Pause();
            }
        }

        private void HandleGameResume()
        {
            _musicSource.Play();
            foreach (var soundSource in _activeAudioSources)
            {
                    soundSource.Play();
            }
        }

        private AudioSource CreateSoundSource()
        {
            var soundSource = gameObject.AddChild<AudioSource>("SoundSource");
            return soundSource;
        }

        private void OnGetSoundSource(AudioSource source)
        {
            _activeAudioSources.Add(source);
        }

        private void OnReleaseSoundSource(AudioSource source)
        {
            source.clip = null;
            source.pitch = 1;
            if (_activeAudioSources.Contains(source))
                _activeAudioSources.Remove(source);
        }
    }
}

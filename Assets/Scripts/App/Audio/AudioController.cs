using System;
using System.Collections;
using System.Collections.Generic;
using App.Config;
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

        private EventBinding<SetMusicVolume> _musicVolumeBinding;
        private EventBinding<SetSfxVolume> _sfxVolumeBinding;
        private EventBinding<PlayMusicEvent> _playMusicBinding;
        private EventBinding<StopMusicEvent> _stopMusicBinding;
        private EventBinding<PlaySoundEvent> _playSoundBinding;
        private EventBinding<GamePauseEvent> _gamePauseBinding;
        private EventBinding<GameResumeEvent> _gameResumeBinding;

        private AudioSource _musicSource;
        private readonly Dictionary<string, AudioClip> _soundClips = new();
        private readonly List<AudioSource> _activeAudioSources = new();
        private ObjectPool<AudioSource> _soundSourcesPool;
        
        private ConfigController _configController;
        public float MusicVolume { get; private set; }= 0.3f;
        public float SfxVolume { get; private set; } = 0.6f;

        public void Initialize()
        {
            _configController = ServiceLocator.Instance.Get<ConfigController>();
            MusicVolume = _configController.Config.MusicVolume;
            SfxVolume = _configController.Config.SfxVolume;
            
            _soundSourcesPool = new ObjectPool<AudioSource>(CreateSoundSource, OnGetSoundSource, OnReleaseSoundSource);
            _musicSource = gameObject.AddChild<AudioSource>("Music");
            _musicSource.loop = true;
            _musicSource.clip = Resources.Load<AudioClip>("Audio/Music/musicA");
            _musicSource.volume = MusicVolume;

            _musicVolumeBinding = new EventBinding<SetMusicVolume>(HandleSetMusicVolume);
            EventBus<SetMusicVolume>.Register(_musicVolumeBinding);

            _sfxVolumeBinding = new EventBinding<SetSfxVolume>(HandleSetSfxVolume);
            EventBus<SetSfxVolume>.Register(_sfxVolumeBinding);
            
            _playMusicBinding = new EventBinding<PlayMusicEvent>(HandlePlayMusic);
            EventBus<PlayMusicEvent>.Register(_playMusicBinding);
            
            _stopMusicBinding = new EventBinding<StopMusicEvent>(HandleStopMusic);
            EventBus<StopMusicEvent>.Register(_stopMusicBinding);
            
            _playSoundBinding = new EventBinding<PlaySoundEvent>(HandlePlaySound);
            EventBus<PlaySoundEvent>.Register(_playSoundBinding);
            
            _gamePauseBinding = new EventBinding<GamePauseEvent>(HandleGamePause);
            EventBus<GamePauseEvent>.Register(_gamePauseBinding);
            
            _gameResumeBinding = new EventBinding<GameResumeEvent>(HandleGameResume);
            EventBus<GameResumeEvent>.Register(_gameResumeBinding);
            
            HandlePlayMusic();
        }

        public void Dispose()
        {
            _soundClips.Clear();
            _soundSourcesPool.Clear();
            
            EventBus<SetMusicVolume>.Deregister(_musicVolumeBinding);
            EventBus<SetSfxVolume>.Deregister(_sfxVolumeBinding);
            EventBus<PlayMusicEvent>.Deregister(_playMusicBinding);
            EventBus<StopMusicEvent>.Deregister(_stopMusicBinding);
            EventBus<PlaySoundEvent>.Deregister(_playSoundBinding);
            EventBus<GamePauseEvent>.Deregister(_gamePauseBinding);
            EventBus<GameResumeEvent>.Deregister(_gameResumeBinding);
            
            _musicVolumeBinding = null;
            _sfxVolumeBinding = null;
            _playMusicBinding = null;
            _stopMusicBinding = null;
            _playSoundBinding = null;
            _gamePauseBinding = null;
            _gameResumeBinding = null;
            ServiceLocator.Instance.Deregister(this);
        }

        public void RegisterSound(string soundID, AudioClip clip)
        {
            _soundClips.TryAdd(soundID, clip);
        }

        private void HandleSetMusicVolume(SetMusicVolume evt)
        {
            MusicVolume = Mathf.Clamp01(evt.Volume);
            _musicSource.volume = MusicVolume;
            
            var config = _configController.Config;
            config.MusicVolume = MusicVolume;
            _configController.SetConfig(config);
        }

        private void HandleSetSfxVolume(SetSfxVolume evt)
        {
            SfxVolume = Mathf.Clamp01(evt.Volume);
            foreach (var audioSource in _activeAudioSources)
            {
                audioSource.volume = SfxVolume;
            }
            
            var config = _configController.Config;
            config.SfxVolume = SfxVolume;
            _configController.SetConfig(config);
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
            source.volume = SfxVolume;
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

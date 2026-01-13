using System;
using App.Audio;
using App.Events;
using App.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Options
{
    public class OptionsView : MonoBehaviour
    {
        private UIDocument _document;
        private Slider _musicVolumeSlider;
        private Slider _sfxVolumeSlider;

        private void Start()
        {
            var audioController = ServiceLocator.Instance.Get<AudioController>();
            _document = GetComponent<UIDocument>();
            
            _musicVolumeSlider = _document.rootVisualElement.Q<Slider>("MusicVolume");
            _musicVolumeSlider.SetValueWithoutNotify(audioController.MusicVolume);
            _musicVolumeSlider.RegisterValueChangedCallback(HandleMusicVolumeChanged);
            
            _sfxVolumeSlider = _document.rootVisualElement.Q<Slider>("SfxVolume");
            _sfxVolumeSlider.SetValueWithoutNotify(audioController.SfxVolume);
            _sfxVolumeSlider.RegisterValueChangedCallback(HandleSfxVolumeChanged);
        }

        private void OnDestroy()
        {
            _musicVolumeSlider.UnregisterValueChangedCallback(HandleMusicVolumeChanged);
            _sfxVolumeSlider.UnregisterValueChangedCallback(HandleSfxVolumeChanged);
        }

        private static void HandleMusicVolumeChanged(ChangeEvent<float> value)
        {
            EventBus<SetMusicVolume>.Raise(new SetMusicVolume(value.newValue));
        }

        private static void HandleSfxVolumeChanged(ChangeEvent<float> value)
        {
            EventBus<SetSfxVolume>.Raise(new SetSfxVolume(value.newValue));
        }
    }
}

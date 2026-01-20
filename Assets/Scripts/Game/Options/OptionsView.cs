using System;
using App.Audio;
using App.Events;
using App.Screenshots;
using App.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Options
{
    public class OptionsView : MonoBehaviour
    {
        private UIDocument _document;
        private Button _screenshotButton;
        private Slider _musicVolumeSlider;
        private Slider _sfxVolumeSlider;

        private void Start()
        {
            var audioController = ServiceLocator.Instance.Get<AudioController>();
            _document = GetComponent<UIDocument>();
            
            _screenshotButton = _document.rootVisualElement.Q<Button>("ScreenshotButton");
            _screenshotButton.clicked += TakeScreenshot;
            
            _musicVolumeSlider = _document.rootVisualElement.Q<Slider>("MusicVolume");
            _musicVolumeSlider.SetValueWithoutNotify(audioController.MusicVolume);
            _musicVolumeSlider.RegisterValueChangedCallback(HandleMusicVolumeChanged);
            
            _sfxVolumeSlider = _document.rootVisualElement.Q<Slider>("SfxVolume");
            _sfxVolumeSlider.SetValueWithoutNotify(audioController.SfxVolume);
            _sfxVolumeSlider.RegisterValueChangedCallback(HandleSfxVolumeChanged);
            
            #if UNITY_WEBGL
            _screenshotButton.style.display = DisplayStyle.None;
            #endif
        }

        private void OnDestroy()
        {
            if (_screenshotButton == null) return;
            _screenshotButton.clicked -= TakeScreenshot;
            _musicVolumeSlider.UnregisterValueChangedCallback(HandleMusicVolumeChanged);
            _sfxVolumeSlider.UnregisterValueChangedCallback(HandleSfxVolumeChanged);
        }

        private void TakeScreenshot()
        {
            ServiceLocator.Instance.Get<ScreenshotController>().TakeScreenshot();
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

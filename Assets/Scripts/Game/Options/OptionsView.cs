using App.Audio;
using App.Events;
using App.Screenshots;
using App.Services;
using App.Utils;
using Game.Menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Options
{
    public class OptionsView : MonoBehaviour
    {
        private Slider _musicVolumeSlider;
        private Slider _sfxVolumeSlider;

        private void Start()
        {
            var audioController = ServiceLocator.Instance.Get<AudioController>();
            var menuBarController = GetComponent<MenuBarController>();

#if !UNITY_WEBGL
            var screenshotIcon = Resources.Load<Sprite>("Sprites/screenshot"); 
            menuBarController.RegisterButton("screenshot", screenshotIcon, TakeScreenshot);
#endif
            
            var optionsIcon = Resources.Load<Sprite>("Sprites/options"); 
            var tabContent = menuBarController.RegisterTab("options", optionsIcon);
            
            _musicVolumeSlider = MakeSlider(tabContent, "Music", audioController.MusicVolume);
            _musicVolumeSlider.RegisterValueChangedCallback(HandleMusicVolumeChanged);
            
            _sfxVolumeSlider = MakeSlider(tabContent, "SFX", audioController.SfxVolume);
            _sfxVolumeSlider.RegisterValueChangedCallback(HandleSfxVolumeChanged);
        }

        private void OnDestroy()
        {
            if (_musicVolumeSlider == null) return;
            _musicVolumeSlider.UnregisterValueChangedCallback(HandleMusicVolumeChanged);
            _sfxVolumeSlider.UnregisterValueChangedCallback(HandleSfxVolumeChanged);
        }

        private static Slider MakeSlider(VisualElement parentElement, string label, float value)
        {
            var slider = parentElement.AddNew(new Slider());
            slider.label = label;
            slider.SetValueWithoutNotify(value);
            slider.lowValue = 0;
            slider.highValue = 1;
            return slider;
        }

        private static void TakeScreenshot()
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

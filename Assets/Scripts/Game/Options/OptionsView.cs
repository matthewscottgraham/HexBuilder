using App.Audio;
using App.Events;
using App.Screenshots;
using App.Services;
using App.Utils;
using Game.Cameras;
using Game.Events;
using Game.Menu;
using Game.Weather;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Options
{
    public class OptionsView : MonoBehaviour
    {
        private const string ScreenShotTabId = "screenshot";
        private MenuBarController _menuBarController;
        
        private Slider _dofSlider;
        private Slider _fovSlider;
        private Slider _timeSlider;
        private bool _userSlidingTimeSlider = false;
        private Toggle _captureUIToggle;
        
        private Slider _musicVolumeSlider;
        private Slider _sfxVolumeSlider;

        private bool _captureUi = true;

        private void Start()
        {
            _menuBarController ??= GetComponent<MenuBarController>();
            var audioController = ServiceLocator.Instance.Get<AudioController>();

            var screenshotIcon = Resources.Load<Sprite>("Sprites/screenshot"); 
            var screenshotTab = _menuBarController.RegisterTab(ScreenShotTabId, screenshotIcon);
            
            _dofSlider = screenshotTab.AddSlider("DOF", 0, 0, 1);
            _dofSlider.RegisterValueChangedCallback(HandleDofChanged);
            
            _fovSlider = screenshotTab.AddSlider("FOV", 60, 10, 110);
            _fovSlider.RegisterValueChangedCallback(HandleFovChanged);
            _fovSlider.SetValueWithoutNotify(Camera.main.fieldOfView);
            
            _timeSlider = screenshotTab.AddSlider("Time", 0, 0, 24);
            _timeSlider.RegisterValueChangedCallback(HandleTimeChanged);
            _timeSlider.RegisterCallback<PointerDownEvent>(_ => _userSlidingTimeSlider = true);
            _timeSlider.RegisterCallback<PointerUpEvent>(_ => _userSlidingTimeSlider = false);
            
            _captureUIToggle = screenshotTab.AddNew(new Toggle("Capture UI"));
            _captureUIToggle.value = _captureUi;
            _captureUIToggle.RegisterValueChangedCallback(HandleCaptureUiToggleChanged);
            
            screenshotTab.AddButton("Take Screenshot", TakeScreenshot);
            
            var optionsIcon = Resources.Load<Sprite>("Sprites/options"); 
            var optionsTab = _menuBarController.RegisterTab("options", optionsIcon);

            _musicVolumeSlider = optionsTab.AddSlider("Music Volume", audioController.MusicVolume, 0, 1);
            _musicVolumeSlider.RegisterValueChangedCallback(HandleMusicVolumeChanged);
            
            _sfxVolumeSlider = optionsTab.AddSlider("SFX Volume", audioController.SfxVolume, 0, 1);
            _sfxVolumeSlider.RegisterValueChangedCallback(HandleSfxVolumeChanged);
        }

        private void OnDestroy()
        {
            if (_musicVolumeSlider == null) return;
            
            _dofSlider.UnregisterValueChangedCallback(HandleDofChanged);
            _fovSlider.UnregisterValueChangedCallback(HandleFovChanged);
            _timeSlider.UnregisterValueChangedCallback(HandleTimeChanged);
            _captureUIToggle.UnregisterValueChangedCallback(HandleCaptureUiToggleChanged);
            _musicVolumeSlider.UnregisterValueChangedCallback(HandleMusicVolumeChanged);
            _sfxVolumeSlider.UnregisterValueChangedCallback(HandleSfxVolumeChanged);
        }

        private void OnEnable()
        {
            _menuBarController ??= GetComponent<MenuBarController>();
            _menuBarController.OnTabChange += HandleTabChanged;
        }

        private void OnDisable()
        {
            _menuBarController.OnTabChange -= HandleTabChanged;
        }

        private void LateUpdate()
        {
            if (_userSlidingTimeSlider) return;
            _timeSlider?.SetValueWithoutNotify(LightController.CurrentTime);
        }

        private void HandleDofChanged(ChangeEvent<float> evt)
        {
            EventBus<SetDofEvent>.Raise(new SetDofEvent(1f - evt.newValue));
        }

        private void HandleFovChanged(ChangeEvent<float> evt)
        {
            EventBus<SetFovEvent>.Raise(new SetFovEvent(evt.newValue));
        }

        private void HandleTimeChanged(ChangeEvent<float> evt)
        {
            EventBus<SetTimeEvent>.Raise(new SetTimeEvent(evt.newValue));
        }

        private void HandleCaptureUiToggleChanged(ChangeEvent<bool> evt)
        {
            _captureUi = evt.newValue;
        }

        private void TakeScreenshot()
        {
            ServiceLocator.Instance.Get<ScreenshotController>().TakeScreenshot(_captureUi);
        }

        private static void HandleMusicVolumeChanged(ChangeEvent<float> value)
        {
            EventBus<SetMusicVolume>.Raise(new SetMusicVolume(value.newValue));
        }

        private static void HandleSfxVolumeChanged(ChangeEvent<float> value)
        {
            EventBus<SetSfxVolume>.Raise(new SetSfxVolume(value.newValue));
        }

        private void HandleTabChanged(string tabId)
        {
            var cameraMode = (tabId == ScreenShotTabId) ? CameraMode.Screenshot : CameraMode.Game;
            EventBus<SetCameraModeEvent>.Raise(new SetCameraModeEvent(cameraMode));
        }
    }
}

using App.Services;
using Game.Hexes;
using Game.Menu;
using UnityEngine;

namespace Game.Screensaver
{
    public class ScreensaverView : MonoBehaviour
    {
        [SerializeField] private Transform cameraParent;
        private Screensaver _screensaver;
        private const string TabName = "Screensaver";
        
        private void Start()
        {
            var menuBarController = GetComponent<MenuBarController>();
            menuBarController.OnTabChange += ToggleScreensaver;
            var icon = Resources.Load<Sprite>("Sprites/film");
            menuBarController.RegisterButton(TabName, icon);
        }

        private void ToggleScreensaver(string tabName)
        {
            if (tabName == "Pause" || tabName == "Resume") return;
            
            _screensaver?.Stop();
            _screensaver = null;

            if (tabName != TabName) return;
            var hexController = ServiceLocator.Instance.Get<HexController>();
            _screensaver = new Screensaver(hexController, cameraParent);
        }
    }
}

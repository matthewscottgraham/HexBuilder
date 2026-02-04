using System.Linq;
using App.Events;
using App.Utils;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace Game.Menu
{
    public class MenuView : MonoBehaviour
    {
        private UIDocument _document;
        private Button _exitButton;
        private Button _loadButton;
        private Button _newGameButton;
        private Button _pauseButton;
        private Button _resumeButton;
        
        private EventBinding<GamePauseEvent> _pauseEventBinding;
        private EventBinding<GameResumeEvent> _resumeEventBinding;
        
        private void Start()
        {
            _document = GetComponent<UIDocument>();
            
            var menuBarController = GetComponent<MenuBarController>();
            var menuIcon = Resources.Load<Sprite>("Sprites/menu");
            var tabContent = menuBarController.RegisterTab("menu", menuIcon);
            tabContent.AddToClassList("horizontal");
            
            var pauseIcon = Resources.Load<Sprite>("Sprites/pause");
            _pauseButton = menuBarController.RegisterButton("Pause", pauseIcon, HandlePauseButtonClicked);
            
            var resumeIcon = Resources.Load<Sprite>("Sprites/play");
            _resumeButton = menuBarController.RegisterButton("Resume", resumeIcon, HandleResumeButtonClicked);
            _resumeButton.Hide();

            _newGameButton = tabContent.AddButton("New", HandleNewGameButtonClicked);
            _loadButton = tabContent.AddButton("Load", HandleLoadButtonClicked);
            _exitButton = tabContent.AddButton("", HandleExitButtonClicked);
            _exitButton.AddToClassList("exit-button");
            _exitButton.iconImage = Resources.Load<Sprite>("Sprites/exit")?.texture;
            
            _pauseEventBinding = new EventBinding<GamePauseEvent>(HandlePauseEvent);
            _resumeEventBinding = new EventBinding<GameResumeEvent>(HandleResumeEvent);
            EventBus<GamePauseEvent>.Register(_pauseEventBinding);
            EventBus<GameResumeEvent>.Register(_resumeEventBinding);
        }

        private void OnDestroy()
        {
            if (_loadButton != null) _loadButton.clicked -= HandleLoadButtonClicked;
            if (_newGameButton != null) _newGameButton.clicked -= HandleNewGameButtonClicked;
            if (_exitButton != null) _exitButton.clicked -= HandleExitButtonClicked;
            if (_pauseButton != null) _pauseButton.clicked -= HandlePauseButtonClicked;
            if (_resumeButton != null) _resumeButton.clicked -= HandleResumeButtonClicked;
            
            EventBus<GamePauseEvent>.Deregister(_pauseEventBinding);
            EventBus<GameResumeEvent>.Deregister(_resumeEventBinding);
        }

        private void HandleNewGameButtonClicked()
        {
            _document.rootVisualElement.Add(new SaveGameChooser(true));
        }

        private void HandleLoadButtonClicked()
        {
            _document.rootVisualElement.Add(new SaveGameChooser(false));
        }

        private static void HandleExitButtonClicked()
        {
            EventBus<GameExitEvent>.Raise(new GameExitEvent());
        }

        private static void HandlePauseButtonClicked()
        {
            EventBus<GamePauseEvent>.Raise(new GamePauseEvent());
        }

        private static void HandleResumeButtonClicked()
        {
            EventBus<GameResumeEvent>.Raise(new GameResumeEvent());
        }

        private void HandlePauseEvent()
        {
            _pauseButton.Hide();
            _resumeButton.Show();
        }

        private void HandleResumeEvent()
        {
            _pauseButton.Show();
            _resumeButton.Hide();
        }
    }
}
using App.Events;
using App.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace Game.Menu
{
    public class MenuView : MonoBehaviour
    {
        private UIDocument _document;
        private VisualElement _menuPanel;
        private VisualElement _gamePanel;
        private VisualElement _timePanel;
        private Button _clearBoardButton;
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
            
            _menuPanel = _document.rootVisualElement.Q<VisualElement>("MenuPanel");
            _gamePanel = _menuPanel.AddNew<VisualElement>(new VisualElement(), "game-panel");
            _timePanel = _menuPanel.AddNew<VisualElement>(new VisualElement(), "game-panel");
            
#if UNITY_WEBGL
            _clearBoardButton = _gamePanel.AddButton("Clear", HandleClearBoardButtonCLicked);
#else
            _newGameButton = _gamePanel.AddButton("New", HandleNewGameButtonClicked);
            _loadButton = _gamePanel.AddButton("Load", HandleLoadButtonClicked);
#endif
            _exitButton = _gamePanel.AddButton("Exit", HandleExitButtonClicked);
            _exitButton.AddToClassList("exit-button");
            
            _pauseButton = _timePanel.AddButton("Pause", HandlePauseButtonClicked);
            
            _resumeButton = _timePanel.AddButton("Resume", HandleResumeButtonClicked);
            _resumeButton.visible = false;
            
            _pauseEventBinding = new EventBinding<GamePauseEvent>(HandlePauseEvent);
            _resumeEventBinding = new EventBinding<GameResumeEvent>(HandleResumeEvent);
            EventBus<GamePauseEvent>.Register(_pauseEventBinding);
            EventBus<GameResumeEvent>.Register(_resumeEventBinding);
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (_clearBoardButton != null) _clearBoardButton.clicked -= HandleClearBoardButtonCLicked;
#else
            if (_loadButton != null) _loadButton.clicked -= HandleLoadButtonClicked;
            if (_newGameButton != null) _newGameButton.clicked -= HandleNewGameButtonClicked;
#endif
            
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

        private void HandleClearBoardButtonCLicked()
        {
            EventBus<GameReloadEvent>.Raise(new GameReloadEvent());
        }

        private static void HandleExitButtonClicked()
        {
            EventBus<GameExitEvent>.Raise(new GameExitEvent());
        }

        private void HandlePauseButtonClicked()
        {
            EventBus<GamePauseEvent>.Raise(new GamePauseEvent());
        }

        private void HandleResumeButtonClicked()
        {
            EventBus<GameResumeEvent>.Raise(new GameResumeEvent());
        }

        private void HandlePauseEvent()
        {
            _pauseButton.visible = false;
            _resumeButton.visible = true;
        }

        private void HandleResumeEvent()
        {
            _pauseButton.visible = true;
            _resumeButton.visible = false;
        }
    }
}
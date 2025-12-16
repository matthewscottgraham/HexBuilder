using App.Events;
using UnityEngine;
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

            _loadButton = _document.rootVisualElement.Q<Button>("LoadGameButton");
            _loadButton.clicked += HandleLoadButtonClicked;

            _newGameButton = _document.rootVisualElement.Q<Button>("NewGameButton");
            _newGameButton.clicked += HandleNewGameButtonClicked;

            _exitButton = _document.rootVisualElement.Q<Button>("ExitGameButton");
            _exitButton.clicked += HandleExitButtonClicked;
            
            _pauseButton = _document.rootVisualElement.Q<Button>("PauseButton");
            _pauseButton.clicked += HandlePauseButtonClicked;
            
            _resumeButton = _document.rootVisualElement.Q<Button>("ResumeButton");
            _resumeButton.clicked += HandleResumeButtonClicked;
            _resumeButton.visible = false;
            
            _pauseEventBinding = new EventBinding<GamePauseEvent>(HandlePauseEvent);
            _resumeEventBinding = new EventBinding<GameResumeEvent>(HandleResumeEvent);
            EventBus<GamePauseEvent>.Register(_pauseEventBinding);
            EventBus<GameResumeEvent>.Register(_resumeEventBinding);
        }

        private void OnDestroy()
        {
            if (_loadButton == null) return;
            _loadButton.clicked -= HandleLoadButtonClicked;
            _newGameButton.clicked -= HandleNewGameButtonClicked;
            _exitButton.clicked -= HandleExitButtonClicked;
            _pauseButton.clicked -= HandlePauseButtonClicked;
            _resumeButton.clicked -= HandleResumeButtonClicked;
            
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
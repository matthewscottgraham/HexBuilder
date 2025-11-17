using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Menu
{
    public class MenuView : MonoBehaviour
    {
        private UIDocument _document;
        private Button _loadButton;
        private Button _newGameButton;
        private void Start()
        {
            _document = GetComponent<UIDocument>();
            
            _loadButton = _document.rootVisualElement.Q<Button>("LoadGameButton");
            _loadButton.clicked += HandleLoadButtonClicked;
            
            _newGameButton = _document.rootVisualElement.Q<Button>("NewGameButton");
            _newGameButton.clicked += HandleNewGameButtonClicked;
        }

        private void OnDestroy()
        {
            if (_loadButton == null) return;
            _loadButton.clicked -= HandleLoadButtonClicked;
            _newGameButton.clicked -= HandleNewGameButtonClicked;
        }

        private void HandleNewGameButtonClicked()
        {
            _document.rootVisualElement.Add(new SaveGameChooser(true));
        }

        private void HandleLoadButtonClicked()
        {
            _document.rootVisualElement.Add(new SaveGameChooser(false));
        }
    }
}

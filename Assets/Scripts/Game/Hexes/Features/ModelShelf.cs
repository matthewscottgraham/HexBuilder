using System;
using App.Events;
using App.Services;
using App.Utils;
using Game.Events;
using UnityEngine;
using UnityEngine.UIElements;
using SelectionType = Game.Selection.SelectionType;

namespace Game.Hexes.Features
{
    public class ModelShelf : MonoBehaviour
    {
        [SerializeField] private Sprite randomIcon;
        
        private const string ShelfClassName = "model-shelf";
        private const string ShelfClosedClass = "model-shelf__closed";
        private const string ShelfContentClass = "model-shelf__content";
        private const string ShelfHeaderClass = "model-shelf__header";
        private const string ModelInfoClass = "model-info__container";
        
        private EventBinding<SelectionEvent> _selectionEventBinding;
        private EventBinding<SelectToolEvent> _selectToolEventBinding;

        private FeatureFactory _featureFactory;
        private VisualElement _shelf;
        private Button _toggleShelfButton;
        private Label _shelfLabel;
        private VisualElement _shelfHeader;
        private VisualElement _shelfContent;
        private Button _randomButton;
        private Button[] _modelButtons;
        
        private bool _isShelfOpen = false;
        private bool _useRandom = true;
        private FeatureModelCatalogue _currentCatalogue;
        
        private void OnEnable()
        {
            _selectionEventBinding ??= new EventBinding<SelectionEvent>(HandleSelection);
            EventBus<SelectionEvent>.Register(_selectionEventBinding);
            
            _selectToolEventBinding ??= new EventBinding<SelectToolEvent>(HandleToolSelection);
            EventBus<SelectToolEvent>.Register(_selectToolEventBinding);
        }

        private void OnDisable()
        {
            EventBus<SelectionEvent>.Deregister(_selectionEventBinding);
            EventBus<SelectToolEvent>.Deregister(_selectToolEventBinding);
        }

        private void Start()
        {
            _featureFactory = ServiceLocator.Instance.Get<FeatureFactory>();
            
            var uiDocument = GetComponent<UIDocument>();
            _shelf = uiDocument.rootVisualElement.AddNew<VisualElement>(new VisualElement(), ShelfClassName);
            _shelfHeader = _shelf.AddNew<VisualElement>(new VisualElement(), ShelfHeaderClass);
            _shelfLabel = _shelfHeader.AddNew(new Label());
            _toggleShelfButton = _shelfHeader.AddNew(new Button(ToggleShelf));
            _toggleShelfButton.text = "Toggle Shelf";
            _shelfContent = _shelf.AddNew<VisualElement>(new VisualElement(), ShelfContentClass);
            
            CloseShelf();
        }

        private void OpenShelf()
        {
            _isShelfOpen = true;
            UpdateShelfVisibility();
        }

        private void CloseShelf()
        {
            _isShelfOpen = false;
            UpdateShelfVisibility();
        }
        private void ToggleShelf()
        {
            _isShelfOpen = !_isShelfOpen;
            UpdateShelfVisibility();
        }

        private void UpdateShelfVisibility()
        {
            if (_isShelfOpen)
                _shelfContent.RemoveFromClassList(ShelfClosedClass);
            else 
                _shelfContent.AddToClassList(ShelfClosedClass);
            SetActiveButtonClass();
        }

        private void DisplayModels(string categoryName, FeatureModelCatalogue catalogue)
        {
            _currentCatalogue = catalogue;
            if (_currentCatalogue == null || _currentCatalogue.Count == 0)
            {
                LockShelf();
                return;
            }

            UnlockShelf();
            _shelfContent.Clear();
            _shelfLabel.text = categoryName;
            
            _randomButton = _shelfContent.AddNew(
                CreateModelButton(ChooseRandomFromCategory, randomIcon.texture));
            
            _modelButtons = new Button[_currentCatalogue.Count];
            for (var i = 0; i < _currentCatalogue.Count; i++)
            {
                var index = i;
                var featurePrefab = _currentCatalogue.GetPrefab(index);
                _modelButtons[i] = _shelfContent.AddNew(
                    CreateModelButton(() => SetModelToCreate(index), featurePrefab.Icon));
            }
            SetActiveButtonClass();
        }

        private static Button CreateModelButton(Action onClickAction, Texture2D icon)
        {
            var button = new Button(onClickAction);
            button.AddToClassList(ModelInfoClass);
            button.iconImage = icon;
            return button;
        }

        private void LockShelf()
        {
            CloseShelf();
            _toggleShelfButton.Hide();
        }

        private void UnlockShelf()
        {
            OpenShelf();
            _toggleShelfButton.Show();
        }

        private void ChooseRandomFromCategory()
        {
            _useRandom = true;
            _featureFactory.CurrentVariation = -1;
            SetActiveButtonClass();
        }

        private void SetModelToCreate(int index)
        {
            _useRandom = false;
            _featureFactory.CurrentVariation = index;
            SetActiveButtonClass();
        }

        private void SetActiveButtonClass()
        {
            if (_randomButton == null) return;
            if (_useRandom) _randomButton.AddToClassList("checked");
            else _randomButton.RemoveFromClassList("checked");

            for (var i = 0; i < _modelButtons.Length; i++)
            {
                if (_featureFactory.CurrentVariation == i)
                    _modelButtons[i].AddToClassList("checked");
                else _modelButtons[i].RemoveFromClassList("checked");
            }
        }

        private void HandleSelection(SelectionEvent selectionEvent)
        {
            if (_useRandom) ChooseRandomFromCategory();
        }

        private void HandleToolSelection(SelectToolEvent selectToolEventEvent)
        {
            if (selectToolEventEvent.Tool.SelectionType != SelectionType.Face) return;
            _featureFactory ??= ServiceLocator.Instance.Get<FeatureFactory>();
            
            
            var catalogue = _featureFactory.GetCatalogue(selectToolEventEvent.Tool.FeatureType);
            if (catalogue == null)
            {
                LockShelf();
                return;
            }
            DisplayModels(catalogue.FeatureType.ToString(), catalogue);
            ChooseRandomFromCategory();
        }
    }
}

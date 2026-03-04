using System;
using App.Events;
using App.Services;
using App.Utils;
using Game.Events;
using Game.Hexes.Features;
using UnityEngine;
using UnityEngine.UIElements;
using SelectionType = Game.Selection.SelectionType;

namespace Game.Tools.UI
{
    public class ModelShelf : MonoBehaviour
    {
        [SerializeField] private Sprite randomIcon;
        
        private const string ShelfClosedClass = "model-shelf__closed";
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
            
            var toolMenuController = GetComponent<ToolMenuController>();
            _shelfContent = toolMenuController.ModelsContainer;
        }

        private void UpdateShelfVisibility(bool isOpen)
        {
            if (isOpen)
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
                UpdateShelfVisibility(false);
                return;
            }

            UpdateShelfVisibility(true);
            _shelfContent.Clear();
            
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
            UpdateShelfVisibility(false);
            if (selectToolEventEvent.Tool.SelectionType != SelectionType.Face) return;
            _featureFactory ??= ServiceLocator.Instance.Get<FeatureFactory>();
            
            
            var catalogue = _featureFactory.GetCatalogue(selectToolEventEvent.Tool.FeatureType);
            if (catalogue == null) return;
            DisplayModels(catalogue.FeatureType.ToString(), catalogue);
            ChooseRandomFromCategory();
        }
    }
}

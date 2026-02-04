using System;
using System.Collections.Generic;
using App.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Menu
{
    public class MenuBarController : MonoBehaviour
    {
        private VisualElement _headerContainer;
        private VisualElement _headerTabButtons;
        private VisualElement _headerCustomElements;
        private VisualElement _contentContainer;

        private readonly Dictionary<string, Button> _tabButtons = new();
        private readonly Dictionary<string, VisualElement> _tabContents = new();

        private string _currentActiveTab;
        
        public VisualElement RegisterTab(string tabId, Sprite sprite)
        {
            RegisterButton(tabId, sprite);
            
            var tabContent = _contentContainer.AddNew(new VisualElement());
            tabContent.AddToClassList("hidden", "menu-bar-content");
            _tabContents.Add(tabId, tabContent);

            return tabContent;
        }

        public Button RegisterButton(string tabId, Sprite sprite, Action action = null)
        {
            if (_tabButtons.ContainsKey(tabId))
            {
                Debug.LogError($"Tab with id {tabId} already registered");
                return null;
            }

            var tabButton = _headerTabButtons.AddButton("", () => ShowTab(tabId));
            if (action != null) tabButton.clicked += action;
            
            tabButton.AddToClassList("menu-bar__tab-button");
            tabButton.iconImage = sprite?.texture;
            _tabButtons.Add(tabId, tabButton);
            
            return tabButton;
        }

        public T RegisterCustomElement<T>(VisualElement visualElement) where T : VisualElement
        {
            _headerCustomElements.Add(visualElement);
            return visualElement as T;
        }

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;
            var mainContainer = root.AddNew<VisualElement>(new VisualElement(), "menu-bar");
            _headerContainer = mainContainer.AddNew<VisualElement>(new VisualElement(), "menu-bar-header");
            _headerTabButtons = _headerContainer.AddNew<VisualElement>(new VisualElement(), "horizontal");
            _headerContainer.AddSpacer();
            _headerCustomElements = _headerContainer.AddNew<VisualElement>(new VisualElement(), "horizontal");
            
            _contentContainer = mainContainer.AddNew(new VisualElement());
            _contentContainer.pickingMode = PickingMode.Ignore;
        }

        private void ShowTab(string tabId)
        {
            if (_currentActiveTab == tabId) tabId = null;
            
            _currentActiveTab = tabId;
            
            foreach (var tab in _tabButtons)
            {
                if (tab.Key == tabId) tab.Value.AddToClassList("active-tab");
                else tab.Value.RemoveFromClassList("active-tab");
            }

            foreach (var tab in _tabContents)
            {
                if (tab.Key != tabId) tab.Value.Hide();
                else tab.Value.Show();
            }
        }
    }
}

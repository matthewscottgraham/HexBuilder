using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace App.Utils
{
    public static class UIUtils
    {
        public static VisualElement AddSpacer(this VisualElement parentElement)
        {
            return parentElement.AddNew<VisualElement>(new VisualElement(), "spacer");
        }

        public static Button AddButton(this VisualElement parentElement, string text, Action onClick)
        {
            var button = parentElement.AddNew(new Button());
            button.text = text;
            button.clicked += onClick;
            return button;
        }
        public static Button AddButton(this VisualElement parentElement, Texture2D icon, Action onClick)
        {
            var button = parentElement.AddNew<Button>(new Button(), "button-icon");
            button.iconImage = icon;
            button.clicked += onClick;
            return button;
        }

        public static T AddNew<T>(this VisualElement parentElement, T childElement) where T : VisualElement
        {
            parentElement.Add(childElement);
            return childElement;
        }

        public static T AddNew<T>(this VisualElement parentElement, VisualElement childElement, string className)
            where T : VisualElement
        {
            var child = parentElement.AddNew(childElement);
            child.AddToClassList(className);
            return (T)child;
        }

        public static void AddToClassList(this VisualElement element, string classNameA, string classNameB)
        {
            element.AddToClassList(classNameA);
            element.AddToClassList(classNameB);
        }

        public static void Hide(this VisualElement element, bool collapse = true)
        {
            var className = collapse ? "hidden-and-collapsed" : "hidden";
            element.AddToClassList(className);
        }

        public static void Show(this VisualElement element)
        {
            element.RemoveFromClassList("hidden-and-collapsed");
            element.RemoveFromClassList("hidden");
        }

        public static void SetVisibility(this VisualElement element, bool isVisible, bool collapse = true)
        {
            if (isVisible) element.Show();
            else element.Hide(collapse);
        }

        public static bool ClassListContainsAny(this VisualElement element, string[] classNames)
        {
            if (classNames == null || classNames.Length == 0) return false;
            foreach (var className in classNames)
            {
                if (element.ClassListContains(className)) return true;
            }
            return false;
        }
    }
}
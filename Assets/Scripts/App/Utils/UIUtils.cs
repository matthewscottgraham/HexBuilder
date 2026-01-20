using System;
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
    }
}
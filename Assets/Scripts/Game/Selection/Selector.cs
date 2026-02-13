using System;
using System.Collections;
using App.Audio;
using App.Events;
using App.Input;
using App.Services;
using Game.Events;
using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class Selector : MonoBehaviour, IDisposable
    {
        private const string HoverSoundID = "Audio/SFX/hover";
        
        private Camera _camera;
        private EventBinding<InteractEvent> _interactEventBinding;
        private bool _isActive;
        protected static readonly SelectionContext BlankSelection = new (SelectionType.None);
        
        public static SelectionContext Hovered { get; private set; } = BlankSelection;
        public virtual SelectionType SelectionType => SelectionType.None;
        
        public void Activate(bool isActive)
        {
            _isActive = isActive;
        }
        
        private void Update()
        {
            if (!_isActive) return;
            if (!InputController.PointerHasMovedThisFrame) return;
            Hover();
        }

        public void Initialize(HexController hexController)
        {
            _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            EventBus<InteractEvent>.Register(_interactEventBinding);

            _camera = Camera.main;
            
            ServiceLocator.Instance.Get<AudioController>().RegisterSound(HoverSoundID, Resources.Load<AudioClip>(HoverSoundID));
        }

        public void Dispose()
        {
            EventBus<InteractEvent>.Deregister(_interactEventBinding);
            _interactEventBinding = null;
        }
        
        protected virtual SelectionContext GetClampedSelection(HexObject hexObject, Vector3 pos)
        {
            return new SelectionContext();
        }

        private void Hover()
        {
            var ray = _camera.ScreenPointToRay(InputController.PointerPosition);
            if (!Physics.Raycast(ray, out var hit)) return;
            SetHoveredSelection(hit.transform.GetComponentInParent<HexObject>(), hit.point);
        }

        private void SetHoveredSelection(HexObject hexObject, Vector3 hoverPosition)
        {
            if (!hexObject) return;
            var originalHover = Hovered;
            var newHover = GetClampedSelection(hexObject, hoverPosition);
            if (originalHover.Equals(newHover)) return;
            Hovered = newHover;
            EventBus<HoverEvent>.Raise(new HoverEvent(newHover));
            EventBus<PlaySoundEvent>.Raise(new PlaySoundEvent(HoverSoundID, true));
        }

        private static IEnumerator InvokeSelectionEvent()
        {
            yield return new WaitForEndOfFrame();
            EventBus<SelectionEvent>.Raise(new SelectionEvent(Hovered));
        }

        private void HandleInteractEvent(InteractEvent interactEvent)
        {
            if (!_isActive) return;
            StartCoroutine(InvokeSelectionEvent());
        }
    }
}
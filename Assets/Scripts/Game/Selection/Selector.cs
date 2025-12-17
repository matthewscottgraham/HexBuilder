using System;
using System.Collections;
using App.Audio;
using App.Events;
using App.Input;
using App.Services;
using App.Utils;
using Game.Events;
using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class Selector : MonoBehaviour, IDisposable
    {
        protected HexController HexController;
        protected Transform Highlighter;
        
        private const string HoverSoundID = "Audio/SFX/hover";
        
        private Transform _guide;
        private Camera _camera;
        private EventBinding<InteractEvent> _interactEventBinding;
        private EventBinding<MoveEvent> _moveEventBinding;
        private bool _isActive;
        protected static readonly SelectionContext BlankSelection = new (SelectionType.None, null, null, null);
        
        public static SelectionContext Hovered { get; private set; } = BlankSelection;
        public virtual SelectionType SelectionType => SelectionType.None;
        private static readonly Vector3 GuideOffset = new Vector3(0, 0.1f, 0);
        
        public void Activate(bool isActive)
        {
            _isActive = isActive;
            Highlighter.gameObject.SetActive(_isActive);
            _guide.gameObject.SetActive(_isActive);
        }
        
        private void Update()
        {
            if (!_isActive) return;
            if (!InputController.PointerHasMovedThisFrame) return;
            Hover();
        }

        public void Initialize(HexController hexController)
        {
            HexController = hexController;
            
            _moveEventBinding = new EventBinding<MoveEvent>(HandleMoveEvent);
            _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            EventBus<MoveEvent>.Register(_moveEventBinding);
            EventBus<InteractEvent>.Register(_interactEventBinding);

            _camera = Camera.main;
            Highlighter = CreateHighlighter();
            _guide = CreateGuide();
            
            ServiceLocator.Instance.Get<AudioController>().RegisterSound(HoverSoundID, Resources.Load<AudioClip>(HoverSoundID));
        }

        public void Dispose()
        {
            EventBus<MoveEvent>.Deregister(_moveEventBinding);
            EventBus<InteractEvent>.Deregister(_interactEventBinding);
            _moveEventBinding = null;
            _interactEventBinding = null;
        }

        protected virtual Transform CreateHighlighter()
        {
            return null;
        }

        protected virtual Transform CreateGuide()
        {
            var guide = gameObject.AddChild<SpriteRenderer>("FaceGuide");
            guide.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            guide.transform.localScale = Vector3.one * 1.7f;
            guide.sprite = Resources.Load<Sprite>("Sprites/FaceGuide");
            guide.color = Color.coral;
            return guide.transform;
        }

        protected virtual SelectionContext GetClampedSelection(Vector3 pos)
        {
            return new SelectionContext();
        }
        
        protected virtual SelectionContext GetClampedSelection(HexObject hexObject, Vector3 pos)
        {
            return new SelectionContext();
        }

        protected virtual void SetHoverRotation(HexObject hexObject)
        {
            Highlighter.LookAt(hexObject.Face.Position);
        }

        private void Hover()
        {
            var ray = _camera.ScreenPointToRay(InputController.PointerPosition);
            if (!Physics.Raycast(ray, out var hit)) return;
            if (!SetHoveredSelection(hit.transform.GetComponentInParent<HexObject>(), hit.point))
            {
                SetHoveredSelection(hit.point);
            }
        }

        private bool SetHoveredSelection(HexObject hexObject, Vector3 hoverPosition)
        {
            if (!hexObject) return false;
            _guide.transform.position = hexObject.Face.Position + GuideOffset;
            var originalHover = Hovered;
            var newHover = GetClampedSelection(hexObject, hoverPosition);
            if (originalHover.Equals(newHover)) return true;
            Hovered = newHover;
            Highlighter.position = newHover.Position;
            SetHoverRotation(hexObject);
            EventBus<HoverEvent>.Raise(new HoverEvent(newHover));
            EventBus<PlaySoundEvent>.Raise(new PlaySoundEvent(HoverSoundID, true));
            return true;
        }
        
        private void SetHoveredSelection(Vector3 hoverPosition)
        {
            var originalHover = Hovered;
            var newHover = GetClampedSelection(hoverPosition);
            if (originalHover.Equals(newHover)) return;
            Hovered = newHover;
            Highlighter.position = newHover.Position;
            EventBus<HoverEvent>.Raise(new HoverEvent(newHover));
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

        private void HandleMoveEvent(MoveEvent moveEvent)
        {
            if (!_isActive) return;
            var worldPosition = transform.position;
            if (moveEvent.Delta.y != 0)
            {
                var nudge = moveEvent.Delta.y > 0 ? 1 : -1;
                worldPosition += new Vector3( moveEvent.Delta.x * 2 + nudge, 0, transform.position.z + moveEvent.Delta.y * 1.5f);
            }
            else
            {
                worldPosition = new Vector3(moveEvent.Delta.x * 2, 0, moveEvent.Delta.y * 1.5f);
            }

            SetHoveredSelection(worldPosition);
        }
    }
}
using System;
using System.Collections.Generic;
using App.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace App.Input
{
    public class InputController : MonoBehaviour, IDisposable
    {
        private EventSystem _eventSystem;
        private InputSystem_Actions _inputSystem;
        
        private const float DragThreshold = 4f;
        
        private Vector2 _clickStartPosition;
        private bool _clicking;
        private bool _rotating;
        private bool _moving;
        
        public static bool IsDragging { get; private set; }
        public static bool PointerIsOverUI { get; private set; }
        public static bool PointerHasMovedThisFrame { get; private set; }
        public static Vector2 PointerPosition => Pointer.current.position.ReadValue();
        private static Vector2 LastMousePosition { get; set; }

        public void Initialize()
        {
            _eventSystem = EventSystem.current;
            _inputSystem = new InputSystem_Actions();
            _inputSystem.Enable();

            _inputSystem.Player.Move.started += HandleMoveStart;
            _inputSystem.Player.Move.canceled += HandleMoveEnd;
            _inputSystem.Player.Interact.started += HandleInteractStart;
            _inputSystem.Player.Interact.canceled += HandleInteractEnd;
            _inputSystem.Player.Zoom.performed += HandleZoom;
            _inputSystem.Player.Focus.performed += HandleFocus;
            _inputSystem.Player.Camera.performed += HandleCameraStart;
            _inputSystem.Player.Camera.canceled += HandleCameraEnd;
            
            LastMousePosition = PointerPosition;
        }

        public void Dispose()
        {
            _inputSystem.Player.Move.started -= HandleMoveStart;
            _inputSystem.Player.Move.canceled -= HandleMoveEnd;
            _inputSystem.Player.Interact.started -= HandleInteractStart;
            _inputSystem.Player.Interact.canceled -= HandleInteractEnd;
            _inputSystem.Player.Zoom.performed -= HandleZoom;
            _inputSystem.Player.Focus.performed -= HandleFocus;
            _inputSystem.Player.Camera.performed -= HandleCameraStart;
            _inputSystem.Player.Camera.canceled -= HandleCameraEnd;
            _inputSystem?.Dispose();
        }

        private void Update()
        {
            if (_moving) EventBus<MoveEvent>.Raise(new MoveEvent(_inputSystem.Player.Move.ReadValue<Vector2>()));

            PointerIsOverUI = IsPointerOverUI();
            
            if (_clicking || _rotating)
            {
                if (PointerIsOverUI)
                {
                    CancelClick();
                    return;
                }
                if (Vector2.Distance(_clickStartPosition, PointerPosition) < DragThreshold) return;
                
                IsDragging = true;
                var delta = LastMousePosition - PointerPosition;
                if (_rotating) EventBus<RotateEvent>.Raise(new RotateEvent(delta.normalized));
                else EventBus<DragEvent>.Raise(new DragEvent(delta.normalized));
            }

            if (PointerIsOverUI) return;
            PointerHasMovedThisFrame = Vector2.Distance(LastMousePosition, PointerPosition) > Mathf.Epsilon;
            LastMousePosition = PointerPosition;
        }

        private bool IsPointerOverUI()
        {
            var pointerEventData = new PointerEventData(_eventSystem)
            {
                position = Pointer.current.position.value
            };
            var results = new List<RaycastResult>();
            _eventSystem.RaycastAll(pointerEventData, results);
            return results.Count > 0;
        }

        private void HandleMoveStart(InputAction.CallbackContext ctx)
        {
            _moving = true;
        }

        private void HandleMoveEnd(InputAction.CallbackContext ctx) => _moving = false;

        private static void HandleZoom(InputAction.CallbackContext ctx)
        {
            EventBus<ZoomEvent>.Raise(new ZoomEvent(ctx.ReadValue<float>()));
        }

        private void HandleInteractStart(InputAction.CallbackContext ctx)
        {
            _clicking = true;
            _clickStartPosition = PointerPosition;
        }

        private void HandleInteractEnd(InputAction.CallbackContext ctx)
        {
            if (_clicking && !IsDragging)
            {
                EventBus<InteractEvent>.Raise(new InteractEvent());
            }
            CancelClick();
        }

        private void CancelClick()
        {
            IsDragging = false;
            _clicking = false;
        }

        private void HandleCameraStart(InputAction.CallbackContext ctx)
        {
            _rotating = true;
        }

        private void HandleCameraEnd(InputAction.CallbackContext ctx)
        {
            _rotating = false;
        }

        private static void HandleFocus(InputAction.CallbackContext ctx)
        {
            EventBus<FocusEvent>.Raise(new FocusEvent());
        }
    }
}
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
        private bool _wasDragged;
        private bool _moving;
        
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
            
            LastMousePosition = PointerPosition;
        }

        public void Dispose()
        {
            _inputSystem.Player.Move.started -= HandleMoveStart;
            _inputSystem.Player.Move.canceled -= HandleMoveEnd;
            _inputSystem.Player.Interact.started -= HandleInteractStart;
            _inputSystem.Player.Interact.canceled -= HandleInteractEnd;
            _inputSystem.Player.Zoom.performed -= HandleZoom;
            _inputSystem?.Dispose();
        }

        private void Update()
        {
            if (_moving) EventBus<MoveEvent>.Raise(new MoveEvent(_inputSystem.Player.Move.ReadValue<Vector2>()));

            var pointerOverUi = IsPointerOverUI();
            
            if (_clicking)
            {
                if (pointerOverUi)
                {
                    CancelClick();
                    return;
                }
                if (Vector2.Distance(_clickStartPosition, PointerPosition) > DragThreshold)
                {
                    _wasDragged = true;
                    var delta = LastMousePosition - PointerPosition;
                    EventBus<DragEvent>.Raise(new DragEvent(delta.normalized));
                }
            }

            if (pointerOverUi) return;
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
            if (_clicking && !_wasDragged)
            {
                EventBus<InteractEvent>.Raise(new InteractEvent());
            }
            CancelClick();
        }

        private void CancelClick()
        {
            _wasDragged = false;
            _clicking = false;
        }
    }
}
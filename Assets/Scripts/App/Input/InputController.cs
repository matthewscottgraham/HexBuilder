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

        public static bool PointerHasMovedThisFrame { get; private set; }
        public static Vector2 PointerPosition => Mouse.current.position.ReadValue();
        private static Vector2 LastMousePosition { get; set; }

        private void Update()
        {
            PointerHasMovedThisFrame = Vector2.Distance(LastMousePosition, PointerPosition) > Mathf.Epsilon;
            LastMousePosition = PointerPosition;
        }

        public void Dispose()
        {
            _inputSystem.Player.Move.performed -= HandleMove;
            _inputSystem.Player.Interact.performed -= HandleInteract;
            _inputSystem?.Dispose();
        }

        public void Initialize()
        {
            _eventSystem = EventSystem.current;
            _inputSystem = new InputSystem_Actions();
            _inputSystem.Enable();

            _inputSystem.Player.Move.performed += HandleMove;
            _inputSystem.Player.Interact.started += HandleInteract;
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

        private static void HandleMove(InputAction.CallbackContext ctx)
        {
            EventBus<MoveEvent>.Raise(new MoveEvent(ctx.ReadValue<Vector2>()));
        }

        private void HandleInteract(InputAction.CallbackContext ctx)
        {
            if (IsPointerOverUI()) return;
            EventBus<InteractEvent>.Raise(new InteractEvent());
        }
    }
}